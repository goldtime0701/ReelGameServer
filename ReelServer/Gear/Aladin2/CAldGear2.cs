using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CAldGear2 : CBaseGear
    {
        private CALDJackpotInfo m_prizeInfo;                        //알라딘잭팟정보
        private List<CALDScoreInfo> m_lstAldScoreInfo;              //빈돌기까지 포함한 완전한 돌기정보
        private List<CALDScoreInfo> m_lstAldPrizeInfo;              //빈돌기까지 포함한 잭팟돌기 정보

        private CALDScoreInfo m_clsSendScoreInfo;
        private int m_nRealPrizeCash;

        private bool m_bGodAnimation;                               //잭팟일때 현재 애미매션동작중인가를 나타낸다.
        private bool m_bSendSpin;                                   //스핀정보를 보내도 되는가? 금액이 모자라는 경우는 false 로 된다.


        public CAldGear2() : base()
        {
            m_lstAldScoreInfo = new List<CALDScoreInfo>();
            m_lstAldPrizeInfo = new List<CALDScoreInfo>();
            m_nSpeedCash = 200;

        }

        public void SetALDJackpotInfo(int nJackCont, int nJackCash)
        {
            m_prizeInfo = new CALDJackpotInfo();
            m_prizeInfo.m_nJackCont = nJackCont;
            m_prizeInfo.m_nJackCash = nJackCash;    //잭팟캐시   
        }

        public CALDJackpotInfo GetAldJackpotInfo()
        {
            return m_prizeInfo;
        }

        public override void ClearPrizeInfo()
        {
            m_prizeInfo = null;
            m_lstAldPrizeInfo.Clear();
        }

        public override void ClearGear()
        {
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            SetGiveStep(0);
            m_lstAldScoreInfo.Clear();
            m_lstAldPrizeInfo.Clear();
            m_prizeInfo = null;
        }

        public int GetRealPrizeCash()
        {
            return m_nRealPrizeCash;
        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            //기계에 충전금액으로 부터 환수률이 계산된 담첨금액을 계산한다.
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<CALDScoreInfo> lstAldScoreInfo = new List<CALDScoreInfo>();
            int nTempCash = nAddCash;
            while (nAddCash > 0)
            {
                int nAppenCash = nAddCash >= 10000 ? 10000 : nAddCash;
                nAddCash -= nAppenCash;
                int nDstCash = (int)(nAppenCash * rate / 100);

                //점수렬을 만든다.
                List<int> lstScore = new List<int>();
                MakeNormalScoreList(nDstCash, lstScore);

                List<CALDScoreInfo> lstScoreInfo = new List<CALDScoreInfo>();

                //점수에 해당한 ALDScoreTableInfo리스트를 구한다.
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CALDScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CALDScoreInfo scoreInfo = lstScoreInfo[RND.Next(lstScoreInfo.Count)];
                    lstAldScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }

            int nRollCount = nTempCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstAldScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CALDScoreInfo scoreInfo = MakeEmptyOneRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstAldScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstAldScoreInfo.Count);
                    lstAldScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                m_lstAldScoreInfo.Clear();
            }

            m_lstAldScoreInfo.AddRange(lstAldScoreInfo);
        }

        public override bool MakePrizeRoll()
        {
            if (m_nGearJack > 0)
                return false;


            //잭팟인 경우에는 m_lstScoreInfo 가 아니라 m_lstPrizeInfo 에 정보를 보관한다.
            m_nRealPrizeCash = 0;
            List<CALDScoreInfo> lstPrizeInfo = m_lstAldPrizeInfo;
            lstPrizeInfo.Clear();

            //잭팟시작
            MakeGodCommand(PRIZE_START);

            switch (m_prizeInfo.m_nJackCont)
            {
                case ALD_HORSE_WWW:
                    MakeHorseWWPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_FIRE_SMALL:
                    MakeFireSmallPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_FIRE_LARGE:
                    MakeFireLargePrizeRoll(lstPrizeInfo);
                    break;
                case ALD_YANGT_LOCK:
                    MakeYangTLockPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_YANGT_OPEN:
                    MakeYangTOpenPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_FEVER_LOCK:
                    MakeFeverLockPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_FEVER_OPEN:
                    MakeFeverOpenPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_THREE_SEVEN:
                    MakeThreeSevenPrizeRoll(lstPrizeInfo);
                    break;
                case ALD_SURPRISE_1:
                    MakeSurprise1PrizeRoll(lstPrizeInfo);
                    break;
                case ALD_SURPRISE_2:
                    MakeSurprise2PrizeRoll(lstPrizeInfo);
                    break;
                case ALD_SURPRISE_3:
                    MakeSurprise3PrizeRoll(lstPrizeInfo);
                    break;
            }


            MakeEmptyRoll(2, 4);

            CALDScoreInfo scoreInfo = new CALDScoreInfo();
            scoreInfo.m_nMult = CGlobal.RandomSelect(m_lstALDMults.ToList());
            scoreInfo.m_lstTile.Add(new CTile(1, 1, 0));
            scoreInfo.m_lstTile.Add(new CTile(1, 2, 1));
            scoreInfo.m_lstTile.Add(new CTile(1, 1, 2));
            scoreInfo.m_lstTile.Add(new CTile(1, 2, 3));
            scoreInfo.m_nScore = -5;

            lstPrizeInfo.Add(scoreInfo);


            //잭팟 끝
            MakeGodCommand(PRIZE_END);
            lstPrizeInfo.Add(MakeEmptyOneRoll());

            //기대를 잭팟중으로 설정 
            m_nGearJack = 1;


            m_dtLastJackTime = CMyTime.GetMyTime();
            m_nLastJackCash = m_nRealPrizeCash;
            if (m_nRealPrizeCash > m_nTopJackCash)
                m_nTopJackCash = m_nRealPrizeCash;

            //if (m_nRealPrizeCash < 20 * 10000)
            //    m_nMiniCount++;
            //else if (m_nRealPrizeCash < 50 * 10000)
            //    m_nMinorCount++;
            //else if (m_nRealPrizeCash < 70 * 10000)
            //    m_nMajorCount++;
            //else
            //    m_nGrandCount++;

            CDataBase.SaveGearInfoToDB(this);
            return true;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            ClearPrizeInfo();

            int[] lst20000 = { 1, 2, 1, 2 };
            int[] lst100000 = { 1, 2, 1, 2, 1, 2, 5 };
            int[] lst500000 = { 1, 2, 5, 7, 1, 2, 7, 1, 2, 7 };
            int[] lst800000 = { 5 };
            int[] lst_500000 = { 3, 1, 2, 5, 7, 1, 2, 3, 7, 1, 2, 3, 7 };
            int[] lst1000000 = { 3, 5 };
            int[] lstEmpty = { 4, 6 };

            int nJackCont = 0;
            int nRetCash = nJackCash;

            if(nJackCash > 200 * 10000)
            {
                nJackCash = 200 * 10000;
            }

            CGameEngine aldEngine = CGlobal.GetEngine(m_nGameCode);
            if (aldEngine.GetGivePrizeCount() > aldEngine.m_nEmptyPrize)
            {
                nJackCont = CGlobal.RandomSelect(lstEmpty.ToList());
                aldEngine.SetGivePrizeCount(0);
                nJackCash = 0;
            }
            else if (nJackCash == 100 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst_500000.ToList());
            }
            else if (nJackCash == 200 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst1000000.ToList());
            }
            else if (nJackCash <= 4 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst20000.ToList());
            }
            else if (nJackCash <= 20 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst100000.ToList());
            }
            else if (nJackCash < 100 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst500000.ToList());
            }
            else
            {
                nJackCont = CGlobal.RandomSelect(lst800000.ToList());
            }


            SetALDJackpotInfo(nJackCont, nJackCash);
            bool ret = MakePrizeRoll();
            if (ret)
            {
                nRetCash -= m_nRealPrizeCash;
                m_prizeInfo.m_nJackCash = m_nRealPrizeCash;
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                if (robot != null)
                {
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, m_nRealPrizeCash, m_prizeInfo.m_nJackCont, -1, 1);
                    robot.PlayAld2Prize();
                }
                    
            }

            return nRetCash;
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            ClearPrizeInfo();

            List<ALDScoreTableInfo> lstScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore <= nJackCash && value.nSlogan != 4
                && value.nSlogan != 14 && value.nSlogan != 24 && value.nSlogan != 34 && value.nSlogan != 44 && value.nSlogan != 54).OrderByDescending(value => value.nGetScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nScore = lstScoreTable[0].nGetScore;
                List<ALDScoreTableInfo> lstTempTableInfo = lstScoreTable.FindAll(value => value.nGetScore == nScore);
                ALDScoreTableInfo tableInfo = CGlobal.RandomSelect(lstTempTableInfo);
                CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
                m_lstAldPrizeInfo.Add(scoreInfo);

                CDataBase.InsertNationalToDB(m_nGearCode, scoreInfo.m_nScore, 1);
                SetGiveStep(1);
                nJackCash -= scoreInfo.m_nScore;

                m_nGearJack = 2;
                m_lstAldPrizeInfo.Add(MakeEmptyOneRoll());
            }

            return nJackCash;
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            int[] lst20000 = { 1, 2, 5 };
            int[] lst100000 = { 1, 2, 5 };
            int[] lst500000 = { 1, 2, 5, 7 };
            int[] lst800000 = { 5 };
            int[] lst_500000 = { 3, 1, 2, 5, 7 };
            int[] lst1000000 = { 3, 5 };
            int[] lstEmpty = { 4, 6 };


            CGameEngine aldEngine = CGlobal.GetEngine(m_nGameCode);

            int nJackCont = 0;
            int nRetCash = nJackCash;

            if (nJackCash > 200 * 10000)
            {
                nJackCash = 200 * 10000;
            }


            if (aldEngine.GetGivePrizeCount() > aldEngine.m_nEmptyPrize)
            {
                nJackCont = CGlobal.RandomSelect(lstEmpty.ToList());
                aldEngine.SetGivePrizeCount(0);
                nJackCash = 0;
                SetALDJackpotInfo(nJackCont, nJackCash);
                MakePrizeRoll();
                return nRetCash;
            }
            else if (nJackCash == 2 * 50 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst_500000.ToList());
            }
            else if (nJackCash == 2 * 100 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst1000000.ToList());
            }
            else if (nJackCash <= 2 * 2 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst20000.ToList());
            }
            else if (nJackCash <= 2 * 10 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst100000.ToList());
            }
            else if (nJackCash <= 2 * 50 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst500000.ToList());
            }
            else
            {
                nJackCont = CGlobal.RandomSelect(lst800000.ToList());
            }


            SetALDJackpotInfo(nJackCont, nJackCash);
            bool ret = MakePrizeRoll();
            if (ret)
            {
                CDataBase.InsertNationalToDB(m_nGearCode, m_nRealPrizeCash, 2);
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, m_nRealPrizeCash, m_prizeInfo.m_nJackCont, 2, 0);
                SetGiveStep(2);

                nRetCash -= m_nRealPrizeCash;
            }

            return nRetCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            int[] lst20000 = { 1, 2, 5 };
            int[] lst100000 = { 1, 2, 5 };
            int[] lst500000 = { 1, 2, 5, 7 };
            int[] lst800000 = { 5 };
            int[] lst_500000 = { 3, 1, 2, 5, 7 };
            int[] lst1000000 = { 3, 5 };
            int[] lstEmpty = { 4, 6 };

            CGameEngine aldEngine = CGlobal.GetEngine(m_nGameCode);

            int nJackCont = 0;
            int nRetCash = nJackCash;

            if (nJackCash > 200 * 10000)
            {
                nJackCash = 200 * 10000;
            }


            if (aldEngine.GetGivePrizeCount() > aldEngine.m_nEmptyPrize)
            {
                nJackCont = CGlobal.RandomSelect(lstEmpty.ToList());
                aldEngine.SetGivePrizeCount(0);
                nJackCash = 0;

                SetALDJackpotInfo(nJackCont, nJackCash);
                MakePrizeRoll();
                return nRetCash;
            }
            else if (nJackCash == 2 * 50 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst_500000.ToList());
            }
            else if (nJackCash == 2 * 100 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst1000000.ToList());
            }
            else if (nJackCash <= 2 * 2 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst20000.ToList());
            }
            else if (nJackCash <= 2 * 10 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst100000.ToList());
            }
            else if (nJackCash <= 2 * 50 * 10000)
            {
                nJackCont = CGlobal.RandomSelect(lst500000.ToList());
            }
            else
            {
                nJackCont = CGlobal.RandomSelect(lst800000.ToList());
            }


            SetALDJackpotInfo(nJackCont, nJackCash);
            bool ret = MakePrizeRoll();
            if (ret)
            {
                CDataBase.InsertNationalToDB(m_nGearCode, m_nRealPrizeCash, 3);
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, m_nRealPrizeCash, m_prizeInfo.m_nJackCont, 3, 0);
                SetGiveStep(3);

                nRetCash -= m_nRealPrizeCash;
            }

            return nRetCash;
        }

        //알라딘에서는 잭팟애니매션이 끝났다는 신호로 사용된다.
        public override void OnCreateCoin(CUserSocket clsSocket)
        {
            m_bGodAnimation = false;
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
            OnAddSlotCash(clsSocket, nSpinCash);

            if (m_bSendSpin == false)
                return;
            if (m_lstAldPrizeInfo.Count > 0)
            {
                //잭팟일때이다.
                if(m_bGodAnimation == false)
                {
                    m_clsSendScoreInfo = m_lstAldPrizeInfo[0];
                    m_lstAldPrizeInfo.RemoveAt(0);
                    int nCmd = m_clsSendScoreInfo.m_nPrizeCmd;

                    if (nCmd == ALD_HORSE_WWW_ANIMATION || nCmd == ALD_HORSE_WRW_ANUMATION || nCmd == ALD_HORSE_WRR_ANIMATION 
                        || nCmd == ALD_YANGT_ANIMATION || nCmd == ALD_YANGT_LOCK_ANIMATION || nCmd == ALD_YANGT_OPEN_ANIMATION 
                        || nCmd == ALD_FIRE_SMALL_ANIMATION || nCmd == ALD_FIRE_LARGE_ANIMATION)
                    {
                        m_bGodAnimation = true;
                    }
                }
                else
                {
                    //잭팟애니매션중이다.
                    m_clsSendScoreInfo = MakeEmptyOneRoll();
                    m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
                }


                if (m_lstAldPrizeInfo.Count == 0)
                {
                    m_nGearJack = 0;
                    SetGiveStep(0);
                    CDataBase.SaveGearInfoToDB(this);

                    if (m_prizeInfo != null)
                    {
                        CGlobal.CalculateJackPotCash(m_prizeInfo.m_nJackCash);
                        OnBroadCastPrizeInfo();
                    }
                        
                    ClearPrizeInfo();
                }
            }
            else if (m_lstAldScoreInfo.Count > 0)
            {
                m_clsSendScoreInfo = m_lstAldScoreInfo[0];
                m_lstAldScoreInfo.RemoveAt(0);
            }
            else
            {
                m_clsSendScoreInfo = MakeEmptyOneRoll();
                m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
            }

            CScorePacket scorePacket = new CScorePacket();
            scorePacket.nGearCode = m_nGearCode;
            scorePacket.strPktData = JsonConvert.SerializeObject(m_clsSendScoreInfo);

            clsSocket.SendScoreInfo(scorePacket);
            OnBroadCastPrizeKind();
        }

        public override void OnEndPrizeCall()
        {
            throw new NotImplementedException();
        }


        public override void OnBroadCastPrizeInfo()
        {
            if (m_prizeInfo == null)
                return;
            if (m_prizeInfo.m_nJackCash == 0)
                return;
            if (m_prizeInfo.m_nJackCont == ALD_YANGT_LOCK)
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
            clsPrizeInfoPacket.m_nGameCode = CDefine.GAME_ALADIN;
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

            int nCont = m_prizeInfo.m_nJackCont;
            string strMessage = string.Empty;
            string strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);

            switch (nCmd)
            {
                case ALD_HORSE_ANIMATION:
                    //if (nCont == 1)
                    //    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]백마[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_HORSE_WRR_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]백마[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_HORSE_WWW_ANIMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]백마[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_HORSE_WRW_ANUMATION:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]백마[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_YANGT_ANIMATION:
                    if (nCont == 4 || nCont == 5)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]양탄자[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_FIRE_SMALL_ANIMATION:
                    if (nCont == 2)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]작은불[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_FIRE_LARGE_ANIMATION:
                    if (nCont == 3)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][EC0DB0]큰불[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_FEVER_LOCK_ANIMATION:
                    if (nCont == 6 || nCont == 7)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]휘버[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_FEVER_OPEN_ANIMATION:
                    if (nCont == 6 || nCont == 7)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]휘버[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ALD_THREE_SEVEN_ANIMATION:
                    if (nCont == 8)
                        strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]삼칠별타[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_END:
                    string[] lstPrizeKind = { "", "백마", "작은불", "큰불", "양탄자", "양탄자", "휘버", "휘버", "삼칠별타", "돌발1", "돌발2", "돌발3" };
                    if (m_prizeInfo.m_nJackCash > 0 && m_prizeInfo.m_nJackCont != ALD_YANGT_LOCK)
                    {
                        strMessage = MakeCongration(lstPrizeKind[nCont], m_prizeInfo.m_nJackCash);
                    }

                    break;
            }

            if (strMessage != string.Empty)
                CGlobal.SendNoticeBroadCast(strMessage);
        }

        public override void OnAddSlotCash(CUserSocket clsSocket, int nCash)
        {
            if (m_nSlotCash == 0 && m_nGiftCash == 0)
            {
                m_bSendSpin = false;
                return;
            }

            m_bSendSpin = true;
            if (m_nSlotCash < 200)
            {
                OnAddGiftCash(clsSocket, m_nSlotCash - 200, -1);
                m_nSlotCash = 0;
            }
            else
            {
                m_nSlotCash -= 200;
                m_nAccuCash += 200;
            }
            

            CGlobal.CalcShowJackPot(200, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(200, m_nTakeUser);//누적금액을 사용한 금액만큼 증가시킨다.

            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            if(nKind > 0)
            {
                if (m_clsSendScoreInfo == null || m_clsSendScoreInfo.m_nScore == 0)
                    return;

                if (nKind > m_clsSendScoreInfo.m_nPrizeCmd)
                    return;

                if (nCash > m_clsSendScoreInfo.m_nScore * Math.Pow(2, m_clsSendScoreInfo.m_nPrizeCmd))
                    return;
            }
            else if(nKind == -1)
            {

            }
            else
            {
                if (m_clsSendScoreInfo == null)
                    return;
                if (nCash != m_clsSendScoreInfo.m_nScore)
                    return;
            }

            m_nGiftCash += nCash;
            if(nCash > 0)
                m_nAccuCash -= nCash;

            CUser user = CGlobal.GetUserByCode(m_nTakeUser);
            CAgent agent = CGlobal.GetAgentByCode(user.m_nAgenCode);
            if (nCash > 0)
            {
                //if (agent.m_nAgenLevel < 10 && user.m_nChargeCnt > 0)
                //    user.m_nUserWinCash += nCash;

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
                    //user.m_nUserWinCash = user.m_nUserWinCash - (nExCount * 4 * 500);
                }
                else
                {
                    user.m_nVirtualCupn += (nExCount * 4);
                }

                m_nGiftCash -= nExCount * 20000;

                //if (m_nGearJack == 1)
                //{
                //    m_nGiftCash = 0;
                //}

                clsSocket.SendExcupn(m_nGearCode, nExCount * 4);
            }

            m_nGiftCash = m_nGiftCash < 0 ? 0 : m_nGiftCash;
            CDataBase.SaveGearInfoToDB(this);
            CDataBase.SaveUserInfoToDB(user);
            clsSocket.SendGearInfo(this);
            CGlobal.SendUserInfoToClient(user);
        }


        //들어온 점수를 여러 점수들의 합으로 나눈다.
        public void MakeNormalScoreList(int nDstCash, List<int> lstScore)
        {
            int idx = 0;
            int nDelta = nDstCash / 2;
            for (int i = 0; i < m_lstALDScores.Count; i++)
            {
                if (nDelta <= m_lstALDScores[i])
                {
                    idx = i + 1;
                    break;
                }
            }

            while (nDstCash >= 200)
            {
                int nScore = 0;
                while (true)
                {
                    int nScoreIndex = RND.Next(0, idx);
                    nScore = m_lstALDScores[nScoreIndex];
                    if (nDstCash >= nScore)
                        break;
                }
                lstScore.Add(nScore);
                nDstCash -= nScore;
            }
        }

        //점수로부터 일반점수정보 만들기
        private CALDScoreInfo MakeNormalScore(int nScore)
        {
            CALDScoreInfo scoreInfo = null;

            if (nScore == 0)
                return scoreInfo;
            //배당설정
            int nMult = MakeMult(nScore);
            //점수테이블리스트를 만든다.
            List<ALDScoreTableInfo> lstMatchScoreInfo = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore == nScore);
            ALDScoreTableInfo scoreTable = CGlobal.RandomSelect(lstMatchScoreInfo);

            scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            if (nScore <= 2000)
            {
                int nRnd = RND.Next(12);
                if (nRnd < 2)
                    scoreInfo.m_nPrizeCmd = 0;  //작은점수일때는 미니게임의 당첨회수를 나타낸다.
                else if (nRnd < 6)
                    scoreInfo.m_nPrizeCmd = 1;
                else if (nRnd < 9)
                    scoreInfo.m_nPrizeCmd = 2;
                else if (nRnd < 11)
                    scoreInfo.m_nPrizeCmd = 3;
                else
                    scoreInfo.m_nPrizeCmd = 4;
            }

            return scoreInfo;
        }

        private int MakeMult(int nDstScore)
        {
            int nLimIdx = m_lstALDMults.Length;
            for (int j = 0; j < m_lstALDMults.Length; j++)
            {
                int nDelta = nDstScore / m_lstALDMults[j];
                if (nDelta < 200)
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
                nMult = m_lstALDMults[nMultIndex];      //배당
                nTempScore = nDstScore / nMult;

                if (m_lstALDScores.ToList().Exists(value => value == nTempScore))
                    break;

            }

            return nMult;
        }


        private void MakeEmptyRoll(int nFrom, int nTo)
        {
            int nCount = RND.Next(nFrom, nTo);
            List<CALDScoreInfo> lstEmpty = new List<CALDScoreInfo>();

            for (int i = 0; i < nCount; i++)
            {
                CALDScoreInfo scoreInfo = new CALDScoreInfo();
                scoreInfo.m_nScore = 0;
                scoreInfo.m_nMult = RND.Next(0, m_lstALDMults.Length);

                lstEmpty.Add(scoreInfo);
            }

            m_lstAldPrizeInfo.AddRange(lstEmpty);
        }

       
        private CALDScoreInfo MakeEmptyOneRoll()
        {
            CALDScoreInfo scoreInfo = new CALDScoreInfo();
            scoreInfo.m_nScore = 0;
            scoreInfo.m_nMult = RND.Next(0, m_lstALDMults.Length);

            return scoreInfo;
        }

        //스코테이블로부터 스코정보객체를 얻는 함수
        private CALDScoreInfo ConvertScoreTableToScoreInfo(ALDScoreTableInfo aldScoreTableInfo)
        {
            CALDScoreInfo scoreInfo = new CALDScoreInfo();
            int nRow = aldScoreTableInfo.nLineIndex;

            for (int i = 0; i < 4; i++)
            {
                int nTile = aldScoreTableInfo.lstTileNote[i];
                if (nTile > 0)
                {
                    CTile tile = new CTile(nTile, nRow, i);
                    scoreInfo.m_lstTile.Add(tile);
                }
            }

            scoreInfo.m_nMult = aldScoreTableInfo.nMulti;
            scoreInfo.m_nScore = aldScoreTableInfo.nGetScore;
            scoreInfo.m_nHintScore = aldScoreTableInfo.nSlogan;

            return scoreInfo;
        }

       
        private void MakeGodCommand(int nCmd)
        {
            CALDScoreInfo cmdInfo = MakeEmptyOneRoll();
            cmdInfo.m_nPrizeCmd = nCmd;

            m_lstAldPrizeInfo.Add(cmdInfo);
        }

        private void MakeHorseWWPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempCash = nPrizeCash;

            MakeGodCommand(ALD_HORSE_ANIMATION);
            

            List<ALDScoreTableInfo> lst3Bar = m_lstALDNormalScoreTable.ToList().FindAll(
                value => (value.lstTileNote[0] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.nCount == 3)
                       || (value.lstTileNote[3] == 2 && value.lstTileNote[1] == 2 && value.lstTileNote[2] == 2 && value.nCount == 3)
                       || (value.lstTileNote[0] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.nCount == 3)
                       || (value.lstTileNote[3] == 3 && value.lstTileNote[1] == 3 && value.lstTileNote[2] == 3 && value.nCount == 3)
                       || (value.lstTileNote[0] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.nCount == 3)
                       || (value.lstTileNote[3] == 4 && value.lstTileNote[1] == 4 && value.lstTileNote[2] == 4 && value.nCount == 3)
            );


            List<ALDComplexScoreTable> lstTempPrizeTable = new List<ALDComplexScoreTable>();  //기본점수배렬
            while (true)
            {
                List<ALDScoreTableInfo> lstTemp = lst3Bar.FindAll(value => value.nGetScore <= nTempCash).OrderByDescending(value => value.nGetScore).ToList();
                ALDComplexScoreTable complexScoreTable = new ALDComplexScoreTable();
                int nCount = 0;
                if (lstTemp.Count > 3)
                    nCount = 3;
                else
                    nCount = lstTemp.Count;
                complexScoreTable.largeScore = lstTemp[RND.Next(nCount)];
                lstTempPrizeTable.Add(complexScoreTable);

                nTempCash -= lstTempPrizeTable[lstTempPrizeTable.Count - 1].largeScore.nGetScore;
                if (nTempCash < 4000)
                {
                    break;
                }
            }

            if (nTempCash > 0)
            {
                for (int i = 0; i < lstTempPrizeTable.Count; i++)
                {
                    int nLineIndex = lstTempPrizeTable[i].largeScore.nLineIndex;
                    int nMulti = lstTempPrizeTable[i].largeScore.nMulti;
                    List<ALDScoreTableInfo> lstScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nLineIndex != nLineIndex && value.nMulti == nMulti && value.nGetScore <= nTempCash);
                    if (lstScoreTable.Count > 0)
                    {
                        ALDScoreTableInfo smallTableInfo = lstScoreTable[RND.Next(lstScoreTable.Count)];
                        lstTempPrizeTable[i].smallScore = smallTableInfo;
                        nTempCash -= smallTableInfo.nGetScore;
                    }

                    if (nTempCash < 200)
                        break;
                }
            }


            List<ALDComplexScoreTable> lstTempPrizeTable1 = new List<ALDComplexScoreTable>();  //기본점수배렬 랜덤으로 다시 설정
            while (lstTempPrizeTable.Count > 0)
            {
                int nIdx = RND.Next(lstTempPrizeTable.Count);
                lstTempPrizeTable1.Add(lstTempPrizeTable[nIdx]);
                lstTempPrizeTable.RemoveAt(nIdx);
            }

            if (nPrizeCash < 100000)
            {
                MakeGodCommand(ALD_HORSE_WWW_ANIMATION);
            }
            else if (nPrizeCash < 400000)
            {
                MakeGodCommand(ALD_HORSE_WRW_ANUMATION);
            }
            else
            {
                MakeGodCommand(ALD_HORSE_WRR_ANIMATION);
            }


            //애니매션만들기
            for (int i = 0; i < lstTempPrizeTable1.Count; i++)
            {
                MakeEmptyRoll(1, 3);
                CALDScoreInfo scoreInfo = ConvertComplexTableToScoreInfo(lstTempPrizeTable1[i]);
                lstPrizeInfo.Add(scoreInfo);
            }
            m_nRealPrizeCash = nPrizeCash - nTempCash;
            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nTempCash);
        }

        //스코테이블로부터 스코정보객체를 얻는 함수
        private CALDScoreInfo ConvertComplexTableToScoreInfo(ALDComplexScoreTable aldComplexTableInfo)
        {
            CALDScoreInfo scoreInfo = new CALDScoreInfo();

            ALDScoreTableInfo largeTable = aldComplexTableInfo.largeScore;
            ALDScoreTableInfo smallTable = aldComplexTableInfo.smallScore;
            ALDScoreTableInfo miniTable = aldComplexTableInfo.miniScore;
            int nRow = largeTable.nLineIndex;
            int nMulti = largeTable.nMulti;
            int nScore = largeTable.nGetScore;


            for (int i = 0; i < 4; i++)
            {
                int nTile = largeTable.lstTileNote[i];
                if (nTile > 0)
                {
                    CTile tile = new CTile(nTile, nRow, i);
                    scoreInfo.m_lstTile.Add(tile);
                }

            }
            if (smallTable != null)
            {
                nRow = smallTable.nLineIndex;
                nScore += smallTable.nGetScore;
                for (int i = 0; i < 4; i++)
                {
                    int nTile = smallTable.lstTileNote[i];
                    if (nTile > 0)
                    {
                        CTile tile = new CTile(nTile, nRow, i);
                        scoreInfo.m_lstTile.Add(tile);
                    }

                }
            }

            if (miniTable != null)
            {
                nRow = miniTable.nLineIndex;
                nScore += miniTable.nGetScore;
                for (int i = 0; i < 4; i++)
                {
                    int nTile = miniTable.lstTileNote[i];
                    if (nTile > 0)
                    {
                        CTile tile = new CTile(nTile, nRow, i);
                        scoreInfo.m_lstTile.Add(tile);
                    }

                }
            }

            scoreInfo.m_nMult = nMulti;
            scoreInfo.m_nScore = nScore;
            scoreInfo.m_nHintScore = largeTable.nSlogan;

            return scoreInfo;
        }

        //나머지 점수를 주는 함수이다.
        private int MakeRememberScore(List<CALDScoreInfo> lstPrizeInfo, int nScore)
        {
            int nRetScore = 0;
            int nTempScore = nScore;
            List<CALDScoreInfo> lstTempScoreInfo = new List<CALDScoreInfo>();
            while (nTempScore >= 200)
            {
                List<ALDScoreTableInfo> lstScoreTableInfo = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore <= nTempScore && value.nGetScore >= nTempScore / 2);
                CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(lstScoreTableInfo[RND.Next(lstScoreTableInfo.Count)]);
                if (scoreInfo.m_nScore <= 4000)
                {
                    int nRnd = RND.Next(12);
                    if (nRnd < 2)
                        scoreInfo.m_nPrizeCmd = 0;  //작은점수일때는 미니게임의 당첨회수를 나타낸다.
                    else if (nRnd < 6)
                        scoreInfo.m_nPrizeCmd = 1;
                    else if (nRnd < 9)
                        scoreInfo.m_nPrizeCmd = 2;
                    else if (nRnd < 11)
                        scoreInfo.m_nPrizeCmd = 3;
                    else
                        scoreInfo.m_nPrizeCmd = 4;
                }
                lstTempScoreInfo.Add(scoreInfo);
                nRetScore += scoreInfo.m_nScore;
                nTempScore -= scoreInfo.m_nScore;
            }

            while (lstTempScoreInfo.Count > 0)
            {
                MakeEmptyRoll(2, 4);
                int nIndex = RND.Next(lstTempScoreInfo.Count);
                lstPrizeInfo.Add(lstTempScoreInfo[nIndex]);
                lstTempScoreInfo.RemoveAt(nIndex);
            }

            return nRetScore;
        }

        private void MakeFireSmallPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempCash = nPrizeCash - 8000;
            if (nTempCash < 32000)
                nTempCash = 32000;

            MakeGodCommand(ALD_FIRE_SMALL_ANIMATION);
            MakeEmptyRoll(1, 3);

            List<ALDScoreTableInfo> lstScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (value.lstTileNote[0] == 2 || value.lstTileNote[1] == 2 || value.lstTileNote[2] == 2 || value.lstTileNote[3] == 2
                           || value.lstTileNote[0] == 3 || value.lstTileNote[1] == 3 || value.lstTileNote[2] == 3 || value.lstTileNote[3] == 3
                           || value.lstTileNote[0] == 4 || value.lstTileNote[1] == 4 || value.lstTileNote[2] == 4 || value.lstTileNote[3] == 4)
                           && value.nCount == 4 && value.nGetScore <= nTempCash
                ).OrderByDescending(value => value.nGetScore).ToList();

            ALDScoreTableInfo scoreTableInfo = lstScoreTable[0];
            CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTableInfo);
            lstPrizeInfo.Add(scoreInfo);

            m_nRealPrizeCash = scoreInfo.m_nScore;
            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nPrizeCash - scoreInfo.m_nScore);
        }

        private void MakeFireLargePrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempCash = nPrizeCash;

            MakeGodCommand(ALD_FIRE_LARGE_ANIMATION);
            MakeEmptyRoll(1, 3);

            List<ALDScoreTableInfo> lstScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => value.lstTileNote[0] == 1 && value.lstTileNote[1] == 1 && value.lstTileNote[2] == 1 && value.lstTileNote[3] == 1
                           && value.nCount == 4 && value.nGetScore == nTempCash && value.nLineIndex == 1
                );

            ALDScoreTableInfo scoreTableInfo = lstScoreTable[RND.Next(lstScoreTable.Count)];
            CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTableInfo);
            lstPrizeInfo.Add(scoreInfo);

            m_nRealPrizeCash = scoreInfo.m_nScore;
            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nPrizeCash - scoreInfo.m_nScore);
        }

        private void MakeYangTLockPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            m_nRealPrizeCash = MakeYangTAnimation(lstPrizeInfo, 0, 0);
            MakeEmptyRoll(1, 3);
        }

        private int MakeYangTAnimation(List<CALDScoreInfo> lstPrizeInfo, int nPrizeCash, int nLockOpen)
        {
            MakeGodCommand(ALD_YANGT_ANIMATION);

            List<ALDScoreTableInfo> lstYangTan = null;

            if (nPrizeCash == 0)
            {
                lstYangTan = m_lstALDTwoJackerScoreTable.ToList();
            }
            else
            {
                if (m_prizeInfo.m_nJackCash < 40 * 10000)
                {
                    int nRnd = RND.Next(10);
                    if (nRnd == 5 || nRnd == 8)
                    {
                        lstYangTan = m_lstALDTwoJackerScoreTable.ToList();
                    }
                    else
                    {
                        lstYangTan = m_lstALDThreeJackerScoreTable.ToList().FindAll(value => value.nGetScore <= nPrizeCash);
                    }
                }
                else
                {
                    lstYangTan = m_lstALDThreeJackerScoreTable.ToList().FindAll(value => value.nGetScore <= nPrizeCash);
                }

            }

            if(lstYangTan == null || lstYangTan.Count == 0)
                lstYangTan = m_lstALDTwoJackerScoreTable.ToList();

            ALDScoreTableInfo tableInfo = lstYangTan[RND.Next(lstYangTan.Count)];
            CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
            if (nLockOpen == 0)
                scoreInfo.m_nPrizeCmd = ALD_YANGT_LOCK_ANIMATION;
            else
                scoreInfo.m_nPrizeCmd = ALD_YANGT_OPEN_ANIMATION;
            lstPrizeInfo.Add(scoreInfo);

            int nScore = scoreInfo.m_nScore < 0 ? 0 : scoreInfo.m_nScore;

            return nScore;
        }

        private void MakeYangTOpenPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempScore = nPrizeCash; //점수를 3 ~ 4번으로 갈라주기 위한것

            List<ALDScoreTableInfo> lstScoreTable1;

            m_nRealPrizeCash = 0;
            m_nRealPrizeCash = MakeYangTAnimation(lstPrizeInfo, nPrizeCash, 1);
            nTempScore -= m_nRealPrizeCash;

            int nTempBackScore = nTempScore;
            if (m_nRealPrizeCash == 0)  //Two 조커로 예시를 주었을때이다.
            {
                lstScoreTable1 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            (value.lstTileNote[0] == 5 || value.lstTileNote[1] == 5 || value.lstTileNote[2] == 5 || value.lstTileNote[3] == 5) ||
                            (value.lstTileNote[0] == 6 || value.lstTileNote[1] == 6 || value.lstTileNote[2] == 6 || value.lstTileNote[3] == 6) ||
                            (value.lstTileNote[0] == 7 || value.lstTileNote[1] == 7 || value.lstTileNote[2] == 7 || value.lstTileNote[3] == 7)
                        ) && (
                            value.lstTileNote[0] != 101 && value.lstTileNote[1] != 101 && value.lstTileNote[2] != 101 && value.lstTileNote[3] != 101
                            && value.lstTileNote[0] != 11 && value.lstTileNote[1] != 11 && value.lstTileNote[2] != 11 && value.lstTileNote[3] != 11
                        ) && value.nLineIndex == 1 && value.nGetScore <= nTempScore
                    )
                ).OrderByDescending(value => value.nGetScore).ToList();


                if (lstScoreTable1.Count > 0)
                {
                    CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(lstScoreTable1[0]);
                    lstPrizeInfo.Add(scoreInfo);

                    m_nRealPrizeCash += scoreInfo.m_nScore;
                }
            }
            else //Three 조커로 예시를 주었을때이다.
            {
                lstScoreTable1 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            (value.lstTileNote[0] == 2 || value.lstTileNote[1] == 2 || value.lstTileNote[2] == 2 || value.lstTileNote[3] == 2) ||
                            (value.lstTileNote[0] == 3 || value.lstTileNote[1] == 3 || value.lstTileNote[2] == 3 || value.lstTileNote[3] == 3) ||
                            (value.lstTileNote[0] == 4 || value.lstTileNote[1] == 4 || value.lstTileNote[2] == 4 || value.lstTileNote[3] == 4)
                        ) && (
                            value.lstTileNote[0] != 101 && value.lstTileNote[1] != 101 && value.lstTileNote[2] != 101 && value.lstTileNote[3] != 101
                            && value.lstTileNote[0] != 11 && value.lstTileNote[1] != 11 && value.lstTileNote[2] != 11 && value.lstTileNote[3] != 11
                        ) && value.nLineIndex == 1 && value.nGetScore <= nTempScore
                    )
                ).OrderByDescending(value => value.nGetScore).ToList();

                if (lstScoreTable1.Count > 0)
                {
                    CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(lstScoreTable1[0]);
                    lstPrizeInfo.Add(scoreInfo);
                    m_nRealPrizeCash += scoreInfo.m_nScore;
                }
            }

            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nPrizeCash - m_nRealPrizeCash);
        }

        private void MakeFeverLockPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            MakeFeverAnimation(lstPrizeInfo);
        }

        private void MakeFeverAnimation(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nRnd = RND.Next(5, 11);
            for (int i = 0; i < nRnd; i++)
            {
                MakeEmptyRoll(1, 4);

                CALDScoreInfo scoreInfo = new CALDScoreInfo();
                scoreInfo.m_nMult = m_lstALDMults[RND.Next(m_lstALDMults.Length)];
                scoreInfo.m_lstTile.Add(new CTile(1, 1, 0));
                scoreInfo.m_lstTile.Add(new CTile(1, 2, 1));
                scoreInfo.m_lstTile.Add(new CTile(1, 1, 2));
                scoreInfo.m_lstTile.Add(new CTile(1, 2, 3));

                scoreInfo.m_nScore = -5;

                lstPrizeInfo.Add(scoreInfo);
            }
        }

        private void MakeFeverOpenPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            MakeFeverAnimation(lstPrizeInfo);
            MakeGodCommand(ALD_FEVER_OPEN_ANIMATION);
            MakeEmptyRoll(1, 3);

            List<CALDScoreInfo> lstTempScoreInfo = new List<CALDScoreInfo>();
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempScore = nPrizeCash;

            if (nPrizeCash < 40 * 10000)
            {
                //한번에 점수를 다 준다.
                int nScore = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore <= nTempScore).Max(value => value.nGetScore);
                List<ALDScoreTableInfo> lstALDScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore == nScore).ToList();
                ALDScoreTableInfo scoreTableInfo = lstALDScoreTable[RND.Next(lstALDScoreTable.Count)];
                CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTableInfo);
                lstTempScoreInfo.Add(scoreInfo);
            }
            else if (nPrizeCash < 60 * 10000)
            {
                //두번에 나누어 준다.
                List<ALDScoreTableInfo> lstALDScoreTable0 = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore >= 20 * 10000 && value.nGetScore <= 40 * 10000).ToList();
                ALDScoreTableInfo scoreTableInfo0 = lstALDScoreTable0[RND.Next(lstALDScoreTable0.Count)];
                CALDScoreInfo scoreInfo0 = ConvertScoreTableToScoreInfo(scoreTableInfo0);
                lstTempScoreInfo.Add(scoreInfo0);

                nTempScore -= scoreInfo0.m_nScore;
                if (m_lstALDNormalScoreTable.ToList().Exists(value => value.nGetScore <= nTempScore))
                {
                    int nScore = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore <= nTempScore).Max(value => value.nGetScore);
                    List<ALDScoreTableInfo> lstALDScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore == nScore).ToList();
                    ALDScoreTableInfo scoreTableInfo = lstALDScoreTable[RND.Next(lstALDScoreTable.Count)];
                    CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTableInfo);
                    lstTempScoreInfo.Add(scoreInfo);
                }
            }
            else
            {
                //세번에 나누어 준다.
                List<ALDScoreTableInfo> lstALDScoreTable0 = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore <= 20 * 10000).ToList();
                ALDScoreTableInfo scoreTableInfo0 = lstALDScoreTable0[RND.Next(lstALDScoreTable0.Count)];
                CALDScoreInfo scoreInfo0 = ConvertScoreTableToScoreInfo(scoreTableInfo0);
                lstTempScoreInfo.Add(scoreInfo0);

                List<ALDScoreTableInfo> lstALDScoreTable1 = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore >= 20 * 10000 && value.nGetScore <= 30 * 10000).ToList();
                ALDScoreTableInfo scoreTableInfo1 = lstALDScoreTable1[RND.Next(lstALDScoreTable1.Count)];
                CALDScoreInfo scoreInfo1 = ConvertScoreTableToScoreInfo(scoreTableInfo0);
                lstTempScoreInfo.Add(scoreInfo1);

                nTempScore = nTempScore - scoreInfo0.m_nScore - scoreInfo1.m_nScore;
                if (m_lstALDNormalScoreTable.ToList().Exists(value => value.nGetScore <= nTempScore))
                {
                    int nScore = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore <= nTempScore).Max(value => value.nGetScore);
                    List<ALDScoreTableInfo> lstALDScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(value => value.nGetScore == nScore).ToList();
                    ALDScoreTableInfo scoreTableInfo = lstALDScoreTable[RND.Next(lstALDScoreTable.Count)];
                    CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTableInfo);
                    lstTempScoreInfo.Add(scoreInfo);
                }
            }

            m_nRealPrizeCash = 0;
            while (lstTempScoreInfo.Count > 0)
            {
                int nIndex = RND.Next(lstTempScoreInfo.Count);
                m_nRealPrizeCash += lstTempScoreInfo[nIndex].m_nScore;
                lstPrizeInfo.Add(lstTempScoreInfo[nIndex]);
                lstTempScoreInfo.RemoveAt(nIndex);
                MakeEmptyRoll(2, 4);
            }

            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nPrizeCash - m_nRealPrizeCash);
        }

        private void MakeThreeSevenPrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempScore = nPrizeCash;

            ALDScoreTableInfo scoreTableInfo = m_lstALDThreeSevenScoreTable[RND.Next(m_lstALDThreeSevenScoreTable.Length)];
            CALDScoreInfo info = ConvertScoreTableToScoreInfo(scoreTableInfo);
            info.m_nPrizeCmd = ALD_THREE_SEVEN_ANIMATION;
            lstPrizeInfo.Add(info);
            MakeEmptyRoll(1, 3);
            CALDScoreInfo info1 = MakeEmptyOneRoll();
            info1.m_nScore = -2;

            List<CALDScoreInfo> lstScore = new List<CALDScoreInfo>();

            while (true)
            {
                List<ALDScoreTableInfo> lstScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => value.nGetScore <= nTempScore
                );
                CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(lstScoreTable[RND.Next(lstScoreTable.Count - 1)]);
                lstScore.Add(scoreInfo);
                nTempScore -= scoreInfo.m_nScore;
                if (nTempScore < 200)
                {
                    if (lstScore.Count < 10)
                        break;
                    else
                    {
                        nTempScore = nPrizeCash;
                        lstScore.Clear();
                    }
                }
            }

            for (int i = 0; i < lstScore.Count; i++)
            {
                m_nRealPrizeCash += lstScore[i].m_nScore;
            }

            while (lstScore.Count < 10)
            {
                CALDScoreInfo emptyInfo = MakeEmptyOneRoll();
                if (RND.Next(2) == 0)
                    lstScore.Add(emptyInfo);
                else
                    lstScore.Insert(RND.Next(lstScore.Count), emptyInfo);
            }

            lstPrizeInfo.AddRange(lstScore);
        }

        private void MakeSurprise1PrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempScore = nPrizeCash - 20000; //점수를 3 ~4번으로 갈라주기위해서이다.

            List<ALDScoreTableInfo> lstScoreTable1;
            List<ALDScoreTableInfo> lstScoreTable2;
            List<ALDScoreTableInfo> lstScoreTable3;

            ALDScoreTableInfo tableInfo1 = null;
            ALDScoreTableInfo tableInfo2 = null;
            ALDScoreTableInfo tableInfo3 = null;

            lstScoreTable1 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            (value.lstTileNote[0] == 2 || value.lstTileNote[1] == 2 || value.lstTileNote[2] == 2 || value.lstTileNote[3] == 2) ||
                            (value.lstTileNote[0] == 3 || value.lstTileNote[1] == 3 || value.lstTileNote[2] == 3 || value.lstTileNote[3] == 3) ||
                            (value.lstTileNote[0] == 4 || value.lstTileNote[1] == 4 || value.lstTileNote[2] == 4 || value.lstTileNote[3] == 4)
                        ) && value.nLineIndex == 1 && value.nGetScore <= nTempScore
                    )
                ).OrderByDescending(value => value.nGetScore).ToList();



            while (true)
            {
                tableInfo1 = lstScoreTable1[0];
                int nTile1 = 0;
                if (tableInfo1.nSlogan < 10)
                {
                    nTile1 = 2;
                }
                else if (tableInfo1.nSlogan < 20)
                {
                    nTile1 = 3;
                }
                else if (tableInfo1.nSlogan < 30)
                {
                    nTile1 = 4;
                }


                nTempScore -= tableInfo1.nGetScore;
                lstScoreTable2 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            (value.lstTileNote[0] == 2 || value.lstTileNote[1] == 2 || value.lstTileNote[2] == 2 || value.lstTileNote[3] == 2) ||
                            (value.lstTileNote[0] == 3 || value.lstTileNote[1] == 3 || value.lstTileNote[2] == 3 || value.lstTileNote[3] == 3) ||
                            (value.lstTileNote[0] == 4 || value.lstTileNote[1] == 4 || value.lstTileNote[2] == 4 || value.lstTileNote[3] == 4)
                        ) && (
                            (value.lstTileNote[0] != nTile1 && value.lstTileNote[1] != nTile1 && value.lstTileNote[2] != nTile1 && value.lstTileNote[3] != nTile1)
                        ) && value.nLineIndex == 0 && value.nMulti == tableInfo1.nMulti && value.nGetScore <= nTempScore

                    )
                ).OrderByDescending(value => value.nGetScore).ToList();

                if (lstScoreTable2 == null || lstScoreTable2.Count == 0)
                    break;

                tableInfo2 = lstScoreTable2[0];
                int nTile2 = 0;
                int nTile3 = 0;
                if (tableInfo2.nSlogan < 10)
                {
                    nTile2 = 2;
                    if (nTile1 == 3 && nTile2 == 2)
                        nTile3 = 4;
                    else
                        nTile3 = 3;
                }
                else if (tableInfo2.nSlogan < 20)
                {
                    nTile2 = 3;
                    if (nTile1 == 2 && nTile2 == 3)
                        nTile3 = 4;
                    else
                        nTile3 = 2;
                }
                else if (tableInfo2.nSlogan < 30)
                {
                    nTile2 = 4;
                    if (nTile1 == 2 && nTile2 == 4)
                        nTile3 = 3;
                    else
                        nTile3 = 2;
                }

                nTempScore -= tableInfo2.nGetScore;

                lstScoreTable3 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            value.lstTileNote[0] == nTile3 || value.lstTileNote[1] == nTile3 || value.lstTileNote[2] == nTile3 || value.lstTileNote[3] == nTile3
                        ) && value.nLineIndex == 2 && value.nMulti == tableInfo1.nMulti && value.nGetScore <= nTempScore

                    )
                ).OrderByDescending(value => value.nGetScore).ToList();
                if (lstScoreTable3 == null || lstScoreTable3.Count == 0)
                    break;

                tableInfo3 = lstScoreTable3[0];
                break;
            }

           
            int nScore = 0;
            if (tableInfo1 != null)
            {
                lstPrizeInfo.Add(ConvertScoreTableToScoreInfo(tableInfo1));
                nScore += tableInfo1.nGetScore;
                MakeEmptyRoll(1, 3);
            }
            if (tableInfo2 != null)
            {
                lstPrizeInfo.Add(ConvertScoreTableToScoreInfo(tableInfo2));
                nScore += tableInfo2.nGetScore;
                MakeEmptyRoll(1, 3);
            }
            if (tableInfo3 != null)
            {
                lstPrizeInfo.Add(ConvertScoreTableToScoreInfo(tableInfo3));
                nScore += tableInfo3.nGetScore;
                MakeEmptyRoll(1, 3);
            }


            m_nRealPrizeCash = nScore;
            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nPrizeCash - m_nRealPrizeCash);

        }

        private void MakeSurprise2PrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;

            List<ALDScoreTableInfo> lstScoreTable = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => value.lstTileNote[0] == 1 && value.lstTileNote[1] == 1 && value.lstTileNote[2] == 1 && value.lstTileNote[3] == 1 && value.nGetScore == nPrizeCash
                );

            ALDScoreTableInfo scoreTableInfo = lstScoreTable[RND.Next(lstScoreTable.Count)];
            CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTableInfo);


            lstPrizeInfo.Add(scoreInfo);
            m_nRealPrizeCash = scoreInfo.m_nScore;
        }

        private void MakeSurprise3PrizeRoll(List<CALDScoreInfo> lstPrizeInfo)
        {
            int nPrizeCash = m_prizeInfo.m_nJackCash;
            int nTempScore = nPrizeCash - 20000; //점수를 3 ~ 4번으로 갈라주기 위한동작

            List<ALDScoreTableInfo> lstYangTan;
            List<ALDScoreTableInfo> lstScoreTable1;
            List<ALDScoreTableInfo> lstScoreTable2;
            List<ALDScoreTableInfo> lstScoreTable3;

            lstYangTan = m_lstALDThreeJackerScoreTable.ToList();

            ALDScoreTableInfo tableInfo = lstYangTan[RND.Next(lstYangTan.Count)];
            CALDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
            lstPrizeInfo.Add(scoreInfo);
            m_nRealPrizeCash = scoreInfo.m_nScore;

            nTempScore -= tableInfo.nGetScore;


            ALDScoreTableInfo tableInfo1 = null;
            ALDScoreTableInfo tableInfo2 = null;
            ALDScoreTableInfo tableInfo3 = null;

            lstScoreTable1 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            (value.lstTileNote[0] == 2 || value.lstTileNote[1] == 2 || value.lstTileNote[2] == 2 || value.lstTileNote[3] == 2) ||
                            (value.lstTileNote[0] == 3 || value.lstTileNote[1] == 3 || value.lstTileNote[2] == 3 || value.lstTileNote[3] == 3) ||
                            (value.lstTileNote[0] == 4 || value.lstTileNote[1] == 4 || value.lstTileNote[2] == 4 || value.lstTileNote[3] == 4)
                        ) && (
                            value.lstTileNote[0] != 101 && value.lstTileNote[1] != 101 && value.lstTileNote[2] != 101 && value.lstTileNote[3] != 101
                            && value.lstTileNote[0] != 11 && value.lstTileNote[1] != 11 && value.lstTileNote[2] != 11 && value.lstTileNote[3] != 11
                        ) && value.nLineIndex == 1 && value.nGetScore <= nTempScore
                    )
                ).OrderByDescending(value => value.nGetScore).ToList();



            while (true)
            {
                tableInfo1 = lstScoreTable1[0];
                int nTile1 = 0;
                if (tableInfo1.nSlogan < 10)
                {
                    nTile1 = 2;
                }
                else if (tableInfo1.nSlogan < 20)
                {
                    nTile1 = 3;
                }
                else if (tableInfo1.nSlogan < 30)
                {
                    nTile1 = 4;
                }


                nTempScore -= tableInfo1.nGetScore;
                lstScoreTable2 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            (value.lstTileNote[0] == 2 || value.lstTileNote[1] == 2 || value.lstTileNote[2] == 2 || value.lstTileNote[3] == 2) ||
                            (value.lstTileNote[0] == 3 || value.lstTileNote[1] == 3 || value.lstTileNote[2] == 3 || value.lstTileNote[3] == 3) ||
                            (value.lstTileNote[0] == 4 || value.lstTileNote[1] == 4 || value.lstTileNote[2] == 4 || value.lstTileNote[3] == 4)
                        ) && (
                            (value.lstTileNote[0] != nTile1 && value.lstTileNote[1] != nTile1 && value.lstTileNote[2] != nTile1 && value.lstTileNote[3] != nTile1)
                        ) && (
                            value.lstTileNote[0] != 101 && value.lstTileNote[1] != 101 && value.lstTileNote[2] != 101 && value.lstTileNote[3] != 101
                            && value.lstTileNote[0] != 11 && value.lstTileNote[1] != 11 && value.lstTileNote[2] != 11 && value.lstTileNote[3] != 11
                        ) && value.nLineIndex == 0 && value.nMulti == tableInfo1.nMulti && value.nGetScore <= nTempScore

                    )
                ).OrderByDescending(value => value.nGetScore).ToList();

                if (lstScoreTable2 == null || lstScoreTable2.Count == 0)
                    break;

                tableInfo2 = lstScoreTable2[0];
                int nTile2 = 0;
                int nTile3 = 0;
                if (tableInfo2.nSlogan < 10)
                {
                    nTile2 = 2;
                    if (nTile1 == 3 && nTile2 == 2)
                        nTile3 = 4;
                    else
                        nTile3 = 3;
                }
                else if (tableInfo2.nSlogan < 20)
                {
                    nTile2 = 3;
                    if (nTile1 == 2 && nTile2 == 3)
                        nTile3 = 4;
                    else
                        nTile3 = 2;
                }
                else if (tableInfo2.nSlogan < 30)
                {
                    nTile2 = 4;
                    if (nTile1 == 2 && nTile2 == 4)
                        nTile3 = 3;
                    else
                        nTile3 = 2;
                }

                nTempScore -= tableInfo2.nGetScore;

                lstScoreTable3 = m_lstALDNormalScoreTable.ToList().FindAll(
                    value => (
                        (
                            value.lstTileNote[0] == nTile3 || value.lstTileNote[1] == nTile3 || value.lstTileNote[2] == nTile3 || value.lstTileNote[3] == nTile3
                        ) && (
                            value.lstTileNote[0] != 101 && value.lstTileNote[1] != 101 && value.lstTileNote[2] != 101 && value.lstTileNote[3] != 101
                            && value.lstTileNote[0] != 11 && value.lstTileNote[1] != 11 && value.lstTileNote[2] != 11 && value.lstTileNote[3] != 11
                        ) && value.nLineIndex == 2 && value.nMulti == tableInfo1.nMulti && value.nGetScore <= nTempScore

                    )
                ).OrderByDescending(value => value.nGetScore).ToList();
                if (lstScoreTable3 == null || lstScoreTable3.Count == 0)
                    break;

                tableInfo3 = lstScoreTable3[0];
                break;
            }

            CALDScoreInfo barSpin = MakeEmptyOneRoll();
            barSpin.m_nScore = -4; //올빠스핀돌기
            lstPrizeInfo.Add(barSpin);

            int nScore = 0;
            if (tableInfo1 != null)
            {
                lstPrizeInfo.Add(ConvertScoreTableToScoreInfo(tableInfo1));
                nScore += tableInfo1.nGetScore;
                MakeEmptyRoll(1, 3);
            }
            if (tableInfo2 != null)
            {
                lstPrizeInfo.Add(ConvertScoreTableToScoreInfo(tableInfo2));
                nScore += tableInfo2.nGetScore;
                MakeEmptyRoll(1, 3);
            }
            if (tableInfo3 != null)
            {
                lstPrizeInfo.Add(ConvertScoreTableToScoreInfo(tableInfo3));
                nScore += tableInfo3.nGetScore;
                MakeEmptyRoll(1, 3);
            }


            m_nRealPrizeCash += nScore;

            CALDScoreInfo releseSpin = MakeEmptyOneRoll();
            releseSpin.m_nScore = -7; //올빠스핀돌기해제
            lstPrizeInfo.Add(releseSpin);

            m_nRealPrizeCash += MakeRememberScore(lstPrizeInfo, nPrizeCash - m_nRealPrizeCash);
        }

        public List<CALDScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstAldPrizeInfo;
        }

        public override void UseItemByUser(CItem clsItem)
        {
            throw new NotImplementedException();
        }
    }
}
