using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlayAld2Prize()
        {
            Thread thr = new Thread(ProcessAld2Prize);
            thr.Start();
        }

        private void ProcessAld2Prize()
        {
            
            CAldGear2 gear = CGlobal.GetGearByCode(m_nGearCode) as CAldGear2;
            if (gear == null)
                return;

            m_nJackpot = 1;
            CALDJackpotInfo prizeInfo = gear.GetAldJackpotInfo();
            int nPrizeCash = prizeInfo.m_nJackCash;

            CDataBase.SaveRobotInfoToDB(this);

            List<CALDScoreInfo> lstPrizeScoreList = gear.GetPrizeScoreInfoList();

            while (lstPrizeScoreList.Count > 0)
            {
                CALDScoreInfo scoreInfo = lstPrizeScoreList[0];
                string strMessage = string.Empty;

                if (scoreInfo.m_nPrizeCmd > 0)
                {
                    switch (scoreInfo.m_nPrizeCmd)
                    {
                        case CAldGear2.ALD_HORSE_ANIMATION:
                            strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]백마[-] 출현을 축하드립니다. 대박 나세요..";
                            CGlobal.SendNoticeBroadCast(strMessage);
                            Thread.Sleep(20 * 1000);
                            strMessage = string.Empty;
                            break;
                        case CAldGear2.ALD_HORSE_WRR_ANIMATION:
                            break;
                        case CAldGear.ALD_HORSE_WWW_ANIMATION:
                            break;
                        case CAldGear2.ALD_HORSE_WRW_ANUMATION:
                            break;
                        case CAldGear2.ALD_YANGT_ANIMATION:
                            if (prizeInfo.m_nJackCont == 4 || prizeInfo.m_nJackCont == 5)
                            {
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][FFFF00]양탄자[-] 출현을 축하드립니다. 대박 나세요..";
                                CGlobal.SendNoticeBroadCast(strMessage);
                                Thread.Sleep(40 * 1000);
                                strMessage = string.Empty;
                            }
                            break;
                        case CAldGear2.ALD_FIRE_SMALL_ANIMATION:
                            if (prizeInfo.m_nJackCont == 2)
                            {
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][D72D8D]작은불[-] 출현을 축하드립니다. 대박 나세요..";
                                CGlobal.SendNoticeBroadCast(strMessage);
                                Thread.Sleep(40 * 1000);
                                strMessage = string.Empty;
                            }

                            break;
                        case CAldGear2.ALD_FIRE_LARGE_ANIMATION:
                            if (prizeInfo.m_nJackCont == 3)
                            {
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][D72D8D]큰불[-] 출현을 축하드립니다. 대박 나세요..";
                                CGlobal.SendNoticeBroadCast(strMessage);
                                Thread.Sleep(30 * 1000);
                                strMessage = string.Empty;
                            }
                            break;
                        case CAldGear2.ALD_FEVER_LOCK_ANIMATION:
                            if (prizeInfo.m_nJackCont == 6 || prizeInfo.m_nJackCont == 7)
                            {
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][D72D8D]휘버[-] 출현을 축하드립니다. 대박 나세요..";
                                CGlobal.SendNoticeBroadCast(strMessage);
                                Thread.Sleep(30 * 1000);
                                strMessage = string.Empty;
                            }

                            break;
                        case CAldGear2.ALD_FEVER_OPEN_ANIMATION:
                            if (prizeInfo.m_nJackCont == 6 || prizeInfo.m_nJackCont == 7)
                            {
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][D72D8D]휘버[-] 출현을 축하드립니다. 대박 나세요..";
                                CGlobal.SendNoticeBroadCast(strMessage);
                                Thread.Sleep(30 * 1000);
                                strMessage = string.Empty;
                            }
                            break;
                        case CAldGear2.ALD_THREE_SEVEN_ANIMATION:
                            if (prizeInfo.m_nJackCont == 8)
                            {
                                strMessage = "[C][FFFFFF]운영자: " + m_strRbNick + " 님 [C][D72D8D]삼칠별타[-] 출현을 축하드립니다. 대박 나세요..";
                            }
                            break;
                        case CAldGear2.PRIZE_END:
                            string[] lstPrizeKind = { "", "백마", "작은불", "큰불", "양탄자", "양탄자", "휘버", "휘버", "삼칠별타", "돌발1", "돌발2", "돌발3" };
                            
                            if (prizeInfo.m_nJackCash > 0)
                            {
                                CGlobal.CalculateJackPotCash(prizeInfo.m_nJackCash);
                                strMessage = gear.MakeCongration(lstPrizeKind[prizeInfo.m_nJackCont], prizeInfo.m_nJackCash);
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
