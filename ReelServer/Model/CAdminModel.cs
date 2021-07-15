using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CAdminModel
    {
        public int m_nOnerCode;                 //코드
        public string m_strOnerID;              //아이디
        public string m_strOnerPW;              //비번
        public string m_strOnerNick;            //닉명
        public string m_strOnerMail;            //메일
        public string m_strOnerPhone;           //손전화
        public int m_nOnerLevel;                //레벨
        public string m_strOnerRegTime;         //등록날자
        public bool m_bOnerLogin = false;       //가입하였는가?
        public string m_strJackPw;              //잭팟콜비번
        public string m_strSessionID;           //세션아이디
    }
}
