using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlayYmtPrize()
        {
            CYmtGear gear = CGlobal.GetGearByCode(m_nGearCode) as CYmtGear;
            if (gear == null)
                return;

            Thread thr = new Thread(ProcessYmtPrize);
            thr.Start();
        }

        private void ProcessYmtPrize()
        {
            CYmtGear gear = CGlobal.GetGearByCode(m_nGearCode) as CYmtGear;
            m_nJackpot = 1;
            CDataBase.SaveRobotInfoToDB(this);

            List<CYMTScoreInfo> lstPrizeInfo = gear.GetPrizeInfoList();
            gear.m_nLstGraph[0] = gear.m_nRaiseCount;

            while(lstPrizeInfo.Count > 0)
            {
                CYMTScoreInfo scoreInfo = lstPrizeInfo[0];
                lstPrizeInfo.Remove(scoreInfo);

                if(scoreInfo.m_nScore > 0)
                {
                    for (int i = 9; i >= 1; i--)
                    {
                        gear.m_nLstGraph[i] = gear.m_nLstGraph[i - 1];
                    }
                    gear.m_nLstGraph[0] = 0;
                    gear.m_nYmtRound = 0;
                    gear.m_nRaiseCount = CGlobal._RND.Next(50, 1200);
                    gear.m_nMajorCount++;
                }
                else
                {
                    gear.m_nLstGraph[0]++;
                    gear.m_nYmtRound++;
                }
            }
            int nPrizeCash = gear.GetPrizeInfo().m_nPrizeCash;

            gear.OnBroadCastPrizeInfo();
            gear.ClearPrizeInfo();
            EndRobotPrize(nPrizeCash);
        }
    }
}
