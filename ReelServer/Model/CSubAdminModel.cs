using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CSubAdminModel
    {
        public int m_nCode;                 //영본사코드
        public string m_strSubID;           //아이디
        public string m_strSubNick;         //닉명
        public string m_strSubPwd;          //비번
        public int m_nTotalPro;             //상위로부터 할당된 요율 = 자체 + 기타
        public int m_nSelfPro;              //자체요율
        public int m_nOtherPro;             //기타요율
        public int m_nRealCash;             //정산금액
        public int m_nEventCash;            //이벤트금액
        public int m_nState;                //상태, 0-대기, 1-정상, 2-차단
        public int m_nChildCnt;             //소속된 부본사개수
        public string m_strRegTime;         //등록시간
        public int m_nSuperCode;            //상위코드
        public string m_strPhone;           //전화번호
        public string m_strMark;            //가입코드
        public long m_nExRealCash;          //정산받은금액
        public long m_nExEventCash;         //정산한 이벤트금액
    }
}
