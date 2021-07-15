using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlaySwkPrize()
        {
            Thread thr = new Thread(ProcessSwkPrize);
            thr.Start();
        }

        private void ProcessSwkPrize()
        {
            
            CSwkGear gear = CGlobal.GetGearByCode(m_nGearCode) as CSwkGear;
            if (gear == null)
                return;

            CSWKJackpotInfo prizeInfo = gear.GetSWKJackpotInfo();
            if (prizeInfo.m_nJackTy == 1)
                return;

            m_nJackpot = 1;

            int nPrizeCash = prizeInfo.m_nJackCash;

            CDataBase.SaveRobotInfoToDB(this);

            List<CSwkScoreInfo> lstPrizeScoreList = gear.GetPrizeScoreInfoList();
            while (lstPrizeScoreList.Count > 0)
            {
                CSwkScoreInfo scoreInfo = lstPrizeScoreList[0];
                string strMessage = string.Empty;

                if (scoreInfo.m_nPrizeCmd > 0)
                {
                    switch (scoreInfo.m_nPrizeCmd)
                    {
                        case CSwkGear.PRIZE_SWK_ANIMATION:
                            strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]손오공[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CSwkGear.PRIZE_JPG_ANIMATION:
                            strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]저팔계[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CSwkGear.PRIZE_SOJ_ANIMATION:
                            strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]사오정[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CSwkGear.PRIZE_SMJ_ANIMATION:
                            strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]삼장법사[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CSwkGear.PRIZE_TSS_ANIMATION:
                            strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]삼칠별타[-] 출현을 축하드립니다. 대박 나세요..";
                            break;
                        case CSwkGear.PRIZE_EVIL_SLIDE:
                            if (gear.GetSWKJackpotInfo().m_nJackCont == 6)
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][D72D8D]제천대성[-] 출현을 축하드립니다. 대박 나세요..";
                            break;

                        case CSwkGear.PRIZE_END:

                            string[] lstPrizeKind = { "", "손오공", "저팔계", "사오정", "삼장법사", "삼칠별타", "제천대성", "제천대성", "제천대성", "제천대성" };

                            int nIdx = 0;
                            if (prizeInfo.m_nJackCont == 6)
                            {
                                if (prizeInfo.m_nJackCash >= 50 * 10000)
                                    nIdx = 6;
                                else if (prizeInfo.m_nJackCash >= 30 * 10000)
                                    nIdx = 7;
                                else if (prizeInfo.m_nJackCash >= 20 * 10000)
                                    nIdx = 8;
                                else
                                    nIdx = 9;
                            }
                            else
                                nIdx = prizeInfo.m_nJackCont;

                            if (prizeInfo.m_nJackCash > 0)
                            {
                                CGlobal.CalculateJackPotCash(prizeInfo.m_nJackCash);
                                strMessage = gear.MakeCongration(lstPrizeKind[nIdx], prizeInfo.m_nJackCash);
                            }

                            break;
                    }

                    if (strMessage != string.Empty)
                    {
                        CGlobal.SendNoticeBroadCast(strMessage);
                    }

                }
                lstPrizeScoreList.Remove(scoreInfo);
                Thread.Sleep(10000);
            }
            gear.OnBroadCastPrizeInfo();
            gear.ClearPrizeInfo();
            EndRobotPrize(nPrizeCash);
        }

    }
}
