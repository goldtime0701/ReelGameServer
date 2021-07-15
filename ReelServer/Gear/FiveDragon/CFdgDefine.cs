using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CFdgScoreTableInfo
    {
        public int nScore;                  //당첨점수
        public int nTile;                   //당첨된타일           
        public int nCount;                  //당첨타일개수

        public CFdgScoreTableInfo(int nScore, int nTile, int nCount)
        {
            this.nScore = nScore;
            this.nTile = nTile;
            this.nCount = nCount;
        }
    }


    //잭팟정보
    public class CFdgPrizeInfo
    {
        public int m_nCont;         //1-5드래곤잭팟, 2-장군잭팟
        public int m_nPrizeCash;    //잭팟금액
        public int m_nType;         //장군잭팟시 grand, major, minor, mini
    }


    //점수정보객체 클라이언트에 보내여지는 객체이다.
    public class CFdgScoreInfo
    {
        public int m_nGearCode;         //기계코드
        public int m_nScore;            //맞은 총합계점수
        public List<int> m_lstScore;    //점수리스트(맞은 타일종류가 최대 3개까지 있을수 있으므로 점수 리스트를 내려보낸다.)
        public int m_nMulti;            //배당
        public int m_nFlag;             //스핀에서 진행하여야 할 동작을 나타내는 기발  1-드래곤잭팟시작 Reel,2-드래곤잭팟시작 Extra, 3-프리돌기, 4-프리잭팟끝, 5-연타
        public int m_nFlower;           //0-련꽃배당이 아님, 1-련꽃배당
        public int m_nMini;             //미니게임에서 당첨회수(2배당을 1회로 본다, 4배당 5회까지이므로 최대수치가 10이다.);   프리돌기일때는 돌기회수를 나타낸다
        public List<CTile> m_lstTile;   //당첨되지 않은 타일까지 포함한 완전한 타일배렬(3 X 5)

        public CFdgScoreInfo()
        {
            m_lstScore = new List<int>();
            m_nMulti = 1;
            m_nFlag = 0;
            m_nMini = 0;
            m_lstTile = new List<CTile>();
        }
    }


    public class CFdgScoreStep
    {
        public int m_nRaiseCash;       //점수를 주어야 하는 누적금
        public int m_nSocreCash;       //당첨점수
    }

    public partial class CFdgGear
    {
        public const int PRIZE_DRAGON = 1;
        public const int PRIZE_GENERAL = 2;

        CFdgScoreTableInfo[] m_lstFDGScoreTable;

        public override void InitScoreTableInfo()
        {
            m_lstFDGScoreTable = new CFdgScoreTableInfo[]
            {
                //0은 조커이다.

                new CFdgScoreTableInfo(1000, 1, 5),       //주작 X 5
                new CFdgScoreTableInfo(100, 1, 4),        //주작 X 4
                new CFdgScoreTableInfo(50,  1, 3),        //주작 X 3

                new CFdgScoreTableInfo(800, 2, 5),        //사자 X 5
                new CFdgScoreTableInfo(100, 2, 4),        //사자 X 4
                new CFdgScoreTableInfo(35,  2, 3),        //사자 X 3

                new CFdgScoreTableInfo(800, 3, 5),        //물고기 X 5
                new CFdgScoreTableInfo(100, 3, 4),        //물고기 X 4
                new CFdgScoreTableInfo(30,  3, 3),        //물고기 X 3

                new CFdgScoreTableInfo(300, 4, 5),        //거부기 X 5
                new CFdgScoreTableInfo(50,  4, 4),        //거부기 X 4
                new CFdgScoreTableInfo(20,  4, 3),        //거부기 X 3

                new CFdgScoreTableInfo(300, 5, 5),        //책 X 5
                new CFdgScoreTableInfo(35,  5, 4),        //책 X 4
                new CFdgScoreTableInfo(15,  5, 3),        //책 X 3

                new CFdgScoreTableInfo(200, 6, 5),        //A X 5
                new CFdgScoreTableInfo(30,  6, 4),        //A X 4
                new CFdgScoreTableInfo(10,  6, 3),        //A X 3

                new CFdgScoreTableInfo(200, 7, 5),        //K X 5
                new CFdgScoreTableInfo(20,  7, 4),        //K X 4
                new CFdgScoreTableInfo(10,  7, 3),        //K X 3

                new CFdgScoreTableInfo(100, 8, 5),        //Q X 5
                new CFdgScoreTableInfo(15,  8, 4),        //Q X 4
                new CFdgScoreTableInfo(10,  8, 3),        //Q X 3

                new CFdgScoreTableInfo(100, 9, 5),        //9 X 5
                new CFdgScoreTableInfo(10,  9, 4),        //9 X 4
                new CFdgScoreTableInfo(5,   9, 3),        //9 X 3

                new CFdgScoreTableInfo(100, 10, 5),       //10 X 5
                new CFdgScoreTableInfo(15,  10, 4),       //10 X 4
                new CFdgScoreTableInfo(5,   10, 3),       //10 X 3

                new CFdgScoreTableInfo(100, 11, 5),       //J X 5
                new CFdgScoreTableInfo(15,  11, 4),       //J X 4
                new CFdgScoreTableInfo(10,  11, 3),       //J X 3

                new CFdgScoreTableInfo(1250,  12, 5),      //동전 X 5
                new CFdgScoreTableInfo(250,  12, 4),       //동전 X 4
                new CFdgScoreTableInfo(125,   12, 3),      //동전 X 3
            };
        }
    }
}
