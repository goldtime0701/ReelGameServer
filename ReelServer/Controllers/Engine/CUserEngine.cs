using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public static class CUserEngine
    {
        public static string UserLogin(string strValue)
        {
            try
            {
                JToken json = JToken.Parse(strValue);
                string userID = Convert.ToString(json["userID"]).Trim();
                string userPW = Convert.ToString(json["userPW"]).Trim();

                CUser clsUser = CGlobal.GetUserByID(userID);
                if (clsUser == null)
                {
                    return "0:아이디가 존재하지 않습니다.";
                }
                if (clsUser.m_strUserPW != userPW)
                {
                    return "0:비번이 정확하지 않습니다.";
                }

                if (clsUser.m_nUserState != 1)
                {
                    return "0:차단된 아이디입니다.";
                }

                if (clsUser.m_nUserLogin == 1)
                {
                    return "0:이미 가입하였습니다.";
                }

                clsUser.m_strToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                CDataBase.SaveUserLogin(clsUser.m_nUserCode);
                string strRet = clsUser.m_nUserCode + ":" + clsUser.m_strToken;

                return strRet;
            }
            catch
            {
                return "0:가입오류입니다.";
            }
        }

        public static string UserSocket()
        {
            int nPort = CDefine.SRV_USER_SOCKET_PORT[CGlobal._nUserSocketIndex];

            CGlobal._nUserSocketIndex++;
            if (CGlobal._nUserSocketIndex >= CDefine.SRV_USER_SOCKET_PORT.Count)
                CGlobal._nUserSocketIndex = 1;

            return nPort.ToString();
        }
    }
}
