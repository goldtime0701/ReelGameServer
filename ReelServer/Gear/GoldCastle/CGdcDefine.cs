using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CGDCPrizeInfo
    {
        public int m_nCont;
        public int m_nPrizeCash;
        public int m_nPrizeMoney;
        public int m_nRemCash;
    }

    public class CGDCRaiseInfo
    {
        public CGDCScoreInfo m_scoreInfo;
        public int m_nRemCash;
    }

    public struct LineMulti
    {
        public int nLine;
        public int nMulti;

        public LineMulti(int line, int multi)
        {
            nLine = line;
            nMulti = multi;
        }
    }

    public class CGDCJackpotInfo
    {
        public int m_nCont;
        public int m_nStartCash;
        public int m_nEndCash;

        public CGDCJackpotInfo(int nCont, int nStartCash, int nEndCash)
        {
            m_nCont = nCont;
            m_nStartCash = nStartCash;
            m_nEndCash = nEndCash;
        }
    }

    public class CGDCScoreInfo
    {
        public int m_nGearCode;                 //기계번호
        public int m_nScore;                    //당첨점수
        public int m_nScore1;                   //오리지날배당점수
        public int m_nMulti0;                   //뒤배경배당
        public int m_nCmd;                      //이벤트종류 0-일반점수, 1-왕다래...
        public List<LineMulti> m_lstTileLine;       //당첨된줄
        public List<CTile> m_lstTile;           //당첨된 타일리스트
        public int m_nCount;                    //당첨된 타일개수
        public List<CTile> m_lstEvent;          //당첨된 이벤트리스트
        public List<LineMulti> m_lstEventLine;      //당첨된 이벤트줄
        public int m_nTile;                     //당첨타일
        public int m_nEvent;                    //당첨이벤트타일
        


        public CGDCScoreInfo()
        {
            m_lstTile = new List<CTile>();
            m_lstEvent = new List<CTile>();
            m_lstTileLine = new List<LineMulti>();
            m_lstEventLine = new List<LineMulti>();
        }

        public CGDCScoreInfo CopyObject()
        {
            CGDCScoreInfo scoreInfo = new CGDCScoreInfo();
            scoreInfo.m_nGearCode = this.m_nGearCode;
            scoreInfo.m_nScore = this.m_nScore;
            scoreInfo.m_nScore1 = this.m_nScore1;
            scoreInfo.m_nMulti0 = this.m_nMulti0;
            scoreInfo.m_nCmd = this.m_nCmd;
            scoreInfo.m_nCount = this.m_nCount;
            scoreInfo.m_nTile = this.m_nTile;
            scoreInfo.m_nEvent = this.m_nEvent;

            foreach(LineMulti lm in this.m_lstTileLine)
            {
                LineMulti newLm = new LineMulti(lm.nLine, lm.nMulti);
                scoreInfo.m_lstTileLine.Add(newLm);
            }

            foreach(CTile tile in m_lstTile)
            {
                CTile newTile = new CTile(tile.m_nTile, tile.m_nRow, tile.m_nCol);
                scoreInfo.m_lstTile.Add(tile);
            }

            foreach (LineMulti lm in this.m_lstEventLine)
            {
                LineMulti newLm = new LineMulti(lm.nLine, lm.nMulti);
                scoreInfo.m_lstEventLine.Add(newLm);
            }

            foreach (CTile tile in m_lstEvent)
            {
                CTile newTile = new CTile(tile.m_nTile, tile.m_nRow, tile.m_nCol);
                scoreInfo.m_lstEvent.Add(tile);
            }



            return scoreInfo;
        }

        public CGDCScoreInfo(int nScore, int nTile, int nCnt, int nLine, int nMulti0, int nMulti1, int[] lstTile, int nCmd = 0)
        {
            int nMulti = 1;
            if (nMulti0 == 1 && nMulti1 == 1)
                nMulti = 1;
            else if (nMulti0 == 1)
                nMulti = nMulti1;
            else if (nMulti1 == 1)
                nMulti = nMulti0;
            else
                nMulti = nMulti0 + nMulti1;

            m_nScore = nScore * 1; //일반점수일때는 배당이 없다.
            m_nScore1 = nScore;
            m_nTile = nTile;
            m_nCount = nCnt;
            m_nMulti0 = nMulti0;
            m_lstTileLine = new List<LineMulti>();
            m_lstTileLine.Add(new LineMulti(nLine, nMulti1));
            m_lstEventLine = new List<LineMulti>();
          
            m_nCmd = nCmd;

            m_lstTile = new List<CTile>();
            m_lstEvent = new List<CTile>();
            for (int i=0; i< lstTile.Length; i++)
            {
                if (lstTile[i] < 0)
                    continue;
                CTile tile = new CTile(lstTile[i], nLine, i);
                tile.m_nAct = 1;
                m_lstTile.Add(tile);
            }
        }

        public CGDCScoreInfo(int nScore, int nTile, int nLine, int nMulti0, int[] lstTile, int nTy)
        {
            m_nScore = nScore * nMulti0;
            m_nScore1 = nScore;
            m_nTile = nTile;
            m_nMulti0 = nMulti0;
            m_lstTileLine = new List<LineMulti>();
            m_lstTileLine.Add(new LineMulti(nLine, 1));
            m_nCount = lstTile.ToList().Count(value => value != -1);
            m_lstTile = new List<CTile>();
            m_lstEvent = new List<CTile>();
            for (int i = 0; i < lstTile.Length; i++)
            {
                if (lstTile[i] < 0)
                    continue;
                CTile tile = new CTile(lstTile[i], nLine, i);
                tile.m_nAct = 1;
                m_lstTile.Add(tile);
            }
        }


        public CGDCScoreInfo(int nScore, int nEvent, int nEventLine, int nMulti0, int[] lstEvent)
        {
            m_nScore = nScore * 1;
            m_nScore1 = nScore;
            m_nEvent = nEvent;
            m_nMulti0 = nMulti0;
            m_lstTileLine = new List<LineMulti>();
            m_lstEventLine = new List<LineMulti>();
            m_lstEventLine.Add(new LineMulti(nEventLine, 1));
            m_nCount = lstEvent.ToList().Count(value => value != -1);
            m_lstTile = new List<CTile>();
            m_lstEvent = new List<CTile>();
            for (int i = 0; i < lstEvent.Length; i++)
            {
                if (lstEvent[i] < 0)
                    continue;
                CTile tile = new CTile(lstEvent[i], nEventLine, i);
                tile.m_nAct = 1;
                m_lstEvent.Add(tile);
            }
        }

        public CGDCScoreInfo(int nScore, int nEvent, int nEventLine, int nMulti0, int nMulti1, int[] lstEvent)
        {
            int nMulti = 1;
            if (nMulti0 == 1 && nMulti1 == 1)
                nMulti = 1;
            else if (nMulti0 == 1)
                nMulti = nMulti1;
            else if (nMulti1 == 1)
                nMulti = nMulti0;
            else
                nMulti = nMulti0 + nMulti1;

            m_nCount = lstEvent.ToList().Count(value => value != -1);
            m_nScore = nScore * nMulti;
            m_nScore1 = nScore;
            m_nEvent = nEvent;
            m_nMulti0 = nMulti0;
            m_lstTileLine = new List<LineMulti>();
            m_lstEventLine = new List<LineMulti>();
            m_lstEventLine.Add(new LineMulti(nEventLine, nMulti1));

            m_lstTile = new List<CTile>();
            m_lstEvent = new List<CTile>();
            for (int i = 0; i < lstEvent.Length; i++)
            {
                if (lstEvent[i] < 0)
                    continue;
                CTile tile = new CTile(lstEvent[i], nEventLine, i);
                tile.m_nAct = 1;
                m_lstEvent.Add(tile);
            }
        }
    }


    public partial class CGdcGear
    {
        private const int TOP = 2;  //상
        private const int MID = 1;  //중
        private const int BOT = 0;  //하

        private int[] m_nlstMulti0 = { 1, 2, 3, 5, 7 };                 //배경색배당
        private int[] m_nlstMulti1 = { 1, 2, 3, 5, 7, 10 };             //페이라인색배당

        //타일종류
        private const int EMPTYT     =-1;           //빈타일
        private const int GJOKER     = 0;           //유리조커
        private const int HJOKER     = 1;           //하트조커
        private const int BUTTER     = 2;           //나비
        private const int RCASTLE    = 3;           //푸른성
        private const int WCASTLE    = 4;           //흰성
        private const int HOOKCAP    = 5;           //후크선장
        private const int HEART      = 6;           //하트
        private const int BEAM       = 7;           //저울
        private const int RACORN     = 8;           //붉은 도토리
        private const int WACORN     = 9;           //흰 도토리

        //이벤트종류
        public const int EVE_4BAR   = 0;           //포빠
        public const int EVE_3BAR0  = 1;           //쓰리빠0
        public const int EVE_3BAR1  = 2;           //쓰리빠1
        public const int EVE_2BAR0  = 3;           //투빠0
        public const int EVE_2BAR1  = 4;           //투빠1
        public const int EVE_1BAR0  = 5;           //원빠0
        public const int EVE_1BAR1  = 6;           //원빠1
        public const int EVE_MUSIC0 = 7;           //뮤직0
        public const int EVE_MUSIC1 = 8;           //뮤직1
        public const int EVE_SEVEN  = 9;           //세븐
        public const int EVE_STAR0  = 10;          //스타0
        public const int EVE_STAR1  = 11;          //스타1
        public const int EVE_TAEGK  = 12;          //태극
        public const int EVE_PPIYO  = 13;          //삐요
        public const int EVE_DRAGON = 14;          //드래곤


        //잭팟종류
        public const int SCORE_NORMAL = 0;
        public const int JACK_GJOKER = 1;          //조커잭팟
        public const int JACK_HJOKER = 2;          //조커잭팟
        public const int PRIZE_GJOKER = 3;         //조커보너스 갈갈이   
        public const int PRIZE_HJOKER = 4;         //조커보너스 용갈이   
        public const int PRIZE_BUTTER = 5;         //나비보너스 황금나비 
        public const int PRIZE_RCASTLE = 6;        //붉은성보너스   전화면이 바뀌는 잭팟이벤트  X 3
        public const int PRIZE_WCASTLE = 7;        //푸른성보너스   전화면이 바뀌는 잭팟이벤트  X 3
        public const int PRIZE_HOOK = 8;           //후크선장보너스 전화면이 바뀌는 잭팟이벤트  X 3
        public const int EVENT_RCASTLE = 9;        //붉은성이벤트
        public const int EVENT_WCASTLE = 10;       //푸른성이벤트
        public const int EVENT_HOOK = 11;          //후크이벤트
        public const int BUTTER_JOKER = 12;        //나비+조커
        public const int LARGE_RACORN = 13;        //초대형 다래, 저울 보너스
        public const int LARGE_WACORN = 14;        //초대형 다래, 저울 보너스
        public const int LARGE_BEAM = 15;          //초대형 다래, 저울 보너스
        public const int PRIZE_END = 20;           //삐에로


        private List<CGDCScoreInfo> m_lstGDCScoreInfo;   //일반점수배렬, 잭팟이 아닌 일반돌기일때만 나온다.
        private List<CGDCScoreInfo> m_lstGDCEventInfo;   //이벤트점수배렬(고배당당첨이후 뒤점수를 줄때 사용)
        private List<CGDCScoreInfo> m_lstGDCNoiseInfo;   //가짜이벤트점수배렬
        private List<CGDCScoreInfo> m_lstGDCPrizeInfo;   //잭팟이벤트배렬
        private List<CGDCScoreInfo> m_lstGDCJokerEvent;  //조커이벤트시 점수배렬
        private List<CGDCScoreInfo> m_lstGDCBieroEvent;  //삐에로이벤트시 점수배렬
        private List<CGDCScoreInfo> m_lstGDCRaise1Jack;  //3개짜리로 고배당을 주는 이벤트(1차환수률을 적용할때 사용하여 드문히 나오게 한다.)

        private List<int> m_lstGDCScoreCash;
        private List<CGDCJackpotInfo> m_lstJackpotInfo;


        public override void InitScoreTableInfo()
        {
            m_lstGDCScoreInfo = new List<CGDCScoreInfo>();
            m_lstGDCEventInfo = new List<CGDCScoreInfo>();
            m_lstGDCPrizeInfo = new List<CGDCScoreInfo>();
            m_lstGDCJokerEvent = new List<CGDCScoreInfo>();
            m_lstGDCBieroEvent = new List<CGDCScoreInfo>();
            m_lstGDCNoiseInfo = new List<CGDCScoreInfo>();
            m_lstGDCRaise1Jack = new List<CGDCScoreInfo>();

            m_lstGDCScoreCash = new List<int>();
            m_lstJackpotInfo = new List<CGDCJackpotInfo>();

            #region 일반점수배렬생성
            for (int i=0; i<m_nlstMulti0.Length; i++)
            {
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, HJOKER }));       //유리조커, 하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, HJOKER }));       //유리조커, 하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, HJOKER }));       //유리조커, 하트조커, 유리조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, HJOKER }));       //유리조커, 유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, HJOKER }));       //유리조커, 유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, HJOKER }));       //유리조커, 유리조커, 하트조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, GJOKER }));       //유리조커, 유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, GJOKER }));       //유리조커, 유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, GJOKER }));       //유리조커, 유리조커, 하트조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, GJOKER }));       //유리조커, 하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, GJOKER }));       //유리조커, 하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, GJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, GJOKER }));       //유리조커, 하트조커, 유리조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, GJOKER }));       //하트조커, 유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, GJOKER }));       //하트조커, 유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, GJOKER }));       //하트조커, 유리조커, 하트조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, GJOKER }));       //하트조커, 하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, GJOKER }));       //하트조커, 하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, GJOKER }));       //하트조커, 하트조커, 유리조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, HJOKER }));       //하트조커, 하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, HJOKER }));       //하트조커, 하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, HJOKER }));       //하트조커, 하트조커, 유리조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, HJOKER }));       //하트조커, 유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, HJOKER }));       //하트조커, 유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(01000, HJOKER, 4, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, HJOKER }));       //하트조커, 유리조커, 하트조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, EMPTYT }));       //유리조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, EMPTYT }));       //유리조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, GJOKER, HJOKER, EMPTYT }));       //유리조커, 유리조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, EMPTYT }));       //유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, EMPTYT }));       //유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, GJOKER, EMPTYT }));       //유리조커, 하트조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, GJOKER, EMPTYT }));       //하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, GJOKER, EMPTYT }));       //하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, GJOKER, EMPTYT }));       //하트조커, 유리조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, EMPTYT }));       //하트조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, EMPTYT }));       //하트조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, HJOKER, GJOKER, EMPTYT }));       //하트조커, 하트조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, EMPTYT }));       //하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, EMPTYT }));       //하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { HJOKER, GJOKER, HJOKER, EMPTYT }));       //하트조커, 유리조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, HJOKER, EMPTYT }));       //유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, HJOKER, EMPTYT }));       //유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { GJOKER, HJOKER, HJOKER, EMPTYT }));       //유리조커, 하트조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, GJOKER, HJOKER }));       //유리조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, GJOKER, HJOKER }));       //유리조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, GJOKER, HJOKER }));       //유리조커, 유리조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, HJOKER, GJOKER }));       //유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, HJOKER, GJOKER }));       //유리조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, HJOKER, GJOKER }));       //유리조커, 하트조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, GJOKER, GJOKER }));       //하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, GJOKER, GJOKER }));       //하트조커, 유리조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, GJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, GJOKER, GJOKER }));       //하트조커, 유리조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, HJOKER, GJOKER }));       //하트조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, HJOKER, GJOKER }));       //하트조커, 하트조커, 유리조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, HJOKER, GJOKER }));       //하트조커, 하트조커, 유리조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, GJOKER, HJOKER }));       //하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, GJOKER, HJOKER }));       //하트조커, 유리조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, HJOKER, GJOKER, HJOKER }));       //하트조커, 유리조커, 하트조커

                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, HJOKER, HJOKER }));       //유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, HJOKER, HJOKER }));       //유리조커, 하트조커, 하트조커
                m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00500, HJOKER, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, GJOKER, HJOKER, HJOKER }));       //유리조커, 하트조커, 하트조커

                for(int j = 0; j < 10; j++)
                {
                    //나비나 하트일때는 일반점수인데도 배당이 적용된다고 하므로 일반점수에서는 배경배당을 1로만 설정해준다.
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(03000, BUTTER, 3, TOP, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }));       //나비 X 3
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(06000, BUTTER, 3, MID, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }));       //나비 X 3
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(03000, BUTTER, 3, BOT, 1, 1, new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }));       //나비 X 3
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(03000, BUTTER, 3, TOP, 1, 1, new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }));       //나비 X 3
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(06000, BUTTER, 3, MID, 1, 1, new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }));       //나비 X 3
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(03000, BUTTER, 3, BOT, 1, 1, new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }));       //나비 X 3

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(05000, HEART, 4, TOP, 1, 1, new int[] { HEART, HEART, HEART, HEART }));            //하트, 하트, 하트, 하트
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(10000, HEART, 4, MID, 1, 1, new int[] { HEART, HEART, HEART, HEART }));            //하트, 하트, 하트, 하트
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(05000, HEART, 4, BOT, 1, 1, new int[] { HEART, HEART, HEART, HEART }));            //하트, 하트, 하트, 하트

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(02500, HEART, 3, TOP, 1, 1, new int[] { HEART, HEART, HEART, EMPTYT }));           //하트, 하트, 하트
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(05000, HEART, 3, MID, 1, 1, new int[] { HEART, HEART, HEART, EMPTYT }));           //하트, 하트, 하트
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(02500, HEART, 3, BOT, 1, 1, new int[] { HEART, HEART, HEART, EMPTYT }));           //하트, 하트, 하트

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(02500, HEART, 3, TOP, 1, 1, new int[] { EMPTYT, HEART, HEART, HEART }));           //하트, 하트, 하트
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(05000, HEART, 3, MID, 1, 1, new int[] { EMPTYT, HEART, HEART, HEART }));           //하트, 하트, 하트
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(02500, HEART, 3, BOT, 1, 1, new int[] { EMPTYT, HEART, HEART, HEART }));           //하트, 하트, 하트

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, BEAM, 4, TOP, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, BEAM }));                 //저울, 저울, 저울, 저울
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, BEAM, 4, MID, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, BEAM }));                 //저울, 저울, 저울, 저울
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, BEAM, 4, BOT, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, BEAM }));                 //저울, 저울, 저울, 저울

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, BEAM, 3, TOP, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, EMPTYT }));               //저울, 저울, 저울
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, BEAM, 3, MID, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, EMPTYT }));               //저울, 저울, 저울
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, BEAM, 3, BOT, m_nlstMulti0[i], 1, new int[] { BEAM, BEAM, BEAM, EMPTYT }));               //저울, 저울, 저울

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, BEAM, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, BEAM, BEAM, BEAM }));               //저울, 저울, 저울
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, BEAM, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, BEAM, BEAM, BEAM }));               //저울, 저울, 저울
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, BEAM, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, BEAM, BEAM, BEAM }));               //저울, 저울, 저울

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, RACORN, 4, TOP, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, RACORN }));       //붉은도토리, 붉은도토리, 붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, RACORN, 4, MID, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, RACORN }));       //붉은도토리, 붉은도토리, 붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, RACORN, 4, BOT, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, RACORN }));       //붉은도토리, 붉은도토리, 붉은도토리, 붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, RACORN, 3, TOP, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, EMPTYT }));       //붉은도토리, 붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, RACORN, 3, MID, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, EMPTYT }));       //붉은도토리, 붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, RACORN, 3, BOT, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, RACORN, EMPTYT }));       //붉은도토리, 붉은도토리, 붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, RACORN, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, RACORN, RACORN, RACORN }));       //붉은도토리, 붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, RACORN, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, RACORN, RACORN, RACORN }));       //붉은도토리, 붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, RACORN, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, RACORN, RACORN, RACORN }));       //붉은도토리, 붉은도토리, 붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, RACORN, 2, TOP, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, EMPTYT, EMPTYT }));       //붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, RACORN, 2, MID, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, EMPTYT, EMPTYT }));       //붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, RACORN, 2, BOT, m_nlstMulti0[i], 1, new int[] { RACORN, RACORN, EMPTYT, EMPTYT }));       //붉은도토리, 붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, RACORN, 2, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, RACORN, RACORN }));       //붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, RACORN, 2, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, RACORN, RACORN }));       //붉은도토리, 붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, RACORN, 2, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, RACORN, RACORN }));       //붉은도토리, 붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, RACORN, 1, TOP, m_nlstMulti0[i], 1, new int[] { RACORN, EMPTYT, EMPTYT, EMPTYT }));       //붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, RACORN, 1, MID, m_nlstMulti0[i], 1, new int[] { RACORN, EMPTYT, EMPTYT, EMPTYT }));       //붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, RACORN, 1, BOT, m_nlstMulti0[i], 1, new int[] { RACORN, EMPTYT, EMPTYT, EMPTYT }));       //붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, RACORN, 1, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, EMPTYT, RACORN }));       //붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, RACORN, 1, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, EMPTYT, RACORN }));       //붉은도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, RACORN, 1, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, EMPTYT, RACORN }));       //붉은도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, WACORN, 4, TOP, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, WACORN }));       //흰도토리, 흰도토리, 흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, WACORN, 4, MID, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, WACORN }));       //흰도토리, 흰도토리, 흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00350, WACORN, 4, BOT, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, WACORN }));       //흰도토리, 흰도토리, 흰도토리, 흰도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, WACORN, 3, TOP, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, EMPTYT }));       //흰도토리, 흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, WACORN, 3, MID, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, EMPTYT }));       //흰도토리, 흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, WACORN, 3, BOT, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, WACORN, EMPTYT }));       //흰도토리, 흰도토리, 흰도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, WACORN, 3, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, WACORN, WACORN, WACORN }));       //흰도토리, 흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, WACORN, 3, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, WACORN, WACORN, WACORN }));       //흰도토리, 흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00250, WACORN, 3, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, WACORN, WACORN, WACORN }));       //흰도토리, 흰도토리, 흰도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, WACORN, 2, TOP, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, EMPTYT, EMPTYT }));       //흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, WACORN, 2, MID, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, EMPTYT, EMPTYT }));       //흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, WACORN, 2, BOT, m_nlstMulti0[i], 1, new int[] { WACORN, WACORN, EMPTYT, EMPTYT }));       //흰도토리, 흰도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, WACORN, 2, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, WACORN, WACORN }));       //흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, WACORN, 2, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, WACORN, WACORN }));       //흰도토리, 흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00150, WACORN, 2, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, WACORN, WACORN }));       //흰도토리, 흰도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, WACORN, 1, TOP, m_nlstMulti0[i], 1, new int[] { WACORN, EMPTYT, EMPTYT, EMPTYT }));       //흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, WACORN, 1, MID, m_nlstMulti0[i], 1, new int[] { WACORN, EMPTYT, EMPTYT, EMPTYT }));       //흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, WACORN, 1, BOT, m_nlstMulti0[i], 1, new int[] { WACORN, EMPTYT, EMPTYT, EMPTYT }));       //흰도토리

                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, WACORN, 1, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, EMPTYT, WACORN }));       //흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, WACORN, 1, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, EMPTYT, WACORN }));       //흰도토리
                    m_lstGDCScoreInfo.Add(new CGDCScoreInfo(00100, WACORN, 1, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EMPTYT, EMPTYT, WACORN }));       //흰도토리
                }
            }
            #endregion

            #region 이벤트점수배렬생성
            //이벤트점수배렬생성  잭팟이 터진다음에 뒤점수를 줄때는 무조건 무배당으로 점수를 준다.
            for (int i=0; i<10; i++)
            {
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(32000, EVE_3BAR0, MID, 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠

                m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(16000, EVE_2BAR0, MID, 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠

                m_lstGDCEventInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(12000, EVE_1BAR0, MID, 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
            }
            for (int i = 0; i < 3; i++)
            {
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(10000, EVE_MUSIC0, MID, 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직

                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(10000, EVE_SEVEN, MID, 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐

                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(10000, EVE_STAR0, MID, 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타

                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(10000, EVE_TAEGK, MID, 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                m_lstGDCEventInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
            }
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_3BAR0, TOP, 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EMPTYT }));  //3빠, 3빠, 3빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, MID, 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EMPTYT }));  //3빠, 3빠, 3빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_3BAR0, BOT, 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EMPTYT }));  //3빠, 3빠, 3빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_3BAR0, TOP, 1, new int[] { EMPTYT, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, MID, 1, new int[] { EMPTYT, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_3BAR0, BOT, 1, new int[] { EMPTYT, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠

            m_lstGDCEventInfo.Add(new CGDCScoreInfo(04000, EVE_2BAR0, TOP, 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EMPTYT }));  //2빠, 2빠, 2빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, MID, 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EMPTYT }));  //2빠, 2빠, 2빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(04000, EVE_2BAR0, BOT, 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EMPTYT }));  //2빠, 2빠, 2빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(04000, EVE_2BAR0, TOP, 1, new int[] { EMPTYT, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, MID, 1, new int[] { EMPTYT, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(04000, EVE_2BAR0, BOT, 1, new int[] { EMPTYT, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠

            m_lstGDCEventInfo.Add(new CGDCScoreInfo(03000, EVE_1BAR0, TOP, 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EMPTYT }));  //1빠빠빠빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, MID, 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EMPTYT }));  //1빠빠빠빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(03000, EVE_1BAR0, BOT, 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EMPTYT }));  //1빠빠빠빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(03000, EVE_1BAR0, TOP, 1, new int[] { EMPTYT, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, MID, 1, new int[] { EMPTYT, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
            m_lstGDCEventInfo.Add(new CGDCScoreInfo(03000, EVE_1BAR0, BOT, 1, new int[] { EMPTYT, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
            #endregion

            #region 가짜이벤트점수배렬생성
            for (int i=0; i<m_nlstMulti0.Length; i++)
            {
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_MUSIC0, TOP, m_nlstMulti0[i], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EMPTYT }));  //뮤직, 뮤직, 뮤직
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_MUSIC0, MID, m_nlstMulti0[i], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EMPTYT }));  //뮤직, 뮤직, 뮤직
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_MUSIC0, BOT, m_nlstMulti0[i], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EMPTYT }));  //뮤직, 뮤직, 뮤직

                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_MUSIC0, TOP, m_nlstMulti0[i], new int[] { EMPTYT, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_MUSIC0, MID, m_nlstMulti0[i], new int[] { EMPTYT, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_MUSIC0, BOT, m_nlstMulti0[i], new int[] { EMPTYT, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직


                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_SEVEN, TOP, m_nlstMulti0[i], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EMPTYT }));  //세븐, 세븐, 세븐
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_SEVEN, MID, m_nlstMulti0[i], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EMPTYT }));  //세븐, 세븐, 세븐
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_SEVEN, BOT, m_nlstMulti0[i], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EMPTYT }));  //세븐, 세븐, 세븐

                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_SEVEN, TOP, m_nlstMulti0[i], new int[] { EMPTYT, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_SEVEN, MID, m_nlstMulti0[i], new int[] { EMPTYT, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_SEVEN, BOT, m_nlstMulti0[i], new int[] { EMPTYT, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐


                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_STAR0, TOP, m_nlstMulti0[i], new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EMPTYT }));  //스타, 스타, 스타
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_STAR0, MID, m_nlstMulti0[i], new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EMPTYT }));  //스타, 스타, 스타
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_STAR0, BOT, m_nlstMulti0[i], new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EMPTYT }));  //스타, 스타, 스타

                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_STAR0, TOP, m_nlstMulti0[i], new int[] { EMPTYT, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_STAR0, MID, m_nlstMulti0[i], new int[] { EMPTYT, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_STAR0, BOT, m_nlstMulti0[i], new int[] { EMPTYT, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타


                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_TAEGK, TOP, m_nlstMulti0[i], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EMPTYT }));  //태극, 태극, 태극
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_TAEGK, MID, m_nlstMulti0[i], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EMPTYT }));  //태극, 태극, 태극
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_TAEGK, BOT, m_nlstMulti0[i], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EMPTYT }));  //태극, 태극, 태극

                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_TAEGK, TOP, m_nlstMulti0[i], new int[] { EMPTYT, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_TAEGK, MID, m_nlstMulti0[i], new int[] { EMPTYT, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극
                m_lstGDCNoiseInfo.Add(new CGDCScoreInfo(00400, EVE_TAEGK, BOT, m_nlstMulti0[i], new int[] { EMPTYT, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극
            }
            #endregion

            #region  조커이벤트점수배렬생
            for (int i = 0; i < m_nlstMulti0.Length; i++)
            {
                for(int j=0; j<3; j++)
                {
                    int nMulti = 1;
                    if (j == 0)
                        nMulti = 1;
                    else if (j == 1)
                        nMulti = 2;
                    else if (j == 2)
                        nMulti = 4;

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(32000, EVE_4BAR, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(64000, EVE_4BAR, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(32000, EVE_4BAR, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(32000, EVE_3BAR0, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(16000, EVE_2BAR0, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(12000, EVE_1BAR0, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠빠빠빠

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(10000, EVE_MUSIC0, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(10000, EVE_SEVEN, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐

                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(10000, EVE_STAR0, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], nMulti, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(10000, EVE_TAEGK, MID, m_nlstMulti0[i], nMulti, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                    m_lstGDCJokerEvent.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], nMulti, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                }
            }
            #endregion

            #region 잭팟이벤트점수배렬생성
            for (int i = 0; i < m_nlstMulti0.Length; i++)
            {
                for(int j=0; j<10; j++)
                {
                    //이벤트점수배렬  int nScore, int nEvent, int nEventLine, int nMulti0, int nMulti1, int[] lstEvent
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(32000, EVE_4BAR, TOP, m_nlstMulti0[i], 1, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(64000, EVE_4BAR, MID, m_nlstMulti0[i], 1, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(32000, EVE_4BAR, BOT, m_nlstMulti0[i], 1, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(32000, EVE_3BAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_2BAR0, EVE_2BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_2BAR0, EVE_2BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_1BAR0, EVE_1BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_1BAR0, EVE_1BAR0 }));  //3빠, 3빠, 3빠, 3빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_2BAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_1BAR0, EVE_1BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_1BAR0, EVE_1BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_1BAR0, EVE_1BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_1BAR0, EVE_1BAR0 }));  //2빠, 2빠, 2빠, 2빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(12000, EVE_1BAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_3BAR0, EVE_3BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_3BAR0, EVE_3BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_2BAR0, EVE_2BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_2BAR0, EVE_2BAR0 }));  //1빠, 1빠, 1빠, 1빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_MUSIC0, MID, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_SEVEN, EVE_SEVEN }));  //뮤직, 뮤직, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_SEVEN, EVE_SEVEN }));  //뮤직, 뮤직, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_STAR0, EVE_STAR0 }));  //뮤직, 뮤직, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_STAR0, EVE_STAR0 }));  //뮤직, 뮤직, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_TAEGK, EVE_TAEGK }));  //뮤직, 뮤직, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_TAEGK, EVE_TAEGK }));  //뮤직, 뮤직, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_SEVEN, MID, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_MUSIC0, EVE_MUSIC0 }));  //세븐, 세븐, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_MUSIC0, EVE_MUSIC0 }));  //세븐, 세븐, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_STAR0, EVE_STAR0 }));  //세븐, 세븐, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_STAR0, EVE_STAR0 }));  //세븐, 세븐, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_TAEGK, EVE_TAEGK }));  //세븐, 세븐, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], 1, new int[] { EVE_SEVEN, EVE_SEVEN, EVE_TAEGK, EVE_TAEGK }));  //세븐, 세븐, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_STAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_SEVEN, EVE_SEVEN }));  //스타, 스타, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_SEVEN, EVE_SEVEN }));  //스타, 스타, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_MUSIC0, EVE_MUSIC0 }));  //스타, 스타, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_MUSIC0, EVE_MUSIC0 }));  //스타, 스타, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_TAEGK, EVE_TAEGK }));  //스타, 스타, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_STAR0, EVE_STAR0, EVE_TAEGK, EVE_TAEGK }));  //스타, 스타, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_TAEGK, MID, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_SEVEN, EVE_SEVEN }));  //태극, 태극, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_SEVEN, EVE_SEVEN }));  //태극, 태극, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_MUSIC0, EVE_MUSIC0 }));  //태극, 태극, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_MUSIC0, EVE_MUSIC0 }));  //태극, 태극, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_STAR0, EVE_STAR0 }));  //태극, 태극, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], 1, new int[] { EVE_TAEGK, EVE_TAEGK, EVE_STAR0, EVE_STAR0 }));  //태극, 태극, 스타, 스타
                }

                for (int j = 0; j < m_nlstMulti1.Length; j++)
                {
                    //이벤트점수배렬  int nScore, int nEvent, int nEventLine, int nMulti0, int nMulti1, int[] lstEvent
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(32000, EVE_4BAR, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(64000, EVE_4BAR, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(32000, EVE_4BAR, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EVE_4BAR }));       //4빠, 4빠, 4빠, 4빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(32000, EVE_3BAR0, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_3BAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));  //3빠, 3빠, 3빠, 3빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(16000, EVE_2BAR0, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(08000, EVE_2BAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));  //2빠, 2빠, 2빠, 2빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(12000, EVE_1BAR0, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(06000, EVE_1BAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));  //1빠, 1빠, 1빠, 1빠

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_MUSIC0, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0, EVE_MUSIC0 }));  //뮤직, 뮤직, 뮤직, 뮤직

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_SEVEN, EVE_SEVEN }));  //뮤직, 뮤직, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_SEVEN, EVE_SEVEN }));  //뮤직, 뮤직, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_STAR0, EVE_STAR0 }));  //뮤직, 뮤직, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_STAR0, EVE_STAR0 }));  //뮤직, 뮤직, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_TAEGK, EVE_TAEGK }));  //뮤직, 뮤직, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_MUSIC0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC0, EVE_MUSIC0, EVE_TAEGK, EVE_TAEGK }));  //뮤직, 뮤직, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_SEVEN, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_SEVEN, EVE_SEVEN }));  //세븐, 세븐, 세븐, 세븐

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_MUSIC0, EVE_MUSIC0 }));  //세븐, 세븐, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_MUSIC0, EVE_MUSIC0 }));  //세븐, 세븐, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_STAR0, EVE_STAR0 }));  //세븐, 세븐, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_STAR0, EVE_STAR0 }));  //세븐, 세븐, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_TAEGK, EVE_TAEGK }));  //세븐, 세븐, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_SEVEN, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_SEVEN, EVE_SEVEN, EVE_TAEGK, EVE_TAEGK }));  //세븐, 세븐, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_STAR0, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_STAR0, EVE_STAR0 }));  //스타, 스타, 스타, 스타

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_SEVEN, EVE_SEVEN }));  //스타, 스타, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_SEVEN, EVE_SEVEN }));  //스타, 스타, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_MUSIC0, EVE_MUSIC0 }));  //스타, 스타, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_MUSIC0, EVE_MUSIC0 }));  //스타, 스타, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_TAEGK, EVE_TAEGK }));  //스타, 스타, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_STAR0, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR0, EVE_STAR0, EVE_TAEGK, EVE_TAEGK }));  //스타, 스타, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(10000, EVE_TAEGK, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_TAEGK, EVE_TAEGK }));  //태극, 태극, 태극, 태극

                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_SEVEN, EVE_SEVEN }));  //태극, 태극, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_SEVEN, EVE_SEVEN }));  //태극, 태극, 세븐, 세븐
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_MUSIC0, EVE_MUSIC0 }));  //태극, 태극, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_MUSIC0, EVE_MUSIC0 }));  //태극, 태극, 음표, 음표
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_STAR0, EVE_STAR0 }));  //태극, 태극, 스타, 스타
                    m_lstGDCPrizeInfo.Add(new CGDCScoreInfo(05000, EVE_TAEGK, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_TAEGK, EVE_TAEGK, EVE_STAR0, EVE_STAR0 }));  //태극, 태극, 스타, 스타
                }
            }
            #endregion

            #region 삐에로이벤트점수배렬생성
            for (int i = 0; i < m_nlstMulti0.Length; i++)
            {
                for (int j = 0; j < m_nlstMulti1.Length; j++)
                {
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(16000, EVE_3BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR1, EVE_3BAR1, EVE_3BAR1, EVE_3BAR1 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(32000, EVE_3BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR1, EVE_3BAR1, EVE_3BAR1, EVE_3BAR1 }));  //3빠, 3빠, 3빠, 3빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(16000, EVE_3BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR1, EVE_3BAR1, EVE_3BAR1, EVE_3BAR1 }));  //3빠, 3빠, 3빠, 3빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_3BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR1, EVE_3BAR1, EVE_3BAR1, EMPTYT }));     //3빠, 3빠, 3빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(16000, EVE_3BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR1, EVE_3BAR1, EVE_3BAR1, EMPTYT }));     //3빠, 3빠, 3빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_3BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_3BAR1, EVE_3BAR1, EVE_3BAR1, EMPTYT }));     //3빠, 3빠, 3빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_3BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_3BAR1, EVE_3BAR1, EVE_3BAR1 }));     //3빠, 3빠, 3빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(16000, EVE_3BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_3BAR1, EVE_3BAR1, EVE_3BAR1 }));     //3빠, 3빠, 3빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_3BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_3BAR1, EVE_3BAR1, EVE_3BAR1 }));     //3빠, 3빠, 3빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_2BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR1, EVE_2BAR1, EVE_2BAR1, EVE_2BAR1 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(16000, EVE_2BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR1, EVE_2BAR1, EVE_2BAR1, EVE_2BAR1 }));  //2빠, 2빠, 2빠, 2빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_2BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR1, EVE_2BAR1, EVE_2BAR1, EVE_2BAR1 }));  //2빠, 2빠, 2빠, 2빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(04000, EVE_2BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR1, EVE_2BAR1, EVE_2BAR1, EMPTYT }));     //2빠, 2빠, 2빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_2BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR1, EVE_2BAR1, EVE_2BAR1, EMPTYT }));     //2빠, 2빠, 2빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(04000, EVE_2BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_2BAR1, EVE_2BAR1, EVE_2BAR1, EMPTYT }));     //2빠, 2빠, 2빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(04000, EVE_2BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_2BAR1, EVE_2BAR1, EVE_2BAR1 }));     //2빠, 2빠, 2빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(08000, EVE_2BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_2BAR1, EVE_2BAR1, EVE_2BAR1 }));     //2빠, 2빠, 2빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(04000, EVE_2BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_2BAR1, EVE_2BAR1, EVE_2BAR1 }));     //2빠, 2빠, 2빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(06000, EVE_1BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR1, EVE_1BAR1, EVE_1BAR1, EVE_1BAR1 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(12000, EVE_1BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR1, EVE_1BAR1, EVE_1BAR1, EVE_1BAR1 }));  //1빠, 1빠, 1빠, 1빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(06000, EVE_1BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR1, EVE_1BAR1, EVE_1BAR1, EVE_1BAR1 }));  //1빠, 1빠, 1빠, 1빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(03000, EVE_1BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR1, EVE_1BAR1, EVE_1BAR1, EMPTYT }));     //1빠, 1빠, 1빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(06000, EVE_1BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR1, EVE_1BAR1, EVE_1BAR1, EMPTYT }));     //1빠, 1빠, 1빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(03000, EVE_1BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_1BAR1, EVE_1BAR1, EVE_1BAR1, EMPTYT }));     //1빠, 1빠, 1빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(03000, EVE_1BAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_1BAR1, EVE_1BAR1, EVE_1BAR1 }));     //1빠, 1빠, 1빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(06000, EVE_1BAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_1BAR1, EVE_1BAR1, EVE_1BAR1 }));     //1빠, 1빠, 1빠
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(03000, EVE_1BAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_1BAR1, EVE_1BAR1, EVE_1BAR1 }));     //1빠, 1빠, 1빠

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(05000, EVE_MUSIC1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(10000, EVE_MUSIC1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1 }));  //뮤직, 뮤직, 뮤직, 뮤직
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(05000, EVE_MUSIC1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1 }));  //뮤직, 뮤직, 뮤직, 뮤직

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_MUSIC1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1, EMPTYT }));  //뮤직, 뮤직, 뮤직
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_MUSIC1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1, EMPTYT }));  //뮤직, 뮤직, 뮤직
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_MUSIC1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1, EMPTYT }));  //뮤직, 뮤직, 뮤직

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_MUSIC1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1 }));  //뮤직, 뮤직, 뮤직
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_MUSIC1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1 }));  //뮤직, 뮤직, 뮤직
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_MUSIC1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_MUSIC1, EVE_MUSIC1, EVE_MUSIC1 }));  //뮤직, 뮤직, 뮤직

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(05000, EVE_STAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR1, EVE_STAR1, EVE_STAR1, EVE_STAR1 }));  //스타, 스타, 스타, 스타
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(10000, EVE_STAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR1, EVE_STAR1, EVE_STAR1, EVE_STAR1 }));  //스타, 스타, 스타, 스타
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(05000, EVE_STAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR1, EVE_STAR1, EVE_STAR1, EVE_STAR1 }));  //스타, 스타, 스타, 스타

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_STAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR1, EVE_STAR1, EVE_STAR1, EMPTYT }));  //스타, 스타, 스타
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_STAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR1, EVE_STAR1, EVE_STAR1, EMPTYT }));  //스타, 스타, 스타
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_STAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EVE_STAR1, EVE_STAR1, EVE_STAR1, EMPTYT }));  //스타, 스타, 스타

                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_STAR1, TOP, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_STAR1, EVE_STAR1, EVE_STAR1 }));  //스타, 스타, 스타
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_STAR1, MID, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_STAR1, EVE_STAR1, EVE_STAR1 }));  //스타, 스타, 스타
                    m_lstGDCBieroEvent.Add(new CGDCScoreInfo(00400, EVE_STAR1, BOT, m_nlstMulti0[i], m_nlstMulti1[j], new int[] { EMPTYT, EVE_STAR1, EVE_STAR1, EVE_STAR1 }));  //스타, 스타, 스타
                }
            }

            #endregion

            #region 3개짜리 이벤트배렬생성
            for (int i = 0; i < m_nlstMulti0.Length; i++)
            {
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(16000, EVE_4BAR, TOP, m_nlstMulti0[i], 1, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EMPTYT }));         //4빠, 4빠, 4빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(32000, EVE_4BAR, MID, m_nlstMulti0[i], 1, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EMPTYT }));         //4빠, 4빠, 4빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(16000, EVE_4BAR, BOT, m_nlstMulti0[i], 1, new int[] { EVE_4BAR, EVE_4BAR, EVE_4BAR, EMPTYT }));         //4빠, 4빠, 4빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(16000, EVE_4BAR, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_4BAR, EVE_4BAR, EVE_4BAR }));         //4빠, 4빠, 4빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(32000, EVE_4BAR, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_4BAR, EVE_4BAR, EVE_4BAR }));         //4빠, 4빠, 4빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(16000, EVE_4BAR, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_4BAR, EVE_4BAR, EVE_4BAR }));         //4빠, 4빠, 4빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(08000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EMPTYT }));     //3빠, 3빠, 3빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(16000, EVE_3BAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EMPTYT }));     //3빠, 3빠, 3빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(08000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_3BAR0, EVE_3BAR0, EVE_3BAR0, EMPTYT }));     //3빠, 3빠, 3빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(08000, EVE_3BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));     //3빠, 3빠, 3빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(16000, EVE_3BAR0, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));     //3빠, 3빠, 3빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(08000, EVE_3BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_3BAR0, EVE_3BAR0, EVE_3BAR0 }));     //3빠, 3빠, 3빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(04000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EMPTYT }));     //2빠, 2빠, 2빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(08000, EVE_2BAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EMPTYT }));     //2빠, 2빠, 2빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(04000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_2BAR0, EVE_2BAR0, EVE_2BAR0, EMPTYT }));     //2빠, 2빠, 2빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(04000, EVE_2BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));     //2빠, 2빠, 2빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(08000, EVE_2BAR0, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));     //2빠, 2빠, 2빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(04000, EVE_2BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_2BAR0, EVE_2BAR0, EVE_2BAR0 }));     //2빠, 2빠, 2빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EMPTYT }));     //1빠, 1빠, 1빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(06000, EVE_1BAR0, MID, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EMPTYT }));     //1빠, 1빠, 1빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EVE_1BAR0, EVE_1BAR0, EVE_1BAR0, EMPTYT }));     //1빠, 1빠, 1빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, EVE_1BAR0, TOP, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));     //1빠, 1빠, 1빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(06000, EVE_1BAR0, MID, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));     //1빠, 1빠, 1빠
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, EVE_1BAR0, BOT, m_nlstMulti0[i], 1, new int[] { EMPTYT, EVE_1BAR0, EVE_1BAR0, EVE_1BAR0 }));     //1빠, 1빠, 1빠

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, BUTTER, TOP, m_nlstMulti0[i], new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }, 1));       //나비 X 3
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(06000, BUTTER, MID, m_nlstMulti0[i], new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }, 1));       //나비 X 3
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, BUTTER, BOT, m_nlstMulti0[i], new int[] { BUTTER, BUTTER, BUTTER, EMPTYT }, 1));       //나비 X 3
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, BUTTER, TOP, m_nlstMulti0[i], new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }, 1));       //나비 X 3
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(06000, BUTTER, MID, m_nlstMulti0[i], new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }, 1));       //나비 X 3
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(03000, BUTTER, BOT, m_nlstMulti0[i], new int[] { EMPTYT, BUTTER, BUTTER, BUTTER }, 1));       //나비 X 3

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(05000, HEART, TOP, m_nlstMulti0[i], new int[] { HEART, HEART, HEART, HEART }, 1));            //하트, 하트, 하트, 하트
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(10000, HEART, MID, m_nlstMulti0[i], new int[] { HEART, HEART, HEART, HEART }, 1));            //하트, 하트, 하트, 하트
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(05000, HEART, BOT, m_nlstMulti0[i], new int[] { HEART, HEART, HEART, HEART }, 1));            //하트, 하트, 하트, 하트

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(02500, HEART, TOP, m_nlstMulti0[i], new int[] { HEART, HEART, HEART, EMPTYT }, 1));           //하트, 하트, 하트
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(05000, HEART, MID, m_nlstMulti0[i], new int[] { HEART, HEART, HEART, EMPTYT }, 1));           //하트, 하트, 하트
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(02500, HEART, BOT, m_nlstMulti0[i], new int[] { HEART, HEART, HEART, EMPTYT }, 1));           //하트, 하트, 하트

                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(02500, HEART, TOP, m_nlstMulti0[i], new int[] { EMPTYT, HEART, HEART, HEART }, 1));           //하트, 하트, 하트
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(05000, HEART, MID, m_nlstMulti0[i], new int[] { EMPTYT, HEART, HEART, HEART }, 1));           //하트, 하트, 하트
                m_lstGDCRaise1Jack.Add(new CGDCScoreInfo(02500, HEART, BOT, m_nlstMulti0[i], new int[] { EMPTYT, HEART, HEART, HEART }, 1));           //하트, 하트, 하트
            }
            #endregion

            foreach (CGDCScoreInfo scoreInfo in m_lstGDCScoreInfo)
            {
                if(!m_lstGDCScoreCash.Exists(value=>value == scoreInfo.m_nScore))
                    m_lstGDCScoreCash.Add(scoreInfo.m_nScore);
            }

            m_lstJackpotInfo.Add(new CGDCJackpotInfo(JACK_GJOKER, 100 * 10000, 350 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(JACK_HJOKER, 100 * 10000, 350 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(PRIZE_GJOKER, 3 * 10000, 45 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(PRIZE_HJOKER, 12 * 10000, 100 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(PRIZE_BUTTER, 2 * 10000, 90 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(PRIZE_RCASTLE, 2 * 10000, 90 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(PRIZE_WCASTLE, 2 * 10000, 90 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(PRIZE_HOOK, 2 * 10000, 30 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(EVENT_HOOK, 2 * 10000, 50 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(BUTTER_JOKER, 10000, 70 * 10000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(LARGE_RACORN, 20000, 25000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(LARGE_WACORN, 20000, 25000));
            m_lstJackpotInfo.Add(new CGDCJackpotInfo(LARGE_BEAM, 20000, 25000));

            return;
        }
    }
}
