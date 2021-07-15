using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CStore : CStoreModel
    {
        public List<CUser> m_lstUser;      //매장에 속한 유저리스트
        public CSubAdmin m_clsSubAdmin { get { return GetSubAdmin(); } }
        public CSubAdmin0 m_clsSubAdmin0 { get { return GetSubAdmin0(); } }
        public CSubAdmin1 m_clsSubAdmin1 { get { return GetSubAdmin1(); } }
        public CSubAdmin2 m_clsSubAdmin2 { get { return GetSubAdmin2(); } }


        public CStore()
        {
            m_lstUser = new List<CUser>();
        }

        private CSubAdmin GetSubAdmin()
        {
            return CGlobal.GetSubAdmin();
        }

        private CSubAdmin0 GetSubAdmin0()
        {
            return CGlobal.GetSubAdmin0ByCode(GetSubAdmin1().m_nSuperCode);
        }

        private CSubAdmin1 GetSubAdmin1()
        {
            return CGlobal.GetSubAdmin1ByCode(GetSubAdmin2().m_nSuperCode);
        }

        private CSubAdmin2 GetSubAdmin2()
        {
            return CGlobal.GetSubAdmin2ByCode(m_nSuperCode);
        }
    }
}
