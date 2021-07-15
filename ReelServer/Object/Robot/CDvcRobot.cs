using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CRobot
    {
        public const int DVC_START = 0x01;
        public const int DVC_END = 0x02;
        public const int DVC_NIGHT = 0x03;
        public const int DVC_DAY = 0x04;
        public const int DVC_DAYA = 0x05;
        public const int DVC_AIRPLANE = 0x06;
        public const int DVC_EARTH = 0x07;
        public const int DVC_MOZARINA = 0x08;
        public const int DVC_BAT = 0x09;
        public const int DVC_FBIRD = 0x0A;

        public void PlayDvcPrize()
        {
            Thread thr = new Thread(ProcessDvcPrize);
            thr.Start();
        }

        private void ProcessDvcPrize()
        {

            CDvcGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as CDvcGear;
            if (clsGear == null)
                return;

            m_nJackpot = 1;
            int nGameCode = clsGear.m_nGameCode;
            CDataBase.SaveRobotInfoToDB(this);

            CDVCPrizeInfo prizeInfo = clsGear.GetPrizeInfo();
            if (prizeInfo == null)
                return;

            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<CDVCScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                string strMessage = string.Empty;

                CDVCScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                switch (nCmd)
                {
                    //case DVC_DAYA:
                    //    strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]물방울[-] 출현을 축하드립니다. 대박 나세요..";
                    //    break;
                    //case DVC_AIRPLANE:
                    //    strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]플랑크톤[-] 출현을 축하드립니다. 대박 나세요..";
                    //    break;
                    case DVC_EARTH:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]지구본[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case DVC_MOZARINA:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]모나리자[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case DVC_BAT:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]황금박쥐[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case DVC_FBIRD:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]불새[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case DVC_END:
                        if (prizeInfo.m_nCont > 2 && prizeInfo.m_nPrizeCash > 0)
                        {
                            string[] strCont = { "", "보석 ", "글라이더 ", "지구본 ", "모나리자 ", "황금박쥐 ", "불새 " };
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
