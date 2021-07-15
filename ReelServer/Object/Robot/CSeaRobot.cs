using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlaySeaPrize()
        {
            Thread thr = new Thread(ProcessSeaPrize);
            thr.Start();
        }

        private void ProcessSeaPrize()
        {
            
            CSeaGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as CSeaGear;
            if (clsGear == null)
                return;

            m_nJackpot = 1;
            int nGameCode = clsGear.m_nGameCode;
            CDataBase.SaveRobotInfoToDB(this);

            CSEAPrizeInfo prizeInfo = clsGear.GetSEAJackpotInfo();
            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<CSEAScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                string strMessage = string.Empty;

                CSEAScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                switch (nCmd)
                {
                    case 3:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]해파리[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case 4:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]상어[-] 출현을 축하드립니다. 대박 나세요..";
                        break;
                    case 5:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]고래[-] 출현을 축하드립니다. 대박 나세요..";
                        break;

                    case 9:
                        if(clsGear.GetPrizeInfo().m_nCont > 0 && clsGear.GetPrizeInfo().m_nCont < 4)
                        {
                            string[] strCont = { "거부기잭팟 ", "해파리잭팟 ", "상어잭팟 ", "고래잭팟 " };
                            strMessage = clsGear.MakeCongration(strCont[clsGear.GetPrizeInfo().m_nCont], clsGear.GetPrizeInfo().m_nPrizeCash);
                            CGlobal.CalculateJackPotCash(prizeInfo.m_nPrizeCash);
                        }
                        
                        break;
                }

                if(strMessage != string.Empty)
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
