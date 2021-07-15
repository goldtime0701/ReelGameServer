using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{ 
    public class CItem
    {
        public int m_nItemCode;
        public int m_nItemModel;
        public int m_nUserCode;
        public int m_nUse;
        public int m_nBuyPrice;
        public string m_strBuyTime;
        public string m_strUseTime;
        public int m_nGearCode;
        public string m_strUseNote;

        public CItem(int nUserCode, int nItemModel, int nPrice) 
        {
            m_nItemModel = nItemModel;
            m_nUserCode = nUserCode;
            m_strBuyTime = CMyTime.GetMyTimeStr();
            m_nBuyPrice = nPrice;
        }
    }
}
