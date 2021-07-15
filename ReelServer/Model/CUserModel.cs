using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CUserModel
    {
        public int m_nUserCode = 0;              //코드
        public string m_strUserID = "";          //아이디
        public string m_strUserPW = "";          //비번
        public string m_strUserNick = "";        //닉명
        public string m_strUserMail = "";        //메일(매장유저 아이디)
        public string m_strUserPhone = "";       //손전화
        public string m_strUserName = "";        //유저이름
        public string m_strBankName = "";        //은행이름
        public string m_strBankAcid = "";        //은행계좌
        public int m_nUserLevel = 1;             //레벨
        public int m_nUserCash = 0;              //보유금액
        public int m_nUserCupn = 0;              //보유쿠폰
        public int m_nAgenCode = 0;              //총판
        public int m_nUserState = 0;             //0-가입대기, 1-정상, 2-차단, 3-삭제
        public string m_strUserRegTime = "";     //등록시간
        public int m_nEventState = 0;            //0-일반, 1-관리자로부터 쪽지를 받음
        public long m_nUserCharge = 0;           //충전합계
        public long m_nUserExcharge = 0;         //환전합계
        public int m_nUserUseCash = 0;           //유저가 사용한 금액(스핀돌기한번할때마다 증가, 총판이 한번 환전하면 0으로 초기화 된다.)
        public int m_nUserWinCash = 0;           //유저가 당첨된 금액(점수가 당첨될때마다 증가, 총판이 한번 환전하면 0으로 초기화 된다.)
        public int m_nUserBonusCash = 0;         //유저거 받은 보너스금액(보너스금액으로 스핀을 돌릴때는 잭팟발동금액이 오르지 않는다.)
        public int m_nUserLogCnt = 0;            //접속회수
        public int m_nUserLogin = 0;             //가입하였는가?
        public int m_nAbsentCnt = 0;             //출석회수
        public int m_nChargeCnt = 0;             //충전회수
        public int m_nExchargeCnt = 0;           //환전회수
        public int m_nAppendCash = 0;            //환전한다음부토 기계에 넣은 금액
        public int m_nBonusCash = 0;             //환전한다음부터 받은 보너스 합계
        public int m_nChargeCash = 0;            //환전한다음부터 충전한 금액
        public int[] m_lstAsent = new int[31];   //날자별 출석체크정보
        public int m_nVirtualCash = 0;           //체험머니
        public int m_nVirtualCupn = 0;           //체험쿠폰
        public string m_strToken;                //웹소켓을 개설하기 위한 토큰
        public string m_strLogTime = "";         //최근로그인시간
        public int m_nChatBlock = 0;             //0-채팅가능, 1-채팅불록
        public int m_nChatBlockA = 0;             //0-전체 채팅가능, 1-채팅불록
        public int m_nStoreCode = 0;             //매장코드
        public string m_strMemo;                 //메모

        public string m_strIP = string.Empty;    //접속아이피
        public int m_nMobile = 0;                //모바일인가 PC인가
        public int m_nAbsentCheck = 0;

        public string m_strAgenMark { get { return GetAgentMark();   } }
        public string m_strAgenNick { get { return GetAgentNick(); } }
        public string m_strStoreNick { get { return GetStoreNick(); } }
        public string m_strSubAdmin0Nick { get { return GetSubAdmin0Nick(); } }
        public string m_strSubAdmin1Nick { get { return GetSubAdmin1Nick(); } }
        public string m_strSubAdmin2Nick { get { return GetSubAdmin2Nick(); } }


        private string GetAgentMark()
        {
            CAgent clsAgent = CGlobal.GetAgentByCode(m_nAgenCode);

            if (clsAgent == null)
                return string.Empty;
            else
                return clsAgent.m_strAgenMark;
        }

        private string GetStoreNick()
        {
            CStore clsStore = CGlobal.GetStoreByCode(m_nStoreCode);
            if (clsStore == null)
                return string.Empty;
            else
                return clsStore.m_strStoreNick;
        }

        private string GetAgentNick()
        {
            CAgent clsAgent = CGlobal.GetAgentByCode(m_nAgenCode);
            if (clsAgent == null)
                return string.Empty;
            else
                return clsAgent.m_strAgenNick;
        }

        private string GetSubAdmin0Nick()
        {
            CStore clsStore = CGlobal.GetStoreByCode(m_nStoreCode);
            if (clsStore == null)
                return string.Empty;
            return clsStore.m_clsSubAdmin0.m_strSubNick;
        }

        private string GetSubAdmin1Nick()
        {
            CStore clsStore = CGlobal.GetStoreByCode(m_nStoreCode);
            if (clsStore == null)
                return string.Empty;
            return clsStore.m_clsSubAdmin1.m_strSubNick;
        }

        private string GetSubAdmin2Nick()
        {
            CStore clsStore = CGlobal.GetStoreByCode(m_nStoreCode);
            if (clsStore == null)
                return string.Empty;
            return clsStore.m_clsSubAdmin2.m_strSubNick;
        }
    }
}
