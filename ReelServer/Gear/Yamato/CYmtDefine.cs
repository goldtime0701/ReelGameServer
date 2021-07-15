using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CYMTPrizeInfo
    {
        public int m_nCont;         //
        public int m_nPrizeCash;    //
    }

    public class CYMTScoreInfo
    {
        public int m_nGearCode;
        public int m_nScore;
        public int m_nCmd;
        public int m_nWinTile;
        public List<CTile> m_lstTile;
        public int m_nLine;
        public string m_strFileName;
    }


    public partial class CYmtGear
    {
        private const int PRIZE_SP = 0x01;
        private const int PRiZE_CON = 0x02;

        public override void InitScoreTableInfo()
        {
            return;
        }

    }
}
