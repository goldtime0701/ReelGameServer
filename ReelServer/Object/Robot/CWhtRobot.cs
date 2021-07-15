using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CRobot
    {
        public const int WHT_START = 0x01;
        public const int WHT_END = 0x02;
        public const int WHT_NIGHT = 0x03;
        public const int WHT_DAY = 0x04;
        public const int WHT_URCHIN = 0x05;
        public const int WHT_CUTTLE = 0x06;
        public const int WHT_TUNA = 0x07;
        public const int WHT_SHARK = 0x08;
        public const int WHT_WHALE = 0x09;


        public void PlayWhtPrize()
        {
            Thread thr = new Thread(ProcessWhtPrize);
            thr.Start();
        }

        private void ProcessWhtPrize()
        {

            CWhtGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as CWhtGear;
            if (clsGear == null)
                return;

            m_nJackpot = 1;
            int nGameCode = clsGear.m_nGameCode;
            CDataBase.SaveRobotInfoToDB(this);

            CWHTPrizeInfo prizeInfo = clsGear.GetPrizeInfo();
            if (prizeInfo == null)
                return;

            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<CWHTScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                string strMessage = string.Empty;

                CWHTScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                switch (nCmd)
                {
                    //case DVC_DAYA:
                    //    strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]물방울[-] 출현을 축하드립니다. 대박 나세요..";
                    //    break;
                    case WHT_CUTTLE:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]오징어[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case WHT_TUNA:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]날치[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case WHT_SHARK:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]상어[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case WHT_WHALE:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]고래[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    
                    case WHT_END:
                        if (prizeInfo.m_nCont > 1 && prizeInfo.m_nPrizeCash > 0)
                        {
                            string[] strCont = { "", "성게 ", "오징어 ", "날치 ", "상어 ", "고래 " };
                            strMessage = clsGear.MakeCongration(strCont[prizeInfo.m_nCont], prizeInfo.m_nPrizeCash);
                            CGlobal.CalculateJackPotCash(prizeInfo.m_nPrizeCash);
                        }

                        break;
                }

                if (strMessage != string.Empty)
                    CGlobal.SendNoticeBroadCast(strMessage);

                lstPrizeScoreList.Remove(scoreInfo);
                Thread.Sleep(10000);
            }
            clsGear.OnBroadCastPrizeInfo();
            clsGear.ClearPrizeInfo();
            EndRobotPrize(nPrizeCash);
        }
    }
}
