using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlayNwdPrize()
        {
            Thread thr = new Thread(ProcessNwdPrize);
            thr.Start();
        }

        private void ProcessNwdPrize()
        {

            CNwdGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as CNwdGear;
            if (clsGear == null)
                return;

            CNWDPrizeInfo prizeInfo = clsGear.GetPrizeInfo();
            if (prizeInfo == null)
                return;

            m_nJackpot = 1;
            CDataBase.SaveRobotInfoToDB(this);

            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<CNWDScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                string strMessage = string.Empty;

                CNWDScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                switch (nCmd)
                {
                    case CNwdGear.PRIZE_FBIRD:
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]불새[-] 출현을 축하드립니다. 대박 나세요..";
                        Thread.Sleep(30 * 1000);
                        break;
                    case CNwdGear.PRIZE_GIRL:
                        Thread.Sleep(30 * 1000);
                        strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]여신[-] 출현을 축하드립니다. 대박 나세요..";
                        break;

                    case CNwdGear.PRIZE_END:
                        if(prizeInfo.m_nPrizeCash > 0)
                        {
                            string[] strCont = { "", "불새", "여신" };
                            strMessage = clsGear.MakeCongration(strCont[prizeInfo.m_nCont], prizeInfo.m_nPrizeCash);
                            CGlobal.CalculateJackPotCash(prizeInfo.m_nPrizeCash);
                        }
                        
                        break;
                }

                if (strMessage != string.Empty)
                    CGlobal.SendNoticeBroadCast(strMessage);

                lstPrizeScoreList.Remove(scoreInfo);
                Thread.Sleep(7000);
            }
            clsGear.OnBroadCastPrizeInfo();
            clsGear.ClearPrizeInfo();
            EndRobotPrize(nPrizeCash);
        }
    }
}
