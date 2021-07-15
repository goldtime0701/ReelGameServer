using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CSeaGear : CBaseGear
    {
        private List<CSEAScoreInfo> m_lstSeaScoreInfo;          //빈돌기까지 포함한 완전한 돌기(한번돌때마다 서버에서 돌기정보를 내려보내야 한다.)
        private CSEAPrizeInfo m_prizeInfo;                      //잭팟정보
        private List<CSEAScoreInfo> m_lstSeaPrizeInfo;          //잭팟정보
        private CSEAScoreInfo m_clsSendScoreInfo;
        private List<CSEAScoreInfo> m_lstSeaReturnGift;
        private int m_nReturnCash;                              //잭팟이 끝난다음에 돌려주어야 하는 점수
        private bool m_bReturnGift;

        private int m_nNoiseCash;                               //가짜잭팟을 자동으로 주기 위한 금액
        private int m_nNoiseRaise;                              //가짜잭팟발동금액


        public CSeaGear() : base()
        {
            m_nSpeedCash = 100;
            m_lstSeaScoreInfo = new List<CSEAScoreInfo>();
            m_lstSeaPrizeInfo = new List<CSEAScoreInfo>();
            m_lstSeaReturnGift = new List<CSEAScoreInfo>();

            m_nNoiseRaise = RND.Next(800, 2000);
            m_nNoiseRaise *= 10;
        }

        public CSEAPrizeInfo GetSEAJackpotInfo()
        {
            return m_prizeInfo;
        }

        public override void ClearPrizeInfo()
        {
            m_bReturnGift = false;
            m_nGearJack = 0;
            SetGiveStep(0);

            m_prizeInfo = null;
            m_lstSeaPrizeInfo.Clear();
            m_lstSeaReturnGift.Clear();
        }

        public override void ClearGear()
        {
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            m_nReturnCash = 0;
            m_bReturnGift = false;
            SetGiveStep(0);
            m_lstSeaPrizeInfo.Clear();
            m_lstSeaScoreInfo.Clear();
            m_lstSeaReturnGift.Clear();
            m_prizeInfo = null;
        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<CSEAScoreInfo> lstSeaScoreInfo = new List<CSEAScoreInfo>();
            int nTempCash = nAddCash;

            while (nTempCash > 0)
            {
                int nAppenCash = nTempCash >= 10000 ? 10000 : nTempCash;
                nTempCash -= nAppenCash;
                int nDstCash = (int)(nAppenCash * rate / 100);

                //점수렬을 만든다.
                List<int> lstScore = new List<int>();
                MakeNormalScoreList(nDstCash, lstScore);

                List<CSEAScoreInfo> lstScoreInfo = new List<CSEAScoreInfo>();
                //점수에 해당한 SeaScoreTableInfo리스트를 구한다.
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CSEAScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CSEAScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstSeaScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstSeaScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CSEAScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstSeaScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstSeaScoreInfo.Count);
                    lstSeaScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                m_lstSeaScoreInfo.Clear();
            }

            m_lstSeaScoreInfo.AddRange(lstSeaScoreInfo);
        }

        private CSEAScoreInfo MakeTestTile()
        {
            CSEAScoreInfo scoreInfo = new CSEAScoreInfo();

            for (int nCol = 0; nCol < 4; nCol++)
            {
                for (int nRow = 0; nRow < 3; nRow++)
                {
                    if (nRow == 0)
                        scoreInfo.m_lstTile.Add(new CTile(6, nRow, nCol));
                    else if (nRow == 1)
                        scoreInfo.m_lstTile.Add(new CTile(7, nRow, nCol));
                    else if (nRow == 2)
                        scoreInfo.m_lstTile.Add(new CTile(8, nRow, nCol));
                }
            }

            return scoreInfo;
        }

        //점수를 여러개의 작은 점수들로 나눈다.
        private void MakeNormalScoreList(int nDstCash, List<int> lstScore)
        {
            int nDelta = nDstCash / 2;
            if (nDelta < 100)
                nDelta = 100;
            while (nDstCash >= 100)
            {
                List<int> lstSeaScore = m_lstSeaScores.FindAll(value => value <= nDelta);
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
        private CSEAScoreInfo MakeNormalScore(int nScore)
        {
            CSEAScoreInfo scoreInfo = null;

            if (nScore == 0)
                return scoreInfo;

            //점수테이블리스트를 만든다.
            List<SEAScoreTableInfo> lstMatchScoreInfo = m_lstSEAScoreTable.FindAll(value => value.nScore == nScore);
            SEAScoreTableInfo scoreTable = CGlobal.RandomSelect(lstMatchScoreInfo);
            scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);

            return scoreInfo;
        }

        //점수표를 점수정보로 변환하기
        private CSEAScoreInfo ConvertScoreTableToScoreInfo(SEAScoreTableInfo tableInfo, bool bPrize = false)
        {
            CSEAScoreInfo scoreInfo = new CSEAScoreInfo();

            //점수표에 있는 타일들을 배치한다.
            for (int nRow = 0; nRow < 3; nRow++)
            {
                for (int nCol = 0; nCol < 4; nCol++)
                {
                    CTile clsTile = new CTile(tableInfo.lstTile[nRow][nCol], nRow, nCol);
                    scoreInfo.m_lstTile.Add(clsTile);
                }
            }
            //당첨된 타일을 표시한다.
            for (int i = 0; i < tableInfo.lstWin.Length; i++)
            {
                int nRow = tableInfo.lstWin[i] / 4;
                int nCol = tableInfo.lstWin[i] % 4;
                scoreInfo.m_lstTile.Find(value => value.m_nRow == nRow && value.m_nCol == nCol).m_nAct = 1;
            }
            //당첨된 행을 표시한다.
            scoreInfo.m_lstLine = tableInfo.lstLine.ToList();
            scoreInfo.m_nScore = tableInfo.nScore;
            scoreInfo.m_nTile = tableInfo.nTile;
            scoreInfo.m_nWinTile = tableInfo.nTile;
            scoreInfo.m_nWinCnt = tableInfo.nCount;
            scoreInfo.m_nMulti = tableInfo.nMulti;
            scoreInfo.m_nFlag = 1;
            scoreInfo.m_nGearCode = m_nGearCode;

            //조커타일을 배치하겠는가를 결정한다.
            if (scoreInfo.m_lstLine.Count(value => value == 1) == 1 && scoreInfo.m_nWinCnt > 2 && scoreInfo.m_nWinTile != JOKE)
            {
                int nRow = ANNY;
                if (scoreInfo.m_lstLine[0] == 1)
                    nRow = 0;
                else if (scoreInfo.m_lstLine[1] == 1)
                    nRow = 1;
                else if (scoreInfo.m_lstLine[2] == 1)
                    nRow = 2;
                //복합점수가 아니고 단일점수일때만 조커를 배치한다.
                if (RND.Next(30) == 17)
                {
                    CTile tile = CGlobal.RandomSelect(scoreInfo.m_lstTile.FindAll(value => value.m_nRow == nRow && value.m_nAct == 1));
                    if (tile != null)
                    {
                        tile.m_nTile = JOKE;
                        List<int> lstOriginalTile = m_lstOriginalTile[tile.m_nCol];
                        if (tile.m_nRow == BOT)
                        {
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == tile.m_nRow + 1 && value.m_nCol == tile.m_nCol).m_nTile = lstOriginalTile[0];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == tile.m_nRow + 2 && value.m_nCol == tile.m_nCol).m_nTile = lstOriginalTile[1];
                        }
                        else if (tile.m_nRow == MID)
                        {
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == tile.m_nRow - 1 && value.m_nCol == tile.m_nCol).m_nTile = lstOriginalTile[lstOriginalTile.Count - 2];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == tile.m_nRow + 1 && value.m_nCol == tile.m_nCol).m_nTile = lstOriginalTile[0];
                        }
                        else if (tile.m_nRow == TOP)
                        {
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == tile.m_nRow - 1 && value.m_nCol == tile.m_nCol).m_nTile = lstOriginalTile[lstOriginalTile.Count - 2];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == tile.m_nRow - 2 && value.m_nCol == tile.m_nCol).m_nTile = lstOriginalTile[lstOriginalTile.Count - 3];
                        }
                    }
                }
            }


            if (tableInfo.nCount == 4 && (tableInfo.nTile == BAR3 || tableInfo.nTile == SEVN || tableInfo.nTile == STAR || tableInfo.nTile == TAGT) && tableInfo.lstTile[MID][0] == tableInfo.nTile)
            {
                //잠수함을 출현시키겠는가를 결정한다.
                if (bPrize)
                {
                    scoreInfo.m_nFlag = 3;

                    int nRnd = RND.Next(2);
                    if (nRnd == 0)
                    {
                        //왼쪽타일 맞추기
                        while (true)
                        {
                            int nIndex = RND.Next(1, m_lstOriginalTile[0].Count - 2);
                            int nTile = m_lstOriginalTile[0][nIndex];
                            if (nTile == tableInfo.nTile || nTile == JOKE || nTile == MENG)
                                continue;
                            if (m_lstOriginalTile[0][nIndex - 1] == MENG || m_lstOriginalTile[0][nIndex + 1] == MENG)
                                continue;

                            CTile tile = scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 0);
                            tile.m_nTile = nTile;
                            tile.m_nAct = 0;

                            scoreInfo.m_lstTile.Find(value => value.m_nRow == TOP && value.m_nCol == 0).m_nTile = m_lstOriginalTile[0][nIndex + 1];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == BOT && value.m_nCol == 0).m_nTile = m_lstOriginalTile[0][nIndex - 1];

                            break;
                        }
                    }
                    else if (nRnd == 1)
                    {
                        //오른쪽타일 맞추기
                        while (true)
                        {
                            int nIndex = RND.Next(1, m_lstOriginalTile[3].Count - 2);
                            int nTile = m_lstOriginalTile[3][nIndex];
                            if (nTile == tableInfo.nTile || nTile == JOKE || nTile == MENG)
                                continue;
                            if (m_lstOriginalTile[3][nIndex - 1] == MENG || m_lstOriginalTile[3][nIndex + 1] == MENG)
                                continue;

                            CTile tile = scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 3);
                            tile.m_nTile = nTile;
                            tile.m_nAct = 0;

                            scoreInfo.m_lstTile.Find(value => value.m_nRow == TOP && value.m_nCol == 3).m_nTile = m_lstOriginalTile[3][nIndex + 1];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == BOT && value.m_nCol == 3).m_nTile = m_lstOriginalTile[3][nIndex - 1];

                            break;
                        }
                    }
                }
            }
            else if (tableInfo.nCount == 3 && (tableInfo.nTile == BAR3 || tableInfo.nTile == SEVN || tableInfo.nTile == STAR || tableInfo.nTile == TAGT) && (tableInfo.lstTile[MID][0] == tableInfo.nTile || tableInfo.lstTile[MID][3] == tableInfo.nTile))
            {
                int nRnd = RND.Next(10);
                if (nRnd < 4)
                {
                    //번개효과주기
                    scoreInfo.m_nFlag = 2;
                    if (tableInfo.lstTile[MID][0] == tableInfo.nTile)
                    {
                        //오른쪽번개
                        while (true)
                        {
                            int nIndex = RND.Next(1, m_lstOriginalTile[2].Count - 2);
                            int nTile = m_lstOriginalTile[2][nIndex];
                            if (nTile == tableInfo.nTile || nTile == JOKE)
                                continue;
                            if (m_lstOriginalTile[2][nIndex - 1] == MENG || m_lstOriginalTile[2][nIndex + 1] == MENG)
                                continue;

                            CTile tile = scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 2);
                            tile.m_nTile = nTile;
                            tile.m_nAct = 0;

                            scoreInfo.m_lstTile.Find(value => value.m_nRow == TOP && value.m_nCol == 2).m_nTile = m_lstOriginalTile[2][nIndex + 1];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == BOT && value.m_nCol == 2).m_nTile = m_lstOriginalTile[2][nIndex - 1];

                            break;
                        }
                    }
                    else if (tableInfo.lstTile[MID][3] == tableInfo.nTile)
                    {
                        //왼쪽번개
                        while (true)
                        {
                            int nIndex = RND.Next(1, m_lstOriginalTile[1].Count - 2);
                            int nTile = m_lstOriginalTile[1][nIndex];
                            if (nTile == tableInfo.nTile || nTile == JOKE)
                                continue;
                            if (m_lstOriginalTile[1][nIndex - 1] == MENG || m_lstOriginalTile[1][nIndex + 1] == MENG)
                                continue;

                            CTile tile = scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 1);
                            tile.m_nTile = nTile;
                            tile.m_nAct = 0;

                            scoreInfo.m_lstTile.Find(value => value.m_nRow == TOP && value.m_nCol == 1).m_nTile = m_lstOriginalTile[1][nIndex + 1];
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == BOT && value.m_nCol == 1).m_nTile = m_lstOriginalTile[1][nIndex - 1];

                            break;
                        }
                    }

                }
            }

            List<CTile> lstAny = scoreInfo.m_lstTile.FindAll(value => value.m_nTile == ANNY);
            if (lstAny != null && lstAny.Count > 0)
            {
                List<int> lstCol = new List<int>();
                for (int i = 0; i < lstAny.Count; i++)
                {
                    if (lstCol.Exists(value => value == lstAny[i].m_nCol))
                        continue;
                    lstCol.Add(lstAny[i].m_nCol);
                }

                for (int i = 0; i < lstCol.Count; i++)
                {
                    int nCol = lstCol[i];
                    while (true)
                    {
                        int nIndex = RND.Next(1, m_lstOriginalTile[nCol].Count - 2);
                        if (m_lstOriginalTile[nCol][nIndex - 1] == MENG || m_lstOriginalTile[nCol][nIndex] == MENG || m_lstOriginalTile[nCol][nIndex + 1] == MENG)
                            continue;
                        scoreInfo.m_lstTile.Find(value => value.m_nRow == BOT && value.m_nCol == nCol).m_nTile = m_lstOriginalTile[nCol][nIndex - 1];
                        scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == nCol).m_nTile = m_lstOriginalTile[nCol][nIndex];
                        scoreInfo.m_lstTile.Find(value => value.m_nRow == TOP && value.m_nCol == nCol).m_nTile = m_lstOriginalTile[nCol][nIndex + 1];

                        if (scoreInfo.m_lstLine[0] == 1 && m_lstOriginalTile[nCol][nIndex - 1] == scoreInfo.m_nWinTile)
                            continue;
                        if (scoreInfo.m_lstLine[1] == 1 && m_lstOriginalTile[nCol][nIndex - 0] == scoreInfo.m_nWinTile)
                            continue;
                        if (scoreInfo.m_lstLine[2] == 1 && m_lstOriginalTile[nCol][nIndex + 1] == scoreInfo.m_nWinTile)
                            continue;

                        
                        break;
                    }
                }
            }

            //멍이 1일때 멍이 아닌 타일이 3개 맞는것을 방지하는 부분이다.
            if(scoreInfo.m_nWinCnt == 1 && scoreInfo.m_nWinTile == MENG)
            {
                while(true)
                {
                    bool bFind = false;
                    for(int nRow=0; nRow<3; nRow++)
                    {
                        List<CTile> lstTile = scoreInfo.m_lstTile.FindAll(value=>value.m_nRow == nRow);
                        if ((lstTile[0].m_nTile == lstTile[1].m_nTile && lstTile[1].m_nTile == lstTile[2].m_nTile) || (lstTile[1].m_nTile == lstTile[2].m_nTile && lstTile[2].m_nTile == lstTile[3].m_nTile))
                        {
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind == false)
                        break;

                    while (true)
                    {
                        int nIndex = RND.Next(1, m_lstOriginalTile[2].Count - 2);
                        if (m_lstOriginalTile[2][nIndex - 1] == MENG || m_lstOriginalTile[2][nIndex] == MENG || m_lstOriginalTile[2][nIndex + 1] == MENG)
                            continue;
                        scoreInfo.m_lstTile.Find(value => value.m_nRow == BOT && value.m_nCol == 2).m_nTile = m_lstOriginalTile[2][nIndex - 1];
                        scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 2).m_nTile = m_lstOriginalTile[2][nIndex];
                        scoreInfo.m_lstTile.Find(value => value.m_nRow == TOP && value.m_nCol == 2).m_nTile = m_lstOriginalTile[2][nIndex + 1];

                        if (scoreInfo.m_lstLine[0] == 1 && m_lstOriginalTile[2][nIndex - 1] == scoreInfo.m_nWinTile)
                            continue;
                        if (scoreInfo.m_lstLine[1] == 1 && m_lstOriginalTile[2][nIndex - 0] == scoreInfo.m_nWinTile)
                            continue;
                        if (scoreInfo.m_lstLine[2] == 1 && m_lstOriginalTile[2][nIndex + 1] == scoreInfo.m_nWinTile)
                            continue;

                        break;
                    }
                }
            }


            return scoreInfo;
        }

        //빈돌기 만드는 함수(점수가 있을때는 나머지 타일들을 채워넣는 기능을 수행한다.)
        private CSEAScoreInfo MakeEmptyRoll(CSEAScoreInfo scoreInfo = null)
        {
            if (scoreInfo == null)
            {
                //빈돌기일때이다.
                scoreInfo = new CSEAScoreInfo();

                bool bFlag = true;
                int nPos = 0;
                List<CTile> lstTempTile = new List<CTile>();

                //첫번째렬 만들기
                while (bFlag)
                {
                    lstTempTile.Clear();
                    nPos = CGlobal.Random(0, 18);
                    bFlag = false;
                    for (int i = 0; i < 3; i++)
                    {
                        int nTile = m_lstOriginalTile[0][nPos];
                        if (nTile == MENG || nTile == JOKE)
                        {
                            bFlag = true;
                            break;
                        }
                        CTile tile = new CTile(nTile, i, 0);
                        lstTempTile.Add(tile);
                        nPos++;
                        if (nPos > 17) nPos = 0;
                    }
                }

                for (int i = 0; i < lstTempTile.Count; i++)
                    scoreInfo.m_lstTile.Add(lstTempTile[i]);


                //두번째렬만들기
                lstTempTile.Clear();
                nPos = CGlobal.Random(0, 18);
                for (int i = 0; i < 3; i++)
                {
                    int nTile = m_lstOriginalTile[1][nPos];
                    CTile tile = new CTile(nTile, i, 1);
                    lstTempTile.Add(tile);
                    nPos++;
                    if (nPos > 17) nPos = 0;
                }
                for (int i = 0; i < lstTempTile.Count; i++)
                    scoreInfo.m_lstTile.Add(lstTempTile[i]);

                //세번째렬만들기
                bFlag = true;
                while (bFlag)
                {
                    lstTempTile.Clear();
                    nPos = RND.Next(0, 18);
                    bFlag = false;
                    for (int i = 0; i < 3; i++)
                    {
                        int nTile = m_lstOriginalTile[2][nPos];
                        if(scoreInfo.m_lstTile.Find(value=>value.m_nRow == i && value.m_nCol == 0).m_nTile == scoreInfo.m_lstTile.Find(value=>value.m_nRow == i && value.m_nCol == 1).m_nTile)
                        {
                            if (scoreInfo.m_lstTile.Exists(value => value.m_nRow == i && value.m_nCol == 0 && value.m_nTile == nTile))
                            {
                                bFlag = true;
                                break;
                            }
                        }
                        
                        if (nTile == JOKE)
                        {
                            if (scoreInfo.m_lstTile.Find(value => value.m_nRow == i && value.m_nCol == 0).m_nTile == scoreInfo.m_lstTile.Find(value => value.m_nRow == i && value.m_nCol == 1).m_nTile)
                            {
                                bFlag = true;
                                break;
                            }
                            if (scoreInfo.m_lstTile.Find(value => value.m_nRow == i && value.m_nCol == 1).m_nTile == JOKE)
                            {
                                bFlag = true;
                                break;
                            }
                        }

                        CTile tile = new CTile(nTile, i, 2);
                        lstTempTile.Add(tile);
                        nPos++;
                        if (nPos > 17) nPos = 0;
                    }
                }
                for (int i = 0; i < lstTempTile.Count; i++)
                    scoreInfo.m_lstTile.Add(lstTempTile[i]);


                //네번째렬만들기
                bFlag = true;
                while (bFlag)
                {
                    lstTempTile.Clear();
                    nPos = CGlobal.Random(0, 18);
                    bFlag = false;
                    for (int i = 0; i < 3; i++)
                    {
                        int nTile = m_lstOriginalTile[3][nPos];

                        if (nTile == MENG || nTile == JOKE)
                        {
                            bFlag = true;
                            break;
                        }

                        if(scoreInfo.m_lstTile.Find(value=>value.m_nRow == i && value.m_nCol == 1).m_nTile == scoreInfo.m_lstTile.Find(value=>value.m_nRow == i && value.m_nCol == 2).m_nTile)
                        {
                            if (scoreInfo.m_lstTile.Exists(value => value.m_nRow == i && value.m_nCol == 1 && value.m_nTile == nTile))
                            {
                                bFlag = true;
                                break;
                            }
                        }

                        if (scoreInfo.m_lstTile.Find(value => value.m_nRow == i && value.m_nCol == 1).m_nTile == JOKE)
                        {
                            //두번째렬이 조커라면 세번째렬과 달라야 한다.
                            if (scoreInfo.m_lstTile.Exists(value => value.m_nRow == i && value.m_nCol == 2 && value.m_nTile == nTile))
                            {
                                bFlag = true;
                                break;
                            }
                        }


                        CTile tile = new CTile(nTile, i, 3);
                        lstTempTile.Add(tile);
                        nPos++;
                        if (nPos > 17) nPos = 0;
                    }
                }
                for (int i = 0; i < lstTempTile.Count; i++)
                    scoreInfo.m_lstTile.Add(lstTempTile[i]);

            }
            else
            {
                //점수돌기일때이다.
                bool bSocre = scoreInfo.m_nScore > 0 ? true : false;
                List<CTile> lstTile = scoreInfo.m_lstTile;

                for (int nCol = 0; nCol < 4; nCol++)
                {
                    List<int> lstTileKind = m_lstTile;
                    if (bSocre)
                        lstTileKind = lstTileKind.FindAll(value => value != 0 && value != 7);

                    if (nCol == 0 || nCol == 3)
                        lstTileKind = lstTileKind.FindAll(value => value != 0 && value != 7);


                    for (int nRow = 0; nRow < 3; nRow++)
                    {
                        if (lstTile.Exists(value => value.m_nCol == nCol && value.m_nRow == nRow))
                            continue;

                        while (true)
                        {
                            int nTile = CGlobal.RandomSelect(lstTileKind);
                            if (lstTile.Exists(value => value.m_nCol == nCol && value.m_nTile == nTile))
                                continue;
                            if (nCol > 0)
                            {
                                if (lstTile.Exists(value => value.m_nCol == nCol - 1 && value.m_nRow == nRow && value.m_nTile == nTile))
                                    continue;
                            }
                            else if (nCol == 0)
                            {
                                if (lstTile.Exists(value => value.m_nTile == nTile && value.m_nRow == nRow))
                                    continue;
                            }


                            CTile tile = new CTile(nTile, nRow, nCol);
                            lstTile.Add(tile);
                            break;
                        }

                    }
                }
            }
            scoreInfo.m_nGearCode = m_nGearCode;

            return scoreInfo;
        }

        public bool SetSEAPrizeCall(int nCont, int nPrizeCash)
        {
            if (m_nGearJack > 0)
                return false;
            m_prizeInfo = new CSEAPrizeInfo();
            m_prizeInfo.m_nCont = nCont;
            m_prizeInfo.m_nPrizeCash = nPrizeCash;

            m_lstSeaPrizeInfo.Clear();
            bool ret = MakePrizeRoll();
            return ret;
        }

        public override bool MakePrizeRoll()
        {
            m_nGearJack = 1;    //잭팟상태로 설정
            CDataBase.SaveGearInfoToDB(this);


            //잭팟시작신호를 보낸다.
            MakePrizeCommand(PRIZE_START);
            //밤을 만든다
            MakePrizeCommand(PRIZE_NIGHT);
            //빈돌기 5회정도 준다
            MakePrizeEmpty(3);

            if(m_prizeInfo.m_nCont == JACK_NISE2)
            {
                //빈돌기 10회정도 준다
                MakePrizeEmpty(10);
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }


            //거부기 출현
            MakePrizeCommand(PRIZE_TURTLE);
            //빈돌기 10회정도 준다
            MakePrizeEmpty(10);
            //거부기출현정지
            MakePrizeCommand(PRIZE_GOD_DISAPPEAR);
            if (m_prizeInfo.m_nCont == JACK_NISE)
            {
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }

            //해파리로 넘어가기전에 주는 예시동작
            MakeNextPrize();
            //해파리출현
            MakePrizeCommand(PRIZE_JELLY);
            //빈돌기 10회정도 준다
            MakePrizeEmpty(10);
            //해파리출현정지
            MakePrizeCommand(PRIZE_GOD_DISAPPEAR);

            if (m_prizeInfo.m_nCont == JACK_NISE1)
            {
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            if (m_prizeInfo.m_nCont == JACK_JELLY)
            {
                //빈돌기 5회정도 준다
                MakePrizeEmpty(5);
                PrizeJellyRoll();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(1);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakeNextPrize();
            //상어출현
            MakePrizeCommand(PRIZE_SHARK);
            //빈돌기 10회정도 준다
            MakePrizeEmpty(10);
            //상어출현정지
            MakePrizeCommand(PRIZE_GOD_DISAPPEAR);
            if (m_prizeInfo.m_nCont == JACK_SHARK)
            {
                //빈돌기 5회정도 준다
                MakePrizeEmpty(7);
                PrizeSharkRoll();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(1);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakeNextPrize();
            //고래출현
            MakePrizeCommand(PRIZE_WHALE);
            //빈돌기 10회정도 준다
            MakePrizeEmpty(15);
            //고래출현정지
            MakePrizeCommand(PRIZE_GOD_DISAPPEAR);
            //빈돌기 10회정도 준다
            MakePrizeEmpty(12);
            if (m_prizeInfo.m_nCont == JACK_WHALE)
            {
                PrizeWhaleRoll();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(1);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }

            MakePrizeCommand(PRIZE_END);
            MakePrizeEmpty(1);

            CDataBase.SaveGearInfoToDB(this);
            return true;
        }

        private void MakePrizeCommand(int nCmd)
        {
            CSEAScoreInfo scoreInfo = MakeEmptyRoll();
            scoreInfo.m_nCmd = nCmd;
            m_lstSeaPrizeInfo.Add(scoreInfo);
        }

        private void MakePrizeEmpty(int nCount)
        {
            nCount = nCount + RND.Next(3);
            for (int i = 0; i < nCount; i++)
            {
                m_lstSeaPrizeInfo.Add(MakeEmptyRoll());
            }
        }

        private void MakeNextPrize()
        {
            int nCnt = 2;
            for (int i = 0; i < nCnt; i++)
            {
                CSEAScoreInfo scoreInfo = new CSEAScoreInfo();
                scoreInfo.m_lstTile.Add(new CTile(RND.Next(5, 11), 1, 0));
                scoreInfo.m_lstTile.Add(new CTile(BAR3, 1, 1));
                scoreInfo.m_lstTile.Add(new CTile(SEVN, 1, 2));
                scoreInfo.m_lstTile.Add(new CTile(STAR, 1, 3));
                MakeEmptyRoll(scoreInfo);

                MakePrizeEmpty(2);
            }

        }


        private void PrizeJellyRoll()
        {
            //해파리잭팟은 25000, 50000
            SEAScoreTableInfo tableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == m_prizeInfo.m_nPrizeCash));
            CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo, true);
            scoreInfo.m_nScore = 20000;
            m_lstSeaPrizeInfo.Add(scoreInfo);
            int nRememberCash = m_prizeInfo.m_nPrizeCash - 20000;
            MakeRememberPrizeScore(nRememberCash);
            MakePrizeEndRoll();
        }

        private void PrizeSharkRoll()
        {
            //상어점수는 10만, 12만 5천, 15만, 20만, 25만, 30만, 40만
            List<SEAScoreTableInfo> lstTableInfo = new List<SEAScoreTableInfo>();
            if (m_prizeInfo.m_nPrizeCash == 10 * 10000)
            {
                int nRnd = RND.Next(3);
                if (nRnd == 0)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 25000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
                else if (nRnd == 1)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 50000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
                else if (nRnd == 2)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 100000));
                    lstTableInfo.Add(scoreTableInfo);
                }
            }
            else if (m_prizeInfo.m_nPrizeCash == 12.5 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 25000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 15 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 50000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 20 * 10000)
            {
                int nRnd = RND.Next(2);
                if (nRnd == 0)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 50000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
                else if (nRnd == 1)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 100000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
            }
            else if (m_prizeInfo.m_nPrizeCash == 25 * 10000)
            {
                if (RND.Next(2) == 0)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 50000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
                else
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 250000));
                    lstTableInfo.Add(scoreTableInfo);
                }

            }
            else if (m_prizeInfo.m_nPrizeCash == 30 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 100000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 40 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 100000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }

            for (int i = 0; i < lstTableInfo.Count; i++)
            {
                SEAScoreTableInfo tableInfo = lstTableInfo[i];
                CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo, true);
                scoreInfo.m_nScore = 20000;
                scoreInfo.m_nMulti = lstTableInfo.Count - i;
                m_lstSeaPrizeInfo.Add(scoreInfo);
                MakeRememberPrizeScore(tableInfo.nScore - 20000);

                if (i < lstTableInfo.Count - 1)
                    PrizeContinue();
            }

            MakePrizeEndRoll();
        }

        private void PrizeWhaleRoll()
        {
            //고래는 50만, 75만, 100만 125만, 150만, 200만, 250만
            List<SEAScoreTableInfo> lstTableInfo = new List<SEAScoreTableInfo>();

            if (m_prizeInfo.m_nPrizeCash == 50 * 10000)
            {
                if (RND.Next(2) == 0)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 250000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
                else
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 500000));
                    lstTableInfo.Add(scoreTableInfo);
                }
            }
            else if (m_prizeInfo.m_nPrizeCash == 75 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 250000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 100 * 10000)
            {
                if (RND.Next(2) == 0)
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 250000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
                else
                {
                    SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 500000));
                    lstTableInfo.Add(scoreTableInfo);
                    lstTableInfo.Add(scoreTableInfo);
                }
            }
            else if (m_prizeInfo.m_nPrizeCash == 125 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 250000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 150 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 500000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 200 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 500000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }
            else if (m_prizeInfo.m_nPrizeCash == 250 * 10000)
            {
                SEAScoreTableInfo scoreTableInfo = CGlobal.RandomSelect(m_lstSEAScoreTable.FindAll(value => value.nScore == 500000));
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
                lstTableInfo.Add(scoreTableInfo);
            }

            for (int i = 0; i < lstTableInfo.Count; i++)
            {
                SEAScoreTableInfo tableInfo = lstTableInfo[i];
                CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo, true);
                scoreInfo.m_nScore = 20000;
                scoreInfo.m_nMulti = lstTableInfo.Count - i;
                m_lstSeaPrizeInfo.Add(scoreInfo);
                MakeRememberPrizeScore(tableInfo.nScore - 20000);

                if (i < lstTableInfo.Count - 1)
                    PrizeContinue();
            }

            MakePrizeEndRoll();
        }



        private void PrizeContinue()
        {
            //련타가 있을때이다.
            CSEAScoreInfo scoreInfo = new CSEAScoreInfo();
            scoreInfo.m_nScore = 100;
            scoreInfo.m_nFlag = 1;
            CTile tile = new CTile(MENG, 0, 0);
            tile.m_nAct = 1;
            scoreInfo.m_lstTile.Add(tile);
            tile = new CTile(CLAM, 1, 0);
            scoreInfo.m_lstTile.Add(tile);
            tile = new CTile(CLAM, 1, 1);
            scoreInfo.m_lstTile.Add(tile);
            tile = new CTile(TAGT, 2, 0);
            scoreInfo.m_lstTile.Add(tile);
            tile = new CTile(TAGT, 2, 1);
            scoreInfo.m_lstTile.Add(tile);
            MakeEmptyRoll(scoreInfo);
            m_nReturnCash -= 100;
            m_lstSeaPrizeInfo.Add(scoreInfo);
        }

        private void MakePrizeEndRoll()
        {
            MakePrizeEmpty(2);
            //잭팟끝점수를 준다.
            List<SEAScoreTableInfo> lstScoreTable = m_lstSEAScoreTable.FindAll(value => value.nTile == MENG && value.nCount == 1 && value.nMulti == 1);
            SEAScoreTableInfo tableInfo = CGlobal.RandomSelect(lstScoreTable);
            CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
            MakeEmptyRoll(scoreInfo);
            m_lstSeaPrizeInfo.Add(scoreInfo);
        }

        private void MakeRememberPrizeScore(int nPrizeCash)
        {
            List<CSEAScoreInfo> lstScoreInfo = new List<CSEAScoreInfo>();
            while (nPrizeCash >= 20000)
            {
                List<SEAScoreTableInfo> lstScoreTable = m_lstSEAScoreTable.FindAll(value => value.nScore <= nPrizeCash && value.nScore == 20000 && value.nMulti == 1);
                SEAScoreTableInfo tableInfo = CGlobal.RandomSelect(lstScoreTable);

                CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
                nPrizeCash -= scoreInfo.m_nScore;

                lstScoreInfo.Add(scoreInfo);
            }

            while (nPrizeCash >= 10000)
            {
                List<SEAScoreTableInfo> lstScoreTable = m_lstSEAScoreTable.FindAll(value => value.nScore <= nPrizeCash && value.nScore >= 10000 && value.nScore <= 20000 && value.nMulti == 1);
                SEAScoreTableInfo tableInfo = CGlobal.RandomSelect(lstScoreTable);

                CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
                nPrizeCash -= scoreInfo.m_nScore;

                lstScoreInfo.Add(scoreInfo);
            }

            while (nPrizeCash >= 150)
            {
                List<SEAScoreTableInfo> lstScoreTable = m_lstSEAScoreTable.FindAll(value => value.nScore <= nPrizeCash && value.nMulti == 1).OrderByDescending(value => value.nScore).ToList();
                SEAScoreTableInfo tableInfo = lstScoreTable[0];
                CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
                nPrizeCash -= scoreInfo.m_nScore;

                lstScoreInfo.Add(scoreInfo);
            }


            if (nPrizeCash > 0)
            {
                List<SEAScoreTableInfo> lstScoreTable = m_lstSEAScoreTable.FindAll(value => value.nScore == nPrizeCash && value.nMulti == 1);
                if (lstScoreTable.Count > 0)
                {
                    SEAScoreTableInfo tableInfo = CGlobal.RandomSelect(lstScoreTable);
                    CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(tableInfo);
                    nPrizeCash -= scoreInfo.m_nScore;
                    lstScoreInfo.Add(scoreInfo);
                }
            }
            if (nPrizeCash > 0)
                m_nReturnCash += nPrizeCash;

            MakePrizeCommand(PRIZE_AFTER);
            while (lstScoreInfo.Count > 0)
            {
                CSEAScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                m_lstSeaPrizeInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);

                MakePrizeEmpty(1);
            }
        }

        public CSEAPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 25000)
                return nJackCash;

            int nPrizeCash = 0;
            int nCont = 0;
            if (nJackCash < 5 * 10000)
            {
                nCont = JACK_JELLY;
                nPrizeCash = 25000;
            }
            else if (nJackCash < 10 * 10000)
            {
                nCont = JACK_JELLY;
                nPrizeCash = 50000;
            }
            else if (nJackCash < 125000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 100000;
            }
            else if (nJackCash < 15 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 125000;
            }
            else if (nJackCash < 20 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 150000;
            }
            else if (nJackCash < 25 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 200000;
            }
            else if (nJackCash < 30 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 200000;
            }
            else if (nJackCash < 40 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 300000;
            }
            else if (nJackCash < 50 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 400000;
            }
            else if (nJackCash < 75 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 500000;
            }
            else if (nJackCash < 100 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 750000;
            }
            else if (nJackCash < 125 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 100 * 10000;
            }
            else if (nJackCash < 150 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 125 * 10000;
            }
            else if (nJackCash < 200 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 150 * 10000;
            }
            else if (nJackCash < 250 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 200 * 10000;
            }
            else
            {
                nCont = JACK_WHALE;
                nPrizeCash = 250 * 10000;
            }
            nJackCash -= nPrizeCash;

            bool bret = SetSEAPrizeCall(nCont, nPrizeCash);
            if (bret)
            {
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                if (robot != null)
                {
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, nPrizeCash, nCont, -1, 1);
                    robot.PlaySeaPrize();
                }
            }

            return nJackCash;
        }

        public List<CSEAScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstSeaPrizeInfo;
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            m_lstSeaPrizeInfo.Clear();
            List<SEAScoreTableInfo> lstScoreTable = m_lstSEAScoreTable.ToList().FindAll(value => value.nScore <= nJackCash).OrderByDescending(value => value.nScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nScore = lstScoreTable[0].nScore;
                List<SEAScoreTableInfo> tempTableInfo = lstScoreTable.FindAll(value => value.nScore == nScore);
                CSEAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(CGlobal.RandomSelect(tempTableInfo));
                m_lstSeaPrizeInfo.Add(scoreInfo);

                CDataBase.InsertNationalToDB(m_nGearCode, scoreInfo.m_nScore, 1);
                SetGiveStep(1);
                nJackCash -= scoreInfo.m_nScore;
                m_nGearJack = 2;

                m_lstSeaPrizeInfo.Add(MakeEmptyRoll());
            }

            return nJackCash;
        }

        public override int RaiseJackPot2(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            CGameEngine seaEngine = CGlobal.GetEngine(m_nGameCode);
            if (seaEngine.GetGivePrizeCount() > seaEngine.m_nEmptyPrize)
            {
                //바당이야기에서는 코인을 생성할때 자동으로 가짜예시를 생성해서 나가므로 여기서는 주지 않는다.
                seaEngine.SetGivePrizeCount(0);
            }

            if (nJackCash < 25000)
                return nJackCash;

            int nPrizeCash = 0;
            int nCont = 0;
            if (nJackCash < 5 * 10000)
            {
                nCont = JACK_JELLY;
                nPrizeCash = 25000;
            }
            else if (nJackCash < 10 * 10000)
            {
                nCont = JACK_JELLY;
                nPrizeCash = 50000;
            }
            else if (nJackCash < 125000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 100000;
            }
            else if (nJackCash < 15 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 125000;
            }
            else if (nJackCash < 20 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 150000;
            }
            else if (nJackCash < 25 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 200000;
            }
            else if (nJackCash < 30 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 200000;
            }
            else if (nJackCash < 40 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 300000;
            }
            else if (nJackCash < 50 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 400000;
            }
            else if (nJackCash < 75 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 500000;
            }
            else if (nJackCash < 100 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 750000;
            }
            else if (nJackCash < 125 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 100 * 10000;
            }
            else if (nJackCash < 150 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 125 * 10000;
            }
            else if (nJackCash < 200 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 150 * 10000;
            }
            else if (nJackCash < 250 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 200 * 10000;
            }
            else
            {
                nCont = JACK_WHALE;
                nPrizeCash = 250 * 10000;
            }
            nJackCash -= nPrizeCash;

            SetSEAPrizeCall(nCont, nPrizeCash);
            CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 2);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, nCont, 2, 0);
            SetGiveStep(2);

            return nJackCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            CGameEngine seaEngine = CGlobal.GetEngine(m_nGameCode);
            if (seaEngine.GetGivePrizeCount() > seaEngine.m_nEmptyPrize)
            {
                //if (CGlobal.Random(0, 2) == 0)
                //    SetSEAPrizeCall(JACK_NISE, 0);
                //else
                //    SetSEAPrizeCall(JACK_NISE1, 0);

                //바당이야기에서는 코인을 생성할때 자동으로 가짜예시를 생성해서 나가므로 여기서는 주지 않는다.
                seaEngine.SetGivePrizeCount(0);
            }

            if (nJackCash < 25000)
                return nJackCash;

            int nPrizeCash = 0;
            int nCont = 0;
            if (nJackCash < 5 * 10000)
            {
                nCont = JACK_JELLY;
                nPrizeCash = 25000;
            }
            else if (nJackCash < 10 * 10000)
            {
                nCont = JACK_JELLY;
                nPrizeCash = 50000;
            }
            else if (nJackCash < 125000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 100000;
            }
            else if (nJackCash < 15 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 125000;
            }
            else if (nJackCash < 20 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 150000;
            }
            else if (nJackCash < 25 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 200000;
            }
            else if (nJackCash < 30 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 200000;
            }
            else if (nJackCash < 40 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 300000;
            }
            else if (nJackCash < 50 * 10000)
            {
                nCont = JACK_SHARK;
                nPrizeCash = 400000;
            }
            else if (nJackCash < 75 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 500000;
            }
            else if (nJackCash < 100 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 750000;
            }
            else if (nJackCash < 125 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 100 * 10000;
            }
            else if (nJackCash < 150 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 125 * 10000;
            }
            else if (nJackCash < 200 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 150 * 10000;
            }
            else if (nJackCash < 250 * 10000)
            {
                nCont = JACK_WHALE;
                nPrizeCash = 200 * 10000;
            }
            else
            {
                nCont = JACK_WHALE;
                nPrizeCash = 250 * 10000;
            }
            nJackCash -= nPrizeCash;

            SetSEAPrizeCall(nCont, nPrizeCash);
            CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 3);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, nCont, 3, 0);
            SetGiveStep(3);

            return nJackCash;
        }

        private void MakeReturnScoreList(int nDstCash, List<int> lstScore)
        {
            while (nDstCash >= 100)
            {
                int nMaxScore = m_lstSeaScores.FindAll(value => value <= nDstCash && value <= 20000).Max(value => value);
                List<int> lstSeaScore = m_lstSeaScores.FindAll(value => value == nMaxScore);
                int nScore = CGlobal.RandomSelect(lstSeaScore);
                lstScore.Add(nScore);

                nDstCash -= nScore;
            }
        }

        private void ReturnGiftCash()
        {
            m_lstSeaReturnGift.Clear();
            if (m_nReturnCash == 0)
                return;

            List<CSEAScoreInfo> lstSeaScoreInfo = new List<CSEAScoreInfo>();
            int nDstCash = m_nReturnCash;
            m_nReturnCash = 0;
            //점수렬을 만든다.
            List<int> lstScore = new List<int>();
            MakeReturnScoreList(nDstCash, lstScore);

            List<CSEAScoreInfo> lstScoreInfo = new List<CSEAScoreInfo>();
            //점수에 해당한 SeaScoreTableInfo리스트를 구한다.
            for (int i = 0; i < lstScore.Count; i++)
            {
                int nDstScore = lstScore[i]; //목표점수
                CSEAScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                lstScoreInfo.Add(scoreInfo);
            }

            while (lstScoreInfo.Count > 0)
            {
                CSEAScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                lstSeaScoreInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                if (RND.Next(2) == 0)
                {
                    scoreInfo = MakeEmptyRoll();
                    lstSeaScoreInfo.Add(scoreInfo);
                }
            }

            for (int i = 0; i < lstSeaScoreInfo.Count; i++)
            {
                //점수정보를 점수타일이 아닌 나머지타일까지 포함한 완전한 타일배렬을 만든다.
                MakeEmptyRoll(lstSeaScoreInfo[i]);
            }
            lstSeaScoreInfo.Add(MakeEmptyRoll());

            m_bReturnGift = true;
            m_lstSeaReturnGift.Clear();
            m_lstSeaReturnGift.InsertRange(0, lstSeaScoreInfo);
        }

        public override void OnGearStart(int nRun)
        {
            m_nGearRun = nRun;
            if(nRun == 1)
            {
                if (m_lstSeaPrizeInfo.Exists(value => value.m_nCmd > PRIZE_START && value.m_nCmd < PRIZE_END))
                {
                    CSEAScoreInfo scoreInfo = MakeEmptyRoll();
                    scoreInfo.m_nCmd = PRIZE_START;
                    m_lstSeaPrizeInfo.Insert(0, scoreInfo);
                    m_nGearJack = 1;
                }
            }
        }


        public override void OnReconnect(CUserSocket clsSocket)
        {
            if(m_lstSeaPrizeInfo.Exists(value=>value.m_nCmd > PRIZE_START && value.m_nCmd < PRIZE_END))
            {
                CSEAScoreInfo scoreInfo = MakeEmptyRoll();
                scoreInfo.m_nCmd = PRIZE_START;
                m_lstSeaPrizeInfo.Insert(0, scoreInfo);
                m_nGearJack = 1;
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

            if (m_lstSeaPrizeInfo.Count > 0)
            {
                //잭팟일때이다.
                m_clsSendScoreInfo = m_lstSeaPrizeInfo[0];
                m_lstSeaPrizeInfo.RemoveAt(0);

                if (m_lstSeaPrizeInfo.Count == 0)
                {
                    ReturnGiftCash();
                    if(m_lstSeaReturnGift.Count == 0)
                        OnEndPrizeCall();
                }
            }
            else if(m_lstSeaReturnGift.Count > 0)
            {
                m_clsSendScoreInfo = m_lstSeaReturnGift[0];
                m_lstSeaReturnGift.RemoveAt(0);
                if(m_lstSeaReturnGift.Count == 0)
                {
                    OnEndPrizeCall();
                }
            }
            else if (m_lstSeaScoreInfo.Count > 0)
            {
                m_clsSendScoreInfo = m_lstSeaScoreInfo[0];
                m_lstSeaScoreInfo.RemoveAt(0);
            }
            else
            {
                int nRollCount = m_nSlotCash / m_nSpeedCash;
                for (int i = 0; i < nRollCount; i++)
                {
                    CSEAScoreInfo scoreInfo = MakeEmptyRoll();
                    m_lstSeaScoreInfo.Add(scoreInfo);
                }

                if (m_lstSeaScoreInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstSeaScoreInfo[0];
                    m_lstSeaScoreInfo.RemoveAt(0);
                }
                else
                {
                    m_clsSendScoreInfo = MakeEmptyRoll();
                    m_clsSendScoreInfo.m_nGearCode = m_nGearCode;
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
            if(m_nGearJack == 2)
            {
                ClearPrizeInfo();
                return;
            }

            if (m_prizeInfo == null)
                return;

            if (m_prizeInfo.m_nCont > 0 && m_prizeInfo.m_nCont < 4 && m_prizeInfo.m_nPrizeCash > 0)
            {
                string[] lstPrizeKind = { "거부기잭팟", "해파리잭팟", "상어잭팟", "고래잭팟" };
                string strMessage = MakeCongration(lstPrizeKind[m_prizeInfo.m_nCont], m_prizeInfo.m_nPrizeCash);
                CGlobal.SendNoticeBroadCast(strMessage);

                CGlobal.CalculateJackPotCash(m_prizeInfo.m_nPrizeCash);
                OnBroadCastPrizeInfo();
            }

            ClearPrizeInfo();
            CDataBase.SaveGearInfoToDB(this);
        }

        public override void OnBroadCastPrizeInfo()
        {
            if (m_prizeInfo == null)
                return;
            if (m_prizeInfo.m_nPrizeCash == 0)
                return;
            if (m_prizeInfo.m_nCont == 0 || m_prizeInfo.m_nCont > 3)
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
                case PRIZE_JELLY:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]해파리[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_SHARK:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]상어[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_WHALE:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]고래[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
               
                case PRIZE_END:
                    break;
            }

            if (strMessage != string.Empty)
                CGlobal.SendNoticeBroadCast(strMessage);
        }

        public override void OnAddGiftCash(CUserSocket clsSocket, int nCash, int nKind = 0)
        {
            m_nGiftCash += nCash;

            CUser user = CGlobal.GetUserByCode(m_nTakeUser);
            CAgent agent = CGlobal.GetAgentByCode(user.m_nAgenCode);
            if (nCash > 0)
            {
                m_nAccuCash -= nCash;
                //if (agent.m_nAgenLevel < 10 && user.m_nChargeCnt > 0)
                //    user.m_nUserWinCash += nCash;

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
                    //user.m_nUserWinCash = user.m_nUserWinCash - (nExCount * 4 * 500);
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

        public override void OnAddSlotCash(CUserSocket clsSocket, int nAddCash)
        {
            m_nAccuCash -= nAddCash;
            m_nSlotCash += nAddCash;
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(-nAddCash, m_nTakeUser);//누적금액을 당첨된 금액만큼 감소시킨다.

            int nRollCount = nAddCash / m_nSpeedCash;
            for (int i = 0; i < nRollCount; i++)
            {
                CSEAScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    m_lstSeaScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(m_lstSeaScoreInfo.Count);
                    m_lstSeaScoreInfo.Insert(nIndex, scoreInfo);
                }
            }
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
                m_nNoiseCash += 100;
                
                if (m_nNoiseCash >= m_nNoiseRaise)
                {
                    m_nNoiseCash = 0;
                    m_nNoiseRaise = RND.Next(800, 2000);
                    m_nNoiseRaise *= 10;

                    int nRnd = CGlobal.Random(0, 10);
                    if (nRnd < 5)
                        SetSEAPrizeCall(JACK_NISE2, 0);
                    else if(nRnd < 8)
                        SetSEAPrizeCall(JACK_NISE, 0);
                    else
                        SetSEAPrizeCall(JACK_NISE1, 0);
                }
            }

            CGlobal.CalcShowJackPot(100, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(100, m_nTakeUser);//누적금액을 사용한 금액만큼 증가시킨다.

            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
        }

        public void OnWaterTank(CUserSocket clsSocket, int nKind, int nValue)
        {
            if(nKind == 0)
            {
                m_nLeftWater += nValue;
                if (m_nLeftWater >= 100)
                    m_nLeftWater = 0;
            }
            else if(nKind == 1)
            {
                m_nRightWater += nValue;
                if (m_nRightWater >= 100)
                    m_nRightWater = 0;
            }

            CDataBase.SaveGearInfoToDB(this);
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nPrizeCash = 0;
            int nJackCont = 0;
            if (clsItem.m_nItemModel == CItemModel.ITEM_SEA_JELLY)
            {
                nJackCont = 1;
                if(nItemCash < 25000)
                {
                    nPrizeCash = 0;
                }
                else if (nItemCash < 5 * 10000)
                {
                    nPrizeCash = 25000;
                }
                else
                {
                    nPrizeCash = 50000;
                }
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_SEA_SHARK)
            {
                nJackCont = JACK_SHARK;
                if (nItemCash < 10 * 10000)
                {
                    nPrizeCash = 0;
                }
                else if (nItemCash < 125 * 1000)
                {
                    nPrizeCash = 10 * 10000;
                }
                else if(nItemCash < 15 * 10000)
                {
                    nPrizeCash = 125 * 1000;
                }
                else if (nItemCash < 20 * 10000)
                {
                    nPrizeCash = 150000;
                }
                else if (nItemCash < 25 * 10000)
                {
                    nPrizeCash = 200000;
                }
                else if (nItemCash < 30 * 10000)
                {
                    nPrizeCash = 200000;
                }
                else if (nItemCash < 40 * 10000)
                {
                    nPrizeCash = 300000;
                }
                else
                {
                    nPrizeCash = 400000;
                }
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_SEA_WHALE)
            {
                nJackCont = JACK_WHALE;
                if(nItemCash < 50 * 10000)
                {
                    nPrizeCash = 0;
                }
                else if (nItemCash < 75 * 10000)
                {
                    nPrizeCash = 500000;
                }
                else if (nItemCash < 100 * 10000)
                {
                    nPrizeCash = 750000;
                }
                else if (nItemCash < 125 * 10000)
                {
                    nPrizeCash = 100 * 10000;
                }
                else if (nItemCash < 150 * 10000)
                {
                    nPrizeCash = 125 * 10000;
                }
                else if (nItemCash < 200 * 10000)
                {
                    nPrizeCash = 150 * 10000;
                }
                else if (nItemCash < 250 * 10000)
                {
                    nPrizeCash = 200 * 10000;
                }
                else
                {
                    nPrizeCash = 250 * 10000;
                }
            }

            if (nPrizeCash == 0)
            {
                //가짜예시이다.
                int nRnd = CGlobal.Random(0, 2);
                if (nRnd == 0)
                    SetSEAPrizeCall(JACK_NISE, 0);
                else
                    SetSEAPrizeCall(JACK_NISE1, 0);

                m_nReturnCash = nItemCash;
            }
            else
            {
                //점수에 해당한 잭팟을 주어야 한다.
                SetSEAPrizeCall(nJackCont, nPrizeCash);
                CGlobal.SetItemEngineRemCash(nItemCash - nPrizeCash);
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, nJackCont, 4, 0);
            }
        }
    }
}
