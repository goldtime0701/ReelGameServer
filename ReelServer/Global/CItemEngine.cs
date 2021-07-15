using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CItemEngine
    {
        public int[] m_nItemCash = { 0, 0, 0 };         //쌓인금액
        public int[] m_nRaiseCash = { 0, 0, 0 };       //발동금액
        public int[] m_nJackCash = { 0, 0, 0 };        //잭팟금액
        public int m_nRemCash;          //잭팟주고 남은금액 다음번잭팟에 이어서 주어야 한다.


        public int UseItem(CItem item)
        {
            int nRetCash = 0;
            bool bJack = false;

            for(int i=0; i<3; i++)
            {
                if (m_nJackCash[i] == 0)
                {
                    CalcJackCash(i);
                }

                int nCash = 0;
                if(i == 0)
                    nCash = Convert.ToInt32(item.m_nBuyPrice * 20 / 100);
                else if(i == 1)
                    nCash = Convert.ToInt32(item.m_nBuyPrice * 30 / 100);
                else if(i == 2)
                    nCash = Convert.ToInt32(item.m_nBuyPrice * 50 / 100);

                m_nItemCash[i] += nCash;
                if (m_nItemCash[i] >= m_nRaiseCash[i])
                {
                    //잭팟을 주어야 한다.
                    nRetCash += m_nJackCash[i];
                    m_nItemCash[i] -= m_nRaiseCash[i];
                    CalcJackCash(i);
                    bJack = true;
                }
                else
                {
                    if(!bJack)
                    {
                        CItemModel model = CGlobal.GetItemModelByCode(item.m_nItemModel);
                        int nRet = Convert.ToInt32(nCash * model.m_nLosePro / 100);
                        nRetCash += nRet;
                        m_nItemCash[i] -= nRet;
                    }
                }
            }
            if(bJack == false)
            {
                nRetCash += m_nRemCash;
                m_nRemCash = 0;
            }

            CDataBase.SaveItemEngine(this);

            return nRetCash;
        }

        private void CalcJackCash(int nStep)
        {
            if (nStep == 0)
            {
                m_nJackCash[nStep] = CGlobal._RND.Next(2, 11) * 10000;
            }
            else if (nStep == 1)
            {
                m_nJackCash[nStep] = CGlobal._RND.Next(10, 51) * 10000;
            }
            else if (nStep == 2)
            {
                int nRnd = CGlobal._RND.Next(1, 3);
                m_nJackCash[nStep] = CGlobal._RND.Next(50, 50 * nRnd + 1) * 10000;
            }
                

            m_nRaiseCash[nStep] = Convert.ToInt32(m_nJackCash[nStep] * 1.2);
            m_nItemCash[nStep] = 0;
        }

        public void SetRemCash(int nCash)
        {
            m_nRemCash = nCash;
        }

        public void SetItemJackCash(int nCode, int nCash)
        {
            m_nJackCash[nCode - 1] = nCash;
            CDataBase.SaveItemEngine(this);
        }

        public void SetItemUseCash(int nCode, int nCash)
        {
            m_nItemCash[nCode - 1] = nCash;
            CDataBase.SaveItemEngine(this);
        }

        public void SetItemRaiseCash(int nCode, int nCash)
        {
            m_nRaiseCash[nCode - 1] = nCash;
            CDataBase.SaveItemEngine(this);
        }
    }
}
