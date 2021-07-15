using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CNWDPrizeInfo
    {
        public int m_nCont;         //
        public int m_nPrizeCash;    //
    }

    public class CNWDScoreInfo
    {
        public int m_nGearCode;                 //기계코드
        public int m_nScore;                    //당첨점수
        public int m_nMulti;                    //배수
        public int m_nCmd;                      //명령코드
        public int m_nWinTile;                  //당첨타일
        public int m_nWinCnt;                   //당첨타일개수
        public int m_nAni;                      //애니매션코드
        public List<CTile> m_lstTile;           //타일배렬
        public List<int> m_lstLine;             //당첨된라인

        public CNWDScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstLine = new List<int>();
        }
    }

    public class CNWDScoreTable
    {
        public int m_nMulti;
        public int m_nScore;
        public int m_nLine;
        public int[] m_nlstTile;
        public int m_nCount;
        public int m_nTile;

        public CNWDScoreTable(int nMulti, int nScore, int nLine, int[] nlstTile, int nTile, int nCount)
        {
            m_nMulti = nMulti;
            if(nMulti == 1)
                m_nScore = nScore * 1;
            else if(nMulti == 2)
                m_nScore = Convert.ToInt32(nScore * 1.5);
            else if (nMulti == 3)
                m_nScore = Convert.ToInt32(nScore * 2);
            else if (nMulti == 4)
                m_nScore = Convert.ToInt32(nScore * 2.5);

            m_nLine = nLine;
            m_nlstTile = nlstTile;
            m_nCount = nCount;
            m_nTile = nTile;
        }
    }


    partial class CNwdGear
    {
        private const int ANNY = -1;
        private const int JOKE = 0x00;
        private const int BAR3 = 0x01;
        private const int SEVE = 0x02;
        private const int STAR = 0x03;
        private const int TARG = 0x04;
        private const int GBOX = 0x05;
        private const int BELL = 0x06;
        private const int AIRC = 0x07;
        private const int SHIP = 0x08;
        private const int FISH = 0x09;
        private const int CHAY = 0x0A;


        private const int JACK_CONT_NULL = 0x0;
        private const int JACK_CONT_FIREBIRD = 0x01;    //불새
        private const int JACK_CONT_GODGIRL = 0x02;     //녀신
        private const int JACK_CONT_NOISE = 0x03;       //뻥

        public const int PRIZE_START = 0x01;
        public const int PRIZE_NIGHT = 0x02;
        public const int PRIZE_DAY = 0x03;
        public const int PRIZE_END = 0x04;
        public const int PRIZE_DOLPIN = 0x05;
        public const int PRIZE_RAIN = 0x06;
        public const int PRIZE_STHUNDER = 0x07;
        public const int PRIZE_FIREFLY = 0x08;
        public const int PRIZE_LTHUNDER = 0x09;
        public const int PRIZE_ZEPEL = 0x0A;
        public const int PRIZE_BUTTERFLY = 0x0B;
        public const int PRIZE_ZEPEL1 = 0x0C;
        public const int PRIZE_WHALE = 0x0D;
        public const int PRIZE_BOAT = 0x0E;
        public const int PRIZE_SHARK = 0x0F;
        public const int PRIZE_GIRL = 0x10;
        public const int PRIZE_SCORE = 0x11;
        public const int PRIZE_FBIRD = 0x12;
        public const int PRIZE_OWL = 0x13;



        private const int TOP = 2;  //상
        private const int MID = 1;  //중
        private const int BOT = 0;  //하

        private int[] m_nlstFireBirdScore = { 100000, 150000, 200000, 250000, 300000, 400000, 500000 };
        private int[] m_nlstGodGirl = { 500000, 750000, 1000000, 1250000, 1500000, 2000000, 2500000 };

        private int[] m_nlstPrizeEmptyRoll = { PRIZE_DOLPIN, PRIZE_RAIN, PRIZE_OWL, PRIZE_ZEPEL, PRIZE_DOLPIN, PRIZE_BUTTERFLY, PRIZE_ZEPEL1, PRIZE_DOLPIN, PRIZE_RAIN };
        private int[] m_lstTiles = { JOKE, BAR3, SEVE, STAR, TARG, GBOX, BELL, AIRC, SHIP, FISH, CHAY };
        private List<int>[] m_lstOriginalTile = {
            new List<int>(), new List<int>(), new List<int>(), new List<int>()
        };
        private List<int> m_lstMulti = new List<int>();
        private List<int> m_lstScores = new List<int>();
        private List<CNWDScoreTable> m_lstScoreTable = new List<CNWDScoreTable>();
        private List<CNWDScoreTable> m_lstJackTable = new List<CNWDScoreTable>();
        private List<CNWDScoreTable> m_lstMultiTable = new List<CNWDScoreTable>();


        public override void InitScoreTableInfo()
        {
            #region 오리지널릴배렬
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(SHIP);
            m_lstOriginalTile[0].Add(BAR3);
            m_lstOriginalTile[0].Add(BELL);
            m_lstOriginalTile[0].Add(SHIP);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(GBOX);
            m_lstOriginalTile[0].Add(AIRC);
            m_lstOriginalTile[0].Add(CHAY);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(AIRC);
            m_lstOriginalTile[0].Add(TARG);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(BELL);
            m_lstOriginalTile[0].Add(AIRC);
            m_lstOriginalTile[0].Add(GBOX);
            m_lstOriginalTile[0].Add(SEVE);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(SHIP);
            m_lstOriginalTile[0].Add(JOKE);

            m_lstOriginalTile[1].Add(JOKE);
            m_lstOriginalTile[1].Add(AIRC);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(CHAY);
            m_lstOriginalTile[1].Add(SHIP);
            m_lstOriginalTile[1].Add(BELL);
            m_lstOriginalTile[1].Add(BAR3);
            m_lstOriginalTile[1].Add(AIRC);
            m_lstOriginalTile[1].Add(CHAY);
            m_lstOriginalTile[1].Add(SHIP);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(GBOX);
            m_lstOriginalTile[1].Add(TARG);
            m_lstOriginalTile[1].Add(SEVE);
            m_lstOriginalTile[1].Add(STAR);
            m_lstOriginalTile[1].Add(AIRC);
            m_lstOriginalTile[1].Add(BELL);
            m_lstOriginalTile[1].Add(SHIP);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(CHAY);

            m_lstOriginalTile[2].Add(CHAY);
            m_lstOriginalTile[2].Add(JOKE);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(GBOX);
            m_lstOriginalTile[2].Add(SHIP);
            m_lstOriginalTile[2].Add(SEVE);
            m_lstOriginalTile[2].Add(BELL);
            m_lstOriginalTile[2].Add(TARG);
            m_lstOriginalTile[2].Add(AIRC);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(SHIP);
            m_lstOriginalTile[2].Add(BAR3);
            m_lstOriginalTile[2].Add(STAR);
            m_lstOriginalTile[2].Add(CHAY);
            m_lstOriginalTile[2].Add(GBOX);
            m_lstOriginalTile[2].Add(BELL);
            m_lstOriginalTile[2].Add(CHAY);
            m_lstOriginalTile[2].Add(AIRC);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(SHIP);

            m_lstOriginalTile[3].Add(SHIP);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(AIRC);
            m_lstOriginalTile[3].Add(JOKE);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(TARG);
            m_lstOriginalTile[3].Add(AIRC);
            m_lstOriginalTile[3].Add(BAR3);
            m_lstOriginalTile[3].Add(SHIP);
            m_lstOriginalTile[3].Add(GBOX);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(STAR);
            m_lstOriginalTile[3].Add(TARG);
            m_lstOriginalTile[3].Add(BELL);
            m_lstOriginalTile[3].Add(SHIP);
            m_lstOriginalTile[3].Add(SEVE);
            m_lstOriginalTile[3].Add(BELL);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(GBOX);
            m_lstOriginalTile[3].Add(CHAY);
            #endregion

            #region 배당
            m_lstMulti.Add(1);
            #endregion

            #region 점수테이블
            foreach (int nMulti in m_lstMulti)
            {
                m_lstJackTable.Add(new CNWDScoreTable(nMulti, 0500000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti, 1000000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti, 0500000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));

                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 2, 0500000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 2, 1000000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 2, 0500000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));

                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 3, 0500000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 3, 1000000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 3, 0500000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));

                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 4, 0500000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 4, 1000000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 4, 0500000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));


                m_lstJackTable.Add(new CNWDScoreTable(nMulti, 100000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti, 200000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti, 100000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));

                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 2, 100000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 2, 200000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 2, 100000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));

                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 3, 100000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 3, 200000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 3, 100000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));

                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 4, 100000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 4, 200000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new CNWDScoreTable(nMulti * 4, 100000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));


                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, BOT, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 15000, MID, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, TOP, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, BOT, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 15000, MID, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, TOP, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 10000, BOT, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 20000, MID, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 10000, TOP, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, BOT, new int[] { SEVE, SEVE, SEVE, ANNY }, SEVE, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 10000, MID, new int[] { SEVE, SEVE, SEVE, ANNY }, SEVE, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, TOP, new int[] { SEVE, SEVE, SEVE, ANNY }, SEVE, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, BOT, new int[] { ANNY, SEVE, SEVE, SEVE }, SEVE, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 10000, MID, new int[] { ANNY, SEVE, SEVE, SEVE }, SEVE, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, TOP, new int[] { ANNY, SEVE, SEVE, SEVE }, SEVE, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 15000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, BOT, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 03000, MID, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, TOP, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, BOT, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 03000, MID, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, TOP, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, BOT, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 15000, MID, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 07500, TOP, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, BOT, new int[] { TARG, TARG, TARG, ANNY }, TARG, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02000, MID, new int[] { TARG, TARG, TARG, ANNY }, TARG, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, TOP, new int[] { TARG, TARG, TARG, ANNY }, TARG, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, BOT, new int[] { ANNY, TARG, TARG, TARG }, TARG, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02000, MID, new int[] { ANNY, TARG, TARG, TARG }, TARG, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, TOP, new int[] { ANNY, TARG, TARG, TARG }, TARG, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, BOT, new int[] { GBOX, GBOX, GBOX, GBOX }, GBOX, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 10000, MID, new int[] { GBOX, GBOX, GBOX, GBOX }, GBOX, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, TOP, new int[] { GBOX, GBOX, GBOX, GBOX }, GBOX, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00750, BOT, new int[] { GBOX, GBOX, GBOX, ANNY }, GBOX, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, MID, new int[] { GBOX, GBOX, GBOX, ANNY }, GBOX, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00750, TOP, new int[] { GBOX, GBOX, GBOX, ANNY }, GBOX, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00750, BOT, new int[] { ANNY, GBOX, GBOX, GBOX }, GBOX, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, MID, new int[] { ANNY, GBOX, GBOX, GBOX }, GBOX, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00750, TOP, new int[] { ANNY, GBOX, GBOX, GBOX }, GBOX, 3));

                m_lstMultiTable.Add(new CNWDScoreTable(nMulti, 10000, MID, new int[] { GBOX, GBOX, GBOX, GBOX }, GBOX, 4));
                m_lstMultiTable.Add(new CNWDScoreTable(nMulti, 01500, MID, new int[] { GBOX, GBOX, GBOX, ANNY }, GBOX, 3));
                m_lstMultiTable.Add(new CNWDScoreTable(nMulti, 00000, MID, new int[] { ANNY, ANNY, GBOX, GBOX }, GBOX, 2));
                m_lstMultiTable.Add(new CNWDScoreTable(nMulti, 00000, MID, new int[] { GBOX, GBOX, ANNY, ANNY }, GBOX, 2));
                m_lstMultiTable.Add(new CNWDScoreTable(nMulti, 00000, MID, new int[] { ANNY, ANNY, ANNY, GBOX }, GBOX, 1));
                m_lstMultiTable.Add(new CNWDScoreTable(nMulti, 00000, MID, new int[] { GBOX, ANNY, ANNY, ANNY }, GBOX, 1));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02500, BOT, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, MID, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02500, TOP, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, BOT, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, MID, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, TOP, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, BOT, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, MID, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, TOP, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, BOT, new int[] { AIRC, AIRC, AIRC, AIRC }, AIRC, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 03000, MID, new int[] { AIRC, AIRC, AIRC, AIRC }, AIRC, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01500, TOP, new int[] { AIRC, AIRC, AIRC, AIRC }, AIRC, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { AIRC, AIRC, AIRC, ANNY }, AIRC, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { AIRC, AIRC, AIRC, ANNY }, AIRC, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { AIRC, AIRC, AIRC, ANNY }, AIRC, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { ANNY, AIRC, AIRC, AIRC }, AIRC, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { ANNY, AIRC, AIRC, AIRC }, AIRC, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { ANNY, AIRC, AIRC, AIRC }, AIRC, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, BOT, new int[] { SHIP, SHIP, SHIP, SHIP }, SHIP, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02000, MID, new int[] { SHIP, SHIP, SHIP, SHIP }, SHIP, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, TOP, new int[] { SHIP, SHIP, SHIP, SHIP }, SHIP, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { SHIP, SHIP, SHIP, ANNY }, SHIP, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { SHIP, SHIP, SHIP, ANNY }, SHIP, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { SHIP, SHIP, SHIP, ANNY }, SHIP, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { ANNY, SHIP, SHIP, SHIP }, SHIP, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { ANNY, SHIP, SHIP, SHIP }, SHIP, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { ANNY, SHIP, SHIP, SHIP }, SHIP, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, BOT, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 01000, MID, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, TOP, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, BOT, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00300, MID, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, TOP, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, BOT, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00300, MID, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, TOP, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02500, BOT, new int[] { CHAY, CHAY, CHAY, CHAY }, CHAY, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 05000, MID, new int[] { CHAY, CHAY, CHAY, CHAY }, CHAY, 4));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 02500, TOP, new int[] { CHAY, CHAY, CHAY, CHAY }, CHAY, 4));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { CHAY, CHAY, CHAY, ANNY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { CHAY, CHAY, CHAY, ANNY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { CHAY, CHAY, CHAY, ANNY }, CHAY, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { ANNY, CHAY, CHAY, CHAY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { ANNY, CHAY, CHAY, CHAY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { ANNY, CHAY, CHAY, CHAY }, CHAY, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { CHAY, ANNY, CHAY, CHAY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { CHAY, ANNY, CHAY, CHAY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { CHAY, ANNY, CHAY, CHAY }, CHAY, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, BOT, new int[] { CHAY, CHAY, ANNY, CHAY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00500, MID, new int[] { CHAY, CHAY, ANNY, CHAY }, CHAY, 3));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00250, TOP, new int[] { CHAY, CHAY, ANNY, CHAY }, CHAY, 3));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, BOT, new int[] { CHAY, CHAY, ANNY, ANNY }, CHAY, 2));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00300, MID, new int[] { CHAY, CHAY, ANNY, ANNY }, CHAY, 2));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, TOP, new int[] { CHAY, CHAY, ANNY, ANNY }, CHAY, 2));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, BOT, new int[] { CHAY, ANNY, ANNY, CHAY }, CHAY, 2));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00300, MID, new int[] { CHAY, ANNY, ANNY, CHAY }, CHAY, 2));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, TOP, new int[] { CHAY, ANNY, ANNY, CHAY }, CHAY, 2));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, BOT, new int[] { ANNY, ANNY, CHAY, CHAY }, CHAY, 2));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00300, MID, new int[] { ANNY, ANNY, CHAY, CHAY }, CHAY, 2));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00150, TOP, new int[] { ANNY, ANNY, CHAY, CHAY }, CHAY, 2));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00050, BOT, new int[] { CHAY, ANNY, ANNY, ANNY }, CHAY, 1));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00100, MID, new int[] { CHAY, ANNY, ANNY, ANNY }, CHAY, 1));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00050, TOP, new int[] { CHAY, ANNY, ANNY, ANNY }, CHAY, 1));

                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00050, BOT, new int[] { ANNY, ANNY, ANNY, CHAY }, CHAY, 1));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00100, MID, new int[] { ANNY, ANNY, ANNY, CHAY }, CHAY, 1));
                m_lstScoreTable.Add(new CNWDScoreTable(nMulti, 00050, TOP, new int[] { ANNY, ANNY, ANNY, CHAY }, CHAY, 1));
            }
            #endregion

            foreach (CNWDScoreTable table in m_lstScoreTable)
            {
                if (!m_lstScores.Exists(value => value == table.m_nScore))
                    m_lstScores.Add(table.m_nScore);
            }
        }
    }
}
