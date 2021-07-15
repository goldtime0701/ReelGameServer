using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CWhtGear : CBaseGear
    {
        private CWHTPrizeInfo m_prizeInfo;                  //잭팟정보
        private List<CWHTScoreInfo> m_lstPrizeInfo;      //잭팟정보리스트
        private List<CWHTScoreInfo> m_lstScoreInfo;

        private List<CWHTScoreInfo> m_lstReturnGift;
        private CWHTScoreInfo m_clsSendScoreInfo;
        private int m_nReturnCash;                          //잭팟이 끝난다음에 돌려주어야 하는 점수
        private bool m_bReturnGift;

        private int m_nOverCash;                            //점수배렬을 만들면서 더 첨부된 점수
        private bool m_bPrizeAni;                           //잭팟예시 애니매션상태인가
        private int m_nPrizeAniCnt;                         //잭팟애니매션이 진행되는동안 몇번돌았는가(애니매션이 진행되던 도중 통신이 끊어지는 경우 사용,  20보다 크면 애니매션이 끝났다고 본다.)
        private int m_nNoiseCash;                           //가짜밤예시를 주기 위한 변수
        private int m_nNoiseRaise;                          //가짜잭팟발동금액



        public CWhtGear() : base()
        {
            m_nSpeedCash = 100;
            m_lstScoreInfo = new List<CWHTScoreInfo>();
            m_lstPrizeInfo = new List<CWHTScoreInfo>();
            m_lstReturnGift = new List<CWHTScoreInfo>();

            m_nNoiseRaise = RND.Next(1500, 3000);
            m_nNoiseRaise *= 10;
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
            List<CWHTScoreInfo> lstWhtScoreInfo = new List<CWHTScoreInfo>();

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

                List<CWHTScoreInfo> lstScoreInfo = new List<CWHTScoreInfo>();
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CWHTScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CWHTScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstWhtScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstWhtScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CWHTScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstWhtScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstWhtScoreInfo.Count);
                    lstWhtScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                m_lstScoreInfo.Clear();
            }

            m_lstScoreInfo.AddRange(lstWhtScoreInfo);
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
        private CWHTScoreInfo MakeNormalScore(int nScore)
        {
            if (nScore == 0)
                return MakeEmptyRoll();

            //점수테이블리스트를 만든다.
            List<CWHTScoreTable> lstMatchScoreInfo = m_lstScoreTable.FindAll(value => value.m_nScore == nScore);
            CWHTScoreTable scoreTable = CGlobal.RandomSelect(lstMatchScoreInfo);
            CWHTScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nOverCash += scoreInfo.m_nScore - nScore;

            return scoreInfo;
        }

        private CWHTScoreInfo ConvertScoreTableToScoreInfo(CWHTScoreTable scoreTable)
        {
            CWHTScoreInfo scoreInfo = MakeScoreInfoFromScoreTable(scoreTable);
            return scoreInfo;
        }

        private CWHTScoreInfo MakeScoreInfoFromScoreTable(CWHTScoreTable scoreTable)
        {
            CWHTScoreInfo scoreInfo = new CWHTScoreInfo();

            int nCol = RND.Next(4);  //조커로 변하지 말아야 할 위치
            int nBar = 0; //맞은 점수렬에서 잡빠없애기 위한 변수

            //맞은 타일을 만든다.
            for (int i = 0; i < scoreTable.m_nlstTile.Length; i++)
            {
                int nTile = scoreTable.m_nlstTile[i];

                if (nTile == BARC)
                {
                    nTile = RND.Next(1, 4);
                    if (i == 2)
                    {
                        int nT1 = scoreInfo.m_lstTile.Find(value => value.m_nRow == scoreTable.m_nLine && value.m_nCol == 1).m_nTile;
                        while (nTile == nT1)
                        {
                            nTile = RND.Next(1, 4);
                        }
                    }
                }

                if (nTile == BAR1 || nTile == BAR2 || nTile == BAR3)
                    nBar++;
                if (scoreTable.m_nCount == 4)
                {
                    if (RND.Next(5) == 2 && i != nCol)
                        nTile = JOKE;
                }
                else if (nTile == ANNY)
                {
                    List<int> lstTiles = m_lstTiles.ToList().FindAll(value => value != scoreTable.m_nTile && value > 0);
                    if (nBar >= 2)
                        lstTiles = lstTiles.FindAll(value => value > 3);

                    if (i == 0 || i == 3)
                        nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != SHRI));
                    else
                        nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != STAR && value != SEVN && value != TAGT));
                }

                CTile tile = new CTile(nTile, scoreTable.m_nLine, i);
                if (scoreTable.m_nlstTile[i] != ANNY)
                    tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);
            }


            int[] nBarCnt = { 0, 0, 0 };
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != SHRI));
                    if (!lstLine0.Exists(value => value == nTile))
                        break;
                }
                lstLine0.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 0);
                scoreInfo.m_lstTile.Add(tile);
                if (nTile == BAR1 || nTile == BAR2 || nTile == BAR3)
                    nBarCnt[nRow]++;
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != SHRI));
                    if (!lstLine3.Exists(value => value == nTile))
                        break;
                }
                lstLine3.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 3);
                scoreInfo.m_lstTile.Add(tile);
                if (nTile == BAR1 || nTile == BAR2 || nTile == BAR3)
                    nBarCnt[nRow]++;
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

                    if (lstLine0[nRow] == nTile && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                        continue;
                    if (lstLine0[nRow] == JOKE && (nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                        continue;
                    if (nTile == JOKE && (lstLine0[nRow] == STAR || lstLine0[nRow] == SEVN || lstLine0[nRow] == TAGT || lstLine0[nRow] == SHRI))
                        continue;
                    if (nTile == JOKE && lstLine3[nRow] == JOKE)
                        continue;
                    if (nBarCnt[nRow] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                        continue;

                    break;
                }
                lstLine1.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 1);
                scoreInfo.m_lstTile.Add(tile);
                if (nTile == BAR1 || nTile == BAR2 || nTile == BAR3)
                    nBarCnt[nRow]++;
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
                    

                    if (nTile == lstLine3[nRow] && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (lstLine3[nRow] == JOKE && (nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine3[nRow] == SHRI || lstLine3[nRow] == STAR || lstLine3[nRow] == SEVN || lstLine3[nRow] == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == JOKE || lstLine0[nRow] == JOKE || lstLine3[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow] || lstLine1[nRow] == lstLine3[nRow]))
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == lstLine3[nRow] || lstLine0[nRow] == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine3[nRow] && (lstLine1[nRow] == JOKE || nTile == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine1[nRow] && (lstLine3[nRow] == JOKE || nTile == lstLine3[nRow] || lstLine0[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow]))
                        continue;

                    if (nBarCnt[nRow] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
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


        private CWHTScoreInfo MakeEmptyRoll()
        {
            CWHTScoreInfo scoreInfo = new CWHTScoreInfo();
            int[] nBarCnt = { 0, 0, 0 };

            //0번째 라인을 만든다.
            List<int> lstLine0 = new List<int>();
            while (true)
            {
                lstLine0.Clear();
                int nPos = RND.Next(0, 11);
                if (m_lstOriginalTile[0][nPos] == SHRI)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[0].Count)
                    nPos = 0;
                if (m_lstOriginalTile[0][nPos] == SHRI)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[0].Count)
                    nPos = 0;
                if (m_lstOriginalTile[0][nPos] == SHRI)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                break;
            }

            for (int i = 0; i < 3; i++)
            {
                if (lstLine0[i] == BAR1 || lstLine0[i] == BAR2 || lstLine0[i] == BAR3)
                    nBarCnt[i]++;
            }


            //3번라인을 만든다.
            List<int> lstLine3 = new List<int>();
            while (true)
            {
                lstLine3.Clear();
                int nPos = RND.Next(0, 11);
                if (m_lstOriginalTile[3][nPos] == SHRI)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[3].Count)
                    nPos = 0;
                if (m_lstOriginalTile[3][nPos] == SHRI)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[3].Count)
                    nPos = 0;
                if (m_lstOriginalTile[3][nPos] == SHRI)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                break;
            }

            for (int i = 0; i < 3; i++)
            {
                if (lstLine3[i] == BAR1 || lstLine3[i] == BAR2 || lstLine3[i] == BAR3)
                    nBarCnt[i]++;
            }

            //1번째 라인을 만든다.
            List<int> lstLine1 = new List<int>();
            while (true)
            {
                lstLine1.Clear();
                int nPos = RND.Next(0, 11);
                int nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[0] == nTile && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                    continue;
                if (lstLine0[0] == JOKE && (nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                    continue;
                if (nTile == JOKE && (lstLine0[0] == STAR || lstLine0[0] == SEVN || lstLine0[0] == TAGT || lstLine0[0] == SHRI))
                    continue;
                if (nTile == JOKE && lstLine3[0] == JOKE)
                    continue;
                if (nBarCnt[0] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                    continue;

                lstLine1.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[1].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[1] == nTile && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                    continue;
                if (lstLine0[1] == JOKE && (nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                    continue;
                if (nTile == JOKE && (lstLine0[1] == STAR || lstLine0[1] == SEVN || lstLine0[1] == TAGT))
                    continue;
                if (nTile == JOKE && lstLine3[1] == JOKE)
                    continue;
                if (nBarCnt[1] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                    continue;

                lstLine1.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[1].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[2] == nTile && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                    continue;
                if (lstLine0[2] == JOKE && (nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                    continue;
                if (nTile == JOKE && (lstLine0[2] == STAR || lstLine0[2] == SEVN || lstLine0[2] == TAGT))
                    continue;
                if (nTile == JOKE && lstLine3[2] == JOKE)
                    continue;
                if (nBarCnt[2] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                    continue;

                lstLine1.Add(nTile);

                break;
            }

            for (int i = 0; i < 3; i++)
            {
                if (lstLine1[i] == BAR1 || lstLine1[i] == BAR2 || lstLine1[i] == BAR3)
                    nBarCnt[i]++;
            }


            //2번라인을 만든다.
            int nTempCnt = 0;

            List<int> lstLine2 = new List<int>();
            while (true)
            {
                nTempCnt++;
                
                lstLine2.Clear();
                int nPos = RND.Next(0, 11);
                int nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[0] && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (lstLine3[0] == JOKE && (nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine3[0] == SHRI || lstLine3[0] == STAR || lstLine3[0] == SEVN || lstLine3[0] == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine1[0] == JOKE || lstLine0[0] == JOKE || lstLine0[0] == lstLine1[0]))
                        continue;
                    if (nTile == lstLine3[0] && (lstLine1[0] == JOKE || nTile == lstLine1[0]))
                        continue;
                    if (nTile == lstLine1[0] && (lstLine3[0] == JOKE || nTile == lstLine3[0] || lstLine0[0] == JOKE || lstLine0[0] == lstLine1[0]))
                        continue;
                    if (nTile == JOKE && lstLine1[0] == lstLine3[0])
                        continue;

                    if (nBarCnt[0] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                        continue;
                }
                lstLine2.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[2].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[1] && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (lstLine3[1] == JOKE && (nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine3[1] == SHRI || lstLine3[1] == STAR || lstLine3[1] == SEVN || lstLine3[1] == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine1[1] == JOKE || lstLine0[1] == JOKE || lstLine0[1] == lstLine1[1]))
                        continue;
                    if (nTile == lstLine3[1] && (lstLine1[1] == JOKE || nTile == lstLine1[1]))
                        continue;
                    if (nTile == lstLine1[1] && (lstLine3[1] == JOKE || nTile == lstLine3[1] || lstLine0[1] == JOKE || lstLine0[1] == lstLine1[1]))
                        continue;
                    if (nTile == JOKE && lstLine1[1] == lstLine3[1])
                        continue;

                    if (nBarCnt[1] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                        continue;
                }
                lstLine2.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[2].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[2] && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (lstLine3[2] == JOKE && (nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine3[2] == SHRI || lstLine3[2] == STAR || lstLine3[2] == SEVN || lstLine3[2] == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine1[2] == JOKE || lstLine0[2] == JOKE || lstLine0[2] == lstLine1[2]))
                        continue;
                    if (nTile == lstLine3[2] && (lstLine1[2] == JOKE || nTile == lstLine1[2]))
                        continue;
                    if (nTile == lstLine1[2] && (lstLine3[2] == JOKE || nTile == lstLine3[2] || lstLine0[2] == JOKE || lstLine0[2] == lstLine1[2]))
                        continue;
                    if (nTile == JOKE && lstLine1[2] == lstLine3[2])
                        continue;

                    if (nBarCnt[2] >= 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
                        continue;
                }
                lstLine2.Add(nTile);

                break;
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
            scoreInfo.m_nMulti = CGlobal.RandomSelect(m_lstMulti);


            return scoreInfo;
        }

        public bool SetWhtPrizeCall(int nCont, int nJackAmount)
        {
            if (m_nGearJack > 0)
                return false;
            m_prizeInfo = new CWHTPrizeInfo();
            m_prizeInfo.m_nCont = nCont;
            m_prizeInfo.m_nPrizeCash = nJackAmount;

            m_lstPrizeInfo.Clear();
            bool ret = MakePrizeRoll();
            return ret;
        }

        public override bool MakePrizeRoll()
        {
            m_nGearJack = 1;    //잭팟상태로 설정
            CDataBase.SaveGearInfoToDB(this);

            //잭팟시작신호를 보낸다.
            MakePrizeCommand(PRIZE_START);
            MakePrizeCommand(PRIZE_NIGHT);

            //가짜밤예시
            if (m_prizeInfo.m_nCont == JACK_NOISE)
            {
                MakePrizeEmpty(10);
                MakePrizeCommand(PRIZE_DAY);
                MakePrizeCommand(PRIZE_END);
                return true;
            }
            MakePrizeEmpty(10);

            //성게
            MakePrizeCommand(PRIZE_URCHIN);
            if (m_prizeInfo.m_nCont == JACK_URCHIN)
            {
                MakePrizeEmpty(2);
                MakePrizeCommand(PRIZE_DAY);
                PrizeUrchin();
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(10, true);

            //오징어
            MakePrizeCommand(PRIZE_CUTTLE);
            if (m_prizeInfo.m_nCont == JACK_CUTTLE)
            {
                MakePrizeEmpty(2);
                MakePrizeCommand(PRIZE_DAY);
                PrizeCuttle();
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(10, true);

            //날치
            MakePrizeCommand(PRIZE_TUNA);
            if (m_prizeInfo.m_nCont == JACK_TUNA)
            {
                MakePrizeEmpty(2);
                MakePrizeCommand(PRIZE_DAY);
                PrizeTuna();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, true);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(10, true);

            //상어
            MakePrizeCommand(PRIZE_SHARK);
            if (m_prizeInfo.m_nCont == JACK_SHARK)
            {
                MakePrizeEmpty(2);
                MakePrizeCommand(PRIZE_DAY);
                PrizeShark();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, true);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(10, true);

            //고래
            MakePrizeCommand(PRIZE_WHALE);
            if (m_prizeInfo.m_nCont == JACK_WHALE)
            {
                MakePrizeEmpty(2);
                MakePrizeCommand(PRIZE_DAY);
                PrizeWhale();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, true);
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
            CWHTScoreInfo scoreInfo = MakeEmptyRoll();
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

        private void PrizeUrchin()
        {
            List<CWHTScoreTable> lstTable = m_lstScoreTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash && value.m_nTile > 3).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CWHTScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CWHTScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            m_lstPrizeInfo.Add(scoreInfo);
            MakePrizeEmpty(1);
        }

        private void PrizeCuttle()
        {
            List<CWHTScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash && value.m_nTile > 3).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CWHTScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CWHTScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
            MakePrizeEmpty(1);
        }

        private void PrizeTuna()
        {
            List<CWHTScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CWHTScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CWHTScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }

        private void PrizeShark()
        {
            List<CWHTScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CWHTScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CWHTScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }

        private void PrizeWhale()
        {
            List<CWHTScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            CWHTScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            CWHTScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }

        private CWHTScoreInfo ConvertScoreTableToPrizeScore(CWHTScoreTable scoreTable)
        {
            CWHTScoreInfo scoreInfo = new CWHTScoreInfo();

            int nCol = RND.Next(4);  //조커로 변하지 말아야 할 위치
            //맞은 타일을 만든다.
            for (int i = 0; i < scoreTable.m_nlstTile.Length; i++)
            {
                int nTile = scoreTable.m_nlstTile[i];
                if (RND.Next(5) == 2 && i != nCol)
                    nTile = JOKE;

                CTile tile = new CTile(nTile, scoreTable.m_nLine, i);
                tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);
            }

            int[] nBarCnt = { 0, 0, 0 };
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != SHRI));
                    if (!lstLine0.Exists(value => value == nTile))
                        break;
                }
                lstLine0.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 0);
                scoreInfo.m_lstTile.Add(tile);
                if (nTile == BAR1 || nTile == BAR2 || nTile == BAR3)
                    nBarCnt[nRow]++;
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != SHRI));
                    if (!lstLine3.Exists(value => value == nTile))
                        break;
                }
                lstLine3.Add(nTile);
                CTile tile = new CTile(nTile, nRow, 3);
                scoreInfo.m_lstTile.Add(tile);
                if (nTile == BAR1 || nTile == BAR2 || nTile == BAR3)
                    nBarCnt[nRow]++;
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

                    if (lstLine0[nRow] == nTile && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                        continue;
                    if (lstLine0[nRow] == JOKE && (nTile == SHRI || nTile == STAR || nTile == TAGT || nTile == SEVN))
                        continue;
                    if (nTile == JOKE && (lstLine0[nRow] == STAR || lstLine0[nRow] == SEVN || lstLine0[nRow] == TAGT || lstLine0[nRow] == SHRI))
                        continue;
                    if (nTile == JOKE && lstLine3[nRow] == JOKE)
                        continue;
                    if (nBarCnt[nRow] == 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
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
                    

                    if (nTile == lstLine3[nRow] && (nTile == JOKE || nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (lstLine3[nRow] == JOKE && (nTile == SHRI || nTile == STAR || nTile == SEVN || nTile == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine3[nRow] == SHRI || lstLine3[nRow] == STAR || lstLine3[nRow] == SEVN || lstLine3[nRow] == TAGT))
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == JOKE || lstLine0[nRow] == JOKE || lstLine3[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow] || lstLine1[nRow] == lstLine3[nRow]))
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == lstLine3[nRow] || lstLine0[nRow] == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine3[nRow] && (lstLine1[nRow] == JOKE || nTile == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine1[nRow] && (lstLine3[nRow] == JOKE || nTile == lstLine3[nRow] || lstLine0[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow]))
                        continue;

                    if (nBarCnt[nRow] == 2 && (nTile == BAR1 || nTile == BAR2 || nTile == BAR3))
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

        private void MakeAfterPrizeScoreList(int nScore)
        {
            if (nScore == 0)
                return;

            List<CWHTScoreInfo> lstScoreInfo = new List<CWHTScoreInfo>();
            while (nScore >= 100)
            {
                int nCash = 0;
                if (nScore > 50000)
                    nCash = m_lstScores.FindAll(value => value <= nScore).Max(value => value);
                else if (nScore > 20000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 5000));
                else if (nScore > 5000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 1000));
                else
                    nCash = m_lstScores.FindAll(value => value <= nScore).Max(value => value);

                CWHTScoreTable scoreTable = CGlobal.RandomSelect(m_lstScoreTable.FindAll(value => value.m_nScore == nCash));
                CWHTScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
                lstScoreInfo.Add(scoreInfo);
                nScore -= scoreInfo.m_nScore;
            }

            MakePrizeEmpty(1);
            while (lstScoreInfo.Count > 0)
            {
                CWHTScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                m_lstPrizeInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                MakePrizeEmpty(1);
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
                case PRIZE_CUTTLE:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]오징어[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case PRIZE_TUNA:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]날치[-] 출현을 축하드립니다. 대박 나세요..";
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
                    SetWhtPrizeCall(JACK_NOISE, 0);
                }
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

            if (m_prizeInfo.m_nCont > 1 && m_prizeInfo.m_nPrizeCash > 0)
            {
                string[] strCont = { "", "성게 ", "오징어 ", "날치 ", "상어 ", "고래 " };
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
                if (m_lstPrizeInfo.Exists(value => value.m_nCmd >= PRIZE_URCHIN && value.m_nCmd <= PRIZE_WHALE))
                {
                    CWHTScoreInfo scoreInfo = MakeEmptyRoll();
                    scoreInfo.m_nCmd = PRIZE_NIGHT;
                    m_lstPrizeInfo.Insert(0, scoreInfo);
                }
            }
        }

        public override void OnReconnect(CUserSocket clsSocket)
        {
            if (m_lstPrizeInfo.Exists(value => value.m_nCmd >= PRIZE_URCHIN && value.m_nCmd <= PRIZE_WHALE))
            {
                CWHTScoreInfo scoreInfo = MakeEmptyRoll();
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

                    if (m_clsSendScoreInfo.m_nCmd == PRIZE_URCHIN || m_clsSendScoreInfo.m_nCmd == PRIZE_CUTTLE || m_clsSendScoreInfo.m_nCmd == PRIZE_TUNA ||
                        m_clsSendScoreInfo.m_nCmd == PRIZE_SHARK || m_clsSendScoreInfo.m_nCmd == PRIZE_WHALE)
                    {
                        m_bPrizeAni = true;
                    }
                }
                else
                {
                    m_nPrizeAniCnt++;
                    if (m_nPrizeAniCnt > 20)
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
                    CWHTScoreInfo scoreInfo = MakeEmptyRoll();
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

            List<CWHTScoreInfo> lstOcaScoreInfo = new List<CWHTScoreInfo>();
            int nDstCash = m_nReturnCash;
            m_nReturnCash = 0;
            //점수렬을 만든다.
            List<int> lstScore = new List<int>();
            MakeReturnScoreList(nDstCash, lstScore);

            List<CWHTScoreInfo> lstScoreInfo = new List<CWHTScoreInfo>();
            //점수에 해당한 SeaScoreTableInfo리스트를 구한다.
            for (int i = 0; i < lstScore.Count; i++)
            {
                int nDstScore = lstScore[i]; //목표점수
                CWHTScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                lstScoreInfo.Add(scoreInfo);
            }

            while (lstScoreInfo.Count > 0)
            {
                CWHTScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                lstOcaScoreInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                if (RND.Next(2) == 0)
                {
                    scoreInfo = MakeEmptyRoll();
                    lstOcaScoreInfo.Add(scoreInfo);
                }
            }

            lstOcaScoreInfo.Add(MakeEmptyRoll());

            m_bReturnGift = true;
            m_lstReturnGift.Clear();
            m_lstReturnGift.InsertRange(0, lstOcaScoreInfo);
        }

        private void MakeReturnScoreList(int nDstCash, List<int> lstScore)
        {
            while (nDstCash >= 100)
            {
                int nMaxScore = m_lstScores.FindAll(value => value <= nDstCash && value <= 20000).Max(value => value);
                List<int> lstOcaScore = m_lstScores.FindAll(value => value == nMaxScore);
                int nScore = CGlobal.RandomSelect(lstOcaScore);
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
            List<CWHTScoreTable> lstScoreTable = m_lstScoreTable.ToList().FindAll(value => value.m_nScore <= nJackCash && value.m_nScore <= 5000).OrderByDescending(value => value.m_nScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nPrizeCash = lstScoreTable[0].m_nScore;

                if (RND.Next(5) == 3)
                {
                    int nCont = 0;

                    if (nPrizeCash <= 5000)
                        nCont = JACK_URCHIN;

                    SetWhtPrizeCall(nCont, nPrizeCash);
                }
                else
                {
                    CWHTScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(CGlobal.RandomSelect(lstScoreTable));
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

            if (nJackCash < 5000)
                return nJackCash;

            CWHTPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            SetWhtPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            CDataBase.InsertNationalToDB(m_nGearCode, prizeCashInfo.m_nCash, 2);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, 2, 0);
            SetGiveStep(2);

            return nJackCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 5000)
                return nJackCash;

            CWHTPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            SetWhtPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            CDataBase.InsertNationalToDB(m_nGearCode, prizeCashInfo.m_nCash, 3);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, 3, 0);
            SetGiveStep(3);

            return nJackCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 5000)
                return nJackCash;

            CWHTPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            bool bret = SetWhtPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            if (bret)
            {
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                if (robot != null)
                {
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, -1, 1);
                    robot.PlayWhtPrize();
                }
            }

            return nJackCash;
        }

        public CWHTPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public List<CWHTScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstPrizeInfo;
        }

        public void OnFinishAnimation()
        {
            m_bPrizeAni = false;
            m_nPrizeAniCnt = 0;
        }

        public void OnWaterTank(CUserSocket clsSocket)
        {
            m_nLeftWater++;
            if (m_nLeftWater >= 51)
            {
                m_nLeftWater = 0;
                OnAddGiftCash(clsSocket, 5000, 1);
            }

            CDataBase.SaveGearInfoToDB(this);
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nJackCont = 0;
            if (clsItem.m_nItemModel == CItemModel.ITEM_WHT_OCTOR)
            {
                nJackCont = JACK_CUTTLE;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_WHT_FISH)
            {
                nJackCont = JACK_TUNA;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_WHT_SHARK)
            {
                nJackCont = JACK_SHARK;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_WHT_WHALE)
            {
                nJackCont = JACK_WHALE;
            }


            List<CWHTPrizeCashInfo> lstPrizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nPrize == nJackCont && value.m_nCash <= nItemCash);
            if (lstPrizeCashInfo == null || lstPrizeCashInfo.Count == 0)
            {
                //가짜예시이다.
                SetWhtPrizeCall(JACK_NOISE, 0);
                m_nReturnCash = nItemCash;
            }
            else
            {
                //점수에 해당한 잭팟을 주어야 한다.
                int nJackScore = lstPrizeCashInfo.Max(value => value.m_nCash);
                SetWhtPrizeCall(nJackCont, nJackScore);
                CGlobal.SetItemEngineRemCash(nItemCash - nJackScore);

                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nJackScore, nJackCont, 4, 0);
            }
        }
    }
}
