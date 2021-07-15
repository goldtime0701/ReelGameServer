using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public class CUser : CUserModel
    {
        public List<CBaseGear> m_lstGear;           //유저가 보유하고 있는 기어코드리스트

        //public CAgent m_clsAgent;                 //총판객체
        public DateTime m_dtChargeTime;             //마지막으로 충전신청한 시간
        public DateTime m_dtExChargeTime;           //마지막으로 환전신청한 시간
        public int m_nGearAppendCash;               //금일 기계에 충전한금액
        private bool[] m_lstGameLogin = { false, false, false, false, false, false, false, false, false, false, false, false, false };  //Web 받식에서 게임별로 로그인하였는가를 가려내기 위한 변수

        public string m_strTempAcid;                //통신이 끊어지였을때 계좌를 보관하였다가 통신이 이어지면 자동으로 보내주기 위한 변수
        public List<CSChatPacket> m_lstTempSecurityChat;   //통신이 끊어지였을때 관리자채팅을 보관하였다가 통신이 이어지면 자동으로 보내주기 위한 변수


        public CUser()
        {
            m_lstGear = new List<CBaseGear>();
            m_dtChargeTime = CMyTime.ConvertStrToTime("2020-01-01 00:00:00");
            m_dtExChargeTime = CMyTime.ConvertStrToTime("2020-01-01 00:00:00");
            m_lstTempSecurityChat = new List<CSChatPacket>();
        }

        

        public List<CBaseGear> GetUserGearList()
        {
            return m_lstGear;
        }

        public void AddUserGear(CBaseGear gear)
        {
            m_lstGear.Add(gear);
        }

        public CUserModel GetUserModel()
        {
            CUserModel clsModel = new CUserModel();
            clsModel.m_nUserCode = m_nUserCode;
            clsModel.m_strUserID = m_strUserID;          //아이디
            clsModel.m_strUserPW = m_strUserPW;          //비번
            clsModel.m_strUserNick = m_strUserNick;        //닉명
            clsModel.m_strUserMail = m_strUserMail;        //메일
            clsModel.m_strUserPhone = m_strUserPhone;       //손전화
            clsModel.m_strUserName = m_strUserName;        //유저이름
            clsModel.m_strBankName = m_strBankName;        //은행이름
            clsModel.m_strBankAcid = m_strBankAcid;        //은행계좌
            clsModel.m_nUserLevel = m_nUserLevel;             //레벨
            clsModel.m_nUserCash = m_nUserCash;              //보유금액
            clsModel.m_nUserCupn = m_nUserCupn;              //보유쿠폰
            clsModel.m_nAgenCode = m_nAgenCode;              //총판
            clsModel.m_nUserState = m_nUserState;             //0-가입대기, 1-정상, 2-차단, 3-삭제
            clsModel.m_strUserRegTime = m_strUserRegTime;     //등록시간
            clsModel.m_nEventState = m_nEventState;            //0-일반, 1-관리자로부터 쪽지를 받음
            clsModel.m_nUserCharge = m_nUserCharge;           //충전합계
            clsModel.m_nUserExcharge = m_nUserExcharge;         //환전합계
            clsModel.m_nUserUseCash = m_nUserUseCash;           //유저가 사용한 금액(스핀돌기한번할때마다 증가, 총판이 한번 환전하면 0으로 초기화 된다.)
            clsModel.m_nUserWinCash = m_nUserWinCash;           //유저가 당첨된 금액(점수가 당첨될때마다 증가, 총판이 한번 환전하면 0으로 초기화 된다.)
            clsModel.m_nUserBonusCash = m_nUserBonusCash;         //유저거 받은 보너스금액(보너스금액으로 스핀을 돌릴때는 잭팟발동금액이 오르지 않는다.)
            clsModel.m_nUserLogCnt = m_nUserLogCnt;            //접속회수
            clsModel.m_nUserLogin = m_nUserLogin;             //가입하였는가?
            clsModel.m_nAbsentCnt = m_nAbsentCnt;             //출석회수
            clsModel.m_nChargeCnt = m_nChargeCnt;             //충전회수
            clsModel.m_nExchargeCnt = m_nExchargeCnt;           //환전회수
            clsModel.m_nAppendCash = m_nAppendCash;            //환전한다음부토 기계에 넣은 금액
            clsModel.m_nBonusCash = m_nBonusCash;             //환전한다음부터 받은 보너스 합계
            clsModel.m_nChargeCash = m_nChargeCash;            //환전한다음부터 충전한 금액
            clsModel.m_lstAsent = m_lstAsent;                   //날자별 출석체크정보
            clsModel.m_nVirtualCash = m_nVirtualCash;           //체험머니
            clsModel.m_nVirtualCupn = m_nVirtualCupn;           //체험쿠폰
            clsModel.m_strToken = m_strToken;
            clsModel.m_strLogTime = m_strLogTime;               //최종가입일시
            clsModel.m_nChatBlock = m_nChatBlock;              //채팅블록
            clsModel.m_nChatBlockA = m_nChatBlockA;            //전체채팅블록
            clsModel.m_nStoreCode = m_nStoreCode;
            clsModel.m_nMobile = m_nMobile;
            clsModel.m_strIP = m_strIP;
            clsModel.m_strMemo = m_strMemo;

            return clsModel;
        }

        public void SetUserGameLogin(int nGameCode)
        {
            m_lstGameLogin[nGameCode] = true;
        }

        public void SetUserGameLogout(int nGameCode)
        {
            m_lstGameLogin[nGameCode] = false;
        }

        public bool GetUserGameLogin(int nGameCode)
        {
            return m_lstGameLogin[nGameCode];
        }

        public bool CheckUserLogin()
        {
            bool bRet = m_lstGameLogin.ToList().Exists(value => value == true);
            return bRet;
        }

        public void UserLogout()
        {
            m_nUserLogin = 0;
            List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value => value.m_nTakeUser == m_nUserCode);
            for (int i = 0; i < lstGear.Count; i++)
            {
                lstGear[i].LogoutGear();
            }
            m_strIP = string.Empty;


            CGlobal.SendUserInfoToClient(this);
            CUserSocket client = CGlobal.GetUserSocketList().Find(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == m_nUserCode && value.IsApp());
            if(client != null)
                client.SetUserObject(null);

            CGlobal.SendUserInfoToChatAdmin(this);
        }

        public void UserLogout(int nGameCode)
        {
            SetUserGameLogout(nGameCode);
            m_nUserLogin = CheckUserLogin() ? 1 : 0;

            if(m_nUserLogin == 1)
            {
                List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value => value.m_nTakeUser == m_nUserCode && value.m_nGameCode == nGameCode);
                for(int i=0; i<lstGear.Count; i++)
                {
                    lstGear[i].LogoutGear();
                }
            }
            else
            {
                List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value => value.m_nTakeUser == m_nUserCode);
                for (int i = 0; i < lstGear.Count; i++)
                {
                    lstGear[i].LogoutGear();
                }
            }
            
            if(m_nUserLogin == 0)
                m_strIP = string.Empty;

            if(m_nStoreCode > 0)
            {
                CStore clsStore = CGlobal.GetStoreByCode(m_nStoreCode);
                if(clsStore != null && clsStore.m_nDemo == 1)
                {
                    m_nVirtualCash = 500000;
                    m_nVirtualCupn = 0;
                }
            }

            CGlobal.SendUserInfoToChatAdmin(this);
        }

        public void StartReconnectWait()
        {
            new Thread(() => CheckUserState(this)).Start();
        }

        private void CheckUserState(CUser user)
        {
            Thread.Sleep(30000);  //30초동안 대기하였다가 소켓이 이어지였는가를 검사한다.
            if (!CGlobal.GetUserSocketList().Exists(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == user.m_nUserCode))
            {
                if(user.m_nUserLogin == 1)
                {
                    user.UserLogout();
                    CGlobal.Log(user.m_strUserNick + " 통신오유, 30초 자동로그아웃");
                }
            }
                
        }
    }
}
