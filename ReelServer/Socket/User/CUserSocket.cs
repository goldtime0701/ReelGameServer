using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace ReelServer
{
    public partial class CUserSocket : WebSocketBehavior
    {
        private CUser m_clsUser;
        private bool m_bApp;            //앱통신소켓인가를 검사한다.
        public int m_nGameCode;        //Web 방식에서 통신게임종류를 나타낸다.

        public CUser GetUserObject()
        {
            return m_clsUser;
        }

        public void SetUserObject(CUser user)
        {
            m_clsUser = user;
        }

        public bool IsApp()
        {
            return m_bApp;
        }


        protected override void OnOpen()
        {
            CGlobal.Log("새로운 유저 접속대기중...");
            base.OnOpen();
        }


        protected override void OnClose(CloseEventArgs e)
        {
            if (m_clsUser != null)
            {
                if (e.Code == 1000)
                {
                    CGlobal.Log(m_clsUser.m_strUserNick + " 유저 접속해제");
                    if (m_nGameCode == 0)
                    {
                        //앱방식일때이다.
                        m_clsUser.UserLogout();
                    }
                    else
                    {
                        //웹방식일때이다.
                        m_clsUser.UserLogout(m_nGameCode);
                    }
                }
                else
                {
                    if (m_nGameCode == 0)
                    {
                        //앱방식일때이다.
                        m_clsUser.StartReconnectWait();
                        CGlobal.Log(m_clsUser.m_strUserNick + " 유저 통신오유");
                    }
                    else
                    {
                        //웹방식일때이다.
                        m_clsUser.UserLogout(m_nGameCode);
                    }
                }

                m_clsUser = null;
            }
            CGlobal.RemoveUserSocket(this);
            CGlobal.RemoveBroadCastSocket(this);
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            if (m_clsUser != null)
            {
                if (m_nGameCode == 0)
                {
                    //앱방식일때이다.
                    m_clsUser.StartReconnectWait();
                    CGlobal.Log(m_clsUser.m_strUserNick + " 유저 통신오유");
                }
                else
                {
                    //웹방식일때이다.
                    //m_clsUser.UserLogout(m_nGameCode);
                }

                //m_clsUser = null;
            }
            else
            {
                CGlobal.Log("Exception: 서버 - 유저 소켓통신 오유");
            }
            CGlobal.RemoveUserSocket(this);
            CGlobal.RemoveBroadCastSocket(this);
            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            string strPacket = string.Empty;
            if (e.IsBinary)
            {
                strPacket = System.Text.Encoding.UTF8.GetString(e.RawData);
            }
            else
            {
                strPacket = e.Data.ToString();
            }
            strPacket = CEncrypt.Decrypt(strPacket).Trim();


            if (strPacket == "ReelServer")
            {
                CGlobal.AddBroadCastSocket(this);
            }
            else
            {
                //try
                {
                    CPacket pktRecvData = JsonConvert.DeserializeObject<CPacket>(strPacket);
                    RecvParsing(pktRecvData);
                }
                //catch(Exception ex)
                {
                    //CGlobal.Log(ex.Message);
                }
                
            }

            base.OnMessage(e);
        }

        protected void SendPacket(string strPacket)
        {
            strPacket = CEncrypt.Encrypt(strPacket);
            try
            {
                if (this.State == WebSocketState.Open)
                    Send(strPacket);
            }
            catch
            {

            }
        }

        public void UserBroadCastPacket(string strPacket)
        {
            strPacket = CEncrypt.Encrypt(strPacket);
            try
            {
                if (this.State == WebSocketSharp.WebSocketState.Open)
                    this.Sessions.Broadcast(strPacket);
            }
            catch
            {

            }
        }
    }

    public static class CUserServer
    {
        public static void Start()
        {
            foreach(int nPort in CDefine.SRV_USER_SOCKET_PORT)
            {
                WebSocketServer wssv = new WebSocketServer(nPort);
                wssv.AddWebSocketService<CUserSocket>("/");
                wssv.Start();
                CGlobal.Log("유저 소켓 접속포트 " + nPort);
                WebSocket ws = new WebSocket("ws://127.0.0.1:" + nPort);
                ws.OnOpen += Ws_OnOpen;
                ws.OnError += Ws_OnError;
                ws.OnClose += Ws_OnClose;
                ws.Connect();
                Thread.Sleep(200);
            }
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
            string strPacket = CEncrypt.Encrypt("ReelServer");
            (sender as WebSocket).Send(strPacket);
        }
    }
}
