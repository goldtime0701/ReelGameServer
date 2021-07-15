using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace ReelServer
{
    public partial class CAdminSocket
    {
        public void SendPacket(CPacket clsPacket)
        {
            string strPacket = JsonConvert.SerializeObject(clsPacket);
            SendPacket(strPacket);
        }

        public void SendUserInfo(CUser clsUser)
        {
            CPacket clsPacket = new CPacket();
            clsPacket.nPktHeader = CDefine.PKT_HEADER_O2S_CHAT_USERINFO;
            CUserModel clsUserModel = clsUser.GetUserModel();
            clsPacket.strPktData = JsonConvert.SerializeObject(clsUserModel);

            AdminBroadCastPacket(JsonConvert.SerializeObject(clsPacket));
        }

        public void BroadCastTimeToChatApp()
        {
            CPacket clsPacket = new CPacket();
            clsPacket.nPktHeader = CDefine.PKT_HEADER_O2S_CHAT_SERVERTIME;

            while (true)
            {
                clsPacket.strPktData = CMyTime.GetMyTimeStr();
                AdminBroadCastPacket(JsonConvert.SerializeObject(clsPacket));
                Thread.Sleep(1000);
            }
        }

        public void SendSecurityChat(CSChatPacket chatPacket)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_O2S_CHAT_SECURITY);
            sendPktData.strPktData = JsonConvert.SerializeObject(chatPacket);

            Sessions.Broadcast(JsonConvert.SerializeObject(sendPktData));
        }

        public void SendGroupChat(CGChatPacket chatPacket)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_O2S_CHAT_GROUP);
                        sendPktData.strPktData = JsonConvert.SerializeObject(chatPacket);

            AdminBroadCastPacket(JsonConvert.SerializeObject(sendPktData));
        }

        public void StartRealTimeBoardCast(CJackPot clsJackpot)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2A_GLOBAL_REAL_BOARDCAST);
            CAdminRealPacket adminRealPacket = new CAdminRealPacket();
            adminRealPacket.m_clsJackPot = clsJackpot;
            adminRealPacket.m_lstTotalPot = new List<CTotalPot>();
            adminRealPacket.m_lstGivePot = new List<CGivePot>();
            for(int i=0; i<CGlobal._lstGameEngine.Count; i++)
            {
                adminRealPacket.m_lstTotalPot.Add(CGlobal._lstGameEngine[i].m_clsTotalPot);
                adminRealPacket.m_lstGivePot.Add(CGlobal._lstGameEngine[i].m_clsGivePot);
            }

            pktSendData.strPktData = JsonConvert.SerializeObject(adminRealPacket);
            string strPacket = JsonConvert.SerializeObject(pktSendData);

            AdminBroadCastPacket(strPacket);
        }

        public void SendRobotInfo(CRobot robot)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_O2S_CHAT_ROBOTINFO);
            sendPktData.strPktData = JsonConvert.SerializeObject(robot);

            AdminBroadCastPacket(JsonConvert.SerializeObject(sendPktData));
        }

    }
}
