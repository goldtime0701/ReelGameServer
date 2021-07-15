using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CSubAdmin0 : CSubAdminModel
    {
        public CSubAdmin m_clsSubAdmin { get { return GetSubAdmin(); } }

        private CSubAdmin GetSubAdmin()
        {
            return CGlobal.GetSubAdmin();
        }
    }
}
