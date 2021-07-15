using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public abstract class CBaseGear : CGearModel
    {
        //public CUserSocket m_socket;
        protected int m_nStep;   //환수단계   0-0차환수, 1-1차환수, 2-2차환수, 3-3차환수, 4-강제잭팟
        protected Random RND;
        public List<CJackInfoModel> m_lstJackInfo;   //잭팟내역 100개까지만 보관

        public CBaseGear()
        {
            m_lstJackInfo = new List<CJackInfoModel>();
            InitScoreTableInfo();
            RND = new Random();
            m_nLstGraph = new List<int>();
            for (int i = 0; i < 10; i++)
                m_nLstGraph.Add(0);
        }
        //점수표초기화함수
        public abstract void InitScoreTableInfo();
        //일반점수배렬 만들기(만원넣기 할때 일반점수배렬이 만들어 진다.)
        public abstract void MakeNormalScorelRoll(int nAddCash, bool bFlag = true);
        //잭팟돌기생성
        public abstract bool MakePrizeRoll();
        //로봇자연환수잭팟
        public abstract int RaiseJackPotR(int nJackCash);
        //1차환수잭팟
        public abstract int RaiseJackPot1(int nJackCash);
        //2차환수잭팟
        public abstract int RaiseJackPot2(int nJackCash);
        //3차환수잭팟
        public abstract int RaiseJackPot3(int nJackCash);
        //잭팟정보지우기
        public abstract void ClearPrizeInfo();
        //기계정보지우기
        public abstract void ClearGear();
        //코인생성(손오공, 바다이야기에서만 사용)
        public abstract void OnCreateCoin(CUserSocket clsSocket);
        //스핀돌기
        public abstract void OnStartSpin(CUserSocket clsSocket, int nSpinCash = 100);
        //기프트점수증가
        public abstract void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0);
        //슬롯캐시점수증가
        public abstract void OnAddSlotCash(CUserSocket clsSocket, int nCash);
        //잭팟당첨정보를 내려보내는 함수(예시가 나올때마다 호출)
        public abstract void OnBroadCastPrizeKind();
        //잭팟이 끝났을때 당첨점수와 잭팟정보를 방송하는 함수
        public abstract void OnBroadCastPrizeInfo();
        //통신오유시 재접속하였을때 기계가 동작상태에 있었다면 돌기정보를 내려보낸다
        public abstract void OnReconnect(CUserSocket clsSocket);
        public abstract void OnEndPrizeCall();
        public abstract void OnGearStart(int nRun);
        public abstract void UseItemByUser(CItem clsItem);

        public void SetGiveStep(int nStep)
        {
            m_nStep = nStep;
        }


        public virtual void LogoutGear()
        {
            m_nGearRun = 0;
            if (m_nGameCode == CDefine.GAME_ALADIN)
                (this as CAldGear).OnCreateCoin(null);  //알라딘에서는 잭팟중에 탈퇴할때 애니매션을 동작상태에서 해방시켜주어야 한다.
            else if (m_nGameCode == CDefine.GAME_ALADIN2)
                (this as CAldGear2).OnCreateCoin(null);  //알라딘에서는 잭팟중에 탈퇴할때 애니매션을 동작상태에서 해방시켜주어야 한다.
            else if(m_nGameCode == CDefine.GAME_NWD)
                (this as CNwdGear).OnFinishAnimation(0);  //신천지에서는 잭팟중에 탈퇴할때 애니매션을 동작상태에서 해방시켜주어야 한다.
            else if(m_nGameCode == CDefine.GAME_OCEAN)
                (this as COcaGear).OnFinishAnimation();  //오션에서는 잭팟중에 탈퇴할때 애니매션을 동작상태에서 해방시켜주어야 한다.


            CUser clsUser = CGlobal.GetUserByCode(m_nTakeUser);
            if(clsUser != null)
            {
                if (clsUser.m_nChargeCnt == 0)
                {
                    //체험머니를 사용하는 중이다.
                    if (clsUser != null)
                        clsUser.m_nVirtualCash += m_nGiftCash + m_nSlotCash;
                    m_nGiftCash = 0;
                    m_nSlotCash = 0;
                }

                if (m_nGiftCash > 0 || m_nSlotCash > 0)
                {
                    CDataBase.SaveGearInfoToDB(this);
                    return;
                }
            }

            if (m_nGearCheck == 1)
            {
                m_dtCheckTime = CMyTime.GetMyTime();
                CDataBase.SaveGearInfoToDB(this);
                return;
            }

            m_nGearRun = 0;
            m_nTakeUser = 0;
            m_nGearState = 0;
            m_nGearJack = 0;
            this.ClearGear();
            CDataBase.SaveGearInfoToDB(this);

            if (m_nAccuCash >= 20 * 10000 || m_nAccuCash <= 10000)
            {
                m_nAccuCash = CGlobal.Random(100, 300);
                m_nAccuCash *= 100;
                //new Thread(() => CGlobal.InsertRobotToGear(this)).Start();
            }
            else if (m_nAccuCash < -(50 * 10000))
            {
                m_nAccuCash = CGlobal.Random(100, 300);
                m_nAccuCash *= 100;
            }

            if (clsUser != null)
            {
                clsUser.GetUserGearList().Remove(this);

                CDataBase.SaveUserInfoToDB(clsUser);
                CGlobal.SendUserInfoToAdmin(clsUser);
            }
        }

        public CGearModel GetGearModel()
        {
            CGearModel clsModel = new CGearModel();
            clsModel.m_nGearCode = this.m_nGearCode;
            clsModel.m_nGearNum = this.m_nGearNum;
            clsModel.m_nSlotCash = this.m_nSlotCash;
            clsModel.m_nGiftCash = this.m_nGiftCash;
            clsModel.m_nGearState = this.m_nGearState;
            clsModel.m_nGearJack = this.m_nGearJack;
            clsModel.m_nTakeUser = this.m_nTakeUser;
            clsModel.m_nOrderUser = this.m_nOrderUser;
            clsModel.m_nAccuCash = this.m_nAccuCash;
            //clsModel.m_nJackCash = this.m_nJackCash;
            clsModel.m_nGameCode = this.m_nGameCode;
            clsModel.m_nGearRun = this.m_nGearRun;
            clsModel.m_nTakeRobot = this.m_nTakeRobot;
            clsModel.m_nGearCheck = this.m_nGearCheck;
            clsModel.m_dtCheckTime = this.m_dtCheckTime;
            clsModel.m_nTopJackCash = this.m_nTopJackCash;
            clsModel.m_nLastJackCash = this.m_nLastJackCash;
            clsModel.m_dtLastJackTime = this.m_dtLastJackTime;
            clsModel.m_nGrandCount = this.m_nGrandCount;
            clsModel.m_nMajorCount = this.m_nMajorCount;
            clsModel.m_nMinorCount = this.m_nMinorCount;
            clsModel.m_nMiniCount = this.m_nMiniCount;
            clsModel.m_nSpeedCash = this.m_nSpeedCash;
            clsModel.m_nGdcCash = this.m_nGdcCash;
            clsModel.m_nYmtRound = this.m_nYmtRound;
            clsModel.m_nLeftWater = this.m_nLeftWater;
            clsModel.m_nRightWater = this.m_nRightWater;
            clsModel.m_nLstGraph = new List<int>();
            for (int i=0; i<10; i++)
            {
                clsModel.m_nLstGraph.Add(this.m_nLstGraph[i]);
            }

            return clsModel;

        }

        public string MakeCongration(string strJack, int nScore)
        {
            int nShowScore = nScore / 10000;
            int nSubScore = (nScore - nShowScore * 10000) / 1000;

            string strScore = string.Empty;

            if(nShowScore > 0)
            {
                strScore = nShowScore + "만";
            }
                
            if (nSubScore > 0)
            {
                strScore += " " + nSubScore + "천";
            }

            string strNick = string.Empty;
            if (m_nTakeUser > 0)
                strNick = CGlobal.GetUserNickByCode(m_nTakeUser);
            else if (m_nTakeRobot > 0)
                strNick = CGlobal.GetRobotNickByCode(m_nTakeRobot);


            string strMessage = "[C][FFFFFF]운영자: " + strNick + " 님[-] [C][EC0DB0] " + strJack + "[-] [C][FFFF00]" + strScore + "[-][C][FFFFFF]" + "점 당첨되셨습니다. 축하합니다.";
            return strMessage;
        }
    }
}
