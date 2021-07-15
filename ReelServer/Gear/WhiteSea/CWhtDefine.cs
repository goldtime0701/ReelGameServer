using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CWHTScoreInfo
    {
        public int m_nGearCode;                 //기계코드
        public int m_nScore;                    //당첨점수
        public int m_nMulti;                    //배수
        public int m_nCmd;                      //명령코드
        public int m_nWinTile;                  //당첨타일
        public int m_nWinCnt;                   //당첨타일개수
        public List<CTile> m_lstTile;           //타일배렬
        public List<int> m_lstLine;             //당첨된라인

        public CWHTScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstLine = new List<int>();
        }
    }

    public class CWHTScoreTable
    {
        public int m_nMulti;
        public int m_nScore;
        public int m_nLine;
        public int[] m_nlstTile;
        public int m_nCount;
        public int m_nTile;

        public CWHTScoreTable(int nMulti, int nScore, int nLine, int[] nlstTile, int nTile, int nCount)
        {
            m_nMulti = nMulti;
            m_nScore = nScore * nMulti;
            m_nLine = nLine;
            m_nlstTile = nlstTile;
            m_nCount = nCount;
            m_nTile = nTile;
        }
    }

    public class CWHTPrizeInfo
    {
        public int m_nCont;         //1-성게, 2-오징어, 3-날치, 4-상어, 5-고래
        public int m_nPrizeCash;    //잭팟점수
    }

    public class CWHTPrizeCashInfo
    {
        public int m_nPrize;
        public int m_nCash;

        public CWHTPrizeCashInfo(int nPrize, int nCash)
        {
            m_nPrize = nPrize;
            m_nCash = nCash;
        }
    }

    partial class CWhtGear
    {
        private const int JACK_URCHIN = 0x01;
        private const int JACK_CUTTLE = 0x02;
        private const int JACK_TUNA = 0x03;
        private const int JACK_SHARK = 0x04;
        private const int JACK_WHALE = 0x05;
        private const int JACK_NOISE = 0x06;

        public const int PRIZE_START = 0x01;
        public const int PRIZE_END = 0x02;
        public const int PRIZE_NIGHT = 0x03;
        public const int PRIZE_DAY = 0x04;
        public const int PRIZE_URCHIN = 0x05;
        public const int PRIZE_CUTTLE = 0x06;
        public const int PRIZE_TUNA = 0x07;
        public const int PRIZE_SHARK = 0x08;
        public const int PRIZE_WHALE = 0x09;

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
        private const int FISH = 7;
        private const int FLOW = 8;
        private const int OCTO = 9;
        private const int SHRI = 10;
        private const int BARC = 11;

        private int[] m_lstTiles = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private List<int>[] m_lstOriginalTile = {
            new List<int>(), new List<int>(), new List<int>(), new List<int>()
        };
        private List<int> m_lstMulti = new List<int>();
        private List<int> m_lstScores = new List<int>();
        private List<CWHTScoreTable> m_lstScoreTable = new List<CWHTScoreTable>();
        private List<CWHTScoreTable> m_lstJackTable = new List<CWHTScoreTable>();
        private List<CWHTPrizeCashInfo> m_lstPrizeCashInfo = new List<CWHTPrizeCashInfo>();

        public override void InitScoreTableInfo()
        {
            #region 오리지널릴배렬
            m_lstOriginalTile[0].Add(BAR1);
            m_lstOriginalTile[0].Add(BAR2);
            m_lstOriginalTile[0].Add(BAR3);
            m_lstOriginalTile[0].Add(FLOW);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(OCTO);
            m_lstOriginalTile[0].Add(SHRI);
            m_lstOriginalTile[0].Add(JOKE);
            m_lstOriginalTile[0].Add(SEVN);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(TAGT);

            m_lstOriginalTile[1].Add(SHRI);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(BAR1);
            m_lstOriginalTile[1].Add(JOKE);
            m_lstOriginalTile[1].Add(BAR2);
            m_lstOriginalTile[1].Add(SEVN);
            m_lstOriginalTile[1].Add(TAGT);
            m_lstOriginalTile[1].Add(STAR);
            m_lstOriginalTile[1].Add(BAR3);
            m_lstOriginalTile[1].Add(FLOW);
            m_lstOriginalTile[1].Add(OCTO);

            m_lstOriginalTile[2].Add(OCTO);
            m_lstOriginalTile[2].Add(BAR2);
            m_lstOriginalTile[2].Add(STAR);
            m_lstOriginalTile[2].Add(TAGT);
            m_lstOriginalTile[2].Add(SHRI);
            m_lstOriginalTile[2].Add(BAR3);
            m_lstOriginalTile[2].Add(FLOW);
            m_lstOriginalTile[2].Add(SEVN);
            m_lstOriginalTile[2].Add(JOKE);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(BAR1);

            m_lstOriginalTile[3].Add(BAR3);
            m_lstOriginalTile[3].Add(STAR);
            m_lstOriginalTile[3].Add(JOKE);
            m_lstOriginalTile[3].Add(BAR1);
            m_lstOriginalTile[3].Add(SEVN);
            m_lstOriginalTile[3].Add(FLOW);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(TAGT);
            m_lstOriginalTile[3].Add(OCTO);
            m_lstOriginalTile[3].Add(BAR2);
            m_lstOriginalTile[3].Add(SHRI);
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
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, BOT, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 3000, MID, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, TOP, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, BOT, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 3000, MID, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, TOP, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1250, BOT, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2500, MID, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1250, TOP, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1250, BOT, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2500, MID, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1250, TOP, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, BOT, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2000, MID, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, TOP, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, BOT, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2000, MID, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, TOP, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, BOT, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2000, MID, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, TOP, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, BOT, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, MID, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, TOP, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, BOT, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, MID, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, TOP, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 05000, BOT, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 10000, MID, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 05000, TOP, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { SEVN, SEVN, ANNY, ANNY }, SEVN, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { SEVN, SEVN, ANNY, ANNY }, SEVN, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { SEVN, SEVN, ANNY, ANNY }, SEVN, 2));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, SEVN, SEVN }, SEVN, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, SEVN, SEVN }, SEVN, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, SEVN, SEVN }, SEVN, 2));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 3750, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 7000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 3750, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2500, BOT, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 5000, MID, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2500, TOP, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { TAGT, TAGT, TAGT, ANNY }, TAGT, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { TAGT, TAGT, TAGT, ANNY }, TAGT, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { TAGT, TAGT, TAGT, ANNY }, TAGT, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { ANNY, TAGT, TAGT, TAGT }, TAGT, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { ANNY, TAGT, TAGT, TAGT }, TAGT, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { ANNY, TAGT, TAGT, TAGT }, TAGT, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { TAGT, TAGT, ANNY, ANNY }, TAGT, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { TAGT, TAGT, ANNY, ANNY }, TAGT, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { TAGT, TAGT, ANNY, ANNY }, TAGT, 2));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, TAGT, TAGT }, TAGT, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, TAGT, TAGT }, TAGT, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, TAGT, TAGT }, TAGT, 2));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1250, BOT, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 2500, MID, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1250, TOP, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, BOT, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, MID, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, TOP, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, BOT, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, MID, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, TOP, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, BOT, new int[] { FLOW, FLOW, FLOW, FLOW }, FLOW, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1500, MID, new int[] { FLOW, FLOW, FLOW, FLOW }, FLOW, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0750, TOP, new int[] { FLOW, FLOW, FLOW, FLOW }, FLOW, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0250, BOT, new int[] { FLOW, FLOW, FLOW, ANNY }, FLOW, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, MID, new int[] { FLOW, FLOW, FLOW, ANNY }, FLOW, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0250, TOP, new int[] { FLOW, FLOW, FLOW, ANNY }, FLOW, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0250, BOT, new int[] { ANNY, FLOW, FLOW, FLOW }, FLOW, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, MID, new int[] { ANNY, FLOW, FLOW, FLOW }, FLOW, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0250, TOP, new int[] { ANNY, FLOW, FLOW, FLOW }, FLOW, 3));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, BOT, new int[] { OCTO, OCTO, OCTO, OCTO }, OCTO, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 1000, MID, new int[] { OCTO, OCTO, OCTO, OCTO }, OCTO, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0500, TOP, new int[] { OCTO, OCTO, OCTO, OCTO }, OCTO, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, BOT, new int[] { OCTO, OCTO, OCTO, ANNY }, OCTO, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0400, MID, new int[] { OCTO, OCTO, OCTO, ANNY }, OCTO, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, TOP, new int[] { OCTO, OCTO, OCTO, ANNY }, OCTO, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, BOT, new int[] { ANNY, OCTO, OCTO, OCTO }, OCTO, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0400, MID, new int[] { ANNY, OCTO, OCTO, OCTO }, OCTO, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, TOP, new int[] { ANNY, OCTO, OCTO, OCTO }, OCTO, 3));


                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0400, BOT, new int[] { SHRI, SHRI, SHRI, SHRI }, SHRI, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0800, MID, new int[] { SHRI, SHRI, SHRI, SHRI }, SHRI, 4));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0400, TOP, new int[] { SHRI, SHRI, SHRI, SHRI }, SHRI, 4));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, BOT, new int[] { SHRI, SHRI, SHRI, ANNY }, SHRI, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0400, MID, new int[] { SHRI, SHRI, SHRI, ANNY }, SHRI, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, TOP, new int[] { SHRI, SHRI, SHRI, ANNY }, SHRI, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, BOT, new int[] { ANNY, SHRI, SHRI, SHRI }, SHRI, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0400, MID, new int[] { ANNY, SHRI, SHRI, SHRI }, SHRI, 3));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, TOP, new int[] { ANNY, SHRI, SHRI, SHRI }, SHRI, 3));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0150, BOT, new int[] { SHRI, SHRI, ANNY, ANNY }, SHRI, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0300, MID, new int[] { SHRI, SHRI, ANNY, ANNY }, SHRI, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0150, TOP, new int[] { SHRI, SHRI, ANNY, ANNY }, SHRI, 2));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0150, BOT, new int[] { ANNY, ANNY, SHRI, SHRI }, SHRI, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0300, MID, new int[] { ANNY, ANNY, SHRI, SHRI }, SHRI, 2));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0150, TOP, new int[] { ANNY, ANNY, SHRI, SHRI }, SHRI, 2));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { SHRI, ANNY, ANNY, ANNY }, SHRI, 1));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { SHRI, ANNY, ANNY, ANNY }, SHRI, 1));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { SHRI, ANNY, ANNY, ANNY }, SHRI, 1));

                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, ANNY, SHRI }, SHRI, 1));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, ANNY, SHRI }, SHRI, 1));
                m_lstScoreTable.Add(new CWHTScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, ANNY, SHRI }, SHRI, 1));



                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));

                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 10 * 10000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 05 * 10000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 05 * 10000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));

                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 50000, MID, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25000, BOT, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25000, TOP, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));

                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 25000, MID, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 12500, BOT, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 12500, TOP, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));

                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 05000, BOT, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 10000, MID, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 05000, TOP, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, 4));

                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 3750, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 7000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 3750, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 2500, BOT, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 5000, MID, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
                m_lstJackTable.Add(new CWHTScoreTable(nMulti, 2500, TOP, new int[] { TAGT, TAGT, TAGT, TAGT }, TAGT, 4));
            }

            #endregion
            m_lstScoreTable = m_lstScoreTable.FindAll(value => value.m_nScore <= 20000);
            foreach (CWHTScoreTable table in m_lstScoreTable)
            {
                if (!m_lstScores.Exists(value => value == table.m_nScore) && table.m_nScore <= 20000)
                    m_lstScores.Add(table.m_nScore);
            }

            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_CUTTLE, 5000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_CUTTLE, 10000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_CUTTLE, 15000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_CUTTLE, 20000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_CUTTLE, 25000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 30000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 40000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 50000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 60000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 70000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 80000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_TUNA, 90000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 100000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 150000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 200000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 250000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 300000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 350000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_SHARK, 400000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_WHALE, 500000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_WHALE, 1000000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_WHALE, 1500000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_WHALE, 2000000));
            m_lstPrizeCashInfo.Add(new CWHTPrizeCashInfo(JACK_WHALE, 2500000));
        }
    }
}
