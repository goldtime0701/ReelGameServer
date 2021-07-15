using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CAgent : CAgentModel
    {
        public List<CUser> m_lstUser;           //총판에 가입된 유저객체
        public List<CStore> m_lstStore;         //총판에 속한 매장리스트


        public CAgent()
        {
            m_lstUser = new List<CUser>();
            m_lstStore = new List<CStore>();
        }
    }
}
