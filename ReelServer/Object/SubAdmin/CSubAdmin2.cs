using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CSubAdmin2 : CSubAdminModel
    {
        public CSubAdmin m_clsSubAdmin { get { return GetSubAdmin(); } }
        public CSubAdmin0 m_clsSubAdmin0 { get { return GetSubAdmin0(); } }
        public CSubAdmin1 m_clsSubAdmin1 { get { return GetSubAdmin1(); } }

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
            return CGlobal.GetSubAdmin1ByCode(m_nSuperCode);
        }
    }
}
