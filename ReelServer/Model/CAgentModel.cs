using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CAgentModel
    {
        public int m_nAgenCode;                 //코드
        public string m_strAgenID;              //아이디
        public string m_strAgenPW;              //비번
        public string m_strAgenNick;            //닉명
        public string m_strAgenMail;            //메일
        public string m_strAgenPhone;           //손전화
        public string m_strAgenMark;            //마크(유저들이 가입할때 추천인)
        public string m_strBankName;            //은행이름
        public string m_strBankAcid;            //은행계좌
        public int m_nAgenLevel;               //레벨
        public int m_nAgenCash;                 //보유머니
        public int m_nAgenState;                //상태 0-승인대기, 1-정상, 2-차단, 3-삭제
        public decimal m_fAgentPro;             //루징프로
        public string m_strAgenRegTime;         //등록시간
        public bool m_bAgenLogin = false;       //로그인하였는가?
        public string m_strDomain;              //총판도메인
        public int m_nIsStore;                  //매장전용총판인가?
        public int m_nUserCount;                //소속된 유저수
    }
}
