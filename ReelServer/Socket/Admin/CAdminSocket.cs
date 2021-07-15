using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace ReelServer
{
    public partial class CAdminSocket : WebSocketBehavior
    {
        public CAdmin m_clsAdmin;
        public CStore m_clsStore;

        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (m_clsAdmin != null)
            {
                ChatLogout();
                m_clsAdmin.m_bOnerLogin = false;
            }

            CGlobal.RemoveAdminPageSocket(this);
            CGlobal.RemoveAdminSocket(this);
            CGlobal.RemoveStorePageSocket(this);
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            if (m_clsAdmin != null)
            {
                ChatLogout();
                m_clsAdmin.m_bOnerLogin = false;
                CGlobal.Log(m_clsAdmin.m_strOnerNick + " 채팅앱 소켓통신 오유");
            }
            else
            {
                CGlobal.Log("Exception: 서버 - 유저 소켓통신 오유");
            }

            CGlobal.RemoveAdminPageSocket(this);
            CGlobal.RemoveAdminSocket(this);
            CGlobal.RemoveStorePageSocket(this);
            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                string strPacket = e.Data.ToString();
                if (strPacket == "ReelServer")
                {
                    CGlobal.InsertAdminSocket(this);
                    new Thread(BroadCastTimeToChatApp).Start();
                }
                else
                {
                    CPacket pktRecvData = JsonConvert.DeserializeObject<CPacket>(strPacket);
                    RecvParsing(pktRecvData);
                }

                base.OnMessage(e);
            }
            catch (Exception err)
            {
                CGlobal.Log(err.Message);
                SendPacket("잘못된 파케트전송");
            }
        }

        protected void SendPacket(string strPacket)
        {
            try
            {
                if (this.State == WebSocketState.Open)
                    Send(strPacket);
            }
            catch
            {
                Console.WriteLine("CAdminSocket.cs - SendPacket function error!");
            }
            
        }

        private void AdminBroadCastPacket(string strPacket)
        {
            try
            {
                if (this.State == WebSocketSharp.WebSocketState.Open)
                    this.Sessions.Broadcast(strPacket);
            }
            catch
            {
                Console.WriteLine("CAdminSocket.cs - BroadCastPacket function error!");
            }
        }
    }

    public static class CAdminServer
    {
        public static void Start()
        {
            WebSocketServer wssv = new WebSocketServer(CDefine.SRV_ADMIN_SOCKET_PORT);
            wssv.AddWebSocketService<CAdminSocket>("/");
            wssv.Start();
            CGlobal.Log("관리자 소켓 접속포트 " + CDefine.SRV_ADMIN_SOCKET_PORT);
            Ws_Connect();
        }

        private static void Ws_Connect()
        {
            WebSocket ws = new WebSocket("ws://127.0.0.1:" + CDefine.SRV_ADMIN_SOCKET_PORT);
            ws.OnOpen += Ws_OnOpen;
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;
            ws.Connect();
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            (sender as WebSocket).Connect();
        }

        private static void Ws_OnError(object sender, ErrorEventArgs e)
        {
            (sender as WebSocket).Close();
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            (sender as WebSocket).Send("ReelServer");
        }
    }
}
