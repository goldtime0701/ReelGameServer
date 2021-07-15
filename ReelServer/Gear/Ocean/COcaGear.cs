using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class COcaGear : CBaseGear
    {
        private COCAPrizeInfo m_prizeInfo;                  //잭팟정보
        private List<COCAScoreInfo> m_lstOcaPrizeInfo;      //잭팟정보리스트
        private List<COCAScoreInfo> m_lstOcaScoreInfo;

        private List<COCAScoreInfo> m_lstOcaReturnGift;
        private COCAScoreInfo m_clsSendScoreInfo;
        private int m_nReturnCash;                          //잭팟이 끝난다음에 돌려주어야 하는 점수
        private bool m_bReturnGift;

        private int m_nOverCash;                            //점수배렬을 만들면서 더 첨부된 점수
        private bool m_bPrizeAni;                           //잭팟예시 애니매션상태인가
        private int m_nPrizeAniCnt;                         //잭팟애니매션이 진행되는동안 몇번돌았는가(애니매션이 진행되던 도중 통신이 끊어지는 경우 사용,  20보다 크면 애니매션이 끝났다고 본다.)
        private int m_nNoiseCash;                           //가짜밤예시를 주기 위한 변수
        private int m_nNoiseRaise;                          //가짜잭팟발동금액



        public COcaGear() : base()
        {
            m_nSpeedCash = 100;
            m_lstOcaScoreInfo = new List<COCAScoreInfo>();
            m_lstOcaPrizeInfo = new List<COCAScoreInfo>();
            m_lstOcaReturnGift = new List<COCAScoreInfo>();

            m_nNoiseRaise = RND.Next(1500, 3000);
            m_nNoiseRaise *= 10;
        }

        public COCAPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }
        public List<COCAScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstOcaPrizeInfo;
        }


        public override void ClearGear()
        {
            m_nOverCash = 0;
            m_nSlotCash = 0;
            m_nGiftCash = 0;
            m_nGearJack = 0;
            m_nReturnCash = 0;
            SetGiveStep(0);
            m_lstOcaPrizeInfo.Clear();
            m_lstOcaScoreInfo.Clear();
            m_lstOcaReturnGift.Clear();
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
            m_lstOcaPrizeInfo.Clear();
            m_lstOcaReturnGift.Clear();
            m_bPrizeAni = false;
            m_nPrizeAniCnt = 0;
        }
        
        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<COCAScoreInfo> lstOcaScoreInfo = new List<COCAScoreInfo>();

            int nTempCash = nAddCash;

            while (nTempCash > 0)
            {
                int nAppenCash = nTempCash >= 10000 ? 10000 : nTempCash;
                nTempCash -= nAppenCash;
                int nDstCash = (int)(nAppenCash * rate / 100);

                if(m_nOverCash > 0)
                {
                    if(m_nOverCash > nDstCash / 2)
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

                List<COCAScoreInfo> lstScoreInfo = new List<COCAScoreInfo>();
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    COCAScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    COCAScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstOcaScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstOcaScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                COCAScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstOcaScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstOcaScoreInfo.Count);
                    lstOcaScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                m_lstOcaScoreInfo.Clear();
            }

            m_lstOcaScoreInfo.AddRange(lstOcaScoreInfo);
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
        private COCAScoreInfo MakeNormalScore(int nScore)
        {
            if (nScore == 0)
                return MakeEmptyRoll();

            //점수테이블리스트를 만든다.
            List<COCAScoreTable> lstMatchScoreInfo = m_lstScoreTable.FindAll(value => value.m_nScore == nScore);
            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstMatchScoreInfo);
            COCAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nOverCash += scoreInfo.m_nScore - nScore;

            return scoreInfo;
        }

        private int FindTilePos(int nTile, int nCol)
        {
            List<int> nlstPos = new List<int>();
            for (int i = 0; i < m_lstOriginalTile[nCol].Count; i++)
            {
                if (m_lstOriginalTile[nCol][i] == nTile)
                    nlstPos.Add(i);
            }

            int nPos = CGlobal.RandomSelect(nlstPos);

            return nPos;
        }

        private COCAScoreInfo MakeEmptyRoll()
        {
            COCAScoreInfo scoreInfo = new COCAScoreInfo();
            int[] nBarCnt = { 0, 0, 0 };

            //0번째 라인을 만든다.
            List<int> lstLine0 = new List<int>();
            while (true)
            {
                lstLine0.Clear();
                int nPos = RND.Next(0, 16);
                if (m_lstOriginalTile[0][nPos] == FLOW)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[0].Count)
                    nPos = 0;
                if (m_lstOriginalTile[0][nPos] == FLOW)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[0].Count)
                    nPos = 0;
                if (m_lstOriginalTile[0][nPos] == FLOW)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                break;
            }

            for(int i=0; i<3; i++)
            {
                if (lstLine0[i] == BAR1 || lstLine0[i] == BAR2 || lstLine0[i] == BAR3)
                    nBarCnt[i]++;
            }
            

            //3번라인을 만든다.
            List<int> lstLine3 = new List<int>();
            while (true)
            {
                lstLine3.Clear();
                int nPos = RND.Next(0, 16);
                if (m_lstOriginalTile[3][nPos] == FLOW)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[3].Count)
                    nPos = 0;
                if (m_lstOriginalTile[3][nPos] == FLOW)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[3].Count)
                    nPos = 0;
                if (m_lstOriginalTile[3][nPos] == FLOW)
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
                int nPos = RND.Next(0, 16);
                int nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[0] == nTile && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                    continue;
                if (lstLine0[0] == JOKE && (nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                    continue;
                if (nTile == JOKE && (lstLine0[0] == STAR || lstLine0[0] == SEVE || lstLine0[0] == TARG || lstLine0[0] == FLOW))
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
                if (lstLine0[1] == nTile && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                    continue;
                if (lstLine0[1] == JOKE && (nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                    continue;
                if (nTile == JOKE && (lstLine0[1] == STAR || lstLine0[1] == SEVE || lstLine0[1] == TARG))
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
                if (lstLine0[2] == nTile && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                    continue;
                if (lstLine0[2] == JOKE && (nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                    continue;
                if (nTile == JOKE && (lstLine0[2] == STAR || lstLine0[2] == SEVE || lstLine0[2] == TARG))
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

            int nTempCnt = 0;
            //2번째 라인을 만든다.
            List<int> lstLine2 = new List<int>();
            while (true)
            {
                nTempCnt++;


                lstLine2.Clear();
                int nPos = RND.Next(0, 16);
                int nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[0] && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (lstLine3[0] == JOKE && (nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (nTile == JOKE && (lstLine3[0] == FLOW || lstLine3[0] == STAR || lstLine3[0] == SEVE || lstLine3[0] == TARG))
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
                    if (nTile == lstLine3[1] && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (lstLine3[1] == JOKE && (nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (nTile == JOKE && (lstLine3[1] == FLOW || lstLine3[1] == STAR || lstLine3[1] == SEVE || lstLine3[1] == TARG))
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
                    if (nTile == lstLine3[2] && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (lstLine3[2] == JOKE && (nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (nTile == JOKE && (lstLine3[2] == FLOW || lstLine3[2] == STAR || lstLine3[2] == SEVE || lstLine3[2] == TARG))
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


        private COCAScoreInfo ConvertScoreTableToScoreInfo(COCAScoreTable scoreTable)
        {
            //COCAScoreInfo scoreInfo = null;

            //while(true)
            //{
            //    bool ret = MakeScoreInfoFromScoreTable(ref scoreInfo, scoreTable);
            //    if (ret)
            //        break;
            //}

            COCAScoreInfo scoreInfo = MakeScoreInfoFromScoreTable(scoreTable);
            return scoreInfo;
        }

        private COCAScoreInfo MakeScoreInfoFromScoreTable(COCAScoreTable scoreTable)
        {
            COCAScoreInfo scoreInfo = new COCAScoreInfo();

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
                if(scoreTable.m_nCount == 4)
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
                        nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != FLOW ));
                    else
                        nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != STAR && value != SEVE && value != TARG));
                }

                CTile tile = new CTile(nTile, scoreTable.m_nLine, i);
                if(scoreTable.m_nlstTile[i] != ANNY)
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != FLOW));
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != FLOW));
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

                    if (lstLine0[nRow] == nTile && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                        continue;
                    if (lstLine0[nRow] == JOKE && (nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                        continue;
                    if (nTile == JOKE && (lstLine0[nRow] == STAR || lstLine0[nRow] == SEVE || lstLine0[nRow] == TARG || lstLine0[nRow] == FLOW))
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
                    
                    if (nTile == lstLine3[nRow] && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (lstLine3[nRow] == JOKE && (nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (nTile == JOKE && (lstLine3[nRow] == FLOW || lstLine3[nRow] == STAR || lstLine3[nRow] == SEVE || lstLine3[nRow] == TARG))
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

        /*
        private bool MakeScoreInfoFromScoreTable(ref COCAScoreInfo scoreInfo, COCAScoreTable scoreTable)
        {
            scoreInfo = new COCAScoreInfo();

            scoreInfo.m_nGearCode = m_nGearCode;
            scoreInfo.m_nScore = scoreTable.m_nScore;
            scoreInfo.m_nMulti = scoreTable.m_nMulti;
            scoreInfo.m_nWinTile = scoreTable.m_nTile;
            scoreInfo.m_nWinCnt = scoreTable.m_nCount;
            scoreInfo.m_lstLine.Add(scoreTable.m_nLine);

            for (int nCol = 0; nCol < 4; nCol++)
            {
                int nTile = scoreTable.m_nlstTile[nCol];
                if (nTile == ANNY)
                {
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != JOKE && value != scoreTable.m_nTile));
                }
                if (scoreTable.m_nCount == 4)
                {
                    if ((nCol == 3 && scoreInfo.m_lstTile.Exists(value => value.m_nCol == nCol && value.m_nTile != JOKE)) || nCol < 3)
                    {
                        if (RND.Next(5) == 2)
                            nTile = JOKE;
                    }
                }

                if (nTile == BARC)
                {
                    nTile = RND.Next(1, 4);
                    if (nCol == 2)
                    {
                        int nT1 = scoreInfo.m_lstTile.Find(value => value.m_nRow == scoreTable.m_nLine && value.m_nCol == 1).m_nTile;
                        while (nTile == nT1)
                        {
                            nTile = RND.Next(1, 4);
                        }
                    }
                }
                    

                int nPos = FindTilePos(nTile, nCol);
                CTile tile = new CTile(nTile, scoreTable.m_nLine, nCol);
                if(scoreTable.m_nlstTile[nCol] != ANNY)
                    tile.m_nAct = 1;
                scoreInfo.m_lstTile.Add(tile);

                int nPos0 = -1, nPos1 = -1;
                int nRow0 = -1, nRow1 = -1;
                if (scoreTable.m_nLine == TOP)
                {
                    nPos0 = nPos - 1;
                    if (nPos0 < 0)
                        nPos0 = m_lstOriginalTile[nCol].Count - 1;
                    nRow0 = MID;

                    nPos1 = nPos0 - 1;
                    if (nPos1 < 0)
                        nPos1 = m_lstOriginalTile[nCol].Count - 1;
                    nRow1 = BOT;
                }
                else if (scoreTable.m_nLine == MID)
                {
                    nPos0 = nPos + 1;
                    if (nPos0 == m_lstOriginalTile[nCol].Count)
                        nPos0 = 0;
                    nRow0 = TOP;

                    nPos1 = nPos - 1;
                    if (nPos1 < 0)
                        nPos1 = m_lstOriginalTile[nCol].Count - 1;
                    nRow1 = BOT;
                }
                else if (scoreTable.m_nLine == BOT)
                {
                    nPos0 = nPos + 1;
                    if (nPos0 == m_lstOriginalTile[nCol].Count)
                        nPos0 = 0;
                    nRow0 = MID;

                    nPos1 = nPos0 + 1;
                    if (nPos1 == m_lstOriginalTile[nCol].Count)
                        nPos1 = 0;
                    nRow1 = TOP;
                }

                int nTile0 = m_lstOriginalTile[nCol][nPos0];
                CTile tile0 = new CTile(nTile0, nRow0, nCol);
                scoreInfo.m_lstTile.Add(tile0);

                int nTile1 = m_lstOriginalTile[nCol][nPos1];
                CTile tile1 = new CTile(nTile1, nRow1, nCol);
                scoreInfo.m_lstTile.Add(tile1);
            }


            for (int nRow = 0; nRow < 3; nRow++)
            {
                if (nRow == scoreTable.m_nLine)
                    continue;

                List<CTile> lstTile = scoreInfo.m_lstTile.FindAll(value => value.m_nRow == nRow);
                int nCountL = 1, nCountR = 1;

                int nFirstTile = lstTile.Find(value => value.m_nCol == 0).m_nTile;
                int nLastTile = lstTile.Find(value => value.m_nCol == 3).m_nTile;
                if (nFirstTile == JOKE && nLastTile == JOKE)
                    return false;

                if(nFirstTile == JOKE)
                {
                    if (lstTile.Find(value => value.m_nCol == 1).m_nTile == JOKE)
                        return false;
                    nFirstTile = lstTile.Find(value => value.m_nCol == 1).m_nTile;
                }
                for (int nCol = 1; nCol < 4; nCol++)
                {
                    if (nFirstTile == lstTile.Find(value => value.m_nCol == nCol).m_nTile || lstTile.Find(value => value.m_nCol == nCol).m_nTile == JOKE)
                        nCountL++;
                    else
                        break;
                }

                if (nLastTile == JOKE)
                {
                    if (lstTile.Find(value => value.m_nCol == 2).m_nTile == JOKE)
                        return false;
                    nLastTile = lstTile.Find(value => value.m_nCol == 2).m_nTile;
                }
                for (int nCol = 2; nCol >= 0; nCol--)
                {
                    if (nLastTile == lstTile.Find(value => value.m_nCol == nCol).m_nTile || lstTile.Find(value => value.m_nCol == nCol).m_nTile == JOKE)
                        nCountR++;
                    else
                        break;
                }


                if (nCountL > 2 || nCountR > 2)
                    return false;

                if (nCountL == 2 && nCountR == 2)
                    return false;

                if (lstTile.Find(value => value.m_nCol == 0).m_nTile == FLOW && lstTile.Find(value => value.m_nCol == 3).m_nTile == FLOW)
                    return false;

                if (lstTile.Find(value => value.m_nCol == 0).m_nTile == FLOW && nCountR > 1)
                    return false;

                if (lstTile.Find(value => value.m_nCol == 3).m_nTile == FLOW && nCountL > 1)
                    return false;

                if (lstTile.Find(value => value.m_nCol == 0).m_nTile == FLOW && nCountL == 1)
                {
                    scoreInfo.m_lstLine.Add(nRow);
                    lstTile.Find(value => value.m_nCol == 0).m_nAct = 1;
                    scoreInfo.m_nScore += 100 * scoreInfo.m_nMulti;
                }
                else if (lstTile.Find(value => value.m_nCol == 3).m_nTile == FLOW && nCountR == 1)
                {
                    scoreInfo.m_lstLine.Add(nRow);
                    lstTile.Find(value => value.m_nCol == 3).m_nAct = 1;
                    scoreInfo.m_nScore += 100 * scoreInfo.m_nMulti;
                }
                else
                {
                    int nTile = ANNY;
                    if (nCountL > nCountR)
                    {
                        nTile = lstTile.Find(value => value.m_nCol == 0).m_nTile;
                        if (nTile == JOKE)
                            nTile = lstTile.Find(value => value.m_nCol == 1).m_nTile;
                    }
                    else
                    {
                        nTile = lstTile.Find(value => value.m_nCol == 3).m_nTile;
                        if (nTile == JOKE)
                            nTile = lstTile.Find(value => value.m_nCol == 2).m_nTile;
                    }

                    if (nTile == SEVE || nTile == STAR || nTile == TARG)
                    {
                        if (nCountL > nCountR)
                        {
                            if(nCountL >= 2)
                            {
                                scoreInfo.m_nScore += 100 * scoreInfo.m_nMulti;
                                lstTile.Find(value => value.m_nCol == 0).m_nAct = 1;
                                lstTile.Find(value => value.m_nCol == 1).m_nAct = 1;
                                scoreInfo.m_lstLine.Add(nRow);
                            }
                        }
                        else
                        {
                            if(nCountR >= 2)
                            {
                                scoreInfo.m_nScore += 100 * scoreInfo.m_nMulti;
                                lstTile.Find(value => value.m_nCol == 2).m_nAct = 1;
                                lstTile.Find(value => value.m_nCol == 3).m_nAct = 1;
                                scoreInfo.m_lstLine.Add(nRow);
                            }
                        }
                        
                    }
                    else if (nTile == FLOW)
                    {
                        if (nCountL > nCountR)
                        {
                            if(nCountL >= 2)
                            {
                                scoreInfo.m_nScore += 200 * scoreInfo.m_nMulti;
                                lstTile.Find(value => value.m_nCol == 0).m_nAct = 1;
                                lstTile.Find(value => value.m_nCol == 1).m_nAct = 1;
                                scoreInfo.m_lstLine.Add(nRow);
                            }
                        }
                        else
                        {
                            if(nCountR >= 2)
                            {
                                scoreInfo.m_nScore += 200 * scoreInfo.m_nMulti;
                                lstTile.Find(value => value.m_nCol == 2).m_nAct = 1;
                                lstTile.Find(value => value.m_nCol == 3).m_nAct = 1;
                                scoreInfo.m_lstLine.Add(nRow);
                            }
                        }
                    }
                }
            }

            if (scoreInfo.m_nScore > 20000)
                return false;


            if (scoreInfo.m_lstLine.Count == 3)
                return false;
            else
                return true;
        }
        */


        public bool SetOcaPrizeCall(int nCont, int nJackAmount)
        {
            if (m_nGearJack > 0)
                return false;
            m_prizeInfo = new COCAPrizeInfo();
            m_prizeInfo.m_nCont = nCont;
            m_prizeInfo.m_nPrizeCash = nJackAmount;

            m_lstOcaPrizeInfo.Clear();
            bool ret = MakePrizeRoll();
            return ret;
        }

        public override bool MakePrizeRoll()
        {
            m_nGearJack = 1;    //잭팟상태로 설정
            CDataBase.SaveGearInfoToDB(this);

            //잭팟시작신호를 보낸다.
            MakePrizeCommand(PRIZE_START);
            
            //가짜밤예시
            if(m_prizeInfo.m_nCont == JACK_NOISE)
            {
                MakePrizeEmpty(7);
                MakePrizeCommand(PRIZE_END);
                return true;
            }

            //물방울 출현
            MakePrizeCommand(WATER_BALL);
            if (m_prizeInfo.m_nCont == JACK_WATER)
            {
                MakePrizeEmpty(2);
                PrizeWaterBall();
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(7, true);
            //플랑크톤
            MakePrizeCommand(WATER_FLOWER);
            if (m_prizeInfo.m_nCont == JACK_FLOWER)
            {
                MakePrizeEmpty(2);
                PrizeWaterFlower();
                MakePrizeCommand(PRIZE_END);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(7, true);
            //잠수함
            MakePrizeCommand(SUBMARINE);
            MakePrizeEmpty(7, true);
            MakePrizeCommand(SUBMARINE);
            if (m_prizeInfo.m_nCont == JACK_SUBMARINE)
            {
                MakePrizeEmpty(2);
                PrizeSubmarine();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, true);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(7, true);
            //열대어
            MakePrizeCommand(BACKFISH);
            MakePrizeEmpty(7, true);
            MakePrizeCommand(BACKFISH);
            if (m_prizeInfo.m_nCont == JACK_BACkFISH)
            {
                MakePrizeEmpty(2);
                PrizeBackFish();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, true);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(7, true);
            //가오리
            MakePrizeCommand(ANIMATIC);
            MakePrizeEmpty(7, true);
            MakePrizeCommand(ANIMATIC);
            if (m_prizeInfo.m_nCont == JACK_ANIMATIC)
            {
                MakePrizeEmpty(2);
                PrizeAnimatic();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, true);
                CDataBase.SaveGearInfoToDB(this);
                return true;
            }
            MakePrizeEmpty(7, true);
            //상어
            MakePrizeCommand(SHARK);
            MakePrizeEmpty(7, true);
            MakePrizeCommand(SHARK);
            if (m_prizeInfo.m_nCont == JACK_SHARK)
            {
                MakePrizeEmpty(2);
                PrizeShark();
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

        private void PrizeWaterBall()
        {
            List<COCAScoreTable> lstTable = m_lstScoreTable.FindAll(value=>value.m_nScore <= m_prizeInfo.m_nPrizeCash && value.m_nTile > 3).OrderByDescending(value=>value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            COCAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_lstOcaPrizeInfo.Add(scoreInfo);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            MakePrizeEmpty(1);
        }

        private void PrizeWaterFlower()
        {
            List<COCAScoreTable> lstTable = m_lstScoreTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash && value.m_nTile > 3).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            COCAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_lstOcaPrizeInfo.Add(scoreInfo);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            MakePrizeEmpty(1);
        }

        private void PrizeSubmarine()
        {
            List<COCAScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            COCAScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstOcaPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }

        private void PrizeBackFish()
        {
            List<COCAScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            COCAScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstOcaPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }

        private void PrizeAnimatic()
        {
            List<COCAScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            COCAScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstOcaPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }

        private void PrizeShark()
        {
            List<COCAScoreTable> lstTable = m_lstJackTable.FindAll(value => value.m_nScore <= m_prizeInfo.m_nPrizeCash).OrderByDescending(value => value.m_nScore).ToList();
            if (lstTable.Count == 0)
                return;

            COCAScoreTable scoreTable = CGlobal.RandomSelect(lstTable.FindAll(value => value.m_nScore == lstTable[0].m_nScore));
            COCAScoreInfo scoreInfo = ConvertScoreTableToPrizeScore(scoreTable);
            m_nReturnCash += (m_prizeInfo.m_nPrizeCash - scoreInfo.m_nScore);
            int nRemScore = scoreInfo.m_nScore;
            if (scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstOcaPrizeInfo.Add(scoreInfo);
            MakeAfterPrizeScoreList(nRemScore - scoreInfo.m_nScore);
        }


        private COCAScoreInfo ConvertScoreTableToPrizeScore(COCAScoreTable scoreTable)
        {
            COCAScoreInfo scoreInfo = new COCAScoreInfo();

            int nCol = RND.Next(4);  //조커로 변하지 말아야 할 위치
            //맞은 타일을 만든다.
            for(int i=0; i<scoreTable.m_nlstTile.Length; i++)
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
                while(true)
                {
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value=>value != FLOW));
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != FLOW));
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

                    if (lstLine0[nRow] == nTile && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                        continue;
                    if (lstLine0[nRow] == JOKE && (nTile == FLOW || nTile == STAR || nTile == TARG || nTile == SEVE))
                        continue;
                    if (nTile == JOKE && (lstLine0[nRow] == STAR || lstLine0[nRow] == SEVE || lstLine0[nRow] == TARG || lstLine0[nRow] == FLOW))
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
                    

                    if (nTile == lstLine3[nRow] && (nTile == JOKE || nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (lstLine3[nRow] == JOKE && (nTile == FLOW || nTile == STAR || nTile == SEVE || nTile == TARG))
                        continue;
                    if (nTile == JOKE && (lstLine3[nRow] == FLOW || lstLine3[nRow] == STAR || lstLine3[nRow] == SEVE || lstLine3[nRow] == TARG))
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

            List<COCAScoreInfo> lstScoreInfo = new List<COCAScoreInfo>();
            while(nScore >= 100)
            {
                int nCash = 0;
                if(nScore > 50000)
                    nCash = m_lstScores.FindAll(value => value <= nScore).Max(value => value);
                else if(nScore > 20000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 5000));
                else if(nScore > 5000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 1000));
                else
                    nCash = m_lstScores.FindAll(value => value <= nScore).Max(value => value);

                COCAScoreTable scoreTable = CGlobal.RandomSelect(m_lstScoreTable.FindAll(value => value.m_nScore == nCash));
                COCAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
                lstScoreInfo.Add(scoreInfo);
                nScore -= scoreInfo.m_nScore;
            }

            MakePrizeEmpty(1);
            while (lstScoreInfo.Count > 0)
            {
                COCAScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                m_lstOcaPrizeInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                MakePrizeEmpty(1);
            }
        }
        

        public void OnFinishAnimation()
        {
            m_bPrizeAni = false;
            m_nPrizeAniCnt = 0;
        }

        private void MakePrizeCommand(int nCmd)
        {
            COCAScoreInfo scoreInfo = MakeEmptyRoll();
            scoreInfo.m_nCmd = nCmd;
            m_lstOcaPrizeInfo.Add(scoreInfo);
        }

        private void MakePrizeEmpty(int nCount, bool bPrize = false)
        {
            if(!bPrize)
                nCount = nCount + RND.Next(3);
            for (int i = 0; i < nCount; i++)
            {
                m_lstOcaPrizeInfo.Add(MakeEmptyRoll());
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
                case SUBMARINE:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]잠수함[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case BACKFISH:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]열대어[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case ANIMATIC:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]가오리[-] 출현을 축하드립니다. 대박 나세요..";
                    break;
                case SHARK:
                    strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]백상어[-] 출현을 축하드립니다. 대박 나세요..";
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
                    SetOcaPrizeCall(JACK_NOISE, 0);
                }
            }

            CGlobal.CalcShowJackPot(100, 0);
            CGlobal.GetEngine(m_nGameCode).AddTotalPot(100, m_nTakeUser);//누적금액을 사용한 금액만큼 증가시킨다.

            CDataBase.SaveGearInfoToDB(this);
            clsSocket.SendGearInfo(this);
        }

        public override void OnGearStart(int nRun)
        {
            m_nGearRun = nRun;
            if(nRun == 1)
            {
                if (m_lstOcaPrizeInfo.Exists(value => value.m_nCmd >= PRIZE_START && value.m_nCmd < PRIZE_END))
                {
                    COCAScoreInfo scoreInfo = MakeEmptyRoll();
                    scoreInfo.m_nCmd = PRIZE_START;
                    m_lstOcaPrizeInfo.Insert(0, scoreInfo);
                }
            }
        }

        public override void OnReconnect(CUserSocket clsSocket)
        {
            if(m_lstOcaPrizeInfo.Exists(value=>value.m_nCmd >= PRIZE_START && value.m_nCmd < PRIZE_END))
            {
                COCAScoreInfo scoreInfo = MakeEmptyRoll();
                scoreInfo.m_nCmd = PRIZE_START;
                m_lstOcaPrizeInfo.Insert(0, scoreInfo);
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

            if (m_lstOcaPrizeInfo.Count > 0)
            {
                //잭팟일때이다.
                if(m_bPrizeAni == false)
                {
                    m_clsSendScoreInfo = m_lstOcaPrizeInfo[0];
                    m_lstOcaPrizeInfo.RemoveAt(0);

                    if (m_lstOcaPrizeInfo.Count == 0)
                    {
                        ReturnGiftCash();
                        if(m_lstOcaReturnGift.Count == 0)
                        {
                            OnEndPrizeCall();
                        }
                    }

                    if (m_clsSendScoreInfo.m_nCmd > 1 && m_clsSendScoreInfo.m_nCmd < 8)
                    {
                        m_bPrizeAni = true;
                    }
                }
                else
                {
                    m_nPrizeAniCnt++;
                    if(m_nPrizeAniCnt > 20)
                    {
                        m_nPrizeAniCnt = 0;
                        m_bPrizeAni = false;
                    }

                    m_clsSendScoreInfo = MakeEmptyRoll();
                }
            }
            else if (m_lstOcaReturnGift.Count > 0)
            {
                m_clsSendScoreInfo = m_lstOcaReturnGift[0];
                m_lstOcaReturnGift.RemoveAt(0);
                if (m_lstOcaReturnGift.Count == 0)
                {
                    OnEndPrizeCall();
                }
            }
            else if (m_lstOcaScoreInfo.Count > 0)
            {
                m_clsSendScoreInfo = m_lstOcaScoreInfo[0];
                m_lstOcaScoreInfo.RemoveAt(0);
            }
            else
            {
                int nRollCount = m_nSlotCash / m_nSpeedCash;
                for (int i = 0; i < nRollCount; i++)
                {
                    COCAScoreInfo scoreInfo = MakeEmptyRoll();
                    m_lstOcaScoreInfo.Add(scoreInfo);
                }

                if (m_lstOcaScoreInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstOcaScoreInfo[0];
                    m_lstOcaScoreInfo.RemoveAt(0);
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
                string[] strCont = { "", "물방울 ", "플랑크톤 ", "잠수함 ", "열대어 ", "가오리 ", "백상어 " };
                string strMessage = MakeCongration(strCont[m_prizeInfo.m_nCont], m_prizeInfo.m_nPrizeCash);
                CGlobal.SendNoticeBroadCast(strMessage);

                CGlobal.CalculateJackPotCash(m_prizeInfo.m_nPrizeCash);
                OnBroadCastPrizeInfo();
            }
            
            ClearPrizeInfo();
            CDataBase.SaveGearInfoToDB(this);
        }

        public override int RaiseJackPot1(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 1000)
                return nJackCash;

            m_lstOcaPrizeInfo.Clear();
            List<COCAScoreTable> lstScoreTable = m_lstScoreTable.ToList().FindAll(value => value.m_nScore <= nJackCash && value.m_nScore < 10000).OrderByDescending(value => value.m_nScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nPrizeCash = lstScoreTable[0].m_nScore;

                if(RND.Next(5) == 3)
                {
                    int nCont = 0;

                    if (nPrizeCash <= 5000)
                        nCont = JACK_WATER;
                    else if (nPrizeCash < 10000)
                        nCont = JACK_FLOWER;

                    SetOcaPrizeCall(nCont, nPrizeCash);
                }
                else
                {
                    COCAScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(CGlobal.RandomSelect(lstScoreTable));
                    m_lstOcaPrizeInfo.Add(scoreInfo);
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
           
            if (nJackCash < 10000)
                return nJackCash;

            COCAPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            SetOcaPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            CDataBase.InsertNationalToDB(m_nGearCode, prizeCashInfo.m_nCash, 2);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, 2, 0);
            SetGiveStep(2);

            return nJackCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 10000)
                return nJackCash;

            COCAPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;

            SetOcaPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            CDataBase.InsertNationalToDB(m_nGearCode, prizeCashInfo.m_nCash, 3);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, 3, 0);
            SetGiveStep(3);

            return nJackCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 10000)
                return nJackCash;

            COCAPrizeCashInfo prizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nCash <= nJackCash).OrderByDescending(value => value.m_nCash).ToList()[0];
            nJackCash -= prizeCashInfo.m_nCash;


            bool bret = SetOcaPrizeCall(prizeCashInfo.m_nPrize, prizeCashInfo.m_nCash);
            if (bret)
            {
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                if (robot != null)
                {
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, prizeCashInfo.m_nCash, prizeCashInfo.m_nPrize, -1, 1);
                    robot.PlayOcaPrize();
                }
            }

            return nJackCash;
        }

        private void ReturnGiftCash()
        {
            m_lstOcaReturnGift.Clear();

            if (m_nReturnCash == 0)
                return;

            List<COCAScoreInfo> lstOcaScoreInfo = new List<COCAScoreInfo>();
            int nDstCash = m_nReturnCash;
            m_nReturnCash = 0;
            //점수렬을 만든다.
            List<int> lstScore = new List<int>();
            MakeReturnScoreList(nDstCash, lstScore);

            List<COCAScoreInfo> lstScoreInfo = new List<COCAScoreInfo>();
            //점수에 해당한 SeaScoreTableInfo리스트를 구한다.
            for (int i = 0; i < lstScore.Count; i++)
            {
                int nDstScore = lstScore[i]; //목표점수
                COCAScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                lstScoreInfo.Add(scoreInfo);
            }

            while (lstScoreInfo.Count > 0)
            {
                COCAScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
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
            m_lstOcaReturnGift.Clear();
            m_lstOcaReturnGift.InsertRange(0, lstOcaScoreInfo);
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

        public void OnWaterTank()
        {
            m_nLeftWater++;
            if (m_nLeftWater >= 50)
                m_nLeftWater = 0;

            CDataBase.SaveGearInfoToDB(this);
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nJackCont = 0;
            if (clsItem.m_nItemModel == CItemModel.ITEM_OCA_SUBMARINE)
            {
                nJackCont = JACK_SUBMARINE;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_OCA_BACKFISH)
            {
                nJackCont = JACK_BACkFISH;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_OCA_ANIMATIC)
            {
                nJackCont = JACK_ANIMATIC;
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_OCA_SHARK)
            {
                nJackCont = JACK_SHARK;
            }

            List<COCAPrizeCashInfo> lstPrizeCashInfo = m_lstPrizeCashInfo.FindAll(value => value.m_nPrize == nJackCont && value.m_nCash <= nItemCash);
            if (lstPrizeCashInfo == null || lstPrizeCashInfo.Count == 0)
            {
                //가짜예시이다.
                SetOcaPrizeCall(JACK_NOISE, 0);
                m_nReturnCash = nItemCash;
            }
            else
            {
                //점수에 해당한 잭팟을 주어야 한다.
                int nJackScore = lstPrizeCashInfo.Max(value => value.m_nCash);
                SetOcaPrizeCall(nJackCont, nJackScore);
                CGlobal.SetItemEngineRemCash(nItemCash - nJackScore);

                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nJackScore, nJackCont, 4, 0);
            }
        }
    }
}
