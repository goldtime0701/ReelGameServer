using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public class CGameEngine : CGameModel
    {
        public int m_nGivePrizeCount = 0;                       //가짜불상을주기위한 첨수, 잭팟 5번당한번은 가짜불상을준다.
        public int m_nEmptyPrize = 10;                           //가짜잭팟이 몇번만에 나오는가 하는 상수

        private bool m_bStart;                                  //서버가 시작해서 디비에서 자료를 불러들일때 이미 지나간 환수률을 없애기 위한 변수
        
        public CTotalPot m_clsTotalPot;                         //실지머니발동금액
        public CTotalPot m_clsVirtualPot;                       //체험머니발동금액
        public CGivePot m_clsGivePot;                           //유저에게 돌려준 금액상황을 표시한다.

        public List<CTotalStep> m_lstTotalStep1;                //1차잭팟금액발동단계금액리스트
        public List<CTotalStep> m_lstTotalStep2;                //2차잭팟금액발동단계금액리스트
        public List<CTotalStep> m_lstTotalStep3;                //3차잭팟금액발동단계금액리스트

        public List<CTotalStep> m_lstVirtualStep1;              //체험머니 1차잭팟금액발동단계금액리스트
        public List<CTotalStep> m_lstVirtualStep2;              //체험머니 2차잭팟금액발동단계금액리스트
        public List<CTotalStep> m_lstVirtualStep3;              //체험머니 3차잭팟금액발동단계금액리스트

        protected bool m_bJackpot1 = true;
        protected bool m_bJackpot2 = true;
        protected bool m_bJackpot3 = true;

        protected bool m_bVJackpot1 = true;
        protected bool m_bVJackpot2 = true;
        protected bool m_bVJackpot3 = true;

        public CGameEngine(int nGameCode)
        {
            m_nGameCode = nGameCode;

            m_clsTotalPot = new CTotalPot(m_nGameCode);
            m_clsVirtualPot = new CTotalPot(m_nGameCode);
            m_clsGivePot = new CGivePot(m_nGameCode);

            m_lstTotalStep1 = new List<CTotalStep>();
            m_lstTotalStep2 = new List<CTotalStep>();
            m_lstTotalStep3 = new List<CTotalStep>();
            m_lstVirtualStep1 = new List<CTotalStep>();
            m_lstVirtualStep2 = new List<CTotalStep>();
            m_lstVirtualStep3 = new List<CTotalStep>();

            m_bStart = true;
        }

        public void AddTotalPot(int nAddCash, int nUserCode)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user == null)
                return;
            int nLevel = CGlobal.GetLevelByUserCode(user.m_nUserCode);
            if (user.m_nChargeCnt == 0 || user.m_nVirtualCash > 0 || user.m_nVirtualCupn > 0)
            {
                m_clsVirtualPot.m_nFirst += nAddCash;
            }
            else
            {
                if (nLevel > 10)
                {
                    //테스트총판
                    if (user.m_nUserBonusCash > 0)
                    {
                        user.m_nUserBonusCash -= nAddCash;
                    }
                }
                else
                {
                    if (user.m_nUserBonusCash > 0)
                    {
                        if (CGlobal._RND.Next(0, 2) == 1)
                        {
                            user.m_nUserBonusCash -= nAddCash;
                        }
                        else
                        {
                            m_clsTotalPot.m_nFirst += nAddCash;
                            //user.m_nUserUseCash += nAddCash;
                        }
                    }
                    else
                    {
                        m_clsTotalPot.m_nFirst += nAddCash;
                        //user.m_nUserUseCash += nAddCash;
                    }
                }
            }

            CDataBase.SaveUserInfoToDB(user);
        }

        public void AddGivePot(int nGiveCash, int nStep, int nUserCode)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user == null)
                return;
            int nLevel = CGlobal.GetLevelByUserCode(user.m_nUserCode);
            if (nLevel < 10)
            {
                if (user.m_nChargeCnt == 0 || user.m_nVirtualCash > 0 || user.m_nVirtualCupn > 0)
                {
                    return;
                }

                if (nStep == 0)
                {
                    m_clsGivePot.m_nZero -= nGiveCash;
                    if (m_clsGivePot.m_nZero < 0)
                        m_clsGivePot.m_nZero = 0;
                }
                else if(nGiveCash > 0)
                {
                    if (nStep == 1)
                        m_clsGivePot.m_nFirst -= nGiveCash;
                    else if (nStep == 2)
                        m_clsGivePot.m_nSecond -= nGiveCash;
                    else if (nStep == 3)
                        m_clsGivePot.m_nThird -= nGiveCash;
                }

                m_clsGivePot.m_nGiveCash += nGiveCash;
            }
        }

        public void AddAppendPot(int nAppendCash, int nUserCode)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user == null)
                return;
            int nLevel = CGlobal.GetLevelByUserCode(user.m_nUserCode);
            if (user.m_nChargeCnt == 0 || user.m_nVirtualCash > 0 || user.m_nVirtualCupn > 0)
            {
                return;
            }
            else
            {
                if (nLevel > 10)
                {
                    //테스트총판
                    return;
                }
                else
                {
                    m_clsGivePot.m_nAppenCash += nAppendCash;
                    int nDstCash = nAppendCash * m_nGameRate / 100;
                    m_clsGivePot.m_nZero += nDstCash;
                }
            }
        }

        //강제잭팟을 주었을때 자연잭팟단계에서 강제잭팟을 준 금액에 해당한 자연잭팟단계를 삭제하여야 한다.
        public virtual void RemoveNotionalJackStep(int nJackCash, int nUserCode)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user == null)
                return;
            
            int nLevel = CGlobal.GetLevelByUserCode(user.m_nUserCode);

            if (nLevel > 10)
                return;  //테스트총판이라면 리턴
            if (user.m_nChargeCnt == 0  || user.m_nVirtualCash > 0 || user.m_nVirtualCupn > 0)
                return;
           

            int nTempCash = nJackCash + m_clsGivePot.m_nOnerPrizeCall;
            while (true)
            {
                //2차, 3차를 합한곳에서 해당한 금액에 제일 가까운 잭팟을 선택하여 삭제한다.
                List<CTotalStep> lstTotalStep2 = m_lstTotalStep2.FindAll(value => value.m_nPrizeCash <= nTempCash).OrderByDescending(value => value.m_nPrizeCash).ToList();
                List<CTotalStep> lstTotalStep3 = m_lstTotalStep3.FindAll(value => value.m_nPrizeCash <= nTempCash).OrderByDescending(value => value.m_nPrizeCash).ToList();

                if (lstTotalStep2 == null && lstTotalStep3 == null)
                {
                    break;
                }

                if (lstTotalStep2.Count == 0 && lstTotalStep3.Count == 0)
                {
                    break;
                }

                if(m_nAutoPrize == 1 && lstTotalStep3 == null)
                {
                    break;
                }

                if (m_nAutoPrize == 1 && lstTotalStep3.Count == 0)
                {
                    break;
                }

                if (lstTotalStep3.Count > 0)
                {
                    CTotalStep totalStep = lstTotalStep3[0];
                    m_lstTotalStep3.Remove(totalStep);
                    m_clsGivePot.m_nThird -= totalStep.m_nPrizeCash;

                    nTempCash -= totalStep.m_nPrizeCash;
                }
                else if(lstTotalStep2.Count > 0)
                {
                    if(m_nAutoPrize == 0)
                    {
                        CTotalStep totalStep = lstTotalStep2[0];
                        m_lstTotalStep2.Remove(totalStep);
                        m_clsGivePot.m_nSecond -= totalStep.m_nPrizeCash;

                        nTempCash -= totalStep.m_nPrizeCash;
                    }
                    else
                    {
                        break;
                    }
                    
                }
            }

            m_clsGivePot.m_nOnerPrizeCall = nTempCash;
        }

        public void ResetTotalCash()
        {
            m_clsTotalPot.m_nFirst = 0;
            m_clsTotalPot.m_nSecond = 0;
            m_clsTotalPot.m_nThird = 0;

            m_clsVirtualPot.m_nFirst = 0;
            m_clsVirtualPot.m_nSecond = 0;
            m_clsVirtualPot.m_nThird = 0;

            m_lstTotalStep1.Clear();
            m_lstTotalStep2.Clear();
            m_lstTotalStep3.Clear();

            m_lstVirtualStep1.Clear();
            m_lstVirtualStep2.Clear();
            m_lstVirtualStep3.Clear();

            m_bJackpot1 = true;
            m_bJackpot2 = true;
            m_bJackpot3 = true;

            m_bVJackpot1 = true;
            m_bVJackpot2 = true;
            m_bVJackpot3 = true;
        }

        public void ResetTotalJackCash()
        {
            m_clsGivePot.m_nAppenCash = 0;
            m_clsGivePot.m_nGiveCash = 0;
            m_clsGivePot.m_nRealCash = 0;

            m_clsGivePot.m_nZero = 0;
            m_clsGivePot.m_nFirst = 0;
            m_clsGivePot.m_nSecond = 0;
            m_clsGivePot.m_nThird = 0;

            m_clsGivePot.m_nOnerPrizeCall = 0;
            int nAppendCash = CGlobal.GetGearListByGameCode(m_nGameCode).Sum(value=>value.m_nSlotCash);
            if(m_nGameCode == CDefine.GAME_DRAGON)
            {
                nAppendCash *= 10;
            }
            m_clsGivePot.m_nAppenCash = nAppendCash;
            m_clsGivePot.m_nZero = nAppendCash * m_nGameRate / 100;

            ResetTotalCash();
        }

        public void RealTime()
        {
            int nRetJackAmount1 = 0, nRetJackAmount2 = 0, nRetJackAmount3 = 0;
            int nRetVJackAmount1 = 0, nRetVJackAmount2 = 0, nRetVJackAmount3 = 0;
            int nRetPrizeCash1 = 0, nRetPrizeCash2 = 0, nRetPrizeCash3 = 0;
            int nRetVPrizeCash1 = 0, nRetVPrizeCash2 = 0, nRetVPrizeCash3 = 0;

            while (true)
            {
                #region 1차환수단계
                //환수률발동단계별금액을 생성한다. 단계별금액은 관리자에서 설정한 발동금액을 랜덤으로 여러 단계로 나누어 줄수 있게 해준다.
                if (m_lstTotalStep1.Count == 0 && m_bJackpot1)
                {
                    int nPrizeCash1 = m_nGameCash1 * m_nGameRate1 / 100 + nRetPrizeCash1;
                    int nRealCash1 = nPrizeCash1;

                    while(nPrizeCash1 >= 100)
                    {
                        int nRaiseCash = 0;
                        while(true)
                        {
                            nRaiseCash = CGlobal._RND.Next(10, 100) * m_nGameCash1 / 100;
                            if (!m_lstTotalStep1.Exists(value => value.m_nRaiseCash == nRaiseCash))
                                break;
                        }

                        int nPrizeCash = nPrizeCash1;
                        if(nPrizeCash1 > 2000)
                            nPrizeCash = CGlobal._RND.Next(10, 101) * nPrizeCash1 / 100;
                        nPrizeCash = Math.Min(15000, nPrizeCash);
                        nPrizeCash = Convert.ToInt32(nPrizeCash / 100) * 100;

                        CTotalStep totalStep1 = new CTotalStep();
                        totalStep1.m_nRaiseCash = nRaiseCash;
                        totalStep1.m_nPrizeCash = nPrizeCash;

                        m_lstTotalStep1.Add(totalStep1);
                        nPrizeCash1 -= totalStep1.m_nPrizeCash;
                    }

                    m_lstTotalStep1 = m_lstTotalStep1.OrderBy(value=>value.m_nRaiseCash).ToList();
                    nRetPrizeCash1 = nPrizeCash1;
                    m_clsGivePot.m_nFirst = nRealCash1 - nRetPrizeCash1;

                    m_bJackpot1 = false;
                }

                //체험머니환수률발동단계별금액을 생성한다. 단계별금액은 관리자에서 설정한 발동금액을 랜덤으로 여러 단계로 나누어 줄수 있게 해준다.
                if (m_lstVirtualStep1.Count == 0 && m_bVJackpot1)
                {
                    int nPrizeCash1 = m_nGameCash1 * m_nGameRate1 / 100 + nRetVPrizeCash1;
                    int nRealCash1 = nPrizeCash1;

                    while (nPrizeCash1 >= 100)
                    {
                        int nRaiseCash = 0;
                        while (true)
                        {
                            nRaiseCash = CGlobal._RND.Next(10, 100) * m_nGameCash1 / 100;
                            if (!m_lstVirtualStep1.Exists(value => value.m_nRaiseCash == nRaiseCash))
                                break;
                        }

                        int nPrizeCash = nPrizeCash1;
                        if (nPrizeCash1 > 2000)
                            nPrizeCash = CGlobal._RND.Next(10, 101) * nPrizeCash1 / 100;
                        nPrizeCash = Math.Min(15000, nPrizeCash);
                        nPrizeCash = Convert.ToInt32(nPrizeCash / 100) * 100;

                        CTotalStep totalStep1 = new CTotalStep();
                        totalStep1.m_nRaiseCash = nRaiseCash;
                        totalStep1.m_nPrizeCash = nPrizeCash;

                        m_lstVirtualStep1.Add(totalStep1);
                        nPrizeCash1 -= totalStep1.m_nPrizeCash;
                    }
                    m_lstVirtualStep1 = m_lstVirtualStep1.OrderBy(value=>value.m_nRaiseCash).ToList();
                    nRetVPrizeCash1 = nPrizeCash1;
                    m_bVJackpot1 = false;
                }
                #endregion

                #region 2차환수단계  50만원이하의 잭팟점수를 주어야 한다.
                if (m_lstTotalStep2.Count == 0 && m_bJackpot2)
                {
                    int nPrizeCash2 = m_nGameCash2 * m_nGameRate2 / 100 + nRetPrizeCash2;
                    int nRealCash2 = nPrizeCash2;

                    if (m_nAutoPrize == 0 && m_clsGivePot.m_nOnerPrizeCall > 0)
                    {
                        if(nPrizeCash2 > m_clsGivePot.m_nOnerPrizeCall)
                        {
                            nPrizeCash2 -= m_clsGivePot.m_nOnerPrizeCall;
                            m_clsGivePot.m_nOnerPrizeCall = 0;
                            nRealCash2 = nPrizeCash2;
                        }
                        else
                        {
                            m_clsGivePot.m_nOnerPrizeCall -= nPrizeCash2;
                            nPrizeCash2 = 0;
                            nRealCash2 = 0;
                        }
                    }

                    while(nPrizeCash2 > 20000)
                    {
                        int nRaiseCash = 0;
                        while (true)
                        {
                            nRaiseCash = CGlobal._RND.Next(10, 100) * m_nGameCash2 / 100;
                            if (!m_lstTotalStep2.Exists(value => value.m_nRaiseCash == nRaiseCash))
                                break;
                        }
                        int nPrizeCash = CGlobal._RND.Next(10, 101) * nPrizeCash2 / 100;
                        nPrizeCash = Math.Max(20000, nPrizeCash);
                        nPrizeCash = Math.Min(nPrizeCash, 50 * 10000);
                        nPrizeCash = Convert.ToInt32(nPrizeCash / 10000) * 10000;

                        CTotalStep totalStep2 = new CTotalStep();
                        totalStep2.m_nRaiseCash = nRaiseCash;
                        totalStep2.m_nPrizeCash = nPrizeCash;
                        m_lstTotalStep2.Add(totalStep2);
                        nPrizeCash2 -= totalStep2.m_nPrizeCash;
                    }
                    m_lstTotalStep2 = m_lstTotalStep2.OrderBy(value => value.m_nRaiseCash).ToList();
                    nRetPrizeCash2 = nPrizeCash2;
                    m_clsGivePot.m_nSecond = nRealCash2 - nRetPrizeCash2;
                    m_bJackpot2 = false;
                }
               
                
                if (m_lstVirtualStep2.Count == 0 && m_bVJackpot2)
                {
                    int nPrizeCash2 = m_nGameCash2 * m_nGameRate2 / 100 + nRetVPrizeCash2;
                    int nRealCash2 = nPrizeCash2;

                    while (nPrizeCash2 > 20000)
                    {
                        int nRaiseCash = 0;
                        while (true)
                        {
                            nRaiseCash = CGlobal._RND.Next(10, 100) * m_nGameCash2 / 100;
                            if (!m_lstVirtualStep2.Exists(value => value.m_nRaiseCash == nRaiseCash))
                                break;
                        }
                        int nPrizeCash = CGlobal._RND.Next(10, 101) * nPrizeCash2 / 100;
                        nPrizeCash = Math.Max(20000, nPrizeCash);
                        nPrizeCash = Math.Min(nPrizeCash, 50 * 10000);
                        nPrizeCash = Convert.ToInt32(nPrizeCash / 10000) * 10000;

                        CTotalStep totalStep2 = new CTotalStep();
                        totalStep2.m_nRaiseCash = nRaiseCash;
                        totalStep2.m_nPrizeCash = nPrizeCash;
                        m_lstVirtualStep2.Add(totalStep2);
                        nPrizeCash2 -= totalStep2.m_nPrizeCash;
                    }
                    m_lstVirtualStep2 = m_lstVirtualStep2.OrderBy(value => value.m_nRaiseCash).ToList();
                    nRetVPrizeCash2 = nPrizeCash2;
                    m_bVJackpot2 = false;
                }
                #endregion

                #region 3차환수단계 설정
                if (m_lstTotalStep3.Count == 0 && m_bJackpot3)
                {
                    int nPrizeCash3 = m_nGameCash3 * m_nGameRate3 / 100 + nRetPrizeCash3;
                    int nRealCash3 = nPrizeCash3;

                    if (m_clsGivePot.m_nOnerPrizeCall > 0)
                    {
                        if (nPrizeCash3 > m_clsGivePot.m_nOnerPrizeCall)
                        {
                            nPrizeCash3 -= m_clsGivePot.m_nOnerPrizeCall;
                            nRealCash3 = nPrizeCash3;
                            m_clsGivePot.m_nOnerPrizeCall = 0;
                        }
                        else
                        {
                            m_clsGivePot.m_nOnerPrizeCall -= nPrizeCash3;
                            nPrizeCash3 = 0;
                            nRealCash3 = 0;
                        }
                    }

                    while (nPrizeCash3 > 20000)
                    {
                        int nRaiseCash = 0;
                        while (true)
                        {
                            nRaiseCash = CGlobal._RND.Next(10, 100) * m_nGameCash3 / 100;
                            if (!m_lstTotalStep3.Exists(value => value.m_nRaiseCash == nRaiseCash))
                                break;
                        }
                        int nPrizeCash = CGlobal._RND.Next(10, 101) * nPrizeCash3 / 100;
                        nPrizeCash = Math.Max(20000, nPrizeCash);
                        nPrizeCash = Math.Min(nPrizeCash, 100 * 10000);
                        nPrizeCash = Convert.ToInt32(nPrizeCash / 10000) * 10000;

                        CTotalStep totalStep3 = new CTotalStep();
                        totalStep3.m_nRaiseCash = nRaiseCash;
                        totalStep3.m_nPrizeCash = nPrizeCash;
                        m_lstTotalStep3.Add(totalStep3);
                        nPrizeCash3 -= totalStep3.m_nPrizeCash;
                    }
                    m_lstTotalStep3 = m_lstTotalStep3.OrderBy(value => value.m_nRaiseCash).ToList();
                    nRetPrizeCash3 = nPrizeCash3;
                    m_clsGivePot.m_nThird = nRealCash3 - nRetPrizeCash3;
                    m_bJackpot3 = false;
                }
                

                if (m_lstVirtualStep3.Count == 0 && m_bVJackpot3)
                {
                    int nPrizeCash3 = m_nGameCash3 * m_nGameRate3 / 100 + nRetVPrizeCash3;
                    int nRealCash3 = nPrizeCash3;

                    while (nPrizeCash3 > 20000)
                    {
                        int nRaiseCash = 0;
                        while (true)
                        {
                            nRaiseCash = CGlobal._RND.Next(10, 100) * m_nGameCash3 / 100;
                            if (!m_lstVirtualStep3.Exists(value => value.m_nRaiseCash == nRaiseCash))
                                break;
                        }
                        int nPrizeCash = CGlobal._RND.Next(10, 101) * nPrizeCash3 / 100;
                        nPrizeCash = Math.Max(20000, nPrizeCash);
                        nPrizeCash = Math.Min(nPrizeCash, 100 * 10000);
                        nPrizeCash = Convert.ToInt32(nPrizeCash / 10000) * 10000;

                        CTotalStep totalStep3 = new CTotalStep();
                        totalStep3.m_nRaiseCash = nRaiseCash;
                        totalStep3.m_nPrizeCash = nPrizeCash;
                        m_lstVirtualStep3.Add(totalStep3);
                        nPrizeCash3 -= totalStep3.m_nPrizeCash;
                    }
                    m_lstVirtualStep3 = m_lstVirtualStep3.OrderBy(value => value.m_nRaiseCash).ToList();
                    nRetVPrizeCash3 = nPrizeCash3;
                    m_bVJackpot3 = false;
                }
                #endregion


                if(m_bStart)
                {
                    m_bStart = false;
                    m_lstTotalStep1.RemoveAll(value=>value.m_nRaiseCash <= m_clsTotalPot.m_nFirst);
                    m_lstTotalStep2.RemoveAll(value => value.m_nRaiseCash <= m_clsTotalPot.m_nSecond);
                    m_lstTotalStep3.RemoveAll(value => value.m_nRaiseCash <= m_clsTotalPot.m_nThird);
                }


                //발동금액에 도달하면 잭팟을 주어야 한다.
                if (m_lstTotalStep1.Count > 0 && m_clsTotalPot.m_nFirst >= m_lstTotalStep1[0].m_nRaiseCash)
                {
                    CBaseGear gear = SelectRandomPrizeGear0(false);
                    if (gear != null)
                    {
                        nRetJackAmount1 = gear.RaiseJackPot1(m_lstTotalStep1[0].m_nPrizeCash + nRetJackAmount1);
                        m_lstTotalStep1.RemoveAt(0);
                    }
                }

                if (m_lstVirtualStep1.Count > 0 && m_clsVirtualPot.m_nFirst >= m_lstVirtualStep1[0].m_nRaiseCash)
                {
                    CBaseGear gear = SelectRandomPrizeGear0(true);
                    if (gear != null)
                    {
                        nRetVJackAmount1 = gear.RaiseJackPot1(m_lstVirtualStep1[0].m_nPrizeCash + nRetVJackAmount1);
                        m_lstVirtualStep1.RemoveAt(0);
                    }
                }


                if (m_lstTotalStep2.Count > 0 && m_clsTotalPot.m_nSecond >= m_lstTotalStep2[0].m_nRaiseCash)
                {
                    CBaseGear gear = SelectRandomPrizeGear0(false);
                    if (gear != null)
                    {
                        m_nGivePrizeCount++;
                        nRetJackAmount2 = gear.RaiseJackPot2(m_lstTotalStep2[0].m_nPrizeCash + nRetJackAmount2);
                        m_lstTotalStep2.RemoveAt(0);
                    }
                }

                if (m_lstVirtualStep2.Count > 0 && m_clsVirtualPot.m_nSecond >= m_lstVirtualStep2[0].m_nRaiseCash)
                {
                    CBaseGear gear = SelectRandomPrizeGear0(true);
                    if (gear != null)
                    {
                        m_nGivePrizeCount++;
                        nRetVJackAmount2 = gear.RaiseJackPot2(m_lstVirtualStep2[0].m_nPrizeCash + nRetVJackAmount2);
                        m_lstVirtualStep2.RemoveAt(0);
                    }
                }

                if (m_lstTotalStep3.Count > 0 && m_clsTotalPot.m_nThird >= m_lstTotalStep3[0].m_nRaiseCash)
                {
                    CBaseGear gear = SelectRandomPrizeGear0(false);
                    if (gear != null)
                    {
                        m_nGivePrizeCount++;
                        nRetJackAmount3 = gear.RaiseJackPot3(m_lstTotalStep3[0].m_nPrizeCash + nRetJackAmount3);
                        m_lstTotalStep3.RemoveAt(0);
                    }
                }

                if (m_lstVirtualStep3.Count > 0 && m_clsVirtualPot.m_nThird >= m_lstVirtualStep3[0].m_nRaiseCash)
                {
                    CBaseGear gear = SelectRandomPrizeGear0(true);
                    if (gear != null)
                    {
                        m_nGivePrizeCount++;
                        nRetVJackAmount3 = gear.RaiseJackPot3(m_lstVirtualStep3[0].m_nPrizeCash + nRetVJackAmount3);
                        m_lstVirtualStep3.RemoveAt(0);
                    }
                }

                if (m_nGivePrizeCount > 100)

                    m_nGivePrizeCount = 0;

                //환수률을 따져주어야 한다.
                if (m_clsTotalPot.m_nFirst >= m_nGameCash1)
                {
                    if (m_lstTotalStep1.Count > 0)
                    {
                        CBaseGear gear = SelectRandomPrizeGear0(false);
                        if (gear != null)
                        {
                            int nPrizeCash = m_lstTotalStep1.Sum(value => value.m_nPrizeCash);
                            nRetJackAmount1 = gear.RaiseJackPot1(nPrizeCash + nRetJackAmount1);
                            m_lstTotalStep1.Clear();
                        }
                    }

                    //1차잭팟금액 초기화
                    m_clsTotalPot.m_nSecond += m_clsTotalPot.m_nFirst - (m_nGameCash1 * m_nGameRate1 / 100);
                    m_clsTotalPot.m_nFirst = 0;
                    m_bJackpot1 = true;
                }

                //마지막발동금액에 이르면 다음단계에로 금액을 넘기면서 초기화를 진행한다.
                if (m_clsVirtualPot.m_nFirst >= m_nGameCash1)
                {
                    if (m_lstVirtualStep1.Count > 0)
                    {
                        CBaseGear gear = SelectRandomPrizeGear0(true);
                        if (gear != null)
                        {
                            int nPrizeCash = m_lstVirtualStep1.Sum(value => value.m_nPrizeCash);
                            nRetVJackAmount1 = gear.RaiseJackPot1(nPrizeCash + nRetVJackAmount1);
                            m_lstVirtualStep1.Clear();
                        }
                    }


                    //1차잭팟금액 초기화
                    m_clsVirtualPot.m_nSecond += m_clsVirtualPot.m_nFirst - (m_nGameCash1 * m_nGameRate1 / 100);
                    m_clsVirtualPot.m_nFirst = 0;
                    m_bVJackpot1 = true;
                }

                if (m_clsTotalPot.m_nSecond >= m_nGameCash2)
                {
                    if (m_lstTotalStep2.Count > 0)
                    {
                        CBaseGear gear = SelectRandomPrizeGear0(false);
                        if (gear != null)
                        {
                            int nPrizeCash = m_lstTotalStep2.Sum(value => value.m_nPrizeCash);
                            nRetJackAmount2 = gear.RaiseJackPot2(nPrizeCash + nRetJackAmount2);
                            m_lstTotalStep2.Clear();
                        }
                    }

                    //2차잭팟금액 초기화
                    m_clsTotalPot.m_nThird += m_clsTotalPot.m_nSecond - (m_nGameCash2 * m_nGameRate2 / 100);
                    m_clsTotalPot.m_nSecond = 0;
                    m_bJackpot2 = true;
                }

                if (m_clsVirtualPot.m_nSecond >= m_nGameCash2)
                {
                    if (m_lstVirtualStep2.Count > 0)
                    {
                        CBaseGear gear = SelectRandomPrizeGear0(false);
                        if (gear != null)
                        {
                            int nPrizeCash = m_lstVirtualStep2.Sum(value => value.m_nPrizeCash);
                            nRetVJackAmount2 = gear.RaiseJackPot2(nPrizeCash + nRetVJackAmount2);
                            m_lstVirtualStep2.Clear();
                        }
                    }


                    //2차잭팟금액 초기화
                    m_clsVirtualPot.m_nThird += m_clsVirtualPot.m_nSecond - (m_nGameCash2 * m_nGameRate2 / 100);
                    m_clsVirtualPot.m_nSecond = 0;
                    m_bVJackpot2 = true;
                }


                if (m_clsTotalPot.m_nThird >= m_nGameCash3)
                {
                    if (m_lstTotalStep3.Count > 0)
                    {
                        CBaseGear gear = SelectRandomPrizeGear0(false);
                        if (gear != null)
                        {
                            int nPrizeCash = m_lstTotalStep3.Sum(value => value.m_nPrizeCash);
                            nRetJackAmount3 = gear.RaiseJackPot3(nPrizeCash + nRetJackAmount3);
                            m_lstTotalStep3.Clear();
                        }
                    }

                    //3차잭팟금액 초기화
                    int nReal = m_clsTotalPot.m_nThird - (m_nGameCash3 * m_nGameRate3 / 100);
                    CDataBase.InsertRealCash(m_nGameCode, nReal);
                    m_clsTotalPot.m_nThird = 0;
                    m_clsGivePot.m_nRealCash += nReal;
                    m_bJackpot3 = true;
                }

                if (m_clsVirtualPot.m_nThird >= m_nGameCash3)
                {
                    if (m_lstVirtualStep3.Count > 0)
                    {
                        CBaseGear gear = SelectRandomPrizeGear0(false);
                        if (gear != null)
                        {
                            int nPrizeCash = m_lstVirtualStep3.Sum(value => value.m_nPrizeCash);
                            nRetVJackAmount3 = gear.RaiseJackPot2(nPrizeCash + nRetVJackAmount3);
                            m_lstVirtualStep3.Clear();
                        }
                    }

                    //3차잭팟금액 초기화
                    int nReal = m_clsVirtualPot.m_nThird - (m_nGameCash3 * m_nGameRate3 / 100);
                    m_clsVirtualPot.m_nThird = 0;
                    m_bVJackpot3 = true;
                }


                Thread.Sleep(1000);
            }
        }

        private CBaseGear SelectRandomPrizeGear0(bool bVirtual)
        {
            List<CBaseGear> lstGear = null;
            List<CBaseGear> lstJackGear = new List<CBaseGear>();

            if (bVirtual)
            {
                lstGear = CGlobal.GetGearList().FindAll(value => value.m_nGearState == 1 && value.m_nGearRun == 1 && value.m_nTakeUser > 0 && value.m_nGearJack == 0 &&
                                           CGlobal.GetUserByCode(value.m_nTakeUser).m_nChargeCnt == 0 && value.m_nGameCode == m_nGameCode).OrderByDescending(value => value.m_nAccuCash).ToList();
            }
            else
            {
                lstGear = CGlobal.GetGearList().FindAll(value => value.m_nGearState == 1 && value.m_nGearRun == 1 && value.m_nTakeUser > 0 && value.m_nGearJack == 0 &&
                                           CGlobal.GetUserByCode(value.m_nTakeUser).m_nChargeCnt > 0 && value.m_nGameCode == m_nGameCode).OrderByDescending(value => value.m_nAccuCash).ToList();
            }
            

            List<CBaseGear> lstTemp = new List<CBaseGear>();
            while (lstGear.Count > 0)
            {
                for (int i = 0; i < lstGear.Count; i++)
                {
                    lstTemp.Add(lstGear[i]);
                }
                lstGear.RemoveAt(lstGear.Count - 1);
            }

            if (lstTemp.Count == 0)
                return null;

            CBaseGear gear = CGlobal.RandomSelect(lstTemp);

            return gear;
        }

        private CBaseGear SelectRandomPrizeGear1(bool bVirtual)
        {
            List<CBaseGear> lstGear = new List<CBaseGear>();

            if (bVirtual)
            {
                lstGear = CGlobal.GetGearList().FindAll(value => value.m_nGearState == 1 && value.m_nGearRun == 1 && value.m_nTakeUser > 0 && value.m_nGearJack == 0 &&
                                           CGlobal.GetUserByCode(value.m_nTakeUser).m_nChargeCnt == 0 && value.m_nGameCode == m_nGameCode).OrderByDescending(value => value.m_nAccuCash).ToList();
            }
            else
            {
                lstGear = CGlobal.GetGearList().FindAll(value => value.m_nGearState == 1 && value.m_nGearRun == 1 && value.m_nTakeUser > 0 && value.m_nGearJack == 0 &&
                                           CGlobal.GetUserByCode(value.m_nTakeUser).m_nChargeCnt > 0 && value.m_nGameCode == m_nGameCode).OrderByDescending(value => value.m_nAccuCash).ToList();
            }

            if (lstGear.Count == 0)
                return null;

            CBaseGear gear = CGlobal.RandomSelect(lstGear);

            return gear;
        }

        public int GetGivePrizeCount()
        {
            return m_nGivePrizeCount;
        }

        public void SetGivePrizeCount(int nCount)
        {
            m_nGivePrizeCount = nCount;
        }
    }
}
