using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CDVCScoreInfo
    {
        public int m_nGearCode;                 //기계코드
        public int m_nScore;                    //당첨점수
        public int m_nMulti;                    //배수
        public int m_nCmd;                      //명령코드
        public int m_nWinTile;                  //당첨타일
        public int m_nWinCnt;                   //당첨타일개수
        public List<CTile> m_lstTile;           //타일배렬
        public List<int> m_lstLine;             //당첨된라인

        public CDVCScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstLine = new List<int>();
        }
    }

    public class CDVCScoreTable
    {
        public int m_nMulti;
        public int m_nScore;
        public int m_nLine;
        public int[] m_nlstTile;
        public int m_nCount;
        public int m_nTile;

        public CDVCScoreTable(int nMulti, int nScore, int nLine, int[] nlstTile, int nTile, int nCount)
        {
            m_nMulti = nMulti;
            m_nScore = nScore * nMulti;
            m_nLine = nLine;
            m_nlstTile = nlstTile;
            m_nCount = nCount;
            m_nTile = nTile;
        }
    }

    public class CDVCPrizeInfo
    {
        public int m_nCont;         //1-보석, 2-글라이더, 3-지구본, 4-모나리자, 5-황금박쥐, 6-불새
        public int m_nPrizeCash;    //잭팟점수
    }

    public class CDVCPrizeCashInfo
    {
        public int m_nPrize;
        public int m_nCash;

        public CDVCPrizeCashInfo(int nPrize, int nCash)
        {
            m_nPrize = nPrize;
            m_nCash = nCash;
        }
    }

    partial class CDvcGear
    {
        private const int JACK_DAYA = 0x01;
        private const int JACK_AIRPLANE = 0x02;
        private const int JACK_EARTH = 0x03;
        private const int JACK_MOZARINA = 0x04;
        private const int JACK_GOLDBAT = 0x05;
        private const int JACK_FIREBIRD = 0x06;
        private const int JACK_NOISE = 0x07;

        public const int PRIZE_START = 0x01;
        public const int PRIZE_END = 0x02;
        public const int PRIZE_NIGHT = 0x03;
        public const int PRIZE_DAY = 0x04;
        public const int PRIZE_DAYA = 0x05;
        public const int PRIZE_AIRPLANE = 0x06;
        public const int PRIZE_EARTH = 0x07;
        public const int PRIZE_MOZARINA = 0x08;
        public const int PRIZE_BAT = 0x09;
        public const int PRIZE_FBIRD = 0x0A;

        private const int TOP = 2;  //상
        private const int MID = 1;  //중
        private const int BOT = 0;  //하

        private const int MULTI_1 = 1;
        private const int MULTI_2 = 2;
        private const int MULTI_3 = 3;
        private const int MULTI_4 = 4;
        private const int MULTI_5 = 5;

        private const int COUNT_1 = 1;
        private const int COUNT_2 = 2;
        private const int COUNT_3 = 3;
        private const int COUNT_4 = 4;

        private const int ANNY = -1;
        private const int JOKE = 0;
        private const int BAR3 = 1;
        private const int BAR2 = 2;
        private const int BAR1 = 3;
        private const int SEVN = 4;
        private const int STAR = 5;
        private const int TAGT = 6;
        private const int BELL = 7;
        private const int BIRD = 8;
        private const int GOLD = 9;
        private const int DAYA = 10;
        private const int BARC = 11;

        private int[] m_lstTiles = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private List<int>[] m_lstOriginalTile = {
            new List<int>(), new List<int>(), new List<int>(), new List<int>()
        };
        private List<int> m_lstMulti = new List<int>();
        private List<int> m_lstScores = new List<int>();
        private List<CDVCScoreTable> m_lstScoreTable = new List<CDVCScoreTable>();
        private List<CDVCScoreTable> m_lstJackTable = new List<CDVCScoreTable>();
        private List<CDVCPrizeCashInfo> m_lstPrizeCashInfo = new List<CDVCPrizeCashInfo>();

        public override void InitScoreTableInfo()
        {
            #region 오리지널릴배렬
            m_lstOriginalTile[0].Add(DAYA);
            m_lstOriginalTile[0].Add(BIRD);
            m_lstOriginalTile[0].Add(JOKE);
            m_lstOriginalTile[0].Add(BAR2);
            m_lstOriginalTile[0].Add(BELL);
            m_lstOriginalTile[0].Add(BAR1);
            m_lstOriginalTile[0].Add(GOLD);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(BAR3);
            m_lstOriginalTile[0].Add(SEVN);
            m_lstOriginalTile[0].Add(TAGT);

            m_lstOriginalTile[1].Add(BIRD);
            m_lstOriginalTile[1].Add(BAR2);
            m_lstOriginalTile[1].Add(DAYA);
            m_lstOriginalTile[1].Add(STAR);
            m_lstOriginalTile[1].Add(GOLD);
            m_lstOriginalTile[1].Add(BAR3);
            m_lstOriginalTile[1].Add(BAR1);
            m_lstOriginalTile[1].Add(TAGT);
            m_lstOriginalTile[1].Add(JOKE);
            m_lstOriginalTile[1].Add(BELL);
            m_lstOriginalTile[1].Add(SEVN);

            m_lstOriginalTile[2].Add(TAGT);
            m_lstOriginalTile[2].Add(BELL);
            m_lstOriginalTile[2].Add(BAR1);
            m_lstOriginalTile[2].Add(DAYA);
            m_lstOriginalTile[2].Add(JOKE);
            m_lstOriginalTile[2].Add(SEVN);
            m_lstOriginalTile[2].Add(BIRD);
            m_lstOriginalTile[2].Add(STAR);
            m_lstOriginalTile[2].Add(BAR2);
            m_lstOriginalTile[2].Add(GOLD);
            m_lstOriginalTile[2].Add(BAR3);

            m_lstOriginalTile[3].Add(GOLD);
            m_lstOriginalTile[3].Add(SEVN);
            m_lstOriginalTile[3].Add(JOKE);
            m_lstOriginalTile[3].Add(TAGT);
            m_lstOriginalTile[3].Add(BAR3);
            m_lstOriginalTile[3].Add(BAR1);
            m_lstOriginalTile[3].Add(BELL);
            m_lstOriginalTile[3].Add(BAR2);
            m_lstOriginalTile[3].Add(DAYA);
            m_lstOriginalTile[3].Add(STAR);
            m_lstOriginalTile[3].Add(BIRD);
            #endregion

            #region 배당
            m_lstMulti.Add(1);
            m_lstMulti.Add(2);
            m_lstMulti.Add(3);
            m_lstMulti.Add(4);
            m_lstMulti.Add(5);
            #endregion

            #region 점수테이블표
            foreach (int nMulti in m_lstMulti)
            {
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, BOT, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 3000, MID, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, TOP, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, BOT, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 3000, MID, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, TOP, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1250, BOT, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2500, MID, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1250, TOP, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1250, BOT, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2500, MID, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1250, TOP, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, BOT, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2000, MID, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, TOP, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, BOT, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2000, MID, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, TOP, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, BOT, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2000, MID, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, TOP, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, BOT, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, MID, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, TOP, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, BOT, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, MID, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, TOP, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 05000, BOT, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 10000, MID, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 05000, TOP, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { SEVN, SEVN, ANNY, ANNY }, SEVN, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { SEVN, SEVN, ANNY, ANNY }, SEVN, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { SEVN, SEVN, ANNY, ANNY }, SEVN, 2));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, SEVN, SEVN }, SEVN, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, SEVN, SEVN }, SEVN, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, SEVN, SEVN }, SEVN, 2));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 3750, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 7000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 3750, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2500, BOT, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 5000, MID, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2500, TOP, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { TAGT, TAGT, TAGT, ANNY }, TAGT, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { TAGT, TAGT, TAGT, ANNY }, TAGT, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { TAGT, TAGT, TAGT, ANNY }, TAGT, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { ANNY, TAGT, TAGT, TAGT }, TAGT, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { ANNY, TAGT, TAGT, TAGT }, TAGT, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { ANNY, TAGT, TAGT, TAGT }, TAGT, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { TAGT, TAGT, ANNY, ANNY }, TAGT, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { TAGT, TAGT, ANNY, ANNY }, TAGT, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { TAGT, TAGT, ANNY, ANNY }, TAGT, 2));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, TAGT, TAGT }, TAGT, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, TAGT, TAGT }, TAGT, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, TAGT, TAGT }, TAGT, 2));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1250, BOT, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 2500, MID, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1250, TOP, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, BOT, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, MID, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, TOP, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, BOT, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, MID, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, TOP, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, BOT, new int[] { BIRD, BIRD, BIRD, BIRD }, BIRD, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1500, MID, new int[] { BIRD, BIRD, BIRD, BIRD }, BIRD, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0750, TOP, new int[] { BIRD, BIRD, BIRD, BIRD }, BIRD, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0250, BOT, new int[] { BIRD, BIRD, BIRD, ANNY }, BIRD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, MID, new int[] { BIRD, BIRD, BIRD, ANNY }, BIRD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0250, TOP, new int[] { BIRD, BIRD, BIRD, ANNY }, BIRD, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0250, BOT, new int[] { ANNY, BIRD, BIRD, BIRD }, BIRD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, MID, new int[] { ANNY, BIRD, BIRD, BIRD }, BIRD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0250, TOP, new int[] { ANNY, BIRD, BIRD, BIRD }, BIRD, 3));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, BOT, new int[] { GOLD, GOLD, GOLD, GOLD }, GOLD, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 1000, MID, new int[] { GOLD, GOLD, GOLD, GOLD }, GOLD, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0500, TOP, new int[] { GOLD, GOLD, GOLD, GOLD }, GOLD, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, BOT, new int[] { GOLD, GOLD, GOLD, ANNY }, GOLD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0400, MID, new int[] { GOLD, GOLD, GOLD, ANNY }, GOLD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, TOP, new int[] { GOLD, GOLD, GOLD, ANNY }, GOLD, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, BOT, new int[] { ANNY, GOLD, GOLD, GOLD }, GOLD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0400, MID, new int[] { ANNY, GOLD, GOLD, GOLD }, GOLD, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, TOP, new int[] { ANNY, GOLD, GOLD, GOLD }, GOLD, 3));


                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0400, BOT, new int[] { DAYA, DAYA, DAYA, DAYA }, DAYA, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0800, MID, new int[] { DAYA, DAYA, DAYA, DAYA }, DAYA, 4));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0400, TOP, new int[] { DAYA, DAYA, DAYA, DAYA }, DAYA, 4));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, BOT, new int[] { DAYA, DAYA, DAYA, ANNY }, DAYA, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0400, MID, new int[] { DAYA, DAYA, DAYA, ANNY }, DAYA, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, TOP, new int[] { DAYA, DAYA, DAYA, ANNY }, DAYA, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, BOT, new int[] { ANNY, DAYA, DAYA, DAYA }, DAYA, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0400, MID, new int[] { ANNY, DAYA, DAYA, DAYA }, DAYA, 3));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { ANNY, DAYA, DAYA, DAYA }, DAYA, 3));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0150, BOT, new int[] { DAYA, DAYA, ANNY, ANNY }, DAYA, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0300, MID, new int[] { DAYA, DAYA, ANNY, ANNY }, DAYA, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0150, TOP, new int[] { DAYA, DAYA, ANNY, ANNY }, DAYA, 2));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0150, BOT, new int[] { ANNY, ANNY, DAYA, DAYA }, DAYA, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0300, MID, new int[] { ANNY, ANNY, DAYA, DAYA }, DAYA, 2));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0150, TOP, new int[] { ANNY, ANNY, DAYA, DAYA }, DAYA, 2));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { DAYA, ANNY, ANNY, ANNY }, DAYA, 1));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { DAYA, ANNY, ANNY, ANNY }, DAYA, 1));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { DAYA, ANNY, ANNY, ANNY }, DAYA, 1));

                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, ANNY, DAYA }, DAYA, 1));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, ANNY, DAYA }, DAYA, 1));
                m_lstScoreTable.Add(new CDVCScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, ANNY, DAYA }, DAYA, 1));



                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));

                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 10 * 10000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 05 * 10000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 05 * 10000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));

                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 50000, MID, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25000, BOT, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25000, TOP, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));

                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 25000, MID, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 12500, BOT, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 12500, TOP, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));

                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 05000, BOT, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 10000, MID, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 05000, TOP, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));

                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 3750, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 7000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 3750, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 2500, BOT, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 5000, MID, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstJackTable.Add(new CDVCScoreTable(nMulti, 2500, TOP, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
            }

            #endregion
            m_lstScoreTable = m_lstScoreTable.FindAll(value => value.m_nScore <= 20000);
            foreach (CDVCScoreTable table in m_lstScoreTable)
            {
                if (!m_lstScores.Exists(value => value == table.m_nScore) && table.m_nScore <= 20000)
                    m_lstScores.Add(table.m_nScore);
            }

            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 10000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 15000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 20000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 25000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 30000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 35000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 40000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_EARTH, 45000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_MOZARINA, 50000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_MOZARINA, 60000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_MOZARINA, 70000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_MOZARINA, 80000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_MOZARINA, 90000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 100000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 150000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 200000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 250000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 300000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 350000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 400000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_GOLDBAT, 450000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_FIREBIRD, 500000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_FIREBIRD, 1000000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_FIREBIRD, 1500000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_FIREBIRD, 2000000));
            m_lstPrizeCashInfo.Add(new CDVCPrizeCashInfo(JACK_FIREBIRD, 2500000));
        }
    }
}
