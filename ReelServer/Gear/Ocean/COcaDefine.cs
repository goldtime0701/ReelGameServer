using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class COCAScoreInfo
    {
        public int m_nGearCode;                 //기계코드
        public int m_nScore;                    //당첨점수
        public int m_nMulti;                    //배수
        public int m_nCmd;                      //명령코드
        public int m_nWinTile;                  //당첨타일
        public int m_nWinCnt;                   //당첨타일개수
        public List<CTile> m_lstTile;           //타일배렬
        public List<int> m_lstLine;             //당첨된라인

        public COCAScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstLine = new List<int>();
        }
    }

    public class COCAScoreTable
    {
        public int m_nMulti;
        public int m_nScore;
        public int m_nLine;
        public int[] m_nlstTile;
        public int m_nCount;
        public int m_nTile;

        public COCAScoreTable(int nMulti, int nScore, int nLine, int[] nlstTile, int nTile, int nCount)
        {
            m_nMulti = nMulti;
            m_nScore = nScore * nMulti;
            m_nLine = nLine;
            m_nlstTile = nlstTile;
            m_nCount = nCount;
            m_nTile = nTile;
        }
    }


    public class COCAPrizeInfo
    {
        public int m_nCont;         //1-물방울, 2-플랑크톤, 3-잠수함, 4-열대어, 5-가오리, 6-상어
        public int m_nPrizeCash;    //잭팟점수
    }

    public class COCAPrizeCashInfo
    {
        public int m_nPrize;
        public int m_nCash;

        public COCAPrizeCashInfo(int nPrize, int nCash)
        {
            m_nPrize = nPrize;
            m_nCash = nCash;
        }
    }

    partial class COcaGear
    {
        private const int JACK_WATER = 0x01;
        private const int JACK_FLOWER = 0x02;
        private const int JACK_SUBMARINE = 0x03;
        private const int JACK_BACkFISH = 0x04;
        private const int JACK_ANIMATIC = 0x05;
        private const int JACK_SHARK = 0x06;
        private const int JACK_NOISE = 0x07;

        private const int PRIZE_START = 0x01;
        private const int WATER_BALL = 0x02;
        private const int WATER_FLOWER = 0x03;
        private const int SUBMARINE = 0x04;
        private const int BACKFISH = 0x05;
        private const int ANIMATIC = 0x06;
        private const int SHARK = 0x07;
        private const int PRIZE_END = 0x08;

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
        private const int SEVE = 4;
        private const int STAR = 5;
        private const int TARG = 6;
        private const int FISH = 7;
        private const int BELL = 8;
        private const int SFIS = 9;
        private const int FLOW = 10;
        private const int BARC = 11;

        private int[] m_lstTiles = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private List<int>[] m_lstOriginalTile = {
            new List<int>(), new List<int>(), new List<int>(), new List<int>()
        };
        private List<int> m_lstMulti = new List<int>();
        private List<int> m_lstScores = new List<int>();
        private List<COCAScoreTable> m_lstScoreTable = new List<COCAScoreTable>();
        private List<COCAScoreTable> m_lstJackTable = new List<COCAScoreTable>();
        private List<COCAPrizeCashInfo> m_lstPrizeCashInfo = new List<COCAPrizeCashInfo>();

        public override void InitScoreTableInfo()
        {
            #region 오리지널릴배렬
            m_lstOriginalTile[0].Add(JOKE);
            m_lstOriginalTile[0].Add(FLOW);
            m_lstOriginalTile[0].Add(TARG);
            m_lstOriginalTile[0].Add(SFIS);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(BAR3);
            m_lstOriginalTile[0].Add(BELL);
            m_lstOriginalTile[0].Add(JOKE);
            m_lstOriginalTile[0].Add(BAR1);
            m_lstOriginalTile[0].Add(TARG);
            m_lstOriginalTile[0].Add(SFIS);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(BAR2);
            m_lstOriginalTile[0].Add(SEVE);

            m_lstOriginalTile[1].Add(BAR3);
            m_lstOriginalTile[1].Add(TARG);
            m_lstOriginalTile[1].Add(SFIS);
            m_lstOriginalTile[1].Add(BAR1);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(FLOW);
            m_lstOriginalTile[1].Add(STAR);
            m_lstOriginalTile[1].Add(JOKE);
            m_lstOriginalTile[1].Add(BAR2);
            m_lstOriginalTile[1].Add(BELL);
            m_lstOriginalTile[1].Add(SFIS);
            m_lstOriginalTile[1].Add(SEVE);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(FLOW);
            m_lstOriginalTile[1].Add(STAR);
            m_lstOriginalTile[1].Add(JOKE);

            m_lstOriginalTile[2].Add(FLOW);
            m_lstOriginalTile[2].Add(SEVE);
            m_lstOriginalTile[2].Add(BAR2);
            m_lstOriginalTile[2].Add(JOKE);
            m_lstOriginalTile[2].Add(SFIS);
            m_lstOriginalTile[2].Add(STAR);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(BELL);
            m_lstOriginalTile[2].Add(FLOW);
            m_lstOriginalTile[2].Add(SEVE);
            m_lstOriginalTile[2].Add(BAR3);
            m_lstOriginalTile[2].Add(JOKE);
            m_lstOriginalTile[2].Add(BAR1);
            m_lstOriginalTile[2].Add(STAR);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(TARG);

            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(BELL);
            m_lstOriginalTile[3].Add(BAR2);
            m_lstOriginalTile[3].Add(SEVE);
            m_lstOriginalTile[3].Add(SFIS);
            m_lstOriginalTile[3].Add(JOKE);
            m_lstOriginalTile[3].Add(TARG);
            m_lstOriginalTile[3].Add(FLOW);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(STAR);
            m_lstOriginalTile[3].Add(BAR3);
            m_lstOriginalTile[3].Add(SEVE);
            m_lstOriginalTile[3].Add(SFIS);
            m_lstOriginalTile[3].Add(BAR1);
            m_lstOriginalTile[3].Add(TARG);
            m_lstOriginalTile[3].Add(FLOW);
            #endregion

            #region 배당
            m_lstMulti.Add(1);
            m_lstMulti.Add(2);
            m_lstMulti.Add(3);
            m_lstMulti.Add(4);
            m_lstMulti.Add(5);
            #endregion

            #region 점수테이블표
            foreach(int nMulti in m_lstMulti)
            {
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, BOT, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 3000, MID, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, TOP, new int[] { BAR3, BAR3, BAR3, ANNY }, BAR3, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, BOT, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 3000, MID, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, TOP, new int[] { ANNY, BAR3, BAR3, BAR3 }, BAR3, 3));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1250, BOT, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2500, MID, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1250, TOP, new int[] { BAR2, BAR2, BAR2, ANNY }, BAR2, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1250, BOT, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2500, MID, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1250, TOP, new int[] { ANNY, BAR2, BAR2, BAR2 }, BAR2, 3));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, BOT, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2000, MID, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, TOP, new int[] { BAR1, BAR1, BAR1, ANNY }, BAR1, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, BOT, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2000, MID, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, TOP, new int[] { ANNY, BAR1, BAR1, BAR1 }, BAR1, 3));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, BOT, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, MID, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, TOP, new int[] { BARC, BARC, BARC, BARC }, BARC, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0700, BOT, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0700, MID, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0700, TOP, new int[] { BARC, BARC, BARC, ANNY }, BARC, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0700, BOT, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0700, MID, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0700, TOP, new int[] { ANNY, BARC, BARC, BARC }, BARC, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 05000, BOT, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 10000, MID, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 05000, TOP, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { SEVE, SEVE, SEVE, ANNY }, SEVE, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { SEVE, SEVE, SEVE, ANNY }, SEVE, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { SEVE, SEVE, SEVE, ANNY }, SEVE, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { ANNY, SEVE, SEVE, SEVE }, SEVE, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { ANNY, SEVE, SEVE, SEVE }, SEVE, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { ANNY, SEVE, SEVE, SEVE }, SEVE, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { SEVE, SEVE, ANNY, ANNY }, SEVE, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { SEVE, SEVE, ANNY, ANNY }, SEVE, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { SEVE, SEVE, ANNY, ANNY }, SEVE, 2));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, SEVE, SEVE }, SEVE, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, SEVE, SEVE }, SEVE, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, SEVE, SEVE }, SEVE, 2));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 3750, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 7000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 3750, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { STAR, STAR, STAR, ANNY }, STAR, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { ANNY, STAR, STAR, STAR }, STAR, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { STAR, STAR, ANNY, ANNY }, STAR, 2));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, STAR, STAR }, STAR, 2));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2500, BOT, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 5000, MID, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2500, TOP, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { TARG, TARG, TARG, ANNY }, TARG, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { TARG, TARG, TARG, ANNY }, TARG, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { TARG, TARG, TARG, ANNY }, TARG, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { ANNY, TARG, TARG, TARG }, TARG, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { ANNY, TARG, TARG, TARG }, TARG, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { ANNY, TARG, TARG, TARG }, TARG, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { TARG, TARG, ANNY, ANNY }, TARG, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { TARG, TARG, ANNY, ANNY }, TARG, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { TARG, TARG, ANNY, ANNY }, TARG, 2));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, TARG, TARG }, TARG, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, TARG, TARG }, TARG, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, TARG, TARG }, TARG, 2));



                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1250, BOT, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 2500, MID, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1250, TOP, new int[] { BELL, BELL, BELL, BELL }, BELL, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, BOT, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, MID, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, TOP, new int[] { BELL, BELL, BELL, ANNY }, BELL, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, BOT, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, MID, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, TOP, new int[] { ANNY, BELL, BELL, BELL }, BELL, 3));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, BOT, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1500, MID, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0750, TOP, new int[] { FISH, FISH, FISH, FISH }, FISH, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0250, BOT, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, MID, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0250, TOP, new int[] { FISH, FISH, FISH, ANNY }, FISH, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0250, BOT, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, MID, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0250, TOP, new int[] { ANNY, FISH, FISH, FISH }, FISH, 3));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, BOT, new int[] { SFIS, SFIS, SFIS, SFIS }, SFIS, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 1000, MID, new int[] { SFIS, SFIS, SFIS, SFIS }, SFIS, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0500, TOP, new int[] { SFIS, SFIS, SFIS, SFIS }, SFIS, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, BOT, new int[] { SFIS, SFIS, SFIS, ANNY }, SFIS, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0400, MID, new int[] { SFIS, SFIS, SFIS, ANNY }, SFIS, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, TOP, new int[] { SFIS, SFIS, SFIS, ANNY }, SFIS, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, BOT, new int[] { ANNY, SFIS, SFIS, SFIS }, SFIS, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0400, MID, new int[] { ANNY, SFIS, SFIS, SFIS }, SFIS, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, TOP, new int[] { ANNY, SFIS, SFIS, SFIS }, SFIS, 3));


                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0400, BOT, new int[] { FLOW, FLOW, FLOW, FLOW }, FLOW, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0800, MID, new int[] { FLOW, FLOW, FLOW, FLOW }, FLOW, 4));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0400, TOP, new int[] { FLOW, FLOW, FLOW, FLOW }, FLOW, 4));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, BOT, new int[] { FLOW, FLOW, FLOW, ANNY }, FLOW, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0400, MID, new int[] { FLOW, FLOW, FLOW, ANNY }, FLOW, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, TOP, new int[] { FLOW, FLOW, FLOW, ANNY }, FLOW, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, BOT, new int[] { ANNY, FLOW, FLOW, FLOW }, FLOW, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0400, MID, new int[] { ANNY, FLOW, FLOW, FLOW }, FLOW, 3));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, TOP, new int[] { ANNY, FLOW, FLOW, FLOW }, FLOW, 3));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0150, BOT, new int[] { FLOW, FLOW, ANNY, ANNY }, FLOW, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0300, MID, new int[] { FLOW, FLOW, ANNY, ANNY }, FLOW, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0150, TOP, new int[] { FLOW, FLOW, ANNY, ANNY }, FLOW, 2));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0150, BOT, new int[] { ANNY, ANNY, FLOW, FLOW }, FLOW, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0300, MID, new int[] { ANNY, ANNY, FLOW, FLOW }, FLOW, 2));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0150, TOP, new int[] { ANNY, ANNY, FLOW, FLOW }, FLOW, 2));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { FLOW, ANNY, ANNY, ANNY }, FLOW, 1));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { FLOW, ANNY, ANNY, ANNY }, FLOW, 1));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { FLOW, ANNY, ANNY, ANNY }, FLOW, 1));

                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, BOT, new int[] { ANNY, ANNY, ANNY, FLOW }, FLOW, 1));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0200, MID, new int[] { ANNY, ANNY, ANNY, FLOW }, FLOW, 1));
                m_lstScoreTable.Add(new COCAScoreTable(nMulti, 0100, TOP, new int[] { ANNY, ANNY, ANNY, FLOW }, FLOW, 1));



                m_lstJackTable.Add(new COCAScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 50 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, BOT, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25 * 10000, TOP, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));

                m_lstJackTable.Add(new COCAScoreTable(nMulti, 10 * 10000, MID, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 05 * 10000, BOT, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 05 * 10000, TOP, new int[] { BAR3, BAR3, BAR3, BAR3 }, BAR3, 4));

                m_lstJackTable.Add(new COCAScoreTable(nMulti, 50000, MID, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25000, BOT, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25000, TOP, new int[] { BAR2, BAR2, BAR2, BAR2 }, BAR2, 4));

                m_lstJackTable.Add(new COCAScoreTable(nMulti, 25000, MID, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 12500, BOT, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 12500, TOP, new int[] { BAR1, BAR1, BAR1, BAR1 }, BAR1, 4));

                m_lstJackTable.Add(new COCAScoreTable(nMulti, 05000, BOT, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 10000, MID, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 05000, TOP, new int[] { SEVE, SEVE, SEVE, SEVE }, SEVE, 4));

                m_lstJackTable.Add(new COCAScoreTable(nMulti, 3750, BOT, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 7000, MID, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 3750, TOP, new int[] { STAR, STAR, STAR, STAR }, STAR, 4));

                m_lstJackTable.Add(new COCAScoreTable(nMulti, 2500, BOT, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 5000, MID, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
                m_lstJackTable.Add(new COCAScoreTable(nMulti, 2500, TOP, new int[] { TARG, TARG, TARG, TARG }, TARG, 4));
            }
            m_lstJackTable.Add(new COCAScoreTable(5, 60 * 10000, MID, new int[] { JOKE, JOKE, JOKE, JOKE }, JOKE, 4));   //조커5배당일때 300만도 될수 있다고 함

            #endregion
            m_lstScoreTable = m_lstScoreTable.FindAll(value => value.m_nScore <= 20000);
            foreach (COCAScoreTable table in m_lstScoreTable)
            {
                if (!m_lstScores.Exists(value => value == table.m_nScore))
                    m_lstScores.Add(table.m_nScore);
            }

            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 10000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 15000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 20000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 25000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 30000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 35000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 40000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SUBMARINE, 45000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_BACkFISH,  50000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_BACkFISH,  60000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_BACkFISH,  70000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_BACkFISH,  80000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_BACkFISH,  90000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  100000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  150000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  200000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  250000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  300000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  350000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  400000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_ANIMATIC,  450000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SHARK,     500000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SHARK,     1000000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SHARK,     1500000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SHARK,     2000000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SHARK,     2500000));  
            m_lstPrizeCashInfo.Add(new COCAPrizeCashInfo(JACK_SHARK,     3000000));  
        }
    }
}
