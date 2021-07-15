using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CTile
    {
        public int m_nTile;
        public int m_nRow;
        public int m_nCol;
        public int m_nAct;          //5드래곤에서만 사용(당첨된 타일인가? 0-당첨안됨, 1-당첨타일1, 2-당첨타일2, 3-당첨타일3, 4-모든점수당첨에 다 속하는 타일)
                                    //황금성에서는 0-당첨안된타일, 1-당첨된타일, 2-나비로 변환해야 하는 타일

        public CTile(int nTile, int nRow, int nCol)
        {
            m_nTile = nTile;
            m_nRow = nRow;
            m_nCol = nCol;
        }
    }

    public class CTotalPot
    {
        public int m_nGameCode;

        public int m_nFirst;
        public int m_nSecond;
        public int m_nThird;


        public CTotalPot(int nGameCode)
        {
            m_nGameCode = nGameCode;
        }
    }

    public class CGivePot
    {
        public int m_nGameCode;

        public long m_nZero;
        public int m_nFirst;
        public int m_nSecond;
        public int m_nThird;

        public long m_nRealCash;    //수익금
        public long m_nGiveCash;    //지금까지 유저에게 돌려준 금액
        public long m_nAppenCash;   //지금까지 기대에 넣은 금액
        public int m_nOnerPrizeCall; //강제콜 남은점수


        public CGivePot(int nGameCode)
        {
            m_nGameCode = nGameCode;
        }
    }



    public class CTotalStep
    {
        public int m_nPrizeCash;
        public int m_nRaiseCash;
    }


    public class CJackPot
    {
        public int m_nGrand = 0;
        public int m_nMajor = 0;
        public int m_nMinor = 0;
        public int m_nMini = 0;
    }

    public class CBonusVirtual
    {
        public int m_nVirtual = 0;                //첫회원가입시 체험머니(쿠폰)을 주겠는가
        public int m_nVirtualCash = 0;            //첫회원가입시 체험캐시
        public int m_nVirtualCupn = 0;            //첫회원가입시 체험쿠폰

        public int m_nBonus = 0;                  //첫충시 보너스를 주겠는가
        public double m_nBonusPro = 0.0;          //첫충시 주는 보너스 프로
        public int m_nBonusCupn = 0;              //첫층시 주는 보너스 개수

        public int m_nAutoExcharge;                //자동으로 가상실시간 출금현황을 생성하겠는가
    }

    public class CAbsent
    {
        public int m_nCheckCount = 0;
        public int m_nGiveCupn = 0;
        public int m_nPreUseCash = 0;
    }

    public class CAdminRealPacket
    {
        public CJackPot m_clsJackPot;
        public List<CTotalPot> m_lstTotalPot;
        public List<CGivePot> m_lstGivePot;
    }

    public class CAutoExcharge
    {
        public int m_nRbCode;
        public int m_nExCash;
        public int m_nHour;
        public int m_nMinute;
        public int m_nSecond;
    }

    public class CUserCash
    {
        public int nUserCash;
        public int nUserCupn;
        public int nVirtualCash;
        public int nVirtualCupn;
    }
}
