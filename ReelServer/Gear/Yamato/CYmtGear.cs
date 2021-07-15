using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CYmtGear : CBaseGear
    {
        private CYMTPrizeInfo m_prizeInfo;                  //잭팟정보
        private List<CYMTScoreInfo> m_lstYmtPrizeInfo;      //잭팟정보리스트
        public int m_nRaiseCount;                           //잭팟이 나와야 하는 돌기회수
        private CYMTScoreInfo m_clsScoreInfo;               //현재 내려보낸 점수정보(점수가 있을때만 보관한다.)
        private int m_nNationalJackCash;                    //자연잭팟이 쌓인 금액

        private int[] m_nlstStepCash = { 0, 0, 0, 0 };      //0-차환수, 1-차환수, 2-차환수, 3-차환수
        private int m_nOnerPrizeCash;                       //강제잭팟금액


        public CYmtGear() :base() 
        {
            m_lstYmtPrizeInfo = new List<CYMTScoreInfo>();
            m_nRaiseCount = RND.Next(50, 1200);
        }

        public CYMTPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public override void ClearGear()
        {
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            SetGiveStep(0);
            m_prizeInfo = null;
            m_lstYmtPrizeInfo.Clear();
        }

        public override void ClearPrizeInfo()
        {
            m_nGearJack = 0;
            m_prizeInfo = null;
            SetGiveStep(0);
            m_lstYmtPrizeInfo.Clear();
        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            int nDstCash = Convert.ToInt32(nAddCash * rate / 100);

            if(bFlag == false)
            {
                m_nlstStepCash[0] += nDstCash;
            }
            else
            {
                m_nNationalJackCash -= m_nlstStepCash[0];
                m_nlstStepCash[0] = nDstCash;
            }
            m_nNationalJackCash += nDstCash;

            return;
        }

        public override bool MakePrizeRoll()
        {
            if (m_nGearJack > 0)
                return false;

            m_lstYmtPrizeInfo.Clear();
          
            int nCnt = m_prizeInfo.m_nPrizeCash / 20000;
            if (nCnt >= 10)
            {
                MakePrizeScoreInfo(PRIZE_SP);
                nCnt--;
                MakePrizeEmpty();
            }
            
            while(nCnt > 0)
            {
                if(nCnt > 1)
                    MakePrizeScoreInfo(PRiZE_CON);
                else
                    MakePrizeScoreInfo();

                nCnt--;
                MakePrizeEmpty();
            }
            m_nGearJack = 1;

            return true;
        }

        private void MakePrizeEmpty()
        {
            int nCnt = RND.Next(5, 21);

            for(int i=0; i<nCnt; i++)
            {
                CYMTScoreInfo scoreInfo = new CYMTScoreInfo();
                m_lstYmtPrizeInfo.Add(scoreInfo);
            }
        }

        private void MakePrizeScoreInfo(int nCmd = 0)
        {
            CYMTScoreInfo scoreInfo = new CYMTScoreInfo();
            scoreInfo.m_nScore = 20000;
            scoreInfo.m_nCmd = nCmd;

            m_lstYmtPrizeInfo.Add(scoreInfo);
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            if (m_clsScoreInfo == null)
                return;

            if(m_nNationalJackCash < 0)
                m_nRaiseCount = RND.Next(500, 1200);
            else
                m_nRaiseCount = RND.Next(50, 1200);


            for (int i=9; i >= 1; i--)
            {
                m_nLstGraph[i] = m_nLstGraph[i - 1];
            }
            m_nLstGraph[0] = 0;
            m_nYmtRound = 0;
            m_clsScoreInfo = null;

            m_nMajorCount++;
            m_nGiftCash += 20000;
            m_nAccuCash -= 20000;

            if (m_nStep == 4)
            {
                CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(m_nOnerPrizeCash, m_nStep, m_nTakeUser);
                m_nOnerPrizeCash -= 20000;
            }
            else
            {
                if(nCash < m_nlstStepCash[3])
                {
                    CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, 3, m_nTakeUser);
                    m_nlstStepCash[3] -= nCash;
                }
                else
                {
                    CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(m_nlstStepCash[3], 3, m_nTakeUser);
                    nCash -= m_nlstStepCash[3];
                    m_nlstStepCash[3] = 0;

                    if(nCash > 0)
                    {
                        if(nCash < m_nlstStepCash[2])
                        {
                            CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, 2, m_nTakeUser);
                            m_nlstStepCash[2] -= nCash;
                        }
                        else
                        {
                            CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(m_nlstStepCash[2], 2, m_nTakeUser);
                            nCash -= m_nlstStepCash[2];
                            m_nlstStepCash[2] = 0;
                        }

                        if(nCash > 0)
                        {
                            if (nCash < m_nlstStepCash[1])
                            {
                                CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, 1, m_nTakeUser);
                                m_nlstStepCash[1] -= nCash;
                            }
                            else
                            {
                                CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(m_nlstStepCash[1], 1, m_nTakeUser);
                                nCash -= m_nlstStepCash[1];
                                m_nlstStepCash[1] = 0;
                            }

                            if(nCash > 0)
                            {
                                CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, 0, m_nTakeUser);
                                m_nlstStepCash[1] -= nCash;
                            }
                        }
                    }

                    m_nNationalJackCash -= 20000;
                }
            }



            CGlobal.SendGearInfoToClient(this);
            CDataBase.SaveGearInfoToDB(this);
        }

        public override void OnAddSlotCash(CUserSocket clsSocket, int nCash)
        {
            m_nSlotCash -= nCash;
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
            clsPrizeInfoPacket.m_nPrizeKind = 1;
            clsPrizeInfoPacket.m_nPrizeCash = m_prizeInfo.m_nPrizeCash;

            CGlobal.BroadCastPrizeInfo(clsPrizeInfoPacket);
        }

        public override void OnBroadCastPrizeKind()
        {
            if (m_prizeInfo == null)
                return;
            if (m_prizeInfo.m_nPrizeCash == 0)
                return;

            string strNick = string.Empty;

            if (m_nTakeUser > 0)
                strNick = CGlobal.GetUserNickByCode(m_nTakeUser);
            else if (m_nTakeRobot > 0)
                strNick = CGlobal.GetRobotNickByCode(m_nTakeRobot);

            string strMessage = MakeCongration("야마토", m_prizeInfo.m_nPrizeCash);
            CGlobal.SendNoticeBroadCast(strMessage);

            OnBroadCastPrizeInfo();
        }

        public override void OnCreateCoin(CUserSocket clsSocket)
        {
            throw new NotImplementedException();
        }

        public override void OnGearStart(int nRun)
        {
            m_nGearRun = nRun;
        }

        public override void OnReconnect(CUserSocket clsSocket)
        {
            if (m_nGearRun == 0)
                return;

            if (m_clsScoreInfo == null)
            {
                OnStartSpin(clsSocket);
            }
            else
            {
                m_clsScoreInfo.m_nGearCode = m_nGearCode;
                CScorePacket scorePacket = new CScorePacket();
                scorePacket.nGearCode = m_nGearCode;
                scorePacket.strPktData = JsonConvert.SerializeObject(m_clsScoreInfo);

                clsSocket.SendScoreInfo(scorePacket);
            }
        }

        public override void OnStartSpin(CUserSocket clsSocket, int nSpinCash = 100)
        {
            if(m_nSlotCash <= 0)
                return;
            if (m_clsScoreInfo != null)
                OnAddGiftCash(clsSocket, m_clsScoreInfo.m_nScore);

            CYMTScoreInfo scoreInfo = null;
            if(m_lstYmtPrizeInfo.Count > 0)
            {
                //잭팟중일때이다.
                scoreInfo = m_lstYmtPrizeInfo[0];
                m_lstYmtPrizeInfo.Remove(scoreInfo);

                if (m_lstYmtPrizeInfo.Count == 0)
                {
                    //잭팟이 끝났을때이다.
                    OnEndPrizeCall();
                }
            }
            else
            {
                if(m_nYmtRound == m_nRaiseCount)
                {
                    //잭팟을 시작하여야 한다.
                    int nJackCash = 0;
                    if (m_nNationalJackCash > 20000)
                    {
                        nJackCash = m_nNationalJackCash - (m_nNationalJackCash % 20000);
                        int nCount = nJackCash / 20000;
                        nCount = RND.Next(1, nCount + 1);
                        nJackCash = 20000 * nCount;
                    }
                    else
                    {
                        nJackCash = 20000;
                    }
                    m_nNationalJackCash -= nJackCash;
                    CDataBase.InsertNationalToDB(m_nGearCode, nJackCash, 1);
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nJackCash, 1, 1, 0);
                    SetYmtPrizeCall(1, nJackCash);
                }
                scoreInfo = new CYMTScoreInfo();
            }

            if (scoreInfo.m_nScore > 0)
                m_clsScoreInfo = scoreInfo;

            m_nYmtRound++;
            m_nLstGraph[0] = m_nYmtRound;

            OnAddSlotCash(clsSocket, nSpinCash);
            CGlobal.CalcShowJackPot(100, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(nSpinCash, m_nTakeUser);
            m_nAccuCash += 100;

            CScorePacket scorePacket = new CScorePacket();
            scorePacket.nGearCode = m_nGearCode;
            scorePacket.strPktData = JsonConvert.SerializeObject(scoreInfo);
            clsSocket.SendScoreInfo(scorePacket);
            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            m_nNationalJackCash += nJackCash;
            m_nlstStepCash[1] += nJackCash;
            return 0;
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            m_nNationalJackCash += nJackCash;
            m_nlstStepCash[2] += nJackCash;
            return 0;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            m_nNationalJackCash += nJackCash;
            m_nlstStepCash[3] += nJackCash;
            return 0;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (nJackCash < 20000)
                return nJackCash;
            else
            {
                int nRetCash = nJackCash % 20000;
                nJackCash -= nRetCash;

                SetYmtPrizeCall(1, nJackCash);
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                robot.PlayYmtPrize();

                return nRetCash;
            }
        }

        public void SetOnerPrizeCash(int nCash)
        {
            m_nOnerPrizeCash = nCash;
        }

        public bool SetYmtPrizeCall(int nJackCont, int nJackAmount)
        {
            if (m_nGearJack > 0)
                return false;

            m_prizeInfo = new CYMTPrizeInfo();
            m_prizeInfo.m_nCont = nJackCont;
            m_prizeInfo.m_nPrizeCash = nJackAmount;

            bool ret = MakePrizeRoll();

            return ret;
        }

        public void OnExMoney(CUserSocket clsSocket)
        {
            CUser clsUser = CGlobal.GetUserByCode(m_nTakeUser);
            

            CAgent clsAgent = CGlobal.GetAgentByCode(clsUser.m_nAgenCode);
            if (clsUser.m_nChargeCnt > 0)
            {
                string strNote = clsUser.m_strUserNick + "님이 사용하는 " + CGlobal.GetGameNameByGameCode(m_nGameCode) + m_nGearNum + "기대에서 머니전환";
                CDataBase.InsertCacheToDB(clsUser.m_nUserCode, m_nGiftCash, 0, clsUser.m_nUserCash, clsUser.m_nUserCupn, clsUser.m_nUserCash + m_nGiftCash, clsUser.m_nUserCupn, strNote);
                CDataBase.InsertBettingToDB(clsUser.m_nUserCode, m_nGiftCash, clsUser.m_nUserCash, clsUser.m_nUserCash + m_nGiftCash, 9, 1);
                clsUser.m_nUserCash += m_nGiftCash;
            }
            else
            {
                clsUser.m_nVirtualCash += m_nGiftCash;
            }
            m_nGiftCash = 0;

            CDataBase.SaveGearInfoToDB(this);
            CDataBase.SaveUserInfoToDB(clsUser);
            CGlobal.SendGearInfoToClient(this);
            CGlobal.SendUserInfoToClient(clsUser);
        }

        public override void OnEndPrizeCall()
        {
            OnBroadCastPrizeKind();
            ClearPrizeInfo();
            CDataBase.SaveGearInfoToDB(this);
        }

        public List<CYMTScoreInfo> GetPrizeInfoList()
        {
            return m_lstYmtPrizeInfo;
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);

            if(nItemCash < 20000)
            {
                CGlobal.SetItemEngineRemCash(nItemCash);
            }
            else
            {
                int nRetCash = nItemCash % 20000;
                int nJackCash = nItemCash - nRetCash;
                SetYmtPrizeCall(1, nJackCash);
            }
        }
    }
}
