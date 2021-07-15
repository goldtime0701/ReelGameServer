using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelServer
{

    public class CSWKJackpotInfo
    {
        public int m_nJackCont;         //잭팟내용 1-손오공, 2-저팔계, 3-사오정, 4-삼장, 5-삼칠별타, 6-프리게임
        public int m_nJackTy;           //잭팟형식 1-가짜예시, 2-정상예시, 3-돌발, 3-4불상, 5-삼칠별타, 6-프리게임
        public int m_nJackCash;         //잭팟캐시       
        public int m_nActScore;         //0-예시액션내 점수주지 않기, 1-예시액션내 점수주기(액션돌기할때 일반점수를 주겠는가 말겠는가를 결정)
        public int m_nEvilScore;        //0-불상액션내 점수주지 않기, 1-불상액션내 점수주기(불상출현 액션시 일반점수를 주겠는가 말겠는가를 결정)
    }

    public struct SWKJackType
    {
        public int m_nJackCont;         //잭팟내용 1-손오공, 2-저팔계, 3-사오정, 4-삼장, 5-삼칠별타, 6-프리게임
        public int m_nJackScore;        //잭팟점수

        public SWKJackType(int nJackCont, int nJackScore)
        {
            m_nJackCont = nJackCont;
            m_nJackScore = nJackScore;
        }
    }



    public struct SWKScoreTableInfo
    {
        public int nGetScore;
        public int nLineIndex;
        public int[] lstTileNote;
        public int nCount;
        public int nLimit;

        public SWKScoreTableInfo(int nGetScore, int nLineIndex, int[] lstTileNote, int nCount, int nLimit)
        {
            this.nGetScore = nGetScore;
            this.nLineIndex = nLineIndex;
            this.lstTileNote = lstTileNote;
            this.nCount = nCount;
            this.nLimit = nLimit;
        }
    };

    //점수정보 클라스
    public class CSwkScoreInfo
    {
        public int m_nScore;            //점수     ### 0-잭팟일때 빈돌기 일때 사용
        public int m_nMult;             //배수    
        public int m_nPrizeCmd;         //잭팟액션 실행명령코드
        public int m_nHintScore;        //스코종류 0-일반점수, 0-이상 예시점수 표시(알라딘에서는 당첨된 점수의 형식을 나타낸다.)
        public List<CTile> m_lstTile;   //타일배렬(당첨타일이 있는 부분만 생성)
        public int m_nGearCode;         //알라딘효과를 줄때만 사용

        public CSwkScoreInfo(int nScore = 0, int nMult = 1, int nPrizeCmd = 0, int nHintScore = 0)
        {
            m_nScore = nScore;
            m_nMult = nMult;
            m_nPrizeCmd = nPrizeCmd;
            m_nHintScore = nHintScore;
            m_lstTile = new List<CTile>();
        }
    }

    partial class CSwkGear
    {
        public const int JOKER = 0;
        public const int SONWU = 1;
        public const int THRBAR = 2;
        public const int TWOBAR = 3;
        public const int ONEBAR = 4;
        public const int SEVEN = 5;
        public const int STAR = 6;
        public const int TARGET = 7;
        public const int BELL = 8;
        public const int LEMON = 9;
        public const int PICH = 10;
        public const int CHERRY = 11;
        public const int EVIL = 12;

        public const int PRIZE_START = 0x01;
        public const int PRIZE_END = 0x02;
        public const int PRIZE_EVIL_SHOW = 0x03;
        public const int PRIZE_EVIL_LAUGHT = 0x04;
        public const int PRIZE_EVIL_FLY = 0x05;
        public const int PRIZE_EVIL_SLIDE = 0x06;
        public const int PRIZE_EVIL_HIDE = 0x07;
        public const int PRIZE_SWK_ANIMATION = 0x08;
        public const int PRIZE_SWK_ANIMATION_END = 0x09;
        public const int PRIZE_JPG_ANIMATION = 0x0A;
        public const int PRIZE_JPG_ANIMATION_END = 0x0B;
        public const int PRIZE_SOJ_ANIMATION = 0x0C;
        public const int PRIZE_SOJ_ANIMATION_END = 0x0D;
        public const int PRIZE_SOJ_ALLBAR = 0x0E;
        public const int PRIZE_SMJ_ANIMATION = 0x0F;
        public const int PRIZE_SMJ_ANIMATION_END = 0x10;
        public const int PRIZE_TSS_ANIMATION = 0x11;
        public const int PRIZE_FREE_START = 0x12;
        public const int PRIZE_FREE_END = 0x13;

        public const int SWK_PRIZE_SONWUKONG = 0x01;
        public const int SWK_PRIZE_JOPALGYE = 0x02;
        public const int SWK_PRIZE_SAOJONG = 0x03;
        public const int SWK_PRIZE_SAMJANG = 0x04;
        public const int SWK_PRIZE_THREESEVEN = 0x05;
        public const int SWK_PRIZE_FREEEVENT = 0x06;



        public const int DRAGON_YELLOW = 0;
        public const int DRAGON_GRAY = 1;
        public const int DRAGON_GREEN = 2;
        public const int DRAGON_BLUE = 3;
        public const int DRAGON_RED = 4;


        private int[] m_lstSWKScores = { 100, 250, 300, 400, 500, 600, 1000, 2000, 2500, 5000, 10000, 20000 };  //체리 2개가 200점이므로 200점은 없앤다.
        private int[] m_lstSWKMults = { 1, 2, 3, 5, 10 };
        private int[] m_lstSWKTiles = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };


        int[][] m_lstSpecScore = new int[12][] {
            new int[] { 5, 5, 5, 5, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 5, 5, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 5, 5, 5, 5 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 5, 5, 5, 5 },
            new int[] { 6, 6, 6, 6, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 6, 6, 6, 6, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 6, 6, 6, 6 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 6, 6, 6, 6 },
            new int[] { 7, 7, 7, 7, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 7, 7, 7, 7, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 7, 7, 7, 7 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 7, 7, 7, 7 }
        };

        int[][] m_lstSpecScore1 = new int[12][] {
            new int[] { 101, 5, 5, 5, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 101, 5, 5, 5, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 101, 5, 5, 5 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 101, 5, 5, 5 },
            new int[] { 101, 6, 6, 6, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 101, 6, 6, 6, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 101, 6, 6, 6 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 101, 6, 6, 6 },
            new int[] { 101, 7, 7, 7, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 101, 7, 7, 7, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 101, 7, 7, 7 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 101, 7, 7, 7 }
        };

        int[][] m_lstSpecScore2 = new int[12][] {
            new int[] { 5, 5, 5, 101, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 5, 101, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 5, 5, 5, 101 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 5, 5, 5, 101 },
            new int[] { 6, 6, 6, 101, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 6, 6, 6, 101, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 6, 6, 6, 101 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 6, 6, 6, 101 },
            new int[] { 7, 7, 7, 101, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 7, 7, 7, 101, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 7, 7, 7, 101 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 7, 7, 7, 101 }
        };



        int[][] m_lstPrize10000Score = new int[18][] {
            new int[] { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 },

            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 0 },

            new int[] { 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0 },

            new int[] { 3, 3, 3, 0, 0, 0, 0, 0, 3, 3, 3, 0 },
            new int[] { 3, 3, 3, 0, 0, 0, 0, 0, 0, 3, 3, 3 },
            new int[] { 0, 3, 3, 3, 0, 0, 0, 0, 0, 3, 3, 3 },
            new int[] { 0, 3, 3, 3, 0, 0, 0, 0, 3, 3, 3, 0 },

            new int[] { 3, 3, 3, 0, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 3, 3, 3, 0, 0, 4, 4, 4, 0, 0, 0, 0 },
            new int[] { 0, 3, 3, 3, 4, 4, 4, 0, 0, 0, 0, 0 },
            new int[] { 0, 3, 3, 3, 0, 4, 4, 4, 0, 0, 0, 0 },

            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 3, 3, 3, 0 },
            new int[] { 0, 0, 0, 0, 4, 4, 4, 0, 0, 3, 3, 3 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 3, 3, 3, 0 },
            new int[] { 0, 0, 0, 0, 0, 4, 4, 4, 0, 3, 3, 3 }
        };


        SWKScoreTableInfo[] m_lstSWKNormalScoreTable;
        SWKScoreTableInfo[] m_lstSWKPrizeScoreTable;
        SWKScoreTableInfo[] m_lstSWKJockerScoreTable;
        SWKScoreTableInfo[] m_lstSWKThreeSevenTable;
        SWKScoreTableInfo[] m_lstSWKBarScoreTable;
        List<SWKJackType> m_lstSWKJackType;

        
        public override void InitScoreTableInfo()
        {
            m_lstSWKJockerScoreTable = new SWKScoreTableInfo[]
            {
                new SWKScoreTableInfo( 20000, 0, new int[]{1, 1, 1, 1}, 4, 0),      //손오공 x 4  상
                new SWKScoreTableInfo( 20000, 1, new int[]{1, 1, 1, 1}, 4, 0),      //손오공 x 4  중
                new SWKScoreTableInfo( 20000, 2, new int[]{1, 1, 1, 1}, 4, 0),      //손오공 x 4  하
            };


            m_lstSWKNormalScoreTable = new SWKScoreTableInfo[]
            {
                new SWKScoreTableInfo( 20000, 0, new int[]{2, 2, 2, 2}, 4, 0),      //01 삼바   x 4  상               
                new SWKScoreTableInfo( 20000, 1, new int[]{2, 2, 2, 2}, 4, 0),      //01 삼바   x 4  중               
                new SWKScoreTableInfo( 20000, 2, new int[]{2, 2, 2, 2}, 4, 0),      //02 삼바   x 4  하               
                new SWKScoreTableInfo( 20000, 0, new int[]{2, 2, 2, 101}, 3, 1),    //03 삼바   x 3, 조커  상          
                new SWKScoreTableInfo( 20000, 1, new int[]{2, 2, 2, 101}, 3, 1),    //04 삼바   x 3, 조커  중
                new SWKScoreTableInfo( 20000, 2, new int[]{2, 2, 2, 101}, 3, 1),    //05 삼바   x 3, 조커  하
                new SWKScoreTableInfo( 20000, 0, new int[]{3, 3, 3, 3}, 4, 0),      //06 투바   x 4  상
                new SWKScoreTableInfo( 20000, 1, new int[]{3, 3, 3, 3}, 4, 0),      //07 투바   x 4  중
                new SWKScoreTableInfo( 20000, 2, new int[]{3, 3, 3, 3}, 4, 0),      //08 투바   x 4  하
                new SWKScoreTableInfo( 20000, 0, new int[]{3, 3, 3, 101}, 3, 1),    //09 투바   x 3, 조커  상
                new SWKScoreTableInfo( 20000, 1, new int[]{3, 3, 3, 101}, 3, 1),    //10 투바   x 3, 조커  중
                new SWKScoreTableInfo( 20000, 2, new int[]{3, 3, 3, 101}, 3, 1),    //11 투바   x 3, 조커  상하
                new SWKScoreTableInfo( 20000, 0, new int[]{4, 4, 4, 4}, 4, 0),      //12 원바   x 4  상
                new SWKScoreTableInfo( 20000, 1, new int[]{4, 4, 4, 4}, 4, 0),      //13 원바   x 4  중
                new SWKScoreTableInfo( 20000, 2, new int[]{4, 4, 4, 4}, 4, 0),      //14 원바   x 4  하
                new SWKScoreTableInfo( 20000, 0, new int[]{4, 4, 4, 101}, 3, 1),    //15 원바   x 3, 조커  상
                new SWKScoreTableInfo( 20000, 1, new int[]{4, 4, 4, 101}, 3, 1),    //16 원바   x 3, 조커  중
                new SWKScoreTableInfo( 20000, 2, new int[]{4, 4, 4, 101}, 3, 1),    //17 원바   x 3, 조커  하
                new SWKScoreTableInfo( 20000, 1, new int[]{5, 5, 5, 5}, 4, 0),      //18 세븐   X 4  중
                new SWKScoreTableInfo( 20000, 1, new int[]{5, 5, 5, 101}, 3, 1),    //19 세븐   X 3, 조커  중  
                new SWKScoreTableInfo( 20000, 1, new int[]{6, 6, 6, 6}, 4, 0),      //20 스타   X 4  중
                new SWKScoreTableInfo( 20000, 1, new int[]{6, 6, 6, 101}, 3, 1),    //21 스타   X 3, 조커  중
                new SWKScoreTableInfo( 20000, 1, new int[]{7, 7, 7, 7}, 4, 0),      //22 타겟   x 4  중
                new SWKScoreTableInfo( 20000, 1, new int[]{2, 2, 2, 0}, 3, 1),      //23 삼바   X 3  중

                new SWKScoreTableInfo( 15000, 0, new int[]{5, 5, 5, 5}, 4, 0),      //24 세븐   x 4  상
                new SWKScoreTableInfo( 15000, 2, new int[]{5, 5, 5, 5}, 4, 0),      //25 세븐   X 4  하
                new SWKScoreTableInfo( 15000, 0, new int[]{5, 5, 5, 101}, 3, 1),    //26 세븐   x 3, 조커  상
                new SWKScoreTableInfo( 15000, 2, new int[]{5, 5, 5, 101}, 3, 1),    //27 세븐   x 3, 조커  하
                new SWKScoreTableInfo( 15000, 0, new int[]{6, 6, 6, 6}, 4, 0),      //28 스타   x 4  상
                new SWKScoreTableInfo( 15000, 2, new int[]{6, 6, 6, 6}, 4, 0),      //29 스타   x 4  하
                new SWKScoreTableInfo( 15000, 0, new int[]{6, 6, 6, 101}, 3, 1),    //30 스타   x 4, 조커  상
                new SWKScoreTableInfo( 15000, 2, new int[]{6, 6, 6, 101}, 3, 1),    //31 스타   x 4, 조커  하
                new SWKScoreTableInfo( 15000, 0, new int[]{7, 7, 7, 7}, 4, 0),      //32 타겟   x 4  상
                new SWKScoreTableInfo( 15000, 2, new int[]{7, 7, 7, 7}, 4, 0),      //33 타겟   x 4  하
                new SWKScoreTableInfo( 10000, 0, new int[]{2, 2, 2, 0}, 3, 1),      //34 삼바   x 3  상
                new SWKScoreTableInfo( 10000, 2, new int[]{2, 2, 2, 0}, 3, 1),      //35 삼바   x 3  하
                new SWKScoreTableInfo( 10000, 1, new int[]{3, 3, 3, 0}, 3, 1),      //36 투바   x 3  중

                

                new SWKScoreTableInfo( 5000, 0, new int[]{3, 3, 3, 0}, 3, 1),       //37 투바   x 3  상
                new SWKScoreTableInfo( 5000, 2, new int[]{3, 3, 3, 0}, 3, 1),       //38 투바   x 3  하
                new SWKScoreTableInfo( 5000, 1, new int[]{4, 4, 4, 0}, 3, 1),       //39 원바   X 3  중

                new SWKScoreTableInfo( 2500, 0, new int[]{4, 4, 4, 0}, 3, 1),       //40 원바   x 3  상
                new SWKScoreTableInfo( 2500, 2, new int[]{4, 4, 4, 0}, 3, 1),       //41 원바   x 3  하

                new SWKScoreTableInfo( 2000, 0, new int[]{8, 8, 8, 8}, 4, 0),       //42 황벨   x 4  상
                new SWKScoreTableInfo( 2000, 1, new int[]{8, 8, 8, 8}, 4, 0),       //43 황벨   x 4  중
                new SWKScoreTableInfo( 2000, 2, new int[]{8, 8, 8, 8}, 4, 0),       //44 황벨   x 4  하

                new SWKScoreTableInfo( 1000, 0, new int[]{9, 9, 9, 9}, 4, 0),       //45 수박   x 4  상
                new SWKScoreTableInfo( 1000, 1, new int[]{9, 9, 9, 9}, 4, 0),       //46 수박   x 4  중
                new SWKScoreTableInfo( 1000, 2, new int[]{9, 9, 9, 9}, 4, 0),       //47 수박   x 4  하
                new SWKScoreTableInfo( 1000, 0, new int[]{10, 10, 10, 10}, 4, 0),   //48 피치   x 4  상
                new SWKScoreTableInfo( 1000, 1, new int[]{10, 10, 10, 10}, 4, 0),   //49 피치   x 4  중
                new SWKScoreTableInfo( 1000, 2, new int[]{10, 10, 10, 10}, 4, 0),   //50 피치   x 4  하
                new SWKScoreTableInfo( 1000, 0, new int[]{11, 11, 11, 11}, 4, 0),   //51 체리   x 4  상
                new SWKScoreTableInfo( 1000, 1, new int[]{11, 11, 11, 11}, 4, 0),   //52 체리   x 4  중
                new SWKScoreTableInfo( 1000, 2, new int[]{11, 11, 11, 11}, 4, 0),   //53 체리   x 4  하

                new SWKScoreTableInfo( 600, 1, new int[]{5, 5, 5, 0}, 3, 1),        //54 세븐   x 3  중
                new SWKScoreTableInfo( 600, 1, new int[]{6, 6, 6, 0}, 3, 1),        //55 스타   x 3  중
                new SWKScoreTableInfo( 600, 1, new int[]{7, 7, 7, 0}, 3, 1),        //56 타겟   x 3  중

                new SWKScoreTableInfo( 400, 0, new int[]{8, 8, 8, 0}, 3, 1),        //57 황벨   x 3  상
                new SWKScoreTableInfo( 400, 1, new int[]{8, 8, 8, 0}, 3, 1),        //58 황벨   x 3  중
                new SWKScoreTableInfo( 400, 2, new int[]{8, 8, 8, 0}, 3, 1),        //59 황벨   x 3  하

                new SWKScoreTableInfo( 300, 0, new int[]{5, 5, 5, 0}, 3, 1),        //60 세븐   x 3  상
                new SWKScoreTableInfo( 300, 2, new int[]{5, 5, 5, 0}, 3, 1),        //61 세븐   x 3  하
                new SWKScoreTableInfo( 300, 0, new int[]{6, 6, 6, 0}, 3, 1),        //62 스타   x 3  상
                new SWKScoreTableInfo( 300, 2, new int[]{6, 6, 6, 0}, 3, 1),        //63 스타   x 3  하
                new SWKScoreTableInfo( 300, 0, new int[]{7, 7, 7, 0}, 3, 1),        //64 타겟   x 3  상
                new SWKScoreTableInfo( 300, 2, new int[]{7, 7, 7, 0}, 3, 1),        //65 타겟   x 3  하
                new SWKScoreTableInfo( 300, 0, new int[]{9, 9, 9, 0}, 3, 1),        //66 수박   x 3  상
                new SWKScoreTableInfo( 300, 1, new int[]{9, 9, 9, 0}, 3, 1),        //67 수박   x 3  중
                new SWKScoreTableInfo( 300, 2, new int[]{9, 9, 9, 0}, 3, 1),        //68 수박   x 3  하
                new SWKScoreTableInfo( 300, 0, new int[]{10, 10, 10, 0}, 3, 1),     //69 피치   x 3  상
                new SWKScoreTableInfo( 300, 1, new int[]{10, 10, 10, 0}, 3, 1),     //70 피치   x 3  중
                new SWKScoreTableInfo( 300, 2, new int[]{10, 10, 10, 0}, 3, 1),     //71 피치   x 3  하
                new SWKScoreTableInfo( 300, 0, new int[]{11, 11, 11, 0}, 3, 1),     //72 체리   x 3  상
                new SWKScoreTableInfo( 300, 1, new int[]{11, 11, 11, 0}, 3, 1),     //73 체리   x 3  중
                new SWKScoreTableInfo( 300, 2, new int[]{11, 11, 11, 0}, 3, 1),     //74 체리   x 3  하

                new SWKScoreTableInfo( 200, 0, new int[]{11, 11, 0, 0}, 2, 0),      //75 체리   x 2  상
                new SWKScoreTableInfo( 200, 1, new int[]{11, 11, 0, 0}, 2, 0),      //76 체리   x 2  중
                new SWKScoreTableInfo( 200, 2, new int[]{11, 11, 0, 0}, 2, 0),      //77 체리   x 2  하

                new SWKScoreTableInfo( 200, 0, new int[]{11, 11, 0, 0}, 2, -1),     //75 체리   x 2  상  //오른쪽에 붙은때는 마지막 파라메터를 -1로설정
                new SWKScoreTableInfo( 200, 1, new int[]{11, 11, 0, 0}, 2, -1),     //76 체리   x 2  중
                new SWKScoreTableInfo( 200, 2, new int[]{11, 11, 0, 0}, 2, -1),     //77 체리   x 2  하

                new SWKScoreTableInfo( 100, 0, new int[]{5, 5, 0, 0}, 2, 0),        //78 세븐   x 2  상
                new SWKScoreTableInfo( 100, 1, new int[]{5, 5, 0, 0}, 2, 0),        //79 세븐   x 2  중
                new SWKScoreTableInfo( 100, 2, new int[]{5, 5, 0, 0}, 2, 0),        //80 세븐   x 2  하
                new SWKScoreTableInfo( 100, 0, new int[]{6, 6, 0, 0}, 2, 0),        //81 스타   x 2  상
                new SWKScoreTableInfo( 100, 1, new int[]{6, 6, 0, 0}, 2, 0),        //82 스타   x 2  중
                new SWKScoreTableInfo( 100, 2, new int[]{6, 6, 0, 0}, 2, 0),        //83 스타   x 2  하
                new SWKScoreTableInfo( 100, 0, new int[]{7, 7, 0, 0}, 2, 0),        //84 타겟   x 2  상
                new SWKScoreTableInfo( 100, 1, new int[]{7, 7, 0, 0}, 2, 0),        //85 타겟   x 2  중
                new SWKScoreTableInfo( 100, 2, new int[]{7, 7, 0, 0}, 2, 0),        //86 타겟   x 2  하
                new SWKScoreTableInfo( 100, 0, new int[]{11, 0, 102, 0}, 1, 0),       //87 체리   x 1  상
                new SWKScoreTableInfo( 100, 1, new int[]{11, 0, 102, 0}, 1, 0),       //88 체리   x 1  중
                new SWKScoreTableInfo( 100, 2, new int[]{11, 0, 102, 0}, 1, 0),       //89 체리   x 1  하

                new SWKScoreTableInfo( 100, 0, new int[]{11, 0, 102, 0}, 1,-1),       //87 체리   x 1  상   //오른쪽에 붙은때는 마지막 파라메터를 -1로설정
                new SWKScoreTableInfo( 100, 1, new int[]{11, 0, 102, 0}, 1,-1),       //88 체리   x 1  중
                new SWKScoreTableInfo( 100, 2, new int[]{11, 0, 102, 0}, 1,-1),       //89 체리   x 1  하



                new SWKScoreTableInfo( 250, 0, new int[]{ 103, 103, 103, 102 }, 4, 0),       //잡바점수표(102점은 조커와 빠를 빼놓은 타일을 나타낸다. 즉 과일 타일을 나타낸다.)
                new SWKScoreTableInfo( 250, 0, new int[]{ 102, 103, 103, 103 }, 4, 0),       //잡바점수표(102점은 조커와 빠를 빼놓은 타일을 나타낸다. 즉 과일 타일을 나타낸다.)
                new SWKScoreTableInfo( 500, 1, new int[]{ 103, 103, 103, 102 }, 4, 0),       //잡바점수표(102점은 조커와 빠를 빼놓은 타일을 나타낸다. 즉 과일 타일을 나타낸다.)
                new SWKScoreTableInfo( 500, 1, new int[]{ 102, 103, 103, 103 }, 4, 0),       //잡바점수표(102점은 조커와 빠를 빼놓은 타일을 나타낸다. 즉 과일 타일을 나타낸다.)
                new SWKScoreTableInfo( 250, 2, new int[]{ 103, 103, 103, 102 }, 4, 0),       //잡바점수표(102점은 조커와 빠를 빼놓은 타일을 나타낸다. 즉 과일 타일을 나타낸다.)
                new SWKScoreTableInfo( 250, 2, new int[]{ 102, 103, 103, 103 }, 4, 0),       //잡바점수표(102점은 조커와 빠를 빼놓은 타일을 나타낸다. 즉 과일 타일을 나타낸다.)

                new SWKScoreTableInfo( 500,  0, new int[]{ 103, 103, 103, 103 }, 4, 0),
                new SWKScoreTableInfo( 1000, 1, new int[]{ 103, 103, 103, 103 }, 4, 0),
                new SWKScoreTableInfo( 500,  2, new int[]{ 103, 103, 103, 103 }, 4, 0),
            };


            m_lstSWKPrizeScoreTable = new SWKScoreTableInfo[]
            {
                new SWKScoreTableInfo( 50000, 1, new int[]{2, 2, 2, 2}, 4, 0),      //01 삼바 X 4   중
                new SWKScoreTableInfo( 40000, 1, new int[]{3, 3, 3, 3}, 4, 0),      //02 투바 x 4   중
                new SWKScoreTableInfo( 30000, 1, new int[]{4, 4, 4, 4}, 4, 0),      //03 원바 x 4   중
                new SWKScoreTableInfo( 25000, 0, new int[]{2, 2, 2, 2}, 4, 0),      //04 삼바 X 4   상
                new SWKScoreTableInfo( 25000, 2, new int[]{2, 2, 2, 2}, 4, 0),      //05 삼바 X 4   하
                new SWKScoreTableInfo( 20000, 0, new int[]{3, 3, 3, 3}, 4, 0),      //06 투바 x 4   상
                new SWKScoreTableInfo( 20000, 2, new int[]{3, 3, 3, 3}, 4, 0),      //07 투바 x 4   하
                new SWKScoreTableInfo( 15000, 0, new int[]{4, 4, 4, 4}, 4, 0),      //08 원바 x 4   상
                new SWKScoreTableInfo( 15000, 2, new int[]{4, 4, 4, 4}, 4, 0),      //09 원바 x 4   하

            };

            m_lstSWKBarScoreTable = new SWKScoreTableInfo[]
            {
                new SWKScoreTableInfo( 50000, 1, new int[]{2, 2, 2, 2}, 4, 0),      //01 삼바 X 4   중
                new SWKScoreTableInfo( 40000, 1, new int[]{3, 3, 3, 3}, 4, 0),      //02 투바 x 4   중
                new SWKScoreTableInfo( 30000, 1, new int[]{4, 4, 4, 4}, 4, 0),      //03 원바 x 4   중
                new SWKScoreTableInfo( 25000, 0, new int[]{2, 2, 2, 2}, 4, 0),      //04 삼바 X 4   상
                new SWKScoreTableInfo( 25000, 2, new int[]{2, 2, 2, 2}, 4, 0),      //05 삼바 X 4   하
                new SWKScoreTableInfo( 20000, 0, new int[]{3, 3, 3, 3}, 4, 0),      //06 투바 x 4   상
                new SWKScoreTableInfo( 20000, 2, new int[]{3, 3, 3, 3}, 4, 0),      //07 투바 x 4   하
                new SWKScoreTableInfo( 15000, 0, new int[]{4, 4, 4, 4}, 4, 0),      //08 원바 x 4   상
                new SWKScoreTableInfo( 15000, 2, new int[]{4, 4, 4, 4}, 4, 0),      //09 원바 x 4   하
                new SWKScoreTableInfo( 20000, 1, new int[]{2, 2, 2, 0}, 3, 1),      //23 삼바 X 3   중
                new SWKScoreTableInfo( 10000, 0, new int[]{2, 2, 2, 0}, 3, 1),      //34 삼바 x 3   상
                new SWKScoreTableInfo( 10000, 2, new int[]{2, 2, 2, 0}, 3, 1),      //35 삼바 x 3   하
                new SWKScoreTableInfo( 10000, 1, new int[]{3, 3, 3, 0}, 3, 1),      //36 투바 x 3   중
                new SWKScoreTableInfo( 5000,  0, new int[]{3, 3, 3, 0}, 3, 1),      //37 투바 x 3   상
                new SWKScoreTableInfo( 5000,  2, new int[]{3, 3, 3, 0}, 3, 1),      //38 투바 x 3   하
                new SWKScoreTableInfo( 5000,  1, new int[]{4, 4, 4, 0}, 3, 1),      //39 원바 X 3   중
                new SWKScoreTableInfo( 2500,  0, new int[]{4, 4, 4, 0}, 3, 1),      //40 원바 x 3   상
                new SWKScoreTableInfo( 2500,  2, new int[]{4, 4, 4, 0}, 3, 1),      //41 원바 x 3   하

            };

            m_lstSWKThreeSevenTable = new SWKScoreTableInfo[]
            {
                new SWKScoreTableInfo( 0, 1, new int[]{2, 5, 6, 7}, 4, 0),          //10 삼칠별타   중
                new SWKScoreTableInfo( 0, 0, new int[]{2, 5, 6, 7}, 4, 0),          //10 삼칠별타   상
                new SWKScoreTableInfo( 0, 2, new int[]{2, 5, 6, 7}, 4, 0),          //10 삼칠별타   하
            };

            int[] arrScore = { 50000+20000, 40000+25000, 40000+20000, 40000+15000,
                               50000, 30000+15000, 40000, 20000+20000, 25000+15000, 20000+15000, 30000, 15000+15000, 25000, 20000, 15000 };

            m_lstSWKJackType = new List<SWKJackType>();
            //손오공잭팟
            m_lstSWKJackType.Add(new SWKJackType(1, 200000));
            m_lstSWKJackType.Add(new SWKJackType(1, 300000));
            m_lstSWKJackType.Add(new SWKJackType(1, 400000));
            m_lstSWKJackType.Add(new SWKJackType(1, 500000));
            m_lstSWKJackType.Add(new SWKJackType(1, 600000));
            m_lstSWKJackType.Add(new SWKJackType(1, 1000000));
            m_lstSWKJackType.Add(new SWKJackType(1, 1500000));
            m_lstSWKJackType.Add(new SWKJackType(1, 2000000));
            //사오정잭팟
            m_lstSWKJackType.Add(new SWKJackType(3, 30000));
            m_lstSWKJackType.Add(new SWKJackType(3, 40000));
            m_lstSWKJackType.Add(new SWKJackType(3, 50000));
            m_lstSWKJackType.Add(new SWKJackType(3, 60000));
            m_lstSWKJackType.Add(new SWKJackType(3, 70000));
            m_lstSWKJackType.Add(new SWKJackType(3, 80000));
            m_lstSWKJackType.Add(new SWKJackType(3, 90000));
            m_lstSWKJackType.Add(new SWKJackType(3, 100000));
            m_lstSWKJackType.Add(new SWKJackType(3, 110000));
            m_lstSWKJackType.Add(new SWKJackType(3, 120000));
            m_lstSWKJackType.Add(new SWKJackType(3, 130000));
            m_lstSWKJackType.Add(new SWKJackType(3, 140000));
            m_lstSWKJackType.Add(new SWKJackType(3, 150000));
            m_lstSWKJackType.Add(new SWKJackType(3, 200000));
            m_lstSWKJackType.Add(new SWKJackType(3, 250000));
            m_lstSWKJackType.Add(new SWKJackType(3, 300000));
            m_lstSWKJackType.Add(new SWKJackType(3, 350000));
            m_lstSWKJackType.Add(new SWKJackType(3, 400000));
            m_lstSWKJackType.Add(new SWKJackType(3, 450000));
            m_lstSWKJackType.Add(new SWKJackType(3, 500000));
            m_lstSWKJackType.Add(new SWKJackType(3, 600000));
            m_lstSWKJackType.Add(new SWKJackType(3, 650000));
            m_lstSWKJackType.Add(new SWKJackType(3, 700000));
            m_lstSWKJackType.Add(new SWKJackType(3, 750000));
            //삼칠별타잭팟
            m_lstSWKJackType.Add(new SWKJackType(5, 50000));
            m_lstSWKJackType.Add(new SWKJackType(5, 100000));
            m_lstSWKJackType.Add(new SWKJackType(5, 150000));
            m_lstSWKJackType.Add(new SWKJackType(5, 200000));
            m_lstSWKJackType.Add(new SWKJackType(5, 250000));
            m_lstSWKJackType.Add(new SWKJackType(5, 300000));
            m_lstSWKJackType.Add(new SWKJackType(5, 350000));
            m_lstSWKJackType.Add(new SWKJackType(5, 400000));
            m_lstSWKJackType.Add(new SWKJackType(5, 450000));
            m_lstSWKJackType.Add(new SWKJackType(5, 450000));
            m_lstSWKJackType.Add(new SWKJackType(5, 500000));
            m_lstSWKJackType.Add(new SWKJackType(5, 550000));
            m_lstSWKJackType.Add(new SWKJackType(5, 600000));
            m_lstSWKJackType.Add(new SWKJackType(5, 650000));
            m_lstSWKJackType.Add(new SWKJackType(5, 700000));
            m_lstSWKJackType.Add(new SWKJackType(5, 750000));
            m_lstSWKJackType.Add(new SWKJackType(5, 800000));
            m_lstSWKJackType.Add(new SWKJackType(5, 850000));
            m_lstSWKJackType.Add(new SWKJackType(5, 900000));
            m_lstSWKJackType.Add(new SWKJackType(5, 950000));
            m_lstSWKJackType.Add(new SWKJackType(5, 1000000));
            //프리잭팟
            //m_lstSWKJackType.Add(new SWKJackType(6, 100000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 200000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 300000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 400000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 500000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 600000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 700000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 800000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 900000));
            //m_lstSWKJackType.Add(new SWKJackType(6, 1000000));
            //저팔계잭팟
            for (int i = 3; i <= 75; i++)
            {
                m_lstSWKJackType.Add(new SWKJackType(2, i * 10000));
            }
            //삼장법사잭팟
            for (int i = 3; i <= 75; i++)
            {
                m_lstSWKJackType.Add(new SWKJackType(4, i * 10000));
            }
        }
    }
}
