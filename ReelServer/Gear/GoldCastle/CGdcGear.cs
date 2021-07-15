using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CGdcGear : CBaseGear
    {
        private CGDCPrizeInfo m_prizeInfo;                      //잭팟정보
        private CGDCRaiseInfo m_raiseInfo;                      //1차환수금액정보(3개짜리 이벤트로 점수를 준다.)

        private List<CGDCScoreInfo> m_lstScoreInfo;             //빈돌기까지 포함한 완전한 돌기(한번돌때마다 서버에서 돌기정보를 내려보내야 한다.)
        private List<CGDCScoreInfo> m_lstPrizeInfo;             //빈돌기까지 포함한 완전한 돌기 잭팟정보
        private List<CGDCScoreInfo> m_lstEndInfo;               //10만점이상잭팟일때 마지막에 주는 보너스정보
        private CGDCScoreInfo m_clsSendScoreInfo;

        public CGdcGear() : base()
        {
            m_nSpeedCash = 100;
            m_lstScoreInfo = new List<CGDCScoreInfo>();
            m_lstPrizeInfo = new List<CGDCScoreInfo>();
            m_lstEndInfo = new List<CGDCScoreInfo>();
        }

        public override void ClearGear()
        {
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            SetGiveStep(0);
            m_lstPrizeInfo.Clear();
            m_lstScoreInfo.Clear();
            m_lstEndInfo.Clear();
            m_prizeInfo = null;
            m_raiseInfo = null;
        }

        public override void ClearPrizeInfo()
        {
            m_prizeInfo = null;
            m_raiseInfo = null;
            m_lstPrizeInfo.Clear();
            m_lstEndInfo.Clear();
            m_nGearJack = 0;
            SetGiveStep(0);

        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<CGDCScoreInfo> lstGdcScoreInfo = new List<CGDCScoreInfo>();
            int nTempCash = nAddCash;

            while (nTempCash > 0)
            {
                int nAppenCash = nTempCash >= 10000 ? 10000 : nTempCash;
                nTempCash -= nAppenCash;
                int nDstCash = (int)(nAppenCash * rate / 100);

                //점수렬을 만든다.
                List<int> lstScore = new List<int>();
                MakeNormalScoreList(nDstCash, lstScore);

                List<CGDCScoreInfo> lstScoreInfo = new List<CGDCScoreInfo>();
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CGDCScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstGdcScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstGdcScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CGDCScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstGdcScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstGdcScoreInfo.Count);
                    lstGdcScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
                m_lstScoreInfo.Clear();

            m_lstScoreInfo.AddRange(lstGdcScoreInfo);
        }

        public override bool MakePrizeRoll()
        {
            if (m_nGearJack > 0)
                return false;

            switch (m_prizeInfo.m_nCont)
            {
                case JACK_GJOKER: //황금성 유리조커 X 4
                    Prize_Joker(GJOKER);   
                    break;
                case JACK_HJOKER: //황금성 하트조커 X 4
                    Prize_Joker(HJOKER);
                    break;
                case PRIZE_GJOKER: //갈갈이 유리조커 X 3
                    Prize_GJokerX3();
                    break;
                case PRIZE_HJOKER: //용갈이 하트조커 X 3
                    Prize_HJokerX3();
                    break;
                case PRIZE_BUTTER:  //나비요정 X 4, 3
                    Prize_Butter();
                    break;
                case PRIZE_RCASTLE: //파란성보너스 X 3
                    Prize_RCastleX3();
                    break;
                case PRIZE_WCASTLE: //흰성보너스 X 3
                    Prize_WCastleX3();
                    break;
                case PRIZE_HOOK: //선장보너스 X 3
                    Prize_HookX3();
                    break;
                case BUTTER_JOKER: //조커이벤트
                    Prize_ButterJoker();
                    break;
                case LARGE_RACORN:
                    Prize_LargeRAcorn();
                    break;
                case LARGE_WACORN:
                    Prize_LargeWAcorn();
                    break;
                case LARGE_BEAM:
                    Prize_LargeBeam();
                    break;
                case EVENT_HOOK:
                    Prize_Biero();
                    break;

                default:
                    break;
            }

            m_nGearJack = 1;
            CDataBase.SaveGearInfoToDB(this);

            return true;
        }

        private CGDCScoreInfo EndPrizeRoll()
        {
            CGDCScoreInfo scoreInfo = new CGDCScoreInfo();
            scoreInfo.m_nCmd = PRIZE_END;

            return scoreInfo;
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            m_nGiftCash += nCash;

            CUser user = CGlobal.GetUserByCode(m_nTakeUser);
            CAgent agent = CGlobal.GetAgentByCode(user.m_nAgenCode);
            if (nCash > 0)
            {
                m_nAccuCash -= nCash;
                CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, m_nStep, m_nTakeUser);
            }

            if (m_nGiftCash >= 20000)
            {
                int nExCount = m_nGiftCash / 20000;

                if (user.m_nChargeCnt > 0)
                {
                    string strNote = user.m_strUserNick + "님이 사용하는 " + CGlobal.GetGameNameByGameCode(m_nGameCode) + m_nGearNum + "기대에서 기프트머니 쿠폰으로 전환";
                    CDataBase.InsertCacheToDB(user.m_nUserCode, 0, nExCount * 4, user.m_nUserCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn + nExCount * 4, strNote);
                    user.m_nUserCupn += (nExCount * 4);
                }
                else
                {
                    user.m_nVirtualCupn += (nExCount * 4);
                }

                //m_nGdcCash = 0;
                m_nGiftCash -= 20000;
                clsSocket.SendExcupn(m_nGearCode, nExCount * 4);
            }

            m_nGiftCash = m_nGiftCash < 0 ? 0 : m_nGiftCash;
            CDataBase.SaveGearInfoToDB(this);
            CDataBase.SaveUserInfoToDB(user);
            clsSocket.SendGearInfo(this);
            CGlobal.SendUserInfoToClient(user);
        }

        public override void OnAddSlotCash(CUserSocket clsSocket, int nCash)
        {
            throw new NotImplementedException();
        }

        public override void OnBroadCastPrizeInfo()
        {
            if (m_prizeInfo == null)
                return;
            if (m_prizeInfo.m_nPrizeCash == 0)
                return;

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
            clsPrizeInfoPacket.m_nPrizeCash = m_prizeInfo.m_nPrizeMoney;

            CGlobal.BroadCastPrizeInfo(clsPrizeInfoPacket);
        }

        public override void OnBroadCastPrizeKind()
        {
            int nCmd = m_clsSendScoreInfo.m_nCmd;
            if (nCmd == 0)
                return;

            string strMessage = string.Empty;
            string strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);

            switch (nCmd)
            {
                case JACK_GJOKER:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]잭팟이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case JACK_HJOKER:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]잭팟이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_GJOKER:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]조커보너스[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_HJOKER:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]조커보너스[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_BUTTER:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]요정보너스[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_RCASTLE:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]성보너스[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_WCASTLE:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]성보너스[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_HOOK:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]선장보너스[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case EVENT_HOOK:
                    if(m_clsSendScoreInfo.m_nEvent == EVE_3BAR1 || m_clsSendScoreInfo.m_nEvent == EVE_2BAR1 || m_clsSendScoreInfo.m_nEvent == EVE_1BAR1 || m_clsSendScoreInfo.m_nEvent == EVE_MUSIC1 || m_clsSendScoreInfo.m_nEvent == EVE_STAR1)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]선장이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case BUTTER_JOKER:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]조커이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_END:
                    string[] lstPrizeKind = { "", "잭팟이벤트", "잭팟이벤트", "조커보너스", "조커보너스", "요정보너스", "성보너스", "성보너스", "선장보너스", "", "", "선장이벤트", "조커이벤트", "도토리보너스", "도토리보너스", "저울보너스" };
                    string strJack = lstPrizeKind[m_prizeInfo.m_nCont];
                    strMessage = MakeCongration(strJack, m_prizeInfo.m_nPrizeMoney);
                    CGlobal.CalculateJackPotCash(m_prizeInfo.m_nPrizeCash);
                    OnBroadCastPrizeInfo();
                    ClearPrizeInfo();
                    CDataBase.SaveGearInfoToDB(this);
                    break;
            }

            if (strMessage != string.Empty)
                CGlobal.SendNoticeBroadCast(strMessage);
        }

        public override void OnCreateCoin(CUserSocket clsSocket)
        {
            if (m_nGearRun == 0)
                return;

            if (m_nSlotCash < 100)
            {
                m_nSlotCash = 0;
                int nRemScore;
                int nExCount = Math.DivRem(m_nGiftCash, 5000, out nRemScore);
                m_nGiftCash = nRemScore;

                if(nExCount > 0)
                {
                    CUser user = CGlobal.GetUserByCode(m_nTakeUser);
                    if(user.m_nChargeCnt > 0)
                    {
                        string strNote = user.m_strUserNick + "님이 사용하는 " + CGlobal.GetGameNameByGameCode(m_nGameCode) + m_nGearNum + "기대에서 기프트머니 쿠폰으로 전환";
                        CDataBase.InsertCacheToDB(user.m_nUserCode, 0, nExCount, user.m_nUserCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn + nExCount, strNote);
                        user.m_nUserCupn += nExCount;
                    }
                    else
                    {
                        user.m_nVirtualCupn += nExCount;
                    }
                    
                    clsSocket.SendExcupn(m_nGearCode, nExCount);
                }

                OnAddGiftCash(clsSocket, m_nSlotCash - 100, 2);
            }
            else
            {
                m_nSlotCash -= 100;
                m_nAccuCash += 100; 
            }

            CGlobal.CalcShowJackPot(100, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(100, m_nTakeUser);  //누적금액을 사용한 금액만큼 증가시킨다.

            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
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
                OnStartSpin(clsSocket);
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

        public override void OnStartSpin(CUserSocket clsSocket, int nSpinCash = 100)
        {
            if (m_clsSendScoreInfo != null && m_clsSendScoreInfo.m_nScore > 0)
            {
                OnAddGiftCash(clsSocket, m_clsSendScoreInfo.m_nScore);
            }

            m_clsSendScoreInfo = null;
            if (m_nGearJack == 1)
            {
                //잭팟일때이다.
                if (m_lstPrizeInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstPrizeInfo[0];
                    m_lstPrizeInfo.RemoveAt(0);
                }
                else if(m_prizeInfo.m_nRemCash > 0)
                {
                    m_clsSendScoreInfo = CalculateRemPrizeCash(m_prizeInfo.m_nRemCash);
                    InsertEmptyRollToPrizeList(4);
                }
                else if(m_lstEndInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstEndInfo[0];
                    m_lstEndInfo.RemoveAt(0);
                }
                else
                {
                    m_clsSendScoreInfo = EndPrizeRoll();
                }
            }
            else if(m_nGearJack == 2)
            {
                //1차환수중일때이다.
                if (m_lstPrizeInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstPrizeInfo[0];
                    m_lstPrizeInfo.RemoveAt(0);
                }
                else if(m_raiseInfo.m_nRemCash > 0)
                {
                    m_clsSendScoreInfo = CalculateRemRaiseCash(m_raiseInfo.m_nRemCash);
                    InsertEmptyRollToPrizeList(3);
                }
                else
                {
                    ClearPrizeInfo();
                    m_clsSendScoreInfo = MakeEmptyRoll();
                }
            }
            else
            {
                if (m_lstScoreInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstScoreInfo[0];
                    m_lstScoreInfo.RemoveAt(0);

                    if(m_clsSendScoreInfo.m_nScore == 0)
                    {
                        if(RND.Next(300) == 95)
                        {
                            m_clsSendScoreInfo = MakeNoiseEvent();
                        }
                    }

                }
                else
                {
                    int nRollCount = m_nSlotCash / m_nSpeedCash;
                    for (int i = 0; i < nRollCount; i++)
                    {
                        CGDCScoreInfo scoreInfo = MakeEmptyRoll();
                        m_lstScoreInfo.Add(scoreInfo);
                    }

                    if (m_lstScoreInfo.Count > 0)
                    {
                        m_clsSendScoreInfo = m_lstScoreInfo[0];
                        m_lstScoreInfo.RemoveAt(0);
                    }
                    else
                    {
                        m_clsSendScoreInfo = MakeEmptyRoll();
                        m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
                    }
                }
            }
            

            CScorePacket scorePacket = new CScorePacket();
            scorePacket.nGearCode = m_nGearCode;
            scorePacket.strPktData = JsonConvert.SerializeObject(m_clsSendScoreInfo);
            clsSocket.SendScoreInfo(scorePacket);

            OnBroadCastPrizeKind();
        }




        public override void OnEndPrizeCall()
        {
            
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(m_lstGDCRaise1Jack);
            if (scoreInfo.m_nScore > nJackCash)
                return nJackCash;

            if (scoreInfo.m_nEvent > 0)
            {
                CompleteEventScoreInfo(scoreInfo);
            }

            m_raiseInfo = new CGDCRaiseInfo();
            m_raiseInfo.m_nRemCash = scoreInfo.m_nScore - scoreInfo.m_nScore1;
            int nScore = scoreInfo.m_nScore;
            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            m_raiseInfo.m_scoreInfo = scoreInfo;

            m_lstPrizeInfo.Clear();
            m_lstPrizeInfo.Add(scoreInfo);
            m_lstPrizeInfo.Add(MakeEmptyRoll());
            CDataBase.InsertNationalToDB(m_nGearCode, nScore, 1);
            SetGiveStep(1);
            nJackCash -= nScore;
            m_nGearJack = 2;

            return nJackCash;
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            int nPrizeCash = Convert.ToInt32(Math.Truncate((double)nJackCash / 10000)) * 10000;

            List<CGDCJackpotInfo> lstJackpotInfo = m_lstJackpotInfo.FindAll(value => value.m_nStartCash <= nPrizeCash && value.m_nEndCash >= nPrizeCash);
            if (lstJackpotInfo == null || lstJackpotInfo.Count == 0)
                return nJackCash;

            CGDCJackpotInfo jackpotInfo = CGlobal.RandomSelect(lstJackpotInfo);
            
            if (jackpotInfo.m_nCont == LARGE_RACORN || jackpotInfo.m_nCont == LARGE_WACORN || jackpotInfo.m_nCont == LARGE_BEAM)
                nPrizeCash = 20000;
            if(jackpotInfo.m_nCont == PRIZE_HJOKER)
            {
                if (nPrizeCash > 84 * 10000)
                    nPrizeCash = 84 * 10000;
                else if (nPrizeCash > 60 * 10000)
                    nPrizeCash = 60 * 10000;
                else if (nPrizeCash > 36 * 10000)
                    nPrizeCash = 36 * 10000;
                else if (nPrizeCash > 24 * 10000)
                    nPrizeCash = 24 * 10000;
                else if (nPrizeCash > 12 * 10000)
                    nPrizeCash = 12 * 10000;
            }


            int nRemCash = nJackCash - nPrizeCash;

            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, jackpotInfo.m_nCont, 2, 0);
            SetGdcPrizeCall(jackpotInfo.m_nCont, nPrizeCash);
            CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 2);
            
            SetGiveStep(2);
            return nRemCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            int nPrizeCash = Convert.ToInt32(Math.Truncate((double)nJackCash / 10000)) * 10000;
            List<CGDCJackpotInfo> lstJackpotInfo = m_lstJackpotInfo.FindAll(value => value.m_nStartCash <= nPrizeCash && value.m_nEndCash >= nPrizeCash);
            if (lstJackpotInfo == null || lstJackpotInfo.Count == 0)
                return nJackCash;
            CGDCJackpotInfo jackpotInfo = CGlobal.RandomSelect(lstJackpotInfo);
            
            if (jackpotInfo.m_nCont == LARGE_RACORN || jackpotInfo.m_nCont == LARGE_WACORN || jackpotInfo.m_nCont == LARGE_BEAM)
                nPrizeCash = 20000;

            if (jackpotInfo.m_nCont == PRIZE_HJOKER)
            {
                if (nPrizeCash > 84 * 10000)
                    nPrizeCash = 84 * 10000;
                else if (nPrizeCash > 60 * 10000)
                    nPrizeCash = 60 * 10000;
                else if (nPrizeCash > 36 * 10000)
                    nPrizeCash = 36 * 10000;
                else if (nPrizeCash > 24 * 10000)
                    nPrizeCash = 24 * 10000;
                else if (nPrizeCash > 12 * 10000)
                    nPrizeCash = 12 * 10000;
            }

            int nRemCash = nJackCash - nPrizeCash;
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, jackpotInfo.m_nCont, 3, 0);
            SetGdcPrizeCall(jackpotInfo.m_nCont, nPrizeCash);
            CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 3);
            
            SetGiveStep(3);

            return nRemCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            int nPrizeCash = Convert.ToInt32(Math.Truncate((double)nJackCash / 10000)) * 10000;
            List<CGDCJackpotInfo> lstJackpotInfo = m_lstJackpotInfo.FindAll(value => value.m_nStartCash <= nPrizeCash && value.m_nEndCash >= nPrizeCash);
            if (lstJackpotInfo == null || lstJackpotInfo.Count == 0)
                return nJackCash;

            CGDCJackpotInfo jackpotInfo = CGlobal.RandomSelect(lstJackpotInfo);
            
            if (jackpotInfo.m_nCont == LARGE_RACORN || jackpotInfo.m_nCont == LARGE_WACORN || jackpotInfo.m_nCont == LARGE_BEAM)
                nPrizeCash = 20000;

            if (jackpotInfo.m_nCont == PRIZE_HJOKER)
            {
                if (nPrizeCash > 84 * 10000)
                    nPrizeCash = 84 * 10000;
                else if (nPrizeCash > 60 * 10000)
                    nPrizeCash = 60 * 10000;
                else if (nPrizeCash > 36 * 10000)
                    nPrizeCash = 36 * 10000;
                else if (nPrizeCash > 24 * 10000)
                    nPrizeCash = 24 * 10000;
                else if (nPrizeCash > 12 * 10000)
                    nPrizeCash = 12 * 10000;
            }

            int nRemCash = nJackCash - nPrizeCash;
            CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
            if (robot != null)
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, nPrizeCash, jackpotInfo.m_nCont, -1, 1);
            SetGdcPrizeCall(jackpotInfo.m_nCont, nPrizeCash);
            
            if (robot != null)
                robot.PlayGdcPrize();
                

            return nRemCash;
        }

        public void OnAddJashCash(CUserSocket clsSocket, int nCash)
        {
            m_nGdcCash += nCash;
            if (m_nGdcCash > 20000)
                m_nGdcCash = 20000;
        }

        public void OnWinJashCash(CUserSocket clsSocket)
        {
            OnAddGiftCash(clsSocket, m_nGdcCash, 1);
            m_nGdcCash = 0;
        }

        public bool SetGdcPrizeCall(int nJackCont, int nPrizeCash)
        {
            if (m_nGearJack > 0)
                return false;

            int nEndMulti = 0;
            if (nPrizeCash > 80 * 10000)
                nEndMulti = 7;
            else if (nPrizeCash > 60 * 10000)
                nEndMulti = 6;
            else if (nPrizeCash > 40 * 10000)
                nEndMulti = 5;
            else if (nPrizeCash > 12 * 10000)
                nEndMulti = 4;

            if(nJackCont != PRIZE_HJOKER)
            {
                m_prizeInfo = new CGDCPrizeInfo();
                m_prizeInfo.m_nCont = nJackCont;
                m_prizeInfo.m_nPrizeCash = nPrizeCash - 5000 * nEndMulti;
                m_prizeInfo.m_nPrizeMoney = nPrizeCash;
            }
            else
            {
                m_prizeInfo = new CGDCPrizeInfo();
                m_prizeInfo.m_nCont = nJackCont;
                m_prizeInfo.m_nPrizeCash = nPrizeCash;
                m_prizeInfo.m_nPrizeMoney = nPrizeCash;
            }

            bool ret = MakePrizeRoll();

            if (nJackCont != PRIZE_HJOKER)
                MakeEndPrizeInfo(nEndMulti);

            return ret;
        }

        public List<CGDCScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstPrizeInfo;
        }

        public CGDCPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public List<CGDCScoreInfo> GetEndPrizeInfoList()
        {
            return m_lstEndInfo;
        }

        //점수를 여러개의 작은 점수들로 나눈다.
        private void MakeNormalScoreList(int nDstCash, List<int> lstScore)
        {
            int nDelta = nDstCash / 2;
            if (nDelta < 100)
                nDelta = 100;
            while (nDstCash >= 100)
            {
                List<CGDCScoreInfo> lstScoreInfo = m_lstGDCScoreInfo.FindAll(value => value.m_nScore <= nDelta);
                CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                lstScore.Add(scoreInfo.m_nScore);

                nDstCash -= scoreInfo.m_nScore;
                if (nDstCash >= 200)
                {
                    if (RND.Next(0, 2) == 0)
                    {
                        nDelta = nDstCash / 2;
                    }
                    else
                    {
                        nDelta = nDstCash;
                    }
                }

            }
        }

        private CGDCScoreInfo MakeEmptyRoll()
        {
            CGDCScoreInfo scoreInfo = new CGDCScoreInfo();
            scoreInfo.m_nMulti0 = CGlobal.RandomSelect(m_nlstMulti0.ToList());

            return scoreInfo;
        }

        private void InsertEmptyRollToPrizeList(int nCount)
        {
            int nCnt = CGlobal.Random(1, nCount);
            for (int i = 0; i < nCnt; i++)
                m_lstPrizeInfo.Add(MakeEmptyRoll());
        }

        //점수로부터 일반점수정보 만들기
        private CGDCScoreInfo MakeNormalScore(int nScore)
        {
            if (nScore == 0)
                return new CGDCScoreInfo();
            

            List<CGDCScoreInfo> lstScoreInfo = m_lstGDCScoreInfo.FindAll(value => value.m_nScore == nScore);
            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
            
            //일반점수를 주어야 할때이다.
            int nDiv = 1;
            if(nScore == 200)
            {
                if(CGlobal.Random(0, 5) == 3)
                    nDiv = 2;
            }

            if(nDiv == 2)
            {
                List<CGDCScoreInfo> lstScoreInfo100 = m_lstGDCScoreInfo.FindAll(value => value.m_nScore == 100);
                CGDCScoreInfo scoreInfo0 = CGlobal.RandomSelect(lstScoreInfo100);
                int nLine0 = scoreInfo.m_lstTileLine[0].nLine;

                CGDCScoreInfo scoreInfo1 = CGlobal.RandomSelect(lstScoreInfo100.FindAll(value=>value.m_lstTileLine[0].nLine != nLine0));

                while(true)
                {
                    int nCol0 = scoreInfo0.m_lstTile.Find(value => value.m_nTile == scoreInfo0.m_nTile).m_nCol;
                    int nCol1 = scoreInfo1.m_lstTile.Find(value => value.m_nTile == scoreInfo1.m_nTile).m_nCol;

                    if (nCol0 == nCol1 && scoreInfo0.m_nTile == scoreInfo1.m_nTile)
                        scoreInfo1 = CGlobal.RandomSelect(lstScoreInfo100.FindAll(value => value.m_lstTileLine[0].nLine != nLine0));
                    else
                        break;
                }

                scoreInfo = new CGDCScoreInfo();
                scoreInfo.m_nScore = scoreInfo0.m_nScore + scoreInfo1.m_nScore;
                if (scoreInfo0.m_nScore >= scoreInfo1.m_nScore)
                {
                    scoreInfo.m_nTile = scoreInfo0.m_nTile;
                    scoreInfo.m_nCount = scoreInfo0.m_nCount;
                }
                else
                {
                    scoreInfo.m_nTile = scoreInfo1.m_nTile;
                    scoreInfo.m_nCount = scoreInfo1.m_nCount;
                }

                scoreInfo.m_nMulti0 = scoreInfo0.m_nMulti0;
                scoreInfo.m_lstTileLine.Add(scoreInfo0.m_lstTileLine[0]);
                scoreInfo.m_lstTileLine.Add(scoreInfo1.m_lstTileLine[0]);
                scoreInfo.m_lstTile.AddRange(scoreInfo0.m_lstTile);
                scoreInfo.m_lstTile.AddRange(scoreInfo1.m_lstTile);
            }

            return scoreInfo;
        }
       
        //황금성 유리조커 X 4   100만 ~ 350만
        private void Prize_Joker(int nTile)
        {
            //예시정보를 만든다.
            CGDCScoreInfo scoreInfo = new CGDCScoreInfo();
            scoreInfo.m_nCmd = JACK_GJOKER;
            scoreInfo.m_nTile = GJOKER;
            scoreInfo.m_nCount = 4;

            if (m_prizeInfo.m_nPrizeCash <= 130 * 10000)
                scoreInfo.m_nMulti0 = 1;
            else if (m_prizeInfo.m_nPrizeCash <= 160 * 10000)
                scoreInfo.m_nMulti0 = 2;
            else if (m_prizeInfo.m_nPrizeCash < 190 * 10000)
                scoreInfo.m_nMulti0 = 3;
            else if (m_prizeInfo.m_nPrizeCash < 220 * 10000)
                scoreInfo.m_nMulti0 = 5;
            else if (m_prizeInfo.m_nPrizeCash <= 250 * 10000)
                scoreInfo.m_nMulti0 = 7;

            int nLine = RND.Next(3);
            scoreInfo.m_lstTileLine.Add(new LineMulti(nLine, 1));
            for(int i=0; i<4; i++)
            {
                CTile tile = new CTile(nTile, nLine, i);
                tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);
            }

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);

            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash;
        }
        
        //갈갈이 GJoker X 3  21000 ~ 1158000
        private void Prize_GJokerX3()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo = new List<CGDCScoreInfo>();
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, GJOKER, 3, TOP, 1, 1, new int[] { GJOKER, GJOKER, GJOKER, EMPTYT })); 
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, GJOKER, 3, MID, 1, 1, new int[] { GJOKER, GJOKER, GJOKER, EMPTYT })); 
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, GJOKER, 3, BOT, 1, 1, new int[] { GJOKER, GJOKER, GJOKER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, GJOKER, 3, TOP, 1, 1, new int[] { EMPTYT, GJOKER, GJOKER, GJOKER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, GJOKER, 3, MID, 1, 1, new int[] { EMPTYT, GJOKER, GJOKER, GJOKER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, GJOKER, 3, BOT, 1, 1, new int[] { EMPTYT, GJOKER, GJOKER, GJOKER }));

            //이벤트점수판을 만든다
            List<CGDCScoreInfo> lstEventScoreMid = m_lstGDCPrizeInfo.FindAll(value => value.m_nScore < m_prizeInfo.m_nPrizeCash && value.m_lstEventLine[0].nLine == MID).OrderByDescending(value=>value.m_nMulti0).ToList();

            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;

            //처움부터 하나씩 돌면서 조건에 맞는 타일조합을 만든다.
            while(lstEventScoreMid.Count > 0)
            {
                scoreInfoMid = null;
                scoreInfoTop = null;
                scoreInfoBot = null;
                int nPrizeCashMid = m_prizeInfo.m_nPrizeCash;
                if (RND.Next(10) == 3)
                    scoreInfoMid = CGlobal.RandomSelect(lstEventScoreMid);
                else
                    scoreInfoMid = CGlobal.RandomSelect(lstEventScoreMid.FindAll(value=>value.m_nMulti0 == lstEventScoreMid[0].m_nMulti0));

                lstEventScoreMid.Remove(scoreInfoMid);
                nPrizeCashMid -= scoreInfoMid.m_nScore;

                List<CGDCScoreInfo> lstEventScoreTop = m_lstGDCPrizeInfo.FindAll(value => value.m_nScore < nPrizeCashMid && value.m_lstEventLine[0].nLine == TOP && value.m_nMulti0 == scoreInfoMid.m_nMulti0);
                if (lstEventScoreTop == null || lstEventScoreTop.Count == 0)
                    continue;
                //가운데줄에 빠가 당첨이 되였다면 나머지줄에는 빠가 있으면 안된다.
                if(scoreInfoMid.m_nEvent < 7)
                {
                    lstEventScoreTop = lstEventScoreTop.FindAll(value => value.m_nEvent > 6);
                    if (lstEventScoreTop == null || lstEventScoreTop.Count == 0)
                        continue;
                }

                bool bFind = false;
                while(lstEventScoreTop.Count > 0)
                {
                    scoreInfoTop = null;
                    scoreInfoBot = null;

                    int nPrizeCashTop = nPrizeCashMid;
                    scoreInfoTop = CGlobal.RandomSelect(lstEventScoreTop);
                    lstEventScoreTop.Remove(scoreInfoTop);

                    //가운데줄과 같은 타일이 있는가를 검사한다.
                    bool bEqueal = false;
                    foreach(CTile eventTop in scoreInfoTop.m_lstEvent)
                    {
                        CTile eventMid = scoreInfoMid.m_lstEvent.Find(value => value.m_nCol == eventTop.m_nCol);
                        if (eventMid != null && eventTop.m_nTile == eventMid.m_nTile)
                        {
                            bEqueal = true;
                            break;
                        }
                    }
                    if (bEqueal)
                        continue;

                    nPrizeCashTop -= scoreInfoTop.m_nScore;
                    List<CGDCScoreInfo> lstEventScoreBot = m_lstGDCPrizeInfo.FindAll(value => value.m_nScore <= nPrizeCashTop && value.m_lstEventLine[0].nLine == BOT && value.m_nMulti0 == scoreInfoMid.m_nMulti0);
                    if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                        continue;
                    
                    if (scoreInfoMid.m_nEvent < 7 || scoreInfoTop.m_nEvent < 7)
                    {
                        //첫줄과 가운데줄에 빠가 당첨이 되였다면 나머지줄에는 빠가 있으면 안된다.
                        lstEventScoreBot = lstEventScoreBot.FindAll(value => value.m_nEvent > 6).OrderByDescending(value => value.m_nScore).ToList();
                        if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                            continue;
                    }
                    else
                    {
                        //없다면 무조건 빠타일이 되여야 한다.
                        lstEventScoreBot = lstEventScoreBot.FindAll(value => value.m_nEvent < 7).OrderByDescending(value => value.m_nScore).ToList();
                        if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                            continue;
                    }

                    while (lstEventScoreBot.Count > 0)
                    {
                        scoreInfoBot = null;
                        int nPrizeCashBot = nPrizeCashTop;
                        scoreInfoBot = CGlobal.RandomSelect(lstEventScoreBot.FindAll(value=>value.m_nScore == lstEventScoreBot[0].m_nScore));
                        lstEventScoreBot.Remove(scoreInfoBot);
                        nPrizeCashBot -= scoreInfoBot.m_nScore;
                        if(Math.Abs(nPrizeCashBot) <= 10000)
                        {
                            bEqueal = false;
                            foreach (CTile eventBot in scoreInfoBot.m_lstEvent)
                            {
                                CTile eventMid = scoreInfoMid.m_lstEvent.Find(value => value.m_nCol == eventBot.m_nCol);
                                CTile eventTop = scoreInfoTop.m_lstEvent.Find(value => value.m_nCol == eventBot.m_nCol);

                                if ((eventMid != null && eventBot.m_nTile == eventMid.m_nTile) || (eventTop != null && eventBot.m_nTile == eventTop.m_nTile))
                                {
                                    bEqueal = true;
                                    break;
                                }
                            }
                            if (bEqueal)
                                continue;
                            else
                            {
                                //해당한 이벤트조합을 찾았을때이다.
                                bFind = true;
                                break;
                            }
                        }
                    }
                    if (bFind)
                        break;
                }
                if (bFind)
                    break;
            }

            CGDCScoreInfo scoreInfo = null;
            if (scoreInfoMid.m_nEvent < 7)
            {
                scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo.FindAll(value => value.m_lstTileLine[0].nLine == MID));
                scoreInfo.m_nEvent = scoreInfoMid.m_nEvent;
            }
            else if(scoreInfoTop.m_nEvent < 7)
            {
                scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo.FindAll(value => value.m_lstTileLine[0].nLine == TOP));
                scoreInfo.m_nEvent = scoreInfoTop.m_nEvent;
            }
            else if (scoreInfoBot.m_nEvent < 7)
            {
                scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo.FindAll(value => value.m_lstTileLine[0].nLine == BOT));
                scoreInfo.m_nEvent = scoreInfoBot.m_nEvent;
            }

            scoreInfo.m_nMulti0 = scoreInfoMid.m_nMulti0;
            scoreInfo.m_nScore = scoreInfoMid.m_nScore + scoreInfoTop.m_nScore + scoreInfoMid.m_nScore;
            scoreInfo.m_nScore1 = scoreInfoMid.m_nScore1 + scoreInfoTop.m_nScore1 + scoreInfoMid.m_nScore1;

            scoreInfo.m_lstEvent.AddRange(scoreInfoMid.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoTop.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoBot.m_lstEvent);

            scoreInfo.m_lstEventLine.Add(scoreInfoMid.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoTop.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoBot.m_lstEventLine[0]);
            scoreInfo.m_nCmd = PRIZE_GJOKER;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if(scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //용갈이 HJoker X 3   120000, 240000, 360000, 600000, 840000
        private void Prize_HJokerX3()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo = new List<CGDCScoreInfo>();
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HJOKER, 3, TOP, 1, 1, new int[] { HJOKER, HJOKER, HJOKER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HJOKER, 3, MID, 1, 1, new int[] { HJOKER, HJOKER, HJOKER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HJOKER, 3, BOT, 1, 1, new int[] { HJOKER, HJOKER, HJOKER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HJOKER, 3, TOP, 1, 1, new int[] { EMPTYT, HJOKER, HJOKER, HJOKER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HJOKER, 3, MID, 1, 1, new int[] { EMPTYT, HJOKER, HJOKER, HJOKER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HJOKER, 3, BOT, 1, 1, new int[] { EMPTYT, HJOKER, HJOKER, HJOKER }));

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo);
            scoreInfo.m_nMulti0 = Convert.ToInt32(Math.Truncate((double)m_prizeInfo.m_nPrizeCash / 120000));
            scoreInfo.m_nEvent = EVE_DRAGON;
            scoreInfo.m_nCount = 3;
            scoreInfo.m_lstEventLine.Add(new LineMulti(MID, 1));

            List<int> lstEvent = new List<int>();
            lstEvent.Add(EVE_4BAR);
            lstEvent.Add(EVE_3BAR0);
            lstEvent.Add(EVE_2BAR0);
            lstEvent.Add(EVE_1BAR0);
            lstEvent.Add(EVE_MUSIC0);
            lstEvent.Add(EVE_STAR0);
            lstEvent.Add(EVE_TAEGK);

            for (int i=0; i<2; i++)
            {
                CTile eventInfo = new CTile(EVE_DRAGON, MID, 2 * i);
                eventInfo.m_nAct = 1;
                scoreInfo.m_lstEvent.Add(eventInfo);

                eventInfo = new CTile(EVE_DRAGON, MID, 2 * i + 1);
                eventInfo.m_nAct = 1;
                scoreInfo.m_lstEvent.Add(eventInfo);

                int nEvent0 = CGlobal.RandomSelect(lstEvent);
                lstEvent.Remove(nEvent0);
                CTile eventInfo0 = new CTile(nEvent0, BOT, 2 * i);
                scoreInfo.m_lstEvent.Add(eventInfo0);
                eventInfo0 = new CTile(nEvent0, BOT, 2 * i + 1);
                scoreInfo.m_lstEvent.Add(eventInfo0);

                int nEvent1 = CGlobal.RandomSelect(lstEvent);
                lstEvent.Remove(nEvent1);
                CTile eventInfo1 = new CTile(nEvent1, TOP, 2 * i);
                scoreInfo.m_lstEvent.Add(eventInfo1);
                eventInfo1 = new CTile(nEvent1, TOP, 2 * i + 1);
                scoreInfo.m_lstEvent.Add(eventInfo1);
            }

            scoreInfo.m_nCmd = PRIZE_HJOKER;
            scoreInfo.m_nScore = 20000;
            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
            int nCaluCash = m_prizeInfo.m_nPrizeCash - 20000;
            m_prizeInfo.m_nRemCash = nCaluCash;
        }

        //나비요정 ButterX4, ButterX3  20000 ~ 616000(88000 X 7)
        private void Prize_Butter()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo = new List<CGDCScoreInfo>();
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 4, TOP, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, BUTTER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 4, MID, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, BUTTER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 4, BOT, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, BUTTER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 3, TOP, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 3, MID, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 3, BOT, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 3, TOP, 1, 1, new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 3, MID, 1, 1, new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, BUTTER, 3, BOT, 1, 1, new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }));

            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;
            MakePrizeEvent(ref scoreInfoMid, ref scoreInfoTop, ref scoreInfoBot);

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo);
            scoreInfo.m_nMulti0 = scoreInfoMid.m_nMulti0;
            scoreInfo.m_nScore = scoreInfoMid.m_nScore + scoreInfoTop.m_nScore + scoreInfoBot.m_nScore;
            scoreInfo.m_nScore1 = scoreInfoMid.m_nScore1 + scoreInfoTop.m_nScore1 + scoreInfoBot.m_nScore1;

            scoreInfo.m_lstEvent.AddRange(scoreInfoMid.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoTop.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoBot.m_lstEvent);

            scoreInfo.m_lstEventLine.Add(scoreInfoMid.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoTop.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoBot.m_lstEventLine[0]);
            scoreInfo.m_nCmd = PRIZE_BUTTER;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //푸른성보너스 RCastleX3 20000 ~ 616000(88000 X 7)
        private void Prize_RCastleX3()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo = new List<CGDCScoreInfo>();
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, RCASTLE, 3, TOP, 1, 1, new int[] { RCASTLE, RCASTLE, RCASTLE, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, RCASTLE, 3, MID, 1, 1, new int[] { RCASTLE, RCASTLE, RCASTLE, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, RCASTLE, 3, BOT, 1, 1, new int[] { RCASTLE, RCASTLE, RCASTLE, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, RCASTLE, 3, TOP, 1, 1, new int[] { EMPTYT, RCASTLE, RCASTLE, RCASTLE }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, RCASTLE, 3, MID, 1, 1, new int[] { EMPTYT, RCASTLE, RCASTLE, RCASTLE }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, RCASTLE, 3, BOT, 1, 1, new int[] { EMPTYT, RCASTLE, RCASTLE, RCASTLE }));

            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;
            MakePrizeEvent(ref scoreInfoMid, ref scoreInfoTop, ref scoreInfoBot);

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo);
            scoreInfo.m_nMulti0 = scoreInfoMid.m_nMulti0;
            scoreInfo.m_nScore = scoreInfoMid.m_nScore + scoreInfoTop.m_nScore + scoreInfoBot.m_nScore;
            scoreInfo.m_nScore1 = scoreInfoMid.m_nScore1 + scoreInfoTop.m_nScore1 + scoreInfoBot.m_nScore1;

            scoreInfo.m_lstEvent.AddRange(scoreInfoMid.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoTop.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoBot.m_lstEvent);

            scoreInfo.m_lstEventLine.Add(scoreInfoMid.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoTop.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoBot.m_lstEventLine[0]);
            scoreInfo.m_nCmd = PRIZE_RCASTLE;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //흰성보너스 WCastleX4 20000 ~ 616000(88000 X 7)
        private void Prize_WCastleX3()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo = new List<CGDCScoreInfo>();
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, WCASTLE, 3, TOP, 1, 1, new int[] { WCASTLE, WCASTLE, WCASTLE, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, WCASTLE, 3, MID, 1, 1, new int[] { WCASTLE, WCASTLE, WCASTLE, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, WCASTLE, 3, BOT, 1, 1, new int[] { WCASTLE, WCASTLE, WCASTLE, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, WCASTLE, 3, TOP, 1, 1, new int[] { EMPTYT, WCASTLE, WCASTLE, WCASTLE }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, WCASTLE, 3, MID, 1, 1, new int[] { EMPTYT, WCASTLE, WCASTLE, WCASTLE }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, WCASTLE, 3, BOT, 1, 1, new int[] { EMPTYT, WCASTLE, WCASTLE, WCASTLE }));

            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;
            MakePrizeEvent(ref scoreInfoMid, ref scoreInfoTop, ref scoreInfoBot);

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo);
            scoreInfo.m_nMulti0 = scoreInfoMid.m_nMulti0;
            scoreInfo.m_nScore = scoreInfoMid.m_nScore + scoreInfoTop.m_nScore + scoreInfoBot.m_nScore;
            scoreInfo.m_nScore1 = scoreInfoMid.m_nScore1 + scoreInfoTop.m_nScore1 + scoreInfoBot.m_nScore1;

            scoreInfo.m_lstEvent.AddRange(scoreInfoMid.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoTop.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoBot.m_lstEvent);

            scoreInfo.m_lstEventLine.Add(scoreInfoMid.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoTop.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoBot.m_lstEventLine[0]);
            scoreInfo.m_nCmd = PRIZE_WCASTLE;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //후크선장보너스 HookX3 20000 ~ 140000(20000, 40000, 60000, 100000, 140000)
        private void Prize_HookX3()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo = new List<CGDCScoreInfo>();
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HOOKCAP, 3, TOP, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HOOKCAP, 3, MID, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HOOKCAP, 3, BOT, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, EMPTYT }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HOOKCAP, 3, TOP, 1, 1, new int[] { EMPTYT, HOOKCAP, HOOKCAP, HOOKCAP }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HOOKCAP, 3, MID, 1, 1, new int[] { EMPTYT, HOOKCAP, HOOKCAP, HOOKCAP }));
            lstGDCScoreInfo.Add(new CGDCScoreInfo(0, HOOKCAP, 3, BOT, 1, 1, new int[] { EMPTYT, HOOKCAP, HOOKCAP, HOOKCAP }));

            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;
            MakePrizeEvent(ref scoreInfoMid, ref scoreInfoTop, ref scoreInfoBot, 1);

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo);
            scoreInfo.m_nMulti0 = scoreInfoMid.m_nMulti0;
            scoreInfo.m_nScore = scoreInfoMid.m_nScore + scoreInfoTop.m_nScore + scoreInfoBot.m_nScore;
            scoreInfo.m_nScore1 = scoreInfoMid.m_nScore1 + scoreInfoTop.m_nScore1 + scoreInfoBot.m_nScore1;

            scoreInfo.m_lstEvent.AddRange(scoreInfoMid.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoTop.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoBot.m_lstEvent);

            scoreInfo.m_lstEventLine.Add(scoreInfoMid.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoTop.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoBot.m_lstEventLine[0]);
            scoreInfo.m_nCmd = PRIZE_HOOK;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //조커이벤트
        private void Prize_ButterJoker()
        {
            List<CGDCScoreInfo> lstScore = m_lstGDCJokerEvent.FindAll(value => value.m_nScore < m_prizeInfo.m_nPrizeCash);
            int nScoreCash = lstScore.Max(value => value.m_nScore);

            lstScore = lstScore.FindAll(value => value.m_nScore == nScoreCash);
            CGDCScoreInfo orgScoreInfo = CGlobal.RandomSelect(lstScore);
            CGDCScoreInfo scoreInfo = orgScoreInfo.CopyObject();

            scoreInfo.m_lstTileLine.Add(new LineMulti(scoreInfo.m_lstEventLine[0].nLine, 1));
            int nMulti = scoreInfo.m_lstEventLine[0].nMulti;
            int nTile = EMPTYT;
            if(scoreInfo.m_nEvent <7)
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    nTile = RCASTLE;
                else 
                    nTile = WCASTLE;
            }
            else
            {
                int nRnd = RND.Next(3);
                if (nRnd == 0)
                    nTile = RCASTLE;
                else if (nRnd == 1)
                    nTile = WCASTLE;
                else
                    nTile = HOOKCAP;
            }
            scoreInfo.m_nTile = nTile;

            int nRow = scoreInfo.m_lstEventLine[0].nLine;
            for (int i=0; i<4; i++)
            {
                CTile tile = new CTile(nTile, nRow, i);
                tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);
            }

            int[] lstCol = { 0, 1, 2, 3 };
            int nCol0 = CGlobal.RandomSelect(lstCol.ToList());
            int nCol1 = CGlobal.RandomSelect(lstCol.ToList().FindAll(value => value != nCol0));

            if (nMulti == 1)
            {
                scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol0).m_nTile = GJOKER;
                scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol1).m_nTile = HJOKER;
            }
            else if(nMulti == 2) 
            {
                if (CGlobal.Random(0, 2) == 0)
                    scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol0).m_nTile = HJOKER;
                else
                    scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol0).m_nTile = GJOKER;
            }
            else if(nMulti == 4)
            {
                scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol0).m_nTile = GJOKER;
                scoreInfo.m_lstTile.Find(value => value.m_nCol == nCol1).m_nTile = GJOKER;
            }

            scoreInfo.m_nCmd = BUTTER_JOKER;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //초대형 붉은 도토리
        private void Prize_LargeRAcorn()
        {
            List<CGDCScoreInfo> lstScoreInfo = new List<CGDCScoreInfo>();
            for(int i=0; i<m_nlstMulti0.Length; i++)
            {
                lstScoreInfo.Add(new CGDCScoreInfo(20000, RACORN, 4, TOP, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, RACORN }, LARGE_RACORN));
                lstScoreInfo.Add(new CGDCScoreInfo(20000, RACORN, 4, MID, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, RACORN }, LARGE_RACORN));
                lstScoreInfo.Add(new CGDCScoreInfo(20000, RACORN, 4, BOT, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, RACORN }, LARGE_RACORN));
            }

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //초대형 흰도토리
        private void Prize_LargeWAcorn()
        {
            List<CGDCScoreInfo> lstScoreInfo = new List<CGDCScoreInfo>();
            for (int i = 0; i < m_nlstMulti0.Length; i++)
            {
                lstScoreInfo.Add(new CGDCScoreInfo(20000, WACORN, 4, TOP, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, WACORN }, LARGE_WACORN));
                lstScoreInfo.Add(new CGDCScoreInfo(20000, WACORN, 4, MID, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, WACORN }, LARGE_WACORN));
                lstScoreInfo.Add(new CGDCScoreInfo(20000, WACORN, 4, BOT, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, WACORN }, LARGE_WACORN));
            }

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //초대형 저울
        private void Prize_LargeBeam()
        {
            List<CGDCScoreInfo> lstScoreInfo = new List<CGDCScoreInfo>();
            for (int i = 0; i < m_nlstMulti0.Length; i++)
            {
                lstScoreInfo.Add(new CGDCScoreInfo(20000, BEAM, 4, TOP, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, BEAM }, LARGE_BEAM));
                lstScoreInfo.Add(new CGDCScoreInfo(20000, BEAM, 4, MID, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, BEAM }, LARGE_BEAM));
                lstScoreInfo.Add(new CGDCScoreInfo(20000, BEAM, 4, BOT, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, BEAM }, LARGE_BEAM));
            }

            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        //삐에로이벤트
        private void Prize_Biero()
        {
            List<CGDCScoreInfo> lstGDCScoreInfo3R = new List<CGDCScoreInfo>();
            List<CGDCScoreInfo> lstGDCScoreInfo3L = new List<CGDCScoreInfo>();
            List<CGDCScoreInfo> lstGDCScoreInfo4 = new List<CGDCScoreInfo>();
            lstGDCScoreInfo4.Add(new CGDCScoreInfo(0, HOOKCAP, 4, TOP, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, HOOKCAP }));
            lstGDCScoreInfo4.Add(new CGDCScoreInfo(0, HOOKCAP, 4, MID, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, HOOKCAP }));
            lstGDCScoreInfo4.Add(new CGDCScoreInfo(0, HOOKCAP, 4, BOT, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, HOOKCAP }));

            lstGDCScoreInfo3L.Add(new CGDCScoreInfo(0, HOOKCAP, 3, TOP, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, EMPTYT }));
            lstGDCScoreInfo3L.Add(new CGDCScoreInfo(0, HOOKCAP, 3, MID, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, EMPTYT }));
            lstGDCScoreInfo3L.Add(new CGDCScoreInfo(0, HOOKCAP, 3, BOT, 1, 1, new int[] { HOOKCAP, HOOKCAP, HOOKCAP, EMPTYT }));

            lstGDCScoreInfo3R.Add(new CGDCScoreInfo(0, HOOKCAP, 3, TOP, 1, 1, new int[] { EMPTYT, HOOKCAP, HOOKCAP, HOOKCAP }));
            lstGDCScoreInfo3R.Add(new CGDCScoreInfo(0, HOOKCAP, 3, MID, 1, 1, new int[] { EMPTYT, HOOKCAP, HOOKCAP, HOOKCAP }));
            lstGDCScoreInfo3R.Add(new CGDCScoreInfo(0, HOOKCAP, 3, BOT, 1, 1, new int[] { EMPTYT, HOOKCAP, HOOKCAP, HOOKCAP }));


            //이벤트점수판을 만든다
            List<CGDCScoreInfo> lstEventScoreMid = m_lstGDCBieroEvent.FindAll(value => value.m_nScore < m_prizeInfo.m_nPrizeCash && value.m_lstEventLine[0].nLine == MID).OrderByDescending(value=>value.m_nMulti0).ToList();
            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;

            //처움부터 하나씩 돌면서 조건에 맞는 타일조합을 만든다.
            while (lstEventScoreMid.Count > 0)
            {
                scoreInfoMid = null;
                scoreInfoTop = null;
                scoreInfoBot = null;
                int nPrizeCashMid = m_prizeInfo.m_nPrizeCash;
                scoreInfoMid = CGlobal.RandomSelect(lstEventScoreMid.FindAll(value=>value.m_nMulti0 == lstEventScoreMid[0].m_nMulti0));
                lstEventScoreMid.Remove(scoreInfoMid);

                nPrizeCashMid -= scoreInfoMid.m_nScore;

                List<CGDCScoreInfo> lstEventScoreTop = m_lstGDCBieroEvent.FindAll(value => value.m_nScore < nPrizeCashMid && value.m_lstEventLine[0].nLine == TOP && value.m_nMulti0 == scoreInfoMid.m_nMulti0);
                if (lstEventScoreTop == null || lstEventScoreTop.Count == 0)
                    continue;
                //가운데줄에 빠가 당첨이 되였다면 나머지줄에는 빠가 있으면 안된다.
                if (scoreInfoMid.m_nEvent < 7)
                {
                    lstEventScoreTop = lstEventScoreTop.FindAll(value => value.m_nEvent > 6);
                    if (lstEventScoreTop == null || lstEventScoreTop.Count == 0)
                        continue;
                }

                bool bFind = false;
                while (lstEventScoreTop.Count > 0)
                {
                    scoreInfoTop = null;
                    scoreInfoBot = null;

                    int nPrizeCashTop = nPrizeCashMid;
                    scoreInfoTop = CGlobal.RandomSelect(lstEventScoreTop);
                    lstEventScoreTop.Remove(scoreInfoTop);

                    //가운데줄과 같은 타일이 있는가를 검사한다.
                    bool bEqueal = false;
                    foreach (CTile eventTop in scoreInfoTop.m_lstEvent)
                    {
                        CTile eventMid = scoreInfoMid.m_lstEvent.Find(value => value.m_nCol == eventTop.m_nCol);
                        if (eventMid != null && eventTop.m_nTile == eventMid.m_nTile)
                        {
                            bEqueal = true;
                            break;
                        }
                    }
                    if (bEqueal)
                        continue;

                    nPrizeCashTop -= scoreInfoTop.m_nScore;
                    List<CGDCScoreInfo> lstEventScoreBot = m_lstGDCBieroEvent.FindAll(value => value.m_nScore <= nPrizeCashTop && value.m_lstEventLine[0].nLine == BOT && value.m_nMulti0 == scoreInfoMid.m_nMulti0);
                    if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                        continue;

                    if (scoreInfoMid.m_nEvent < 7 || scoreInfoTop.m_nEvent < 7)
                    {
                        //첫줄과 가운데줄에 빠가 당첨이 되였다면 나머지줄에는 빠가 있으면 안된다.
                        lstEventScoreBot = lstEventScoreBot.FindAll(value => value.m_nEvent > 6).OrderByDescending(value => value.m_nScore).ToList();
                        if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                            continue;
                    }
                    else
                    {
                        //없다면 무조건 빠타일이 되여야 한다.
                        lstEventScoreBot = lstEventScoreBot.FindAll(value => value.m_nEvent < 7).OrderByDescending(value => value.m_nScore).ToList();
                        if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                            continue;
                    }

                    while (lstEventScoreBot.Count > 0)
                    {
                        scoreInfoBot = null;
                        int nPrizeCashBot = nPrizeCashTop;
                        scoreInfoBot = lstEventScoreBot[0];
                        lstEventScoreBot.Remove(scoreInfoBot);

                        nPrizeCashBot -= scoreInfoBot.m_nScore;
                        if (nPrizeCashBot <= 5000)
                        {
                            bEqueal = false;
                            foreach (CTile eventBot in scoreInfoBot.m_lstEvent)
                            {
                                CTile eventMid = scoreInfoMid.m_lstEvent.Find(value => value.m_nCol == eventBot.m_nCol);
                                CTile eventTop = scoreInfoTop.m_lstEvent.Find(value => value.m_nCol == eventBot.m_nCol);

                                if ((eventMid != null && eventBot.m_nTile == eventMid.m_nTile) || (eventTop != null && eventBot.m_nTile == eventTop.m_nTile))
                                {
                                    bEqueal = true;
                                    break;
                                }
                            }
                            if (bEqueal)
                                continue;
                            else
                            {
                                //해당한 이벤트조합을 찾았을때이다.
                                bFind = true;
                                break;
                            }
                        }
                    }
                    if (bFind)
                        break;
                }
                if (bFind)
                    break;
            }

            CGDCScoreInfo scoreInfo = null;
            if (scoreInfoMid.m_nEvent < 7)
            {
                if(scoreInfoMid.m_nCount == 3)
                {
                    if(scoreInfoMid.m_lstEvent.Exists(value=>value.m_nCol == 3))
                        scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo3R.FindAll(value => value.m_lstTileLine[0].nLine == MID));
                    else
                        scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo3L.FindAll(value => value.m_lstTileLine[0].nLine == MID));
                    scoreInfo.m_nEvent = scoreInfoMid.m_nEvent;
                }
                else
                {
                    scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo4.FindAll(value => value.m_lstTileLine[0].nLine == MID));
                    scoreInfo.m_nEvent = scoreInfoMid.m_nEvent;
                }
                
            }
            else if (scoreInfoTop.m_nEvent < 7)
            {
                if (scoreInfoTop.m_nCount == 3)
                {
                    if (scoreInfoTop.m_lstEvent.Exists(value => value.m_nCol == 3))
                        scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo3R.FindAll(value => value.m_lstTileLine[0].nLine == TOP));
                    else
                        scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo3L.FindAll(value => value.m_lstTileLine[0].nLine == TOP));
                    scoreInfo.m_nEvent = scoreInfoTop.m_nEvent;
                }
                else
                {
                    scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo4.FindAll(value => value.m_lstTileLine[0].nLine == TOP));
                    scoreInfo.m_nEvent = scoreInfoTop.m_nEvent;
                }
                    
            }
            else if (scoreInfoBot.m_nEvent < 7)
            {
                if (scoreInfoBot.m_nCount == 3)
                {
                    if (scoreInfoBot.m_lstEvent.Exists(value => value.m_nCol == 3))
                        scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo3R.FindAll(value => value.m_lstTileLine[0].nLine == BOT));
                    else
                        scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo3L.FindAll(value => value.m_lstTileLine[0].nLine == BOT));
                    scoreInfo.m_nEvent = scoreInfoBot.m_nEvent;
                }
                else
                {
                    scoreInfo = CGlobal.RandomSelect(lstGDCScoreInfo4.FindAll(value => value.m_lstTileLine[0].nLine == BOT));
                    scoreInfo.m_nEvent = scoreInfoBot.m_nEvent;
                }
                    
            }

            scoreInfo.m_nMulti0 = scoreInfoMid.m_nMulti0;
            scoreInfo.m_nScore = scoreInfoMid.m_nScore + scoreInfoTop.m_nScore + scoreInfoMid.m_nScore;
            scoreInfo.m_nScore1 = scoreInfoMid.m_nScore1 + scoreInfoTop.m_nScore1 + scoreInfoMid.m_nScore1;
            scoreInfo.m_lstEvent.AddRange(scoreInfoMid.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoTop.m_lstEvent);
            scoreInfo.m_lstEvent.AddRange(scoreInfoBot.m_lstEvent);

            scoreInfo.m_lstEventLine.Add(scoreInfoMid.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoTop.m_lstEventLine[0]);
            scoreInfo.m_lstEventLine.Add(scoreInfoBot.m_lstEventLine[0]);
            scoreInfo.m_nCmd = EVENT_HOOK;

            scoreInfo.m_nScore = scoreInfo.m_nScore1;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_prizeInfo.m_nRemCash = m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore;

            m_lstPrizeInfo.Add(scoreInfo);
            InsertEmptyRollToPrizeList(3);
        }

        private void MakePrizeEvent(ref CGDCScoreInfo rfScoreInfoMid, ref CGDCScoreInfo rfScoreInfoTop, ref CGDCScoreInfo rfScoreInfoBot, int nKind = 0)
        {
            List<CGDCScoreInfo> lstEventScoreMid = m_lstGDCPrizeInfo.FindAll(value => value.m_nScore < m_prizeInfo.m_nPrizeCash && value.m_lstEventLine[0].nLine == MID).OrderByDescending(value=>value.m_nMulti0).ToList();
            if (nKind == 1)
                lstEventScoreMid = lstEventScoreMid.FindAll(value => value.m_nEvent > 6);

            CGDCScoreInfo scoreInfoMid = null;
            CGDCScoreInfo scoreInfoTop = null;
            CGDCScoreInfo scoreInfoBot = null;

            //처움부터 하나씩 돌면서 조건에 맞는 타일조합을 만든다.
            while (lstEventScoreMid.Count > 0)
            {
                scoreInfoMid = null;
                scoreInfoTop = null;
                scoreInfoBot = null;
                int nPrizeCashMid = m_prizeInfo.m_nPrizeCash;
                
                if(RND.Next(10) == 3)
                    scoreInfoMid = CGlobal.RandomSelect(lstEventScoreMid);
                else
                    scoreInfoMid = CGlobal.RandomSelect(lstEventScoreMid.FindAll(value=>value.m_nMulti0 == lstEventScoreMid[0].m_nMulti0));
                lstEventScoreMid.Remove(scoreInfoMid);

                nPrizeCashMid -= scoreInfoMid.m_nScore;

                List<CGDCScoreInfo> lstEventScoreTop = m_lstGDCPrizeInfo.FindAll(value => value.m_nScore < nPrizeCashMid && value.m_lstEventLine[0].nLine == TOP && value.m_nMulti0 == scoreInfoMid.m_nMulti0);
                if(nKind == 1)
                    lstEventScoreTop = lstEventScoreTop.FindAll(value => value.m_nEvent > 6);

                if (lstEventScoreTop == null || lstEventScoreTop.Count == 0)
                    continue;

                bool bFind = false;
                while (lstEventScoreTop.Count > 0)
                {
                    scoreInfoTop = null;
                    scoreInfoBot = null;

                    int nPrizeCashTop = nPrizeCashMid;
                    scoreInfoTop = CGlobal.RandomSelect(lstEventScoreTop);
                    lstEventScoreTop.Remove(scoreInfoTop);

                    //가운데줄과 같은 타일이 있는가를 검사한다.
                    bool bEqueal = false;
                    foreach (CTile eventTop in scoreInfoTop.m_lstEvent)
                    {
                        CTile eventMid = scoreInfoMid.m_lstEvent.Find(value => value.m_nCol == eventTop.m_nCol);
                        if (eventMid != null && eventTop.m_nTile == eventMid.m_nTile)
                        {
                            bEqueal = true;
                            break;
                        }
                    }
                    if (bEqueal)
                        continue;

                    nPrizeCashTop -= scoreInfoTop.m_nScore;
                    List<CGDCScoreInfo> lstEventScoreBot = m_lstGDCPrizeInfo.FindAll(value => value.m_nScore <= nPrizeCashTop && value.m_lstEventLine[0].nLine == BOT && value.m_nMulti0 == scoreInfoMid.m_nMulti0).OrderByDescending(value=>value.m_nScore).ToList();
                    if (nKind == 1)
                        lstEventScoreBot = lstEventScoreBot.FindAll(value => value.m_nEvent > 6);

                    if (lstEventScoreBot == null || lstEventScoreBot.Count == 0)
                        continue;


                    while (lstEventScoreBot.Count > 0)
                    {
                        scoreInfoBot = null;
                        int nPrizeCashBot = nPrizeCashTop;
                        scoreInfoBot = CGlobal.RandomSelect(lstEventScoreBot.FindAll(value => value.m_nScore == lstEventScoreBot[0].m_nScore));
                        lstEventScoreBot.Remove(scoreInfoBot);

                        nPrizeCashBot -= scoreInfoBot.m_nScore;
                        if(nPrizeCashBot <= 10000)
                        {
                            bEqueal = false;
                            foreach (CTile eventBot in scoreInfoBot.m_lstEvent)
                            {
                                CTile eventMid = scoreInfoMid.m_lstEvent.Find(value => value.m_nCol == eventBot.m_nCol);
                                CTile eventTop = scoreInfoTop.m_lstEvent.Find(value => value.m_nCol == eventBot.m_nCol);

                                if ((eventMid != null && eventBot.m_nTile == eventMid.m_nTile) || (eventTop != null && eventBot.m_nTile == eventTop.m_nTile))
                                {
                                    bEqueal = true;
                                    break;
                                }
                            }
                            if (bEqueal)
                                continue;
                            else
                            {
                                //해당한 이벤트조합을 찾았을때이다.
                                bFind = true;
                                break;
                            }
                        }
                    }
                    if (bFind)
                        break;
                }
                if (bFind)
                    break;
            }

            rfScoreInfoMid = scoreInfoMid;
            rfScoreInfoTop = scoreInfoTop;
            rfScoreInfoBot = scoreInfoBot;
        }

        //고배당당첨이후 뒤점수를 주는 함수이다.
        private CGDCScoreInfo CalculateRemPrizeCash(int nCash)
        {
            CGDCScoreInfo scoreInfo = null;

            List<CGDCScoreInfo> lstScore = new List<CGDCScoreInfo>();
            lstScore.AddRange(m_lstGDCEventInfo);
            lstScore.AddRange(m_lstGDCScoreInfo);
            lstScore = lstScore.FindAll(value => value.m_nScore <= nCash).OrderByDescending(value=>value.m_nScore).ToList();

            if(lstScore.Count > 0)
            {
                if (nCash >= 32000)
                {
                    if (m_nGiftCash < 8000)
                        lstScore = lstScore.FindAll(value => value.m_nScore == 32000);
                    else
                        lstScore = lstScore.FindAll(value => value.m_nScore >= 10000 && value.m_nScore <= 20000);
                }
                else
                {
                    if(nCash >= 10000)
                        lstScore = lstScore.FindAll(value => value.m_nScore >= 10000);
                    else if(nCash >= 5000)
                        lstScore = lstScore.FindAll(value => value.m_nScore >= 5000);
                    else
                        lstScore = lstScore.FindAll(value => value.m_nScore == lstScore[0].m_nScore);
                }
                scoreInfo = CGlobal.RandomSelect(lstScore);
            }
            else
            {
                lstScore = m_lstGDCScoreInfo.FindAll(value => value.m_nScore == 100);
                scoreInfo = CGlobal.RandomSelect(lstScore);
            }

            m_prizeInfo.m_nRemCash -= scoreInfo.m_nScore;
            if (m_prizeInfo.m_nRemCash < 0)
                m_prizeInfo.m_nRemCash = 0;

            if(scoreInfo.m_nEvent > 0)
                CompleteEventScoreInfo(scoreInfo);

            return scoreInfo;
        }

        //1차환수금액나머지 점수주는 부분
        private CGDCScoreInfo CalculateRemRaiseCash(int nCash)
        {
            CGDCScoreInfo scoreInfo = null;

            List<CGDCScoreInfo> lstScore = new List<CGDCScoreInfo>();
            lstScore.AddRange(m_lstGDCEventInfo);
            lstScore.AddRange(m_lstGDCScoreInfo);
            lstScore = lstScore.FindAll(value => value.m_nScore <= nCash).OrderByDescending(value => value.m_nScore).ToList();

            if (lstScore.Count > 0)
            {
                if (nCash >= 32000)
                {
                    if (m_nGiftCash < 8000)
                        lstScore = lstScore.FindAll(value => value.m_nScore == 32000);
                    else
                        lstScore = lstScore.FindAll(value => value.m_nScore >= 10000 && value.m_nScore <= 20000);
                }
                else
                {
                    if (nCash >= 10000)
                        lstScore = lstScore.FindAll(value => value.m_nScore >= 10000);
                    else if (nCash >= 5000)
                        lstScore = lstScore.FindAll(value => value.m_nScore >= 5000);
                    else
                        lstScore = lstScore.FindAll(value => value.m_nScore == lstScore[0].m_nScore);
                }
                scoreInfo = CGlobal.RandomSelect(lstScore);
            }
            else
            {
                lstScore = m_lstGDCScoreInfo.FindAll(value => value.m_nScore == 100);
                scoreInfo = CGlobal.RandomSelect(lstScore);
            }

            m_raiseInfo.m_nRemCash -= scoreInfo.m_nScore;
            if (m_raiseInfo.m_nRemCash < 0)
                m_raiseInfo.m_nRemCash = 0;

            if (scoreInfo.m_nEvent > 0)
                CompleteEventScoreInfo(scoreInfo);

            return scoreInfo;
        }

        //이벤트점수정보를 완전한 돌기정보로 완성하는 함수
        private void CompleteEventScoreInfo(CGDCScoreInfo scoreInfo)
        {
            int nEvent = scoreInfo.m_nEvent;
            int nTile = EMPTYT;

            if(nEvent < 7)
            {
                if (CGlobal.Random(0, 2) == 0)
                    nTile = RCASTLE;
                else
                    nTile = WCASTLE;
            }
            else
            {
                int nRnd = CGlobal.Random(0, 3);
                if (nRnd == 0)
                    nTile = RCASTLE;
                else if (nRnd == 1)
                    nTile = WCASTLE;
                else
                    nTile = HOOKCAP;
            }

            scoreInfo.m_nTile = nTile;
            LineMulti line = new LineMulti(scoreInfo.m_lstEventLine[0].nLine, scoreInfo.m_lstEventLine[0].nMulti);
            scoreInfo.m_lstTileLine.Add(line);

            if (nTile == RCASTLE)
                scoreInfo.m_nCmd = EVENT_RCASTLE;
            else if (nTile == WCASTLE)
                scoreInfo.m_nCmd = EVENT_WCASTLE;
            else if (nTile == HOOKCAP)
                scoreInfo.m_nCmd = EVENT_HOOK;

            scoreInfo.m_lstTile.Clear();
            foreach (CTile eventInfo in scoreInfo.m_lstEvent)
            {
                CTile tile = new CTile(nTile, eventInfo.m_nRow, eventInfo.m_nCol);
                tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);
            }

        }

        private CGDCScoreInfo MakeNoiseEvent()
        {
            CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(m_lstGDCNoiseInfo);
            CompleteEventScoreInfo(scoreInfo);

            return scoreInfo;
        }

        private void MakeEndPrizeInfo(int nEndMulti)
        {
            m_lstEndInfo.Clear();
            List<CGDCScoreInfo> lstScoreInfo = m_lstGDCPrizeInfo.FindAll(value => value.m_nEvent > 6 && value.m_lstEventLine[0].nMulti == 1 && !value.m_lstEvent.Exists(val=>val.m_nTile != value.m_nEvent));
            lstScoreInfo = lstScoreInfo.FindAll(value => value.m_lstEventLine[0].nLine != MID);
            List<CGDCScoreInfo> lstRemScoreInfo = new List<CGDCScoreInfo>();
            lstRemScoreInfo.AddRange(m_lstGDCEventInfo);
            lstRemScoreInfo.AddRange(m_lstGDCScoreInfo);


            if (nEndMulti == 4)
            {
                for(int i=0; i<2; i++)
                {
                    lstScoreInfo = lstScoreInfo.FindAll(value => value.m_nMulti0 == 2);
                    CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    CompleteEventScoreInfo(scoreInfo);
                    int nRemCash = scoreInfo.m_nScore - scoreInfo.m_nScore1;
                    scoreInfo.m_nScore = scoreInfo.m_nScore1;
                    m_lstEndInfo.Add(scoreInfo);
                    m_lstEndInfo.Add(MakeEmptyRoll());
                    CGDCScoreInfo remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value => value.m_nScore == nRemCash));
                    if(remScoreInfo != null)
                    {
                        if (remScoreInfo.m_nEvent > 0)
                            CompleteEventScoreInfo(remScoreInfo);
                        m_lstEndInfo.Add(remScoreInfo);
                    }
                    m_lstEndInfo.Add(MakeEmptyRoll());
                }
            }
            else if(nEndMulti == 5)
            {
                lstScoreInfo = lstScoreInfo.FindAll(value => value.m_nMulti0 == 5);
                CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                CompleteEventScoreInfo(scoreInfo);
                int nRemCash = scoreInfo.m_nScore - scoreInfo.m_nScore1;
                scoreInfo.m_nScore = scoreInfo.m_nScore1;
                m_lstEndInfo.Add(scoreInfo);
                m_lstEndInfo.Add(MakeEmptyRoll());

                while (nRemCash > 0)
                {
                    CGDCScoreInfo remScoreInfo = null;
                    List<CGDCScoreInfo> lstNormalScore = lstRemScoreInfo.FindAll(value => value.m_nScore <= nRemCash);
                    if(lstNormalScore == null || lstNormalScore.Count == 0)
                    {
                        remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value=>value.m_nScore == 100));
                        nRemCash = 0;
                    }
                    else
                    {
                        int nScore = lstNormalScore.Max(value => value.m_nScore);
                        remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value => value.m_nScore == nScore));
                        nRemCash -= nScore;
                    }
                    if(remScoreInfo != null)
                    {
                        if (remScoreInfo.m_nEvent > 0)
                            CompleteEventScoreInfo(remScoreInfo);

                        m_lstEndInfo.Add(remScoreInfo);
                    }
                    
                    m_lstEndInfo.Add(MakeEmptyRoll());
                }
            }

            else if (nEndMulti == 6)
            {
                for(int i=0; i<2; i++)
                {
                    lstScoreInfo = lstScoreInfo.FindAll(value => value.m_nMulti0 == 3);
                    CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    CompleteEventScoreInfo(scoreInfo);
                    int nRemCash = scoreInfo.m_nScore - scoreInfo.m_nScore1;
                    scoreInfo.m_nScore = scoreInfo.m_nScore1;
                    m_lstEndInfo.Add(scoreInfo);
                    m_lstEndInfo.Add(MakeEmptyRoll());

                    while (nRemCash > 0)
                    {
                        CGDCScoreInfo remScoreInfo = null;
                        List<CGDCScoreInfo> lstNormalScore = lstRemScoreInfo.FindAll(value => value.m_nScore <= nRemCash);
                        if (lstNormalScore == null || lstNormalScore.Count == 0)
                        {
                            remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value => value.m_nScore == 100));
                            nRemCash = 0;
                        }
                        else
                        {
                            int nScore = lstNormalScore.Max(value => value.m_nScore);
                            remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value => value.m_nScore == nScore));
                            nRemCash -= nScore;
                        }
                        if(remScoreInfo != null)
                        {
                            if (remScoreInfo.m_nEvent > 0)
                                CompleteEventScoreInfo(remScoreInfo);

                            m_lstEndInfo.Add(remScoreInfo);
                        }
                        
                        m_lstEndInfo.Add(MakeEmptyRoll());
                    }
                }
            }
            else if(nEndMulti == 7)
            {
                lstScoreInfo = lstScoreInfo.FindAll(value => value.m_nMulti0 == 7);
                CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                CompleteEventScoreInfo(scoreInfo);
                int nRemCash = scoreInfo.m_nScore - scoreInfo.m_nScore1;
                scoreInfo.m_nScore = scoreInfo.m_nScore1;
                m_lstEndInfo.Add(scoreInfo);
                m_lstEndInfo.Add(MakeEmptyRoll());

                while (nRemCash > 0)
                {
                    CGDCScoreInfo remScoreInfo = null;
                    List<CGDCScoreInfo> lstNormalScore = lstRemScoreInfo.FindAll(value => value.m_nScore <= nRemCash);
                    if (lstNormalScore == null || lstNormalScore.Count == 0)
                    {
                        remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value => value.m_nScore == 100));
                        nRemCash = 0;
                    }
                    else
                    {
                        int nScore = lstNormalScore.Max(value => value.m_nScore);
                        remScoreInfo = CGlobal.RandomSelect(lstRemScoreInfo.FindAll(value => value.m_nScore == nScore));
                        nRemCash -= nScore;
                    }
                    if(remScoreInfo != null)
                    {
                        if (remScoreInfo.m_nEvent > 0)
                            CompleteEventScoreInfo(remScoreInfo);

                        m_lstEndInfo.Add(remScoreInfo);
                    }
                    
                    m_lstEndInfo.Add(MakeEmptyRoll());
                }
            }
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nJackCont = 0;
            if (clsItem.m_nItemModel == CItemModel.ITEM_GDC_BUTTER)
            {
                nJackCont = 5;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_GDC_JOKER)
            {
                nJackCont = 12;
            }


            List<CGDCJackpotInfo> lstJackpotInfo = m_lstJackpotInfo.FindAll(value => value.m_nStartCash <= nItemCash && value.m_nCont == nJackCont);
            if (lstJackpotInfo == null || lstJackpotInfo.Count == 0)
            {
                //가짜예시이다.
                CGDCScoreInfo scoreInfo = CGlobal.RandomSelect(m_lstGDCRaise1Jack.FindAll(value=>value.m_nTile == BUTTER));
                if (scoreInfo.m_nScore > nItemCash)
                {
                    CGlobal.SetItemEngineRemCash(nItemCash);
                }
                else
                {
                    if (scoreInfo.m_nEvent > 0)
                    {
                        CompleteEventScoreInfo(scoreInfo);
                    }

                    m_raiseInfo = new CGDCRaiseInfo();
                    m_raiseInfo.m_nRemCash = scoreInfo.m_nScore - scoreInfo.m_nScore1;
                    int nScore = scoreInfo.m_nScore;
                    scoreInfo.m_nScore = scoreInfo.m_nScore1;
                    m_raiseInfo.m_scoreInfo = scoreInfo;

                    m_lstPrizeInfo.Clear();
                    m_lstPrizeInfo.Add(scoreInfo);
                    m_lstPrizeInfo.Add(MakeEmptyRoll());
                    m_nGearJack = 2;

                    CGlobal.SetItemEngineRemCash(nItemCash - nScore);
                }
            }
            else
            {
                //점수에 해당한 잭팟을 주어야 한다.
                int nPrizeCash = Convert.ToInt32(Math.Truncate((double)nItemCash / 10000)) * 10000;
                CGDCJackpotInfo jackpotInfo = CGlobal.RandomSelect(lstJackpotInfo);
                SetGdcPrizeCall(jackpotInfo.m_nCont, nPrizeCash);
                CGlobal.SetItemEngineRemCash(nItemCash - nPrizeCash);

                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, jackpotInfo.m_nCont, 4, 0);
            }
        }
    }
}
