using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public static class CMyTime
    {
        public static DateTime GetMyTime()
        {
            DateTime dtCurrentTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Korea Standard Time");
            return dtCurrentTime;
        }

        public static DateTime AddTime(DateTime srcTime, int D, int H, int m, int s)
        {
            TimeSpan addTime = new TimeSpan(D, H, m, s);
            DateTime dstTime = srcTime + addTime;

            return dstTime;
        }

        public static string GetMyTimeStr(string strFormat = "yyyy-MM-dd HH:mm:ss")
        {
            return GetMyTime().ToString(strFormat);
        }

        public static DateTime ConvertStrToTime(string strTime)
        {
            return DateTime.Parse(strTime);
        }
    }
}
