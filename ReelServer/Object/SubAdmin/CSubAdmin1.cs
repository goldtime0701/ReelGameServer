using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CSubAdmin1 : CSubAdminModel
    {
        public CSubAdmin m_clsSubAdmin { get { return GetSubAdmin(); } }
        public CSubAdmin0 m_clsSubAdmin0 { get { return GetSubAdmin0(); } }

        private CSubAdmin GetSubAdmin()
        {
            return CGlobal.GetSubAdmin();
        }

        private CSubAdmin0 GetSubAdmin0()
        {
            return CGlobal.GetSubAdmin0ByCode(m_nSuperCode);
        }
    }
}
