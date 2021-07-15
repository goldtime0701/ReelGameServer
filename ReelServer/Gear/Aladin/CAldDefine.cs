using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    //점수정보 클라스
    public class CALDScoreInfo
    {
        public int m_nScore;            //점수     ### 0-잭팟일때 빈돌기 일때 사용
        public int m_nMult;             //배수    
        public int m_nPrizeCmd;         //잭팟액션 실행명령코드
        public int m_nHintScore;        //스코종류 0-일반점수, 0-이상 예시점수 표시(알라딘에서는 당첨된 점수의 형식을 나타낸다.)
        public List<CTile> m_lstTile;   //타일배렬(당첨타일이 있는 부분만 생성)
        public int m_nGearCode;         //알라딘효과를 줄때만 사용

        public CALDScoreInfo(int nScore = 0, int nMult = 1, int nPrizeCmd = 0, int nHintScore = 0)
        {
            m_nScore = nScore;
            m_nMult = nMult;
            m_nPrizeCmd = nPrizeCmd;
            m_nHintScore = nHintScore;
            m_lstTile = new List<CTile>();
        }
    }


    //점수정보클라스
    public class ALDScoreTableInfo
    {
        public int nGetScore;
        public int nLineIndex;
        public int[] lstTileNote;
        public int nCount;
        public int nSlogan;
        public int nMulti;

        public ALDScoreTableInfo(int nGetScore, int nLineIndex, int[] lstTileNote, int nCount, int nSlogan, int nMulti)
        {
            this.nGetScore = nGetScore;
            this.nLineIndex = nLineIndex;
            this.lstTileNote = lstTileNote;
            this.nCount = nCount;
            this.nSlogan = nSlogan;
            this.nMulti = nMulti;
        }

    }

    public class CALDJackpotInfo
    {
        public int m_nJackCont;         //잭팟내용      1-백마, 2-작은불, 3-큰불, 4-양탄자-열림, 5-양탄자 닫김, 7-휘버
        public int m_nJackCash;         //잭팟캐시       
    }

    public class ALDComplexScoreTable
    {
        public ALDScoreTableInfo largeScore;
        public ALDScoreTableInfo smallScore;
        public ALDScoreTableInfo miniScore;
    }





    partial class CAldGear
    {
        private const int ALD_HORSE_WWW = 1;
        private const int ALD_FIRE_SMALL = 2;
        private const int ALD_FIRE_LARGE = 3;
        private const int ALD_YANGT_LOCK = 4;
        private const int ALD_YANGT_OPEN = 5;
        private const int ALD_FEVER_LOCK = 6;
        private const int ALD_FEVER_OPEN = 7;
        private const int ALD_THREE_SEVEN = 8;
        private const int ALD_SURPRISE_1 = 9;
        private const int ALD_SURPRISE_2 = 10;
        private const int ALD_SURPRISE_3 = 11;

        //잭팟명령
        public const int PRIZE_START = 0x71;
        public const int PRIZE_END = 0x72;
        public const int ALD_HORSE_ANIMATION = 0x100;
        public const int ALD_HORSE_WWW_ANIMATION = 0x101;
        public const int ALD_HORSE_WRW_ANUMATION = 0x103;
        public const int ALD_HORSE_WRR_ANIMATION = 0x105;
        public const int ALD_YANGT_ANIMATION = 0x107;
        public const int ALD_YANGT_LOCK_ANIMATION = 0x108;
        public const int ALD_YANGT_OPEN_ANIMATION = 0x109;
        public const int ALD_YANGT_RELEASE_SPIN = 0x110;
        public const int ALD_FIRE_SMALL_ANIMATION = 0x111;
        public const int ALD_FIRE_LARGE_ANIMATION = 0x113;
        public const int ALD_FEVER_LOCK_ANIMATION = 0x115;
        public const int ALD_FEVER_OPEN_ANIMATION = 0x117;
        public const int ALD_THREE_SEVEN_ANIMATION = 0x119;
        public const int ALD_REEL_STOP = 0x11A;



        private List<int> m_lstALDScores;
        private int[] m_lstALDMults = { 1, 2, 3, 5, 10 };


        ALDScoreTableInfo[] m_lstALDNormalScoreTable;
        ALDScoreTableInfo[] m_lstALDThreeJackerScoreTable;
        ALDScoreTableInfo[] m_lstALDTwoJackerScoreTable;
        ALDScoreTableInfo[] m_lstALDThreeSevenScoreTable;


        override
        public void InitScoreTableInfo()
        {
            m_lstALDThreeSevenScoreTable = new ALDScoreTableInfo[]
            {
                new ALDScoreTableInfo(-1, 0, new int[] { 2, 5, 6, 7 }, 4, 4, 1),    //삼칠별타
                new ALDScoreTableInfo(-1, 1, new int[] { 2, 5, 6, 7 }, 4, 4, 1),    //삼칠별타
                new ALDScoreTableInfo(-1, 2, new int[] { 2, 5, 6, 7 }, 4, 4, 1),    //삼칠별타

                new ALDScoreTableInfo(-1, 0, new int[] { 2, 5, 6, 7 }, 4, 4, 2),    //삼칠별타
                new ALDScoreTableInfo(-1, 1, new int[] { 2, 5, 6, 7 }, 4, 4, 2),    //삼칠별타
                new ALDScoreTableInfo(-1, 2, new int[] { 2, 5, 6, 7 }, 4, 4, 2),    //삼칠별타

                new ALDScoreTableInfo(-1, 0, new int[] { 2, 5, 6, 7 }, 4, 4, 3),    //삼칠별타
                new ALDScoreTableInfo(-1, 1, new int[] { 2, 5, 6, 7 }, 4, 4, 3),    //삼칠별타
                new ALDScoreTableInfo(-1, 2, new int[] { 2, 5, 6, 7 }, 4, 4, 3),    //삼칠별타

                new ALDScoreTableInfo(-1, 0, new int[] { 2, 5, 6, 7 }, 4, 4, 5),    //삼칠별타
                new ALDScoreTableInfo(-1, 1, new int[] { 2, 5, 6, 7 }, 4, 4, 5),    //삼칠별타
                new ALDScoreTableInfo(-1, 2, new int[] { 2, 5, 6, 7 }, 4, 4, 5),    //삼칠별타

                new ALDScoreTableInfo(-1, 0, new int[] { 2, 5, 6, 7 }, 4, 4, 10),    //삼칠별타
                new ALDScoreTableInfo(-1, 1, new int[] { 2, 5, 6, 7 }, 4, 4, 10),    //삼칠별타
                new ALDScoreTableInfo(-1, 2, new int[] { 2, 5, 6, 7 }, 4, 4, 10),    //삼칠별타
            };


            m_lstALDThreeJackerScoreTable = new ALDScoreTableInfo[]
            {
                new ALDScoreTableInfo( 5000 * 1, 0, new int[]{1, 1, 1, 104 }, 3, -3, 1),     //104 => 7, 별, 타
                new ALDScoreTableInfo( 5000 * 1, 1, new int[]{1, 1, 1, 104 }, 3, -3, 1),     //조커3개당첨일때는 슬로간점수를 2로 준다.
                new ALDScoreTableInfo( 5000 * 1, 2, new int[]{1, 1, 1, 104 }, 3, -3, 1),

                new ALDScoreTableInfo( 5000 * 2, 0, new int[]{1, 1, 1, 104 }, 3, -3, 2),
                new ALDScoreTableInfo( 5000 * 2, 1, new int[]{1, 1, 1, 104 }, 3, -3, 2),
                new ALDScoreTableInfo( 5000 * 2, 2, new int[]{1, 1, 1, 104 }, 3, -3, 2),

                new ALDScoreTableInfo( 5000 * 3, 0, new int[]{1, 1, 1, 104 }, 3, -3, 3),
                new ALDScoreTableInfo( 5000 * 3, 1, new int[]{1, 1, 1, 104 }, 3, -3, 3),
                new ALDScoreTableInfo( 5000 * 3, 2, new int[]{1, 1, 1, 104 }, 3, -3, 3),

                new ALDScoreTableInfo( 5000 * 5, 0, new int[]{1, 1, 1, 104 }, 3, -3, 5),
                new ALDScoreTableInfo( 5000 * 5, 1, new int[]{1, 1, 1, 104 }, 3, -3, 5),
                new ALDScoreTableInfo( 5000 * 5, 2, new int[]{1, 1, 1, 104 }, 3, -3, 5),

                new ALDScoreTableInfo( 5000 * 10, 0, new int[]{1, 1, 1, 104 }, 3, -3, 10),
                new ALDScoreTableInfo( 5000 * 10, 1, new int[]{1, 1, 1, 104 }, 3, -3, 10),
                new ALDScoreTableInfo( 5000 * 10, 2, new int[]{1, 1, 1, 104 }, 3, -3, 10),

                new ALDScoreTableInfo( 5000 * 1, 0, new int[]{ 104, 1, 1, 1}, 3, -3, 1),
                new ALDScoreTableInfo( 5000 * 1, 1, new int[]{ 104, 1, 1, 1}, 3, -3, 1),
                new ALDScoreTableInfo( 5000 * 1, 2, new int[]{ 104, 1, 1, 1}, 3, -3, 1),

                new ALDScoreTableInfo( 5000 * 2, 0, new int[]{ 104, 1, 1, 1}, 3, -3, 2),
                new ALDScoreTableInfo( 5000 * 2, 1, new int[]{ 104, 1, 1, 1}, 3, -3, 2),
                new ALDScoreTableInfo( 5000 * 2, 2, new int[]{ 104, 1, 1, 1}, 3, -3, 2),

                new ALDScoreTableInfo( 5000 * 3, 0, new int[]{ 104, 1, 1, 1}, 3, -3, 3),
                new ALDScoreTableInfo( 5000 * 3, 1, new int[]{ 104, 1, 1, 1}, 3, -3, 3),
                new ALDScoreTableInfo( 5000 * 3, 2, new int[]{ 104, 1, 1, 1}, 3, -3, 3),

                new ALDScoreTableInfo( 5000 * 5, 0, new int[]{ 104, 1, 1, 1}, 3, -3, 5),
                new ALDScoreTableInfo( 5000 * 5, 1, new int[]{ 104, 1, 1, 1}, 3, -3, 5),
                new ALDScoreTableInfo( 5000 * 5, 2, new int[]{ 104, 1, 1, 1}, 3, -3, 5),

                new ALDScoreTableInfo( 5000 * 10, 0, new int[]{ 104, 1, 1, 1}, 3, -3, 10),
                new ALDScoreTableInfo( 5000 * 10, 1, new int[]{ 104, 1, 1, 1}, 3, -3, 10),
                new ALDScoreTableInfo( 5000 * 10, 2, new int[]{ 104, 1, 1, 1}, 3, -3, 10)
            };


            m_lstALDTwoJackerScoreTable = new ALDScoreTableInfo[]
            {
                new ALDScoreTableInfo( -8, 0, new int[]{ 1, 1, 104, 0 }, 3, 0, 1),
                new ALDScoreTableInfo( -8, 1, new int[]{ 1, 1, 104, 0 }, 3, 0, 1),
                new ALDScoreTableInfo( -8, 2, new int[]{ 1, 1, 104, 0 }, 3, 0, 1),

                new ALDScoreTableInfo( -8, 0, new int[]{ 1, 1, 104, 0 }, 3, 0, 2),
                new ALDScoreTableInfo( -8, 1, new int[]{ 1, 1, 104, 0 }, 3, 0, 2),
                new ALDScoreTableInfo( -8, 2, new int[]{ 1, 1, 104, 0 }, 3, 0, 2),

                new ALDScoreTableInfo( -8, 0, new int[]{ 1, 1, 104, 0 }, 3, 0, 3),
                new ALDScoreTableInfo( -8, 1, new int[]{ 1, 1, 104, 0 }, 3, 0, 3),
                new ALDScoreTableInfo( -8, 2, new int[]{ 1, 1, 104, 0 }, 3, 0, 3),

                new ALDScoreTableInfo( -8, 0, new int[]{ 1, 1, 104, 0 }, 3, 0, 5),
                new ALDScoreTableInfo( -8, 1, new int[]{ 1, 1, 104, 0 }, 3, 0, 5),
                new ALDScoreTableInfo( -8, 2, new int[]{ 1, 1, 104, 0 }, 3, 0, 5),

                new ALDScoreTableInfo( -8, 0, new int[]{ 1, 1, 104, 0 }, 3, 0, 10),
                new ALDScoreTableInfo( -8, 1, new int[]{ 1, 1, 104, 0 }, 3, 0, 10),
                new ALDScoreTableInfo( -8, 2, new int[]{ 1, 1, 104, 0 }, 3, 0, 10),

                new ALDScoreTableInfo( -8, 0, new int[]{ 0, 104, 1, 1 }, 3, 0, 1),
                new ALDScoreTableInfo( -8, 1, new int[]{ 0, 104, 1, 1 }, 3, 0, 1),
                new ALDScoreTableInfo( -8, 2, new int[]{ 0, 104, 1, 1 }, 3, 0, 1),

                new ALDScoreTableInfo( -8, 0, new int[]{ 0, 104, 1, 1 }, 3, 0, 2),
                new ALDScoreTableInfo( -8, 1, new int[]{ 0, 104, 1, 1 }, 3, 0, 2),
                new ALDScoreTableInfo( -8, 2, new int[]{ 0, 104, 1, 1 }, 3, 0, 2),

                new ALDScoreTableInfo( -8, 0, new int[]{ 0, 104, 1, 1 }, 3, 0, 3),
                new ALDScoreTableInfo( -8, 1, new int[]{ 0, 104, 1, 1 }, 3, 0, 3),
                new ALDScoreTableInfo( -8, 2, new int[]{ 0, 104, 1, 1 }, 3, 0, 3),

                new ALDScoreTableInfo( -8, 0, new int[]{ 0, 104, 1, 1 }, 3, 0, 5),
                new ALDScoreTableInfo( -8, 1, new int[]{ 0, 104, 1, 1 }, 3, 0, 5),
                new ALDScoreTableInfo( -8, 2, new int[]{ 0, 104, 1, 1 }, 3, 0, 5),

                new ALDScoreTableInfo( -8, 0, new int[]{ 0, 104, 1, 1 }, 3, 0, 10),
                new ALDScoreTableInfo( -8, 1, new int[]{ 0, 104, 1, 1 }, 3, 0, 10),
                new ALDScoreTableInfo( -8, 2, new int[]{ 0, 104, 1, 1 }, 3, 0, 10),
            };

            m_lstALDNormalScoreTable = new ALDScoreTableInfo[]
            {
                new ALDScoreTableInfo( 500000 * 1, 0, new int[]{1, 1, 1, 1}, 4, 1, 1),         //01 조커 X 4  상
                new ALDScoreTableInfo( 500000 * 1, 1, new int[]{1, 1, 1, 1}, 4, 1, 1),         //02 조커 X 4  중
                new ALDScoreTableInfo( 500000 * 1, 2, new int[]{1, 1, 1, 1}, 4, 1, 1),         //03 조커 X 4  하
                new ALDScoreTableInfo( 500000 * 2, 0, new int[]{1, 1, 1, 1}, 4, 1, 2),         //01 조커 X 4  상
                new ALDScoreTableInfo( 500000 * 2, 1, new int[]{1, 1, 1, 1}, 4, 1, 2),         //02 조커 X 4  중
                new ALDScoreTableInfo( 500000 * 2, 2, new int[]{1, 1, 1, 1}, 4, 1, 2),         //03 조커 X 4  하
                new ALDScoreTableInfo( 500000 * 3, 0, new int[]{1, 1, 1, 1}, 4, 1, 2),         //01 조커 X 4  상
                new ALDScoreTableInfo( 500000 * 3, 1, new int[]{1, 1, 1, 1}, 4, 1, 2),         //02 조커 X 4  중
                new ALDScoreTableInfo( 500000 * 3, 2, new int[]{1, 1, 1, 1}, 4, 1, 2),         //03 조커 X 4  하
                new ALDScoreTableInfo( 500000 * 5, 0, new int[]{1, 1, 1, 1}, 4, 1, 2),         //01 조커 X 4  상
                new ALDScoreTableInfo( 500000 * 5, 1, new int[]{1, 1, 1, 1}, 4, 1, 2),         //02 조커 X 4  중
                new ALDScoreTableInfo( 500000 * 5, 2, new int[]{1, 1, 1, 1}, 4, 1, 2),         //03 조커 X 4  하
                new ALDScoreTableInfo( 500000 * 10, 0, new int[]{1, 1, 1, 1}, 4, 1, 2),         //01 조커 X 4  상
                new ALDScoreTableInfo( 500000 * 10, 1, new int[]{1, 1, 1, 1}, 4, 1, 2),         //02 조커 X 4  중
                new ALDScoreTableInfo( 500000 * 10, 2, new int[]{1, 1, 1, 1}, 4, 1, 2),         //03 조커 X 4  하



                new ALDScoreTableInfo( 25000 * 1, 0, new int[]{2, 2, 2, 2}, 4, 4, 1),          //04 삼바 x 4  상
                new ALDScoreTableInfo( 50000 * 1, 1, new int[]{2, 2, 2, 2}, 4, 4, 1),          //05 삼바 x 4  중
                new ALDScoreTableInfo( 25000 * 1, 2, new int[]{2, 2, 2, 2}, 4, 4, 1),          //06 삼바 x 4  하
                new ALDScoreTableInfo( 25000 * 1, 0, new int[]{2, 2, 2, 101}, 4, 4, 1),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 1, 1, new int[]{2, 2, 2, 101}, 4, 4, 1),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 1, 2, new int[]{2, 2, 2, 101}, 4, 4, 1),        //09 삼바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 25000 * 1, 0, new int[]{101, 2, 2, 2}, 4, 4, 1),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 1, 1, new int[]{101, 2, 2, 2}, 4, 4, 1),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 1, 2, new int[]{101, 2, 2, 2}, 4, 4, 1),        //09 삼바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 25000 * 2, 0, new int[]{2, 2, 2, 2}, 4, 4, 2),          //04 삼바 x 4  상
                new ALDScoreTableInfo( 50000 * 2, 1, new int[]{2, 2, 2, 2}, 4, 4, 2),          //05 삼바 x 4  중
                new ALDScoreTableInfo( 25000 * 2, 2, new int[]{2, 2, 2, 2}, 4, 4, 2),          //06 삼바 x 4  하
                new ALDScoreTableInfo( 25000 * 2, 0, new int[]{2, 2, 2, 101}, 4, 4, 2),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 2, 1, new int[]{2, 2, 2, 101}, 4, 4, 2),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 2, 2, new int[]{2, 2, 2, 101}, 4, 4, 2),        //09 삼바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 25000 * 2, 0, new int[]{101, 2, 2, 2}, 4, 4, 2),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 2, 1, new int[]{101, 2, 2, 2}, 4, 4, 2),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 2, 2, new int[]{101, 2, 2, 2}, 4, 4, 2),        //09 삼바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 25000 * 3, 0, new int[]{2, 2, 2, 2}, 4, 4, 3),          //04 삼바 x 4  상
                new ALDScoreTableInfo( 50000 * 3, 1, new int[]{2, 2, 2, 2}, 4, 4, 3),          //05 삼바 x 4  중
                new ALDScoreTableInfo( 25000 * 3, 2, new int[]{2, 2, 2, 2}, 4, 4, 3),          //06 삼바 x 4  하
                new ALDScoreTableInfo( 25000 * 3, 0, new int[]{2, 2, 2, 101}, 4, 4, 3),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 3, 1, new int[]{2, 2, 2, 101}, 4, 4, 3),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 3, 2, new int[]{2, 2, 2, 101}, 4, 4, 3),        //09 삼바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 25000 * 3, 0, new int[]{101, 2, 2, 2}, 4, 4, 3),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 3, 1, new int[]{101, 2, 2, 2}, 4, 4, 3),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 3, 2, new int[]{101, 2, 2, 2}, 4, 4, 3),        //09 삼바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 25000 * 5, 0, new int[]{2, 2, 2, 2}, 4, 4, 5),          //04 삼바 x 4  상
                new ALDScoreTableInfo( 50000 * 5, 1, new int[]{2, 2, 2, 2}, 4, 4, 5),          //05 삼바 x 4  중
                new ALDScoreTableInfo( 25000 * 5, 2, new int[]{2, 2, 2, 2}, 4, 4, 5),          //06 삼바 x 4  하
                new ALDScoreTableInfo( 25000 * 5, 0, new int[]{2, 2, 2, 101}, 4, 4, 5),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 5, 1, new int[]{2, 2, 2, 101}, 4, 4, 5),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 5, 2, new int[]{2, 2, 2, 101}, 4, 4, 5),        //09 삼바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 25000 * 5, 0, new int[]{101, 2, 2, 2}, 4, 4, 5),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 5, 1, new int[]{101, 2, 2, 2}, 4, 4, 5),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 5, 2, new int[]{101, 2, 2, 2}, 4, 4, 5),        //09 삼바 x 3, 조커 X 1, 하
            
                new ALDScoreTableInfo( 25000 * 10, 0, new int[]{2, 2, 2, 2}, 4, 4, 10),          //04 삼바 x 4  상
                new ALDScoreTableInfo( 50000 * 10, 1, new int[]{2, 2, 2, 2}, 4, 4, 10),          //05 삼바 x 4  중
                new ALDScoreTableInfo( 25000 * 10, 2, new int[]{2, 2, 2, 2}, 4, 4, 10),          //06 삼바 x 4  하
                new ALDScoreTableInfo( 25000 * 10, 0, new int[]{2, 2, 2, 101}, 4, 4, 10),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 10, 1, new int[]{2, 2, 2, 101}, 4, 4, 10),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 10, 2, new int[]{2, 2, 2, 101}, 4, 4, 10),        //09 삼바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 25000 * 10, 0, new int[]{101, 2, 2, 2}, 4, 4, 10),        //07 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 50000 * 10, 1, new int[]{101, 2, 2, 2}, 4, 4, 10),        //08 삼바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 25000 * 10, 2, new int[]{101, 2, 2, 2}, 4, 4, 10),        //09 삼바 x 3, 조커 X 1, 하
                

                new ALDScoreTableInfo( 10100 * 1, 0, new int[]{2, 2, 2, 11}, 3, 3, 1),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 1, 1, new int[]{2, 2, 2, 11}, 3, 3, 1),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 1, 2, new int[]{2, 2, 2, 11}, 4, 3, 1),         //18 삼바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 10100 * 1, 0, new int[]{11, 2, 2, 2}, 3, 3, 1),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 1, 1, new int[]{11, 2, 2, 2}, 3, 3, 1),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 1, 2, new int[]{11, 2, 2, 2}, 4, 3, 1),         //18 삼바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 10100 * 2, 0, new int[]{2, 2, 2, 11}, 3, 3, 2),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 2, 1, new int[]{2, 2, 2, 11}, 3, 3, 2),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 2, 2, new int[]{2, 2, 2, 11}, 4, 3, 2),         //18 삼바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 10100 * 2, 0, new int[]{11, 2, 2, 2}, 3, 3, 2),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 2, 1, new int[]{11, 2, 2, 2}, 3, 3, 2),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 2, 2, new int[]{11, 2, 2, 2}, 4, 3, 2),         //18 삼바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 10100 * 3, 0, new int[]{2, 2, 2, 11}, 3, 3, 3),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 3, 1, new int[]{2, 2, 2, 11}, 3, 3, 3),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 3, 2, new int[]{2, 2, 2, 11}, 4, 3, 3),         //18 삼바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 10100 * 3, 0, new int[]{11, 2, 2, 2}, 3, 3, 3),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 3, 1, new int[]{11, 2, 2, 2}, 3, 3, 3),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 3, 2, new int[]{11, 2, 2, 2}, 4, 3, 3),         //18 삼바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 10100 * 5, 0, new int[]{2, 2, 2, 11}, 3, 3, 5),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 5, 1, new int[]{2, 2, 2, 11}, 3, 3, 5),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 5, 2, new int[]{2, 2, 2, 11}, 4, 3, 5),         //18 삼바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 10100 * 5, 0, new int[]{11, 2, 2, 2}, 3, 3, 5),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 5, 1, new int[]{11, 2, 2, 2}, 3, 3, 5),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 5, 2, new int[]{11, 2, 2, 2}, 4, 3, 5),         //18 삼바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 10100 * 10, 0, new int[]{2, 2, 2, 11}, 3, 3, 10),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 10, 1, new int[]{2, 2, 2, 11}, 3, 3, 10),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 10, 2, new int[]{2, 2, 2, 11}, 4, 3, 10),         //18 삼바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 10100 * 10, 0, new int[]{11, 2, 2, 2}, 3, 3, 10),         //16 삼바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 20100 * 10, 1, new int[]{11, 2, 2, 2}, 3, 3, 10),         //17 삼바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 10100 * 10, 2, new int[]{11, 2, 2, 2}, 4, 3, 10),         //18 삼바 x 3, 체리 X 1, 하



                new ALDScoreTableInfo( 10000 * 1, 0, new int[]{2, 2, 2, 0}, 3, 3, 1),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 1, 1, new int[]{2, 2, 2, 0}, 3, 3, 1),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 1, 2, new int[]{2, 2, 2, 0}, 3, 3, 1),          //21 삼바 x 3, 하
                new ALDScoreTableInfo( 10000 * 1, 0, new int[]{0, 2, 2, 2}, 3, 3, 1),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 1, 1, new int[]{0, 2, 2, 2}, 3, 3, 1),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 1, 2, new int[]{0, 2, 2, 2}, 3, 3, 1),          //21 삼바 x 3, 하

                new ALDScoreTableInfo( 10000 * 2, 0, new int[]{2, 2, 2, 0}, 3, 3, 2),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 2, 1, new int[]{2, 2, 2, 0}, 3, 3, 2),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 2, 2, new int[]{2, 2, 2, 0}, 3, 3, 2),          //21 삼바 x 3, 하
                new ALDScoreTableInfo( 10000 * 2, 0, new int[]{0, 2, 2, 2}, 3, 3, 2),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 2, 1, new int[]{0, 2, 2, 2}, 3, 3, 2),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 2, 2, new int[]{0, 2, 2, 2}, 3, 3, 2),          //21 삼바 x 3, 하

                new ALDScoreTableInfo( 10000 * 3, 0, new int[]{2, 2, 2, 0}, 3, 3, 3),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 3, 1, new int[]{2, 2, 2, 0}, 3, 3, 3),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 3, 2, new int[]{2, 2, 2, 0}, 3, 3, 3),          //21 삼바 x 3, 하
                new ALDScoreTableInfo( 10000 * 3, 0, new int[]{0, 2, 2, 2}, 3, 3, 3),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 3, 1, new int[]{0, 2, 2, 2}, 3, 3, 3),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 3, 2, new int[]{0, 2, 2, 2}, 3, 3, 3),          //21 삼바 x 3, 하

                new ALDScoreTableInfo( 10000 * 5, 0, new int[]{2, 2, 2, 0}, 3, 3, 5),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 5, 1, new int[]{2, 2, 2, 0}, 3, 3, 5),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 5, 2, new int[]{2, 2, 2, 0}, 3, 3, 5),          //21 삼바 x 3, 하
                new ALDScoreTableInfo( 10000 * 5, 0, new int[]{0, 2, 2, 2}, 3, 3, 5),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 5, 1, new int[]{0, 2, 2, 2}, 3, 3, 5),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 5, 2, new int[]{0, 2, 2, 2}, 3, 3, 5),          //21 삼바 x 3, 하

                new ALDScoreTableInfo( 10000 * 10, 0, new int[]{2, 2, 2, 0}, 3, 3, 10),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 10, 1, new int[]{2, 2, 2, 0}, 3, 3, 10),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 10, 2, new int[]{2, 2, 2, 0}, 3, 3, 10),          //21 삼바 x 3, 하
                new ALDScoreTableInfo( 10000 * 10, 0, new int[]{0, 2, 2, 2}, 3, 3, 10),          //19 삼바 x 3, 상
                new ALDScoreTableInfo( 20000 * 10, 1, new int[]{0, 2, 2, 2}, 3, 3, 10),          //20 삼바 x 3, 중
                new ALDScoreTableInfo( 10000 * 10, 2, new int[]{0, 2, 2, 2}, 3, 3, 10),          //21 삼바 x 3, 하
             



                new ALDScoreTableInfo( 20000 * 1, 0, new int[]{3, 3, 3, 3}, 4, 14, 1),         //22 투바 x 4, 상
                new ALDScoreTableInfo( 40000 * 1, 1, new int[]{3, 3, 3, 3}, 4, 14, 1),         //23 투바 x 4, 중
                new ALDScoreTableInfo( 20000 * 1, 2, new int[]{3, 3, 3, 3}, 4, 14, 1),         //24 투바 x 4, 하
                new ALDScoreTableInfo( 20000 * 1, 0, new int[]{3, 3, 3, 101}, 4, 14, 1),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 1, 1, new int[]{3, 3, 3, 101}, 4, 14, 1),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 1, 2, new int[]{3, 3, 3, 101}, 4, 14, 1),       //27 투바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 20000 * 1, 0, new int[]{101, 3, 3, 3}, 4, 14, 1),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 1, 1, new int[]{101, 3, 3, 3}, 4, 14, 1),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 1, 2, new int[]{101, 3, 3, 3}, 4, 14, 1),       //27 투바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 20000 * 2, 0, new int[]{3, 3, 3, 3}, 4, 14, 2),         //22 투바 x 4, 상
                new ALDScoreTableInfo( 40000 * 2, 1, new int[]{3, 3, 3, 3}, 4, 14, 2),         //23 투바 x 4, 중
                new ALDScoreTableInfo( 20000 * 2, 2, new int[]{3, 3, 3, 3}, 4, 14, 2),         //24 투바 x 4, 하
                new ALDScoreTableInfo( 20000 * 2, 0, new int[]{3, 3, 3, 101}, 4, 14, 2),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 2, 1, new int[]{3, 3, 3, 101}, 4, 14, 2),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 2, 2, new int[]{3, 3, 3, 101}, 4, 14, 2),       //27 투바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 20000 * 2, 0, new int[]{101, 3, 3, 3}, 4, 14, 2),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 2, 1, new int[]{101, 3, 3, 3}, 4, 14, 2),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 2, 2, new int[]{101, 3, 3, 3}, 4, 14, 2),       //27 투바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 20000 * 3, 0, new int[]{3, 3, 3, 3}, 4, 14, 3),         //22 투바 x 4, 상
                new ALDScoreTableInfo( 40000 * 3, 1, new int[]{3, 3, 3, 3}, 4, 14, 3),         //23 투바 x 4, 중
                new ALDScoreTableInfo( 20000 * 3, 2, new int[]{3, 3, 3, 3}, 4, 14, 3),         //24 투바 x 4, 하
                new ALDScoreTableInfo( 20000 * 3, 0, new int[]{3, 3, 3, 101}, 4, 14, 3),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 3, 1, new int[]{3, 3, 3, 101}, 4, 14, 3),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 3, 2, new int[]{3, 3, 3, 101}, 4, 14, 3),       //27 투바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 20000 * 3, 0, new int[]{101, 3, 3, 3}, 4, 14, 3),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 3, 1, new int[]{101, 3, 3, 3}, 4, 14, 3),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 3, 2, new int[]{101, 3, 3, 3}, 4, 14, 3),       //27 투바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 20000 * 5, 0, new int[]{3, 3, 3, 3}, 4, 14, 5),         //22 투바 x 4, 상
                new ALDScoreTableInfo( 40000 * 5, 1, new int[]{3, 3, 3, 3}, 4, 14, 5),         //23 투바 x 4, 중
                new ALDScoreTableInfo( 20000 * 5, 2, new int[]{3, 3, 3, 3}, 4, 14, 5),         //24 투바 x 4, 하
                new ALDScoreTableInfo( 20000 * 5, 0, new int[]{3, 3, 3, 101}, 4, 14, 5),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 5, 1, new int[]{3, 3, 3, 101}, 4, 14, 5),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 5, 2, new int[]{3, 3, 3, 101}, 4, 14, 5),       //27 투바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 20000 * 5, 0, new int[]{101, 3, 3, 3}, 4, 14, 5),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 5, 1, new int[]{101, 3, 3, 3}, 4, 14, 5),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 5, 2, new int[]{101, 3, 3, 3}, 4, 14, 5),       //27 투바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 20000 * 10, 0, new int[]{3, 3, 3, 3}, 4, 14, 10),         //22 투바 x 4, 상
                new ALDScoreTableInfo( 40000 * 10, 1, new int[]{3, 3, 3, 3}, 4, 14, 10),         //23 투바 x 4, 중
                new ALDScoreTableInfo( 20000 * 10, 2, new int[]{3, 3, 3, 3}, 4, 14, 10),         //24 투바 x 4, 하
                new ALDScoreTableInfo( 20000 * 10, 0, new int[]{3, 3, 3, 101}, 4, 14, 10),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 10, 1, new int[]{3, 3, 3, 101}, 4, 14, 10),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 10, 2, new int[]{3, 3, 3, 101}, 4, 14, 10),       //27 투바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 20000 * 10, 0, new int[]{101, 3, 3, 3}, 4, 14, 10),       //25 투바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 40000 * 10, 1, new int[]{101, 3, 3, 3}, 4, 14, 10),       //26 투바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 20000 * 10, 2, new int[]{101, 3, 3, 3}, 4, 14, 10),       //27 투바 x 3, 조커 X 1, 하
                

                new ALDScoreTableInfo( 5100 * 1,  0, new int[]{3, 3, 3, 11}, 3, 13, 1),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 1, 1, new int[]{3, 3, 3, 11}, 3, 13, 1),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 1,  2, new int[]{3, 3, 3, 11}, 3, 13, 1),        //36 투바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 5100 * 1,  0, new int[]{11, 3, 3, 3}, 3, 13, 1),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 1, 1, new int[]{11, 3, 3, 3}, 3, 13, 1),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 1,  2, new int[]{11, 3, 3, 3}, 3, 13, 1),        //36 투바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 5100 * 2,  0, new int[]{3, 3, 3, 11}, 3, 13, 2),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 2, 1, new int[]{3, 3, 3, 11}, 3, 13, 2),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 2,  2, new int[]{3, 3, 3, 11}, 3, 13, 2),        //36 투바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 5100 * 2,  0, new int[]{11, 3, 3, 3}, 3, 13, 2),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 2, 1, new int[]{11, 3, 3, 3}, 3, 13, 2),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 2,  2, new int[]{11, 3, 3, 3}, 3, 13, 2),        //36 투바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 5100 * 3,  0, new int[]{3, 3, 3, 11}, 3, 13, 3),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 3, 1, new int[]{3, 3, 3, 11}, 3, 13, 3),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 3,  2, new int[]{3, 3, 3, 11}, 3, 13, 3),        //36 투바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 5100 * 3,  0, new int[]{11, 3, 3, 3}, 3, 13, 3),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 3, 1, new int[]{11, 3, 3, 3}, 3, 13, 3),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 3,  2, new int[]{11, 3, 3, 3}, 3, 13, 3),        //36 투바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 5100 * 5,  0, new int[]{3, 3, 3, 11}, 3, 13, 5),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 5, 1, new int[]{3, 3, 3, 11}, 3, 13, 5),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 5,  2, new int[]{3, 3, 3, 11}, 3, 13, 5),        //36 투바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 5100 * 5,  0, new int[]{11, 3, 3, 3}, 3, 13, 5),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 5, 1, new int[]{11, 3, 3, 3}, 3, 13, 5),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 5,  2, new int[]{11, 3, 3, 3}, 3, 13, 5),        //36 투바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 5100 * 10,  0, new int[]{3, 3, 3, 11}, 3, 13, 10),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 10, 1, new int[]{3, 3, 3, 11}, 3, 13, 10),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 10,  2, new int[]{3, 3, 3, 11}, 3, 13, 10),        //36 투바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 5100 * 10,  0, new int[]{11, 3, 3, 3}, 3, 13, 10),        //34 투바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 10100 * 10, 1, new int[]{11, 3, 3, 3}, 3, 13, 10),        //35 투바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 5100 * 10,  2, new int[]{11, 3, 3, 3}, 3, 13, 10),        //36 투바 x 3, 체리 X 1, 하



                new ALDScoreTableInfo( 5000 * 1,  0, new int[]{3, 3, 3, 0}, 3, 13, 1),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 1, 1, new int[]{3, 3, 3, 0}, 3, 13, 1),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 1,  2, new int[]{3, 3, 3, 0}, 3, 13, 1),         //39 투바 x 3, 하
                new ALDScoreTableInfo( 5000 * 1,  0, new int[]{0, 3, 3, 3}, 3, 13, 1),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 1, 1, new int[]{0, 3, 3, 3}, 3, 13, 1),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 1,  2, new int[]{0, 3, 3, 3}, 3, 13, 1),         //39 투바 x 3, 하

                new ALDScoreTableInfo( 5000 * 2,  0, new int[]{3, 3, 3, 0}, 3, 13, 2),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 2, 1, new int[]{3, 3, 3, 0}, 3, 13, 2),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 2,  2, new int[]{3, 3, 3, 0}, 3, 13, 2),         //39 투바 x 3, 하
                new ALDScoreTableInfo( 5000 * 2,  0, new int[]{0, 3, 3, 3}, 3, 13, 2),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 2, 1, new int[]{0, 3, 3, 3}, 3, 13, 2),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 2,  2, new int[]{0, 3, 3, 3}, 3, 13, 2),         //39 투바 x 3, 하

                new ALDScoreTableInfo( 5000 * 3,  0, new int[]{3, 3, 3, 0}, 3, 13, 3),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 3, 1, new int[]{3, 3, 3, 0}, 3, 13, 3),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 3,  2, new int[]{3, 3, 3, 0}, 3, 13, 3),         //39 투바 x 3, 하
                new ALDScoreTableInfo( 5000 * 3,  0, new int[]{0, 3, 3, 3}, 3, 13, 3),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 3, 1, new int[]{0, 3, 3, 3}, 3, 13, 3),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 3,  2, new int[]{0, 3, 3, 3}, 3, 13, 3),         //39 투바 x 3, 하

                new ALDScoreTableInfo( 5000 * 5,  0, new int[]{3, 3, 3, 0}, 3, 13, 5),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 5, 1, new int[]{3, 3, 3, 0}, 3, 13, 5),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 5,  2, new int[]{3, 3, 3, 0}, 3, 13, 5),         //39 투바 x 3, 하
                new ALDScoreTableInfo( 5000 * 5,  0, new int[]{0, 3, 3, 3}, 3, 13, 5),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 5, 1, new int[]{0, 3, 3, 3}, 3, 13, 5),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 5,  2, new int[]{0, 3, 3, 3}, 3, 13, 5),         //39 투바 x 3, 하

                new ALDScoreTableInfo( 5000 * 10,  0, new int[]{3, 3, 3, 0}, 3, 13, 10),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 10, 1, new int[]{3, 3, 3, 0}, 3, 13, 10),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 10,  2, new int[]{3, 3, 3, 0}, 3, 13, 10),         //39 투바 x 3, 하
                new ALDScoreTableInfo( 5000 * 10,  0, new int[]{0, 3, 3, 3}, 3, 13, 10),         //37 투바 x 3, 상
                new ALDScoreTableInfo( 10000 * 10, 1, new int[]{0, 3, 3, 3}, 3, 13, 10),         //38 투바 x 3, 중
                new ALDScoreTableInfo( 5000 * 10,  2, new int[]{0, 3, 3, 3}, 3, 13, 10),         //39 투바 x 3, 하




                new ALDScoreTableInfo( 16000 * 1, 0, new int[]{4, 4, 4, 4}, 4, 24, 1),         //40 원바 x 4, 상
                new ALDScoreTableInfo( 20000 * 1, 1, new int[]{4, 4, 4, 4}, 4, 24, 1),         //41 원바 x 4, 중
                new ALDScoreTableInfo( 16000 * 1, 2, new int[]{4, 4, 4, 4}, 4, 24, 1),         //42 원바 x 4, 하
                new ALDScoreTableInfo( 16000 * 1, 0, new int[]{4, 4, 4, 101}, 4, 24, 1),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 1, 1, new int[]{4, 4, 4, 101}, 4, 24, 1),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 1, 2, new int[]{4, 4, 4, 101}, 4, 24, 1),       //45 원바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 16000 * 1, 0, new int[]{101, 4, 4, 4}, 4, 24, 1),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 1, 1, new int[]{101, 4, 4, 4}, 4, 24, 1),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 1, 2, new int[]{101, 4, 4, 4}, 4, 24, 1),       //45 원바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 16000 * 2, 0, new int[]{4, 4, 4, 4}, 4, 24, 2),         //40 원바 x 4, 상
                new ALDScoreTableInfo( 20000 * 2, 1, new int[]{4, 4, 4, 4}, 4, 24, 2),         //41 원바 x 4, 중
                new ALDScoreTableInfo( 16000 * 2, 2, new int[]{4, 4, 4, 4}, 4, 24, 2),         //42 원바 x 4, 하
                new ALDScoreTableInfo( 16000 * 2, 0, new int[]{4, 4, 4, 101}, 4, 24, 2),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 2, 1, new int[]{4, 4, 4, 101}, 4, 24, 2),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 2, 2, new int[]{4, 4, 4, 101}, 4, 24, 2),       //45 원바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 16000 * 2, 0, new int[]{101, 4, 4, 4}, 4, 24, 2),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 2, 1, new int[]{101, 4, 4, 4}, 4, 24, 2),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 2, 2, new int[]{101, 4, 4, 4}, 4, 24, 2),       //45 원바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 16000 * 3, 0, new int[]{4, 4, 4, 4}, 4, 24, 3),         //40 원바 x 4, 상
                new ALDScoreTableInfo( 20000 * 3, 1, new int[]{4, 4, 4, 4}, 4, 24, 3),         //41 원바 x 4, 중
                new ALDScoreTableInfo( 16000 * 3, 2, new int[]{4, 4, 4, 4}, 4, 24, 3),         //42 원바 x 4, 하
                new ALDScoreTableInfo( 16000 * 3, 0, new int[]{4, 4, 4, 101}, 4, 24, 3),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 3, 1, new int[]{4, 4, 4, 101}, 4, 24, 3),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 3, 2, new int[]{4, 4, 4, 101}, 4, 24, 3),       //45 원바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 16000 * 3, 0, new int[]{101, 4, 4, 4}, 4, 24, 3),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 3, 1, new int[]{101, 4, 4, 4}, 4, 24, 3),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 3, 2, new int[]{101, 4, 4, 4}, 4, 24, 3),       //45 원바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 16000 * 5, 0, new int[]{4, 4, 4, 4}, 4, 24, 5),         //40 원바 x 4, 상
                new ALDScoreTableInfo( 20000 * 5, 1, new int[]{4, 4, 4, 4}, 4, 24, 5),         //41 원바 x 4, 중
                new ALDScoreTableInfo( 16000 * 5, 2, new int[]{4, 4, 4, 4}, 4, 24, 5),         //42 원바 x 4, 하
                new ALDScoreTableInfo( 16000 * 5, 0, new int[]{4, 4, 4, 101}, 4, 24, 5),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 5, 1, new int[]{4, 4, 4, 101}, 4, 24, 5),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 5, 2, new int[]{4, 4, 4, 101}, 4, 24, 5),       //45 원바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 16000 * 5, 0, new int[]{101, 4, 4, 4}, 4, 24, 5),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 5, 1, new int[]{101, 4, 4, 4}, 4, 24, 5),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 5, 2, new int[]{101, 4, 4, 4}, 4, 24, 5),       //45 원바 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 16000 * 10, 0, new int[]{4, 4, 4, 4}, 4, 24, 10),         //40 원바 x 4, 상
                new ALDScoreTableInfo( 20000 * 10, 1, new int[]{4, 4, 4, 4}, 4, 24, 10),         //41 원바 x 4, 중
                new ALDScoreTableInfo( 16000 * 10, 2, new int[]{4, 4, 4, 4}, 4, 24, 10),         //42 원바 x 4, 하
                new ALDScoreTableInfo( 16000 * 10, 0, new int[]{4, 4, 4, 101}, 4, 24, 10),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 10, 1, new int[]{4, 4, 4, 101}, 4, 24, 10),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 10, 2, new int[]{4, 4, 4, 101}, 4, 24, 10),       //45 원바 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 16000 * 10, 0, new int[]{101, 4, 4, 4}, 4, 24, 10),       //43 원바 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 20000 * 10, 1, new int[]{101, 4, 4, 4}, 4, 24, 10),       //44 원바 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 16000 * 10, 2, new int[]{101, 4, 4, 4}, 4, 24, 10),       //45 원바 x 3, 조커 X 1, 하
                

                new ALDScoreTableInfo( 2100 * 1, 0, new int[]{4, 4, 4, 11}, 3, 23, 1),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 1, 1, new int[]{4, 4, 4, 11}, 3, 23, 1),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 1, 2, new int[]{4, 4, 4, 11}, 3, 23, 1),         //54 원바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 2100 * 1, 0, new int[]{11, 4, 4, 4}, 3, 23, 1),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 1, 1, new int[]{11, 4, 4, 4}, 3, 23, 1),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 1, 2, new int[]{11, 4, 4, 4}, 3, 23, 1),         //54 원바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 2100 * 2, 0, new int[]{4, 4, 4, 11}, 3, 23, 2),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 2, 1, new int[]{4, 4, 4, 11}, 3, 23, 2),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 2, 2, new int[]{4, 4, 4, 11}, 3, 23, 2),         //54 원바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 2100 * 2, 0, new int[]{11, 4, 4, 4}, 3, 23, 2),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 2, 1, new int[]{11, 4, 4, 4}, 3, 23, 2),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 2, 2, new int[]{11, 4, 4, 4}, 3, 23, 2),         //54 원바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 2100 * 3, 0, new int[]{4, 4, 4, 11}, 3, 23, 3),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 3, 1, new int[]{4, 4, 4, 11}, 3, 23, 3),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 3, 2, new int[]{4, 4, 4, 11}, 3, 23, 3),         //54 원바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 2100 * 3, 0, new int[]{11, 4, 4, 4}, 3, 23, 3),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 3, 1, new int[]{11, 4, 4, 4}, 3, 23, 3),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 3, 2, new int[]{11, 4, 4, 4}, 3, 23, 3),         //54 원바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 2100 * 5, 0, new int[]{4, 4, 4, 11}, 3, 23, 5),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 5, 1, new int[]{4, 4, 4, 11}, 3, 23, 5),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 5, 2, new int[]{4, 4, 4, 11}, 3, 23, 5),         //54 원바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 2100 * 5, 0, new int[]{11, 4, 4, 4}, 3, 23, 5),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 5, 1, new int[]{11, 4, 4, 4}, 3, 23, 5),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 5, 2, new int[]{11, 4, 4, 4}, 3, 23, 5),         //54 원바 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 2100 * 10, 0, new int[]{4, 4, 4, 11}, 3, 23, 10),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 10, 1, new int[]{4, 4, 4, 11}, 3, 23, 10),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 10, 2, new int[]{4, 4, 4, 11}, 3, 23, 10),         //54 원바 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 2100 * 10, 0, new int[]{11, 4, 4, 4}, 3, 23, 10),         //52 원바 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 4100 * 10, 1, new int[]{11, 4, 4, 4}, 3, 23, 10),         //53 원바 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 2100 * 10, 2, new int[]{11, 4, 4, 4}, 3, 23, 10),         //54 원바 x 3, 체리 X 1, 하



                new ALDScoreTableInfo( 2000 * 1, 0, new int[]{4, 4, 4, 0}, 3, 23, 1),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 1, 1, new int[]{4, 4, 4, 0}, 3, 23, 1),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 1, 2, new int[]{4, 4, 4, 0}, 3, 23, 1),           //57 원바 x 3, 하
                new ALDScoreTableInfo( 2000 * 1, 0, new int[]{0, 4, 4, 4}, 3, 23, 1),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 1, 1, new int[]{0, 4, 4, 4}, 3, 23, 1),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 1, 2, new int[]{0, 4, 4, 4}, 3, 23, 1),           //57 원바 x 3, 하

                new ALDScoreTableInfo( 2000 * 2, 0, new int[]{4, 4, 4, 0}, 3, 23, 2),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 2, 1, new int[]{4, 4, 4, 0}, 3, 23, 2),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 2, 2, new int[]{4, 4, 4, 0}, 3, 23, 2),           //57 원바 x 3, 하
                new ALDScoreTableInfo( 2000 * 2, 0, new int[]{0, 4, 4, 4}, 3, 23, 2),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 2, 1, new int[]{0, 4, 4, 4}, 3, 23, 2),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 2, 2, new int[]{0, 4, 4, 4}, 3, 23, 2),           //57 원바 x 3, 하

                new ALDScoreTableInfo( 2000 * 3, 0, new int[]{4, 4, 4, 0}, 3, 23, 3),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 3, 1, new int[]{4, 4, 4, 0}, 3, 23, 3),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 3, 2, new int[]{4, 4, 4, 0}, 3, 23, 3),           //57 원바 x 3, 하
                new ALDScoreTableInfo( 2000 * 3, 0, new int[]{0, 4, 4, 4}, 3, 23, 3),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 3, 1, new int[]{0, 4, 4, 4}, 3, 23, 3),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 3, 2, new int[]{0, 4, 4, 4}, 3, 23, 3),           //57 원바 x 3, 하

                new ALDScoreTableInfo( 2000 * 5, 0, new int[]{4, 4, 4, 0}, 3, 23, 5),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 5, 1, new int[]{4, 4, 4, 0}, 3, 23, 5),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 5, 2, new int[]{4, 4, 4, 0}, 3, 23, 5),           //57 원바 x 3, 하
                new ALDScoreTableInfo( 2000 * 5, 0, new int[]{0, 4, 4, 4}, 3, 23, 5),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 5, 1, new int[]{0, 4, 4, 4}, 3, 23, 5),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 5, 2, new int[]{0, 4, 4, 4}, 3, 23, 5),           //57 원바 x 3, 하

                new ALDScoreTableInfo( 2000 * 10, 0, new int[]{4, 4, 4, 0}, 3, 23, 10),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 10, 1, new int[]{4, 4, 4, 0}, 3, 23, 10),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 10, 2, new int[]{4, 4, 4, 0}, 3, 23, 10),           //57 원바 x 3, 하
                new ALDScoreTableInfo( 2000 * 10, 0, new int[]{0, 4, 4, 4}, 3, 23, 10),           //55 원바 x 3, 상
                new ALDScoreTableInfo( 4000 * 10, 1, new int[]{0, 4, 4, 4}, 3, 23, 10),           //56 원바 x 3, 중
                new ALDScoreTableInfo( 2000 * 10, 2, new int[]{0, 4, 4, 4}, 3, 23, 10),           //57 원바 x 3, 하




                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{5, 5, 5, 5}, 4, 34, 1),         //58 세븐 x 4, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{5, 5, 5, 5}, 4, 34, 1),         //59 세븐 x 4, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{5, 5, 5, 5}, 4, 34, 1),         //60 세븐 x 4, 하
                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{5, 5, 5, 101}, 4, 34, 1),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{5, 5, 5, 101}, 4, 34, 1),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{5, 5, 5, 101}, 4, 34, 1),       //63 세븐 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{101, 5, 5, 5}, 4, 34, 1),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{101, 5, 5, 5}, 4, 34, 1),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{101, 5, 5, 5}, 4, 34, 1),       //63 세븐 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{5, 5, 5, 5}, 4, 34, 2),         //58 세븐 x 4, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{5, 5, 5, 5}, 4, 34, 2),         //59 세븐 x 4, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{5, 5, 5, 5}, 4, 34, 2),         //60 세븐 x 4, 하
                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{5, 5, 5, 101}, 4, 34, 2),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{5, 5, 5, 101}, 4, 34, 2),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{5, 5, 5, 101}, 4, 34, 2),       //63 세븐 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{101, 5, 5, 5}, 4, 34, 2),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{101, 5, 5, 5}, 4, 34, 2),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{101, 5, 5, 5}, 4, 34, 2),       //63 세븐 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{5, 5, 5, 5}, 4, 34, 3),         //58 세븐 x 4, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{5, 5, 5, 5}, 4, 34, 3),         //59 세븐 x 4, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{5, 5, 5, 5}, 4, 34, 3),         //60 세븐 x 4, 하
                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{5, 5, 5, 101}, 4, 34, 3),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{5, 5, 5, 101}, 4, 34, 3),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{5, 5, 5, 101}, 4, 34, 3),       //63 세븐 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{101, 5, 5, 5}, 4, 34, 3),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{101, 5, 5, 5}, 4, 34, 3),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{101, 5, 5, 5}, 4, 34, 3),       //63 세븐 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{5, 5, 5, 5}, 4, 34, 5),         //58 세븐 x 4, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{5, 5, 5, 5}, 4, 34, 5),         //59 세븐 x 4, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{5, 5, 5, 5}, 4, 34, 5),         //60 세븐 x 4, 하
                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{5, 5, 5, 101}, 4, 34, 5),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{5, 5, 5, 101}, 4, 34, 5),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{5, 5, 5, 101}, 4, 34, 5),       //63 세븐 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{101, 5, 5, 5}, 4, 34, 5),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{101, 5, 5, 5}, 4, 34, 5),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{101, 5, 5, 5}, 4, 34, 5),       //63 세븐 x 3, 조커 X 1, 하
            
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{5, 5, 5, 5}, 4, 34, 10),         //58 세븐 x 4, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{5, 5, 5, 5}, 4, 34, 10),         //59 세븐 x 4, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{5, 5, 5, 5}, 4, 34, 10),         //60 세븐 x 4, 하
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{5, 5, 5, 101}, 4, 34, 10),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{5, 5, 5, 101}, 4, 34, 10),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{5, 5, 5, 101}, 4, 34, 10),       //63 세븐 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{101, 5, 5, 5}, 4, 34, 10),       //61 세븐 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{101, 5, 5, 5}, 4, 34, 10),       //62 세븐 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{101, 5, 5, 5}, 4, 34, 10),       //63 세븐 x 3, 조커 X 1, 하


                new ALDScoreTableInfo( 700 * 1, 0, new int[]{5, 5, 5, 11}, 3, 33, 1),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 1, 1, new int[]{5, 5, 5, 11}, 3, 33, 1),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 1, 2, new int[]{5, 5, 5, 11}, 3, 33, 1),          //72 세븐 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 1, 0, new int[]{11, 5, 5, 5}, 3, 33, 1),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 1, 1, new int[]{11, 5, 5, 5}, 3, 33, 1),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 1, 2, new int[]{11, 5, 5, 5}, 3, 33, 1),          //72 세븐 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 2, 0, new int[]{5, 5, 5, 11}, 3, 33, 2),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 2, 1, new int[]{5, 5, 5, 11}, 3, 33, 2),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 2, 2, new int[]{5, 5, 5, 11}, 3, 33, 2),          //72 세븐 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 2, 0, new int[]{11, 5, 5, 5}, 3, 33, 2),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 2, 1, new int[]{11, 5, 5, 5}, 3, 33, 2),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 2, 2, new int[]{11, 5, 5, 5}, 3, 33, 2),          //72 세븐 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 3, 0, new int[]{5, 5, 5, 11}, 3, 33, 3),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 3, 1, new int[]{5, 5, 5, 11}, 3, 33, 3),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 3, 2, new int[]{5, 5, 5, 11}, 3, 33, 3),          //72 세븐 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 3, 0, new int[]{11, 5, 5, 5}, 3, 33, 3),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 3, 1, new int[]{11, 5, 5, 5}, 3, 33, 3),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 3, 2, new int[]{11, 5, 5, 5}, 3, 33, 3),          //72 세븐 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 5, 0, new int[]{5, 5, 5, 11}, 3, 33, 5),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 5, 1, new int[]{5, 5, 5, 11}, 3, 33, 5),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 5, 2, new int[]{5, 5, 5, 11}, 3, 33, 5),          //72 세븐 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 5, 0, new int[]{11, 5, 5, 5}, 3, 33, 5),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 5, 1, new int[]{11, 5, 5, 5}, 3, 33, 5),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 5, 2, new int[]{11, 5, 5, 5}, 3, 33, 5),          //72 세븐 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 10, 0, new int[]{5, 5, 5, 11}, 3, 33, 10),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 10, 1, new int[]{5, 5, 5, 11}, 3, 33, 10),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 10, 2, new int[]{5, 5, 5, 11}, 3, 33, 10),          //72 세븐 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 10, 0, new int[]{11, 5, 5, 5}, 3, 33, 10),          //70 세븐 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 10, 1, new int[]{11, 5, 5, 5}, 3, 33, 10),          //71 세븐 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 10, 2, new int[]{11, 5, 5, 5}, 3, 33, 10),          //72 세븐 x 3, 체리 X 1, 하



                new ALDScoreTableInfo( 600 * 1, 0, new int[]{5, 5, 5, 0}, 3, 33, 1),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{5, 5, 5, 0}, 3, 33, 1),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{5, 5, 5, 0}, 3, 33, 1),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{0, 5, 5, 5}, 3, 33, 1),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{0, 5, 5, 5}, 3, 33, 1),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{0, 5, 5, 5}, 3, 33, 1),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{5, 5, 0, 5}, 3, 33, 1),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{5, 5, 0, 5}, 3, 33, 1),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{5, 5, 0, 5}, 3, 33, 1),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{5, 0, 5, 5}, 3, 33, 1),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{5, 0, 5, 5}, 3, 33, 1),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{5, 0, 5, 5}, 3, 33, 1),            //75 세븐 x 3, 하
          

                new ALDScoreTableInfo( 600 * 2, 0, new int[]{5, 5, 5, 0}, 3, 33, 2),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{5, 5, 5, 0}, 3, 33, 2),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{5, 5, 5, 0}, 3, 33, 2),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{0, 5, 5, 5}, 3, 33, 2),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{0, 5, 5, 5}, 3, 33, 2),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{0, 5, 5, 5}, 3, 33, 2),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{5, 5, 0, 5}, 3, 33, 2),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{5, 5, 0, 5}, 3, 33, 2),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{5, 5, 0, 5}, 3, 33, 2),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{5, 0, 5, 5}, 3, 33, 2),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{5, 0, 5, 5}, 3, 33, 2),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{5, 0, 5, 5}, 3, 33, 2),            //75 세븐 x 3, 하

                new ALDScoreTableInfo( 600 * 3, 0, new int[]{5, 5, 5, 0}, 3, 33, 3),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{5, 5, 5, 0}, 3, 33, 3),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{5, 5, 5, 0}, 3, 33, 3),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{0, 5, 5, 5}, 3, 33, 3),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{0, 5, 5, 5}, 3, 33, 3),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{0, 5, 5, 5}, 3, 33, 3),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{5, 5, 0, 5}, 3, 33, 3),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{5, 5, 0, 5}, 3, 33, 3),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{5, 5, 0, 5}, 3, 33, 3),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{5, 0, 5, 5}, 3, 33, 3),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{5, 0, 5, 5}, 3, 33, 3),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{5, 0, 5, 5}, 3, 33, 3),            //75 세븐 x 3, 하

                new ALDScoreTableInfo( 600 * 5, 0, new int[]{5, 5, 5, 0}, 3, 33, 5),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{5, 5, 5, 0}, 3, 33, 5),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{5, 5, 5, 0}, 3, 33, 5),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{0, 5, 5, 5}, 3, 33, 5),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{0, 5, 5, 5}, 3, 33, 5),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{0, 5, 5, 5}, 3, 33, 5),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{5, 5, 0, 5}, 3, 33, 5),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{5, 5, 0, 5}, 3, 33, 5),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{5, 5, 0, 5}, 3, 33, 5),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{5, 0, 5, 5}, 3, 33, 5),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{5, 0, 5, 5}, 3, 33, 5),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{5, 0, 5, 5}, 3, 33, 5),            //75 세븐 x 3, 하

                new ALDScoreTableInfo( 600 * 10, 0, new int[]{5, 5, 5, 0}, 3, 33, 10),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{5, 5, 5, 0}, 3, 33, 10),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{5, 5, 5, 0}, 3, 33, 10),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{0, 5, 5, 5}, 3, 33, 10),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{0, 5, 5, 5}, 3, 33, 10),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{0, 5, 5, 5}, 3, 33, 10),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{5, 5, 0, 5}, 3, 33, 10),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{5, 5, 0, 5}, 3, 33, 10),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{5, 5, 0, 5}, 3, 33, 10),            //75 세븐 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{5, 0, 5, 5}, 3, 33, 10),            //73 세븐 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{5, 0, 5, 5}, 3, 33, 10),            //74 세븐 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{5, 0, 5, 5}, 3, 33, 10),            //75 세븐 x 3, 하

                new ALDScoreTableInfo( 100 * 1, 0, new int[]{5, 5, 0, 0}, 2, 32, 1),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{5, 5, 0, 0}, 2, 32, 1),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{5, 5, 0, 0}, 2, 32, 1),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{0, 0, 5, 5}, 2, 32, 1),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{0, 0, 5, 5}, 2, 32, 1),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{0, 0, 5, 5}, 2, 32, 1),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{5, 0, 0, 5}, 2, 32, 1),            //78 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{5, 0, 0, 5}, 2, 32, 1),            //78 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{5, 0, 0, 5}, 2, 32, 1),            //78 세븐 x 2, 하


                new ALDScoreTableInfo( 100 * 2, 0, new int[]{5, 5, 0, 0}, 2, 32, 2),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{5, 5, 0, 0}, 2, 32, 2),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{5, 5, 0, 0}, 2, 32, 2),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 2, 0, new int[]{0, 0, 5, 5}, 2, 32, 2),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{0, 0, 5, 5}, 2, 32, 2),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{0, 0, 5, 5}, 2, 32, 2),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 2, 0, new int[]{5, 0, 0, 5}, 2, 32, 2),            //78 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{5, 0, 0, 5}, 2, 32, 2),            //78 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{5, 0, 0, 5}, 2, 32, 2),            //78 세븐 x 2, 상

                new ALDScoreTableInfo( 100 * 3, 0, new int[]{5, 5, 0, 0}, 2, 32, 3),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{5, 5, 0, 0}, 2, 32, 3),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{5, 5, 0, 0}, 2, 32, 3),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 3, 0, new int[]{0, 0, 5, 5}, 2, 32, 3),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{0, 0, 5, 5}, 2, 32, 3),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{0, 0, 5, 5}, 2, 32, 3),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 3, 0, new int[]{5, 0, 0, 5}, 2, 32, 3),            //78 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{5, 0, 0, 5}, 2, 32, 3),            //78 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{5, 0, 0, 5}, 2, 32, 3),            //78 세븐 x 2, 하

                new ALDScoreTableInfo( 100 * 5, 0, new int[]{5, 5, 0, 0}, 2, 32, 5),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{5, 5, 0, 0}, 2, 32, 5),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{5, 5, 0, 0}, 2, 32, 5),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 5, 0, new int[]{0, 0, 5, 5}, 2, 32, 5),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{0, 0, 5, 5}, 2, 32, 5),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{0, 0, 5, 5}, 2, 32, 5),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 5, 0, new int[]{5, 0, 0, 5}, 2, 32, 5),            //78 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{5, 0, 0, 5}, 2, 32, 5),            //78 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{5, 0, 0, 5}, 2, 32, 5),            //78 세븐 x 2, 하

                new ALDScoreTableInfo( 100 * 10, 0, new int[]{5, 5, 0, 0}, 2, 32, 10),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{5, 5, 0, 0}, 2, 32, 10),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{5, 5, 0, 0}, 2, 32, 10),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 10, 0, new int[]{0, 0, 5, 5}, 2, 32, 10),            //76 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{0, 0, 5, 5}, 2, 32, 10),            //77 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{0, 0, 5, 5}, 2, 32, 10),            //78 세븐 x 2, 하
                new ALDScoreTableInfo( 100 * 10, 0, new int[]{5, 0, 0, 5}, 2, 32, 10),            //78 세븐 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{5, 0, 0, 5}, 2, 32, 10),            //78 세븐 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{5, 0, 0, 5}, 2, 32, 10),            //78 세븐 x 2, 하




                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{6, 6, 6, 6}, 4, 44, 1),         //79 스타 x 4, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{6, 6, 6, 6}, 4, 44, 1),         //80 스타 x 4, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{6, 6, 6, 6}, 4, 44, 1),         //81 스타 x 4, 하
                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{6, 6, 6, 101}, 4, 44, 1),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{6, 6, 6, 101}, 4, 44, 1),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{6, 6, 6, 101}, 4, 44, 1),       //84 스타 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{101, 6, 6, 6}, 4, 44, 1),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{101, 6, 6, 6}, 4, 44, 1),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{101, 6, 6, 6}, 4, 44, 1),       //84 스타 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{6, 6, 6, 6}, 4, 44, 2),         //79 스타 x 4, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{6, 6, 6, 6}, 4, 44, 2),         //80 스타 x 4, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{6, 6, 6, 6}, 4, 44, 2),         //81 스타 x 4, 하
                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{6, 6, 6, 101}, 4, 44, 2),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{6, 6, 6, 101}, 4, 44, 2),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{6, 6, 6, 101}, 4, 44, 2),       //84 스타 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{101, 6, 6, 6}, 4, 44, 2),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{101, 6, 6, 6}, 4, 44, 2),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{101, 6, 6, 6}, 4, 44, 2),       //84 스타 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{6, 6, 6, 6}, 4, 44, 3),         //79 스타 x 4, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{6, 6, 6, 6}, 4, 44, 3),         //80 스타 x 4, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{6, 6, 6, 6}, 4, 44, 3),         //81 스타 x 4, 하
                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{6, 6, 6, 101}, 4, 44, 3),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{6, 6, 6, 101}, 4, 44, 3),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{6, 6, 6, 101}, 4, 44, 3),       //84 스타 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{101, 6, 6, 6}, 4, 44, 3),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{101, 6, 6, 6}, 4, 44, 3),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{101, 6, 6, 6}, 4, 44, 3),       //84 스타 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{6, 6, 6, 6}, 4, 44, 5),         //79 스타 x 4, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{6, 6, 6, 6}, 4, 44, 5),         //80 스타 x 4, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{6, 6, 6, 6}, 4, 44, 5),         //81 스타 x 4, 하
                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{6, 6, 6, 101}, 4, 44, 5),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{6, 6, 6, 101}, 4, 44, 5),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{6, 6, 6, 101}, 4, 44, 5),       //84 스타 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{101, 6, 6, 6}, 4, 44, 5),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{101, 6, 6, 6}, 4, 44, 5),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{101, 6, 6, 6}, 4, 44, 5),       //84 스타 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{6, 6, 6, 6}, 4, 44, 10),         //79 스타 x 4, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{6, 6, 6, 6}, 4, 44, 10),         //80 스타 x 4, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{6, 6, 6, 6}, 4, 44, 10),         //81 스타 x 4, 하
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{6, 6, 6, 101}, 4, 44, 10),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{6, 6, 6, 101}, 4, 44, 10),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{6, 6, 6, 101}, 4, 44, 10),       //84 스타 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{101, 6, 6, 6}, 4, 44, 10),       //82 스타 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{101, 6, 6, 6}, 4, 44, 10),       //83 스타 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{101, 6, 6, 6}, 4, 44, 10),       //84 스타 x 3, 조커 X 1, 하
                

                new ALDScoreTableInfo( 700 * 1, 0, new int[]{6, 6, 6, 11}, 3, 43, 1),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 1, 1, new int[]{6, 6, 6, 11}, 3, 43, 1),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 1, 2, new int[]{6, 6, 6, 11}, 3, 43, 1),          //93 스타 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 1, 0, new int[]{11, 6, 6, 6}, 3, 43, 1),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 1, 1, new int[]{11, 6, 6, 6}, 3, 43, 1),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 1, 2, new int[]{11, 6, 6, 6}, 3, 43, 1),          //93 스타 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 2, 0, new int[]{6, 6, 6, 11}, 3, 43, 2),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 2, 1, new int[]{6, 6, 6, 11}, 3, 43, 2),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 2, 2, new int[]{6, 6, 6, 11}, 3, 43, 2),          //93 스타 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 2, 0, new int[]{11, 6, 6, 6}, 3, 43, 2),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 2, 1, new int[]{11, 6, 6, 6}, 3, 43, 2),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 2, 2, new int[]{11, 6, 6, 6}, 3, 43, 2),          //93 스타 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 3, 0, new int[]{6, 6, 6, 11}, 3, 43, 3),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 3, 1, new int[]{6, 6, 6, 11}, 3, 43, 3),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 3, 2, new int[]{6, 6, 6, 11}, 3, 43, 3),          //93 스타 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 3, 0, new int[]{11, 6, 6, 6}, 3, 43, 3),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 3, 1, new int[]{11, 6, 6, 6}, 3, 43, 3),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 3, 2, new int[]{11, 6, 6, 6}, 3, 43, 3),          //93 스타 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 5, 0, new int[]{6, 6, 6, 11}, 3, 43, 5),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 5, 1, new int[]{6, 6, 6, 11}, 3, 43, 5),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 5, 2, new int[]{6, 6, 6, 11}, 3, 43, 5),          //93 스타 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 5, 0, new int[]{11, 6, 6, 6}, 3, 43, 5),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 5, 1, new int[]{11, 6, 6, 6}, 3, 43, 5),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 5, 2, new int[]{11, 6, 6, 6}, 3, 43, 5),          //93 스타 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 10, 0, new int[]{6, 6, 6, 11}, 3, 43, 10),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 10, 1, new int[]{6, 6, 6, 11}, 3, 43, 10),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 10, 2, new int[]{6, 6, 6, 11}, 3, 43, 10),          //93 스타 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 10, 0, new int[]{11, 6, 6, 6}, 3, 43, 10),          //91 스타 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 10, 1, new int[]{11, 6, 6, 6}, 3, 43, 10),          //92 스타 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 10, 2, new int[]{11, 6, 6, 6}, 3, 43, 10),          //93 스타 x 3, 체리 X 1, 하



                new ALDScoreTableInfo( 600 * 1, 0, new int[]{6, 6, 6, 0}, 3, 43, 1),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{6, 6, 6, 0}, 3, 43, 1),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{6, 6, 6, 0}, 3, 43, 1),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{0, 6, 6, 6}, 3, 43, 1),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{0, 6, 6, 6}, 3, 43, 1),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{0, 6, 6, 6}, 3, 43, 1),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{6, 6, 0, 6}, 3, 43, 1),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{6, 6, 0, 6}, 3, 43, 1),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{6, 6, 0, 6}, 3, 43, 1),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{6, 0, 6, 6}, 3, 43, 1),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{6, 0, 6, 6}, 3, 43, 1),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{6, 0, 6, 6}, 3, 43, 1),            //96 스타 x 3, 하

                new ALDScoreTableInfo( 600 * 2, 0, new int[]{6, 6, 6, 0}, 3, 43, 2),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{6, 6, 6, 0}, 3, 43, 2),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{6, 6, 6, 0}, 3, 43, 2),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{0, 6, 6, 6}, 3, 43, 2),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{0, 6, 6, 6}, 3, 43, 2),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{0, 6, 6, 6}, 3, 43, 2),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{6, 6, 0, 6}, 3, 43, 2),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{6, 6, 0, 6}, 3, 43, 2),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{6, 6, 0, 6}, 3, 43, 2),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{6, 0, 6, 6}, 3, 43, 2),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{6, 0, 6, 6}, 3, 43, 2),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{6, 0, 6, 6}, 3, 43, 2),            //96 스타 x 3, 하

                new ALDScoreTableInfo( 600 * 3, 0, new int[]{6, 6, 6, 0}, 3, 43, 3),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{6, 6, 6, 0}, 3, 43, 3),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{6, 6, 6, 0}, 3, 43, 3),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{0, 6, 6, 6}, 3, 43, 3),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{0, 6, 6, 6}, 3, 43, 3),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{0, 6, 6, 6}, 3, 43, 3),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{6, 6, 0, 6}, 3, 43, 3),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{6, 6, 0, 6}, 3, 43, 3),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{6, 6, 0, 6}, 3, 43, 3),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{6, 0, 6, 6}, 3, 43, 3),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{6, 0, 6, 6}, 3, 43, 3),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{6, 0, 6, 6}, 3, 43, 3),            //96 스타 x 3, 하

                new ALDScoreTableInfo( 600 * 5, 0, new int[]{6, 6, 6, 0}, 3, 43, 5),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{6, 6, 6, 0}, 3, 43, 5),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{6, 6, 6, 0}, 3, 43, 5),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{0, 6, 6, 6}, 3, 43, 5),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{0, 6, 6, 6}, 3, 43, 5),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{0, 6, 6, 6}, 3, 43, 5),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{6, 6, 0, 6}, 3, 43, 5),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{6, 6, 0, 6}, 3, 43, 5),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{6, 6, 0, 6}, 3, 43, 5),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{6, 0, 6, 6}, 3, 43, 5),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{6, 0, 6, 6}, 3, 43, 5),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{6, 0, 6, 6}, 3, 43, 5),            //96 스타 x 3, 하

                new ALDScoreTableInfo( 600 * 10, 0, new int[]{6, 6, 6, 0}, 3, 43, 10),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{6, 6, 6, 0}, 3, 43, 10),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{6, 6, 6, 0}, 3, 43, 10),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{0, 6, 6, 6}, 3, 43, 10),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{0, 6, 6, 6}, 3, 43, 10),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{0, 6, 6, 6}, 3, 43, 10),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{6, 6, 0, 6}, 3, 43, 10),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{6, 6, 0, 6}, 3, 43, 10),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{6, 6, 0, 6}, 3, 43, 10),            //96 스타 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{6, 0, 6, 6}, 3, 43, 10),            //94 스타 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{6, 0, 6, 6}, 3, 43, 10),            //95 스타 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{6, 0, 6, 6}, 3, 43, 10),            //96 스타 x 3, 하


                new ALDScoreTableInfo( 100 * 1, 0, new int[]{6, 6, 0, 0}, 2, 42, 1),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{6, 6, 0, 0}, 2, 42, 1),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{6, 6, 0, 0}, 2, 42, 1),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{0, 0, 6, 6}, 2, 42, 1),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{0, 0, 6, 6}, 2, 42, 1),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{0, 0, 6, 6}, 2, 42, 1),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{6, 0, 0, 6}, 2, 42, 1),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{6, 0, 0, 6}, 2, 42, 1),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{6, 0, 0, 6}, 2, 42, 1),            //99 스타 x 2, 하

                new ALDScoreTableInfo( 100 * 2, 0, new int[]{6, 6, 0, 0}, 2, 42, 2),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{6, 6, 0, 0}, 2, 42, 2),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{6, 6, 0, 0}, 2, 42, 2),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 2, 0, new int[]{0, 0, 6, 6}, 2, 42, 2),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{0, 0, 6, 6}, 2, 42, 2),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{0, 0, 6, 6}, 2, 42, 2),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 2, 0, new int[]{6, 0, 0, 6}, 2, 42, 2),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{6, 0, 0, 6}, 2, 42, 2),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{6, 0, 0, 6}, 2, 42, 2),            //99 스타 x 2, 하

                new ALDScoreTableInfo( 100 * 3, 0, new int[]{6, 6, 0, 0}, 2, 42, 3),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{6, 6, 0, 0}, 2, 42, 3),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{6, 6, 0, 0}, 2, 42, 3),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 3, 0, new int[]{0, 0, 6, 6}, 2, 42, 3),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{0, 0, 6, 6}, 2, 42, 3),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{0, 0, 6, 6}, 2, 42, 3),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 3, 0, new int[]{6, 0, 0, 6}, 2, 42, 3),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{6, 0, 0, 6}, 2, 42, 3),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{6, 0, 0, 6}, 2, 42, 3),            //99 스타 x 2, 하

                new ALDScoreTableInfo( 100 * 5, 0, new int[]{6, 6, 0, 0}, 2, 42, 5),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{6, 6, 0, 0}, 2, 42, 5),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{6, 6, 0, 0}, 2, 42, 5),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 5, 0, new int[]{0, 0, 6, 6}, 2, 42, 5),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{0, 0, 6, 6}, 2, 42, 5),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{0, 0, 6, 6}, 2, 42, 5),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 5, 0, new int[]{6, 0, 0, 6}, 2, 42, 5),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{6, 0, 0, 6}, 2, 42, 5),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{6, 0, 0, 6}, 2, 42, 5),            //99 스타 x 2, 하

                new ALDScoreTableInfo( 100 * 10, 0, new int[]{6, 6, 0, 0}, 2, 42, 10),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{6, 6, 0, 0}, 2, 42, 10),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{6, 6, 0, 0}, 2, 42, 10),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 10, 0, new int[]{0, 0, 6, 6}, 2, 42, 10),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{0, 0, 6, 6}, 2, 42, 10),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{0, 0, 6, 6}, 2, 42, 10),            //99 스타 x 2, 하
                new ALDScoreTableInfo( 100 * 10, 0, new int[]{6, 0, 0, 6}, 2, 42, 10),            //97 스타 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{6, 0, 0, 6}, 2, 42, 10),            //98 스타 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{6, 0, 0, 6}, 2, 42, 10),            //99 스타 x 2, 하


                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{7, 7, 7, 7}, 4, 54, 1),         //100 타겟 x 4, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{7, 7, 7, 7}, 4, 54, 1),         //101 타겟 x 4, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{7, 7, 7, 7}, 4, 54, 1),         //102 타겟 x 4, 하
                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{7, 7, 7, 101}, 4, 54, 1),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{7, 7, 7, 101}, 4, 54, 1),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{7, 7, 7, 101}, 4, 54, 1),       //105 타겟 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 1, 0, new int[]{101, 7, 7, 7}, 4, 54, 1),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 1, 1, new int[]{101, 7, 7, 7}, 4, 54, 1),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 1, 2, new int[]{101, 7, 7, 7}, 4, 54, 1),       //105 타겟 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{7, 7, 7, 7}, 4, 54, 2),         //100 타겟 x 4, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{7, 7, 7, 7}, 4, 54, 2),         //101 타겟 x 4, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{7, 7, 7, 7}, 4, 54, 2),         //102 타겟 x 4, 하
                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{7, 7, 7, 101}, 4, 54, 2),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{7, 7, 7, 101}, 4, 54, 2),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{7, 7, 7, 101}, 4, 54, 2),       //105 타겟 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 2, 0, new int[]{101, 7, 7, 7}, 4, 54, 2),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 2, 1, new int[]{101, 7, 7, 7}, 4, 54, 2),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 2, 2, new int[]{101, 7, 7, 7}, 4, 54, 2),       //105 타겟 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{7, 7, 7, 7}, 4, 54, 3),         //100 타겟 x 4, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{7, 7, 7, 7}, 4, 54, 3),         //101 타겟 x 4, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{7, 7, 7, 7}, 4, 54, 3),         //102 타겟 x 4, 하
                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{7, 7, 7, 101}, 4, 54, 3),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{7, 7, 7, 101}, 4, 54, 3),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{7, 7, 7, 101}, 4, 54, 3),       //105 타겟 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 3, 0, new int[]{101, 7, 7, 7}, 4, 54, 3),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 3, 1, new int[]{101, 7, 7, 7}, 4, 54, 3),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 3, 2, new int[]{101, 7, 7, 7}, 4, 54, 3),       //105 타겟 x 3, 조커 X 1, 하

                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{7, 7, 7, 7}, 4, 54, 5),         //100 타겟 x 4, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{7, 7, 7, 7}, 4, 54, 5),         //101 타겟 x 4, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{7, 7, 7, 7}, 4, 54, 5),         //102 타겟 x 4, 하
                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{7, 7, 7, 101}, 4, 54, 5),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{7, 7, 7, 101}, 4, 54, 5),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{7, 7, 7, 101}, 4, 54, 5),       //105 타겟 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 5, 0, new int[]{101, 7, 7, 7}, 4, 54, 5),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 5, 1, new int[]{101, 7, 7, 7}, 4, 54, 5),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 5, 2, new int[]{101, 7, 7, 7}, 4, 54, 5),       //105 타겟 x 3, 조커 X 1, 하
            
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{7, 7, 7, 7}, 4, 54, 10),         //100 타겟 x 4, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{7, 7, 7, 7}, 4, 54, 10),         //101 타겟 x 4, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{7, 7, 7, 7}, 4, 54, 10),         //102 타겟 x 4, 하
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{7, 7, 7, 101}, 4, 54, 10),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{7, 7, 7, 101}, 4, 54, 10),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{7, 7, 7, 101}, 4, 54, 10),       //105 타겟 x 3, 조커 X 1, 하
                new ALDScoreTableInfo( 12000 * 10, 0, new int[]{101, 7, 7, 7}, 4, 54, 10),       //103 타겟 x 3, 조커 X 1, 상
                new ALDScoreTableInfo( 24000 * 10, 1, new int[]{101, 7, 7, 7}, 4, 54, 10),       //104 타겟 x 3, 조커 X 1, 중
                new ALDScoreTableInfo( 12000 * 10, 2, new int[]{101, 7, 7, 7}, 4, 54, 10),       //105 타겟 x 3, 조커 X 1, 하
               




                new ALDScoreTableInfo( 700 * 1, 0, new int[]{7, 7, 7, 11}, 3, 53, 1),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 1, 1, new int[]{7, 7, 7, 11}, 3, 53, 1),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 1, 2, new int[]{7, 7, 7, 11}, 3, 53, 1),          //114 타겟 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 1, 0, new int[]{11, 7, 7, 7}, 3, 53, 1),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 1, 1, new int[]{11, 7, 7, 7}, 3, 53, 1),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 1, 2, new int[]{11, 7, 7, 7}, 3, 53, 1),          //114 타겟 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 2, 0, new int[]{7, 7, 7, 11}, 3, 53, 2),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 2, 1, new int[]{7, 7, 7, 11}, 3, 53, 2),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 2, 2, new int[]{7, 7, 7, 11}, 3, 53, 2),          //114 타겟 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 2, 0, new int[]{11, 7, 7, 7}, 3, 53, 2),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 2, 1, new int[]{11, 7, 7, 7}, 3, 53, 2),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 2, 2, new int[]{11, 7, 7, 7}, 3, 53, 2),          //114 타겟 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 3, 0, new int[]{7, 7, 7, 11}, 3, 53, 3),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 3, 1, new int[]{7, 7, 7, 11}, 3, 53, 3),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 3, 2, new int[]{7, 7, 7, 11}, 3, 53, 3),          //114 타겟 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 3, 0, new int[]{11, 7, 7, 7}, 3, 53, 3),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 3, 1, new int[]{11, 7, 7, 7}, 3, 53, 3),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 3, 2, new int[]{11, 7, 7, 7}, 3, 53, 3),          //114 타겟 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 5, 0, new int[]{7, 7, 7, 11}, 3, 53, 5),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 5, 1, new int[]{7, 7, 7, 11}, 3, 53, 5),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 5, 2, new int[]{7, 7, 7, 11}, 3, 53, 5),          //114 타겟 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 5, 0, new int[]{11, 7, 7, 7}, 3, 53, 5),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 5, 1, new int[]{11, 7, 7, 7}, 3, 53, 5),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 5, 2, new int[]{11, 7, 7, 7}, 3, 53, 5),          //114 타겟 x 3, 체리 X 1, 하

                new ALDScoreTableInfo( 700 * 10, 0, new int[]{7, 7, 7, 11}, 3, 53, 10),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 10, 1, new int[]{7, 7, 7, 11}, 3, 53, 10),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 10, 2, new int[]{7, 7, 7, 11}, 3, 53, 10),          //114 타겟 x 3, 체리 X 1, 하
                new ALDScoreTableInfo( 700 * 10, 0, new int[]{11, 7, 7, 7}, 3, 53, 10),          //112 타겟 x 3, 체리 X 1, 상
                new ALDScoreTableInfo( 700 * 10, 1, new int[]{11, 7, 7, 7}, 3, 53, 10),          //113 타겟 x 3, 체리 X 1, 중
                new ALDScoreTableInfo( 700 * 10, 2, new int[]{11, 7, 7, 7}, 3, 53, 10),          //114 타겟 x 3, 체리 X 1, 하



                new ALDScoreTableInfo( 600 * 1, 0, new int[]{7, 7, 7, 0}, 3, 53, 1),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{7, 7, 7, 0}, 3, 53, 1),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{7, 7, 7, 0}, 3, 53, 1),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{0, 7, 7, 7}, 3, 53, 1),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{0, 7, 7, 7}, 3, 53, 1),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{0, 7, 7, 7}, 3, 53, 1),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{7, 7, 0, 7}, 3, 53, 1),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{7, 7, 0, 7}, 3, 53, 1),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{7, 7, 0, 7}, 3, 53, 1),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 1, 0, new int[]{7, 0, 7, 7}, 3, 53, 1),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 1, 1, new int[]{7, 0, 7, 7}, 3, 53, 1),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 1, 2, new int[]{7, 0, 7, 7}, 3, 53, 1),            //117 타겟 x 3, 하

                new ALDScoreTableInfo( 600 * 2, 0, new int[]{7, 7, 7, 0}, 3, 53, 2),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{7, 7, 7, 0}, 3, 53, 2),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{7, 7, 7, 0}, 3, 53, 2),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{0, 7, 7, 7}, 3, 53, 2),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{0, 7, 7, 7}, 3, 53, 2),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{0, 7, 7, 7}, 3, 53, 2),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{7, 7, 0, 7}, 3, 53, 2),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{7, 7, 0, 7}, 3, 53, 2),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{7, 7, 0, 7}, 3, 53, 2),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 2, 0, new int[]{7, 0, 7, 7}, 3, 53, 2),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 2, 1, new int[]{7, 0, 7, 7}, 3, 53, 2),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 2, 2, new int[]{7, 0, 7, 7}, 3, 53, 2),            //117 타겟 x 3, 하

                new ALDScoreTableInfo( 600 * 3, 0, new int[]{7, 7, 7, 0}, 3, 53, 3),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{7, 7, 7, 0}, 3, 53, 3),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{7, 7, 7, 0}, 3, 53, 3),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{0, 7, 7, 7}, 3, 53, 3),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{0, 7, 7, 7}, 3, 53, 3),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{0, 7, 7, 7}, 3, 53, 3),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{7, 7, 0, 7}, 3, 53, 3),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{7, 7, 0, 7}, 3, 53, 3),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{7, 7, 0, 7}, 3, 53, 3),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 3, 0, new int[]{7, 0, 7, 7}, 3, 53, 3),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 3, 1, new int[]{7, 0, 7, 7}, 3, 53, 3),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 3, 2, new int[]{7, 0, 7, 7}, 3, 53, 3),            //117 타겟 x 3, 하

                new ALDScoreTableInfo( 600 * 5, 0, new int[]{7, 7, 7, 0}, 3, 53, 5),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{7, 7, 7, 0}, 3, 53, 5),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{7, 7, 7, 0}, 3, 53, 5),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{0, 7, 7, 7}, 3, 53, 5),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{0, 7, 7, 7}, 3, 53, 5),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{0, 7, 7, 7}, 3, 53, 5),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{7, 7, 0, 7}, 3, 53, 5),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{7, 7, 0, 7}, 3, 53, 5),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{7, 7, 0, 7}, 3, 53, 5),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 5, 0, new int[]{7, 0, 7, 7}, 3, 53, 5),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 5, 1, new int[]{7, 0, 7, 7}, 3, 53, 5),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 5, 2, new int[]{7, 0, 7, 7}, 3, 53, 5),            //117 타겟 x 3, 하

                new ALDScoreTableInfo( 600 * 10, 0, new int[]{7, 7, 7, 0}, 3, 53, 10),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{7, 7, 7, 0}, 3, 53, 10),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{7, 7, 7, 0}, 3, 53, 10),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{0, 7, 7, 7}, 3, 53, 10),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{0, 7, 7, 7}, 3, 53, 10),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{0, 7, 7, 7}, 3, 53, 10),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{7, 7, 0, 7}, 3, 53, 10),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{7, 7, 0, 7}, 3, 53, 10),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{7, 7, 0, 7}, 3, 53, 10),            //117 타겟 x 3, 하
                new ALDScoreTableInfo( 600 * 10, 0, new int[]{7, 0, 7, 7}, 3, 53, 10),            //115 타겟 x 3, 상
                new ALDScoreTableInfo( 600 * 10, 1, new int[]{7, 0, 7, 7}, 3, 53, 10),            //116 타겟 x 3, 중
                new ALDScoreTableInfo( 600 * 10, 2, new int[]{7, 0, 7, 7}, 3, 53, 10),            //117 타겟 x 3, 하



                new ALDScoreTableInfo( 100 * 1, 0, new int[]{7, 7, 0, 0}, 2, 52, 1),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{7, 7, 0, 0}, 2, 52, 1),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{7, 7, 0, 0}, 2, 52, 1),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{0, 0, 7, 7}, 2, 52, 1),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{0, 0, 7, 7}, 2, 52, 1),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{0, 0, 7, 7}, 2, 52, 1),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{7, 0, 0, 7}, 2, 52, 1),            //120 타겟 x 2, 하

                new ALDScoreTableInfo( 100 * 2, 0, new int[]{7, 7, 0, 0}, 2, 52, 2),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{7, 7, 0, 0}, 2, 52, 2),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{7, 7, 0, 0}, 2, 52, 2),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 2, 0, new int[]{0, 0, 7, 7}, 2, 52, 2),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{0, 0, 7, 7}, 2, 52, 2),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{0, 0, 7, 7}, 2, 52, 2),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{7, 0, 0, 7}, 2, 52, 2),            //120 타겟 x 2, 하

                new ALDScoreTableInfo( 100 * 3, 0, new int[]{7, 7, 0, 0}, 2, 52, 3),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{7, 7, 0, 0}, 2, 52, 3),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{7, 7, 0, 0}, 2, 52, 3),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 3, 0, new int[]{0, 0, 7, 7}, 2, 52, 3),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{0, 0, 7, 7}, 2, 52, 3),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{0, 0, 7, 7}, 2, 52, 3),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{0, 0, 7, 7}, 2, 52, 3),            //120 타겟 x 2, 하

                new ALDScoreTableInfo( 100 * 5, 0, new int[]{7, 7, 0, 0}, 2, 52, 5),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{7, 7, 0, 0}, 2, 52, 5),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{7, 7, 0, 0}, 2, 52, 5),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 5, 0, new int[]{0, 0, 7, 7}, 2, 52, 5),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{0, 0, 7, 7}, 2, 52, 5),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{7, 0, 0, 7}, 2, 52, 5),            //120 타겟 x 2, 하

                new ALDScoreTableInfo( 100 * 10, 0, new int[]{7, 7, 0, 0}, 2, 52, 10),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{7, 7, 0, 0}, 2, 52, 10),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{7, 7, 0, 0}, 2, 52, 10),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 10, 0, new int[]{0, 0, 7, 7}, 2, 52, 10),            //118 타겟 x 2, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{0, 0, 7, 7}, 2, 52, 10),            //119 타겟 x 2, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{0, 0, 7, 7}, 2, 52, 10),            //120 타겟 x 2, 하
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{7, 0, 0, 7}, 2, 52, 10),            //120 타겟 x 2, 하



                 
                new ALDScoreTableInfo( 2000 * 1, 0, new int[]{8, 8, 8, 8}, 4, 64, 1),          //121 황벨 x 4, 상
                new ALDScoreTableInfo( 2000 * 1, 1, new int[]{8, 8, 8, 8}, 4, 64, 1),          //122 황벨 x 4, 중
                new ALDScoreTableInfo( 2000 * 1, 2, new int[]{8, 8, 8, 8}, 4, 64, 1),          //123 황벨 x 4, 하

                new ALDScoreTableInfo( 2000 * 2, 0, new int[]{8, 8, 8, 8}, 4, 64, 2),          //121 황벨 x 4, 상
                new ALDScoreTableInfo( 2000 * 2, 1, new int[]{8, 8, 8, 8}, 4, 64, 2),          //122 황벨 x 4, 중
                new ALDScoreTableInfo( 2000 * 2, 2, new int[]{8, 8, 8, 8}, 4, 64, 2),          //123 황벨 x 4, 하

                new ALDScoreTableInfo( 2000 * 3, 0, new int[]{8, 8, 8, 8}, 4, 64, 3),          //121 황벨 x 4, 상
                new ALDScoreTableInfo( 2000 * 3, 1, new int[]{8, 8, 8, 8}, 4, 64, 3),          //122 황벨 x 4, 중
                new ALDScoreTableInfo( 2000 * 3, 2, new int[]{8, 8, 8, 8}, 4, 64, 3),          //123 황벨 x 4, 하

                new ALDScoreTableInfo( 2000 * 5, 0, new int[]{8, 8, 8, 8}, 4, 64, 5),          //121 황벨 x 4, 상
                new ALDScoreTableInfo( 2000 * 5, 1, new int[]{8, 8, 8, 8}, 4, 64, 5),          //122 황벨 x 4, 중
                new ALDScoreTableInfo( 2000 * 5, 2, new int[]{8, 8, 8, 8}, 4, 64, 5),          //123 황벨 x 4, 하

                new ALDScoreTableInfo( 2000 * 10, 0, new int[]{8, 8, 8, 8}, 4, 64, 10),          //121 황벨 x 4, 상
                new ALDScoreTableInfo( 2000 * 10, 1, new int[]{8, 8, 8, 8}, 4, 64, 10),          //122 황벨 x 4, 중
                new ALDScoreTableInfo( 2000 * 10, 2, new int[]{8, 8, 8, 8}, 4, 64, 10),          //123 황벨 x 4, 하



                new ALDScoreTableInfo( 400 * 1, 0, new int[]{8, 8, 8, 0}, 3, 63, 1),            //124 황벨 x 3, 상
                new ALDScoreTableInfo( 400 * 1, 1, new int[]{8, 8, 8, 0}, 3, 63, 1),            //125 황벨 x 3, 중
                new ALDScoreTableInfo( 400 * 1, 2, new int[]{8, 8, 8, 0}, 3, 63, 1),            //126 황벨 x 3, 하

                new ALDScoreTableInfo( 400 * 2, 0, new int[]{8, 8, 8, 0}, 3, 63, 2),            //124 황벨 x 3, 상
                new ALDScoreTableInfo( 400 * 2, 1, new int[]{8, 8, 8, 0}, 3, 63, 2),            //125 황벨 x 3, 중
                new ALDScoreTableInfo( 400 * 2, 2, new int[]{8, 8, 8, 0}, 3, 63, 2),            //126 황벨 x 3, 하

                new ALDScoreTableInfo( 400 * 3, 0, new int[]{8, 8, 8, 0}, 3, 63, 3),            //124 황벨 x 3, 상
                new ALDScoreTableInfo( 400 * 3, 1, new int[]{8, 8, 8, 0}, 3, 63, 3),            //125 황벨 x 3, 중
                new ALDScoreTableInfo( 400 * 3, 2, new int[]{8, 8, 8, 0}, 3, 63, 3),            //126 황벨 x 3, 하

                new ALDScoreTableInfo( 400 * 5, 0, new int[]{8, 8, 8, 0}, 3, 63, 5),            //124 황벨 x 3, 상
                new ALDScoreTableInfo( 400 * 5, 1, new int[]{8, 8, 8, 0}, 3, 63, 5),            //125 황벨 x 3, 중
                new ALDScoreTableInfo( 400 * 5, 2, new int[]{8, 8, 8, 0}, 3, 63, 5),            //126 황벨 x 3, 하

                new ALDScoreTableInfo( 400 * 10, 0, new int[]{8, 8, 8, 0}, 3, 63, 10),            //124 황벨 x 3, 상
                new ALDScoreTableInfo( 400 * 10, 1, new int[]{8, 8, 8, 0}, 3, 63, 10),            //125 황벨 x 3, 중
                new ALDScoreTableInfo( 400 * 10, 2, new int[]{8, 8, 8, 0}, 3, 63, 10),            //126 황벨 x 3, 하


                new ALDScoreTableInfo( 1000 * 1, 0, new int[]{9, 9, 9, 9}, 4, 74, 1),          //127 수박 x 4, 상
                new ALDScoreTableInfo( 1000 * 1, 1, new int[]{9, 9, 9, 9}, 4, 74, 1),          //128 수박 x 4, 중
                new ALDScoreTableInfo( 1000 * 1, 2, new int[]{9, 9, 9, 9}, 4, 74, 1),          //129 수박 x 4, 하

                new ALDScoreTableInfo( 1000 * 2, 0, new int[]{9, 9, 9, 9}, 4, 74, 2),          //127 수박 x 4, 상
                new ALDScoreTableInfo( 1000 * 2, 1, new int[]{9, 9, 9, 9}, 4, 74, 2),          //128 수박 x 4, 중
                new ALDScoreTableInfo( 1000 * 2, 2, new int[]{9, 9, 9, 9}, 4, 74, 2),          //129 수박 x 4, 하

                new ALDScoreTableInfo( 1000 * 3, 0, new int[]{9, 9, 9, 9}, 4, 74, 3),          //127 수박 x 4, 상
                new ALDScoreTableInfo( 1000 * 3, 1, new int[]{9, 9, 9, 9}, 4, 74, 3),          //128 수박 x 4, 중
                new ALDScoreTableInfo( 1000 * 3, 2, new int[]{9, 9, 9, 9}, 4, 74, 3),          //129 수박 x 4, 하

                new ALDScoreTableInfo( 1000 * 5, 0, new int[]{9, 9, 9, 9}, 4, 74, 5),          //127 수박 x 4, 상
                new ALDScoreTableInfo( 1000 * 5, 1, new int[]{9, 9, 9, 9}, 4, 74, 5),          //128 수박 x 4, 중
                new ALDScoreTableInfo( 1000 * 5, 2, new int[]{9, 9, 9, 9}, 4, 74, 5),          //129 수박 x 4, 하

                new ALDScoreTableInfo( 1000 * 10, 0, new int[]{9, 9, 9, 9}, 4, 74, 10),          //127 수박 x 4, 상
                new ALDScoreTableInfo( 1000 * 10, 1, new int[]{9, 9, 9, 9}, 4, 74, 10),          //128 수박 x 4, 중
                new ALDScoreTableInfo( 1000 * 10, 2, new int[]{9, 9, 9, 9}, 4, 74, 10),          //129 수박 x 4, 하



                new ALDScoreTableInfo( 400 * 1, 0, new int[]{9, 9, 9, 0}, 3, 73, 1),            //130 수박 x 3, 상
                new ALDScoreTableInfo( 400 * 1, 1, new int[]{9, 9, 9, 0}, 3, 73, 1),            //131 수박 x 3, 중
                new ALDScoreTableInfo( 400 * 1, 2, new int[]{9, 9, 9, 0}, 3, 73, 1),            //132 수박 x 3, 하

                new ALDScoreTableInfo( 400 * 2, 0, new int[]{9, 9, 9, 0}, 3, 73, 2),            //130 수박 x 3, 상
                new ALDScoreTableInfo( 400 * 2, 1, new int[]{9, 9, 9, 0}, 3, 73, 2),            //131 수박 x 3, 중
                new ALDScoreTableInfo( 400 * 2, 2, new int[]{9, 9, 9, 0}, 3, 73, 2),            //132 수박 x 3, 하

                new ALDScoreTableInfo( 400 * 3, 0, new int[]{9, 9, 9, 0}, 3, 73, 3),            //130 수박 x 3, 상
                new ALDScoreTableInfo( 400 * 3, 1, new int[]{9, 9, 9, 0}, 3, 73, 3),            //131 수박 x 3, 중
                new ALDScoreTableInfo( 400 * 3, 2, new int[]{9, 9, 9, 0}, 3, 73, 3),            //132 수박 x 3, 하

                new ALDScoreTableInfo( 400 * 5, 0, new int[]{9, 9, 9, 0}, 3, 73, 5),            //130 수박 x 3, 상
                new ALDScoreTableInfo( 400 * 5, 1, new int[]{9, 9, 9, 0}, 3, 73, 5),            //131 수박 x 3, 중
                new ALDScoreTableInfo( 400 * 5, 2, new int[]{9, 9, 9, 0}, 3, 73, 5),            //132 수박 x 3, 하

                new ALDScoreTableInfo( 400 * 10, 0, new int[]{9, 9, 9, 0}, 3, 73, 10),            //130 수박 x 3, 상
                new ALDScoreTableInfo( 400 * 10, 1, new int[]{9, 9, 9, 0}, 3, 73, 10),            //131 수박 x 3, 중
                new ALDScoreTableInfo( 400 * 10, 2, new int[]{9, 9, 9, 0}, 3, 73, 10),            //132 수박 x 3, 하




                new ALDScoreTableInfo( 1000 * 1, 0, new int[]{10, 10, 10, 10}, 4, 84, 1),      //133 레몬 x 4, 상
                new ALDScoreTableInfo( 1000 * 1, 1, new int[]{10, 10, 10, 10}, 4, 84, 1),      //134 레몬 x 4, 중
                new ALDScoreTableInfo( 1000 * 1, 2, new int[]{10, 10, 10, 10}, 4, 84, 1),      //135 레몬 x 4, 하

                new ALDScoreTableInfo( 1000 * 2, 0, new int[]{10, 10, 10, 10}, 4, 84, 2),      //133 레몬 x 4, 상
                new ALDScoreTableInfo( 1000 * 2, 1, new int[]{10, 10, 10, 10}, 4, 84, 2),      //134 레몬 x 4, 중
                new ALDScoreTableInfo( 1000 * 2, 2, new int[]{10, 10, 10, 10}, 4, 84, 2),      //135 레몬 x 4, 하

                new ALDScoreTableInfo( 1000 * 3, 0, new int[]{10, 10, 10, 10}, 4, 84, 3),      //133 레몬 x 4, 상
                new ALDScoreTableInfo( 1000 * 3, 1, new int[]{10, 10, 10, 10}, 4, 84, 3),      //134 레몬 x 4, 중
                new ALDScoreTableInfo( 1000 * 3, 2, new int[]{10, 10, 10, 10}, 4, 84, 3),      //135 레몬 x 4, 하

                new ALDScoreTableInfo( 1000 * 5, 0, new int[]{10, 10, 10, 10}, 4, 84, 5),      //133 레몬 x 4, 상
                new ALDScoreTableInfo( 1000 * 5, 1, new int[]{10, 10, 10, 10}, 4, 84, 5),      //134 레몬 x 4, 중
                new ALDScoreTableInfo( 1000 * 5, 2, new int[]{10, 10, 10, 10}, 4, 84, 5),      //135 레몬 x 4, 하

                new ALDScoreTableInfo( 1000 * 10, 0, new int[]{10, 10, 10, 10}, 4, 84, 10),      //133 레몬 x 4, 상
                new ALDScoreTableInfo( 1000 * 10, 1, new int[]{10, 10, 10, 10}, 4, 84, 10),      //134 레몬 x 4, 중
                new ALDScoreTableInfo( 1000 * 10, 2, new int[]{10, 10, 10, 10}, 4, 84, 10),      //135 레몬 x 4, 하



                new ALDScoreTableInfo( 400 * 1, 0, new int[]{10, 10, 10, 0}, 3, 83, 1),         //136 레몬 x 3, 상
                new ALDScoreTableInfo( 400 * 1, 1, new int[]{10, 10, 10, 0}, 3, 83, 1),         //137 레몬 x 3, 중
                new ALDScoreTableInfo( 400 * 1, 2, new int[]{10, 10, 10, 0}, 3, 83, 1),         //138 레몬 x 3, 하

                new ALDScoreTableInfo( 400 * 2, 0, new int[]{10, 10, 10, 0}, 3, 83, 2),         //136 레몬 x 3, 상
                new ALDScoreTableInfo( 400 * 2, 1, new int[]{10, 10, 10, 0}, 3, 83, 2),         //137 레몬 x 3, 중
                new ALDScoreTableInfo( 400 * 2, 2, new int[]{10, 10, 10, 0}, 3, 83, 2),         //138 레몬 x 3, 하

                new ALDScoreTableInfo( 400 * 3, 0, new int[]{10, 10, 10, 0}, 3, 83, 3),         //136 레몬 x 3, 상
                new ALDScoreTableInfo( 400 * 3, 1, new int[]{10, 10, 10, 0}, 3, 83, 3),         //137 레몬 x 3, 중
                new ALDScoreTableInfo( 400 * 3, 2, new int[]{10, 10, 10, 0}, 3, 83, 3),         //138 레몬 x 3, 하

                new ALDScoreTableInfo( 400 * 5, 0, new int[]{10, 10, 10, 0}, 3, 83, 5),         //136 레몬 x 3, 상
                new ALDScoreTableInfo( 400 * 5, 1, new int[]{10, 10, 10, 0}, 3, 83, 5),         //137 레몬 x 3, 중
                new ALDScoreTableInfo( 400 * 5, 2, new int[]{10, 10, 10, 0}, 3, 83, 5),         //138 레몬 x 3, 하

                new ALDScoreTableInfo( 400 * 10, 0, new int[]{10, 10, 10, 0}, 3, 83, 10),         //136 레몬 x 3, 상
                new ALDScoreTableInfo( 400 * 10, 1, new int[]{10, 10, 10, 0}, 3, 83, 10),         //137 레몬 x 3, 중
                new ALDScoreTableInfo( 400 * 10, 2, new int[]{10, 10, 10, 0}, 3, 83, 10),         //138 레몬 x 3, 하




                new ALDScoreTableInfo( 1000 * 1, 0, new int[]{11, 11, 11, 11}, 4, 94, 1),      //139 체리 x 4, 상
                new ALDScoreTableInfo( 1000 * 1, 1, new int[]{11, 11, 11, 11}, 4, 94, 1),      //140 체리 x 4, 중
                new ALDScoreTableInfo( 1000 * 1, 2, new int[]{11, 11, 11, 11}, 4, 94, 1),      //141 체리 x 4, 하

                new ALDScoreTableInfo( 1000 * 2, 0, new int[]{11, 11, 11, 11}, 4, 94, 2),      //139 체리 x 4, 상
                new ALDScoreTableInfo( 1000 * 2, 1, new int[]{11, 11, 11, 11}, 4, 94, 2),      //140 체리 x 4, 중
                new ALDScoreTableInfo( 1000 * 2, 2, new int[]{11, 11, 11, 11}, 4, 94, 2),      //141 체리 x 4, 하

                new ALDScoreTableInfo( 1000 * 3, 0, new int[]{11, 11, 11, 11}, 4, 94, 3),      //139 체리 x 4, 상
                new ALDScoreTableInfo( 1000 * 3, 1, new int[]{11, 11, 11, 11}, 4, 94, 3),      //140 체리 x 4, 중
                new ALDScoreTableInfo( 1000 * 3, 2, new int[]{11, 11, 11, 11}, 4, 94, 3),      //141 체리 x 4, 하

                new ALDScoreTableInfo( 1000 * 5, 0, new int[]{11, 11, 11, 11}, 4, 94, 5),      //139 체리 x 4, 상
                new ALDScoreTableInfo( 1000 * 5, 1, new int[]{11, 11, 11, 11}, 4, 94, 5),      //140 체리 x 4, 중
                new ALDScoreTableInfo( 1000 * 5, 2, new int[]{11, 11, 11, 11}, 4, 94, 5),      //141 체리 x 4, 하

                new ALDScoreTableInfo( 1000 * 10, 0, new int[]{11, 11, 11, 11}, 4, 94, 10),      //139 체리 x 4, 상
                new ALDScoreTableInfo( 1000 * 10, 1, new int[]{11, 11, 11, 11}, 4, 94, 10),      //140 체리 x 4, 중
                new ALDScoreTableInfo( 1000 * 10, 2, new int[]{11, 11, 11, 11}, 4, 94, 10),      //141 체리 x 4, 하



                new ALDScoreTableInfo( 400 * 1, 0, new int[]{11, 11, 11, 0}, 3, 93, 1),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 1, 1, new int[]{11, 11, 11, 0}, 3, 93, 1),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 1, 2, new int[]{11, 11, 11, 0}, 3, 93, 1),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 400 * 1, 0, new int[]{0, 11, 11, 11}, 3, 93, 1),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 1, 1, new int[]{0, 11, 11, 11}, 3, 93, 1),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 1, 2, new int[]{0, 11, 11, 11}, 3, 93, 1),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 400 * 2, 0, new int[]{11, 11, 11, 0}, 3, 93, 2),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 2, 1, new int[]{11, 11, 11, 0}, 3, 93, 2),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 2, 2, new int[]{11, 11, 11, 0}, 3, 93, 2),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 400 * 2, 0, new int[]{0, 11, 11, 11}, 3, 93, 2),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 2, 1, new int[]{0, 11, 11, 11}, 3, 93, 2),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 2, 2, new int[]{0, 11, 11, 11}, 3, 93, 2),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 400 * 3, 0, new int[]{11, 11, 11, 0}, 3, 93, 3),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 3, 1, new int[]{11, 11, 11, 0}, 3, 93, 3),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 3, 2, new int[]{11, 11, 11, 0}, 3, 93, 3),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 400 * 3, 0, new int[]{0, 11, 11, 11}, 3, 93, 3),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 3, 1, new int[]{0, 11, 11, 11}, 3, 93, 3),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 3, 2, new int[]{0, 11, 11, 11}, 3, 93, 3),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 400 * 5, 0, new int[]{11, 11, 11, 0}, 3, 93, 5),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 5, 1, new int[]{11, 11, 11, 0}, 3, 93, 5),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 5, 2, new int[]{11, 11, 11, 0}, 3, 93, 5),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 400 * 5, 0, new int[]{0, 11, 11, 11}, 3, 93, 5),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 5, 1, new int[]{0, 11, 11, 11}, 3, 93, 5),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 5, 2, new int[]{0, 11, 11, 11}, 3, 93, 5),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 400 * 10, 0, new int[]{11, 11, 11, 0}, 3, 93, 10),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 10, 1, new int[]{11, 11, 11, 0}, 3, 93, 10),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 10, 2, new int[]{11, 11, 11, 0}, 3, 93, 10),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 400 * 10, 0, new int[]{0, 11, 11, 11}, 3, 93, 10),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 400 * 10, 1, new int[]{0, 11, 11, 11}, 3, 93, 10),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 400 * 10, 2, new int[]{0, 11, 11, 11}, 3, 93, 10),         //144 체리 x 3, 하


                new ALDScoreTableInfo( 300 * 1, 0, new int[]{11, 11, 0, 11}, 3, 93, 1),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 1, 1, new int[]{11, 11, 0, 11}, 3, 93, 1),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 1, 2, new int[]{11, 11, 0, 11}, 3, 93, 1),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 300 * 1, 0, new int[]{11, 0, 11, 11}, 3, 93, 1),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 1, 1, new int[]{11, 0, 11, 11}, 3, 93, 1),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 1, 2, new int[]{11, 0, 11, 11}, 3, 93, 1),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 300 * 2, 0, new int[]{11, 11, 0, 11}, 3, 93, 2),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 2, 1, new int[]{11, 11, 0, 11}, 3, 93, 2),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 2, 2, new int[]{11, 11, 0, 11}, 3, 93, 2),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 300 * 2, 0, new int[]{11, 0, 11, 11}, 3, 93, 2),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 2, 1, new int[]{11, 0, 11, 11}, 3, 93, 2),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 2, 2, new int[]{11, 0, 11, 11}, 3, 93, 2),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 300 * 3, 0, new int[]{11, 11, 0, 11}, 3, 93, 3),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 3, 1, new int[]{11, 11, 0, 11}, 3, 93, 3),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 3, 2, new int[]{11, 11, 0, 11}, 3, 93, 3),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 300 * 3, 0, new int[]{11, 0, 11, 11}, 3, 93, 3),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 3, 1, new int[]{11, 0, 11, 11}, 3, 93, 3),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 3, 2, new int[]{11, 0, 11, 11}, 3, 93, 3),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 300 * 5, 0, new int[]{11, 11, 0, 11}, 3, 93, 5),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 5, 1, new int[]{11, 11, 0, 11}, 3, 93, 5),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 5, 2, new int[]{11, 11, 0, 11}, 3, 93, 5),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 300 * 5, 0, new int[]{11, 0, 11, 11}, 3, 93, 5),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 5, 1, new int[]{11, 0, 11, 11}, 3, 93, 5),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 5, 2, new int[]{11, 0, 11, 11}, 3, 93, 5),         //144 체리 x 3, 하

                new ALDScoreTableInfo( 300 * 10, 0, new int[]{11, 11, 0, 11}, 3, 93, 10),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 10, 1, new int[]{11, 11, 0, 11}, 3, 93, 10),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 10, 2, new int[]{11, 11, 0, 11}, 3, 93, 10),         //144 체리 x 3, 하
                new ALDScoreTableInfo( 300 * 10, 0, new int[]{11, 0, 11, 11}, 3, 93, 10),         //142 체리 x 3, 상
                new ALDScoreTableInfo( 300 * 10, 1, new int[]{11, 0, 11, 11}, 3, 93, 10),         //143 체리 x 3, 중
                new ALDScoreTableInfo( 300 * 10, 2, new int[]{11, 0, 11, 11}, 3, 93, 10),         //144 체리 x 3, 하





                
                new ALDScoreTableInfo( 200 * 1, 0, new int[]{11, 11, 0, 0}, 2, 92, 1),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 1, 1, new int[]{11, 11, 0, 0}, 2, 92, 1),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 1, 2, new int[]{11, 11, 0, 0}, 2, 92, 1),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 1, 0, new int[]{0, 0, 11, 11}, 2, 92, 1),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 1, 1, new int[]{0, 0, 11, 11}, 2, 92, 1),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 1, 2, new int[]{0, 0, 11, 11}, 2, 92, 1),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 1, 0, new int[]{11, 0, 0, 11}, 2, 92, 1),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 1, 1, new int[]{11, 0, 0, 11}, 2, 92, 1),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 1, 2, new int[]{11, 0, 0, 11}, 2, 92, 1),          //147 체리 x 2, 하

                new ALDScoreTableInfo( 200 * 2, 0, new int[]{11, 11, 0, 0}, 2, 92, 2),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 2, 1, new int[]{11, 11, 0, 0}, 2, 92, 2),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 2, 2, new int[]{11, 11, 0, 0}, 2, 92, 2),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 2, 0, new int[]{0, 0, 11, 11}, 2, 92, 2),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 2, 1, new int[]{0, 0, 11, 11}, 2, 92, 2),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 2, 2, new int[]{0, 0, 11, 11}, 2, 92, 2),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 2, 0, new int[]{11, 0, 0, 11}, 2, 92, 2),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 2, 1, new int[]{11, 0, 0, 11}, 2, 92, 2),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 2, 2, new int[]{11, 0, 0, 11}, 2, 92, 2),          //147 체리 x 2, 하

                new ALDScoreTableInfo( 200 * 3, 0, new int[]{11, 11, 0, 0}, 2, 92, 3),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 3, 1, new int[]{11, 11, 0, 0}, 2, 92, 3),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 3, 2, new int[]{11, 11, 0, 0}, 2, 92, 3),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 3, 0, new int[]{0, 0, 11, 11}, 2, 92, 3),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 3, 1, new int[]{0, 0, 11, 11}, 2, 92, 3),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 3, 2, new int[]{0, 0, 11, 11}, 2, 92, 3),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 3, 0, new int[]{11, 0, 0, 11}, 2, 92, 3),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 3, 1, new int[]{11, 0, 0, 11}, 2, 92, 3),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 3, 2, new int[]{11, 0, 0, 11}, 2, 92, 3),          //147 체리 x 2, 하

                new ALDScoreTableInfo( 200 * 5, 0, new int[]{11, 11, 0, 0}, 2, 92, 5),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 5, 1, new int[]{11, 11, 0, 0}, 2, 92, 5),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 5, 2, new int[]{11, 11, 0, 0}, 2, 92, 5),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 5, 0, new int[]{0, 0, 11, 11}, 2, 92, 5),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 5, 1, new int[]{0, 0, 11, 11}, 2, 92, 5),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 5, 2, new int[]{0, 0, 11, 11}, 2, 92, 5),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 5, 0, new int[]{11, 0, 0, 11}, 2, 92, 5),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 5, 1, new int[]{11, 0, 0, 11}, 2, 92, 5),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 5, 2, new int[]{11, 0, 0, 11}, 2, 92, 5),          //147 체리 x 2, 하

                new ALDScoreTableInfo( 200 * 10, 0, new int[]{11, 11, 0, 0}, 2, 92, 10),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 10, 1, new int[]{11, 11, 0, 0}, 2, 92, 10),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 10, 2, new int[]{11, 11, 0, 0}, 2, 92, 10),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 10, 0, new int[]{0, 0, 11, 11}, 2, 92, 10),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 10, 1, new int[]{0, 0, 11, 11}, 2, 92, 10),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 10, 2, new int[]{0, 0, 11, 11}, 2, 92, 10),          //147 체리 x 2, 하
                new ALDScoreTableInfo( 200 * 10, 0, new int[]{11, 0, 0, 11}, 2, 92, 10),          //145 체리 x 2, 상
                new ALDScoreTableInfo( 200 * 10, 1, new int[]{11, 0, 0, 11}, 2, 92, 10),          //146 체리 x 2, 중
                new ALDScoreTableInfo( 200 * 10, 2, new int[]{11, 0, 0, 11}, 2, 92, 10),          //147 체리 x 2, 하


                
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{11, 0, 102, 0}, 1, 91, 1),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{11, 0, 102, 0}, 1, 91, 1),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{11, 0, 102, 0}, 1, 91, 1),           //150 체리 x 1, 하
                new ALDScoreTableInfo( 100 * 1, 0, new int[]{0, 102, 0, 11}, 1, 91, 1),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 1, 1, new int[]{0, 102, 0, 11}, 1, 91, 1),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 1, 2, new int[]{0, 102, 0, 11}, 1, 91, 1),           //150 체리 x 1, 하

                new ALDScoreTableInfo( 100 * 2, 0, new int[]{11, 0, 102, 0}, 1, 91, 2),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{11, 0, 102, 0}, 1, 91, 2),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{11, 0, 102, 0}, 1, 91, 2),           //150 체리 x 1, 하
                new ALDScoreTableInfo( 100 * 2, 0, new int[]{0, 102, 0, 11}, 1, 91, 2),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 2, 1, new int[]{0, 102, 0, 11}, 1, 91, 2),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 2, 2, new int[]{0, 102, 0, 11}, 1, 91, 2),           //150 체리 x 1, 하

                new ALDScoreTableInfo( 100 * 3, 0, new int[]{11, 0, 102, 0}, 1, 91, 3),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{11, 0, 102, 0}, 1, 91, 3),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{11, 0, 102, 0}, 1, 91, 3),           //150 체리 x 1, 하
                new ALDScoreTableInfo( 100 * 3, 0, new int[]{0, 102, 0, 11}, 1, 91, 3),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 3, 1, new int[]{0, 102, 0, 11}, 1, 91, 3),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 3, 2, new int[]{0, 102, 0, 11}, 1, 91, 3),           //150 체리 x 1, 하

                new ALDScoreTableInfo( 100 * 5, 0, new int[]{11, 0, 102, 0}, 1, 91, 5),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{11, 0, 102, 0}, 1, 91, 5),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{11, 0, 102, 0}, 1, 91, 5),           //150 체리 x 1, 하
                new ALDScoreTableInfo( 100 * 5, 0, new int[]{0, 102, 0, 11}, 1, 91, 5),           //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 5, 1, new int[]{0, 102, 0, 11}, 1, 91, 5),           //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 5, 2, new int[]{0, 102, 0, 11}, 1, 91, 5),           //150 체리 x 1, 하

                new ALDScoreTableInfo( 100 * 10, 0, new int[]{11, 0, 102, 0}, 1, 91, 10),         //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{11, 0, 102, 0}, 1, 91, 10),         //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{11, 0, 102, 0}, 1, 91, 10),         //150 체리 x 1, 하
                new ALDScoreTableInfo( 100 * 10, 0, new int[]{0, 102, 0, 11}, 1, 91, 10),         //148 체리 x 1, 상
                new ALDScoreTableInfo( 100 * 10, 1, new int[]{0, 102, 0, 11}, 1, 91, 10),         //149 체리 x 1, 중
                new ALDScoreTableInfo( 100 * 10, 2, new int[]{0, 102, 0, 11}, 1, 91, 10),         //150 체리 x 1, 하

                new ALDScoreTableInfo( 400 * 1,  0, new int[]{ 103, 103, 103, 102 }, 4, 103, 1),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 1,  1, new int[]{ 103, 103, 103, 102 }, 4, 103, 1),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 1,  2, new int[]{ 103, 103, 103, 102 }, 4, 103, 1),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 2,  0, new int[]{ 103, 103, 103, 102 }, 4, 103, 2),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 2,  1, new int[]{ 103, 103, 103, 102 }, 4, 103, 2),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 2,  2, new int[]{ 103, 103, 103, 102 }, 4, 103, 2),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 3,  0, new int[]{ 103, 103, 103, 102 }, 4, 103, 3),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 3,  1, new int[]{ 103, 103, 103, 102 }, 4, 103, 3),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 3,  2, new int[]{ 103, 103, 103, 102 }, 4, 103, 3),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 5,  0, new int[]{ 103, 103, 103, 102 }, 4, 103, 5),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 5,  1, new int[]{ 103, 103, 103, 102 }, 4, 103, 5),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 5,  2, new int[]{ 103, 103, 103, 102 }, 4, 103, 5),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 10,  0, new int[]{ 103, 103, 103, 102 }, 4, 103, 10),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 10,  1, new int[]{ 103, 103, 103, 102 }, 4, 103, 10),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 10,  2, new int[]{ 103, 103, 103, 102 }, 4, 103, 10),        //쓰리잡바 x 1, 하

                new ALDScoreTableInfo( 400 * 1,  0, new int[]{ 102, 103, 103, 103 }, 4, 103, 1),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 1,  1, new int[]{ 102, 103, 103, 103 }, 4, 103, 1),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 1,  2, new int[]{ 102, 103, 103, 103 }, 4, 103, 1),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 2,  0, new int[]{ 102, 103, 103, 103 }, 4, 103, 2),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 2,  1, new int[]{ 102, 103, 103, 103 }, 4, 103, 2),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 2,  2, new int[]{ 102, 103, 103, 103 }, 4, 103, 2),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 3,  0, new int[]{ 102, 103, 103, 103 }, 4, 103, 3),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 3,  1, new int[]{ 102, 103, 103, 103 }, 4, 103, 3),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 3,  2, new int[]{ 102, 103, 103, 103 }, 4, 103, 3),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 5,  0, new int[]{ 102, 103, 103, 103 }, 4, 103, 5),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 5,  1, new int[]{ 102, 103, 103, 103 }, 4, 103, 5),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 5,  2, new int[]{ 102, 103, 103, 103 }, 4, 103, 5),        //쓰리잡바 x 1, 하
                new ALDScoreTableInfo( 400 * 10,  0, new int[]{ 102, 103, 103, 103 }, 4, 103, 10),        //쓰리잡바 x 1, 상   102-빠를 제외한 타일
                new ALDScoreTableInfo( 400 * 10,  1, new int[]{ 102, 103, 103, 103 }, 4, 103, 10),        //쓰리잡바 x 1, 중
                new ALDScoreTableInfo( 400 * 10,  2, new int[]{ 102, 103, 103, 103 }, 4, 103, 10),        //쓰리잡바 x 1, 하

                new ALDScoreTableInfo( 1000 * 1,  0, new int[]{ 103, 103, 103, 103 }, 4, 104, 1),        //포잡바 x 1, 상   103-빠타일
                new ALDScoreTableInfo( 1000 * 1, 1, new int[]{ 103, 103, 103, 103 }, 4, 104, 1),        //포잡바 x 1, 중
                new ALDScoreTableInfo( 1000 * 1,  2, new int[]{ 103, 103, 103, 103 }, 4, 104, 1),        //포잡바 x 1, 하
                new ALDScoreTableInfo( 1000 * 2,  0, new int[]{ 103, 103, 103, 103 }, 4, 104, 2),        //포잡바 x 1, 상   103-빠타일
                new ALDScoreTableInfo( 1000 * 2, 1, new int[]{ 103, 103, 103, 103 }, 4, 104, 2),        //포잡바 x 1, 중
                new ALDScoreTableInfo( 1000 * 2,  2, new int[]{ 103, 103, 103, 103 }, 4, 104, 2),        //포잡바 x 1, 하
                new ALDScoreTableInfo( 1000 * 3,  0, new int[]{ 103, 103, 103, 103 }, 4, 104, 3),        //포잡바 x 1, 상   103-빠타일
                new ALDScoreTableInfo( 1000 * 3, 1, new int[]{ 103, 103, 103, 103 }, 4, 104, 3),        //포잡바 x 1, 중
                new ALDScoreTableInfo( 1000 * 3,  2, new int[]{ 103, 103, 103, 103 }, 4, 104, 3),        //포잡바 x 1, 하
                new ALDScoreTableInfo( 1000 * 5,  0, new int[]{ 103, 103, 103, 103 }, 4, 104, 5),        //포잡바 x 1, 상   103-빠타일
                new ALDScoreTableInfo( 1000 * 5, 1, new int[]{ 103, 103, 103, 103 }, 4, 104, 5),        //포잡바 x 1, 중
                new ALDScoreTableInfo( 1000 * 5,  2, new int[]{ 103, 103, 103, 103 }, 4, 104, 5),        //포잡바 x 1, 하
                new ALDScoreTableInfo( 1000 * 10,  0, new int[]{ 103, 103, 103, 103 }, 4, 104, 10),        //포잡바 x 1, 상   103-빠타일
                new ALDScoreTableInfo( 1000 * 10, 1, new int[]{ 103, 103, 103, 103 }, 4, 104, 10),        //포잡바 x 1, 중
                new ALDScoreTableInfo( 1000 * 10,  2, new int[]{ 103, 103, 103, 103 }, 4, 104, 10),        //포잡바 x 1, 하
                
            };





            m_lstALDScores = new List<int>();
            for (int i = 0; i < m_lstALDNormalScoreTable.Length; i++)
            {
                int nScore = m_lstALDNormalScoreTable[i].nGetScore;
                if (!m_lstALDScores.Exists(value => value == nScore))
                {
                    m_lstALDScores.Add(nScore);
                }
            }
            m_lstALDScores = m_lstALDScores.OrderBy(value => value).ToList();
        }
    }
}
