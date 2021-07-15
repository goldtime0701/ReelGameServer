using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CYANScoreInfo
    {
        public int m_nGearCode;                 //기계코드
        public int m_nScore;                    //당첨점수
        public int m_nMulti;                    //배수
        public int m_nCmd;                      //명령코드
        public int m_nWinTile;                  //당첨타일
        public int m_nWinCnt;                   //당첨타일개수
        public List<CTile> m_lstTile;           //타일배렬
        public List<int> m_lstLine;             //당첨된라인

        public CYANScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstLine = new List<int>();
        }
    }

    public class CYANScoreTable
    {
        public int m_nMulti;
        public int m_nScore;
        public int m_nLine;
        public int[] m_nlstTile;
        public int m_nCount;
        public int m_nTile;

        public CYANScoreTable(int nMulti, int nScore, int nLine, int[] nlstTile, int nTile, int nCount)
        {
            m_nMulti = nMulti;
            m_nScore = nScore * nMulti;
            m_nLine = nLine;
            m_nlstTile = nlstTile;
            m_nCount = nCount;
            m_nTile = nTile;
        }
    }

    public class CYANPrizeInfo
    {
        public int m_nCont;         //1-번개, 2-무지개, 3-거북, 4-청룡, 5-금룡
        public int m_nPrizeCash;    //잭팟점수
    }

    public class CYANPrizeCashInfo
    {
        public int m_nPrize;
        public int m_nCash;

        public CYANPrizeCashInfo(int nPrize, int nCash)
        {
            m_nPrize = nPrize;
            m_nCash = nCash;
        }
    }

    partial class CYanGear
    {
        private const int JACK_THUNDER = 0x01;
        private const int JACK_RAINBOW = 0x02;
        private const int JACK_TURTLE = 0x03;
        private const int JACK_BDRAGON = 0x04;
        private const int JACK_GDRAGON = 0x05;
        //private const int JACK_NOISE = 0x06;

        public const int ANNY = -1;
        public const int JOKE = 0x00;
        public const int YANG = 0x01;
        public const int PRES = 0x02;
        public const int BAR3 = 0x03;
        public const int BAR2 = 0x04;
        public const int BAR1 = 0x05;
        public const int HORS = 0x06;
        public const int SEVN = 0x07;
        public const int STAR = 0x08;
        public const int TARG = 0x09;
        public const int FOXT = 0x0A;
        public const int PAND = 0x0B;
        public const int FISH = 0x0C;
        public const int FRUT = 0x0D;
        public const int PAUN = 0x0E;
        public const int BERY = 0x0F;
        private int[] m_lstTiles = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        private const int TOP = 2;  //상
        private const int MID = 1;  //중
        private const int BOT = 0;  //하

        public const int PRIZE_START = 0x01;
        public const int PRIZE_NIGHT = 0x02;
        public const int PRIZE_DAY = 0x03;
        public const int PRIZE_END = 0x04;
        public const int PRIZE_THUNDER = 0x05;
        public const int PRIZE_RAINBOW = 0x06;
        public const int PRIZE_TURTLE = 0x07;
        public const int PRIZE_BDRAGON = 0x08;
        public const int PRIZE_GDRAGON = 0x09;

        private const int COUNT_1 = 1;
        private const int COUNT_2 = 2;
        private const int COUNT_3 = 3;
        private const int COUNT_4 = 4;

        private const int MULTI_01 = 1;
        private const int MULTI_02 = 2;
        private const int MULTI_03 = 3;
        private const int MULTI_05 = 5;
        private const int MULTI_10 = 10;
        private int[] m_lstMulti = { 2, 3, 5, 10 };

        private List<CYANScoreTable> m_lstScoreTable = new List<CYANScoreTable>();
        private List<int> m_lstScores = new List<int>();
        private List<CYANScoreTable> m_lstJackTable = new List<CYANScoreTable>();
        private List<CYANPrizeCashInfo> m_lstPrizeCashInfo = new List<CYANPrizeCashInfo>();


        public override void InitScoreTableInfo()
        {
            #region 일반점수표
            //조커
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, COUNT_4));

            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, BOT, new int[] { JOKE, JOKE, JOKE, ANNY }, JOKE, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { JOKE, JOKE, JOKE, ANNY }, JOKE, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, TOP, new int[] { JOKE, JOKE, JOKE, ANNY }, JOKE, COUNT_3));

            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, BOT, new int[] { ANNY, JOKE, JOKE, JOKE }, JOKE, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { ANNY, JOKE, JOKE, JOKE }, JOKE, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, TOP, new int[] { ANNY, JOKE, JOKE, JOKE }, JOKE, COUNT_3));

            //양귀비
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, BOT, new int[] { YANG, YANG, YANG, YANG }, YANG, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { YANG, YANG, YANG, YANG }, YANG, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, TOP, new int[] { YANG, YANG, YANG, YANG }, YANG, COUNT_4));

            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, BOT, new int[] { YANG, YANG, YANG, ANNY }, YANG, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { YANG, YANG, YANG, ANNY }, YANG, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, TOP, new int[] { YANG, YANG, YANG, ANNY }, YANG, COUNT_3));

            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, BOT, new int[] { ANNY, YANG, YANG, YANG }, YANG, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { ANNY, YANG, YANG, YANG }, YANG, COUNT_3));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, TOP, new int[] { ANNY, YANG, YANG, YANG }, YANG, COUNT_3));

            //프레젼트
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, BOT, new int[] { PRES, PRES, PRES, PRES }, PRES, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { PRES, PRES, PRES, PRES }, PRES, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, TOP, new int[] { PRES, PRES, PRES, PRES }, PRES, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 08000, BOT, new int[] { PRES, PRES, PRES, ANNY }, PRES, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 16000, MID, new int[] { PRES, PRES, PRES, ANNY }, PRES, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 08000, TOP, new int[] { PRES, PRES, PRES, ANNY }, PRES, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 08000, BOT, new int[] { ANNY, PRES, PRES, PRES }, PRES, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 16000, MID, new int[] { ANNY, PRES, PRES, PRES }, PRES, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 08000, TOP, new int[] { ANNY, PRES, PRES, PRES }, PRES, COUNT_3));

            //말
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { HORS, HORS, HORS, HORS }, HORS, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, MID, new int[] { HORS, HORS, HORS, HORS }, HORS, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { HORS, HORS, HORS, HORS }, HORS, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, BOT, new int[] { HORS, HORS, HORS, ANNY }, HORS, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 04000, MID, new int[] { HORS, HORS, HORS, ANNY }, HORS, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, TOP, new int[] { HORS, HORS, HORS, ANNY }, HORS, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, BOT, new int[] { ANNY, HORS, HORS, HORS }, HORS, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 04000, MID, new int[] { ANNY, HORS, HORS, HORS }, HORS, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, TOP, new int[] { ANNY, HORS, HORS, HORS }, HORS, COUNT_3));

            //물고기
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { FISH, FISH, FISH, FISH }, FISH, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, MID, new int[] { FISH, FISH, FISH, FISH }, FISH, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { FISH, FISH, FISH, FISH }, FISH, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, BOT, new int[] { FISH, FISH, FISH, ANNY }, FISH, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 04000, MID, new int[] { FISH, FISH, FISH, ANNY }, FISH, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, TOP, new int[] { FISH, FISH, FISH, ANNY }, FISH, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, BOT, new int[] { ANNY, FISH, FISH, FISH }, FISH, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 04000, MID, new int[] { ANNY, FISH, FISH, FISH }, FISH, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, TOP, new int[] { ANNY, FISH, FISH, FISH }, FISH, COUNT_3));

            //판다
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { PAND, PAND, PAND, PAND }, PAND, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, MID, new int[] { PAND, PAND, PAND, PAND }, PAND, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { PAND, PAND, PAND, PAND }, PAND, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01500, BOT, new int[] { PAND, PAND, PAND, ANNY }, PAND, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 03000, MID, new int[] { PAND, PAND, PAND, ANNY }, PAND, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01500, TOP, new int[] { PAND, PAND, PAND, ANNY }, PAND, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01500, BOT, new int[] { ANNY, PAND, PAND, PAND }, PAND, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 03000, MID, new int[] { ANNY, PAND, PAND, PAND }, PAND, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01500, TOP, new int[] { ANNY, PAND, PAND, PAND }, PAND, COUNT_3));

            //여우
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, BOT, new int[] { FOXT, FOXT, FOXT, FOXT }, FOXT, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { FOXT, FOXT, FOXT, FOXT }, FOXT, COUNT_4));
            //m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, TOP, new int[] { FOXT, FOXT, FOXT, FOXT }, FOXT, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { FOXT, FOXT, FOXT, ANNY }, FOXT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { FOXT, FOXT, FOXT, ANNY }, FOXT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { FOXT, FOXT, FOXT, ANNY }, FOXT, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { ANNY, FOXT, FOXT, FOXT }, FOXT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { ANNY, FOXT, FOXT, FOXT }, FOXT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { ANNY, FOXT, FOXT, FOXT }, FOXT, COUNT_3));

            //3빠
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 20000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, COUNT_3));

            //2빠
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 08000, BOT, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 16000, MID, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 08000, TOP, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 03000, BOT, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 06000, MID, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 03000, TOP, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 03000, BOT, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 06000, MID, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 03000, TOP, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, COUNT_3));

            //1빠
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 06000, BOT, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 12000, MID, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 06000, TOP, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01000, BOT, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, MID, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01000, TOP, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01000, BOT, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 02000, MID, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 01000, TOP, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, COUNT_3));

            //쎄븐
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { SEVN, SEVN, SEVN, SEVN }, SEVN, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, BOT, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, MID, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, TOP, new int[] { SEVN, SEVN, SEVN, ANNY }, SEVN, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, BOT, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, MID, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, TOP, new int[] { ANNY, SEVN, SEVN, SEVN }, SEVN, COUNT_3));

            //스타
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, BOT, new int[] { STAR, STAR, STAR, ANNY }, STAR, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, MID, new int[] { STAR, STAR, STAR, ANNY }, STAR, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, TOP, new int[] { STAR, STAR, STAR, ANNY }, STAR, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, BOT, new int[] { ANNY, STAR, STAR, STAR }, STAR, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, MID, new int[] { ANNY, STAR, STAR, STAR }, STAR, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, TOP, new int[] { ANNY, STAR, STAR, STAR }, STAR, COUNT_3));

            //타겟
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { TARG, TARG, TARG, TARG }, TARG, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { TARG, TARG, TARG, TARG }, TARG, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { TARG, TARG, TARG, TARG }, TARG, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, BOT, new int[] { TARG, TARG, TARG, ANNY }, TARG, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, MID, new int[] { TARG, TARG, TARG, ANNY }, TARG, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, TOP, new int[] { TARG, TARG, TARG, ANNY }, TARG, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, BOT, new int[] { ANNY, TARG, TARG, TARG }, TARG, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, MID, new int[] { ANNY, TARG, TARG, TARG }, TARG, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00700, TOP, new int[] { ANNY, TARG, TARG, TARG }, TARG, COUNT_3));


            //과일
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { FRUT, FRUT, FRUT, FRUT }, FRUT, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { FRUT, FRUT, FRUT, FRUT }, FRUT, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { FRUT, FRUT, FRUT, FRUT }, FRUT, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, BOT, new int[] { FRUT, FRUT, FRUT, ANNY }, FRUT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, MID, new int[] { FRUT, FRUT, FRUT, ANNY }, FRUT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, TOP, new int[] { FRUT, FRUT, FRUT, ANNY }, FRUT, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, BOT, new int[] { ANNY, FRUT, FRUT, FRUT }, FRUT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, MID, new int[] { ANNY, FRUT, FRUT, FRUT }, FRUT, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, TOP, new int[] { ANNY, FRUT, FRUT, FRUT }, FRUT, COUNT_3));

            //배
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { PAUN, PAUN, PAUN, PAUN }, PAUN, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { PAUN, PAUN, PAUN, PAUN }, PAUN, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { PAUN, PAUN, PAUN, PAUN }, PAUN, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, BOT, new int[] { PAUN, PAUN, PAUN, ANNY }, PAUN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, MID, new int[] { PAUN, PAUN, PAUN, ANNY }, PAUN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, TOP, new int[] { PAUN, PAUN, PAUN, ANNY }, PAUN, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, BOT, new int[] { ANNY, PAUN, PAUN, PAUN }, PAUN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, MID, new int[] { ANNY, PAUN, PAUN, PAUN }, PAUN, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, TOP, new int[] { ANNY, PAUN, PAUN, PAUN }, PAUN, COUNT_3));

            //딸기
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, BOT, new int[] { BERY, BERY, BERY, BERY }, BERY, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 10000, MID, new int[] { BERY, BERY, BERY, BERY }, BERY, COUNT_4));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 05000, TOP, new int[] { BERY, BERY, BERY, BERY }, BERY, COUNT_4));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, BOT, new int[] { BERY, BERY, BERY, ANNY }, BERY, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, MID, new int[] { BERY, BERY, BERY, ANNY }, BERY, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, TOP, new int[] { BERY, BERY, BERY, ANNY }, BERY, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, BOT, new int[] { ANNY, BERY, BERY, BERY }, BERY, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, MID, new int[] { ANNY, BERY, BERY, BERY }, BERY, COUNT_3));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00300, TOP, new int[] { ANNY, BERY, BERY, BERY }, BERY, COUNT_3));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00200, BOT, new int[] { BERY, BERY, ANNY, ANNY }, BERY, COUNT_2));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00200, MID, new int[] { BERY, BERY, ANNY, ANNY }, BERY, COUNT_2));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00200, TOP, new int[] { BERY, BERY, ANNY, ANNY }, BERY, COUNT_2));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00200, BOT, new int[] { ANNY, ANNY, BERY, BERY }, BERY, COUNT_2));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00200, MID, new int[] { ANNY, ANNY, BERY, BERY }, BERY, COUNT_2));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00200, TOP, new int[] { ANNY, ANNY, BERY, BERY }, BERY, COUNT_2));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00100, BOT, new int[] { BERY, ANNY, ANNY, ANNY }, BERY, COUNT_1));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00100, MID, new int[] { BERY, ANNY, ANNY, ANNY }, BERY, COUNT_1));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00100, TOP, new int[] { BERY, ANNY, ANNY, ANNY }, BERY, COUNT_1));

            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00100, BOT, new int[] { ANNY, ANNY, ANNY, BERY }, BERY, COUNT_1));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00100, MID, new int[] { ANNY, ANNY, ANNY, BERY }, BERY, COUNT_1));
            m_lstScoreTable.Add(new CYANScoreTable(MULTI_01, 00100, TOP, new int[] { ANNY, ANNY, ANNY, BERY }, BERY, COUNT_1));
            #endregion

            #region 잭팟예시점수표
            foreach(int nMulti in m_lstMulti)
            {
                //조커
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 250000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, COUNT_4));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 500000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, COUNT_4));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 250000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, COUNT_4));

                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, BOT, new int[] { JOKE, JOKE, JOKE, ANNY }, JOKE, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 100000, MID, new int[] { JOKE, JOKE, JOKE, ANNY }, JOKE, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, TOP, new int[] { JOKE, JOKE, JOKE, ANNY }, JOKE, COUNT_3));

                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, BOT, new int[] { ANNY, JOKE, JOKE, JOKE }, JOKE, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 100000, MID, new int[] { ANNY, JOKE, JOKE, JOKE }, JOKE, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, TOP, new int[] { ANNY, JOKE, JOKE, JOKE }, JOKE, COUNT_3));

                //양귀비
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 250000, BOT, new int[] { YANG, YANG, YANG, YANG }, YANG, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 500000, MID, new int[] { YANG, YANG, YANG, YANG }, YANG, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 250000, TOP, new int[] { YANG, YANG, YANG, YANG }, YANG, COUNT_4));

                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, BOT, new int[] { YANG, YANG, YANG, ANNY }, YANG, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 100000, MID, new int[] { YANG, YANG, YANG, ANNY }, YANG, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, TOP, new int[] { YANG, YANG, YANG, ANNY }, YANG, COUNT_3));

                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, BOT, new int[] { ANNY, YANG, YANG, YANG }, YANG, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 100000, MID, new int[] { ANNY, YANG, YANG, YANG }, YANG, COUNT_3));
                m_lstScoreTable.Add(new CYANScoreTable(nMulti, 050000, TOP, new int[] { ANNY, YANG, YANG, YANG }, YANG, COUNT_3));

                //프레젼트
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 10000, BOT, new int[] { PRES, PRES, PRES, PRES }, PRES, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 20000, MID, new int[] { PRES, PRES, PRES, PRES }, PRES, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 10000, TOP, new int[] { PRES, PRES, PRES, PRES }, PRES, COUNT_4));

                //말
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, BOT, new int[] { HORS, HORS, HORS, HORS }, HORS, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, MID, new int[] { HORS, HORS, HORS, HORS }, HORS, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, TOP, new int[] { HORS, HORS, HORS, HORS }, HORS, COUNT_4));

                //물고기
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, BOT, new int[] { FISH, FISH, FISH, FISH }, FISH, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, MID, new int[] { FISH, FISH, FISH, FISH }, FISH, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, TOP, new int[] { FISH, FISH, FISH, FISH }, FISH, COUNT_4));

                //판다
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, BOT, new int[] { PAND, PAND, PAND, PAND }, PAND, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, MID, new int[] { PAND, PAND, PAND, PAND }, PAND, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 05000, TOP, new int[] { PAND, PAND, PAND, PAND }, PAND, COUNT_4));

                //여우
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 10000, BOT, new int[] { FOXT, FOXT, FOXT, FOXT }, FOXT, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 20000, MID, new int[] { FOXT, FOXT, FOXT, FOXT }, FOXT, COUNT_4));
                m_lstJackTable.Add(new CYANScoreTable(nMulti, 10000, TOP, new int[] { FOXT, FOXT, FOXT, FOXT }, FOXT, COUNT_4));
            }
            #endregion

            foreach (CYANScoreTable table in m_lstScoreTable)
            {
                if (!m_lstScores.Exists(value => value == table.m_nScore) && table.m_nScore <= 20000)
                    m_lstScores.Add(table.m_nScore);
            }

            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 20000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 30000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 40000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 50000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 60000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 70000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 80000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_TURTLE, 90000));

            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 100000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 150000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 200000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 250000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 300000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 350000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 400000));

            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 500000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 1000000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 1500000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 2000000));
            m_lstPrizeCashInfo.Add(new CYANPrizeCashInfo(JACK_BDRAGON, 2500000));
        }
    }
}
