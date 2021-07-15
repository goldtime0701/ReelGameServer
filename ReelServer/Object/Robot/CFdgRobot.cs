using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public partial class CRobot
    {
        public void PlayFdgPrize(int nPrizeCash)
        {
            CFdgGear gear = CGlobal.GetGearByCode(m_nGearCode) as CFdgGear;
            if (gear == null)
                return;

            string strMessage = gear.MakeCongration("드래곤잭팟", nPrizeCash * 10);
            CGlobal.SendNoticeBroadCast(strMessage);

            gear.m_nAccuCash -= nPrizeCash;
            gear.m_nLastJackCash = nPrizeCash;
            gear.m_dtLastJackTime = CMyTime.GetMyTime();
            gear.m_nGearJack = 0;

            //gear.m_nJackCash += nPrizeCash;
            if (gear.m_nTopJackCash < nPrizeCash)
                gear.m_nTopJackCash = nPrizeCash;
        }
    }
}
