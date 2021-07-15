using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CRobot
    {
        private const int PRIZE_START = 0x01;
        private const int WATER_BALL = 0x02;
        private const int WATER_FLOWER = 0x03;
        private const int SUBMARINE = 0x04;
        private const int BACKFISH = 0x05;
        private const int ANIMATIC = 0x06;
        private const int SHARK = 0x07;
        private const int PRIZE_END = 0x08;

        public void PlayOcaPrize()
        {
            Thread thr = new Thread(ProcessOcaPrize);
            thr.Start();
        }

        private void ProcessOcaPrize()
        {

            COcaGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as COcaGear;
            if (clsGear == null)
                return;

            m_nJackpot = 1;
            int nGameCode = clsGear.m_nGameCode;
            CDataBase.SaveRobotInfoToDB(this);

            COCAPrizeInfo prizeInfo = clsGear.GetPrizeInfo();
            if (prizeInfo == null)
                return;

            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<COCAScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                string strMessage = string.Empty;

                COCAScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                switch (nCmd)
                {
                    //case 0x02:
                    //    strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]물방울[-] 출현을 축하드립니다. 대박 나세요..";
                    //    break;
                    //case 0x03:
                    //    strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]플랑크톤[-] 출현을 축하드립니다. 대박 나세요..";
                    //    break;
                    case SUBMARINE:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]잠수함[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case BACKFISH:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]열대어[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case ANIMATIC:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]가오리[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case SHARK:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]백상어[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case PRIZE_END:
                        if(prizeInfo.m_nCont > 2 && prizeInfo.m_nPrizeCash > 0)
                        {
                            string[] strCont = { "", "물방울 ", "플랑크톤 ", "잠수함 ", "열대어 ", "가오리 ", "백상어 " };
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
