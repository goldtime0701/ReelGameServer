using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CSEAScoreInfo
    {
        public int m_nGearCode;         //기계코드
        public int m_nScore;            //맞은 점수
        public int m_nMulti;            //배당
        public List<CTile> m_lstTile;   //당첨되지 않은 타일까지 포함한 완전한 타일배렬(3 X 5)
        public int m_nFlag;             //0-일반빈돌기, 1-일반점수돌기, 2-멎은다음에 번개(2개를 먼저 맞추어 준다음에 맞은 타일을 돌려준다.), 3-잠수함미사일 맞추기(빠, 별, 타 X 3시 잠수함출현 맞추기)
        public int m_nTile;             //숨기였다가 보여주는 맞은 타일
        public List<int> m_lstLine;     //맞은 렬번호리스트
        public int m_nCmd;              //잭팟애니매션시작동작을 나타내는 값
        public int m_nWinTile;          //맞은 타일의 번호
        public int m_nWinCnt;           //맞은 타일의 개수


        public CSEAScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstLine = new List<int>();
            m_nMulti = 1;
        }
    }

    public class SEAScoreTableInfo
    {
        public int nScore;      //당첨점수
        public int[] lstLine;   //당첨행
        public int[][] lstTile; //3X4타일행렬  
        public int nCount;      //당첨타일개수
        public int nTile;       //당첨타일
        public int nMulti;      //배당     
        public int[] lstWin;    //당첨타일리스트


        public SEAScoreTableInfo(int nScore, int[] nlstLine, int[][] lstTile, int nTile, int nCount, int nMulti, int[] lstWin)
        {
            this.nScore = nScore;
            this.lstLine = nlstLine;
            this.lstTile = lstTile;
            this.nCount = nCount;
            this.nTile = nTile;
            this.nMulti = nMulti;
            this.lstWin = lstWin;
        }
    }


    public class CSEAPrizeInfo
    {
        public int m_nCont;       //1-해파리잭팟, 2-상어잭팟, 3-고래잭팟
        public int m_nPrizeCash; //잭팟점수
    }



    partial class CSeaGear
    {
        private const int TOP = 2;  //상
        private const int MID = 1;  //중
        private const int BOT = 0;  //하

        private const int MULTI_1 = 1;
        private const int MULTI_2 = 2;
        private const int MULTI_3 = 3;
        private const int MULTI_4 = 4;

        private const int COUNT_1 = 1;
        private const int COUNT_2 = 2;
        private const int COUNT_3 = 3;
        private const int COUNT_4 = 4;

        //잭팟형식
        private const int JACK_NISE = 0;           //가짜예시
        private const int JACK_JELLY = 1;          //해파리
        private const int JACK_SHARK = 2;          //상어
        private const int JACK_WHALE = 3;          //고래
        private const int JACK_NISE1 = 4;          //가짜예시
        private const int JACK_NISE2 = 5;          //가짜예시

        //잭팟명령
        private const int PRIZE_START = 1;          //예시시작
        private const int PRIZE_TURTLE = 2;         //거부기출현
        private const int PRIZE_JELLY = 3;          //해파리출현
        private const int PRIZE_SHARK = 4;          //상어출현
        private const int PRIZE_WHALE = 5;          //고래출현
        private const int PRIZE_GOD_DISAPPEAR = 6;  //출현하였던 객체 사라지기
        private const int PRIZE_NIGHT = 7;          //밤
        private const int PRIZE_AFTER = 8;          //낮
        private const int PRIZE_END = 9;            //예시끝


        List<SEAScoreTableInfo> m_lstSEAScoreTable;
        List<int> m_lstSeaScores;
        List<int> m_lstTile;

        List<int>[] m_lstOriginalTile = {
            new List<int>(), new List<int>(), new List<int>(), new List<int>()
        };

        private const int ANNY = -1;
        private const int JOKE = 0;  //조커
        private const int BAR3 = 1;    //빠  
        private const int SEVN = 2;  //세븐
        private const int STAR = 3;   //스타
        private const int TAGT = 4; //타겟
        private const int CLAM = 5;   //조개
        private const int HEMA = 6;   //헤마
        private const int MENG = 7;   //멍
        private const int BOGA = 8;   //보가지
        private const int OCTO = 9;   //문어
        private const int FISH = 10;  //물고기

        override
        public void InitScoreTableInfo()
        {
            m_lstSEAScoreTable = new List<SEAScoreTableInfo>();

            //조커 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(250000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { SEVN, BAR3, FISH, MENG }, new int[] { BAR3, CLAM, BOGA, FISH }, new int[] { JOKE, JOKE, JOKE, JOKE } }, JOKE, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(500000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { BAR3, CLAM, BOGA, FISH }, new int[] { JOKE, JOKE, JOKE, JOKE }, new int[] { HEMA, MENG, TAGT, OCTO } }, JOKE, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));     //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(250000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { JOKE, JOKE, JOKE, JOKE }, new int[] { HEMA, MENG, TAGT, OCTO }, new int[] { FISH, SEVN, CLAM, TAGT } }, JOKE, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하


            //빠 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(050000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { STAR, FISH, SEVN, HEMA }, new int[] { SEVN, MENG, HEMA, CLAM }, new int[] { BAR3, BAR3, BAR3, BAR3 } }, BAR3, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(100000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { SEVN, MENG, HEMA, CLAM }, new int[] { BAR3, BAR3, BAR3, BAR3 }, new int[] { JOKE, CLAM, FISH, OCTO } }, BAR3, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            //m_lstSEAScoreTable.Add(new SEAScoreTableInfo(050000 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BAR3, BAR3, BAR3, BAR3 }, new int[] { JOKE, CLAM, FISH, OCTO }, new int[] { HEMA, JOKE, BOGA, MENG } }, BAR3, COUNT_4, MULTI_1));   //하(2만점이상 예시에서 복합점수가 발생.)

            //빠 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { STAR, FISH, SEVN, ANNY }, new int[] { SEVN, MENG, HEMA, ANNY }, new int[] { BAR3, BAR3, BAR3, ANNY } }, BAR3, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));     //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { SEVN, MENG, HEMA, ANNY }, new int[] { BAR3, BAR3, BAR3, ANNY }, new int[] { JOKE, CLAM, FISH, ANNY } }, BAR3, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));     //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { BAR3, BAR3, BAR3, ANNY }, new int[] { JOKE, CLAM, FISH, ANNY }, new int[] { HEMA, JOKE, BOGA, ANNY } }, BAR3, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));     //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, FISH, SEVN, HEMA }, new int[] { ANNY, MENG, HEMA, CLAM }, new int[] { ANNY, BAR3, BAR3, BAR3 } }, BAR3, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));     //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, MENG, HEMA, CLAM }, new int[] { ANNY, BAR3, BAR3, BAR3 }, new int[] { ANNY, CLAM, FISH, OCTO } }, BAR3, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));       //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { ANNY, BAR3, BAR3, BAR3 }, new int[] { ANNY, CLAM, FISH, OCTO }, new int[] { ANNY, JOKE, BOGA, MENG } }, BAR3, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 11 }));   //하


            //세븐 X 4
            //m_lstSEAScoreTable.Add(new SEAScoreTableInfo(25000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { TAGT, OCTO, MENG, MENG }, new int[] { STAR, HEMA, OCTO, FISH }, new int[] { SEVN, SEVN, SEVN, SEVN } }, SEVN, COUNT_4, MULTI_1));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(50000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, HEMA, OCTO, FISH }, new int[] { SEVN, SEVN, SEVN, SEVN }, new int[] { BAR3, BOGA, HEMA, FISH } }, SEVN, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(25000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { SEVN, SEVN, SEVN, SEVN }, new int[] { BAR3, BOGA, HEMA, FISH }, new int[] { JOKE, FISH, BAR3, HEMA } }, SEVN, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하
            //m_lstSEAScoreTable.Add(new SEAScoreTableInfo(25000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { TAGT, JOKE, MENG, MENG }, new int[] { STAR, MENG, OCTO, FISH }, new int[] { SEVN, SEVN, SEVN, SEVN } }, SEVN, COUNT_4, MULTI_1));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(50000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, MENG, OCTO, FISH }, new int[] { SEVN, SEVN, SEVN, SEVN }, new int[] { BAR3, OCTO, HEMA, FISH } }, SEVN, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(25000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { SEVN, SEVN, SEVN, SEVN }, new int[] { BAR3, OCTO, HEMA, FISH }, new int[] { JOKE, HEMA, BAR3, HEMA } }, SEVN, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            //세븐 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { TAGT, OCTO, MENG, ANNY }, new int[] { STAR, HEMA, OCTO, ANNY }, new int[] { SEVN, SEVN, SEVN, ANNY } }, SEVN, COUNT_3, MULTI_1, new int[] { 8, 9, 10}));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, HEMA, OCTO, ANNY }, new int[] { SEVN, SEVN, SEVN, ANNY }, new int[] { BAR3, BOGA, HEMA, ANNY } }, SEVN, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { SEVN, SEVN, SEVN, ANNY }, new int[] { BAR3, BOGA, HEMA, ANNY }, new int[] { JOKE, FISH, BAR3, ANNY } }, SEVN, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { ANNY, OCTO, MENG, MENG }, new int[] { ANNY, HEMA, OCTO, FISH }, new int[] { ANNY, SEVN, SEVN, SEVN } }, SEVN, COUNT_3, MULTI_1, new int[] { 9, 10, 11, 2, 3 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, HEMA, OCTO, FISH }, new int[] { ANNY, SEVN, SEVN, SEVN }, new int[] { ANNY, BOGA, HEMA, FISH } }, SEVN, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, SEVN, SEVN, SEVN }, new int[] { ANNY, BOGA, HEMA, FISH }, new int[] { ANNY, FISH, BAR3, HEMA } }, SEVN, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { TAGT, JOKE, MENG, ANNY }, new int[] { STAR, MENG, OCTO, ANNY }, new int[] { SEVN, SEVN, SEVN, ANNY } }, SEVN, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, MENG, OCTO, ANNY }, new int[] { SEVN, SEVN, SEVN, ANNY }, new int[] { BAR3, OCTO, HEMA, ANNY } }, SEVN, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { SEVN, SEVN, SEVN, ANNY }, new int[] { BAR3, OCTO, HEMA, ANNY }, new int[] { JOKE, HEMA, BAR3, ANNY } }, SEVN, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { ANNY, JOKE, MENG, MENG }, new int[] { ANNY, MENG, OCTO, FISH }, new int[] { ANNY, SEVN, SEVN, SEVN } }, SEVN, COUNT_3, MULTI_1, new int[] { 9, 10, 11, 2, 3 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, MENG, OCTO, FISH }, new int[] { ANNY, SEVN, SEVN, SEVN }, new int[] { ANNY, OCTO, HEMA, FISH } }, SEVN, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, SEVN, SEVN, SEVN }, new int[] { ANNY, OCTO, HEMA, FISH }, new int[] { ANNY, HEMA, BAR3, HEMA } }, SEVN, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하


            //스타 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(10000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, HEMA, HEMA }, new int[] { OCTO, OCTO, BOGA, CLAM }, new int[] { STAR, STAR, STAR, STAR } }, STAR, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(20000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, OCTO, BOGA, CLAM }, new int[] { STAR, STAR, STAR, STAR }, new int[] { FISH, TAGT, MENG, BOGA } }, STAR, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(10100 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { STAR, STAR, STAR, STAR }, new int[] { FISH, TAGT, MENG, BOGA }, new int[] { OCTO, FISH, OCTO, MENG } }, STAR, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 11 }));   //하

            //스타 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, HEMA, ANNY }, new int[] { OCTO, OCTO, BOGA, ANNY }, new int[] { STAR, STAR, STAR, ANNY } }, STAR, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, OCTO, BOGA, ANNY }, new int[] { STAR, STAR, STAR, ANNY }, new int[] { FISH, TAGT, MENG, ANNY } }, STAR, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { STAR, STAR, STAR, ANNY }, new int[] { FISH, TAGT, MENG, ANNY }, new int[] { OCTO, FISH, OCTO, ANNY } }, STAR, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, BOGA, HEMA, HEMA }, new int[] { ANNY, OCTO, BOGA, CLAM }, new int[] { ANNY, STAR, STAR, STAR } }, STAR, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, BOGA, CLAM }, new int[] { ANNY, STAR, STAR, STAR }, new int[] { ANNY, TAGT, MENG, BOGA } }, STAR, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { ANNY, STAR, STAR, STAR }, new int[] { ANNY, TAGT, MENG, BOGA }, new int[] { ANNY, FISH, OCTO, MENG } }, STAR, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 11 }));   //하


            //타겟 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05100 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, OCTO, CLAM, JOKE }, new int[] { CLAM, STAR, OCTO, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT } }, TAGT, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(10000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, STAR, OCTO, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, FISH, MENG, BOGA } }, TAGT, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, FISH, MENG, BOGA }, new int[] { SEVN, MENG, OCTO, HEMA } }, TAGT, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05100 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, FISH, CLAM, JOKE }, new int[] { CLAM, CLAM, OCTO, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT } }, TAGT, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(10000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, CLAM, OCTO, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, BOGA, MENG, BOGA } }, TAGT, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, BOGA, MENG, BOGA }, new int[] { SEVN, OCTO, OCTO, HEMA } }, TAGT, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05100 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, OCTO, BOGA, JOKE }, new int[] { CLAM, STAR, JOKE, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT } }, TAGT, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(10000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, STAR, JOKE, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, FISH, CLAM, BOGA } }, TAGT, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, FISH, CLAM, BOGA }, new int[] { SEVN, MENG, OCTO, HEMA } }, TAGT, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05100 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, FISH, BOGA, JOKE }, new int[] { CLAM, CLAM, JOKE, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT } }, TAGT, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(10000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, CLAM, JOKE, OCTO }, new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, BOGA, CLAM, BOGA } }, TAGT, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(05000 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, TAGT }, new int[] { STAR, BOGA, CLAM, BOGA }, new int[] { SEVN, OCTO, OCTO, HEMA } }, TAGT, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            //타겟 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0850 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, OCTO, CLAM, ANNY }, new int[] { CLAM, STAR, OCTO, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, STAR, OCTO, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, FISH, MENG, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, FISH, MENG, ANNY }, new int[] { SEVN, MENG, OCTO, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, OCTO, CLAM, JOKE }, new int[] { ANNY, STAR, OCTO, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT } }, TAGT, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, STAR, OCTO, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, FISH, MENG, BOGA } }, TAGT, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, FISH, MENG, BOGA }, new int[] { ANNY, MENG, OCTO, HEMA } }, TAGT, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0850 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, FISH, CLAM, ANNY }, new int[] { CLAM, CLAM, OCTO, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, CLAM, OCTO, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, BOGA, MENG, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, BOGA, MENG, ANNY }, new int[] { SEVN, OCTO, OCTO, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, FISH, CLAM, JOKE }, new int[] { ANNY, CLAM, OCTO, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT } }, TAGT, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, CLAM, OCTO, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, BOGA, MENG, BOGA } }, TAGT, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, BOGA, MENG, BOGA }, new int[] { ANNY, OCTO, OCTO, HEMA } }, TAGT, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0850 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, OCTO, BOGA, ANNY }, new int[] { CLAM, STAR, JOKE, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, STAR, JOKE, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, FISH, CLAM, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, FISH, CLAM, ANNY }, new int[] { SEVN, MENG, OCTO, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, OCTO, BOGA, JOKE }, new int[] { ANNY, STAR, JOKE, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT } }, TAGT, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, STAR, JOKE, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, FISH, CLAM, BOGA } }, TAGT, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, FISH, CLAM, BOGA }, new int[] { ANNY, MENG, OCTO, HEMA } }, TAGT, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0850 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { MENG, FISH, BOGA, ANNY }, new int[] { CLAM, CLAM, JOKE, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 0 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { CLAM, CLAM, JOKE, ANNY }, new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, BOGA, CLAM, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { TAGT, TAGT, TAGT, ANNY }, new int[] { STAR, BOGA, CLAM, ANNY }, new int[] { SEVN, OCTO, OCTO, ANNY } }, TAGT, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, FISH, BOGA, JOKE }, new int[] { ANNY, CLAM, JOKE, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT } }, TAGT, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, CLAM, JOKE, OCTO }, new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, BOGA, CLAM, BOGA } }, TAGT, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, TAGT, TAGT, TAGT }, new int[] { ANNY, BOGA, CLAM, BOGA }, new int[] { ANNY, OCTO, OCTO, HEMA } }, TAGT, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하


            //조개 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, MENG, JOKE, FISH }, new int[] { MENG, BAR3, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM } }, CLAM, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 4 } ));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(7600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, BAR3, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, JOKE, OCTO, BAR3 } }, CLAM, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, JOKE, OCTO, BAR3 }, new int[] { STAR, MENG, TAGT, OCTO } }, CLAM, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, BOGA, JOKE, FISH }, new int[] { MENG, FISH, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM } }, CLAM, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(7600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, FISH, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, TAGT, OCTO, BAR3 } }, CLAM, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, TAGT, OCTO, BAR3 }, new int[] { STAR, BOGA, TAGT, OCTO } }, CLAM, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, MENG, JOKE, BOGA }, new int[] { MENG, BAR3, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM } }, CLAM, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(7600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, BAR3, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, JOKE, OCTO, STAR } }, CLAM, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, JOKE, OCTO, STAR }, new int[] { STAR, MENG, TAGT, BOGA } }, CLAM, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, BOGA, JOKE, BOGA }, new int[] { MENG, FISH, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM } }, CLAM, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(7600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, FISH, TAGT, HEMA }, new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, TAGT, OCTO, STAR } }, CLAM, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(3750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, CLAM }, new int[] { TAGT, TAGT, OCTO, STAR }, new int[] { STAR, BOGA, TAGT, BOGA } }, CLAM, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            //조개 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, MENG, JOKE, ANNY }, new int[] { MENG, BAR3, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, BAR3, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, JOKE, OCTO, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, JOKE, OCTO, ANNY }, new int[] { STAR, MENG, TAGT, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, MENG, JOKE, FISH }, new int[] { ANNY, BAR3, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM } }, CLAM, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, BAR3, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, JOKE, OCTO, BAR3 } }, CLAM, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, JOKE, OCTO, BAR3 }, new int[] { ANNY, MENG, TAGT, OCTO } }, CLAM, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, BOGA, JOKE, ANNY }, new int[] { MENG, FISH, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, FISH, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, TAGT, OCTO, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, TAGT, OCTO, ANNY }, new int[] { STAR, BOGA, TAGT, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, BOGA, JOKE, FISH }, new int[] { ANNY, FISH, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM } }, CLAM, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, FISH, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, TAGT, OCTO, BAR3 } }, CLAM, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, TAGT, OCTO, BAR3 }, new int[] { ANNY, BOGA, TAGT, OCTO } }, CLAM, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, MENG, JOKE, ANNY }, new int[] { MENG, BAR3, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, BAR3, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, JOKE, OCTO, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, JOKE, OCTO, ANNY }, new int[] { STAR, MENG, TAGT, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, MENG, JOKE, BOGA }, new int[] { ANNY, BAR3, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM } }, CLAM, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, BAR3, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, JOKE, OCTO, STAR } }, CLAM, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, JOKE, OCTO, STAR }, new int[] { ANNY, MENG, TAGT, BOGA } }, CLAM, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { HEMA, BOGA, JOKE, ANNY }, new int[] { MENG, FISH, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 8, 9, 10, 4 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { MENG, FISH, TAGT, ANNY }, new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, TAGT, OCTO, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 0 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { CLAM, CLAM, CLAM, ANNY }, new int[] { TAGT, TAGT, OCTO, ANNY }, new int[] { STAR, BOGA, TAGT, ANNY } }, CLAM, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, BOGA, JOKE, BOGA }, new int[] { ANNY, FISH, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM } }, CLAM, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, FISH, TAGT, HEMA }, new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, TAGT, OCTO, STAR } }, CLAM, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, CLAM, CLAM, CLAM }, new int[] { ANNY, TAGT, OCTO, STAR }, new int[] { ANNY, BOGA, TAGT, BOGA } }, CLAM, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하


            //헤마 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, SEVN }, new int[] { BOGA, OCTO, SEVN, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, SEVN, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BAR3, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2750 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BAR3, CLAM }, new int[] { CLAM, BOGA, FISH, BAR3 } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 4 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BAR3, SEVN, OCTO, SEVN }, new int[] { JOKE, OCTO, SEVN, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { JOKE, OCTO, SEVN, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { FISH, SEVN, BAR3, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { FISH, SEVN, BAR3, CLAM }, new int[] { OCTO, BOGA, FISH, BAR3 } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, SEVN }, new int[] { BOGA, OCTO, FISH, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, FISH, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BOGA, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BOGA, CLAM }, new int[] { CLAM, BOGA, STAR, BAR3 } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 4 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BAR3, SEVN, OCTO, SEVN }, new int[] { JOKE, OCTO, FISH, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { JOKE, OCTO, FISH, FISH }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { FISH, SEVN, BOGA, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { FISH, SEVN, BOGA, CLAM }, new int[] { OCTO, BOGA, STAR, BAR3 } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, TAGT }, new int[] { BOGA, OCTO, SEVN, BOGA }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, SEVN, BOGA }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BAR3, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BAR3, CLAM }, new int[] { CLAM, BOGA, FISH, STAR } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 4 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, TAGT }, new int[] { BOGA, OCTO, FISH, BOGA }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, FISH, BOGA }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BAR3, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { MENG, SEVN, BOGA, CLAM }, new int[] { CLAM, BOGA, FISH, STAR } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 4 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BAR3, SEVN, OCTO, TAGT }, new int[] { JOKE, OCTO, FISH, BOGA }, new int[] { HEMA, HEMA, HEMA, HEMA } }, HEMA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { JOKE, OCTO, FISH, BOGA }, new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { FISH, SEVN, BAR3, CLAM } }, HEMA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, HEMA }, new int[] { FISH, SEVN, BOGA, CLAM }, new int[] { OCTO, BOGA, FISH, STAR } }, HEMA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            //헤마 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, ANNY }, new int[] { BOGA, OCTO, SEVN, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, SEVN, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { MENG, SEVN, BAR3, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { MENG, SEVN, BAR3, ANNY }, new int[] { CLAM, BOGA, FISH, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 4 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, SEVN, OCTO, SEVN }, new int[] { ANNY, OCTO, SEVN, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA } }, HEMA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, SEVN, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BAR3, CLAM } }, HEMA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BAR3, CLAM }, new int[] { ANNY, BOGA, FISH, BAR3 } }, HEMA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BAR3, SEVN, OCTO, ANNY }, new int[] { JOKE, OCTO, SEVN, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { JOKE, OCTO, SEVN, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { FISH, SEVN, BAR3, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { FISH, SEVN, BAR3, ANNY }, new int[] { OCTO, BOGA, FISH, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, SEVN, OCTO, SEVN }, new int[] { ANNY, OCTO, SEVN, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA } }, HEMA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, SEVN, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BAR3, CLAM } }, HEMA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BAR3, CLAM }, new int[] { ANNY, BOGA, FISH, BAR3 } }, HEMA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, ANNY }, new int[] { BOGA, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { MENG, SEVN, BOGA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { MENG, SEVN, BOGA, ANNY }, new int[] { CLAM, BOGA, STAR, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 4 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, SEVN, OCTO, SEVN }, new int[] { ANNY, OCTO, FISH, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA } }, HEMA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, FISH, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BOGA, CLAM } }, HEMA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BOGA, CLAM }, new int[] { ANNY, BOGA, STAR, BAR3 } }, HEMA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BAR3, SEVN, OCTO, ANNY }, new int[] { JOKE, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { JOKE, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { FISH, SEVN, BOGA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { FISH, SEVN, BOGA, ANNY }, new int[] { OCTO, BOGA, STAR, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, SEVN, OCTO, SEVN }, new int[] { ANNY, OCTO, FISH, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA } }, HEMA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, FISH, FISH }, new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BOGA, CLAM } }, HEMA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BOGA, CLAM }, new int[] { ANNY, BOGA, STAR, BAR3 } }, HEMA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, OCTO, ANNY }, new int[] { BOGA, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { MENG, SEVN, BAR3, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 4, 5, 6, 8 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { MENG, SEVN, BOGA, ANNY }, new int[] { CLAM, BOGA, FISH, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 4 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, SEVN, OCTO, TAGT }, new int[] { ANNY, OCTO, FISH, BOGA }, new int[] { ANNY, HEMA, HEMA, HEMA } }, HEMA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, FISH, BOGA }, new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BAR3, CLAM } }, HEMA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BOGA, CLAM }, new int[] { ANNY, BOGA, FISH, STAR } }, HEMA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BAR3, SEVN, OCTO, ANNY }, new int[] { JOKE, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { JOKE, OCTO, FISH, ANNY }, new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { FISH, SEVN, BAR3, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { HEMA, HEMA, HEMA, ANNY }, new int[] { FISH, SEVN, BOGA, ANNY }, new int[] { OCTO, BOGA, FISH, ANNY } }, HEMA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, SEVN, OCTO, TAGT }, new int[] { ANNY, OCTO, FISH, BOGA }, new int[] { ANNY, HEMA, HEMA, HEMA } }, HEMA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, FISH, BOGA }, new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BAR3, CLAM } }, HEMA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, HEMA, HEMA, HEMA }, new int[] { ANNY, SEVN, BOGA, CLAM }, new int[] { ANNY, BOGA, FISH, STAR } }, HEMA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하


            //보가지 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, CLAM, BAR3, OCTO }, new int[] { OCTO, TAGT, FISH, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, TAGT, FISH, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, OCTO, JOKE, HEMA } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, OCTO, JOKE, HEMA }, new int[] { MENG, STAR, TAGT, CLAM } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 8 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, HEMA, BAR3, OCTO }, new int[] { OCTO, SEVN, FISH, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, SEVN, FISH, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, FISH, JOKE, HEMA } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, FISH, JOKE, HEMA }, new int[] { MENG, CLAM, TAGT, CLAM } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 8 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, CLAM, FISH, OCTO }, new int[] { OCTO, TAGT, HEMA, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, TAGT, HEMA, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, OCTO, STAR, HEMA } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, OCTO, STAR, HEMA }, new int[] { MENG, STAR, MENG, CLAM } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 8 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, HEMA, FISH, OCTO }, new int[] { OCTO, SEVN, HEMA, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, SEVN, HEMA, TAGT }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, FISH, STAR, HEMA } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { HEMA, FISH, STAR, HEMA }, new int[] { MENG, CLAM, MENG, CLAM } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 8 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, CLAM, BAR3, CLAM }, new int[] { FISH, TAGT, FISH, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { FISH, TAGT, FISH, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, OCTO, JOKE, MENG } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1450 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, OCTO, JOKE, MENG }, new int[] { STAR, STAR, TAGT, FISH } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7}));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, HEMA, BAR3, CLAM }, new int[] { FISH, SEVN, FISH, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { FISH, SEVN, FISH, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, FISH, JOKE, MENG } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1450 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, FISH, JOKE, MENG }, new int[] { STAR, CLAM, TAGT, FISH } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, CLAM, FISH, CLAM }, new int[] { FISH, TAGT, HEMA, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { FISH, TAGT, HEMA, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, OCTO, STAR, MENG } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1450 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, OCTO, STAR, MENG }, new int[] { STAR, STAR, MENG, FISH } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, HEMA, FISH, CLAM }, new int[] { FISH, SEVN, HEMA, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA } }, BOGA, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { FISH, SEVN, HEMA, STAR }, new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, FISH, STAR, MENG } }, BOGA, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1450 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, BOGA }, new int[] { OCTO, FISH, STAR, MENG }, new int[] { STAR, CLAM, MENG, FISH } }, BOGA, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하


            //보가지 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, CLAM, BAR3, ANNY }, new int[] { OCTO, TAGT, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, TAGT, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, OCTO, JOKE, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, OCTO, JOKE, ANNY }, new int[] { MENG, STAR, TAGT, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 8}));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, BAR3, OCTO }, new int[] { ANNY, TAGT, FISH, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, TAGT, FISH, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, JOKE, HEMA } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, JOKE, HEMA }, new int[] { ANNY, STAR, TAGT, CLAM } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, HEMA, BAR3, ANNY }, new int[] { OCTO, SEVN, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, SEVN, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, FISH, JOKE, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, FISH, JOKE, ANNY }, new int[] { MENG, CLAM, TAGT, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 8 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, HEMA, BAR3, OCTO }, new int[] { ANNY, SEVN, FISH, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, SEVN, FISH, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, JOKE, HEMA } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, JOKE, HEMA }, new int[] { ANNY, CLAM, TAGT, CLAM } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, CLAM, FISH, ANNY }, new int[] { OCTO, TAGT, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, TAGT, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, OCTO, STAR, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, OCTO, STAR, ANNY }, new int[] { MENG, STAR, MENG, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 8 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, FISH, OCTO }, new int[] { ANNY, TAGT, HEMA, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, TAGT, HEMA, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, STAR, HEMA } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, STAR, HEMA }, new int[] { ANNY, STAR, MENG, CLAM } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, HEMA, FISH, ANNY }, new int[] { OCTO, SEVN, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { OCTO, SEVN, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, FISH, STAR, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 0, 1 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { HEMA, FISH, STAR, ANNY }, new int[] { MENG, CLAM, MENG, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2, 8 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, HEMA, FISH, OCTO }, new int[] { ANNY, SEVN, HEMA, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, SEVN, HEMA, TAGT }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, STAR, HEMA } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, STAR, HEMA }, new int[] { ANNY, CLAM, MENG, CLAM } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, CLAM, BAR3, ANNY }, new int[] { FISH, TAGT, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { FISH, TAGT, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, OCTO, JOKE, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, OCTO, JOKE, ANNY }, new int[] { STAR, STAR, TAGT, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, BAR3, CLAM }, new int[] { ANNY, TAGT, FISH, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, TAGT, FISH, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, JOKE, MENG } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11}));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, JOKE, MENG }, new int[] { ANNY, STAR, TAGT, FISH } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, HEMA, BAR3, ANNY }, new int[] { FISH, SEVN, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { FISH, SEVN, FISH, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, FISH, JOKE, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, FISH, JOKE, ANNY }, new int[] { STAR, CLAM, TAGT, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, HEMA, BAR3, CLAM }, new int[] { ANNY, SEVN, FISH, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, SEVN, FISH, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, JOKE, MENG } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, JOKE, MENG }, new int[] { ANNY, CLAM, TAGT, FISH } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, CLAM, FISH, ANNY }, new int[] { FISH, TAGT, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { FISH, TAGT, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, OCTO, STAR, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, OCTO, STAR, ANNY }, new int[] { STAR, STAR, MENG, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, FISH, CLAM }, new int[] { ANNY, TAGT, HEMA, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, TAGT, HEMA, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, STAR, MENG } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, OCTO, STAR, MENG }, new int[] { ANNY, STAR, MENG, FISH } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, HEMA, FISH, ANNY }, new int[] { FISH, SEVN, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { FISH, SEVN, HEMA, ANNY }, new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, FISH, STAR, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, BOGA, ANNY }, new int[] { OCTO, FISH, STAR, ANNY }, new int[] { STAR, CLAM, MENG, ANNY } }, BOGA, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, HEMA, FISH, CLAM }, new int[] { ANNY, SEVN, HEMA, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA } }, BOGA, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, SEVN, HEMA, STAR }, new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, STAR, MENG } }, BOGA, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, BOGA, BOGA, BOGA }, new int[] { ANNY, FISH, STAR, MENG }, new int[] { ANNY, CLAM, MENG, FISH } }, BOGA, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하


            //물고기 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { OCTO, STAR, HEMA, OCTO }, new int[] { STAR, TAGT, BAR3, MENG }, new int[] { FISH, FISH, FISH, FISH } }, FISH, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { STAR, TAGT, BAR3, MENG }, new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, MENG, BOGA, JOKE } }, FISH, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 3}));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, MENG, BOGA, JOKE }, new int[] { BOGA, BAR3, JOKE, OCTO } }, FISH, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { OCTO, SEVN, HEMA, OCTO }, new int[] { STAR, BOGA, BAR3, MENG }, new int[] { FISH, FISH, FISH, FISH } }, FISH, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { STAR, BOGA, BAR3, MENG }, new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, CLAM, BOGA, JOKE } }, FISH, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, CLAM, BOGA, JOKE }, new int[] { BOGA, TAGT, JOKE, OCTO } }, FISH, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { OCTO, SEVN, MENG, OCTO }, new int[] { STAR, BOGA, OCTO, MENG }, new int[] { FISH, FISH, FISH, FISH } }, FISH, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { STAR, BOGA, OCTO, MENG }, new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, CLAM, HEMA, JOKE } }, FISH, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, CLAM, HEMA, JOKE }, new int[] { BOGA, TAGT, BOGA, OCTO } }, FISH, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0700 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { OCTO, STAR, MENG, OCTO }, new int[] { STAR, TAGT, OCTO, MENG }, new int[] { FISH, FISH, FISH, FISH } }, FISH, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1100 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { STAR, TAGT, OCTO, MENG }, new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, MENG, HEMA, JOKE } }, FISH, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, FISH }, new int[] { OCTO, MENG, HEMA, JOKE }, new int[] { BOGA, BAR3, BOGA, OCTO } }, FISH, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            //물고기 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, STAR, HEMA, ANNY }, new int[] { STAR, TAGT, BAR3, ANNY }, new int[] { FISH, FISH, FISH, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0300 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, TAGT, BAR3, ANNY }, new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, MENG, BOGA, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, MENG, BOGA, ANNY }, new int[] { BOGA, BAR3, JOKE, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, STAR, HEMA, OCTO }, new int[] { ANNY, TAGT, BAR3, MENG }, new int[] { ANNY, FISH, FISH, FISH } }, FISH, COUNT_3, MULTI_1, new int[] { 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, TAGT, BAR3, MENG }, new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, MENG, BOGA, JOKE } }, FISH, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, MENG, BOGA, JOKE }, new int[] { ANNY, BAR3, JOKE, OCTO } }, FISH, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, HEMA, ANNY }, new int[] { STAR, BOGA, BAR3, ANNY }, new int[] { FISH, FISH, FISH, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0300 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, BOGA, BAR3, ANNY }, new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, CLAM, BOGA, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, CLAM, BOGA, ANNY }, new int[] { BOGA, TAGT, JOKE, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, SEVN, HEMA, OCTO }, new int[] { ANNY, BOGA, BAR3, MENG }, new int[] { ANNY, FISH, FISH, FISH } }, FISH, COUNT_3, MULTI_1, new int[] { 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, BOGA, BAR3, MENG }, new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, CLAM, BOGA, JOKE } }, FISH, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, CLAM, BOGA, JOKE }, new int[] { ANNY, TAGT, JOKE, OCTO } }, FISH, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, SEVN, MENG, ANNY }, new int[] { STAR, BOGA, OCTO, ANNY }, new int[] { FISH, FISH, FISH, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0300 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, BOGA, OCTO, ANNY }, new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, CLAM, HEMA, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, CLAM, HEMA, ANNY }, new int[] { BOGA, TAGT, BOGA, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, SEVN, MENG, OCTO }, new int[] { ANNY, BOGA, OCTO, MENG }, new int[] { ANNY, FISH, FISH, FISH } }, FISH, COUNT_3, MULTI_1, new int[] { 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, BOGA, OCTO, MENG }, new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, CLAM, HEMA, JOKE } }, FISH, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, CLAM, HEMA, JOKE }, new int[] { ANNY, TAGT, BOGA, OCTO } }, FISH, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { OCTO, STAR, MENG, ANNY }, new int[] { STAR, TAGT, OCTO, ANNY }, new int[] { FISH, FISH, FISH, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0300 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { STAR, TAGT, OCTO, ANNY }, new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, MENG, HEMA, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { FISH, FISH, FISH, ANNY }, new int[] { OCTO, MENG, HEMA, ANNY }, new int[] { BOGA, BAR3, BOGA, ANNY } }, FISH, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0350 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, STAR, MENG, OCTO }, new int[] { ANNY, TAGT, OCTO, MENG }, new int[] { ANNY, FISH, FISH, FISH } }, FISH, COUNT_3, MULTI_1, new int[] { 9, 10, 11, 7 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, TAGT, OCTO, MENG }, new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, MENG, HEMA, JOKE } }, FISH, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 3 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0150 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, FISH, FISH, FISH }, new int[] { ANNY, MENG, HEMA, JOKE }, new int[] { ANNY, BAR3, BOGA, OCTO } }, FISH, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하


            //문어 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { STAR, TAGT, STAR, CLAM }, new int[] { FISH, BOGA, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO } }, OCTO, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { FISH, BOGA, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { BOGA, STAR, SEVN, MENG } }, OCTO, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { BOGA, STAR, SEVN, MENG }, new int[] { HEMA, TAGT, HEMA, FISH } }, OCTO, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, TAGT, STAR, CLAM }, new int[] { BOGA, BOGA, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO } }, OCTO, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, BOGA, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { STAR, STAR, SEVN, MENG } }, OCTO, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { BOGA, STAR, SEVN, MENG }, new int[] { FISH, TAGT, HEMA, FISH } }, OCTO, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, MENG, STAR, CLAM }, new int[] { BOGA, SEVN, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO } }, OCTO, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, SEVN, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { STAR, HEMA, SEVN, MENG } }, OCTO, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { BOGA, HEMA, SEVN, MENG }, new int[] { FISH, SEVN, HEMA, FISH } }, OCTO, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0750 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, MENG, TAGT, CLAM }, new int[] { BOGA, SEVN, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO } }, OCTO, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1600 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { BOGA, SEVN, MENG, BAR3 }, new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { STAR, HEMA, FISH, MENG } }, OCTO, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0950 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, OCTO }, new int[] { BOGA, HEMA, FISH, MENG }, new int[] { FISH, SEVN, HEMA, FISH } }, OCTO, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3, 7 }));   //하

            //문어 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { STAR, TAGT, STAR, ANNY }, new int[] { FISH, BOGA, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { FISH, BOGA, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { BOGA, STAR, SEVN, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { BOGA, STAR, SEVN, ANNY }, new int[] { HEMA, TAGT, HEMA, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, TAGT, STAR, CLAM }, new int[] { ANNY, BOGA, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO } }, OCTO, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, BOGA, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, STAR, SEVN, MENG } }, OCTO, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, STAR, SEVN, MENG }, new int[] { ANNY, TAGT, HEMA, FISH } }, OCTO, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, TAGT, STAR, ANNY }, new int[] { BOGA, BOGA, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { BOGA, BOGA, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { STAR, STAR, SEVN, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { BOGA, STAR, SEVN, ANNY }, new int[] { FISH, TAGT, HEMA, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, TAGT, STAR, CLAM }, new int[] { ANNY, BOGA, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO } }, OCTO, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, BOGA, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, STAR, SEVN, MENG } }, OCTO, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, STAR, SEVN, MENG }, new int[] { ANNY, TAGT, HEMA, FISH } }, OCTO, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, MENG, STAR, ANNY }, new int[] { BOGA, SEVN, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { BOGA, SEVN, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { STAR, HEMA, SEVN, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { BOGA, HEMA, SEVN, ANNY }, new int[] { FISH, SEVN, HEMA, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, MENG, STAR, CLAM }, new int[] { ANNY, SEVN, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO } }, OCTO, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, SEVN, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, HEMA, SEVN, MENG } }, OCTO, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, HEMA, SEVN, MENG }, new int[] { ANNY, SEVN, HEMA, FISH } }, OCTO, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { FISH, MENG, TAGT, ANNY }, new int[] { BOGA, SEVN, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { BOGA, SEVN, MENG, ANNY }, new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { STAR, HEMA, FISH, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { OCTO, OCTO, OCTO, ANNY }, new int[] { BOGA, HEMA, FISH, ANNY }, new int[] { FISH, SEVN, HEMA, ANNY } }, OCTO, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, MENG, TAGT, CLAM }, new int[] { ANNY, SEVN, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO } }, OCTO, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 1 }, new int[][] { new int[] { ANNY, SEVN, MENG, BAR3 }, new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, HEMA, FISH, MENG } }, OCTO, COUNT_3, MULTI_1, new int[] { 5, 6, 7, 11 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0400 * MULTI_1, new int[] { 1, 1, 0 }, new int[][] { new int[] { ANNY, OCTO, OCTO, OCTO }, new int[] { ANNY, HEMA, FISH, MENG }, new int[] { ANNY, SEVN, HEMA, FISH } }, OCTO, COUNT_3, MULTI_1, new int[] { 1, 2, 3, 7 }));   //하


            //멍 X 4
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, TAGT, BOGA, BAR3 }, new int[] { HEMA, FISH, STAR, OCTO }, new int[] { MENG, MENG, MENG, MENG } }, MENG, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, FISH, STAR, OCTO }, new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, BAR3, OCTO, FISH } }, MENG, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, BAR3, OCTO, FISH }, new int[] { TAGT, CLAM, SEVN, JOKE } }, MENG, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, OCTO, BAR3 }, new int[] { HEMA, JOKE, TAGT, OCTO }, new int[] { MENG, MENG, MENG, MENG } }, MENG, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, TAGT, OCTO }, new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, SEVN, OCTO, FISH } }, MENG, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, SEVN, OCTO, FISH }, new int[] { TAGT, OCTO, FISH, JOKE } }, MENG, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, OCTO, STAR }, new int[] { HEMA, JOKE, TAGT, BOGA }, new int[] { MENG, MENG, MENG, MENG } }, MENG, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, TAGT, BOGA }, new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, SEVN, OCTO, FISH } }, MENG, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, SEVN, OCTO, FISH }, new int[] { TAGT, OCTO, FISH, SEVN } }, MENG, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, BOGA, STAR }, new int[] { HEMA, JOKE, STAR, BOGA }, new int[] { MENG, MENG, MENG, MENG } }, MENG, COUNT_4, MULTI_1, new int[] { 8, 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(5000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, STAR, BOGA }, new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, SEVN, OCTO, FISH } }, MENG, COUNT_4, MULTI_1, new int[] { 4, 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, MENG }, new int[] { CLAM, SEVN, OCTO, FISH }, new int[] { TAGT, OCTO, SEVN, SEVN } }, MENG, COUNT_4, MULTI_1, new int[] { 0, 1, 2, 3 }));   //하


            //멍 X 3
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, TAGT, BOGA, ANNY }, new int[] { HEMA, FISH, STAR, ANNY }, new int[] { MENG, MENG, MENG, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, FISH, STAR, ANNY }, new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, BAR3, OCTO, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, BAR3, OCTO, ANNY }, new int[] { TAGT, CLAM, SEVN, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, TAGT, BOGA, BAR3 }, new int[] { ANNY, FISH, STAR, OCTO }, new int[] { ANNY, MENG, MENG, MENG } }, MENG, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, FISH, STAR, OCTO }, new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, BAR3, OCTO, FISH } }, MENG, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, BAR3, OCTO, FISH }, new int[] { ANNY, CLAM, SEVN, JOKE } }, MENG, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, OCTO, ANNY }, new int[] { HEMA, JOKE, TAGT, ANNY }, new int[] { MENG, MENG, MENG, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, TAGT, ANNY }, new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, SEVN, OCTO, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, SEVN, OCTO, ANNY }, new int[] { TAGT, OCTO, FISH, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, OCTO, BAR3 }, new int[] { ANNY, JOKE, TAGT, OCTO }, new int[] { ANNY, MENG, MENG, MENG } }, MENG, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, JOKE, TAGT, OCTO }, new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, SEVN, OCTO, FISH } }, MENG, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(2500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, SEVN, OCTO, FISH }, new int[] { ANNY, OCTO, FISH, JOKE } }, MENG, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, OCTO, ANNY }, new int[] { HEMA, JOKE, TAGT, ANNY }, new int[] { MENG, MENG, MENG, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, TAGT, ANNY }, new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, SEVN, OCTO, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, SEVN, OCTO, ANNY }, new int[] { TAGT, OCTO, FISH, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, OCTO, STAR }, new int[] { ANNY, JOKE, TAGT, BOGA }, new int[] { ANNY, MENG, MENG, MENG } }, MENG, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, JOKE, TAGT, BOGA }, new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, SEVN, OCTO, FISH } }, MENG, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, SEVN, OCTO, FISH }, new int[] { ANNY, OCTO, FISH, SEVN } }, MENG, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, BOGA, ANNY }, new int[] { HEMA, JOKE, STAR, ANNY }, new int[] { MENG, MENG, MENG, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 8, 9, 10 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, STAR, ANNY }, new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, SEVN, OCTO, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 4, 5, 6 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, MENG, ANNY }, new int[] { CLAM, SEVN, OCTO, ANNY }, new int[] { TAGT, OCTO, SEVN, ANNY } }, MENG, COUNT_3, MULTI_1, new int[] { 0, 1, 2 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, CLAM, BOGA, STAR }, new int[] { ANNY, JOKE, STAR, BOGA }, new int[] { ANNY, MENG, MENG, MENG } }, MENG, COUNT_3, MULTI_1, new int[] { 9, 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(1000 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, JOKE, STAR, BOGA }, new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, SEVN, OCTO, FISH } }, MENG, COUNT_3, MULTI_1, new int[] { 5, 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, MENG, MENG, MENG }, new int[] { ANNY, SEVN, OCTO, FISH }, new int[] { ANNY, OCTO, SEVN, SEVN } }, MENG, COUNT_3, MULTI_1, new int[] { 1, 2, 3 }));   //하


            //멍 X 2
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, TAGT, ANNY, ANNY }, new int[] { HEMA, FISH, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 8, 9 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, FISH, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, BAR3, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 4, 5 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, BAR3, ANNY, ANNY }, new int[] { TAGT, CLAM, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 0, 1 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, BOGA, BAR3 }, new int[] { ANNY, ANNY, STAR, OCTO }, new int[] { ANNY, ANNY, MENG, MENG } }, MENG, COUNT_2, MULTI_1, new int[] { 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, STAR, OCTO }, new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH } }, MENG, COUNT_2, MULTI_1, new int[] { 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH }, new int[] { ANNY, ANNY, SEVN, JOKE } }, MENG, COUNT_2, MULTI_1, new int[] { 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, ANNY, ANNY }, new int[] { HEMA, JOKE, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 8, 9 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, SEVN, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 4, 5 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, SEVN, ANNY, ANNY }, new int[] { TAGT, OCTO, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 0, 1 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, OCTO, BAR3 }, new int[] { ANNY, ANNY, TAGT, OCTO }, new int[] { ANNY, ANNY, MENG, MENG } }, MENG, COUNT_2, MULTI_1, new int[] { 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, TAGT, OCTO }, new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH } }, MENG, COUNT_2, MULTI_1, new int[] { 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH }, new int[] { ANNY, ANNY, FISH, JOKE } }, MENG, COUNT_2, MULTI_1, new int[] { 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, ANNY, ANNY }, new int[] { HEMA, JOKE, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 8, 9 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, SEVN, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 4, 5 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, SEVN, ANNY, ANNY }, new int[] { TAGT, OCTO, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 0, 1 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, OCTO, STAR }, new int[] { ANNY, ANNY, TAGT, BOGA }, new int[] { ANNY, ANNY, MENG, MENG } }, MENG, COUNT_2, MULTI_1, new int[] { 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, TAGT, BOGA }, new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH } }, MENG, COUNT_2, MULTI_1, new int[] { 6, 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH }, new int[] { ANNY, ANNY, FISH, SEVN } }, MENG, COUNT_2, MULTI_1, new int[] { 2, 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, CLAM, ANNY, ANNY }, new int[] { HEMA, JOKE, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 8, 9 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, JOKE, ANNY, ANNY }, new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, SEVN, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 4, 5 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, MENG, ANNY, ANNY }, new int[] { CLAM, SEVN, ANNY, ANNY }, new int[] { TAGT, OCTO, ANNY, ANNY } }, MENG, COUNT_2, MULTI_1, new int[] { 0, 1 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, BOGA, STAR }, new int[] { ANNY, ANNY, STAR, BOGA }, new int[] { ANNY, ANNY, MENG, MENG } }, MENG, COUNT_2, MULTI_1, new int[] { 10, 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0500 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, STAR, BOGA }, new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH } }, MENG, COUNT_2, MULTI_1, new int[] { 6, 7 }));   //중
            //m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0250 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, MENG, MENG }, new int[] { ANNY, ANNY, OCTO, FISH }, new int[] { ANNY, ANNY, SEVN, SEVN } }, MENG, COUNT_2, MULTI_1, new int[] { 2, 3 }));   //하


            //멍 X 1
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, ANNY, ANNY, ANNY }, new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 8 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 4 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY }, new int[] { TAGT, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 0 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, ANNY, BAR3 }, new int[] { ANNY, ANNY, ANNY, OCTO }, new int[] { ANNY, ANNY, ANNY, MENG } }, MENG, COUNT_1, MULTI_1, new int[] { 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, OCTO }, new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH } }, MENG, COUNT_1, MULTI_1, new int[] { 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH }, new int[] { ANNY, ANNY, ANNY, JOKE } }, MENG, COUNT_1, MULTI_1, new int[] { 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, ANNY, ANNY, ANNY }, new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 8 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 4 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY }, new int[] { TAGT, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 0 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, ANNY, BAR3 }, new int[] { ANNY, ANNY, ANNY, OCTO }, new int[] { ANNY, ANNY, ANNY, MENG } }, MENG, COUNT_1, MULTI_1, new int[] { 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, OCTO }, new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH } }, MENG, COUNT_1, MULTI_1, new int[] { 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH }, new int[] { ANNY, ANNY, ANNY, JOKE } }, MENG, COUNT_1, MULTI_1, new int[] { 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, ANNY, ANNY, ANNY }, new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 8 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 4 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY }, new int[] { TAGT, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 0 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, ANNY, STAR }, new int[] { ANNY, ANNY, ANNY, BOGA }, new int[] { ANNY, ANNY, ANNY, MENG } }, MENG, COUNT_1, MULTI_1, new int[] { 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, BOGA }, new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH } }, MENG, COUNT_1, MULTI_1, new int[] { 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH }, new int[] { ANNY, ANNY, ANNY, SEVN } }, MENG, COUNT_1, MULTI_1, new int[] { 3 }));   //하

            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { BOGA, ANNY, ANNY, ANNY }, new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 8 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { HEMA, ANNY, ANNY, ANNY }, new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 4 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { MENG, ANNY, ANNY, ANNY }, new int[] { CLAM, ANNY, ANNY, ANNY }, new int[] { TAGT, ANNY, ANNY, ANNY } }, MENG, COUNT_1, MULTI_1, new int[] { 0 }));   //하
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 0, 0, 1 }, new int[][] { new int[] { ANNY, ANNY, ANNY, STAR }, new int[] { ANNY, ANNY, ANNY, BOGA }, new int[] { ANNY, ANNY, ANNY, MENG } }, MENG, COUNT_1, MULTI_1, new int[] { 11 }));   //상
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0200 * MULTI_1, new int[] { 0, 1, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, BOGA }, new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH } }, MENG, COUNT_1, MULTI_1, new int[] { 7 }));   //중
            m_lstSEAScoreTable.Add(new SEAScoreTableInfo(0100 * MULTI_1, new int[] { 1, 0, 0 }, new int[][] { new int[] { ANNY, ANNY, ANNY, MENG }, new int[] { ANNY, ANNY, ANNY, FISH }, new int[] { ANNY, ANNY, ANNY, SEVN } }, MENG, COUNT_1, MULTI_1, new int[] { 3 }));   //하


            m_lstSeaScores = new List<int>();
            for (int i = 0; i < m_lstSEAScoreTable.Count; i++)
            {
                int nScore = m_lstSEAScoreTable[i].nScore;
                if (!m_lstSeaScores.Exists(value => value == nScore))
                {
                    m_lstSeaScores.Add(nScore);
                }
            }
            m_lstSeaScores = m_lstSeaScores.OrderBy(value => value).ToList();

            m_lstTile = new List<int>();
            for (int i = 0; i < 11; i++)
            {
                m_lstTile.Add(i);
            }

            #region Original Tile Line
            m_lstOriginalTile[0].Add(HEMA);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(OCTO);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(BOGA);
            m_lstOriginalTile[0].Add(OCTO);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(FISH);
            m_lstOriginalTile[0].Add(OCTO);
            m_lstOriginalTile[0].Add(BOGA);
            m_lstOriginalTile[0].Add(HEMA);
            m_lstOriginalTile[0].Add(MENG);
            m_lstOriginalTile[0].Add(CLAM);
            m_lstOriginalTile[0].Add(TAGT);
            m_lstOriginalTile[0].Add(STAR);
            m_lstOriginalTile[0].Add(SEVN);
            m_lstOriginalTile[0].Add(BAR3);
            m_lstOriginalTile[0].Add(JOKE);


            m_lstOriginalTile[1].Add(MENG);
            m_lstOriginalTile[1].Add(SEVN);
            m_lstOriginalTile[1].Add(OCTO);
            m_lstOriginalTile[1].Add(HEMA);
            m_lstOriginalTile[1].Add(SEVN);
            m_lstOriginalTile[1].Add(BOGA);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(CLAM);
            m_lstOriginalTile[1].Add(TAGT);
            m_lstOriginalTile[1].Add(BOGA);
            m_lstOriginalTile[1].Add(OCTO);
            m_lstOriginalTile[1].Add(STAR);
            m_lstOriginalTile[1].Add(TAGT);
            m_lstOriginalTile[1].Add(FISH);
            m_lstOriginalTile[1].Add(MENG);
            m_lstOriginalTile[1].Add(BAR3);
            m_lstOriginalTile[1].Add(CLAM);
            m_lstOriginalTile[1].Add(JOKE);


            m_lstOriginalTile[2].Add(TAGT);
            m_lstOriginalTile[2].Add(CLAM);
            m_lstOriginalTile[2].Add(OCTO);
            m_lstOriginalTile[2].Add(TAGT);
            m_lstOriginalTile[2].Add(MENG);
            m_lstOriginalTile[2].Add(OCTO);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(HEMA);
            m_lstOriginalTile[2].Add(BOGA);
            m_lstOriginalTile[2].Add(STAR);
            m_lstOriginalTile[2].Add(MENG);
            m_lstOriginalTile[2].Add(OCTO);
            m_lstOriginalTile[2].Add(SEVN);
            m_lstOriginalTile[2].Add(HEMA);
            m_lstOriginalTile[2].Add(BAR3);
            m_lstOriginalTile[2].Add(FISH);
            m_lstOriginalTile[2].Add(BOGA);
            m_lstOriginalTile[2].Add(JOKE);


            m_lstOriginalTile[3].Add(OCTO);
            m_lstOriginalTile[3].Add(TAGT);
            m_lstOriginalTile[3].Add(BOGA);
            m_lstOriginalTile[3].Add(HEMA);
            m_lstOriginalTile[3].Add(CLAM);
            m_lstOriginalTile[3].Add(STAR);
            m_lstOriginalTile[3].Add(BOGA);
            m_lstOriginalTile[3].Add(MENG);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(SEVN);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(HEMA);
            m_lstOriginalTile[3].Add(CLAM);
            m_lstOriginalTile[3].Add(BAR3);
            m_lstOriginalTile[3].Add(OCTO);
            m_lstOriginalTile[3].Add(MENG);
            m_lstOriginalTile[3].Add(FISH);
            m_lstOriginalTile[3].Add(JOKE);
            #endregion
        }

    }
}
