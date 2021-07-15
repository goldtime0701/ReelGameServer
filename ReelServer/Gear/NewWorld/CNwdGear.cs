using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CNwdGear : CBaseGear
    {
        private CNWDPrizeInfo m_prizeInfo;                  //잭팟정보
        private List<CNWDScoreInfo> m_lstNwdPrizeInfo;      //잭팟정보리스트
        private List<CNWDScoreInfo> m_lstNwdScoreInfo;
        private List<CNWDScoreInfo> m_lstNwdReturnGift;
        private int m_nReturnCash;                          //잭팟이 끝난다음에 돌려주어야 하는 점수
        private bool m_bReturnGift;
        private int m_nOverCash;                            //점수배렬을 만들면서 더 첨부된 점수
        private bool m_bPrizeAni;
        private int m_nNoiseCash;                           //가짜밤예시를 주기 위한 변수
        private int m_nNoiseRaise;                          //가짜잭팟발동금액

        private CNWDScoreInfo m_clsSendScoreInfo;

        public CNwdGear() : base()
        {
            m_lstNwdPrizeInfo = new List<CNWDScoreInfo>();
            m_lstNwdScoreInfo = new List<CNWDScoreInfo>();
            m_lstNwdReturnGift = new List<CNWDScoreInfo>();
            m_nSpeedCash = 100;
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
            m_lstNwdPrizeInfo.Clear();
            m_lstNwdScoreInfo.Clear();
            m_lstNwdReturnGift.Clear();
            m_prizeInfo = null;
            m_nNoiseCash = 0;
            m_bPrizeAni = false;
            m_bReturnGift = false;
        }

        public override void ClearPrizeInfo()
        {
            m_nGearJack = 0;
            m_bReturnGift = false;
            SetGiveStep(0);

            m_nNoiseCash = 0;
            m_prizeInfo = null;
            m_lstNwdPrizeInfo.Clear();
            m_lstNwdReturnGift.Clear();
            SetGiveStep(0);
            m_bPrizeAni = false;
        }

        public override void MakeNormalScorelRoll(int nAddCash, bool bFlag = true)
        {
            int rate = CGlobal.GetGameRate(m_nGameCode);
            List<CNWDScoreInfo> lstNwdScoreInfo = new List<CNWDScoreInfo>();

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

                List<CNWDScoreInfo> lstScoreInfo = new List<CNWDScoreInfo>();
                for (int i = 0; i < lstScore.Count; i++)
                {
                    int nDstScore = lstScore[i]; //목표점수
                    CNWDScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                    lstScoreInfo.Add(scoreInfo);
                }

                while (lstScoreInfo.Count > 0)
                {
                    CNWDScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                    lstNwdScoreInfo.Add(scoreInfo);
                    lstScoreInfo.Remove(scoreInfo);
                }
            }


            int nRollCount = nAddCash / m_nSpeedCash;
            int nEmptyCount = nRollCount - lstNwdScoreInfo.Count;
            for (int i = 0; i < nEmptyCount; i++)
            {
                CNWDScoreInfo scoreInfo = MakeEmptyRoll();
                int nRnd = RND.Next(5);
                if (nRnd == 3)
                    lstNwdScoreInfo.Add(scoreInfo);
                else
                {
                    int nIndex = RND.Next(lstNwdScoreInfo.Count);
                    lstNwdScoreInfo.Insert(nIndex, scoreInfo);
                }
            }

            if (bFlag)
            {
                lstNwdScoreInfo.Clear();
            }

            m_lstNwdScoreInfo.AddRange(lstNwdScoreInfo);
        }
        
        //점수를 여러개의 작은 점수들로 나눈다.
        private void MakeNormalScoreList(int nDstCash, List<int> lstScore)
        {
            int nDelta = nDstCash / 2;
            if (nDelta < 100)
                nDelta = 100;
            while (nDstCash >= 50)
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
        private CNWDScoreInfo MakeNormalScore(int nScore)
        {
            if (nScore == 0)
                return MakeEmptyRoll();

            //점수테이블리스트를 만든다.
            List<CNWDScoreTable> lstMatchScoreInfo = m_lstScoreTable.FindAll(value => value.m_nScore == nScore);
            CNWDScoreTable scoreTable = CGlobal.RandomSelect(lstMatchScoreInfo);
            CNWDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
            m_nOverCash += scoreInfo.m_nScore - nScore;

            return scoreInfo;
        }

        private CNWDScoreInfo MakeEmptyRoll()
        {
            CNWDScoreInfo scoreInfo = new CNWDScoreInfo();

            //0번째 라인을 만든다.
            List<int> lstLine0 = new List<int>();
            while (true)
            {
                lstLine0.Clear();
                int nPos = RND.Next(0, 20);
                if (m_lstOriginalTile[0][nPos] == CHAY)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                nPos++; 
                if (nPos >= m_lstOriginalTile[0].Count)
                    nPos = 0;
                if (m_lstOriginalTile[0][nPos] == CHAY)
                    continue;
                if (m_nGearJack > 0 && m_lstOriginalTile[0][nPos] == GBOX)
                    continue;

                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[0].Count)
                    nPos = 0;
                if (m_lstOriginalTile[0][nPos] == CHAY)
                    continue;
                lstLine0.Add(m_lstOriginalTile[0][nPos]);

                break;
            }


            //3번라인을 만든다.
            List<int> lstLine3 = new List<int>();
            while (true)
            {
                lstLine3.Clear();
                int nPos = RND.Next(0, 16);
                if (m_lstOriginalTile[3][nPos] == CHAY)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[3].Count)
                    nPos = 0;
                if (m_lstOriginalTile[3][nPos] == CHAY)
                    continue;
                if (m_nGearJack > 0 && m_lstOriginalTile[3][nPos] == GBOX)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                nPos++;
                if (nPos >= m_lstOriginalTile[3].Count)
                    nPos = 0;
                if (m_lstOriginalTile[3][nPos] == CHAY)
                    continue;
                lstLine3.Add(m_lstOriginalTile[3][nPos]);

                break;
            }

            //1번째 라인을 만든다.
            List<int> lstLine1 = new List<int>();
            while (true)
            {
                lstLine1.Clear();
                int nPos = RND.Next(0, 16);
                int nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[0] == nTile && (nTile == JOKE || nTile == CHAY))
                    continue;
                if (lstLine0[0] == JOKE && (nTile == CHAY || nTile == JOKE))
                    continue;
                if (nTile == JOKE && lstLine0[0] == CHAY)
                    continue;
                if (nTile == JOKE && lstLine3[0] == JOKE)
                    continue;
                
                lstLine1.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[1].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[1] == nTile && (nTile == JOKE || nTile == CHAY))
                    continue;
                if (lstLine0[1] == JOKE && (nTile == CHAY || nTile == JOKE))
                    continue;
                if (nTile == JOKE && lstLine3[1] == JOKE)
                    continue;
                if (m_nGearJack > 0 && m_lstOriginalTile[1][nPos] == GBOX)
                    continue;

                lstLine1.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[1].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[1][nPos];
                if (lstLine0[2] == nTile && (nTile == JOKE || nTile == CHAY))
                    continue;
                if (lstLine0[2] == JOKE && (nTile == CHAY || nTile == JOKE))
                    continue;
                if (nTile == JOKE && lstLine3[2] == JOKE)
                    continue;

                lstLine1.Add(nTile);

                break;
            }


            //2번라인을 만든다.
            int nTempCnt = 0;

            List<int> lstLine2 = new List<int>();
            while (true)
            {
                nTempCnt++;
                
                lstLine2.Clear();
                int nPos = RND.Next(0, 16);
                int nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[0] && (nTile == JOKE || nTile == CHAY))
                        continue;
                    if (lstLine3[0] == JOKE && (nTile == CHAY || nTile == JOKE))
                        continue;
                    if (nTile == JOKE && lstLine3[0] == CHAY)
                        continue;
                    if (nTile == JOKE && (lstLine1[0] == JOKE || lstLine0[0] == JOKE || lstLine0[0] == lstLine1[0]))
                        continue;
                    if (nTile == lstLine3[0] && (lstLine1[0] == JOKE || nTile == lstLine1[0]))
                        continue;
                    if (nTile == lstLine1[0] && (lstLine3[0] == JOKE || nTile == lstLine3[0] || lstLine0[0] == JOKE || lstLine0[0] == lstLine1[0]))
                        continue;
                    if (nTile == JOKE && lstLine1[0] == lstLine3[0])
                        continue;
                }
                
                lstLine2.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[2].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[1] && (nTile == JOKE || nTile == CHAY))
                        continue;
                    if (lstLine3[1] == JOKE && (nTile == CHAY || nTile == JOKE))
                        continue;
                    if (nTile == JOKE && lstLine3[1] == CHAY)
                        continue;
                    if (nTile == JOKE && (lstLine1[1] == JOKE || lstLine0[1] == JOKE || lstLine0[1] == lstLine1[1]))
                        continue;
                    if (nTile == lstLine3[1] && (lstLine1[1] == JOKE || nTile == lstLine1[1]))
                        continue;
                    if (nTile == lstLine1[1] && (lstLine3[1] == JOKE || nTile == lstLine3[1] || lstLine0[1] == JOKE || lstLine0[1] == lstLine1[1]))
                        continue;
                    if (nTile == JOKE && lstLine1[1] == lstLine3[1])
                        continue;
                    if (m_nGearJack > 0 && m_lstOriginalTile[2][nPos] == GBOX)
                        continue;
                }
                    
                lstLine2.Add(nTile);

                nPos++;
                if (nPos >= m_lstOriginalTile[2].Count)
                    nPos = 0;
                nTile = m_lstOriginalTile[2][nPos];
                if (nTempCnt < 5000)
                {
                    if (nTile == lstLine3[2] && (nTile == JOKE || nTile == CHAY))
                        continue;
                    if (lstLine3[2] == JOKE && (nTile == CHAY || nTile == JOKE))
                        continue;
                    if (nTile == JOKE && lstLine3[2] == CHAY)
                        continue;
                    if (nTile == JOKE && (lstLine1[2] == JOKE || lstLine0[2] == JOKE || lstLine0[2] == lstLine1[2]))
                        continue;
                    if (nTile == lstLine3[2] && (lstLine1[2] == JOKE || nTile == lstLine1[2]))
                        continue;
                    if (nTile == lstLine1[2] && (lstLine3[2] == JOKE || nTile == lstLine3[2] || lstLine0[2] == JOKE || lstLine0[2] == lstLine1[2]))
                        continue;
                    if (nTile == JOKE && lstLine1[2] == lstLine3[2])
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

        public override bool MakePrizeRoll()
        {
            m_nGearJack = 1;    //잭팟상태로 설정
            CDataBase.SaveGearInfoToDB(this);

            //잭팟시작신호를 보낸다.
            MakePrizeCommand(PRIZE_START);
            
            
            //가짜밤예시
            if (m_prizeInfo.m_nCont == JACK_CONT_NOISE)
            {
                MakeNoisePrizeRoll();
                MakePrizeCommand(PRIZE_END);
                MakePrizeEmpty(2, false);
                return true;
            }
            
            if (m_prizeInfo.m_nCont == JACK_CONT_FIREBIRD)
            {
                //불새잭팟
                if(m_prizeInfo.m_nPrizeCash >= 20 * 10000 && RND.Next(3) == 1)
                {
                    //불새 20만이상일때는 연타일 가능성도 있다.
                    MakeContinuePrizeRoll();
                }
                else
                {
                    MakeFBirdPrizeRoll();
                    MakePrizeCommand(PRIZE_FBIRD);
                    MakePrizeScore(m_prizeInfo.m_nPrizeCash);
                }
            }
            else if(m_prizeInfo.m_nCont == JACK_CONT_GODGIRL)
            {
                //녀신잭팟
                MakeGodGirlPrizeRoll();
                MakePrizeCommand(PRIZE_GIRL);
                MakePrizeScore(m_prizeInfo.m_nPrizeCash);
            }
            
            MakePrizeCommand(PRIZE_END);
            MakePrizeEmpty(2, false);
            CDataBase.SaveGearInfoToDB(this);

            return true;
        }

        private void MakePrizeCommand(int nCmd)
        {
            CNWDScoreInfo scoreInfo = MakeEmptyRoll();
            scoreInfo.m_nCmd = nCmd;
            m_lstNwdPrizeInfo.Add(scoreInfo);
        }

        private void MakeNoisePrizeRoll()
        {
            MakePrizeEmpty(1, false);
            MakePrizeCommand(PRIZE_STHUNDER); //시작하자마자 실번개를 한번 친다.
            MakePrizeEmpty(2, false);
            
            //반디불출현
            MakePrizeCommand(PRIZE_FIREFLY);
            MakePrizeEmpty(3);

            if(RND.Next(2) == 1)
            {
                MakePrizeCommand(PRIZE_FIREFLY);
                MakePrizeEmpty(10);
            }

            if (RND.Next(5) == 3)
            {
                MakePrizeCommand(PRIZE_WHALE);
                MakePrizeEmpty(10);
            }
        }

        private void MakeContinuePrizeRoll()
        {
            MakePrizeEmpty(1, false);
            MakePrizeCommand(PRIZE_STHUNDER); //시작하자마자 실번개를 한번 친다.
            MakePrizeEmpty(2);

            //요정3회이상, 고래출현
            MakePrizeCommand(PRIZE_FIREFLY); //요정출현
            MakePrizeEmpty(5);
            if (RND.Next(5) == 1)
            {
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
            }
            MakePrizeCommand(PRIZE_STHUNDER); //실번개
            MakePrizeEmpty(5);
            if (RND.Next(5) == 1)
            {
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(2);
            }
            MakePrizeCommand(PRIZE_FIREFLY); //요정출현
            MakePrizeEmpty(5);
            MakePrizeCommand(PRIZE_STHUNDER); //실번개
            if (RND.Next(3) == 1)
                MakePrizeCommand(PRIZE_BOAT); //뽀트지나가기
            MakePrizeEmpty(5);
            MakePrizeCommand(PRIZE_STHUNDER); //실번개
            MakePrizeEmpty(5);
            
            if (RND.Next(5) == 1)
            {
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(2);
            }
            if (RND.Next(2) == 1)
            {
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(2);
            }
            MakePrizeCommand(PRIZE_WHALE); //고래출현
            MakePrizeEmpty(2);
            MakePrizeCommand(PRIZE_SHARK); //상어출현
            MakePrizeCommand(PRIZE_FBIRD);

            //련타회수를 확정한다.
            int nCnt = 0;
            if (m_prizeInfo.m_nPrizeCash == 30 * 10000)
                nCnt = RND.Next(1, 3);
            else if(m_prizeInfo.m_nPrizeCash == 40 * 10000)
                nCnt = RND.Next(1, 4);
            else if (m_prizeInfo.m_nPrizeCash == 50 * 10000)
                nCnt = RND.Next(1, 5);

            int nPrizeCash = m_prizeInfo.m_nPrizeCash - (nCnt * 100000);
            MakePrizeScore(nPrizeCash);
            MakePrizeEmpty(5, false);
            for(int i=0; i<nCnt; i++)
            {
                MakePrizeCommand(PRIZE_NIGHT);
                MakePrizeEmpty(2, false);
                MakePrizeCommand(PRIZE_SHARK); //상어출현
                MakePrizeCommand(PRIZE_FBIRD);
                MakePrizeScore(10 * 10000);
                MakePrizeEmpty(5, false);
            }
        }

        private void MakeGodGirlPrizeRoll()
        {
            MakePrizeEmpty(1, false);
            MakePrizeCommand(PRIZE_STHUNDER); //시작하자마자 실번개를 한번 친다.
            MakePrizeEmpty(2);

            int nRnd = RND.Next(3);
            if(nRnd == 0)
            {
                //밤이와서 3천정도 먹고 우번3회, 쌍밤배출현, 7천 안에서 바로 상어출현시 여신확률 업
                MakePrizeEmpty(5);
                if(RND.Next(2) == 1)
                    MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);

                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(4);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(5);
                }
                if (RND.Next(2) == 1)
                    MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(6);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(5);
                }
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(5);
                }
                MakePrizeCommand(PRIZE_BOAT); //뽀트지나가기
                MakePrizeEmpty(RND.Next(50, 60));
            }
            else if(nRnd == 1)
            {
                //밤이 길고 우번4회이상(좌번이 절대 없음)여신출현 확률 업
                MakePrizeEmpty(RND.Next(10, 20));
                if (RND.Next(2) == 1)
                    MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(10);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(RND.Next(10, 30));
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(RND.Next(10, 30));
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(RND.Next(10, 20));
                if (RND.Next(2) == 1)
                    MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(10);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(RND.Next(10, 30));
            }
            else if(nRnd == 2)
            {
                //밤이오고 3천안에 바로 상어출현시 여신출현
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                }
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_STHUNDER); //실번개
                }
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                }
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_STHUNDER); //실번개
                }
                MakePrizeEmpty(5);
            }

            MakePrizeCommand(PRIZE_SHARK); //상어출현
        }

        private void MakeFBirdPrizeRoll()
        {
            MakePrizeEmpty(1, false);
            MakePrizeCommand(PRIZE_STHUNDER); //시작하자마자 실번개를 한번 친다.
            MakePrizeEmpty(2);

            int nRnd = RND.Next(3);
            if(nRnd == 0)
            {
                //요정3회, 우측실번개3회
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(5);
                }
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(2);
                }
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                if(RND.Next(3) == 1)
                    MakePrizeCommand(PRIZE_BOAT); //뽀트지나가기
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(2);
                }
            }
            else if(nRnd == 1)
            {
                //요정3회, 좌번개 7회
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_STHUNDER); //실번개
                    MakePrizeEmpty(2);
                }
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                if (RND.Next(3) == 1)
                    MakePrizeCommand(PRIZE_BOAT); //뽀트지나가기
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(4);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_STHUNDER); //실번개
                    MakePrizeEmpty(2);
                }
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_STHUNDER); //실번개
                    MakePrizeEmpty(5);
                }
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(4);
            }
            else if(nRnd == 2)
            {
                //좌우번개 7번이상
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(5);
                }
                MakePrizeCommand(PRIZE_FIREFLY); //요정출현
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(5);
                if (RND.Next(3) == 1)
                    MakePrizeCommand(PRIZE_BOAT); //뽀트지나가기
                MakePrizeEmpty(4);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(5);
                if (RND.Next(5) == 1)
                {
                    MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                    MakePrizeEmpty(5);
                }
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_STHUNDER); //실번개
                MakePrizeEmpty(5);
                MakePrizeCommand(PRIZE_LTHUNDER); //큰번개
                MakePrizeEmpty(5);
            }
            MakePrizeCommand(PRIZE_SHARK); //상어출현
        }

        private CNWDScoreInfo MakePrizeOneEmpty()
        {
            CNWDScoreInfo scoreInfo = MakeEmptyRoll();
            if (RND.Next(3) == 2)
            {
                int nRnd = RND.Next(5);
                if(m_prizeInfo != null)
                {
                    if (nRnd == 3 && m_lstNwdPrizeInfo.Count(value => value.m_nCmd == PRIZE_STHUNDER) < 4 && m_prizeInfo.m_nCont == JACK_CONT_NOISE)
                    {
                        MakePrizeCommand(PRIZE_STHUNDER);
                    }
                    else if (nRnd == 4 && m_lstNwdPrizeInfo.Count(value => value.m_nCmd == PRIZE_LTHUNDER) < 7 && m_prizeInfo.m_nCont == JACK_CONT_NOISE)
                    {
                        MakePrizeCommand(PRIZE_LTHUNDER);
                    }
                    else
                    {
                        scoreInfo.m_nCmd = CGlobal.RandomSelect(m_nlstPrizeEmptyRoll.ToList());
                    }
                }
            }

            return scoreInfo;
        }

        private void MakePrizeEmpty(int nCount, bool bNight = true)
        {
            nCount = nCount + RND.Next(3);
            for (int i = 0; i < nCount; i++)
            {
                if (bNight)
                    m_lstNwdPrizeInfo.Add(MakePrizeOneEmpty());
                else
                    m_lstNwdPrizeInfo.Add(MakeEmptyRoll());
            }
        }

        private void MakePrizeScore(int nPrizeCash)
        {
            CNWDScoreTable scoreTable = CGlobal.RandomSelect(m_lstJackTable.FindAll(value => value.m_nScore == nPrizeCash));
            CNWDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);

            if(scoreInfo.m_nMulti > 1)
            {
                CNWDScoreTable scoreTable1 = CGlobal.RandomSelect(m_lstMultiTable.FindAll(value => value.m_nCount == scoreInfo.m_nMulti));
                CNWDScoreInfo scoreInfo1 = ConvertScoreTableToScoreInfo(scoreTable1);
                m_lstNwdPrizeInfo.Add(scoreInfo1);
                m_nOverCash += scoreInfo1.m_nScore;
                MakePrizeEmpty(1);
            }
            scoreInfo.m_nCmd = PRIZE_SCORE;
            if(scoreInfo.m_nScore > 20000)
                scoreInfo.m_nScore = 20000;
            m_lstNwdPrizeInfo.Add(scoreInfo);
            MakePrizeCommand(PRIZE_DAY);
            MakeAfterPrizeScoreList(nPrizeCash - scoreInfo.m_nScore);
        }

        private void MakeAfterPrizeScoreList(int nScore)
        {
            if (nScore == 0)
                return;

            List<CNWDScoreInfo> lstScoreInfo = new List<CNWDScoreInfo>();
            while (nScore >= 50)
            {
                int nCash = 0;
                if (nScore >= 7500)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value >= 7500 && value <= nScore));
                else if (nScore > 5000)
                    nCash = CGlobal.RandomSelect(m_lstScores.FindAll(value => value <= nScore && value >= 1000));
                else
                    nCash = m_lstScores.FindAll(value => value <= nScore).Max(value => value);

                List<CNWDScoreTable> lst = m_lstScoreTable.FindAll(value => value.m_nScore == nCash && value.m_nCount == 4);
                if(lst == null || lst.Count == 0)
                    lst = m_lstScoreTable.FindAll(value => value.m_nScore == nCash);
                CNWDScoreTable scoreTable = CGlobal.RandomSelect(lst);
                CNWDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(scoreTable);
                lstScoreInfo.Add(scoreInfo);
                nScore -= scoreInfo.m_nScore;
            }

            MakePrizeEmpty(1, false);
            while (lstScoreInfo.Count > 0)
            {
                CNWDScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
                m_lstNwdPrizeInfo.Add(scoreInfo);
                lstScoreInfo.Remove(scoreInfo);
                MakePrizeEmpty(1, false);
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


            string strMessage = string.Empty;
            string strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);

            if(m_prizeInfo.m_nCont != 3)
            {
                string[] strCont = { "", "불새", "여신" };
                strMessage = MakeCongration(strCont[m_prizeInfo.m_nCont], m_prizeInfo.m_nPrizeCash);
                OnBroadCastPrizeInfo();
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
                m_nAccuCash += 100; 
                m_nNoiseCash += 100;

                if (m_nNoiseCash >= m_nNoiseRaise)
                {
                    m_nNoiseCash = 0;
                    if(m_nGearJack == 0)
                    {
                        m_nNoiseRaise = RND.Next(800, 2000);
                        m_nNoiseRaise *= 10;
                        SetNwdPrizeCall(JACK_CONT_NOISE, 0);
                    }
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
            if (nRun == 1)
            {
                if (m_lstNwdPrizeInfo.Exists(value => value.m_nCmd > 0 && value.m_nCmd != PRIZE_END))
                {
                    CNWDScoreInfo scoreInfo = MakeEmptyRoll();
                    scoreInfo.m_nCmd = PRIZE_START;
                    m_lstNwdPrizeInfo.Insert(0, scoreInfo);
                }
            }
        }

        public override void OnReconnect(CUserSocket clsSocket)
        {
            if(m_lstNwdPrizeInfo.Exists(value=>value.m_nCmd > 0 && value.m_nCmd != PRIZE_END))
            {
                CNWDScoreInfo scoreInfo = MakeEmptyRoll();
                scoreInfo.m_nCmd = PRIZE_START;
                m_lstNwdPrizeInfo.Insert(0, scoreInfo);
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
            }
        }

        public override void OnStartSpin(CUserSocket clsSocket, int nSpinCash = 100)
        {
            if (m_clsSendScoreInfo != null && m_clsSendScoreInfo.m_nScore > 0)
            {
                OnAddGiftCash(clsSocket, m_clsSendScoreInfo.m_nScore);
            }

            if (m_lstNwdPrizeInfo.Count > 0)
            {
                //잭팟일때이다.
                if (m_bPrizeAni == false)
                {
                    m_clsSendScoreInfo = m_lstNwdPrizeInfo[0];
                    m_lstNwdPrizeInfo.RemoveAt(0);

                    if (m_lstNwdPrizeInfo.Count == 0)
                    {
                        ReturnGiftCash();
                        if (m_lstNwdReturnGift.Count == 0)
                        {
                            OnEndPrizeCall();
                        }
                    }
                    if (m_clsSendScoreInfo.m_nCmd == PRIZE_SHARK || m_clsSendScoreInfo.m_nCmd == PRIZE_GIRL || m_clsSendScoreInfo.m_nCmd == PRIZE_FBIRD)
                    {
                        m_bPrizeAni = true;
                    }
                }
                else
                {
                    m_clsSendScoreInfo = MakePrizeOneEmpty();
                }
            }
            else if (m_lstNwdReturnGift.Count > 0)
            {
                m_clsSendScoreInfo = m_lstNwdReturnGift[0];
                m_lstNwdReturnGift.RemoveAt(0);
                if (m_lstNwdReturnGift.Count == 0)
                {
                    OnEndPrizeCall();
                }
            }
            else if (m_lstNwdScoreInfo.Count > 0)
            {
                m_clsSendScoreInfo = m_lstNwdScoreInfo[0];
                m_lstNwdScoreInfo.RemoveAt(0);
            }
            else
            {
                int nRollCount = m_nSlotCash / m_nSpeedCash;
                for (int i = 0; i < nRollCount; i++)
                {
                    CNWDScoreInfo scoreInfo = MakeEmptyRoll();
                    m_lstNwdScoreInfo.Add(scoreInfo);
                }

                if (m_lstNwdScoreInfo.Count > 0)
                {
                    m_clsSendScoreInfo = m_lstNwdScoreInfo[0];
                    m_lstNwdScoreInfo.RemoveAt(0);
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

            if(m_prizeInfo.m_nPrizeCash > 0)
            {
                CGlobal.CalculateJackPotCash(m_prizeInfo.m_nPrizeCash);
                OnBroadCastPrizeKind();
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

            m_lstNwdPrizeInfo.Clear();
            List<CNWDScoreTable> lstScoreTable = m_lstScoreTable.ToList().FindAll(value => value.m_nScore <= nJackCash && value.m_nScore <= 5000).OrderByDescending(value => value.m_nScore).ToList();
            if (lstScoreTable.Count > 0)
            {
                int nPrizeCash = lstScoreTable[0].m_nScore;

                CNWDScoreInfo scoreInfo = ConvertScoreTableToScoreInfo(CGlobal.RandomSelect(lstScoreTable));
                m_lstNwdPrizeInfo.Add(scoreInfo);
                MakePrizeEmpty(1);

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

            if (nJackCash < 10 * 10000)
                return nJackCash;

            int nPrizeCash = 0;
            int nCont = 0;
            if (nJackCash < 50 * 10000)
            {
                nCont = JACK_CONT_FIREBIRD;
                nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nJackCash).Max();
            }
            else if (nJackCash >= 50 * 10000)
            {
                nCont = JACK_CONT_GODGIRL;
                nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nJackCash).Max();
            }

            nJackCash -= nPrizeCash;

            SetNwdPrizeCall(nCont, nPrizeCash);
            CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 2);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, nCont, 2, 0);
            SetGiveStep(2);

            return nJackCash;
        }

        public override int RaiseJackPot3(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 10 * 10000)
                return nJackCash;

            int nPrizeCash = 0;
            int nCont = 0;
            if (nJackCash < 50 * 10000)
            {
                nCont = JACK_CONT_FIREBIRD;
                nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nJackCash).Max();
            }
            else if (nJackCash >= 50 * 10000)
            {
                nCont = JACK_CONT_GODGIRL;
                nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nJackCash).Max();
            }

            nJackCash -= nPrizeCash;

            SetNwdPrizeCall(nCont, nPrizeCash);
            CDataBase.InsertNationalToDB(m_nGearCode, nPrizeCash, 2);
            CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, nCont, 2, 0);
            SetGiveStep(3);

            return nJackCash;
        }

        public override int RaiseJackPotR(int nJackCash)
        {
            if (m_nGearJack > 0)
                return nJackCash;

            if (nJackCash < 10 * 10000)
                return nJackCash;

            int nPrizeCash = 0;
            int nCont = 0;
            if (nJackCash < 50 * 10000)
            {
                nCont = JACK_CONT_FIREBIRD;
                nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nJackCash).Max();
            }
            else if (nJackCash >= 50 * 10000)
            {
                nCont = JACK_CONT_GODGIRL;
                nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nJackCash).Max();
            }

            nJackCash -= nPrizeCash;


            bool bret = SetNwdPrizeCall(nCont, nPrizeCash);
            if (bret)
            {
                CRobot robot = CGlobal.GetRobotByCode(m_nTakeRobot);
                if (robot != null)
                {
                    CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeRobot, m_nGearCode, nPrizeCash, nCont, -1, 1);
                    robot.PlayNwdPrize();
                }
            }

            return nJackCash;
        }

        private void ReturnGiftCash()
        {
            m_lstNwdReturnGift.Clear();

            if (m_nReturnCash == 0)
                return;

            List<CNWDScoreInfo> lstOcaScoreInfo = new List<CNWDScoreInfo>();
            int nDstCash = m_nReturnCash;
            m_nReturnCash = 0;
            //점수렬을 만든다.
            List<int> lstScore = new List<int>();
            MakeReturnScoreList(nDstCash, lstScore);

            List<CNWDScoreInfo> lstScoreInfo = new List<CNWDScoreInfo>();
            //점수에 해당한 SeaScoreTableInfo리스트를 구한다.
            for (int i = 0; i < lstScore.Count; i++)
            {
                int nDstScore = lstScore[i]; //목표점수
                CNWDScoreInfo scoreInfo = MakeNormalScore(nDstScore);
                lstScoreInfo.Add(scoreInfo);
            }

            while (lstScoreInfo.Count > 0)
            {
                CNWDScoreInfo scoreInfo = CGlobal.RandomSelect(lstScoreInfo);
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
            m_lstNwdReturnGift.Clear();
            m_lstNwdReturnGift.InsertRange(0, lstOcaScoreInfo);
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

        private CNWDScoreInfo ConvertScoreTableToScoreInfo(CNWDScoreTable scoreTable)
        {
            CNWDScoreInfo scoreInfo = MakeScoreInfoFromScoreTable(scoreTable);
            return scoreInfo;
        }

        private CNWDScoreInfo MakeScoreInfoFromScoreTable(CNWDScoreTable scoreTable)
        {
            CNWDScoreInfo scoreInfo = new CNWDScoreInfo();

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
                    List<int> lstTiles = m_lstTiles.ToList().FindAll(value => value != scoreTable.m_nTile && value > 0);
                    if (i == 0 || i == 3)
                        nTile = CGlobal.RandomSelect(lstTiles.FindAll(value => value != CHAY));
                    else
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != CHAY));
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
                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != CHAY));
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

                    if (lstLine0[nRow] == nTile && (nTile == JOKE || nTile == CHAY))
                        continue;
                    if (lstLine0[nRow] == JOKE && (nTile == CHAY || nTile == JOKE))
                        continue;
                    if (nTile == JOKE && lstLine0[nRow] == CHAY)
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
                    if(nTempCnt > 5000)
                    {
                        break;
                    }

                    nTile = CGlobal.RandomSelect(m_lstTiles.ToList());
                    

                    if (nTile == lstLine3[nRow] && (nTile == JOKE || nTile == CHAY))
                        continue;
                    if (lstLine3[nRow] == JOKE && (nTile == CHAY || nTile == JOKE))
                        continue;
                    if (nTile == JOKE && lstLine3[nRow] == CHAY)
                        continue;
                    if (nTile == JOKE && (lstLine1[nRow] == JOKE || lstLine0[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine3[nRow] && (lstLine1[nRow] == JOKE || nTile == lstLine1[nRow]))
                        continue;
                    if (nTile == lstLine1[nRow] && (lstLine3[nRow] == JOKE || nTile == lstLine3[nRow] || lstLine0[nRow] == JOKE || lstLine0[nRow] == lstLine1[nRow]))
                        continue;
                    if (nTile == JOKE && lstLine1[nRow] == lstLine3[nRow])
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

            if (scoreInfo.m_nWinCnt >= 3 && scoreInfo.m_lstLine[0] == MID && scoreInfo.m_nWinTile > JOKE && scoreInfo.m_nWinTile <= GBOX && !scoreInfo.m_lstTile.Exists(value => value.m_nAct == 1 && value.m_nTile == JOKE))
            {
                if (RND.Next(2) == 0)
                {
                    if (scoreInfo.m_nWinCnt == 3)
                    {
                        if (scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 0).m_nAct == 1)
                        {
                            int nTile0 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 0 && value.m_nCol == 2).m_nTile;
                            int nTile1 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 2 && value.m_nCol == 2).m_nTile;
                            int nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != scoreInfo.m_nWinTile && value != JOKE && value != nTile0 && value != nTile1));
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 2).m_nTile = nTile;
                        }
                        else if (scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 3).m_nAct == 1)
                        {
                            int nTile0 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 0 && value.m_nCol == 1).m_nTile;
                            int nTile1 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 2 && value.m_nCol == 1).m_nTile;
                            int nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != scoreInfo.m_nWinTile && value != JOKE && value != nTile0 && value != nTile1));
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 1).m_nTile = nTile;
                        }
                    }
                    else if (scoreInfo.m_nWinCnt == 4)
                    {
                        if (RND.Next(2) == 1)
                        {
                            int nTile0 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 0 && value.m_nCol == 2).m_nTile;
                            int nTile1 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 2 && value.m_nCol == 2).m_nTile;
                            int nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != scoreInfo.m_nWinTile && value != JOKE && value != nTile0 && value != nTile1));
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 2).m_nTile = nTile;
                        }
                        else
                        {
                            int nTile0 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 0 && value.m_nCol == 1).m_nTile;
                            int nTile1 = scoreInfo.m_lstTile.Find(value => value.m_nRow == 2 && value.m_nCol == 1).m_nTile;
                            int nTile = CGlobal.RandomSelect(m_lstTiles.ToList().FindAll(value => value != scoreInfo.m_nWinTile && value != JOKE && value != nTile0 && value != nTile1));
                            scoreInfo.m_lstTile.Find(value => value.m_nRow == MID && value.m_nCol == 1).m_nTile = nTile;
                        }
                    }
                }
            }

            return scoreInfo;
        }

        public List<CNWDScoreInfo> GetPrizeScoreInfoList()
        {
            return m_lstNwdPrizeInfo;
        }

        public void OnWaterTank(int nLR)
        {
            if(nLR == 0)
            {
                m_nLeftWater++;
                if (m_nLeftWater >= 30)
                    m_nLeftWater = 0;
            }
            else if (nLR == 1)
            {
                m_nRightWater++;
                if (m_nRightWater >= 30)
                    m_nRightWater = 0;
            }

            CDataBase.SaveGearInfoToDB(this);
        }

        public void OnFinishAnimation(int nKind)
        {
            m_bPrizeAni = false;

            string strMessage = string.Empty;

            string strUserNick = CGlobal.GetUserNickByCode(m_nTakeUser);

            if (nKind == 2)
                strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]불새[-] 출현을 축하드립니다. 대박 나세요..";
            else if(nKind == 3)
                strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]여신[-] 출현을 축하드립니다. 대박 나세요..";
          

            if(nKind > 1)
            {
                CGlobal.SendNoticeBroadCast(strMessage);
            }

        }

        public bool SetNwdPrizeCall(int nJackCont, int nJackAmount)
        {
            if (nJackCont == 1 && !m_nlstFireBirdScore.ToList().Exists(value => value == nJackAmount))
                return false;
            if (nJackCont == 2 && !m_nlstGodGirl.ToList().Exists(value => value == nJackAmount))
                return false;

            m_prizeInfo = new CNWDPrizeInfo();
            m_prizeInfo.m_nCont = nJackCont;
            m_prizeInfo.m_nPrizeCash = nJackAmount;

            return MakePrizeRoll();
        }

        public CNWDPrizeInfo GetPrizeInfo()
        {
            return m_prizeInfo;
        }

        public override void UseItemByUser(CItem clsItem)
        {
            int nItemCash = CGlobal.UseItemByUser(clsItem);
            int nJackCont = 0;
            int nPrizeCash = 0;
            if (clsItem.m_nItemModel == CItemModel.ITEM_NWD_FBIRD)
            {
                nJackCont = JACK_CONT_FIREBIRD;
                if (nItemCash < 10 * 10000)
                    nJackCont = JACK_CONT_NOISE;
                else
                    nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nItemCash).Max();
            }
            else if (clsItem.m_nItemModel == CItemModel.ITEM_NWD_GIRL)
            {
                nJackCont = JACK_CONT_GODGIRL;
                if (nItemCash < 50 * 10000)
                    nJackCont = JACK_CONT_NOISE;
                else
                    nPrizeCash = m_nlstFireBirdScore.ToList().FindAll(value => value <= nItemCash).Max();
            }
            
            if(nJackCont == JACK_CONT_NOISE)
            {
                SetNwdPrizeCall(nJackCont, 0);
                m_nReturnCash = nItemCash;
            }
            else
            {
                SetNwdPrizeCall(nJackCont, nPrizeCash);
                CDataBase.InsertJackpotToDB(m_nGameCode, 0, m_nTakeUser, m_nGearCode, nPrizeCash, nJackCont, 4, 0);

                CGlobal.SetItemEngineRemCash(nItemCash - nPrizeCash);
            }
        }
    }
}
