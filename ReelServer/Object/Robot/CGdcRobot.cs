using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlayGdcPrize()
        {
            Thread thr = new Thread(ProcessGdcPrize);
            thr.Start();
        }

        private void ProcessGdcPrize()
        {
            CGdcGear clsGear = CGlobal.GetGearByCode(m_nGearCode) as CGdcGear;
            if (clsGear == null)
                return;

            m_nJackpot = 1;
            int nGameCode = clsGear.m_nGameCode;
            CDataBase.SaveRobotInfoToDB(this);

            CGDCPrizeInfo prizeInfo = clsGear.GetPrizeInfo();
            int nPrizeCash = prizeInfo.m_nPrizeCash;

            List<CGDCScoreInfo> lstPrizeScoreList = clsGear.GetPrizeScoreInfoList();
            string strUserNick = CGlobal.GetRobotNickByCode(m_nRbCode);
            string strMessage = string.Empty;

            while (lstPrizeScoreList.Count > 0)
            {
                strMessage = string.Empty;
                CGDCScoreInfo scoreInfo = lstPrizeScoreList[0];
                int nCmd = scoreInfo.m_nCmd;
                if(nCmd > 0)
                {
                    switch (nCmd)
                    {
                        case CGdcGear.JACK_GJOKER:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]잭팟이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.JACK_HJOKER:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]잭팟이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.PRIZE_GJOKER:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]조커보너스[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.PRIZE_HJOKER:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]조커보너스[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.PRIZE_BUTTER:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]요정보너스[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.PRIZE_RCASTLE:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]성보너스[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.PRIZE_WCASTLE:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]성보너스[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.PRIZE_HOOK:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]선장보너스[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.EVENT_HOOK:
                            if (scoreInfo.m_nEvent == CGdcGear.EVE_3BAR1 || scoreInfo.m_nEvent == CGdcGear.EVE_2BAR1 || scoreInfo.m_nEvent == CGdcGear.EVE_1BAR1 || scoreInfo.m_nEvent == CGdcGear.EVE_MUSIC1 || scoreInfo.m_nEvent == CGdcGear.EVE_STAR1)
                                strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]선장이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CGdcGear.BUTTER_JOKER:
                            strMessage = "[C][FFFFFF]운영자: " + strUserNick + " 님 [C][FFFF00]조커이벤트[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                    }
                }


                if (strMessage != string.Empty)
                    CGlobal.SendNoticeBroadCast(strMessage);

                lstPrizeScoreList.Remove(scoreInfo);
                Thread.Sleep(7000);
            }
            while(prizeInfo.m_nRemCash > 0)
            {
                int nScore = CGlobal.Random(10, 30) * 1000;
                prizeInfo.m_nRemCash -= nScore;
                Thread.Sleep(10000);
            }

            List<CGDCScoreInfo> lstEndInfo = clsGear.GetEndPrizeInfoList();
            if(lstEndInfo != null)
            {
                while (lstEndInfo.Count > 0)
                {
                    lstEndInfo.RemoveAt(0);
                    Thread.Sleep(7000);
                }
            }

            string[] lstPrizeKind = { "", "잭팟이벤트", "잭팟이벤트", "조커보너스", "조커보너스", "요정보너스", "성보너스", "성보너스", "선장보너스", "", "", "선장이벤트", "조커이벤트", "도토리보너스", "도토리보너스", "저울보너스" };
            strMessage = clsGear.MakeCongration(lstPrizeKind[prizeInfo.m_nCont], prizeInfo.m_nPrizeMoney);
            CGlobal.SendNoticeBroadCast(strMessage);

            CGlobal.CalculateJackPotCash(prizeInfo.m_nPrizeCash);
            clsGear.OnBroadCastPrizeInfo();
            clsGear.ClearPrizeInfo();
            CDataBase.SaveGearInfoToDB(clsGear);

            EndRobotPrize(nPrizeCash);
        }

    }
}
