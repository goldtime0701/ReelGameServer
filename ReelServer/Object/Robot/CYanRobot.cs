using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CRobot
    {
        public const int YAN_START = 0x01;
        public const int YAN_NIGHT = 0x02;
        public const int YAN_DAY = 0x03;
        public const int YAN_END = 0x04;
        public const int YAN_THUNDER = 0x05;
        public const int YAN_RAINBOW = 0x06;
        public const int YAN_TURTLE = 0x07;
        public const int YAN_BDRAGON = 0x08;
        public const int YAN_GDRAGON = 0x09;


        public void PlayYanPrize()
        {
            Thread thr = new Thread(ProcessYanPrize);
            thr.Start();
        }

        private void ProcessYanPrize()
        {

            CYanGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as CYanGear;
            if (clsGear == null)
                return;

            m_nJackpot = 1;
            int nGameCode = clsGear.m_nGameCode;
            CDataBase.SaveRobotInfoToDB(this);

            CYANPrizeInfo prizeInfo = clsGear.GetPrizeInfo();
            if (prizeInfo == null)
                return;

            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<CYANScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                string strMessage = string.Empty;

                CYANScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                switch (nCmd)
                {
                    case YAN_TURTLE:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]거북[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case YAN_BDRAGON:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]청룡[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case YAN_GDRAGON:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]금룡[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case YAN_END:
                        if (prizeInfo.m_nCont > 2 && prizeInfo.m_nPrizeCash > 0)
                        {
                            string[] strCont = { "", "번개 ", "무지개 ", "거북 ", "청룡 ", "금룡 " };
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
