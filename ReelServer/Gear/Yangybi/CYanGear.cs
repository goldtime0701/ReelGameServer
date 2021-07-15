using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CYanGear : CBaseGear
    {
        private CYANPrizeInfo m_prizeInfo;                  //잭팟정보
        private List<CYANScoreInfo> m_lstPrizeInfo;         //잭팟정보리스트
        private List<CYANScoreInfo> m_lstScoreInfo;         //점수정보리스트
        private CYANScoreInfo m_clsSendScoreInfo;           //현재클라이언트에 전송한 릴배렬

        private List<CYANScoreInfo> m_lstReturnGift;        //잭팟이 끝난다음에 돌려주어야 하는 점수정보리스트
        private int m_nReturnCash;                          //잭팟이 끝난다음에 돌려주어야 하는 점수
        private bool m_bReturnGift;                         //잭팟이 끝난다음에 점수를 돌려주는 상태인가

        private int m_nOverCash;                            //점수배렬을 만들면서 더 첨부된 점수
        private bool m_bPrizeAni;                           //잭팟예시 애니매션상태인가
        private int m_nPrizeAniCnt;                         //잭팟애니매션이 진행되는동안 몇번돌았는가(애니매션이 진행되던 도중 통신이 끊어지는 경우 사용,  20보다 크면 애니매션이 끝났다고 본다.)
        //private int m_nNoiseCash;                           //가짜밤예시를 주기 위한 변수
        //private int m_nNoiseRaise;                          //가짜잭팟발동금액


        public CYanGear() : base()
        {
            m_nSpeedCash = 100;
            m_lstScoreInfo = new List<CYANScoreInfo>();
            m_lstPrizeInfo = new List<CYANScoreInfo>();
            m_lstReturnGift = new List<CYANScoreInfo>();

            //m_nNoiseRaise = RND.Next(1500, 3000);
            //m_nNoiseRaise *= 10;
        }


        public override void ClearGear()
        {
            m_nOverCash = 0;
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            m_nReturnCash = 0;
            SetGiveStep(0);
            m_lstPrizeInfo.Clear();
            m_lstScoreInfo.Clear();
            m_lstReturnGift.Clear();
            m_prizeInfo = null;
            m_bPrizeAni = false;
            m_nPrizeAniCnt = 0;
            m_bReturnGift = false;
        }

        public override void ClearPrizeInfo()
        {
            m_bReturnGift = false;
            m_nGearJack = 0;
            SetGiveStep(0);

            m_prizeInfo = null;
            m_lstPrizeInfo.Clear();
            m_lstReturnGift.Clear();
            m_bPrizeAni = false;
            m_nPrizeAniCnt = 0;
        }


        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<CYANScoreInfo> lstYanScoreInfo = new List<CYANScoreInfo>();

            int nTempCash = nAddCash;

            while (nTempCash > 0)
            {
                int nAppenCash = nTempCash >= 10000 ? 10000 : nTempCash;
                nTempCash -= nAppenCash;
                int nDstCash = (int)(nAppenCash * rate / 100);

                if (m_nOverCash > 0)
                {
                    if (m_nOverCash > nDstCash / 2)
                    {
                        nDstCash = nDstCash / 2;
                        m_nOverCash -= nDstCash;
                    }
                    else
                    {
                        nDstCash -= m_nOverCash;
                        m_nOverCash = 0;
                    }
                }

                //점수렬을 만든다.
                List<int> lstScore = new List<int>();
                MakeNormalScoreList(nDstCash, lstScore);

                List<CYANScoreInfo> lstScoreInfo = new List<CYANScoreInfo>();
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CYANScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CYANScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstYanScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstYanScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CYANScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstYanScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstYanScoreInfo.Count);
                    lstYanScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                m_lstScoreInfo.Clear();
            }

            m_lstScoreInfo.AddRange(lstYanScoreInfo);
        }

        //점수를 여러개의 작은 점수들로 나눈다.
        private void MakeNormalScoreList(int nDstCash, List<int> lstScore)
        {
            int nDelta = nDstCash / 2;
            if (nDelta < 100)
                nDelta = 100;
            while (nDstCash >= 100)
            {
                List<int> lstSeaScore = m_lstScores.FindAll(value => value <= nDelta);
                int nScore = CGlobal.RandomSelect(lstSeaScore);
                lstScore.Add(nScore);

                nDstCash -= nScore;
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

        //점수로부터 일반점수정보 만들기
        private CYANScoreInfo MakeNormalScore(int nScore)
        {
            if (nScore == 0)
                return MakeEmptyRoll();

            //점수테이블리스트를 만든다.
            List<CYANScoreTable> lstMatchScoreInfo = m_lstScoreTable.FindAll(value => value.m_nScore == nScore);
            CYANScoreTable scoreTable = CGlobal.RandomSelect(lstMatchScoreInfo);
            CYANScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nOverCash += scoreInfo.m_nScore - nScore;

            return scoreInfo;
        }

        private CYANScoreInfo ConvertScoreTableToScoreInfo(CYANScoreTable scoreTable)
        {
            CYANScoreInfo scoreInfo = new CYANScoreInfo();

            int nCol = RND.Next(4);  //조커로 변하지 말아야 할 위치

            //맞은 타일을 만든다.
            for (int i = 0; i < scoreTable.m_nlstTile.Length; i++)
            {
                int nTile = scoreTable.m_nlstTile[i];

                if (scoreTable.m_nCount == 4)
                {
                    if (RND.Next(5) == 2 && i != nCol)
                        nTile = JOKE;
                }
                else if (nTile == ANNY)
                {
                    List<int> lstTiles = m_lstTiles.ToList().FindAll(value => value != scoreTable.m_nTile && value != JOKE && value != BERY);
                    nTile = CGlobal.RandomSelect(lstTiles);
                }

                CTile tile = new CTile(nTile, scoreTable.m_nLine, i);
                if (scoreTable.m_nlstTile[i] != ANNY)
                    tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);
            }


            //0번째 라인을 만든다.
            List<int> lstLine0 = new List<int>();
            lstLine0.Add(scoreInfo.m_lstTile.Find(value => value.m_nCol == 0 && value.m_nRow == scoreTable.m_nLine).m_nTile);

            for (int nRow = 0; nRow < 3; nRow++)
            {
                if (nRow == scoreTable.m_nLine)
                {
                    continue;
                }

                int nTile = ANNY;
                while (true)
                {
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != BERY));
                    if (!lstLine0.Exists(value => value == nTile))
                        break;
                }
                lstLine0.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 0);
                scoreInfo.m_lstTile.Add(tile);
            }

            //3번째라인을 만든다.
            List<int> lstLine3 = new List<int>();
            lstLine3.Add(scoreInfo.m_lstTile.Find(value => value.m_nCol == 3 && value.m_nRow == scoreTable.m_nLine).m_nTile);

            for (int nRow = 0; nRow < 3; nRow++)
            {
                if (nRow == scoreTable.m_nLine)
                {
                    continue;
                }

                int nTile = ANNY;
                while (true)
                {
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != BERY));
                    if (!lstLine3.Exists(value => value == nTile))
                        break;
                }
                lstLine3.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 3);
                scoreInfo.m_lstTile.Add(tile);
            }

            //1번째 라인을 만든다.
            List<int> lstLine1 = new List<int>();
            lstLine1.Add(scoreInfo.m_lstTile.Find(value => value.m_nCol == 1 && value.m_nRow == scoreTable.m_nLine).m_nTile);

            for (int nRow = 0; nRow < 3; nRow++)
            {
                if (nRow == scoreTable.m_nLine)
                {
                    continue;
                }

                int nTile = ANNY;
                while (true)
                {
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList());
                    if (lstLine1.Exists(value => value == nTile))
                        continue;

                    if (lstLine0[nRow] == nTile && (nTile == JOKE || nTile == BERY))
                        continue;
                    if (lstLine0[nRow] == JOKE && nTile == BERY)
                        continue;
                    if (nTile == JOKE && lstLine0[nRow] == BERY)
                        continue;
                    if (nTile == JOKE && lstLine3[nRow] == JOKE)
                        continue;

                    break;
                }
                lstLine1.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 1);
                scoreInfo.m_lstTile.Add(tile);
            }

            //2번째 라인을 만든다.
            List<int> lstLine2 = new List<int>();
            lstLine2.Add(scoreInfo.m_lstTile.Find(value => value.m_nCol == 2 && value.m_nRow == scoreTable.m_nLine).m_nTile);

            for (int nRow = 0; nRow < 3; nRow++)
            {
                if (nRow == scoreTable.m_nLine)
                {
                    continue;
                }

                int nTempCnt = 0;
                int nTile = ANNY;
                while (true)
                {
                    nTempCnt++;
                    if (nTempCnt > 5000)
                    {
                        break;
                    }
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList());


                    if (nTile == lstLine3[nRow] && (nTile == JOKE || nTile == BERY))
                        continue;
                    if (lstLine3[nRow] == JOKE && nTile == BERY)
                        continue;
                    if (nTile == JOKE && lstLine3[nRow] == BERY)
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == JOKE || lstLine0[nRow] == JOKE || lstLine3[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow] || lstLine1[nRow] == lstLine3[nRow]))
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == lstLine3[nRow] || lstLine0[nRow] == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine3[nRow] && (lstLine1[nRow] == JOKE || nTile == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine1[nRow] && (lstLine3[nRow] == JOKE || nTile == lstLine3[nRow] || lstLine0[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow]))
                        continue;

                    if (lstLine2.Exists(value => value == nTile))
                        continue;

                    break;
                }

                lstLine2.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 2);
                scoreInfo.m_lstTile.Add(tile);

            }

            scoreInfo.m_nWinTile = scoreTable.m_nTile;
            scoreInfo.m_nWinCnt = scoreTable.m_nCount;
            scoreInfo.m_nScore = scoreTable.m_nScore;
            scoreInfo.m_nMulti = scoreTable.m_nMulti;
            scoreInfo.m_lstLine.Add(scoreTable.m_nLine);
            scoreInfo.m_nGearCode = m_nGearCode;


            return scoreInfo;
        }

        private CYANScoreInfo MakeEmptyRoll()
        {
            CYANScoreInfo scoreInfo = new CYANScoreInfo();
            int nTile = ANNY;
            List<int> lstTiles = m_lstTiles.ToList();

            //0번째 라인을 만든다.
            List<int> lstLine0 = new List<int>();
            for(int i=0; i<3; i++)
            {
                nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != BERY && !lstLine0.Exists(val => val == value)));
                lstLine0.Add(nTile);
            }

            //3번라인을 만든다.
            List<int> lstLine3 = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != BERY && !lstLine3.Exists(val => val == value)));
                lstLine3.Add(nTile);
            }

            //1번째 라인을 만든다.
            List<int> lstLine1 = new List<int>();
            for(int i=0; i<3; i++)
            {
                while(true)
                {
                    nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => !lstLine1.Exists(val => val == value)));
                    if (lstLine0[i] == JOKE && nTile == BERY)
                        continue;
                    if (nTile == JOKE && lstLine0[i] == BERY)
                        continue;
                    if (nTile == JOKE && lstLine3[i] == JOKE)
                        continue;

                    lstLine1.Add(nTile);
                    break;
                }
            }

            //2번라인을 만든다.
            List<int> lstLine2 = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                int nTempCnt = 0;
                while (true)
                {
                    nTempCnt++;
                    if (nTempCnt > 5000)
                        break;
                    nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => !lstLine2.Exists(val => val == value)));
                    if (nTile == lstLine3[i] && nTile == JOKE || nTile == BERY)
                        continue;
                    if (lstLine3[i] == JOKE && nTile == BERY)
                        continue;
                    if (nTile == JOKE && lstLine3[i] == BERY)
                        continue;
                    if (nTile == JOKE && (lstLine1[i] == JOKE || lstLine0[i] == JOKE || lstLine0[i] == lstLine1[i]))
                        continue;
                    if (nTile == lstLine3[i] && (lstLine1[i] == JOKE || nTile == lstLine1[i]))
                        continue;
                    if (nTile == lstLine1[i] && (lstLine3[i] == JOKE || nTile == lstLine3[i] || lstLine0[i] == JOKE || lstLine0[i] == lstLine1[i]))
                        continue;
                    if (nTile == JOKE && lstLine1[i] == lstLine3[i])
                        continue;

                    lstLine2.Add(nTile);
                    break;
                }
            }

            for (int nRow = 0; nRow < 3; nRow++)
            {
                CTile tile = new CTile(lstLine0[nRow], nRow, 0);
                scoreInfo.m_lstTile.Add(tile);
                tile = new CTile(lstLine1[nRow], nRow, 1);
                scoreInfo.m_lstTile.Add(tile);
                tile = new CTile(lstLine2[nRow], nRow, 2);
                scoreInfo.m_lstTile.Add(tile);
                tile = new CTile(lstLine3[nRow], nRow, 3);
                scoreInfo.m_lstTile.Add(tile);
            }
            scoreInfo.m_nMulti = 1;

            return scoreInfo;
        }

        public override bool MakePrizeRoll()
        {
            m_nGearJack = 1;    //잭팟상태로 설정
            CDataBase.SaveGearInfoToDB(this);

            //잭팟시작신호를 보낸다.
            MakePrizeCommand(PRIZE_START);
            MakePrizeCommand(PRIZE_NIGHT);
            MakePrizeEmpty(3);

            if (m_prizeInfo.m_nCont == JACK_THUNDER)
            {
                //번개
                MakePrizeCommand(PRIZE_THUNDER);
                MakePrizeEmpty(1, true);
                MakePrizeCommand(PRIZE_DAY);
                PrizeThunder();
            }
            else if (m_prizeInfo.m_nCont == JACK_RAINBOW)
            {
                //무지개
                MakePrizeCommand(PRIZE_RAINBOW);
                MakePrizeEmpty(1, true);
                MakePrizeCommand(PRIZE_DAY);
                PrizeRainbow();
            }
            else if(m_prizeInfo.m_nCont == JACK_TURTLE)
            {
                //거북이
                MakePrizeCommand(PRIZE_TURTLE);
                MakePrizeEmpty(1, true);
                MakePrizeCommand(PRIZE_DAY);
                PrizeScore();
            }
            else if (m_prizeInfo.m_nCont == JACK_BDRAGON)
            {
                //청룡
                MakePrizeCommand(PRIZE_BDRAGON);
                MakePrizeEmpty(1, true);
                MakePrizeCommand(PRIZE_DAY);
                PrizeScore();
            }
            else if (m_prizeInfo.m_nCont == JACK_GDRAGON)
            {
                //금룡
                MakePrizeCommand(PRIZE_GDRAGON);
                MakePrizeEmpty(1, true);
                MakePrizeCommand(PRIZE_DAY);
                PrizeScore();
            }

            MakePrizeCommand(PRIZE_END);
            MakePrizeEmpty(1);
            CDataBase.SaveGearInfoToDB(this);
            return true;
        }

        private void PrizeThunder()
        {
            List<CYANScoreTable> lstTable = m_lstScoreTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CYANScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CYANScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            m_lstPrizeInfo.Add(scoreInfo);
            MakePrizeEmpty(1);
        }

        private void PrizeRainbow()
        {
            List<CYANScoreTable> lstTable = m_lstScoreTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CYANScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CYANScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            m_lstPrizeInfo.Add(scoreInfo);
            MakePrizeEmpty(1);
        }

        private void PrizeScore()
        {
            int nScore = m_prizeInfo.m_nPrizeCash;

            while(true)
            {
                List<CYANScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= nScore).OrderByDescending(value => value.m_nScore).ToList();
                if (lstTable.Count == 0)
                    break;

                CYANScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
                CYANScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
                nScore = nScore - scoreInfo.m_nScore;
                int nRemScore = scoreInfo.m_nScore;
                if (scoreInfo.m_nScore > 20000)
                    scoreInfo.m_nScore = 20000;
                nRemScore = nRemScore - scoreInfo.m_nScore;
                m_lstPrizeInfo.Add(scoreInfo);
                MakeAfterPrizeScoreList(nRemScore);
                MakePrizeEmpty(2);
            }

            m_nReturnCash += nScore;
        }

        private void MakeAfterPrizeScoreList(int nScore)
        {
            if (nScore == 0)
                return;

            List<CYANScoreInfo> lstScoreInfo = new List<CYANScoreInfo>();
            while (nScore >= 100)
            {
                int nCash = 0;
                if (nScore > 20000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 10000));
                else if (nScore > 5000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 1000));
                else
                    nCash = m_lstScores.FindAll(value => value <= nScore).Max(value => value);

                CYANScoreTable scoreTable = CGlobal.RandomSelect(m_lstScoreTable.FindAll(value => value.m_nScore == nCash));
                CYANScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
                lstScoreInfo.Add(scoreInfo);
                nScore -= scoreInfo.m_nScore;
            }

            MakePrizeEmpty(1);
            while (lstScoreInfo.Count > 0)
            {
                CYANScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                m_lstPrizeInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                MakePrizeEmpty(1);
            }
        }


        private void MakePrizeCommand(int nCmd)
        {
            CYANScoreInfo scoreInfo = MakeEmptyRoll();
            scoreInfo.m_nCmd = nCmd;
            m_lstPrizeInfo.Add(scoreInfo);
        }

        private void MakePrizeEmpty(int nCount, bool bPrize = false)
        {
            if (!bPrize)
                nCount = nCount + RND.Next(3);
            for (int i = 0; i < nCount; i++)
            {
                m_lstPrizeInfo.Add(MakeEmptyRoll());
            }
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            m_nGiftCash += nCash;
            if (nKind == 1)
                m_nOverCash += nCash;

            CUser user = CGlobal.GetUserByCode(m_nTakeUser);
            CAgent agent = CGlobal.GetAgentByCode(user.m_nAgenCode);
            if (nCash > 0)
            {
                m_nAccuCash -= nCash;
                if (!m_bReturnGift)
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
                }
                else
                {
                    user.m_nVirtualCupn += (nExCount * 4);
                }

                m_nGiftCash -= 20000;
                if (m_nGearJack == 1)
                {
                    m_nReturnCash += m_nGiftCash;
                    m_nGiftCash = 0;
                }

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
            return;
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
            clsPrizeInfoPacket.m_nPrizeCash = m_prizeInfo.m_nPrizeCash;

            CGlobal.BroadCastPrizeInfo(clsPrizeInfoPacket);
        }

        public override void OnBroadCastPrizeKind()
        {
            if (m_prizeInfo == null)
                return;

            int nCmd = m_clsSendScoreInfo.m_nCmd;
            if (nCmd == 0)
                return;

            string strMessage = string.Empty;
            string strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);

            switch (nCmd)
            {
                case PRIZE_TURTLE:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]거북[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_BDRAGON:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]청룡[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_GDRAGON:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]금룡[-] 출현을 축하드립니다. 대박 나세요..";
                    break;

                case PRIZE_END:
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
                OnAddGiftCash(clsSocket, m_nSlotCash - 100);
                m_nSlotCash = 0;
            }
            else
            {
                m_nSlotCash -= 100;
                m_nAccuCash += 100;  //100원스핀이 돌면 기계누적금액을 30원 올려준다.
            }

            CGlobal.CalcShowJackPot(100, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(100, m_nTakeUser);//누적금액을 사용한 금액만큼 증가시킨다.

            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
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

            if (m_prizeInfo.m_nCont > 2 && m_prizeInfo.m_nPrizeCash > 0)
            {
                string[] strCont = { "", "번개 ", "무지개 ", "거북 ", "청룡 ", "금룡 " };
                string strMessage = MakeCongration(strCont[m_prizeInfo.m_nCont], m_prizeInfo.m_nPrizeCash);
                CGlobal.SendNoticeBroadCast(strMessage);

                CGlobal.CalculateJackPotCash(m_prizeInfo.m_nPrizeCash);
                OnBroadCastPrizeInfo();
            }

            ClearPrizeInfo();
            CDataBase.SaveGearInfoToDB(this);
        }

        public override void OnGearStart(int nRun)
        {
            m_nGearRun = nRun;
            if (nRun == 1)
            {
                if (m_lstPrizeInfo.Exists(value => value.m_nCmd >= PRIZE_THUNDER && value.m_nCmd <= PRIZE_GDRAGON))
                {
                    CYANScoreInfo scoreInfo = MakeEmptyRoll();
                    scoreInfo.m_nCmd = PRIZE_NIGHT;
                    m_lstPrizeInfo.Insert(0, scoreInfo);
                }
            }
        }

        public override void OnReconnect(CUserSocket clsSocket)
        {
            if (m_lstPrizeInfo.Exists(value => value.m_nCmd >= PRIZE_THUNDER && value.m_nCmd <= PRIZE_GDRAGON))
            {
                CYANScoreInfo scoreInfo = MakeEmptyRoll();
                scoreInfo.m_nCmd = PRIZE_NIGHT;
                m_lstPrizeInfo.Insert(0, scoreInfo);
            }

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

            if (m_lstPrizeInfo.Count > 0)
            {
                //잭팟일때이다.
                if (m_bPrizeAni == false)
                {
                    m_clsSendScoreInfo = m_lstPrizeInfo[0];
                    m_lstPrizeInfo.RemoveAt(0);

                    if (m_lstPrizeInfo.Count == 0)
                    {
                        ReturnGiftCash();
                        if (m_lstReturnGift.Count == 0)
                        {
                            OnEndPrizeCall();
                        }
                    }

                    if (m_clsSendScoreInfo.m_nCmd == PRIZE_THUNDER || m_clsSendScoreInfo.m_nCmd == PRIZE_RAINBOW || 
                        m_clsSendScoreInfo.m_nCmd == PRIZE_BDRAGON || m_clsSendScoreInfo.m_nCmd == PRIZE_GDRAGON)
                    {
                        m_bPrizeAni = true;
                    }
                }
                else
                {
                    m_nPrizeAniCnt++;
                    if (m_nPrizeAniCnt > 50)
                    {
                        m_nPrizeAniCnt = 0;
                        m_bPrizeAni = false;
                    }

                    m_clsSendScoreInfo = MakeEmptyRoll();
                }
            }
            else if (m_lstReturnGift.Count > 0)
            {
                m_clsSendScoreInfo = m_lstReturnGift[0];
                m_lstReturnGift.RemoveAt(0);
                if (m_lstReturnGift.Count == 0)
                {
                    OnEndPrizeCall();
                }
            }
            else if (m_lstScoreInfo.Count > 0)
            {
                m_clsSendScoreInfo = m_lstScoreInfo[0];
                m_lstScoreInfo.RemoveAt(0);
            }
            else
            {
                int nRollCount = m_nSlotCash / m_nSpeedCash;
                for (int i = 0; i < nRollCount; i++)
                {
                    CYANScoreInfo scoreInfo = MakeEmptyRoll();
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
                }
            }
            m_clsSendScoreInfo.m_nGearCode = m_nGearCode;

            CScorePacket scorePacket = new CScorePacket();
            scorePacket.nGearCode = m_nGearCode;
            scorePacket.strPktData = JsonConvert.SerializeObject(m_clsSendScoreInfo);
            clsSocket.SendScoreInfo(scorePacket);

            OnBroadCastPrizeKind();
        }

        private void ReturnGiftCash()
        {
            m_lstReturnGift.Clear();

            if (m_nReturnCash == 0)
                return;

            List<CYANScoreInfo> lstYanScoreInfo = new List<CYANScoreInfo>();
            int nDstCash = m_nReturnCash;
            m_nReturnCash = 0;
            //점수렬을 만든다.
            List<int> lstScore = new List<int>();
            MakeReturnScoreList(nDstCash, lstScore);

            List<CYANScoreInfo> lstScoreInfo = new List<CYANScoreInfo>();
            for (int i = 0; i < lstScore.Count; i++)
            {
                int nDstScore = lstScore[i]; //목표점수
                CYANScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                lstScoreInfo.Add(scoreInfo);
            }

            while (lstScoreInfo.Count > 0)
            {
                CYANScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                lstYanScoreInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                if (RND.Next(2) == 0)
                {
                    scoreInfo = MakeEmptyRoll();
                    lstYanScoreInfo.Add(scoreInfo);
                }
            }

            lstYanScoreInfo.Add(MakeEmptyRoll());

            m_bReturnGift = true;
            m_lstReturnGift.Clear();
            m_lstReturnGift.InsertRange(0, lstYanScoreInfo);
        }

        private void MakeReturnScoreList(int nDstCash, List<int> lstScore)
        {
            while (nDstCash >= 100)
            {
                int nMaxScore = m_lstScores.FindAll(value => value <= nDstCash && value <= 20000).Max(value => value);
                List<int> lstYanScore = m_lstScores.FindAll(value => value == nMaxScore);
                int nScore = CGlobal.RandomSelect(lstYanScore);
                lstScore.Add(nScore);

                nDstCash -= nScore;
            }
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 1000)
                return nJackCash;

            m_lstPrizeInfo.Clear();
            List<CYANScoreTable> lstScoreTable = m_lstScoreTable.ToList().FindAll(value => value.m_nScore <= nJackCash && value.m_nScore <= 10000).OrderByDescending(value => value.m_nScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nPrizeCash = lstScoreTable[0].m_nScore;

                if (RND.Next(5) == 3)
                {
                    int nCont = 0;

                    if (nPrizeCash <= 5000)
                        nCont = JACK_THUNDER;
                    else if (nPrizeCash <= 10000)
                        nCont = JACK_RAINBOW;

                    SetYanPrizeCall(nCont, nPrizeCash);
                }
                else
                {
                    CYANScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(CGlobal.RandomSelect(lstScoreTable));
                    m_lstPrizeInfo.Add(scoreInfo);
                    MakePrizeEmpty(1);
                }

                nJackCash -= nPrizeCash;
                CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 1);
                SetGiveStep(1);

                m_nGearJack = 2;
            }

            return nJackCash;
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 20000)
                return nJackCash;

            CYANPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            SetYanPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            CDataBase.InsertNationalToDB(m_nGearCode, prizeCashInfo.m_nCash, 2);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, 2, 0);
            SetGiveStep(2);

            return nJackCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 20000)
                return nJackCash;

            CYANPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            SetYanPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            CDataBase.InsertNationalToDB(m_nGearCode, prizeCashInfo.m_nCash, 3);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, 3, 0);
            SetGiveStep(3);

            return nJackCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 20000)
                return nJackCash;

            CYANPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            bool bret = SetYanPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            if (bret)
            {
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                if (robot != null)
                {
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, -1, 1);
                    robot.PlayYanPrize();
                }
            }

            return nJackCash;
        }

        public CYANPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public List<CYANScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstPrizeInfo;
        }

        public void OnFinishAnimation()
        {
            m_bPrizeAni = false;
            m_nPrizeAniCnt = 0;
        }

        public bool SetYanPrizeCall(int nJackCont, int nJackAmount)
        {
            if (m_nGearJack > 0)
                return false;
            m_prizeInfo = new CYANPrizeInfo();
            m_prizeInfo.m_nCont = nJackCont;
            m_prizeInfo.m_nPrizeCash = nJackAmount;

            m_lstPrizeInfo.Clear();
            bool ret = MakePrizeRoll();
            return ret;
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nJackCont = 0;
            if (clsItem.m_nItemModel == CItemModel.ITEM_YAN_TURTLE)
            {
                nJackCont = JACK_TURTLE;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_YAN_BDRAGON)
            {
                nJackCont = JACK_BDRAGON;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_YAN_GDRAGON)
            {
                nJackCont = JACK_GDRAGON;
            }


            List<CYANPrizeCashInfo> lstPrizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nPrize == nJackCont && value.m_nCash <= nItemCash);
            if (lstPrizeCashInfo == null || lstPrizeCashInfo.Count == 0)
            {
                //가짜예시이다.
                int nPrizeCash = m_lstScoreTable.ToList().FindAll(value => value.m_nScore <= nItemCash && value.m_nScore <= 10000).Max(value => value.m_nScore);
                if (nPrizeCash <= 5000)
                    nJackCont = JACK_THUNDER;
                else if (nPrizeCash <= 10000)
                    nJackCont = JACK_RAINBOW;

                SetYanPrizeCall(nJackCont, nPrizeCash);
                m_nReturnCash = nItemCash - nPrizeCash;
            }
            else
            {
                //점수에 해당한 잭팟을 주어야 한다.
                int nJackScore = lstPrizeCashInfo.Max(value => value.m_nCash);
                SetYanPrizeCall(nJackCont, nJackScore);
                CGlobal.SetItemEngineRemCash(nItemCash - nJackScore);
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nJackScore, nJackCont, 4, 0);
            }
        }
    }
}
