using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CGameModel
    {
        public int m_nGameCode;
        public string m_strGameID;
        public string m_strGameName;
        public int m_nGameUse;
        public int m_nCashAppend;
        public int m_nAutoPrize;

        public int m_nGameCash;
        public int m_nGameRate;
        public int m_nGameCash1;
        public int m_nGameRate1;
        public int m_nGameCash2;
        public int m_nGameRate2;
        public int m_nGameCash3;
        public int m_nGameRate3;
    }

    public class CAcidModel
    {
        public string m_strAcid;
        public int m_nStoreCode;
    }
}
