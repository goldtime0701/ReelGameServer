using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CRobotModel
    {
        public int m_nRbCode;
        public string m_strRbNick;
        public int m_nOnerCode;
        public int m_nGearCode;
        public int m_nGameCode;
        public int m_nRbLogin;
        public int m_nJackpot;      //잭팟상태 0-일반, 1-잭팟
        public int m_nAutoJack;     //자동잭팟 0-사용안함, 1-사용
    }
}
