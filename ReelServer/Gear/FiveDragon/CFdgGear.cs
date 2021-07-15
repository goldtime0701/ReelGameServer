using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CFdgGear : CBaseGear
    {
        private CFdgPrizeInfo m_prizeInfo;                          //알라딘잭팟정보

        private CFdgScoreInfo m_clsSendScoreInfo;                 //클라이언트에 내려보낸 스핀정보
        private List<CFdgScoreStep>[] m_lstScoreStepList;         //내부점수렬들이 모두 0일때 현재 기계머니를 가지고 다시 계산된다.
        private int m_nSpinCash;                                  //릴이 한바퀴돌때마다 쌓이는 캐시(점수대기렬개수가 0이 되면 초기화 하여야 한다.)
        //private int m_nNewAppendCash;                           //현재 계산된 점수배렬이 다 실행된다음에 계산되여야 할 금액
        private List<CFdgScoreInfo> m_lstPrizeScore;              //잭팟점수정보리스트
        private int m_nFreeTotalCnt;                              //프리돌기수
        private int m_nFreeCurrentCnt;                            //프리돌기 현재돌아간 회수
        private int[] m_lstMulti = { 1, 1, 1 };                   //드래곤잭팟 돌기 배수 지정리스트
        private int m_nRealPrizeCash;                             //잭팟시에는 당첨될때마다 점수를 주는것이 아니고 모았다가 잭팟이 끝나면 기계머니를 올려주어야 한다.
        private int m_nGiveCnt;                                   //점수가 지내 자주나오는것을 막기 위한 턱값
        private int m_nGiveIndex;                                 //점수가 지내 자주나오는것을 막기 위한 인덱스
        private int m_nGiveScore;

        private int m_nDRGMulti;                                  //드래곤잭팟당시 배수
        private int m_nDRGExtra;                                  //드래곤잭팟당시 Extra 스핀상태인가  0-미사용, 1-25, 2-30
        private int m_nDRGContinueRem;                            //진행중인 련타회수

        //private bool m_bDrgPrize;                                 //드래곤잭팟진행중인가
        private int m_nDrgSpinCash;                               //드래곤잭팟진행당시 한번 도는데 드는 캐시(잭팟도중에 탈퇴되였다가 들어오면 이어서 잭팟을 줄때 사용)


        public CFdgGear() : base()
        {
            m_lstScoreStepList = new List<CFdgScoreStep>[] {
                 new List<CFdgScoreStep>(), new List<CFdgScoreStep>()
            };
            m_lstPrizeScore = new List<CFdgScoreInfo>();
            //m_bDrgPrize = false;
        }

        public override void ClearPrizeInfo()
        {
            m_nGearJack = 0;
            m_prizeInfo = null;
            //m_bDrgPrize = false;
            m_nRealPrizeCash = 0;
            m_nDrgSpinCash = 0;
            m_nDRGContinueRem = 0;
            m_lstPrizeScore.Clear();
        }

        public override void ClearGear()
        {
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            SetGiveStep(0);
            ClearPrizeInfo();
        }

        public override void LogoutGear()
        {
            if (m_nRealPrizeCash > 0 && m_prizeInfo != null)
                m_prizeInfo.m_nPrizeCash += m_nRealPrizeCash;

            m_nGearJack = 0;
            //m_bDrgPrize = false;
            m_nRealPrizeCash = 0;
            m_nDRGContinueRem = 0;
            m_lstPrizeScore.Clear();

            SetGiveStep(0);
            m_clsSendScoreInfo = null;
            base.LogoutGear();
        }


        public CFdgPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            nAddCash = nAddCash / 10; //5드래곤에서는 10분의 1로 계산을 진행한다.

            if (m_lstScoreStepList[0].Count == 0 && m_lstScoreStepList[1].Count == 0)
            {
                m_nGiveCnt = RND.Next(2, 7);

                //5드래곤에서는 작은 점수를 자주 주어 유저들이 게임을 오랜시간동안 하게 한다.
                int nRate = CGlobal.GetGameRate(m_nGameCode);
                int nDstCash = (int)Math.Truncate((double)nAddCash * nRate / 100); 
                //점수를 주게된것을 2개통로로 나누어 계산을 한다.
                int[] lstDatCash = { (int)Math.Truncate((double)nDstCash / 2), (int)Math.Truncate((double)nDstCash / 2) };
                for (int i = 0; i < lstDatCash.Length; i++)
                {
                    MakeNormalScoreStepList(nAddCash, lstDatCash[i], m_lstScoreStepList[i]);
                }
            }
            

            return;
        }

        public override bool MakePrizeRoll()
        {
            throw new NotImplementedException();
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            if (m_clsSendScoreInfo == null)
                return;

            CUser user = CGlobal.GetUserByCode(m_nTakeUser);
            int nLevel = CGlobal.GetLevelByUserCode(user.m_nUserCode);

            CFdgScoreInfo scoreInfo = m_clsSendScoreInfo;

            if (scoreInfo.m_nFlag > 0)
            {
                nCash = scoreInfo.m_nScore;

                if (nCash > 3)
                {
                    m_prizeInfo.m_nPrizeCash -= nCash;
                    m_nRealPrizeCash += nCash;
                }
                
                if (scoreInfo.m_nFlag == 4)
                {
                    bool bret = ClearDrgPrizeInfo(); //잭팟이 끝났다.
                    if (bret)
                    {
                        //련타까지 모든 잭팟이 끝났다.
                        EndDRGJackPrize();
                        if (nLevel < 10 && user.m_nChargeCnt > 0)
                        {
                            //user.m_nUserWinCash += (m_nRealPrizeCash * 10);

                            CGlobal.GetGameEngineByCode(m_nGameCode).AddAppendPot(m_nRealPrizeCash * 10, m_nTakeUser);
                            CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(m_nRealPrizeCash * 10, m_nStep, m_nTakeUser);
                        }
                        m_nGearJack = 0;
                        SetGiveStep(0);
                        OnAddSlotCash(clsSocket, m_nRealPrizeCash);
                        m_nRealPrizeCash = 0;
                    }
                }
            }
            else
            {
                if (scoreInfo.m_nScore != nCash)
                {
                    if (scoreInfo.m_nScore * scoreInfo.m_nMini < nCash)
                    {
                        return;
                    }
                }

                if (scoreInfo.m_nFlag == -1)   //장군잭팟이 끝났을때이다.
                {
                    EndGeneralJackPrize(scoreInfo.m_nScore, scoreInfo.m_nMini);
                    if (user.m_nChargeCnt > 0)
                    {
                        user.m_nUserCash += (nCash * 10);
                        if(nLevel < 10)
                            CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash * 10, 4, m_nTakeUser);
                    }
                    else
                    {
                        user.m_nVirtualCash += (nCash * 10);
                    }
                    m_nAccuCash -= (nCash * 10);

                    CGlobal.SendUserInfoToClient(user);
                }
                else
                {
                    OnAddSlotCash(clsSocket, nCash);

                    if (nCash > 0)
                    {
                        if (nLevel < 10 && user.m_nChargeCnt > 0)
                        {
                            //user.m_nUserWinCash += (nCash * 10);
                            CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash * 10, m_nStep, m_nTakeUser);
                            CGlobal.GetGameEngineByCode(m_nGameCode).AddAppendPot(nCash * 10, m_nTakeUser);
                        }

                    }
                }

            }

            m_clsSendScoreInfo = null;
            CDataBase.SaveGearInfoToDB(this);
            CDataBase.SaveUserInfoToDB(user);
            clsSocket.SendGearInfo(this);
            CGlobal.SendUserInfoToClient(user);
        }

        public void OnExMoney()
        {
            int nCash = m_nSlotCash * 10;
            CUser clsUser = CGlobal.GetUserByCode(m_nTakeUser);
            CAgent clsAgent = CGlobal.GetAgentByCode(clsUser.m_nAgenCode);
            if (clsUser.m_nChargeCnt > 0)
            {
                string strNote = clsUser.m_strUserNick + "님이 사용하는 " + CGlobal.GetGameNameByGameCode(m_nGameCode) + m_nGearNum + "기대에서 머니전환";
                CDataBase.InsertCacheToDB(clsUser.m_nUserCode, nCash, 0, clsUser.m_nUserCash, clsUser.m_nUserCupn, clsUser.m_nUserCash + nCash, clsUser.m_nUserCupn, strNote);
                CDataBase.InsertBettingToDB(clsUser.m_nUserCode, nCash, clsUser.m_nUserCash, clsUser.m_nUserCash + nCash, 3, 1);
                clsUser.m_nUserCash += nCash;
            }
            else
            {
                clsUser.m_nVirtualCash += nCash;
            }

            CGlobal.GetGameEngineByCode(m_nGameCode).AddAppendPot(-nCash, m_nTakeUser);
            m_nSlotCash = 0;
            m_lstScoreStepList[0].Clear();
            m_lstScoreStepList[1].Clear();
            m_nGiveScore = 0;
            m_nGiveIndex = 0;

            CDataBase.SaveGearInfoToDB(this);
            CDataBase.SaveUserInfoToDB(clsUser);
            CGlobal.SendGearInfoToClient(this);
            CGlobal.SendUserInfoToClient(clsUser);
        }

        public override void OnCreateCoin(CUserSocket clsSocket)
        {
            throw new NotImplementedException();
        }

        public override void OnAddSlotCash(CUserSocket clsSocket, int nCash)
        {
            m_nSlotCash += nCash;
            if (m_nSlotCash < 0) m_nSlotCash = 0;

            m_nAccuCash -= nCash;
        }
        public override void OnGearStart(int nRun)
        {
            m_nGearRun = nRun;
        }

        public override void OnReconnect(CUserSocket clsSocket)
        {
            if (m_nGearRun == 0)
                return;
            if (m_clsSendScoreInfo == null)
            {
                OnStartSpin(clsSocket, m_nSpeedCash);
            }
            else
            {
                m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
                CScorePacket scorePacket = new CScorePacket();
                scorePacket.nGearCode = m_nGearCode;
                scorePacket.strPktData = JsonConvert.SerializeObject(m_clsSendScoreInfo);

                clsSocket.SendScoreInfo(scorePacket);
                OnBroadCastPrizeKind();
            }
        }


        public override void OnStartSpin(CUserSocket clsSocket, int nSpinCash)
        {
            if (m_nFreeTotalCnt > 0 && m_nFreeCurrentCnt <= m_nFreeTotalCnt)
            {
                m_nFreeCurrentCnt++;
            }
            else
            {
                if (m_nSlotCash < 0)
                    return;

                m_nSpeedCash = nSpinCash;
                m_nSpinCash += nSpinCash;
                OnAddSlotCash(clsSocket , - nSpinCash);
                

                CGlobal.CalcShowJackPot(nSpinCash * 4, 0);
                CGlobal.GetEngine(m_nGameCode).AddTotalPot(nSpinCash * 10, m_nTakeUser);
            }


            CFdgScoreInfo scoreInfo = null;
            if (m_prizeInfo != null)
            {
                if (m_nGearJack == 0 && nSpinCash > 0)
                {
                    if (m_prizeInfo.m_nCont == PRIZE_DRAGON)
                    {
                        m_nGearJack = 1;
                        if(m_nDrgSpinCash > 0)
                        {
                            //잭팟도중에 유저가 가입탈퇴되였다가 다시 들어온 상태이다.
                            m_nSpeedCash = m_nDrgSpinCash;
                            nSpinCash = m_nDrgSpinCash;
                        }
                        m_nDrgSpinCash = nSpinCash;
                        //5드래곤 잭팟예시돌기를 생성해서 내려보낸다.
                        scoreInfo = CreatePrizeSpin(nSpinCash);
                    }
                    else if (m_prizeInfo.m_nCont == PRIZE_GENERAL)
                    {
                        //장군잭팟을 준다.
                        scoreInfo = CreateGeneralPrize();
                    }

                }
                else if (m_lstPrizeScore.Count > 0)
                {
                    //잭팟정보를 얻어 내려보낸다.
                    scoreInfo = m_lstPrizeScore[0];
                    m_lstPrizeScore.Remove(scoreInfo);
                }
                else
                {
                    scoreInfo = MakeFDGScoreInfo(nSpinCash);
                }
            }
            else
            {
                //회전할 타일정보를 생성하여 내려보낸다.
                scoreInfo = MakeFDGScoreInfo(nSpinCash);
            }
            scoreInfo.m_nGearCode = m_nGearCode;
            m_clsSendScoreInfo = scoreInfo;

            CScorePacket scorePacket = new CScorePacket();
            scorePacket.nGearCode = m_nGearCode;
            scorePacket.strPktData = JsonConvert.SerializeObject(scoreInfo);
            clsSocket.SendScoreInfo(scorePacket);
            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
        }

        public override void OnEndPrizeCall()
        {
            throw new NotImplementedException();
        }

        public override void OnBroadCastPrizeInfo()
        {
            string strNick = string.Empty;
            if (m_nTakeUser > 0)
            {
                strNick = CGlobal.GetUserNickByCode(m_nTakeUser);
            }
            else if (m_nTakeRobot > 0)
            {
                strNick = CGlobal.GetRobotNickByCode(m_nTakeRobot);
            }

            CPrizeInfoBroadCast clsPrizeInfoPacket = new CPrizeInfoBroadCast();
            clsPrizeInfoPacket.m_strUserNick = strNick;
            clsPrizeInfoPacket.m_nGameCode = m_nGameCode;
            clsPrizeInfoPacket.m_nGearCode = m_nGearCode;
            clsPrizeInfoPacket.m_nGearNum = m_nGearNum;
            clsPrizeInfoPacket.m_nPrizeKind = m_prizeInfo.m_nCont;
            clsPrizeInfoPacket.m_nPrizeCash = m_nRealPrizeCash * 10;
            clsPrizeInfoPacket.m_nPrizeType = m_prizeInfo.m_nType;

            CGlobal.BroadCastPrizeInfo(clsPrizeInfoPacket);
        }

        public override void OnBroadCastPrizeKind()
        {

        }

        public override int RaiseJackPot1(int nJackCash)
        {
            return RaiseJackPot(nJackCash, 1);
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            return RaiseJackPot(nJackCash, 2);
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            return RaiseJackPot(nJackCash, 3);
        }

        private int RaiseJackPot(int nJackCash, int nStep)
        {
            if (m_nGearJack > 0)
                return nJackCash;
            if (m_prizeInfo != null)
                return nJackCash;
            if (nJackCash <= 0)
                return nJackCash;

            int nRnd = 0;

            int nMinCash = 0;
            int nMaxCash = 0;
            if (m_nSpeedCash == 25)
            {
                nMinCash = 10000;
                nMaxCash = 200000;
                nRnd = 1;
            }
            else if (m_nSpeedCash == 30)
            {
                nMinCash = 15000;
                nMaxCash = 300000;
                nRnd = 1;
            }
            else if (m_nSpeedCash == 50)
            {
                nMinCash = 20000;
                nMaxCash = 400000;
                nRnd = 2;
            }
            else if (m_nSpeedCash == 60)
            {
                nMinCash = 30000;
                nMaxCash = 600000;
                nRnd = 3;
            }
            else if (m_nSpeedCash == 125)
            {
                nMinCash = 50000;
                nMaxCash = 1000000;
                nRnd = 4;
            }
            else if (m_nSpeedCash == 150)
            {
                nMinCash = 70000;
                nMaxCash = 1500000;
                nRnd = 5;
            }
            else if (m_nSpeedCash == 250)
            {
                nMinCash = 100000;
                nMaxCash = 2500000;
                nRnd = 6;
            }
            else if (m_nSpeedCash == 300)
            {
                nMinCash = 150000;
                nMaxCash = 3000000;
                nRnd = 7;
            }
            else if (m_nSpeedCash == 500)
            {
                nMinCash = 200000;
                nMaxCash = 4000000;
                nRnd = 9;
            }
            else if (m_nSpeedCash == 600)
            {
                nMinCash = 400000;
                nMaxCash = 5000000;
                nRnd = 10;
            }

            if (nJackCash < nMinCash)
                return nJackCash;

            int nTempCash = nJackCash * nRnd / 10;
            int nPrizeCash = Convert.ToInt32(RND.Next(nTempCash, nMaxCash + 1) / 10000) * 10000;
            int nRetCash = nJackCash - nPrizeCash;

            m_prizeInfo = new CFdgPrizeInfo();
            m_prizeInfo.m_nCont = 1;
            m_prizeInfo.m_nPrizeCash = ((int)Math.Truncate((double)nPrizeCash / 1000)) * 100; //점수를 1000점단위로 주어야 하며 금액은 10분의 1이 되여야 한다.
            CDataBase.InsertNationalToDB(m_nGearCode, m_prizeInfo.m_nPrizeCash, nStep);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, m_prizeInfo.m_nPrizeCash * 10, 1, nStep, 0);
            SetGiveStep(nStep);
            nRetCash += (nPrizeCash - m_prizeInfo.m_nPrizeCash * 10);

            return nRetCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
            int nPrizeCash = ((int)Math.Truncate((double)nJackCash / 1000)) * 100;

            if (nPrizeCash > 50 * 10000)
                nPrizeCash = 50 * 10000;
            if (robot != null)
            {
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, robot.m_nRbCode, m_nGearCode, nPrizeCash * 10, 1, -1, 1);
                robot.PlayFdgPrize(nPrizeCash);
            }
                
            return nJackCash - nPrizeCash * 10;
        }

        public bool SetFdgPrizeInfo(int nCont, int nPrizeCash, int nJackType)
        {
            if (m_nGearJack > 0 || m_prizeInfo != null)
                return false;
            if (m_prizeInfo != null)
                return false;

            if (nPrizeCash == 0 && nCont == 1)
            {
                m_prizeInfo = null;
                return false;
            }

            m_prizeInfo = new CFdgPrizeInfo();
            m_prizeInfo.m_nCont = nCont;

            if (nCont == 1)
            {
                if (m_nTakeUser == 0 && m_nTakeRobot > 0)
                {
                    //로봇이 사용한다면
                    CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                    robot.PlayFdgPrize(((int)Math.Truncate((double)nPrizeCash / 1000)) * 100);
                }
                else
                {
                    m_prizeInfo.m_nPrizeCash = ((int)Math.Truncate((double)nPrizeCash / 1000)) * 100;
                }
            }
            else
            {
                if (nJackType == 1)
                {
                    m_prizeInfo.m_nPrizeCash = ((int)Math.Truncate((double)CGlobal.GetJackPot().m_nMini / 1000)) * 100;
                    m_nMiniCount++;
                }
                else if (nJackType == 2)
                {
                    m_prizeInfo.m_nPrizeCash = ((int)Math.Truncate((double)CGlobal.GetJackPot().m_nMinor / 1000)) * 100;
                    m_nMinorCount++;
                }
                else if (nJackType == 3)
                {
                    m_prizeInfo.m_nPrizeCash = ((int)Math.Truncate((double)CGlobal.GetJackPot().m_nMajor / 1000)) * 100;
                    m_nMajorCount++;
                }
                else if (nJackType == 4)
                {
                    m_prizeInfo.m_nPrizeCash = ((int)Math.Truncate((double)CGlobal.GetJackPot().m_nGrand / 1000)) * 100;
                    m_nGrandCount++;
                }
                m_prizeInfo.m_nType = nJackType;

                if (m_nTakeRobot > 0 && m_nTakeUser == 0)
                {
                    //로봇이 사용한다면
                    EndGeneralJackPrize(m_prizeInfo.m_nPrizeCash, nJackType);
                }
            }

            m_dtLastJackTime = CMyTime.GetMyTime();
            m_nLastJackCash = m_prizeInfo.m_nPrizeCash;
            if (m_prizeInfo.m_nPrizeCash > m_nTopJackCash)
                m_nTopJackCash = m_prizeInfo.m_nPrizeCash;

            CDataBase.SaveGearInfoToDB(this);


            return true;
        }

        //들어온 점수에 맞는 점수단계리스트를 생성한다.
        private void MakeNormalScoreStepList(int nAddCash, int nDstCash, List<CFdgScoreStep> lstScoreStep, bool bPrize = false)
        {
            while (nDstCash >= 5)
            {
                List<CFdgScoreTableInfo> lstScoreList = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore <= Math.Min(nDstCash, 800));
                CFdgScoreStep scoreStep = new CFdgScoreStep();
                scoreStep.m_nSocreCash = CGlobal.RandomSelect(lstScoreList).nScore;
                if (bPrize == false)
                    scoreStep.m_nRaiseCash = RND.Next(1, nAddCash / 25) * 25;
                else
                    scoreStep.m_nRaiseCash = RND.Next(1, nAddCash);

                if (lstScoreStep.Exists(value => value.m_nRaiseCash == scoreStep.m_nRaiseCash))
                {
                    lstScoreStep.Find(value => value.m_nRaiseCash == scoreStep.m_nRaiseCash).m_nSocreCash += scoreStep.m_nSocreCash;
                }
                else
                {
                    lstScoreStep.Add(scoreStep);
                }

                nDstCash -= scoreStep.m_nSocreCash;
            }

            CFdgScoreStep lastScoreStep = new CFdgScoreStep();
            lastScoreStep.m_nSocreCash = 0;
            lastScoreStep.m_nRaiseCash = nAddCash;
            lstScoreStep.Add(lastScoreStep);

        }

        //잭팟예시스핀생성함수
        private CFdgScoreInfo CreatePrizeSpin(int nSpinCash)
        {
            int nExtra = 0;
            if (nSpinCash == 25 || nSpinCash == 50 || nSpinCash == 125 || nSpinCash == 250 || nSpinCash == 500)
                nExtra = 1;
            else if (nSpinCash == 30 || nSpinCash == 60 || nSpinCash == 150 || nSpinCash == 300 || nSpinCash == 600)
                nExtra = 2;

            //배수를 확정한다.
            int nMulti = 1;
            if (nSpinCash <= 30)
                nMulti = 1;
            else if (nSpinCash <= 60)
                nMulti = 2;
            else if (nSpinCash <= 150)
                nMulti = 5;
            else if (nSpinCash <= 300)
                nMulti = 10;
            else if (nSpinCash <= 600)
                nMulti = 20;


            CFdgScoreInfo scoreInfo = null;

            if (m_prizeInfo.m_nCont == PRIZE_DRAGON)
            {
                scoreInfo = CreateDragonPrize(nMulti, nExtra);
            }


            m_lstPrizeScore.Clear();
            return scoreInfo;
        }

        private CFdgScoreInfo CreateGeneralPrize()
        {
            CFdgScoreInfo scoreInfo = new CFdgScoreInfo();
            MakeOneEmptyRoll(scoreInfo);

            scoreInfo.m_nFlag = -1; //장군잭팟
            scoreInfo.m_nScore = m_prizeInfo.m_nPrizeCash; //잭팟점수
            scoreInfo.m_nMini = m_prizeInfo.m_nType;

            m_prizeInfo = null;
            return scoreInfo;
        }

        private CFdgScoreInfo MakeFDGScoreInfo(int nSpinCash)
        {
            m_nGiveIndex++;
            //배수를 확정한다.
            int nMulti = 1;
            if (nSpinCash <= 30)
                nMulti = 1;
            else if (nSpinCash <= 60)
                nMulti = 2;
            else if (nSpinCash <= 150)
                nMulti = 5;
            else if (nSpinCash <= 300)
                nMulti = 10;
            else if (nSpinCash <= 600)
                nMulti = 20;


            if (m_lstScoreStepList[0].Count == 0 && m_lstScoreStepList[1].Count == 0)
            {
                m_nSpinCash = nSpinCash;
                int nAppendCash = (m_nSlotCash + nSpinCash) * 10;  //기계에 금액을 충진하였을때 한번 도는 금액을 소비한다음에 새로운 점수렬을 계산하려 한다.
                MakeNormalScorelRoll(nAppendCash);     //계산된점수배렬이 없다면 새로운 점수배렬을 계산하여야 한다.
            }


            int nSumCash = 0;
            CFdgScoreInfo scoreInfo = new CFdgScoreInfo();

            //점수를 주어야 한다.
            nSumCash += m_lstScoreStepList[0].FindAll(value => value.m_nRaiseCash <= m_nSpinCash).Sum(value=>value.m_nSocreCash);
            m_lstScoreStepList[0].RemoveAll(value => value.m_nRaiseCash <= m_nSpinCash);
            nSumCash += m_lstScoreStepList[1].FindAll(value => value.m_nRaiseCash <= m_nSpinCash).Sum(value=>value.m_nSocreCash);
            m_lstScoreStepList[1].RemoveAll(value => value.m_nRaiseCash <= m_nSpinCash);

            //주어야 하는 점수를 계산한다.
            m_nGiveScore += nSumCash;

            if (m_nSlotCash < nSpinCash)
            {
                nSumCash = 0;
                nSumCash += m_lstScoreStepList[0].Sum(value => value.m_nSocreCash);
                m_lstScoreStepList[0].Clear();
                nSumCash += m_lstScoreStepList[1].Sum(value => value.m_nSocreCash);
                m_lstScoreStepList[1].Clear();

                m_nGiveScore += nSumCash;
            }

            if (m_nGiveIndex >= m_nGiveCnt || m_nSlotCash < nSpinCash)
            {
                m_nGiveCnt = RND.Next(1, 10);
                m_nGiveIndex = 0;

                List<int> lstScore = new List<int>();
                lstScore.Add(m_nGiveScore);
                scoreInfo.m_nGearCode = m_nGearCode;
                scoreInfo.m_nScore = m_nGiveScore;
                scoreInfo.m_lstScore = lstScore;
                scoreInfo.m_nMulti = nMulti;
                scoreInfo.m_nMini = RND.Next(0, 11);
               
                int nRemCash = MakeOneScoreRoll(scoreInfo);
                if (scoreInfo.m_nScore > 0)
                    m_nGiveScore = 0;
               
            }
            else
            {
                MakeOneEmptyRoll(scoreInfo);
                scoreInfo.m_nScore = 0;
                scoreInfo.m_lstScore = new List<int>();
                scoreInfo.m_nMulti = nMulti;
            }

            
            

            return scoreInfo;
        }

        private bool ClearDrgPrizeInfo()
        {
            if (m_nDRGContinueRem == 0)
            {
                OnBroadCastPrizeInfo();
                m_prizeInfo = null;

                return true;
            }
            
            m_lstPrizeScore.Clear();
            m_nFreeTotalCnt = 0;
            m_nFreeCurrentCnt = 0;

            return false;
        }

        private void EndDRGJackPrize()
        {
            string strMessage = MakeCongration("드래곤잭팟", m_nRealPrizeCash * 10);
            CGlobal.SendNoticeBroadCast(strMessage);
            m_nDrgSpinCash = 0;
            //m_bDrgPrize = false;
        }

        private void EndGeneralJackPrize(int nPrizeCash, int nType)
        {
            string strNick = string.Empty;
            if (m_nTakeUser > 0)
            {
                CUser user = CGlobal.GetUserByCode(m_nTakeUser);
                strNick = user.m_strUserNick;
            }
            else if (m_nTakeRobot > 0)
            {
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                strNick = robot.m_strRbNick;
            }


            int nShowPrizeCash = Convert.ToInt32(nPrizeCash * 10 / 10000);
            string strMessage = "[C][FFFFFF]운영자: ";
            string strType = string.Empty;
            if (nType == 4)
            {
                strType = "Grand";
                strMessage += "축하드립니다! " + strNick + " 사장님 [C][EC0DB0]Grand Jackpot[-]에 당첨되셨습니다";
            }
            else if (nType == 3)
            {
                strType = "Major";
                strMessage += "축하드립니다! " + strNick + " 사장님 [C][EC0DB0]Major Jackpot[-]에 당첨되셨습니다";
            }
            else if (nType == 2)
            {
                strType = "Minor";
                strMessage += "축하드립니다! " + strNick + " 사장님 [C][EC0DB0]Minor Jackpot[-]에 당첨되셨습니다";
            }
            else if (nType == 1)
            {
                strType = "Mini";
                strMessage += "축하드립니다! " + strNick + " 사장님 [C][EC0DB0]Mini Jackpot[-]에 당첨되셨습니다";
            }

            CGlobal.SendNoticeBroadCast(strMessage);
            CGlobal.CalculateJackPotCash(nShowPrizeCash * 10000, nType);
            SetGiveStep(0);

            CPrizeInfoBroadCast clsPrizeInfoPacket = new CPrizeInfoBroadCast();
            if (m_nTakeUser > 0)
                clsPrizeInfoPacket.m_strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);
            else if (m_nTakeRobot > 0)
                clsPrizeInfoPacket.m_strUserNick = CGlobal.GetRobotNickByCode(m_nTakeRobot);
            clsPrizeInfoPacket.m_nGameCode = m_nGameCode;
            clsPrizeInfoPacket.m_nGearCode = m_nGearCode;
            clsPrizeInfoPacket.m_nGearNum = m_nGearNum;
            clsPrizeInfoPacket.m_nPrizeKind = 2;
            clsPrizeInfoPacket.m_nPrizeCash = nShowPrizeCash * 10000;
            clsPrizeInfoPacket.m_nPrizeType = nType;

            CGlobal.BroadCastPrizeInfo(clsPrizeInfoPacket);
        }

        private CFdgScoreInfo CreateDragonPrize(int nMulti, int nExtra)
        {
            //동전3개를 맞추어준다.
            CFdgScoreTableInfo scoreTable = null;
            int nRnd = RND.Next(10);
            if (m_prizeInfo.m_nPrizeCash <= 20000)
            {
                scoreTable = m_lstFDGScoreTable.ToList().Find(value => value.nTile == 12 && value.nCount == 3);
            }
            else if (m_prizeInfo.m_nPrizeCash <= 50000)
            {
                if (nRnd < 8)
                    scoreTable = m_lstFDGScoreTable.ToList().Find(value => value.nTile == 12 && value.nCount == 3);
                else
                    scoreTable = m_lstFDGScoreTable.ToList().Find(value => value.nTile == 12 && value.nCount == 4);
            }
            else
            {
                if (nRnd < 4)
                    scoreTable = m_lstFDGScoreTable.ToList().Find(value => value.nTile == 12 && value.nCount == 3);
                else if (nRnd < 7)
                    scoreTable = m_lstFDGScoreTable.ToList().Find(value => value.nTile == 12 && value.nCount == 4);
                else
                    scoreTable = m_lstFDGScoreTable.ToList().Find(value => value.nTile == 12 && value.nCount == 5);
            }

            CFdgScoreInfo scoreInfo = new CFdgScoreInfo();
            MakeOneEmptyRoll(scoreInfo);

            List<CTile> lstTile = scoreInfo.m_lstTile;
            for (int i = 0; i < 5; i++)
            {
                if (i < scoreTable.nCount)
                {
                    if (lstTile.Exists(value => value.m_nCol == i && value.m_nTile == 12))
                    {
                        lstTile.Find(value => value.m_nCol == i && value.m_nTile == 12).m_nAct = 1;
                        continue;
                    }
                    else
                    {
                        int nRow = RND.Next(3);
                        lstTile.Find(value => value.m_nRow == nRow && value.m_nCol == i).m_nTile = 12;
                        lstTile.Find(value => value.m_nRow == nRow && value.m_nCol == i).m_nAct = 1;
                    }
                }
                else
                {
                    if (lstTile.Exists(value => value.m_nCol == i && value.m_nTile == 12))
                    {
                        int nTile = RND.Next(1, 11);
                        while (true)
                        {
                            if (lstTile.Exists(value => value.m_nCol == i && value.m_nTile == nTile))
                            {
                                nTile = RND.Next(1, 11);
                                continue;
                            }
                            if (i > 0)
                            {
                                if (lstTile.Exists(value => value.m_nCol == i - 1 && value.m_nTile == nTile))
                                {
                                    nTile = RND.Next(1, 11);
                                    continue;
                                }

                            }
                            break;
                        }
                        lstTile.Find(value => value.m_nCol == i && value.m_nTile == 12).m_nTile = nTile;
                    }
                }
            }
            m_nDRGMulti = nMulti;
            m_nDRGExtra = nExtra;

            scoreInfo.m_nMulti = nMulti;
            scoreInfo.m_nScore = scoreTable.nScore * nMulti;
            scoreInfo.m_nFlag = m_nDRGExtra;

            return scoreInfo;

        }

        //빈돌기를 생성하는함수
        private void MakeOneEmptyRoll(CFdgScoreInfo scoreInfo)
        {
            int[] lstTileKind = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };  //타일종류
            List<CTile> lstWinTile = scoreInfo.m_lstTile.FindAll(value => value.m_nAct > 0);

            List<int>[] lstTile = { new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>() };

            for (int nCol = 0; nCol < 5; nCol++)
            {
                if (nCol == 0 || nCol == 4)
                {
                    //첫줄과 마지막줄이라면
                    List<int> lstTileKind0 = lstTileKind.ToList().FindAll(value => !lstWinTile.Exists(value1 => value1.m_nTile == value) && value != 0);
                    for (int nRow = 0; nRow < 3; nRow++)
                    {
                        if (scoreInfo.m_lstTile.Exists(value => value.m_nCol == nCol && value.m_nRow == nRow))
                        {
                            int nTileKind = scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol && value.m_nRow == nRow).m_nTile;
                            lstTileKind0.Remove(nTileKind);
                        }
                        else
                        {
                            int nTileKind = lstTileKind0[RND.Next(lstTileKind0.Count)];
                            CTile tile = new CTile(nTileKind, nRow, nCol);
                            scoreInfo.m_lstTile.Add(tile);
                            lstTileKind0.Remove(nTileKind);
                        }
                    }
                }
                else if (nCol == 1 || nCol == 3)
                {
                    //두번째 세번째줄이라면
                    List<int> lstTileKind1 = lstTileKind.ToList().FindAll(value => !lstWinTile.Exists(value1 => value1.m_nTile == value));
                    if (lstWinTile.Count > 0)
                    {
                        //당첨된파일에서 채워넣기이라면 첫번째줄과 달라야 하며 조커가 없어야 한다.
                        lstTileKind1 = lstTileKind1.FindAll(value => !scoreInfo.m_lstTile.Exists(value1 => value1.m_nTile == value) && value != 0);
                    }
                    for (int nRow = 0; nRow < 3; nRow++)
                    {
                        if (scoreInfo.m_lstTile.Exists(value => value.m_nCol == nCol && value.m_nRow == nRow))
                        {
                            int nTileKind = scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol && value.m_nRow == nRow).m_nTile;
                            lstTileKind1.Remove(nTileKind);
                        }
                        else
                        {
                            int nTileKind = lstTileKind1[RND.Next(lstTileKind1.Count)];
                            lstTileKind1.Remove(nTileKind);
                            CTile tile = new CTile(nTileKind, nRow, nCol);
                            scoreInfo.m_lstTile.Add(tile);
                        }
                    }

                }
                else if (nCol == 2)
                {
                    //세번째줄이라면
                    int nFlag = 0; //두번째둘이 첫줄과 같은 타일이 하나라도 있거나 조커가 있는가를 검사
                    List<CTile> lstTile0 = scoreInfo.m_lstTile.FindAll(value => value.m_nCol == 0); //첫줄에 선택된 타일
                    List<CTile> lstTile1 = scoreInfo.m_lstTile.FindAll(value => value.m_nCol == 1); //두번째줄에 선택된 타일

                    if (lstTile1.Exists(value => value.m_nTile == 0)) //두번째줄에 조커가 있는가를 검사
                    {
                        nFlag = 1;
                    }
                    else if (lstTile1.Exists(value1 => lstTile0.Exists(value0 => value0.m_nTile == value1.m_nTile)))
                    {
                        //첫줄과 두번째줄에서 같은 타일이 있는가를 검사한다.
                        nFlag = 2;
                    }
                    List<int> lstTileKind2 = null;  //세번째줄에 배치할 타일종류리스트
                    if (nFlag == 0)
                    {
                        //첫줄과 두번째줄에 일치되는 타일이 없다면 세번째줄은 임의의 타일이여도 된다.
                        lstTileKind2 = lstTileKind.ToList();
                    }
                    else if (nFlag == 1)
                    {
                        //두번째줄에 조커가 있을때는 첫줄과 달라야 한다.
                        lstTileKind2 = lstTileKind.ToList().FindAll(value => !lstTile0.Exists(value0 => value0.m_nTile == value) && value != 0);
                    }
                    else if (nFlag == 2)
                    {
                        //첫줄과 두번째줄에 일치되는 타일이 있다면 세번째줄 첫줄 또는 두번째줄과 달라야 한다.
                        if (RND.Next(2) == 0)
                            lstTileKind2 = lstTileKind.ToList().FindAll(value => !lstTile0.Exists(value0 => value0.m_nTile == value) && value != 0);
                        else
                            lstTileKind2 = lstTileKind.ToList().FindAll(value => !lstTile1.Exists(value1 => value1.m_nTile == value) && value != 0);
                    }

                    lstTileKind2 = lstTileKind2.FindAll(value => !lstWinTile.Exists(value1 => value1.m_nTile == value));  //당첨된 타일들을 빼야 한다.

                    for (int nRow = 0; nRow < 3; nRow++)
                    {
                        if (scoreInfo.m_lstTile.Exists(value => value.m_nCol == nCol && value.m_nRow == nRow))
                        {
                            int nTileKind = scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol && value.m_nRow == nRow).m_nTile;
                            lstTileKind2.Remove(nTileKind);
                        }
                        else
                        {
                            int nTileKind = lstTileKind2[RND.Next(lstTileKind2.Count)];
                            lstTileKind2.Remove(nTileKind);
                            CTile tile = new CTile(nTileKind, nRow, nCol);
                            scoreInfo.m_lstTile.Add(tile);
                        }
                    }
                }
            }
        }

        //점수돌기를 생성하는 함수
        private int MakeOneScoreRoll(CFdgScoreInfo scoreInfo)
        {
            int nRemCash = 0;
            int nMulti = scoreInfo.m_nMulti;
            List<int> lstScore = new List<int>();
            for (int i = 0; i < scoreInfo.m_lstScore.Count; i++)
            {
                int nScore = scoreInfo.m_lstScore[i];
                lstScore.Add(nScore);
            }

            int nIndex = 1;    //당첨된 점수 인덱스
            List<int> lstTileKind = new List<int>();  //당첨된 타일종류리스트


            //점수타일들을 생성하여 배치한다.
            for (int i = 0; i < lstScore.Count; i++)
            {
                int nRemScore;
                int nDstScore = Math.DivRem(lstScore[i], nMulti, out nRemScore); //배수로 나누고 나머지를 다음 점수단계에 더해주어야 한다.

                if (nRemScore > 0)
                {
                    nRemCash += nRemScore;
                    scoreInfo.m_nScore -= nRemScore;
                }

                if (nDstScore < 5)                      //배수로 나눈 점수가 5보다 작으면 점수를 주지 않는다.
                {
                    nRemScore = nDstScore * nMulti;     //배수로 나눈 점수가 5보다 작으면 점수자체를 다음 단계에 더해주어야 한다.
                    nRemCash += nRemScore;
                    scoreInfo.m_nScore -= nRemScore;

                    scoreInfo.m_lstScore.Remove(scoreInfo.m_lstScore.Find(value => value == lstScore[i]));   //점수객체에서 해당한 점수를 삭제하여야 한다.
                    continue;
                }

                bool bCheck = false;        //주작, 사자, 물고기, 거부기, 책이 한데 나오지 않게 하기 위한 변수
                if (lstTileKind.Exists(value => value > 0 && value < 5))
                    bCheck = true;
                int nScore = 0;
                if (bCheck)
                    nScore = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore <= nDstScore && value.nTile != 12 && value.nTile > 5).Max(value => value.nScore);  //실지주는 점수
                else
                    nScore = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore <= nDstScore && value.nTile != 12).Max(value => value.nScore);  //실지주는 점수

                nRemScore = (nDstScore - nScore) * nMulti;
                if (nRemScore > 0)
                {
                    nRemCash += nRemScore;
                    scoreInfo.m_nScore -= nRemScore;
                }

                List<CFdgScoreTableInfo> lstScoreTable;
                //점수테이블을 선택한다.
                if (bCheck)
                {
                    lstScoreTable = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore == nScore && value.nTile != 12 && value.nTile > 5)
                                                                                    .FindAll(value => !lstTileKind.Exists(value1 => value1 == value.nTile));
                }
                else
                {
                    lstScoreTable = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore == nScore && value.nTile != 12)
                                                                                    .FindAll(value => !lstTileKind.Exists(value1 => value1 == value.nTile));
                }



                if (lstScoreTable.Count == 0)
                {
                    nRemCash += nScore;
                    scoreInfo.m_nScore -= nScore;
                    continue;
                }

                CFdgScoreTableInfo scoreTable = CGlobal.RandomSelect(lstScoreTable);
                lstTileKind.Add(scoreTable.nTile);  //선택된 타일을 보관한다.

                //선택된 점수테이블에 맞게 타일을 배치한다.
                for (int nCol = 0; nCol < scoreTable.nCount; nCol++)
                {
                    int nRow = RND.Next(3);
                    while (true)
                    {
                        if (scoreInfo.m_lstTile.Exists(value => value.m_nCol == nCol && value.m_nRow == nRow))
                            nRow = RND.Next(3);
                        else
                            break;
                    }

                    //타일을 생성하여 추가한다.
                    CTile tile = new CTile(scoreTable.nTile, nRow, nCol);
                    tile.m_nAct = nIndex;
                    scoreInfo.m_lstTile.Add(tile);
                }
                nIndex++;


            }

            if (scoreInfo.m_nFlag != 3)
            {

                //조커타일을 배치하겠는가를 결정하고 배치한다.
                for (int nCol = 1; nCol < 4; nCol++)
                {
                    if (scoreInfo.m_lstTile.Exists(value => value.m_nCol == nCol)) //렬에 맞은 타일이 있는가를 검사
                    {
                        //렬에 맞은 타일이 있다면 조커를 배치하겠는가를 결정한다.
                        int nRnd = RND.Next(5);
                        if (nRnd == 3)
                        {
                            //조커타일을 배치한다.
                            InsertJockerToScoreTiles(scoreInfo, nCol);

                            if (nCol == 1)
                                break;
                        }
                    }
                }
            }
            else if (scoreInfo.m_nFlag == 3)
            {
                if (scoreInfo.m_nFlower == 1)
                {
                    //프리돌기이면서 련꽃배당이라면 무저건 조커가 끼워야 한다.
                    int nColCnt = scoreInfo.m_lstTile.Max(value => value.m_nCol);
                    if (nColCnt == 4)
                        nColCnt = 3;
                    int nJockerCol = RND.Next(1, nColCnt + 1);
                    //조커타일을 배치한다.
                    InsertJockerToScoreTiles(scoreInfo, nJockerCol);
                    if (nJockerCol > 1)
                    {
                        for (int nCol = 2; nCol <= nColCnt; nCol++)
                        {
                            if (!scoreInfo.m_lstTile.Exists(value => value.m_nTile == 0 && value.m_nCol == nCol))
                            {
                                //렬에 조커가 없다면 조커를 배치하겠는가를 결정한다.
                                int nRnd = RND.Next(5);
                                if (nRnd == 3)
                                {
                                    //조커타일을 배치한다.
                                    InsertJockerToScoreTiles(scoreInfo, nCol);
                                }
                            }
                        }
                    }
                }
            }

            //점수타일이 놓이지 않은 남은 부분을 임의의 타일로 채워넣어야 한다.
            MakeOneEmptyRoll(scoreInfo);

            return nRemCash;
        }

        private void InsertJockerToScoreTiles(CFdgScoreInfo scoreInfo, int nCol)
        {
            //조커타일을 배치한다.
            int nCnt = scoreInfo.m_lstTile.Count(value => value.m_nCol == nCol);
            if (nCnt == 1)  //렬에 맞은 타일이 1개이라면 맞은 타일을 조커로 바꾸면 된다.
            {
                CTile tile = scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol);
                tile.m_nTile = 0;   //조커를 배치하였다.
            }
            else if (nCnt == 2)
            {
                if (RND.Next(2) == 0)
                {
                    //맞은 타일들을 합해서 표시한다.
                    List<CTile> lstTile = scoreInfo.m_lstTile.FindAll(value => value.m_nCol == nCol);
                    lstTile[0].m_nTile = 0;
                    lstTile[0].m_nAct = 4;
                    //두번째로 맞은 타일을 삭제한다.
                    scoreInfo.m_lstTile.Remove(lstTile[1]);
                }
                else
                {
                    List<CTile> lstTile = scoreInfo.m_lstTile.FindAll(value => value.m_nCol == nCol);
                    lstTile[0].m_nTile = 0;
                    lstTile[0].m_nAct = 1;
                }
            }
            else if (nCnt == 3)
            {
                //맞은 타일들을 합해서 표시한다.
                List<CTile> lstTile = scoreInfo.m_lstTile.FindAll(value => value.m_nCol == nCol);
                lstTile[0].m_nTile = 0;
                lstTile[0].m_nAct = 4;
                //두번째로 맞은 타일을 삭제한다.
                scoreInfo.m_lstTile.Remove(lstTile[1]);
                if (RND.Next(2) == 0)
                {
                    //세번째로 맞은 타일을 삭제한다.
                    scoreInfo.m_lstTile.Remove(lstTile[2]);
                }
            }
        }

        //나머지점수를 다음단계에 더해주는 함수
        private void AddRemScoreToNextStep(int nRemScore)
        {
            for (int j = 0; j < 2; j++)
            {
                if (m_lstScoreStepList[j].Count > 0)
                {
                    m_lstScoreStepList[j][0].m_nSocreCash += nRemScore;
                    nRemScore = 0;
                    break;
                }
            }
        }

        //드래곤잭팟 프리스핀돌기시작
        public void OnStartFreeSpin(int nDrg)
        {
            //if (m_bDrgPrize)
            //    return;

            //m_bDrgPrize = true;

            int[] lstMulti = { 1, 1, 1 }; //배수
            if (nDrg == 0)
            {
                m_lstMulti[0] = 2; m_lstMulti[1] = 3; m_lstMulti[2] = 5;
                m_nFreeTotalCnt = m_nDRGExtra == 1 ? 20 : 25;
            }
            else if (nDrg == 1)
            {
                m_lstMulti[0] = 3; m_lstMulti[1] = 5; m_lstMulti[2] = 8;
                m_nFreeTotalCnt = m_nDRGExtra == 1 ? 15 : 20;
            }
            else if (nDrg == 2)
            {
                m_lstMulti[0] = 5; m_lstMulti[1] = 8; m_lstMulti[2] = 10;
                m_nFreeTotalCnt = m_nDRGExtra == 1 ? 10 : 15;
            }
            else if (nDrg == 3)
            {
                m_lstMulti[0] = 8; m_lstMulti[1] = 10; m_lstMulti[2] = 15;
                m_nFreeTotalCnt = m_nDRGExtra == 1 ? 8 : 13;
            }
            else if (nDrg == 4)
            {
                m_lstMulti[0] = 10; m_lstMulti[1] = 15; m_lstMulti[2] = 30;
                m_nFreeTotalCnt = m_nDRGExtra == 1 ? 5 : 10;
            }

            if (m_nDRGContinueRem > 0)
                m_nDRGContinueRem--;


            int nPrizeCnt = 0;
            if (m_nFreeTotalCnt == 5)
                nPrizeCnt = m_nFreeTotalCnt - 3;
            else
                nPrizeCnt = (int)Math.Truncate((double)(m_nFreeTotalCnt / 2));

            int nContinueCnt = 0;   //련타회수
            if(m_prizeInfo == null)
            {
                m_prizeInfo = new CFdgPrizeInfo();
                m_prizeInfo.m_nCont = 1;
                m_prizeInfo.m_nPrizeCash = 0;
            }
            int nRemCash = MakePrizeScorelRoll(m_prizeInfo.m_nPrizeCash, nPrizeCnt);  //드래곤잭팟점수리스트를 만든다.

            int nTempCnt = 0;
            while(nRemCash < 0)  //무한순환이 빠지는것 같다.
            {
                if (m_prizeInfo == null)
                {
                    m_prizeInfo = new CFdgPrizeInfo();
                    m_prizeInfo.m_nCont = 1;
                    m_prizeInfo.m_nPrizeCash = 0;
                }
                //0보다 작으면 오유이므로 다시 만든다.
                m_lstPrizeScore.Clear();
                nRemCash = MakePrizeScorelRoll(m_prizeInfo.m_nPrizeCash, nPrizeCnt);  //드래곤잭팟점수리스트를 만든다.
                nTempCnt++;
                if (nTempCnt >= 5)
                {
                    CGlobal.Log("5드래곤잭팟 생성오유 " + nRemCash.ToString("N0"));
                    break;
                }
            }

            if (nRemCash > 125 * m_nDRGMulti)
            {
                //련타를 주어야 한다. 련타는 줄수있는 금액의 최대금액보다 크다면 2련타를 주고 그나머지인 경우는 1련타를 준다.
                int nMulti = m_lstMulti[2] * m_nDRGMulti;
                if (nMulti * 800 < nRemCash)
                {
                    //2련타를 주어야 한다.
                    nContinueCnt = 2;
                }
                else
                {
                    //1련타를 주어야 한다.
                    nContinueCnt = 1;
                }
            }


            if (m_nDRGContinueRem == 0)
            {
                for (int i = 0; i < nContinueCnt; i++)
                {
                    List<CFdgScoreInfo> lstEmptyScoreInfo = m_lstPrizeScore.FindAll(value => value.m_nScore == 0);
                    if (lstEmptyScoreInfo.Count <= 1)
                        break;

                    m_nDRGContinueRem++;
                    //련타를 설정한다.
                    CFdgScoreInfo scoreInfo = CreateDragonPrize(m_nDRGMulti, m_nDRGExtra);
                    scoreInfo.m_nFlag = 5;      //연타예시이다.
                    int nIndex = RND.Next(1, lstEmptyScoreInfo.Count);
                    scoreInfo.m_nMini = lstEmptyScoreInfo[nIndex].m_nMini;
                    nIndex = m_lstPrizeScore.FindIndex(value => value.m_nMini == lstEmptyScoreInfo[nIndex].m_nMini);
                    m_lstPrizeScore.RemoveAt(nIndex);
                    m_lstPrizeScore.Insert(nIndex, scoreInfo);
                }
            }

            if (m_nDRGContinueRem == 0)
            {
                if (nRemCash > 0)
                {
                    MakeTwoBook(nRemCash);
                }
            }

            //프리돌기끝신호를 보내야 한다.
            m_lstPrizeScore[m_lstPrizeScore.Count - 1].m_nFlag = 4;
            if (m_lstPrizeScore[m_lstPrizeScore.Count - 1].m_nScore == 0)
                m_lstPrizeScore[m_lstPrizeScore.Count - 1].m_nScore = 3; //점수가 없다면 효과에 걸리게 하기 위하여 3점을 준다.


            //m_nGearJack = 1; //기계를 잭팟상태로 설정
        }

        private void MakeTwoBook(int nScore)
        {
            //nScore = Convert.ToInt32(nScore / 10) * 10;

            //잭팟을 주고 남은 점수가 있다면 복책2권을 주어야 한다.
            List<CFdgScoreInfo> lstEmptyScoreInfo = m_lstPrizeScore.FindAll(value => value.m_nScore == 0);
            int nIndex = RND.Next(1, lstEmptyScoreInfo.Count);
            lstEmptyScoreInfo[nIndex].m_nFlower = -3; //복책2권
            lstEmptyScoreInfo[nIndex].m_nScore = nScore;
            List<CTile> lstTile = lstEmptyScoreInfo[nIndex].m_lstTile;
            if (!lstTile.Exists(value => value.m_nTile == 5 && value.m_nCol == 0))
            {
                int nRow = RND.Next(0, 3);
                lstTile.Find(value => value.m_nCol == 0 && value.m_nRow == nRow).m_nTile = 5;
                lstTile.Find(value => value.m_nCol == 0 && value.m_nRow == nRow).m_nAct = 1;
            }
            else
            {
                lstTile.Find(value => value.m_nTile == 5 && value.m_nCol == 0).m_nAct = 1;
            }

            if (!lstTile.Exists(value => value.m_nTile == 5 && value.m_nCol == 4))
            {
                int nRow = RND.Next(0, 3);
                lstTile.Find(value => value.m_nCol == 4 && value.m_nRow == nRow).m_nTile = 5;
                lstTile.Find(value => value.m_nCol == 4 && value.m_nRow == nRow).m_nAct = 1;
            }
            else
            {
                lstTile.Find(value => value.m_nTile == 5 && value.m_nCol == 4).m_nAct = 1;
            }

            if (lstTile.Exists(value => value.m_nTile == 5 && value.m_nCol == 1))
            {
                int nTile = 0;
                lstTile = lstTile.FindAll(value => value.m_nCol == 1);
                while (true)
                {
                    nTile = RND.Next(1, 13);
                    if (nTile == 5)
                        continue;
                    bool bFlag = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (nTile == lstTile[i].m_nTile)
                        {
                            bFlag = false;
                            break;
                        }
                    }

                    if (bFlag)
                        break;
                }
                lstTile.Find(value => value.m_nTile == 5 && value.m_nCol == 1).m_nTile = nTile;
            }
            if (lstTile.Exists(value => value.m_nTile == 0 && value.m_nCol == 1))
            {
                int nTile = 0;
                lstTile = lstTile.FindAll(value => value.m_nCol == 1);
                while (true)
                {
                    nTile = RND.Next(1, 13);
                    if (nTile == 5)
                        continue;
                    bool bFlag = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (nTile == lstTile[i].m_nTile)
                        {
                            bFlag = false;
                            break;
                        }
                    }

                    if (bFlag)
                        break;
                }
                lstTile.Find(value => value.m_nTile == 0 && value.m_nCol == 1).m_nTile = nTile;
            }
        }

        //5드래곤잭팟돌기를 생성하는 함수
        private int MakePrizeScorelRoll(int nDstCash, int nPrizeCnt)
        {
            List<CFdgScoreInfo> lstPrizeScoreInfo = new List<CFdgScoreInfo>();

            int nMaxMulti = m_lstMulti[2] * m_nDRGMulti;
            //최고배당으로 최대점수를 우선 준다.
            int nTempDstCash = (int)Math.Truncate((double)(nDstCash / nMaxMulti));
            List<CFdgScoreTableInfo> lstScoreTable = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore < nTempDstCash);
            if (lstScoreTable != null && lstScoreTable.Count > 0)
            {
                int nMaxScore = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore < nTempDstCash).Max(value => value.nScore);
                nDstCash -= (nMaxScore * nMaxMulti);
                nDstCash += MakePrizeOneScore(nMaxScore * nMaxMulti, nMaxMulti, lstPrizeScoreInfo, 1);   //최대금액을 추가한다.
            }

            while (lstPrizeScoreInfo.Count < nPrizeCnt)
            {
                int nMulti = CGlobal.RandomSelect(m_lstMulti.ToList()) * m_nDRGMulti;
                nTempDstCash = (int)Math.Truncate((double)(nDstCash / nMulti));

                lstScoreTable = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore < nTempDstCash);
                if (lstScoreTable == null || lstScoreTable.Count == 0)
                    break;

                int nMaxScore = m_lstFDGScoreTable.ToList().FindAll(value => value.nScore < nTempDstCash).Max(value => value.nScore);
                nDstCash -= (nMaxScore * nMulti);
                nDstCash += MakePrizeOneScore(nMaxScore * nMulti, nMulti, lstPrizeScoreInfo, 1);   
            }


            //련꽃배당으로 점수를 주고 나머지 점수는 해당점수배당으로 점수를 준다.
            while (nDstCash >= 5 * m_nDRGMulti)
            {
                if (lstPrizeScoreInfo.Count >= nPrizeCnt)
                {
                    break;
                }

                nDstCash = MakePrizeOneScore(nDstCash, m_nDRGMulti, lstPrizeScoreInfo, 0);
            }

            for (int i = 0; i < m_nFreeTotalCnt; i++)
            {
                CFdgScoreInfo scoreInfo = new CFdgScoreInfo();
                scoreInfo.m_nGearCode = m_nGearCode;
                scoreInfo.m_nScore = 0;
                scoreInfo.m_lstScore = new List<int>();
                scoreInfo.m_nMulti = 1;

                MakeOneEmptyRoll(scoreInfo);
                scoreInfo.m_nFlag = 3;
                scoreInfo.m_nMini = i; //프리돌기가 몇번째인가를 나타낸다.
                m_lstPrizeScore.Add(scoreInfo);
            }

            while (lstPrizeScoreInfo.Count > 0)
            {
                int nIndex = RND.Next(1, m_lstPrizeScore.Count);
                if (m_lstPrizeScore[nIndex].m_nScore == 0)
                {
                    CFdgScoreInfo scoreInfo = CGlobal.RandomSelect(lstPrizeScoreInfo);
                    scoreInfo.m_nMini = m_lstPrizeScore[nIndex].m_nMini;
                    lstPrizeScoreInfo.Remove(scoreInfo);
                    m_lstPrizeScore[nIndex] = scoreInfo;
                }
            }

            //프리돌기 시작 예시를 추가한다.
            StartDragonPrizeSpin();

            return nDstCash;
        }

        private int MakePrizeOneScore(int nScore, int nMulti, List<CFdgScoreInfo> lstPrizeScore, int nFlower)
        {
            CFdgScoreInfo scoreInfo = new CFdgScoreInfo();
            scoreInfo.m_nGearCode = m_nGearCode;
            scoreInfo.m_nScore = nScore;
            scoreInfo.m_nFlower = nFlower;
            scoreInfo.m_nMulti = nMulti;
            scoreInfo.m_nFlag = 3;

            List<int> lstScore = new List<int>();
            if (nFlower == 1)
                lstScore.Add(nScore);
            else
                MakeScoreList(nScore, nMulti, lstScore);

            scoreInfo.m_lstScore = lstScore;

            int nRemScore = MakeOneScoreRoll(scoreInfo);
            if (nFlower == 1)
            {
                scoreInfo.m_nMulti = (int)Math.Truncate((double)(nMulti / m_nDRGMulti));
            }


            lstPrizeScore.Add(scoreInfo);   //잭팟점수배렬에 추가한다.

            return nRemScore;
        }

        private void StartDragonPrizeSpin()
        {
            //2번째 렬 첫 빈돌기일때 조커타일을 보여준다.
            CFdgScoreInfo scoreInfo = m_lstPrizeScore[0];
            List<CTile> lstTile = scoreInfo.m_lstTile;
            if (lstTile.Exists(value => value.m_nCol == 1 && value.m_nTile == 0))
                return;
            int nRow = RND.Next(3);
            lstTile.Find(value => value.m_nRow == nRow && value.m_nCol == 1).m_nTile = 0;

            for (int i = 0; i < 3; i++)
            {
                lstTile.Find(value => value.m_nCol == 2 && value.m_nRow == i).m_nTile = -1;
            }

            for (int i = 0; i < 3; i++)
            {
                int nTile = RND.Next(1, 13);
                while (true)
                {
                    if (lstTile.Exists(value => value.m_nCol == 0 && value.m_nTile == nTile))
                        nTile = RND.Next(1, 13);
                    else
                    {
                        if (lstTile.Exists(value => value.m_nCol == 2 && value.m_nTile == nTile))
                            nTile = RND.Next(1, 13);
                        else
                            break;
                    }
                }
                lstTile.Find(value => value.m_nCol == 2 && value.m_nRow == i).m_nTile = nTile;
            }

            return;
        }

        private List<int> MakeScoreList(int nScore, int nMulti, List<int> lstScore)
        {
            int nUnitScore = (int)Math.Truncate((double)(nScore / nMulti));
            if (nUnitScore > 25)
            {
                int nRnd = RND.Next(20);
                if (nRnd == 3)
                {
                    lstScore.Add((int)Math.Truncate((double)nScore * 4 / 5));  //점수를 주는것을 2개통로로 나누어야 한다.
                    lstScore.Add((int)Math.Truncate((double)nScore * 1 / 5));
                }
                else
                {
                    lstScore.Add(nScore);
                }
            }
            else
            {
                lstScore.Add(nScore);
            }

            return lstScore;
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nRemCash = RaiseJackPot(nItemCash, 4);
            CGlobal.SetItemEngineRemCash(nRemCash);
        }
    }
}
