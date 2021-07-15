using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CSwkGear : CBaseGear
    {
        private CSWKJackpotInfo m_prizeInfo;                        //손오공잭팟정보
        private List<CSwkScoreInfo> m_lstSwkScoreInfo;              //빈돌기까지 포함한 완전한 돌기정보
        private List<CSwkScoreInfo> m_lstSwkPrizeInfo;              //빈돌기까지 포함한 잭팟돌기 정보
        private List<CSwkScoreInfo> m_lstSwkReturnGift;             //잭팟이 끝난다음에 잭팟금액을 돌려주는 부분
        private int m_nReturnCash;                                  //잭팟이 끝난다음에 돌려주어야 하는 점수

        private int m_nItemCash;                                    //아이템을 맞아 올라가는 점수
        private bool m_bReturnGift;                                 //잭팟이 끝난다음에 기프트머니를 돌려주는 중이다.

        private CSwkScoreInfo m_clsSendScoreInfo;


        public CSwkGear() : base()
        {
            m_nSpeedCash = 100;
            m_lstSwkScoreInfo = new List<CSwkScoreInfo>();
            m_lstSwkPrizeInfo = new List<CSwkScoreInfo>();
            m_lstSwkReturnGift = new List<CSwkScoreInfo>();


        }

        public CSWKJackpotInfo GetSWKJackpotInfo()
        {
            return m_prizeInfo;
        }

        public override void ClearPrizeInfo()
        {
            m_bReturnGift = false;
            m_nGearJack = 0;
            SetGiveStep(0);

            m_prizeInfo = null;
            m_lstSwkPrizeInfo.Clear();
            m_lstSwkReturnGift.Clear();
        }


        public void SetSWKJackpotInfo(int nJackCont, int nJackTy, int nJackCash, int nActScore, int nEvalScore)
        {
            m_prizeInfo = new CSWKJackpotInfo();

            m_prizeInfo.m_nJackCont = nJackCont;    //잭팟내용 1-손오공, 2-저팔계, 3-사오정, 4-삼장, 5-삼칠별타, 6-프리게임
            m_prizeInfo.m_nJackTy = nJackTy;        //잭팟형식 1-가짜예시, 2-정상예시, 3-돌발, 3-4불상, 5-삼칠별타, 6-프리게임
            m_prizeInfo.m_nJackCash = nJackCash;    //잭팟캐시   
            m_prizeInfo.m_nActScore = nActScore;    //0-예시액션내 점수주지 않기, 1-예시액션내 점수주기(액션돌기할때 일반점수를 주겠는가 말겠는가를 결정)
            m_prizeInfo.m_nEvilScore = nEvalScore;  //0-불상액션내 점수주지 않기, 1-불상액션내 점수주기(불상출현 액션시 일반점수를 주겠는가 말겠는가를 결정)

            m_nJackCash += nJackCash;
        }

        public override void ClearGear()
        {
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            m_nReturnCash = 0;
            m_bReturnGift = false;
            m_lstSwkScoreInfo.Clear();
            m_lstSwkPrizeInfo.Clear();
            m_lstSwkReturnGift.Clear();
            m_prizeInfo = null;
        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<CSwkScoreInfo> lstSwkScoreInfo = new List<CSwkScoreInfo>();
            int nTempCash = nAddCash;

            while (nTempCash > 0)
            {
                int nAppenCash = nTempCash >= 10000 ? 10000 : nTempCash;
                nTempCash -= nAppenCash;
                int nDstCash = (int)(nAppenCash * rate / 100);
                if(m_nItemCash <= nDstCash)
                {
                    nDstCash -= m_nItemCash;
                    m_nItemCash = 0;
                }
                else
                {
                    nDstCash = 0;
                    m_nItemCash -= nDstCash;

                }

                //점수렬을 만든다.
                List<int> lstScore = new List<int>();
                MakeNormalScoreList(nDstCash, lstScore);

                List<CSwkScoreInfo> lstScoreInfo = new List<CSwkScoreInfo>();
                //점수에 해당한 SwkScoreTableInfo리스트를 구한다.
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CSwkScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CSwkScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstSwkScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstSwkScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CSwkScoreInfo scoreInfo = MakeEmptyOneRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstSwkScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstSwkScoreInfo.Count);
                    lstSwkScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                m_lstSwkScoreInfo.Clear();
            }

            m_lstSwkScoreInfo.AddRange(lstSwkScoreInfo);
        }

        public override bool MakePrizeRoll()
        {
            if (m_nGearJack == 1)
                return false;
            if (m_prizeInfo == null)
                return false;
            if (m_prizeInfo.m_nJackTy != 1 && m_prizeInfo.m_nJackCash == 0)
                return false;

            //잭팟인 경우에는 m_lstScoreInfo 가 아니라 m_lstPrizeInfo 에 정보를 보관한다.
            m_lstSwkPrizeInfo.Clear();
            //잭팟시작
            MakePrizeCommand(PRIZE_START);
            //빈돌기 3~4회 생성
            MakeEmptyRoll(2, 4);

            if (m_prizeInfo.m_nJackTy == 1 || m_prizeInfo.m_nJackTy == 2)
            {
                //정상예시, 가짜예시
                MakeNormalPrizeRoll();
                if (m_prizeInfo.m_nJackTy == 1)
                {
                    //잭팟 끝
                    MakePrizeCommand(PRIZE_END);

                    //기대를 잭팟중으로 설정 
                    m_nGearJack = 1;
                    return true;
                }
            }
            else if (m_prizeInfo.m_nJackTy == 3)
            {
                //돌발
                MakeSuprisePrize1Roll();
            }
            else if (m_prizeInfo.m_nJackTy == 4)
            {
                //불상
                MakeEvilPrizeRoll();
            }
            else if (m_prizeInfo.m_nJackTy == 5)
            {
                //삼칠별타

            }
            else if (m_prizeInfo.m_nJackTy == 6)
            {
                //프리잭팟

            }


            switch (m_prizeInfo.m_nJackCont)
            {
                case SWK_PRIZE_SONWUKONG:
                    MakeSonWuKongPrizeRoll();
                    break;
                case SWK_PRIZE_JOPALGYE:
                    MakeJoPalGyePrizeRoll();
                    break;
                case SWK_PRIZE_SAOJONG:
                    MakeSaOJongPrizeRollX2();
                    break;
                case SWK_PRIZE_SAMJANG:
                    MakeSamJangPrizeRoll();
                    break;
                case SWK_PRIZE_THREESEVEN:
                    MakeThreeSevenPrizeRoll();
                    break;
                case SWK_PRIZE_FREEEVENT:
                    MakeFreeEvenPrizeRoll();
                    break;
            }

            //잭팟 끝
            MakePrizeCommand(PRIZE_END);
            m_lstSwkPrizeInfo.Add(MakeEmptyOneRoll());

            //기대를 잭팟중으로 설정 
            m_nGearJack = 1;

            m_dtLastJackTime = CMyTime.GetMyTime();
            m_nLastJackCash = m_prizeInfo.m_nJackCash;
            if (m_prizeInfo.m_nJackCash > m_nTopJackCash)
                m_nTopJackCash = m_prizeInfo.m_nJackCash;

            //if (m_prizeInfo.m_nJackCash < 20 * 10000)
            //    m_nMiniCount++;
            //else if (m_prizeInfo.m_nJackCash < 50 * 10000)
            //    m_nMinorCount++;
            //else if (m_prizeInfo.m_nJackCash < 70 * 10000)
            //    m_nMajorCount++;
            //else
            //    m_nGrandCount++;

            CDataBase.SaveGearInfoToDB(this);


            return true;
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            m_nGiftCash += nCash;
            m_nAccuCash -= nCash;

            CUser user = CGlobal.GetUserByCode(m_nTakeUser);
            CAgent agent = CGlobal.GetAgentByCode(user.m_nAgenCode);
            if (nCash > 0)
            {
                //if (agent.m_nAgenLevel < 10 && user.m_nChargeCnt > 0)
                //    user.m_nUserWinCash += nCash;

                if (nKind == 0)
                {
                    CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, 0, m_nTakeUser);
                }
                else if(!m_bReturnGift)
                {
                    CGlobal.GetGameEngineByCode(m_nGameCode).AddGivePot(nCash, m_nStep, m_nTakeUser);
                }
            }


            if (m_nGiftCash >= 20000)
            {
                int nExCount = m_nGiftCash / 20000;

                if (user.m_nChargeCnt > 0)
                {
                    string strNote = user.m_strUserNick + "님이 사용하는 " + CGlobal.GetGameNameByGameCode(m_nGameCode) + m_nGearNum + "기대에서 기프트머니 쿠폰으로 전환";
                    CDataBase.InsertCacheToDB(user.m_nUserCode, 0, nExCount * 4, user.m_nUserCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn + nExCount * 4, strNote);
                    user.m_nUserCupn += (nExCount * 4);
                    //user.m_nUserWinCash = user.m_nUserWinCash - (nExCount * 4 * 500);
                }
                else
                {
                    user.m_nVirtualCupn += (nExCount * 4);
                }

                m_nGiftCash -= nExCount * 20000;

                if (m_nGearJack == 1)
                {
                    m_nReturnCash += m_nGiftCash;
                    m_nGiftCash = 0;
                }

                clsSocket.SendExcupn(m_nGearCode, nExCount * 4);
            }

            if (nKind == 0)
            {
                if(nCash == 100)
                {
                    m_nItemCash += 100;
                }

                //염주아이템을 맞아 올린 점수가 존재한다면 점수리스트에서 해당 맞는 점수를 없애야 한다.
                List<CSwkScoreInfo> lstScoreInfo = m_lstSwkScoreInfo.FindAll(value => value.m_nScore <= m_nItemCash).OrderByDescending(value => value.m_nScore).ToList();
                if (lstScoreInfo != null && lstScoreInfo.Count > 0)
                {
                    CSwkScoreInfo scoreInfo = lstScoreInfo[0];
                    m_nItemCash -= scoreInfo.m_nScore;
                    m_lstSwkScoreInfo.Remove(scoreInfo);
                    m_lstSwkScoreInfo.Add(MakeEmptyOneRoll());
                }
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

        public override void OnCreateCoin(CUserSocket clsSocket)
        {
            if (m_nGearRun == 0)
                return;

            if (m_nSlotCash < 100)
            {
                OnAddGiftCash(clsSocket, m_nSlotCash - 100, 2);
                m_nSlotCash = 0;
            }
            else
            {
                m_nSlotCash -= 100;
                m_nAccuCash += 100; 
            }

            CGlobal.CalcShowJackPot(100, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(100, m_nTakeUser);//누적금액을 사용한 금액만큼 증가시킨다.

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
                OnAddGiftCash(clsSocket, m_clsSendScoreInfo.m_nScore, 1);
            }

            if (m_lstSwkPrizeInfo.Count > 0)
            {
                //잭팟일때이다.
                m_clsSendScoreInfo = m_lstSwkPrizeInfo[0];
                m_lstSwkPrizeInfo.RemoveAt(0);

                if (m_lstSwkPrizeInfo.Count == 0)
                {
                    ReturnGiftCash();
                    if (m_lstSwkReturnGift.Count == 0)
                    {
                        OnEndPrizeCall();
                    }
                }
            }
            else if(m_lstSwkReturnGift.Count > 0)
            {
                m_clsSendScoreInfo = m_lstSwkReturnGift[0];
                m_lstSwkReturnGift.RemoveAt(0);
                if(m_lstSwkReturnGift.Count == 0)
                {
                    OnEndPrizeCall();
                }
            }
            else if (m_lstSwkScoreInfo.Count > 0)
            {
                m_clsSendScoreInfo = m_lstSwkScoreInfo[0];
                m_lstSwkScoreInfo.RemoveAt(0);
            }
            else
            {
                m_clsSendScoreInfo = MakeEmptyOneRoll();
                m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
            }
            m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
            CScorePacket scorePacket = new CScorePacket();
            scorePacket.nGearCode = m_nGearCode;
            scorePacket.strPktData = JsonConvert.SerializeObject(m_clsSendScoreInfo);

            clsSocket.SendScoreInfo(scorePacket);
            OnBroadCastPrizeKind();
        }

        public override void OnEndPrizeCall()
        {
            if (m_nGearJack == 2)
            {
                ClearPrizeInfo();
                return;
            }

            if (m_prizeInfo == null)
                return;

            if(m_prizeInfo.m_nJackCash > 0)
            {
                string[] lstPrizeKind = { "", "손오공", "저팔계", "사오정", "삼장법사", "삼칠별타", "제천대성", "제천대성", "제천대성", "제천대성" };
                string strMessage = MakeCongration(lstPrizeKind[m_prizeInfo.m_nJackCont], m_prizeInfo.m_nJackCash);
                CGlobal.SendNoticeBroadCast(strMessage);

                CGlobal.CalculateJackPotCash(m_prizeInfo.m_nJackCash);
                OnBroadCastPrizeInfo();
            }

            ClearPrizeInfo();
            CDataBase.SaveGearInfoToDB(this);
        }

        public override void OnBroadCastPrizeInfo()
        {
            if (m_prizeInfo == null)
                return;
            if (m_prizeInfo.m_nJackCash == 0)
                return;

            string strNick = string.Empty;
            if(m_nTakeUser > 0)
            {
                strNick = CGlobal.GetUserNickByCode(m_nTakeUser);
            }
            else if(m_nTakeRobot > 0)  {
                strNick = CGlobal.GetRobotNickByCode(m_nTakeRobot);
            }


            CPrizeInfoBroadCast clsPrizeInfoPacket = new CPrizeInfoBroadCast();
            clsPrizeInfoPacket.m_strUserNick = strNick;
            clsPrizeInfoPacket.m_nGameCode = m_nGameCode;
            clsPrizeInfoPacket.m_nGearCode = m_nGearCode;
            clsPrizeInfoPacket.m_nGearNum = m_nGearNum;
            clsPrizeInfoPacket.m_nPrizeKind = m_prizeInfo.m_nJackCont;
            clsPrizeInfoPacket.m_nPrizeCash = m_prizeInfo.m_nJackCash;

            CGlobal.BroadCastPrizeInfo(clsPrizeInfoPacket);
        }

        public override void OnBroadCastPrizeKind()
        {
            if (m_prizeInfo == null)
                return;

            int nCmd = m_clsSendScoreInfo.m_nPrizeCmd;
            if (nCmd == 0)
                return;

            string strMessage = string.Empty;
            string strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);

            switch (nCmd)
            {
                case PRIZE_SWK_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]손오공[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_JPG_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]저팔계[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_SOJ_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]사오정[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_SMJ_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]삼장법사[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_TSS_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]삼칠별타[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_EVIL_SLIDE:
                    if(GetSWKJackpotInfo().m_nJackCont == 6)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][EC0DB0]제천대성[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_END:
                    break;
            }

            if (strMessage != string.Empty)
                CGlobal.SendNoticeBroadCast(strMessage);
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            List<SWKScoreTableInfo> lstScoreTable = m_lstSWKNormalScoreTable.ToList().FindAll(value => value.nGetScore <= nJackCash).OrderByDescending(value => value.nGetScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nScore = lstScoreTable[0].nGetScore;
                List<SWKScoreTableInfo> tempTableInfo = lstScoreTable.FindAll(value => value.nGetScore == nScore);
                CSwkScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tempTableInfo[RND.Next(tempTableInfo.Count)]);
                m_lstSwkPrizeInfo.Add(scoreInfo);

                CDataBase.InsertNationalToDB(m_nGearCode, scoreInfo.m_nScore, 1);
                nJackCash -= scoreInfo.m_nScore;
                SetGiveStep(1);
                m_nGearJack = 2;

                m_lstSwkPrizeInfo.Add(MakeEmptyOneRoll());
            }

            return nJackCash;
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            m_lstSwkPrizeInfo.Clear();

            CGameEngine swkEngine = CGlobal.GetEngine(m_nGameCode);
            if (swkEngine.GetGivePrizeCount() > swkEngine.m_nEmptyPrize)
            {
                //잭팟형식 1-가짜예시, 2-정상예시, 3-돌발, 3-4불상, 5-삼칠별타, 6-프리게임
                SetSWKJackpotInfo(1, 1, 0, 0, 0);
                MakePrizeRoll();
                swkEngine.SetGivePrizeCount(0);

                return nJackCash;  //가짜예시 준다.
            }


            List<SWKJackType> lstJackType = m_lstSWKJackType.FindAll(value => value.m_nJackScore <= nJackCash).OrderByDescending(value => value.m_nJackScore).ToList();
            if (lstJackType.Count > 0)
            {
                int nScore = lstJackType[0].m_nJackScore;
                List<SWKJackType> lstTempJackType = lstJackType.FindAll(value => value.m_nJackScore == nScore);
                SWKJackType swkJackType = lstTempJackType[RND.Next(lstTempJackType.Count)];

                int nJackCont = swkJackType.m_nJackCont;
                int nJackTy = 0;
                if (nJackCont < 5)
                {
                    int nRnd = RND.Next(3);
                    if (nRnd == 0)
                        nJackTy = 2;
                    else if (nRnd == 1)
                        nJackTy = 3;
                    else
                        nJackTy = 2;
                }
                else if (nJackCont == 5)
                {
                    nJackTy = 5;
                }
                else if (nJackCont == 6)
                {
                    nJackTy = 6;
                }

                SetSWKJackpotInfo(nJackCont, nJackTy, swkJackType.m_nJackScore, 0, 0);
                bool ret = MakePrizeRoll();
                if (ret)
                {
                    CDataBase.InsertNationalToDB(m_nGearCode, swkJackType.m_nJackScore, 2);
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, swkJackType.m_nJackScore, swkJackType.m_nJackCont, 2, 0);
                    SetGiveStep(2);

                    nJackCash -= swkJackType.m_nJackScore;
                }
            }

            return nJackCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            CGameEngine swkEngine = CGlobal.GetEngine(m_nGameCode);
            if (swkEngine.GetGivePrizeCount() > swkEngine.m_nEmptyPrize)
            {
                //잭팟형식 1-가짜예시, 2-정상예시, 3-돌발, 3-4불상, 5-삼칠별타, 6-프리게임
                SetSWKJackpotInfo(1, 1, 0, 0, 0);
                MakePrizeRoll();
                swkEngine.SetGivePrizeCount(0);

                return nJackCash;  //가짜예시 준다.
            }



            List<SWKJackType> lstJackType = m_lstSWKJackType.FindAll(value => value.m_nJackScore <= nJackCash).OrderByDescending(value => value.m_nJackScore).ToList();
            if (lstJackType.Count > 0)
            {
                int nScore = lstJackType[0].m_nJackScore;
                List<SWKJackType> lstTempJackType = lstJackType.FindAll(value => value.m_nJackScore == nScore);
                SWKJackType swkJackType = lstTempJackType[RND.Next(lstTempJackType.Count)];

                int nJackCont = swkJackType.m_nJackCont;
                int nJackTy = 0;
                if (nJackCont < 5)
                {
                    int nRnd = RND.Next(3);
                    if (nRnd == 0)
                        nJackTy = 2;
                    else if (nRnd == 1)
                        nJackTy = 3;
                    else
                        nJackTy = 2;
                }
                else if (nJackCont == 5)
                {
                    nJackTy = 5;
                }
                else if (nJackTy == 6)
                {
                    nJackTy = 6;
                }

                SetSWKJackpotInfo(nJackCont, nJackTy, swkJackType.m_nJackScore, 0, 0);
                bool ret = MakePrizeRoll();
                if (ret)
                {
                    CDataBase.InsertNationalToDB(m_nGearCode, swkJackType.m_nJackScore, 3);
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, swkJackType.m_nJackScore, swkJackType.m_nJackCont, 3, 0);
                    SetGiveStep(3);
                    nJackCash -= swkJackType.m_nJackScore;
                }
            }

            return nJackCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;


            List<SWKJackType> lstJackType = m_lstSWKJackType.FindAll(value => value.m_nJackScore <= nJackCash).OrderByDescending(value => value.m_nJackScore).ToList();
            if (lstJackType.Count > 0)
            {
                int nScore = lstJackType[0].m_nJackScore;
                List<SWKJackType> lstTempJackType = lstJackType.FindAll(value => value.m_nJackScore == nScore);
                SWKJackType swkJackType = lstTempJackType[RND.Next(lstTempJackType.Count)];

                int nJackCont = swkJackType.m_nJackCont;
                int nJackTy = 0;
                if (nJackCont < 5)
                {
                    int nRnd = RND.Next(3);
                    if (nRnd == 0)
                        nJackTy = 2;
                    else if (nRnd == 1)
                        nJackTy = 3;
                    else if (nRnd == 2)
                        nJackTy = 4;
                }
                else if (nJackCont == 5)
                {
                    nJackTy = 5;
                }
                else if (nJackCont == 6)
                {
                    nJackTy = 6;
                }

                SetSWKJackpotInfo(nJackCont, nJackTy, swkJackType.m_nJackScore, 0, 0);
                bool ret = MakePrizeRoll();
                if (ret)
                {
                    
                    nJackCash -= swkJackType.m_nJackScore;
                    CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                    if(robot != null)
                    {
                        CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, swkJackType.m_nJackScore, swkJackType.m_nJackCont, -1, 1);
                        robot.PlaySwkPrize();
                    }
                        
                }
            }

            return nJackCash;
        }

        //들어온점수를 여러개의 점수배렬로 나누는 함수
        private void MakeNormalScoreList(int nDstCash, List<int> lstScore)
        {
            int idx = 0;
            int nDelta = nDstCash / 2;
            for (int i = 0; i < m_lstSWKScores.Length; i++)
            {
                if (nDelta <= m_lstSWKScores[i])
                {
                    idx = i + 1;
                    break;
                }
            }

            while (nDstCash >= 100)
            {
                int nScore = 0;
                while (true)
                {
                    int nScoreIndex = RND.Next(0, idx);
                    nScore = m_lstSWKScores[nScoreIndex];
                    if (nDstCash >= nScore)
                        break;
                }
                lstScore.Add(nScore);
                nDstCash -= nScore;
            }
        }

        //점수로부터 일반점수정보 만들기  
        private CSwkScoreInfo MakeNormalScore(int nScore)
        {
            CSwkScoreInfo scoreInfo = null;

            if (nScore == 0)
                return scoreInfo;
            //배당설정
            int nMult = MakeSwkMult(nScore);
            int nTempScore = nScore / nMult;
            //점수테이블리스트를 만든다.
            List<SWKScoreTableInfo> lstMatchScoreInfo;

            lstMatchScoreInfo = m_lstSWKNormalScoreTable.ToList().FindAll(value => value.nGetScore == nTempScore);

            SWKScoreTableInfo scoreTable = lstMatchScoreInfo[RND.Next(0, lstMatchScoreInfo.Count)];
            scoreInfo = ConvertScoreTableToScoreInfo(scoreTable, nMult);

            return scoreInfo;
        }

        //들어온 점수를 가지고 배수를 랜덤으로 구하는 함수
        private int MakeSwkMult(int nDstScore)
        {
            int nLimIdx = m_lstSWKMults.Length;
            for (int j = 0; j < m_lstSWKMults.Length; j++)
            {
                int nDelta = nDstScore / m_lstSWKMults[j];
                if (nDelta < 100)
                {
                    nLimIdx = j;
                    break;
                }
            }

            int nMult = 1; //배당
            int nTempScore = 0;
            while (true)
            {
                int nMultIndex = RND.Next(0, nLimIdx);  //배당인덱스
                nMult = m_lstSWKMults[nMultIndex];  //배당
                nTempScore = nDstScore / nMult;

                if (m_lstSWKScores.ToList().Exists(value => value == nTempScore))
                    break;

            }

            return nMult;
        }

        //스코테이블로부터 스코정보객체를 얻는 함수  kind=1 올바점수돌기, 2 과일점수주기
        private CSwkScoreInfo ConvertScoreTableToScoreInfo(SWKScoreTableInfo swkScoreTableInfo, int nMult = 1)
        {
            CSwkScoreInfo scoreInfo = new CSwkScoreInfo();
            int nRow = swkScoreTableInfo.nLineIndex;
            int nCnt = swkScoreTableInfo.nCount;
            int nLimit = swkScoreTableInfo.nLimit;
            int nStart = 0;
            if (nLimit == -1)
            {
                nStart = 4 - nCnt;
            }
            else
            {
                nStart = RND.Next(0, nLimit);
            }


            int nScore = swkScoreTableInfo.nGetScore * nMult;
            if (nScore > 20000)
                nScore = 20000;

            for (int j = 0; j < 4; j++)
            {
                int nCol = nStart + j;
                if (nCol >= 4)
                    nCol -= 4;
                int nTile = swkScoreTableInfo.lstTileNote[j];
                CTile tile = new CTile(nTile, nRow, nCol);
                scoreInfo.m_lstTile.Add(tile);
            }

            scoreInfo.m_nMult = nMult;
            scoreInfo.m_nScore = nScore;

            return scoreInfo;
        }

        
        //빈돌기 만드는 함수 nKind = 1 작은불상타일 릴돌기에 출현
        private void MakeEmptyRoll(int nFrom, int nTo)
        {
            int nCount = RND.Next(nFrom, nTo);
            List<CSwkScoreInfo> lstEmpty = new List<CSwkScoreInfo>();

            for (int i = 0; i < nCount; i++)
            {
                CSwkScoreInfo scoreInfo = MakeEmptyOneRoll();
                m_lstSwkPrizeInfo.Add(scoreInfo);
            }
        }

        private CSwkScoreInfo MakeEmptyOneRoll()
        {
            CSwkScoreInfo scoreInfo = new CSwkScoreInfo();
            scoreInfo.m_nScore = 0;
            scoreInfo.m_nMult = CGlobal.RandomSelect(m_lstSWKMults.ToList());

            return scoreInfo;
        }

        //잭팟명령생성
        private void MakePrizeCommand(int nCmd)
        {
            CSwkScoreInfo cmdInfo = MakeEmptyOneRoll();
            cmdInfo.m_nPrizeCmd = nCmd;
            m_lstSwkPrizeInfo.Add(cmdInfo);
        }

        //정상예시, 가짜예시, 잭팟예시 공통부분생성
        private void MakeNormalPrizeRoll()
        {
            //불상현시부분생성 28 ~ 32 회  여느때보다 큰 점수로 일반점수를 준다.
            //불상출현명령생성
            MakePrizeCommand(PRIZE_EVIL_SHOW);
            if (m_prizeInfo.m_nEvilScore == 0)
            {
                //불상출현시에 점수를 주지 않는다.
                MakeEmptyRoll(50, 51);
            }
            else
            {
                //불상이 출현하는 동안 점수를 주어야 한다.
                //이때 점수는 300 ~ 1000사이에서 주어야 한다.
                List<int> lstEvilScore = m_lstSWKScores.ToList().FindAll(value => value >= 300 && value <= 1000);
                int nRound = RND.Next(28, 33);

                for (int i = 0; i < nRound; i++)
                {
                    if (RND.Next(0, 10) == 7)
                    {
                        //점수주기
                        int nScore = lstEvilScore[RND.Next(0, lstEvilScore.Count)];
                        CSwkScoreInfo scoreInfo = MakeNormalScore(nScore);
                        m_lstSwkPrizeInfo.Add(scoreInfo);
                    }
                    else
                    {
                        //빈돌기 한개를 추가한다.
                        m_lstSwkPrizeInfo.Add(MakeEmptyOneRoll());
                    }
                }
            }

            if (m_prizeInfo.m_nJackTy == 2)
            {
                //정상예시라면 불상웃기 진행
                //불상웃기명령생성
                MakePrizeCommand(PRIZE_EVIL_LAUGHT);
                //앵두예시생성
                CharrySuggest();
            }

            //작은불상사라짐이벤트설정
            CSwkScoreInfo scoreInfo1 = new CSwkScoreInfo();
            scoreInfo1.m_nScore = -4;
            scoreInfo1.m_nMult = RND.Next(0, m_lstSWKMults.Length);
            scoreInfo1.m_nPrizeCmd = PRIZE_EVIL_FLY;
            lock (m_lstSwkPrizeInfo)
                m_lstSwkPrizeInfo.Add(scoreInfo1);
            //불상오르기
            MakeEmptyRoll(3, 5);
        }

        //잭팟암시: 3~4 회 빈돌기중 무배당으로 앵두 X 1, 2, 3 중 하나를 보여준다
        private void CharrySuggest()
        {
            //체리암시 회전수를 설정한다.
            int nRound = RND.Next(3, 5);
            //암시 앵두리스트를 구한다.
            List<SWKScoreTableInfo> lstCharryTable = m_lstSWKNormalScoreTable.ToList().FindAll(value => value.lstTileNote[0] == 11);

            int nSuIndex = RND.Next(0, nRound);
            int nChIndex = RND.Next(0, lstCharryTable.Count);
            for (int i = 0; i < nRound; i++)
            {
                CSwkScoreInfo scoreInfo = null;
                if (i == nSuIndex)
                {
                    scoreInfo = ConvertScoreTableToScoreInfo(lstCharryTable[nChIndex]);
                }
                else
                    scoreInfo = MakeEmptyOneRoll();

                m_lstSwkPrizeInfo.Add(scoreInfo);
            }

        }

        //돌발-1, 돌발-2
        private void MakeSuprisePrize1Roll()
        {
            //암시 앵두리스트를 구한다.
            //체리1추가
            List<SWKScoreTableInfo> lstCharryTable1 = m_lstSWKNormalScoreTable.ToList().FindAll(value => (value.lstTileNote[0] == 11 && value.lstTileNote[1] == 0));
            CSwkScoreInfo charry1 = ConvertScoreTableToScoreInfo(lstCharryTable1[RND.Next(0, lstCharryTable1.Count)], 1);
            m_lstSwkPrizeInfo.Add(charry1);
            MakeEmptyRoll(2, 4);
            //체리2추가
            List<SWKScoreTableInfo> lstCharryTable2 = m_lstSWKNormalScoreTable.ToList().FindAll(value => (value.lstTileNote[0] == 11 && value.lstTileNote[1] == 11 && value.lstTileNote[2] == 0));
            CSwkScoreInfo charry2 = ConvertScoreTableToScoreInfo(lstCharryTable2[RND.Next(0, lstCharryTable2.Count)], 1);
            m_lstSwkPrizeInfo.Add(charry2);
            MakeEmptyRoll(2, 4);
            //체리3추가
            List<SWKScoreTableInfo> lstCharryTable3 = m_lstSWKNormalScoreTable.ToList().FindAll(value => (value.lstTileNote[0] == 11 && value.lstTileNote[1] == 11 && value.lstTileNote[2] == 11 && value.lstTileNote[3] == 0));
            CSwkScoreInfo charry3 = ConvertScoreTableToScoreInfo(lstCharryTable3[RND.Next(0, lstCharryTable3.Count)], 1);
            m_lstSwkPrizeInfo.Add(charry3);
            MakeEmptyRoll(2, 4);
            //빈돌기 2 ~ 3 회 추가
            MakeEmptyRoll(3, 5);
        }

        //불상
        private void MakeEvilPrizeRoll()
        {
            //불상현시부분생성 28 ~ 32 회  여느때보다 큰 점수로 일반점수를 준다.
            CSwkScoreInfo evilInfo = MakeEmptyOneRoll();
            evilInfo.m_nPrizeCmd = PRIZE_EVIL_SLIDE;
            m_lstSwkPrizeInfo.Add(evilInfo);

            if (m_prizeInfo.m_nEvilScore == 0)
            {
                //불상출현시에 점수를 주지 않는다.
                MakeEmptyRoll(28, 33);
            }
            else
            {
                //불상이 출현하는 동안 점수를 주어야 한다.
                //이때 점수는 300 ~ 1000사이에서 주어야 한다.
                List<int> lstEvilScore = m_lstSWKScores.ToList().FindAll(value => value >= 300 && value <= 1000);
                int nRound = RND.Next(28, 33);

                for (int i = 0; i < nRound; i++)
                {
                    if (RND.Next(0, 5) == 3)
                    {
                        //점수주기
                        int nScore = lstEvilScore[RND.Next(0, lstEvilScore.Count)];
                        CSwkScoreInfo scoreInfo = MakeNormalScore(nScore);
                        m_lstSwkPrizeInfo.Add(scoreInfo);
                    }
                    else
                    {
                        //빈돌기를 추가한다.
                        m_lstSwkPrizeInfo.Add(MakeEmptyOneRoll());
                    }
                }
            }


            //정상예시라면 불상웃기 진행
            //불상웃기명령생성
            MakePrizeCommand(PRIZE_EVIL_LAUGHT);

            //앵두예시생성
            CharrySuggest();

            //불상없어지기
            MakePrizeCommand(PRIZE_EVIL_HIDE);
            MakeEmptyRoll(2, 4);
        }

        //점수에 해당한 점수표배렬을 파라메터를 받아 점수객체를 생성하여 되돌리는 함수(잭팟예시점수를 만들때 사용)
        private CSwkScoreInfo MakePrizeHintScore(int nMult, List<SWKScoreTableInfo> lstScoreTable1, List<SWKScoreTableInfo> lstScoreTable2 = null, int nKind = 0)
        {
            SWKScoreTableInfo tableInfo1 = CGlobal.RandomSelect(lstScoreTable1);
            CSwkScoreInfo info = ConvertScoreTableToScoreInfo(tableInfo1);
            info.m_nMult = nMult;
            info.m_nScore = 20000;

            if (lstScoreTable2 != null)
            {
                SWKScoreTableInfo tableInfo2;
                while (true)
                {
                    tableInfo2 = lstScoreTable2[RND.Next(lstScoreTable2.Count)];
                    if (tableInfo1.nLineIndex != tableInfo2.nLineIndex)
                        break;
                }

                AddScoreTableToScoreInfo(tableInfo2, info);
            }

            return info;
        }

        //스코객체에 스코테이블정보를 추가한다.
        private void AddScoreTableToScoreInfo(SWKScoreTableInfo swkScoreTableInfo, CSwkScoreInfo info)
        {
            int nLimit = swkScoreTableInfo.nLimit;
            int nStart = RND.Next(0, nLimit);

            int nRow = swkScoreTableInfo.nLineIndex;

            for (int j = 0; j < 4; j++)
            {
                int nCol = nStart + j;
                if (nCol >= 4)
                    nCol -= 4;
                int nTile = swkScoreTableInfo.lstTileNote[j];
                CTile tile = new CTile(nTile, nRow, nCol);
                info.m_lstTile.Add(tile);
            }
        }

        //잭팟뒤점수 주는 함수
        private void MakeLatePrizeRoll(int nPrizeCash)
        {
            while (nPrizeCash >= 20000)
            {
                m_lstSwkPrizeInfo.Add(MakeLatePrizeScore(20000));
                nPrizeCash -= 20000;
                MakeEmptyRoll(0, 4);
            }
            if (nPrizeCash >= 15000)
            {
                m_lstSwkPrizeInfo.Add(MakeLatePrizeScore(15000));
                nPrizeCash -= 15000;
                MakeEmptyRoll(0, 4);
            }
            if (nPrizeCash >= 10000)
            {
                m_lstSwkPrizeInfo.Add(MakeLatePrizeScore(10000));
                nPrizeCash -= 10000;
                MakeEmptyRoll(0, 4);
            }

            for (int i = m_lstSWKScores.Length - 2; i >= 0; i--)
            {
                if (nPrizeCash > m_lstSWKScores[i])
                {
                    m_lstSwkPrizeInfo.Add(MakeNormalScore(m_lstSWKScores[i]));
                    nPrizeCash -= m_lstSWKScores[i];
                    MakeEmptyRoll(0, 4);
                }
            }
        }

        //잭팟뒤점수 만들기
        private CSwkScoreInfo MakeLatePrizeScore(int nScore)
        {
            CSwkScoreInfo scoreInfo = null;

            if (nScore == 0)
                return scoreInfo;
            //배당설정
            int nTempScore = nScore;
            //점수테이블리스트를 만든다.
            List<SWKScoreTableInfo> lstMatchScoreInfo = m_lstSWKNormalScoreTable.ToList().FindAll(value => value.nGetScore == nTempScore && (value.lstTileNote[0] == 5 || value.lstTileNote[0] == 6 || value.lstTileNote[0] == 7));

            if (nScore == 20000)
            {
                int nRnd = RND.Next(12 + lstMatchScoreInfo.Count);
                if (nRnd < 12)
                {
                    nRnd = RND.Next(5);
                    int[][] lstTempSpecScore;
                    lstTempSpecScore = m_lstSpecScore;


                    scoreInfo = new CSwkScoreInfo();
                    scoreInfo.m_nScore = 20000;
                    scoreInfo.m_nMult = 1;
                    int[] lstSpecScore = lstTempSpecScore[RND.Next(12)];

                    for (int i = 0; i < 12; i++)
                    {
                        if (lstSpecScore[i] > 0)
                        {
                            int nRow = i / 4;
                            int nCol = i % 4;
                            scoreInfo.m_lstTile.Add(new CTile(lstSpecScore[i], nRow, nCol));
                        }
                    }
                }
                else
                {
                    SWKScoreTableInfo scoreTable = lstMatchScoreInfo[RND.Next(0, lstMatchScoreInfo.Count)];
                    scoreInfo = ConvertScoreTableToScoreInfo(scoreTable, 1);
                }

            }
            else if (nScore == 10000)
            {
                scoreInfo = new CSwkScoreInfo();
                scoreInfo.m_nScore = 10000;
                scoreInfo.m_nMult = 1;
                int[] lstSpecScore = m_lstPrize10000Score[RND.Next(18)];

                for (int i = 0; i < 12; i++)
                {
                    if (lstSpecScore[i] > 0)
                    {
                        int nRow = i / 4;
                        int nCol = i % 4;
                        scoreInfo.m_lstTile.Add(new CTile(lstSpecScore[i], nRow, nCol));
                    }

                }
            }
            else
            {
                SWKScoreTableInfo scoreTable = lstMatchScoreInfo[RND.Next(0, lstMatchScoreInfo.Count)];
                scoreInfo = ConvertScoreTableToScoreInfo(scoreTable, 1);
            }

            return scoreInfo;
        }

        //애니매션동작 만들기
        private void MakeGodAmination(int nGod)
        {
            MakePrizeCommand(nGod);

            if (m_prizeInfo.m_nActScore == 0)
            {
                //애니매션시 점수를 주지 않는다.
                MakeEmptyRoll(15, 18);
            }
            else
            {
                //이때 점수는 300 ~ 1000사이에서 주어야 한다.
                List<int> lstSWKScore = m_lstSWKScores.ToList().FindAll(value => value >= 300 && value <= 1000);
                int nRound = RND.Next(15, 18);

                for (int i = 0; i < nRound; i++)
                {
                    if (RND.Next(0, 10) == 7)
                    {
                        //점수주기
                        int nScore = lstSWKScore[RND.Next(0, lstSWKScore.Count)];
                        CSwkScoreInfo scoreInfo = MakeNormalScore(nScore);
                        m_lstSwkPrizeInfo.Add(scoreInfo);
                    }
                    else
                    {
                        //빈돌기를 추가한다.
                        m_lstSwkPrizeInfo.Add(MakeEmptyOneRoll());
                    }
                }
            }

            //빈돌기 2 ~ 3 회 추가
            MakeEmptyRoll(2, 4);
            MakePrizeCommand(nGod + 0x01);
        }

        //손오공잭팟생성   20만부터 100만사이  100배로 점수를 보여준다.
        private void MakeSonWuKongPrizeRoll()
        {
            //손오공애니매션만들기
            MakeGodAmination(PRIZE_SWK_ANIMATION);

            //잭팟점수를 얻는다.
            int nPrizeCash = m_prizeInfo.m_nJackCash;


            //예시점수주기  체리, 7, 종으로 예시점수를 만든다.
            List<SWKScoreTableInfo> lstPredictScoreTable = m_lstSWKNormalScoreTable.ToList()
                .FindAll(value => value.lstTileNote[0] == 11 || value.lstTileNote[0] == 5 || value.lstTileNote[0] == 8);
            List<SWKScoreTableInfo> lstPredictScoreTable20000 = lstPredictScoreTable.FindAll(value => value.nGetScore == 20000);
            List<SWKScoreTableInfo> lstPredictScoreTable15000 = lstPredictScoreTable.FindAll(value => value.nGetScore == 15000);
            List<SWKScoreTableInfo> lstPredictScoreTable2000 = lstPredictScoreTable.FindAll(value => value.nGetScore == 2000);
            List<SWKScoreTableInfo> lstPredictScoreTable1000 = lstPredictScoreTable.FindAll(value => value.nGetScore == 1000);
            List<SWKScoreTableInfo> lstPredictScoreTable600 = lstPredictScoreTable.FindAll(value => value.nGetScore == 600);
            List<SWKScoreTableInfo> lstPredictScoreTable400 = lstPredictScoreTable.FindAll(value => value.nGetScore == 400);
            List<SWKScoreTableInfo> lstPredictScoreTable300 = lstPredictScoreTable.FindAll(value => value.nGetScore == 300);
            List<SWKScoreTableInfo> lstPredictScoreTable200 = lstPredictScoreTable.FindAll(value => value.nGetScore == 200);
            List<SWKScoreTableInfo> lstPredictScoreTable100 = lstPredictScoreTable.FindAll(value => value.nGetScore == 100);


            CSwkScoreInfo info = null;

            if (nPrizeCash == 5 * 10000) //100 X 5
            {
                info = MakePrizeHintScore(5, lstPredictScoreTable100);
            }
            else if (nPrizeCash == 6 * 10000) //200 X 3, 300 X 2
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                {
                    info = MakePrizeHintScore(3, lstPredictScoreTable200);
                    info.m_nScore = 200 * 3;
                }
                else
                {
                    info = MakePrizeHintScore(2, lstPredictScoreTable300);
                    info.m_nScore = 300 * 2;
                }
            }
            else if (nPrizeCash == 8 * 10000) //400 X 2
            {
                info = MakePrizeHintScore(2, lstPredictScoreTable400);
                info.m_nScore = 400 * 2;
            }
            else if (nPrizeCash == 9 * 10000) //300 X 3
            {
                info = MakePrizeHintScore(3, lstPredictScoreTable300);
                info.m_nScore = 300 * 3;
            }
            else if (nPrizeCash == 10 * 10000) //100 X 10, 200 X 5, 1000 X 1
            {
                int nRnd = RND.Next(3);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable100);
                else if (nRnd == 1)
                    info = MakePrizeHintScore(5, lstPredictScoreTable200);
                else
                    info = MakePrizeHintScore(1, lstPredictScoreTable1000);

                info.m_nScore = 1000;
            }
            else if (nPrizeCash == 12 * 10000) //400 X 3, 600 X 2
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(3, lstPredictScoreTable400);
                else
                    info = MakePrizeHintScore(2, lstPredictScoreTable600);

                info.m_nScore = 1200;
            }
            else if (nPrizeCash == 15 * 10000) //300 X 5
            {
                info = MakePrizeHintScore(5, lstPredictScoreTable300);
                info.m_nScore = 1500;
            }
            else if (nPrizeCash == 18 * 10000) //600 X 3
            {
                info = MakePrizeHintScore(3, lstPredictScoreTable600);
                info.m_nScore = 1800;
            }
            else if (nPrizeCash == 20 * 10000) //200 X 10, 400 X 5, 2000 X 1, 1000 X 2
            {
                int nRnd = RND.Next(4);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable200);
                else if (nRnd == 1)
                    info = MakePrizeHintScore(5, lstPredictScoreTable400);
                else if (nRnd == 2)
                    info = MakePrizeHintScore(2, lstPredictScoreTable1000);
                else
                    info = MakePrizeHintScore(1, lstPredictScoreTable2000);

                info.m_nScore = 2000;
            }
            else if (nPrizeCash == 30 * 10000) //300 X 10, 600 X 5, 1000 X 3
            {
                int nRnd = RND.Next(3);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable300);
                else if (nRnd == 1)
                    info = MakePrizeHintScore(5, lstPredictScoreTable600);
                else
                    info = MakePrizeHintScore(3, lstPredictScoreTable1000);

                info.m_nScore = 3000;
            }
            else if (nPrizeCash == 40 * 10000) //400 X 10, 2000 X 2
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable400);
                else
                    info = MakePrizeHintScore(2, lstPredictScoreTable2000);

                info.m_nScore = 4000;
            }
            else if (nPrizeCash == 50 * 10000) //1000 X 5
            {
                info = MakePrizeHintScore(5, lstPredictScoreTable1000);

                info.m_nScore = 5000;
            }
            else if (nPrizeCash == 60 * 10000) //600 X 10, 2000 X 3
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable600);
                else
                    info = MakePrizeHintScore(3, lstPredictScoreTable2000);

                info.m_nScore = 6000;
            }
            else if (nPrizeCash == 100 * 10000) //1000 X 10, 2000 X 5
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable1000);
                else
                    info = MakePrizeHintScore(5, lstPredictScoreTable2000);

                info.m_nScore = 10000;
            }
            else if (nPrizeCash == 150 * 10000) //15000 X 1
            {
                info = MakePrizeHintScore(1, lstPredictScoreTable15000);
                info.m_nScore = 15000;
            }
            else if (nPrizeCash == 200 * 10000) //2000 X 10, 20000 X 1
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstPredictScoreTable2000);
                else
                    info = MakePrizeHintScore(1, lstPredictScoreTable20000);

                info.m_nScore = 20000;
            }

            if (info != null)
            {
                m_lstSwkPrizeInfo.Add(info);
                nPrizeCash -= info.m_nScore;
            }
            else
                return;

            //빈돌기 3 ~ 5회
            MakeEmptyRoll(3, 6);

            //손오공 X 4 를 한번 보여준다.
            CSwkScoreInfo swkShowInfo = ConvertScoreTableToScoreInfo(m_lstSWKJockerScoreTable[RND.Next(0, m_lstSWKJockerScoreTable.Length)]);
            swkShowInfo.m_nHintScore = m_prizeInfo.m_nJackCash; //여기서 예시 액션을 보여준다.
            m_lstSwkPrizeInfo.Add(swkShowInfo);
            nPrizeCash -= 20000;

            MakeEmptyRoll(3, 6);
            MakeLatePrizeRoll(nPrizeCash);
        }

        //저팔계잭팟생성
        private void MakeJoPalGyePrizeRoll()
        {
            //저팔계애미매션만들기
            MakeGodAmination(PRIZE_JPG_ANIMATION);
            //잭팟점수를 얻는다.
            int nPrizeCash = m_prizeInfo.m_nJackCash;

            //련타테이블을 얻는다.
            List<SWKScoreTableInfo> lstJPGTable = m_lstSWKNormalScoreTable.ToList()
                .FindAll(value => (value.lstTileNote[0] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.lstTileNote[3] == 0)
                               || (value.lstTileNote[0] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.lstTileNote[3] == 0)
                               || (value.lstTileNote[0] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.lstTileNote[3] == 0)
                        );

            List<SWKScoreTableInfo> lstJPGTable20000 = m_lstSWKNormalScoreTable.ToList()
                .FindAll(value => ((value.lstTileNote[0] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.lstTileNote[3] == 0))
                                && value.nGetScore == 20000
                        );

            List<SWKScoreTableInfo> lstJPGTable10000 = m_lstSWKNormalScoreTable.ToList()
                .FindAll(value => ((value.lstTileNote[0] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.lstTileNote[3] == 0))
                                && value.nGetScore == 10000
                        );

            List<SWKScoreTableInfo> lstJPGTable5000 = m_lstSWKNormalScoreTable.ToList()
                .FindAll(value => ((value.lstTileNote[0] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.lstTileNote[3] == 0))
                                && value.nGetScore == 5000
                        );

            List<SWKScoreTableInfo> lstJPGTable2500 = m_lstSWKNormalScoreTable.ToList()
                .FindAll(value => ((value.lstTileNote[0] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.lstTileNote[3] == 0)
                                || (value.lstTileNote[0] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.lstTileNote[3] == 0))
                                && value.nGetScore == 2500
                        );


            List<CSwkScoreInfo> lstPredict = new List<CSwkScoreInfo>();


            //저팔계의 점수는 10만, 20만, 30만, 50만
            int[] lstPrizeAllScore = { 10000, 20000, 30000, 40000, 50000, 60000, 100000, 200000 };
            int[] lstPrizeScore = null;
            List<int> lstComplexScore = new List<int>();
            //련타점수배렬을 만든다.
            if (nPrizeCash >= 500000)
                lstPrizeScore = new int[] { 10000, 20000, 30000, 40000, 50000, 60000, 100000, 200000 };
            else if (nPrizeCash >= 300000)
                lstPrizeScore = new int[] { 10000, 20000, 30000, 40000, 50000, 60000 };
            else if (nPrizeCash >= 200000)
                lstPrizeScore = new int[] { 10000, 20000, 30000, 40000, 50000 };
            else if (nPrizeCash >= 70000)
                lstPrizeScore = new int[] { 10000, 20000 };
            else if (nPrizeCash >= 30000)
                lstPrizeScore = new int[] { 10000 };
            else
                return;

            int nTempScore = nPrizeCash;

            while (true)
            {
                while (nTempScore >= lstPrizeScore[lstPrizeScore.Length - 1])
                {
                    int nScore = lstPrizeScore[RND.Next(lstPrizeScore.Length)];
                    lstComplexScore.Add(nScore);
                    nTempScore -= nScore;
                }

                if (lstPrizeAllScore.ToList().Exists(value => value == nTempScore) || nTempScore == 0)
                {
                    if (nTempScore > 0)
                        lstComplexScore.Add(nTempScore);
                    if (lstComplexScore.Count > 2 && lstComplexScore.Count < 11)
                        break;
                }
                lstComplexScore.Clear();
                nTempScore = nPrizeCash;
            }

            CSwkScoreInfo scoreInfo = null;
            for (int i = 0; i < lstComplexScore.Count; i++)
            {
                if (lstComplexScore[i] == 200000)
                    scoreInfo = MakePrizeHintScore(10, lstJPGTable20000);
                else if (lstComplexScore[i] == 100000)
                {
                    if (RND.Next(2) == 0)
                        scoreInfo = MakePrizeHintScore(10, lstJPGTable10000);
                    else
                        scoreInfo = MakePrizeHintScore(5, lstJPGTable20000);
                }
                else if (lstComplexScore[i] == 60000)
                    scoreInfo = MakePrizeHintScore(3, lstJPGTable20000);
                else if (lstComplexScore[i] == 50000)
                {
                    if (RND.Next(2) == 0)
                        scoreInfo = MakePrizeHintScore(5, lstJPGTable10000);
                    else
                        scoreInfo = MakePrizeHintScore(10, lstJPGTable5000);
                }
                else if (lstComplexScore[i] == 40000)
                    scoreInfo = MakePrizeHintScore(2, lstJPGTable20000);
                else if (lstComplexScore[i] == 30000)
                    scoreInfo = MakePrizeHintScore(3, lstJPGTable10000);
                else if (lstComplexScore[i] == 20000)
                {
                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(1, lstJPGTable20000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(2, lstJPGTable10000);
                }
                else if (lstComplexScore[i] == 10000)
                {
                    int nRnd = RND.Next(3);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(1, lstJPGTable10000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(2, lstJPGTable5000);
                    else if (nRnd == 2)
                        scoreInfo = MakePrizeHintScore(4, lstJPGTable2500);


                    scoreInfo.m_nScore = 10000;
                }

                scoreInfo.m_nHintScore = lstComplexScore[i];
                lstPredict.Add(scoreInfo);

            }

            int nLateScore = nPrizeCash;
            while (lstPredict.Count > 0)
            {
                CharrySuggest();
                int nIdx = RND.Next(0, lstPredict.Count);
                m_lstSwkPrizeInfo.Add(lstPredict[nIdx]);

                nLateScore -= lstPredict[nIdx].m_nScore;
                lstPredict.RemoveAt(nIdx);
            }

            //뒤점수를 준다.
            MakeLatePrizeRoll(nLateScore);
        }

        //2배당이상으로 사오정잭팟생성
        private void MakeSaOJongPrizeRollX2()
        {
            List<SWKScoreTableInfo> lstSOJTable = m_lstSWKPrizeScoreTable.ToList();
            List<SWKScoreTableInfo> lstSOJTable50000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 50000);
            List<SWKScoreTableInfo> lstSOJTable40000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 40000);
            List<SWKScoreTableInfo> lstSOJTable30000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 30000);
            List<SWKScoreTableInfo> lstSOJTable25000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 25000);
            List<SWKScoreTableInfo> lstSOJTable20000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 20000);
            List<SWKScoreTableInfo> lstSOJTable15000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 15000);

            //사오정애니매션만들기
            MakeGodAmination(PRIZE_SOJ_ANIMATION);
            //사오정올바그림출현
            MakePrizeCommand(PRIZE_SOJ_ALLBAR);

            //잭팟점수를 얻는다.
            int nPrizeCash = m_prizeInfo.m_nJackCash;

            CSwkScoreInfo info = null;

            if (nPrizeCash == 3 * 10000) //15000 X 2
            {
                info = MakePrizeHintScore(2, lstSOJTable15000);
            }
            else if (nPrizeCash == 4 * 10000) //20000 X 2
            {
                info = MakePrizeHintScore(2, lstSOJTable20000);
            }
            else if (nPrizeCash == 5 * 10000) //25000 X 2
            {
                info = MakePrizeHintScore(2, lstSOJTable25000);
            }
            else if (nPrizeCash == 6 * 10000) //(15000 + 15000) X 2, 20000 X 3
            {
                int nRnd = RND.Next(5);
                if (nRnd < 4)
                    info = MakePrizeHintScore(2, lstSOJTable15000, lstSOJTable15000);
                else
                    info = MakePrizeHintScore(3, lstSOJTable20000);
            }
            else if (nPrizeCash == 7 * 10000) //(15000 + 20000) X 2
            {
                info = MakePrizeHintScore(2, lstSOJTable15000, lstSOJTable20000);
            }
            else if (nPrizeCash == 8 * 10000) //(20000 + 20000) X 2, (15000 + 25000) X 2, 40000 X 2
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(2, lstSOJTable20000, lstSOJTable20000);
                else if (nRnd < 9)
                    info = MakePrizeHintScore(2, lstSOJTable15000, lstSOJTable25000);
                else
                    info = MakePrizeHintScore(2, lstSOJTable40000);
            }
            else if (nPrizeCash == 9 * 10000) //(15000 + 30000) X 2, (20000 + 25000) X 2, (15000 + 15000) X 3, 30000 X 3
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(2, lstSOJTable15000, lstSOJTable30000);
                else if (nRnd < 7)
                    info = MakePrizeHintScore(2, lstSOJTable20000, lstSOJTable25000);
                else if (nRnd < 10)
                    info = MakePrizeHintScore(3, lstSOJTable15000, lstSOJTable15000);
                else
                    info = MakePrizeHintScore(3, lstSOJTable30000);
            }
            else if (nPrizeCash == 10 * 10000) //(20000 + 30000) X 2, 50000 X 2, 20000 X 5
            {
                int nRnd = RND.Next(10);
                if (nRnd < 8)
                    info = MakePrizeHintScore(2, lstSOJTable20000, lstSOJTable30000);
                else if (nRnd == 8)
                    info = MakePrizeHintScore(5, lstSOJTable20000);
                else
                    info = MakePrizeHintScore(2, lstSOJTable50000);
            }
            else if (nPrizeCash == 11 * 10000) //(15000 + 40000) X 2, (25000 + 30000) X 2
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(2, lstSOJTable15000, lstSOJTable40000);
                else
                    info = MakePrizeHintScore(2, lstSOJTable25000, lstSOJTable30000);
            }
            else if (nPrizeCash == 12 * 10000) //(20000 + 40000) X 2, (15000 + 25000) X 3, (20000 + 20000) X 3, 40000 X 3
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(2, lstSOJTable20000, lstSOJTable40000);
                else if (nRnd < 7)
                    info = MakePrizeHintScore(3, lstSOJTable15000, lstSOJTable25000);
                else if (nRnd < 9)
                    info = MakePrizeHintScore(3, lstSOJTable20000, lstSOJTable20000);
                else
                    info = MakePrizeHintScore(3, lstSOJTable40000);
            }
            else if (nPrizeCash == 13 * 10000) //(25000 + 40000) X 2, (15000 + 50000) X 2
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(2, lstSOJTable25000, lstSOJTable40000);
                else
                    info = MakePrizeHintScore(2, lstSOJTable15000, lstSOJTable50000);
            }
            else if (nPrizeCash == 14 * 10000) //(20000 + 50000) X 2
            {
                info = MakePrizeHintScore(2, lstSOJTable20000, lstSOJTable50000);
            }
            else if (nPrizeCash == 15 * 10000) //(25000 + 50000) X 2, (20000 + 30000) X 3, (25000 + 25000) X 3, 50000 X 3, 30000 X 5, 15000 X 10
            {
                int nRnd = RND.Next(12);
                if (nRnd < 4)
                    info = MakePrizeHintScore(2, lstSOJTable25000, lstSOJTable50000);
                else if (nRnd < 7)
                    info = MakePrizeHintScore(3, lstSOJTable20000, lstSOJTable30000);
                else if (nRnd < 9)
                    info = MakePrizeHintScore(3, lstSOJTable25000, lstSOJTable25000);
                else if (nRnd == 9)
                    info = MakePrizeHintScore(5, lstSOJTable30000);
                else if (nRnd == 10)
                    info = MakePrizeHintScore(10, lstSOJTable15000);
                else
                    info = MakePrizeHintScore(3, lstSOJTable50000);
            }
            else if (nPrizeCash == 20 * 10000)  //(15000 + 25000) X 5, (20000 + 20000) X 5, 40000 X 5, 20000 X 10
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(5, lstSOJTable15000, lstSOJTable25000);
                else if (nRnd < 8)
                    info = MakePrizeHintScore(5, lstSOJTable20000, lstSOJTable20000);
                else if (nRnd == 8)
                    info = MakePrizeHintScore(10, lstSOJTable20000);
                else
                    info = MakePrizeHintScore(5, lstSOJTable40000);
            }
            else if (nPrizeCash == 25 * 10000)  //(20000 + 30000) X 5, (25000 + 25000) X 5, 50000 X 5, 25000 X 10
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(5, lstSOJTable20000, lstSOJTable30000);
                else if (nRnd < 8)
                    info = MakePrizeHintScore(5, lstSOJTable25000, lstSOJTable25000);
                else if (nRnd == 8)
                    info = MakePrizeHintScore(10, lstSOJTable25000);
                else
                    info = MakePrizeHintScore(5, lstSOJTable50000);
            }
            else if (nPrizeCash == 30 * 10000) //(20000 + 40000) X 5, (15000 + 15000) X 10, 30000 X 10
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(5, lstSOJTable20000, lstSOJTable40000);
                else if (nRnd < 9)
                    info = MakePrizeHintScore(10, lstSOJTable15000, lstSOJTable15000);
                else
                    info = MakePrizeHintScore(10, lstSOJTable30000);
            }
            else if (nPrizeCash == 35 * 10000) //(20000 + 50000) X 5, (15000 + 20000) X 10
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(5, lstSOJTable20000, lstSOJTable50000);
                else
                    info = MakePrizeHintScore(10, lstSOJTable15000, lstSOJTable20000);
            }
            else if (nPrizeCash == 40 * 10000) //(20000 + 20000) X 10, (15000 + 25000) X 10, 40000 X 10
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                    info = MakePrizeHintScore(10, lstSOJTable20000, lstSOJTable20000);
                else if (nRnd < 9)
                    info = MakePrizeHintScore(10, lstSOJTable15000, lstSOJTable25000);
                else
                    info = MakePrizeHintScore(10, lstSOJTable40000);
            }
            else if (nPrizeCash == 45 * 10000) //(15000 + 30000) X 10, (20000 + 25000) X 10
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                    info = MakePrizeHintScore(10, lstSOJTable15000, lstSOJTable30000);
                else
                    info = MakePrizeHintScore(10, lstSOJTable20000, lstSOJTable25000);
            }
            else if (nPrizeCash == 50 * 10000) //(20000 + 30000) X 10, 50000 X 10
            {
                int nRnd = RND.Next(5);
                if (nRnd < 4)
                    info = MakePrizeHintScore(10, lstSOJTable20000, lstSOJTable30000);
                else
                    info = MakePrizeHintScore(10, lstSOJTable50000);
            }
            else if (nPrizeCash == 55 * 10000) //(25000 + 30000) X 10
            {
                info = MakePrizeHintScore(10, lstSOJTable25000, lstSOJTable30000);
            }
            else if (nPrizeCash == 60 * 10000) //(20000 + 40000) X 10
            {
                info = MakePrizeHintScore(10, lstSOJTable20000, lstSOJTable40000);
            }
            else if (nPrizeCash == 65 * 10000) //(25000 + 40000) X 10
            {
                info = MakePrizeHintScore(10, lstSOJTable25000, lstSOJTable40000);
            }
            else if (nPrizeCash == 70 * 10000) //(20000 + 50000) X 10
            {
                info = MakePrizeHintScore(10, lstSOJTable20000, lstSOJTable50000);
            }
            else if (nPrizeCash == 75 * 10000) //(25000 + 50000) X 10
            {
                info = MakePrizeHintScore(10, lstSOJTable25000, lstSOJTable50000);
            }

            if (info != null)
            {
                //예시점수추가
                info.m_nHintScore = m_prizeInfo.m_nJackCash;
                m_lstSwkPrizeInfo.Add(info);
            }
            else
                return;

            int nLateCash = nPrizeCash - 20000; //예시에서 맞은 점수를 던다.
            MakeEmptyRoll(3, 6);
            //뒤점수를 준다.
            MakeLatePrizeRoll(nLateCash);
        }


        //삼장잭팟생성
        private void MakeSamJangPrizeRoll()
        {
            //삼장애미매션만들기
            MakeGodAmination(PRIZE_SMJ_ANIMATION);

            //련타테이블을 얻는다.
            List<SWKScoreTableInfo> lstSMJTable = m_lstSWKPrizeScoreTable.ToList();
            List<SWKScoreTableInfo> lstSMJTable50000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 50000);
            List<SWKScoreTableInfo> lstSMJTable40000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 40000);
            List<SWKScoreTableInfo> lstSMJTable30000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 30000);
            List<SWKScoreTableInfo> lstSMJTable25000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 25000);
            List<SWKScoreTableInfo> lstSMJTable20000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 20000);
            List<SWKScoreTableInfo> lstSMJTable15000 = m_lstSWKPrizeScoreTable.ToList().FindAll(value => value.nGetScore == 15000);

            //잭팟점수를 얻는다.
            int nPrizeCash = m_prizeInfo.m_nJackCash;


            List<CSwkScoreInfo> lstPredict = new List<CSwkScoreInfo>();
            int nTempScore = nPrizeCash;
            //삼장의 점수는 10만, 20만, 30만, 50만, 100만
            //20000, 25000, 30000, 40000, 45000, 50000, 60000, 75000, 80000, 90000, 100000, 125000, 150000, 200000, 250000, 300000, 400000, 500000
            int[] lstPrizeAllScore = { 15000, 20000, 25000, 30000, 40000, 45000, 50000, 60000, 75000, 80000, 90000, 100000, 120000, 125000, 150000, 200000, 250000, 300000, 400000, 500000 };
            int[] lstPrizeScore;
            List<int> lstComplexScore = new List<int>();

            //련타점수배렬을 만든다.
            while (true)
            {
                if (nTempScore >= 80 * 10000)
                    lstPrizeScore = new int[] { 400000, 500000 };
                else if (nTempScore >= 60 * 10000)
                    lstPrizeScore = new int[] { 300000, 400000, 500000 };
                else if (nTempScore >= 50 * 10000)
                    lstPrizeScore = new int[] { 250000, 300000, 400000, 500000 };
                else if (nTempScore >= 40 * 10000)
                    lstPrizeScore = new int[] { 200000, 250000, 300000, 400000 };
                else if (nTempScore >= 30 * 10000)
                    lstPrizeScore = new int[] { 150000, 200000, 250000, 300000 };
                else if (nTempScore >= 20 * 10000)
                    lstPrizeScore = new int[] { 100000, 120000, 125000, 150000, 200000 };
                else if (nTempScore >= 10 * 10000)
                    lstPrizeScore = new int[] { 50000, 60000, 75000, 80000, 90000, 100000 };
                else if (nTempScore >= 5 * 10000)
                    lstPrizeScore = new int[] { 25000, 30000, 40000, 45000, 50000 };
                else
                    lstPrizeScore = new int[] { 15000, 20000, 25000, 30000, 40000, 45000, 50000 };


                int nCash = lstPrizeScore[RND.Next(lstPrizeScore.Length)];
                lstComplexScore.Add(nCash);
                nTempScore -= nCash;

                if (lstPrizeAllScore.ToList().Exists(value => value == nTempScore))
                {
                    lstComplexScore.Add(nTempScore);
                    if (lstComplexScore.Count > 0 && lstComplexScore.Count < 5)
                        break;
                }

                if (lstComplexScore.Count > 4)
                {
                    lstComplexScore.Clear();
                    nTempScore = nPrizeCash;
                    continue;
                }

                if (nTempScore < 15000)
                {
                    lstComplexScore.Clear();
                    nTempScore = nPrizeCash;
                    continue;
                }
            }


            CSwkScoreInfo scoreInfo = null;
            for (int i = 0; i < lstComplexScore.Count; i++)
            {
                if (lstComplexScore[i] == 500000)
                    scoreInfo = MakePrizeHintScore(10, lstSMJTable50000);
                else if (lstComplexScore[i] == 400000)
                    scoreInfo = MakePrizeHintScore(10, lstSMJTable40000);
                else if (lstComplexScore[i] == 300000)
                    scoreInfo = MakePrizeHintScore(10, lstSMJTable30000);
                else if (lstComplexScore[i] == 250000)
                {
                    if (RND.Next(2) == 0)
                        scoreInfo = MakePrizeHintScore(10, lstSMJTable25000);
                    else
                        scoreInfo = MakePrizeHintScore(5, lstSMJTable50000);
                }
                else if (lstComplexScore[i] == 200000)
                {
                    if (RND.Next(2) == 0)
                        scoreInfo = MakePrizeHintScore(10, lstSMJTable20000);
                    else
                        scoreInfo = MakePrizeHintScore(5, lstSMJTable40000);
                }
                else if (lstComplexScore[i] == 150000)
                {
                    int nRnd = RND.Next(3);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(10, lstSMJTable15000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(3, lstSMJTable50000);
                    else if (nRnd == 2)
                        scoreInfo = MakePrizeHintScore(5, lstSMJTable30000);
                }
                else if (lstComplexScore[i] == 125000)
                    scoreInfo = MakePrizeHintScore(5, lstSMJTable25000);
                else if (lstComplexScore[i] == 120000)
                    scoreInfo = MakePrizeHintScore(3, lstSMJTable40000);
                else if (lstComplexScore[i] == 100000)
                {
                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(5, lstSMJTable20000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(2, lstSMJTable50000);
                }
                else if (lstComplexScore[i] == 90000)
                    scoreInfo = MakePrizeHintScore(3, lstSMJTable30000);
                else if (lstComplexScore[i] == 80000)
                    scoreInfo = MakePrizeHintScore(2, lstSMJTable40000);
                else if (lstComplexScore[i] == 75000)
                {
                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(3, lstSMJTable25000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(5, lstSMJTable15000);
                }
                else if (lstComplexScore[i] == 60000)
                    scoreInfo = MakePrizeHintScore(3, lstSMJTable20000);
                else if (lstComplexScore[i] == 50000)
                {
                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(1, lstSMJTable50000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(2, lstSMJTable25000);
                }
                else if (lstComplexScore[i] == 45000)
                    scoreInfo = MakePrizeHintScore(3, lstSMJTable15000);
                else if (lstComplexScore[i] == 40000)
                {
                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(1, lstSMJTable40000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(2, lstSMJTable20000);
                }
                else if (lstComplexScore[i] == 30000)
                {
                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                        scoreInfo = MakePrizeHintScore(1, lstSMJTable30000);
                    else if (nRnd == 1)
                        scoreInfo = MakePrizeHintScore(2, lstSMJTable15000);
                }
                else if (lstComplexScore[i] == 25000)
                    scoreInfo = MakePrizeHintScore(1, lstSMJTable25000);
                else if (lstComplexScore[i] == 20000)
                    scoreInfo = MakePrizeHintScore(1, lstSMJTable20000);
                else if (lstComplexScore[i] == 15000)
                {
                    scoreInfo = MakePrizeHintScore(1, lstSMJTable15000);
                    scoreInfo.m_nScore = 15000;
                }

                scoreInfo.m_nHintScore = lstComplexScore[i];
                lstPredict.Add(scoreInfo);
            }

            int nLateScore = nPrizeCash;
            while (lstPredict.Count > 0)
            {
                CharrySuggest();
                int nIdx = RND.Next(0, lstPredict.Count);
                m_lstSwkPrizeInfo.Add(lstPredict[nIdx]);
                nLateScore -= lstPredict[nIdx].m_nScore;
                lstPredict.RemoveAt(nIdx);
            }

            //뒤점수를 준다.
            MakeLatePrizeRoll(nLateScore);

        }

        //37별타
        private void MakeThreeSevenPrizeRoll()
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;

            //앵두 1개를 10배당으로 주어야 한다.
            List<SWKScoreTableInfo> lstCharry = m_lstSWKNormalScoreTable.ToList().FindAll(value => value.lstTileNote[0] == 11 && value.lstTileNote[1] == 11 && value.lstTileNote[2] == 11 && value.lstTileNote[3] == 11);
            SWKScoreTableInfo charryTable1 = lstCharry[RND.Next(lstCharry.Count)];
            CSwkScoreInfo scoreInfo1 = ConvertScoreTableToScoreInfo(charryTable1, 10);
            m_lstSwkPrizeInfo.Add(scoreInfo1);
            MakeEmptyRoll(4, 6);
            if (nPrizeCash > 500000)
            {
                SWKScoreTableInfo charryTable2 = lstCharry[RND.Next(lstCharry.Count)];
                CSwkScoreInfo scoreInfo2 = ConvertScoreTableToScoreInfo(charryTable2, 10);
                m_lstSwkPrizeInfo.Add(scoreInfo2);
                MakeEmptyRoll(4, 6);
            }


            List<CSwkScoreInfo> lstPredict = new List<CSwkScoreInfo>();
            int nKind = 0;
            CSwkScoreInfo tsScoreInfo = null;
            //예시점수주기  예수점수는 10배당으로 10회 안에 준다.
            //37별타 현시
            if (nPrizeCash > 500000)
            {
                nKind = 0;
                tsScoreInfo = ConvertScoreTableToScoreInfo(m_lstSWKThreeSevenTable[0], RND.Next(m_lstSWKMults.Length));
                tsScoreInfo.m_nScore = -2;
            }
            else
            {
                nKind = 1;
                tsScoreInfo = ConvertScoreTableToScoreInfo(m_lstSWKThreeSevenTable[RND.Next(1, 3)], RND.Next(m_lstSWKMults.Length));
                tsScoreInfo.m_nScore = -9;
            }
            MakePrizeCommand(PRIZE_TSS_ANIMATION);

            m_lstSwkPrizeInfo.Add(tsScoreInfo);

            //빈돌기 2 ~ 3 회 추가
            MakeEmptyRoll(2, 4);


            //련타테이블을 얻는다.
            List<SWKScoreTableInfo> lstSMJTable = m_lstSWKPrizeScoreTable.ToList();
            List<SWKScoreTableInfo> lstSMJTable50000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 50000);
            List<SWKScoreTableInfo> lstSMJTable40000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 40000);
            List<SWKScoreTableInfo> lstSMJTable30000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 30000);
            List<SWKScoreTableInfo> lstSMJTable25000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 25000);
            List<SWKScoreTableInfo> lstSMJTable20000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 20000);
            List<SWKScoreTableInfo> lstSMJTable15000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 15000);
            List<SWKScoreTableInfo> lstSMJTable10000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 10000);
            List<SWKScoreTableInfo> lstSMJTable5000 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 5000);
            List<SWKScoreTableInfo> lstSMJTable2500 = m_lstSWKBarScoreTable.ToList().FindAll(value => value.nGetScore == 2500);


            int nTempScore;
            if (nKind == 0)
                nTempScore = nPrizeCash / 10;
            else
                nTempScore = nPrizeCash / 5;

            //삼칠별타 점수는 10만, 20만, 30만, 50만, 100만
            int[] lstPrizeAllScore = { 2500, 5000, 10000, 15000, 20000, 25000, 30000, 40000, 50000 };
            int[] lstPrizeScore;
            List<int> lstComplexScore = new List<int>();
            //련타점수배렬을 만든다.
            if (nTempScore >= 70000)
                lstPrizeScore = new int[] { 20000, 25000, 30000, 40000, 50000 };
            else if (nTempScore >= 50000)
                lstPrizeScore = new int[] { 10000, 15000, 20000, 25000, 30000, 40000, 50000 };
            else if (nTempScore >= 30000)
                lstPrizeScore = new int[] { 10000, 15000, 20000, 25000, 30000 };
            else if (nTempScore >= 20000)
                lstPrizeScore = new int[] { 5000, 10000, 15000, 20000 };
            else if (nTempScore >= 10000)
                lstPrizeScore = new int[] { 5000, 10000 };
            else
                lstPrizeScore = new int[] { 2500, 5000 };

            while (true)
            {
                while (nTempScore >= lstPrizeScore[lstPrizeScore.Length - 1])
                {
                    int nScore = lstPrizeScore[RND.Next(lstPrizeScore.Length)];
                    lstComplexScore.Add(nScore);
                    nTempScore -= nScore;
                }
                if (lstPrizeAllScore.ToList().Exists(value => value == nTempScore) || nTempScore == 0)
                {
                    if (nTempScore > 0)
                        lstComplexScore.Add(nTempScore);

                    if (nKind == 0)
                    {
                        if (lstComplexScore.Count > 0 && lstComplexScore.Count < 11)
                            break;
                    }
                    else
                    {
                        if (lstComplexScore.Count > 0 && lstComplexScore.Count < 6)
                            break;
                    }

                }
                lstComplexScore.Clear();

                if (nKind == 0)
                    nTempScore = nPrizeCash / 10;
                else
                    nTempScore = nPrizeCash / 5;
            }


            CSwkScoreInfo scoreInfo = null;

            int nMult = 0;
            if (nKind == 0)
                nMult = 10;
            else
                nMult = 5;

            for (int i = 0; i < lstComplexScore.Count; i++)
            {

                if (lstComplexScore[i] == 50000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable50000);
                else if (lstComplexScore[i] == 40000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable40000);
                else if (lstComplexScore[i] == 30000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable30000);
                else if (lstComplexScore[i] == 25000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable25000);
                else if (lstComplexScore[i] == 20000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable20000);
                else if (lstComplexScore[i] == 15000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable15000);
                else if (lstComplexScore[i] == 10000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable10000);
                else if (lstComplexScore[i] == 5000)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable5000);
                else if (lstComplexScore[i] == 2500)
                    scoreInfo = MakePrizeHintScore(nMult, lstSMJTable2500);

                scoreInfo.m_nHintScore = lstComplexScore[i] * nMult;
                lstPredict.Add(scoreInfo);
            }

            if (nKind == 0)
            {
                while (lstPredict.Count < 10)
                {
                    CSwkScoreInfo threeSevenZero = new CSwkScoreInfo();
                    threeSevenZero.m_nScore = 0;
                    threeSevenZero.m_nMult = 10;
                    if (RND.Next(2) == 0)
                        lstPredict.Insert(RND.Next(lstPredict.Count), threeSevenZero);
                    else
                        lstPredict.Add(threeSevenZero);
                }
            }
            else
            {
                while (lstPredict.Count < 5)
                {
                    CSwkScoreInfo threeSevenZero = MakeEmptyOneRoll();
                    threeSevenZero.m_nScore = 0;
                    threeSevenZero.m_nMult = 5;
                    if (RND.Next(2) == 0)
                        lstPredict.Insert(RND.Next(lstPredict.Count), threeSevenZero);
                    else
                        lstPredict.Add(threeSevenZero);
                }
            }




            lstPredict[0].m_nMult = -1;  //fever 스핀 시작
            if (nKind == 0)
                lstPredict[9].m_nMult = -2;  //fever 스핀 끝
            else
                lstPredict[4].m_nMult = -2;  //fever 스핀 끝



            m_lstSwkPrizeInfo.AddRange(lstPredict);
            //빈돌기 3~4 회 추가
            MakeEmptyRoll(3, 5);

            nPrizeCash = nPrizeCash - 20000 * lstComplexScore.Count;
            //뒤점수를 준다.
            MakeLatePrizeRoll(nPrizeCash);

        }

        //프리잭팟
        private void MakeFreeEvenPrizeRoll()
        {
            //불상출현
            MakeNormalPrizeRoll();
            MakeEmptyRoll(1, 3);
            //불상타일맞추기
            CSwkScoreInfo evilTile = new CSwkScoreInfo();
            if (RND.Next(2) == 0)
            {
                evilTile.m_nScore = -5;
                evilTile.m_nMult = RND.Next(0, m_lstSWKMults.Length);
                evilTile.m_lstTile.Add(new CTile(12, 0, 0));
                evilTile.m_lstTile.Add(new CTile(12, 1, 1));
                evilTile.m_lstTile.Add(new CTile(12, 1, 2));
                evilTile.m_lstTile.Add(new CTile(12, 2, 3));

            }
            else
            {
                evilTile.m_nScore = -5;
                evilTile.m_nMult = RND.Next(0, m_lstSWKMults.Length);
                evilTile.m_lstTile.Add(new CTile(12, 0, 3));
                evilTile.m_lstTile.Add(new CTile(12, 1, 1));
                evilTile.m_lstTile.Add(new CTile(12, 1, 2));
                evilTile.m_lstTile.Add(new CTile(12, 2, 0));
            }

            m_lstSwkPrizeInfo.Add(evilTile);

            //MakeEmptyRoll(3, 6);

            //큰불상나오기
            MakePrizeCommand(PRIZE_EVIL_SLIDE);
            MakeEmptyRoll(4, 6);

            //프리돌기시작이벤트추가
            List<CSwkScoreInfo> lstTempScoreList = new List<CSwkScoreInfo>();
            CSwkScoreInfo scoreInfo2 = new CSwkScoreInfo();
            scoreInfo2.m_nScore = -6;
            scoreInfo2.m_nPrizeCmd = PRIZE_FREE_START;
            m_lstSwkPrizeInfo.Add(scoreInfo2);

            int nPrizeCash = m_prizeInfo.m_nJackCash;

            CSwkScoreInfo score0 = new CSwkScoreInfo();
            score0.m_nScore = 100000;
            score0.m_lstTile.Add(new CTile(1, 0, 0));
            score0.m_lstTile.Add(new CTile(1, 0, 1));
            score0.m_lstTile.Add(new CTile(1, 0, 2));
            score0.m_lstTile.Add(new CTile(1, 0, 3));

            CSwkScoreInfo score1 = new CSwkScoreInfo();
            score1.m_nScore = 200000;
            score1.m_lstTile.Add(new CTile(1, 1, 0));
            score1.m_lstTile.Add(new CTile(1, 1, 1));
            score1.m_lstTile.Add(new CTile(1, 1, 2));
            score1.m_lstTile.Add(new CTile(1, 1, 3));

            CSwkScoreInfo score2 = new CSwkScoreInfo();
            score2.m_nScore = 100000;
            score2.m_lstTile.Add(new CTile(1, 2, 0));
            score2.m_lstTile.Add(new CTile(1, 2, 1));
            score2.m_lstTile.Add(new CTile(1, 2, 2));
            score2.m_lstTile.Add(new CTile(1, 2, 3));


            CSwkScoreInfo[] arrScore = { score0, score1, score2 };

            while (nPrizeCash > 0)
            {
                CSwkScoreInfo score = arrScore[RND.Next(arrScore.Length)];
                if (nPrizeCash >= score.m_nScore)
                {
                    nPrizeCash -= score.m_nScore;
                    lstTempScoreList.Add(score);
                }
            }

            while (lstTempScoreList.Count < 9)
            {
                CSwkScoreInfo emptyScore = MakeEmptyOneRoll();
                if (RND.Next(2) == 0)
                    lstTempScoreList.Insert(RND.Next(lstTempScoreList.Count), emptyScore);
                else
                    lstTempScoreList.Add(emptyScore);
            }
            m_lstSwkPrizeInfo.AddRange(lstTempScoreList);
            //프리돌기끝이벤트추가
            CSwkScoreInfo scoreInfo3 = new CSwkScoreInfo();
            scoreInfo3.m_nScore = -7;
            scoreInfo2.m_nPrizeCmd = PRIZE_FREE_END;
            lstTempScoreList.Add(scoreInfo3);

            m_lstSwkPrizeInfo.AddRange(lstTempScoreList);
        }

        private void ReturnGiftCash()
        {
            m_lstSwkReturnGift.Clear();
            List<CSwkScoreInfo> lstScoreInfo = new List<CSwkScoreInfo>();

            if (m_nReturnCash == 0)
                return;

            List<int> lstScore = new List<int>();

            int nTempScore = m_nReturnCash;
            if (nTempScore > 15000)
            {
                lstScore.Add(15000);
                nTempScore -= 15000;
            }
            if (nTempScore > 10000)
            {
                lstScore.Add(10000);
                nTempScore -= 10000;
            }
            if (nTempScore > 5000)
            {
                lstScore.Add(5000);
                nTempScore -= 5000;
            }
            if (nTempScore > 2000)
            {
                lstScore.Add(2000);
                nTempScore -= 2000;
            }
            if (nTempScore > 1000)
            {
                lstScore.Add(1000);
                nTempScore -= 1000;
            }
            if (nTempScore >= 600)
            {
                lstScore.Add(600);
                nTempScore -= 600;
            }
            if (nTempScore >= 400)
            {
                lstScore.Add(400);
                nTempScore -= 400;
            }
            if (nTempScore >= 300)
            {
                lstScore.Add(300);
                nTempScore -= 300;
            }
            if (nTempScore >= 200)
            {
                lstScore.Add(200);
                nTempScore -= 200;
            }
            if (nTempScore >= 100)
            {
                lstScore.Add(100);
                nTempScore -= 100;
            }

            for (int i = 0; i < lstScore.Count; i++)
            {
                //점수테이블리스트를 만든다.
                List<SWKScoreTableInfo> lstMatchScoreInfo = m_lstSWKNormalScoreTable.ToList().FindAll(value => value.nGetScore == lstScore[i] && (value.lstTileNote[0] >= 2 && value.lstTileNote[0] <= 11));
                CSwkScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(lstMatchScoreInfo[RND.Next(lstMatchScoreInfo.Count)]);
                lstScoreInfo.Add(scoreInfo);
            }
            lstScoreInfo.Add(MakeEmptyOneRoll());
            m_bReturnGift = true;
            m_lstSwkReturnGift.InsertRange(0, lstScoreInfo);
            m_nReturnCash = 0;
        }

        public List<CSwkScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstSwkPrizeInfo;
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nJackCont = 0;
            if(clsItem.m_nItemModel == CItemModel.ITEM_SWK_SONWUKONG)
            {
                nJackCont = 1;
            }
            else if(clsItem.m_nItemModel == CItemModel.ITEM_SWK_JOPALGYE)
            {
                nJackCont = 2;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_SWK_SAOJONG)
            {
                nJackCont = 3;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_SWK_SAMJANG)
            {
                nJackCont = 4;
            }

            List<SWKJackType> lstSWKJackType = m_lstSWKJackType.FindAll(value => value.m_nJackCont == nJackCont && value.m_nJackScore <= nItemCash);
            if (lstSWKJackType == null || lstSWKJackType.Count == 0)
            {
                //가짜예시이다.
                SetSWKJackpotInfo(nJackCont, 1, 0, 0, 0);
                MakePrizeRoll();
                m_nReturnCash = nItemCash;
            }
            else
            {
                //점수에 해당한 잭팟을 주어야 한다.
                int nJackScore = lstSWKJackType.Max(value => value.m_nJackScore);
                SetSWKJackpotInfo(nJackCont, 2, nJackScore, 0, 0);
                MakePrizeRoll();
                CGlobal.SetItemEngineRemCash(nItemCash - nJackScore);

                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nJackScore, nJackCont, 4, 0);
            }
        }
    }
}
