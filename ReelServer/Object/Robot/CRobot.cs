using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot : CRobotModel
    {
        private bool m_bStart;  //로봇이 현재 동작상태인가

        private void EndRobotPrize(int nCash)
        {
            m_nJackpot = 0;
            CDataBase.SaveRobotInfoToDB(this);

            CBaseGear gear = CGlobal.GetGearByCode(m_nGearCode);

            if (gear == null)
                return;

            gear.m_nAccuCash -= nCash;
            gear.m_nLastJackCash = nCash;
            gear.m_dtLastJackTime = CMyTime.GetMyTime();
            gear.m_nGearJack = 0;
            //if (nCash >= 70 * 10000)
            //    gear.m_nGrandCount++;
            //else if (nCash >= 50 * 10000)
            //    gear.m_nMajorCount++;
            //else if (nCash >= 20 * 10000)
            //    gear.m_nMinorCount++;
            //else
            //    gear.m_nMiniCount++;
            //gear.m_nJackCash += nCash;
            if (gear.m_nTopJackCash < nCash)
                gear.m_nTopJackCash = nCash;

            CDataBase.SaveGearInfoToDB(gear);
        }

        public void StartChangeGear()
        {
            if (m_bStart)
                return;

            Thread thr = new Thread(ChangeGear);
            thr.Start();
        }


        //로봇이 기계들을 골고루 선택해서 잭팟내역이 쌓이게 하기 위한 함수이다.
        private void ChangeGear()
        {
            while(m_nAutoJack == 1)
            {
                int nTime = CGlobal.Random(1800, 7200);
                Thread.Sleep(nTime * 1000);

                if (m_nJackpot == 1)
                    continue;
                CBaseGear gear = CGlobal.GetGearByCode(m_nGearCode);

                int nGameCode = 0;
                if (gear != null)
                {
                    if (gear.m_nGearJack == 1)
                        continue;

                    gear.ClearGear();
                    nGameCode = gear.m_nGameCode;
                    gear.m_nTakeRobot = 0;
                    gear.m_nGearState = 0;
                    gear.m_nGearRun = 0;

                    CDataBase.SaveGearInfoToDB(gear);
                }

                if (nGameCode > 0)
                    gear = CGlobal.RandomSelect(CGlobal.GetGearList().FindAll(value => value.m_nGearState == 0 && value.m_nGameCode == nGameCode && value.m_nTakeUser == 0 && value.m_nTakeRobot == 0));
                else
                    gear = CGlobal.RandomSelect(CGlobal.GetGearList().FindAll(value => value.m_nGearState == 0 && value.m_nTakeUser == 0 && value.m_nTakeRobot == 0));

                if (gear == null)
                    continue;

                gear.m_nTakeRobot = m_nRbCode;
                gear.m_nGearState = 1;
                gear.m_nGearRun = 1;
                m_nGameCode = nGameCode;
                m_nGearCode = gear.m_nGearCode;

                CDataBase.SaveGearInfoToDB(gear);
                CDataBase.SaveRobotInfoToDB(this);
            }

            m_bStart = false;
        }
    }
}
