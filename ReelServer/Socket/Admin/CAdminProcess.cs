using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ReelServer
{
    public partial class CAdminSocket
    {
        public string m_strValue;       //관리자페지에서 유저리스트검색조건
        public bool m_bSendUserList;    //유저정보를 내려보내도 되는가

        private void AdminAuth(string strPktData)
        {
            string strOnerID = strPktData;
            CAdmin admin = CGlobal.GetAdminByOnerID(strOnerID);
            m_clsAdmin = admin;

            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_AUTH_RESPONE);
            if (m_clsAdmin != null)
            {
                CGlobal.AddAdminPageSocket(this);
                sendPktData.strPktData = "success";
                new Thread(() => StartGameStateSend(this)).Start();
            }
            else
            {
                sendPktData.strPktData = "failed";
            }

            SendPacket(sendPktData);
        }

        private void StoreAuth(string strPktData)
        {
            string strStoreID = strPktData;
            CStore store = CGlobal.GetStoreByID(strStoreID);
            m_clsStore = store;

            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_STOREAUTH_RESPONE);
            if (m_clsStore != null)
            {
                sendPktData.strPktData = "success";
                CGlobal.AddStorePageSocket(this);
                new Thread(() => StartStoreStateSend(this)).Start();
            }
            else
            {
                sendPktData.strPktData = "failed";
            }

            SendPacket(sendPktData);
        }


        public void StartSendUserList(string strValue)
        {
            m_strValue = strValue;
            if (m_bSendUserList == false)
            {
                new Thread(() => StartSendUserList(this)).Start();
                
            }
            
        }

        public void StartSendUserList(CAdminSocket clsSocket)
        {
            int nIdx = 0;
            clsSocket.AdminUserList(clsSocket.m_strValue);

            while (clsSocket.State == WebSocketSharp.WebSocketState.Open)
            {
                if(nIdx == 2)
                {
                    nIdx = 0;
                    clsSocket.AdminUserList(clsSocket.m_strValue);
                }
                nIdx++;
                clsSocket.AdminUserGearList(clsSocket.m_strValue);

                Thread.Sleep(5000);
            }
        }


        private void AdminUserList(string strPktData)
        {
            string[] param = strPktData.Split(":");
            int nPCnt = 50;
            
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_USERLIST_RESPONE);

            try
            {
                string key = param[0];
                string condition = param[1];
                int nKind = Convert.ToInt32(param[2]);
                int nOnline = Convert.ToInt32(param[3]);
                int nUserCode = Convert.ToInt32(param[4]);
                int nStoreCode = Convert.ToInt32(param[5]);
                int nPage = Convert.ToInt32(param[6]) - 1;
                string strApp = string.Empty;
                if(param.Length == 7)
                {
                    strApp = "APP";
                }
                else if(param.Length == 8)
                {
                    strApp = Convert.ToString(param[7]);
                }
                

                List<CUser> lstUser = CGlobal.GetUserList();

                if (strApp == "WEB")
                    lstUser = lstUser.FindAll(value => value.m_nStoreCode > 0);
                else if (strApp == "APP")
                    lstUser = lstUser.FindAll(value => value.m_nAgenCode > 0);
                else
                    return;

                if (nStoreCode > 0)
                    lstUser = lstUser.FindAll(value => value.m_nStoreCode == nStoreCode);

                if (key != string.Empty && condition != string.Empty)
                {
                    if (key == "userID")
                    {
                        lstUser = lstUser.FindAll(user => user != null && user.m_strUserID.Contains(condition) && user.m_nUserState != 3);
                    }
                    else if (key == "userNick")
                    {
                        lstUser = lstUser.FindAll(user => user != null && user.m_strUserNick.Contains(condition) && user.m_nUserState != 3);
                    }
                    else if (key == "agenNick")
                    {
                        lstUser = lstUser.FindAll(user => user != null && CGlobal.GetAgentByCode(user.m_nAgenCode).m_strAgenNick.Contains(condition) && user.m_nUserState != 3);
                    }
                    else if (key == "storeNick")
                    {
                        lstUser = lstUser.FindAll(user => user != null && CGlobal.GetStoreNickByCode(user.m_nStoreCode).Contains(condition) && user.m_nUserState != 3);
                    }
                }
                else
                {
                    lstUser = lstUser.FindAll(user => user != null && user.m_nUserState != 3);
                }


                if (nKind == 4)
                {
                    //신규회원보기
                    string strTime = CMyTime.GetMyTimeStr("yyyy-MM-dd");
                    lstUser = lstUser.FindAll(value => value.m_strUserRegTime.IndexOf(strTime) > -1);
                }
                else if (nKind < 4)
                {
                    lstUser = lstUser.FindAll(value => value.m_nUserState == nKind);
                }

                if (nOnline < 2)
                {
                    lstUser = lstUser.FindAll(value => value.m_nUserLogin == nOnline);
                }

                lstUser = lstUser.OrderByDescending(value => value.m_strUserRegTime).ToList();
                int nCnt = lstUser.Count - nPage * nPCnt;
                if (nCnt <= 0)
                {
                    lstUser.Clear();
                }
                else if (nCnt < nPCnt)
                    lstUser = lstUser.GetRange(nPage * nPCnt, nCnt);
                else
                    lstUser = lstUser.GetRange(nPage * nPCnt, nPCnt);

                List<CUserModel> lstUserModel = new List<CUserModel>();

                for (int i = 0; i < lstUser.Count; i++)
                {
                    CUserModel clsUserModel = lstUser[i].GetUserModel();
                    lstUserModel.Add(clsUserModel);
                }

                CUserInfoListS2APacket clsUserListPacket = new CUserInfoListS2APacket();
                clsUserListPacket.clsUserList = lstUserModel;
                clsUserListPacket.nTotalCnt = CGlobal.GetUserList().Count;
                clsUserListPacket.nNewCnt = CGlobal.GetUserList().FindAll(value => (CMyTime.GetMyTime() - CMyTime.ConvertStrToTime(value.m_strUserRegTime)).Days < 1).Count;
                clsUserListPacket.nBlockCnt = CGlobal.GetUserList().FindAll(value => value.m_nUserState == 2).Count;
                clsUserListPacket.nOnlineCnt = CGlobal.GetUserList().FindAll(value => value.m_nUserLogin == 1 && value.m_nUserState == 1).Count;

                sendPktData.strPktData = JsonConvert.SerializeObject(clsUserListPacket);
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminUserList");
                sendPktData.strPktData = "-1";
            }

            SendPacket(sendPktData);
        }

        private void StartGameStateSend(CAdminSocket clsSocket)
        {
            clsSocket.SendGameState();

            while (clsSocket.State == WebSocketSharp.WebSocketState.Open)
            {
                clsSocket.SendGameState();
                Thread.Sleep(3000);
            }
        }

        private void SendGameState()
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_GAMESTATE_RSPONSE);
            CGameStateS2APacket clsPacket = new CGameStateS2APacket();
            clsPacket.nUserTotalCnt = CGlobal.GetUserList().Count;
            clsPacket.nUserNewCnt = CGlobal.GetUserList().FindAll(value => (CMyTime.GetMyTime() - CMyTime.ConvertStrToTime(value.m_strUserRegTime)).Days < 1).Count;
            clsPacket.nUserBlockCnt = CGlobal.GetUserList().FindAll(value => value.m_nUserState == 2).Count;
            clsPacket.nUserOnlineCnt = CGlobal.GetUserList().FindAll(value => value.m_nUserLogin == 1 && value.m_nUserState == 1).Count;

            string sql = "SELECT COUNT(chgCode) AS chargeCnt FROM tbl_charge WHERE chgCheck = 0";
            clsPacket.nUserChargeCnt = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["chargeCnt"]);

            sql = "SELECT COUNT(exCode) AS exchargeCnt FROM tbl_excharge WHERE exCheck = 0";
            clsPacket.nUserExchargeCnt = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["exchargeCnt"]);

            sql = "SELECT COUNT(aexCheck) AS realCnt FROM tbl_agentex WHERE aexCheck = 0";
            clsPacket.nAgentExchargeCnt = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["realCnt"]);

            sql = "SELECT COUNT(nCode) AS exCnt FROM tbl_subex WHERE nCheck = 0";
            clsPacket.nUnderExchargeCnt = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["exCnt"]);

            sendPktData.strPktData = JsonConvert.SerializeObject(clsPacket);
            SendPacket(sendPktData);
        }

        private void StartStoreStateSend(CAdminSocket clsSocket)
        {
            
            clsSocket.SendStoreState();
            
            while (clsSocket.State == WebSocketSharp.WebSocketState.Open)
            {
                clsSocket.SendStoreState();
                Thread.Sleep(3000);
            }
        }

        private void SendStoreState()
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_GAMESTATE_RSPONSE);
            CGameStateS2APacket clsPacket = new CGameStateS2APacket();
            clsPacket.nUserTotalCnt = CGlobal.GetUserList().FindAll(value=>value.m_nStoreCode == m_clsStore.m_nStoreCode).Count;
            clsPacket.nUserNewCnt = CGlobal.GetUserList().FindAll(value => (CMyTime.GetMyTime() - CMyTime.ConvertStrToTime(value.m_strUserRegTime)).Days < 1 && value.m_nStoreCode == m_clsStore.m_nStoreCode).Count;
            clsPacket.nUserBlockCnt = CGlobal.GetUserList().FindAll(value => value.m_nUserState == 2 && value.m_nStoreCode == m_clsStore.m_nStoreCode).Count;
            clsPacket.nUserOnlineCnt = CGlobal.GetUserList().FindAll(value => value.m_nUserLogin == 1 && value.m_nUserState == 1 && value.m_nStoreCode == m_clsStore.m_nStoreCode).Count;
            clsPacket.nStoreAlCash = m_clsStore.m_nStoreCash;

            string sql = $"SELECT COUNT(chgCode) AS chargeCnt FROM tbl_storecharge WHERE chgCheck = 0 AND storeCode = {m_clsStore.m_nStoreCode}";
            clsPacket.nUserChargeCnt = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["chargeCnt"]);

            sql = $"SELECT COUNT(exCode) AS exchargeCnt FROM tbl_storeexcharge WHERE exCheck = 0 AND storeCode = {m_clsStore.m_nStoreCode}";
            clsPacket.nUserExchargeCnt = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["exchargeCnt"]);

            sendPktData.strPktData = JsonConvert.SerializeObject(clsPacket);
            SendPacket(sendPktData);
        }


        private void StoreUserList(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_USERLIST_RESPONE);

            try
            {
                JToken json = JToken.Parse(strPktData);

                string key = Convert.ToString(json["key"]);
                string condition = Convert.ToString(json["condition"]);
                int nKind = Convert.ToInt32(json["kind"]);
                int nOnline = Convert.ToInt32(json["online"]);
                int nStoreCode = Convert.ToInt32(json["storeCode"]);

                if (nStoreCode == 0)
                    return;

                List<CUser> lstUser = CGlobal.GetUserList().FindAll(value => value.m_nStoreCode == nStoreCode);

                if (key != string.Empty && condition != string.Empty)
                {
                    if (key == "userID")
                    {
                        lstUser = lstUser.FindAll(user => user != null && user.m_strUserID.Contains(condition) && user.m_nUserState != 3);
                    }
                    else if (key == "userNick")
                    {
                        lstUser = lstUser.FindAll(user => user != null && user.m_strUserNick.Contains(condition) && user.m_nUserState != 3);
                    }
                    else if (key == "agenNick")
                    {
                        lstUser = lstUser.FindAll(user => user != null && CGlobal.GetAgentByCode(user.m_nAgenCode).m_strAgenNick.Contains(condition) && user.m_nUserState != 3);
                    }
                    else if (key == "storeNick")
                    {
                        lstUser = lstUser.FindAll(user => user != null && CGlobal.GetStoreNickByCode(user.m_nStoreCode).Contains(condition) && user.m_nUserState != 3);
                    }
                }
                else
                {
                    lstUser = lstUser.FindAll(user => user != null && user.m_nUserState != 3);
                }


                if (nKind == 4)
                {
                    //신규회원보기
                    string strTime = CMyTime.GetMyTimeStr("yyyy-MM-dd");
                    lstUser = lstUser.FindAll(value => value.m_strUserRegTime.IndexOf(strTime) > -1);
                }
                else if (nKind < 4)
                {
                    lstUser = lstUser.FindAll(value => value.m_nUserState == nKind);
                }

                if (nOnline < 2)
                {
                    lstUser = lstUser.FindAll(value => value.m_nUserLogin == nOnline);
                }

                lstUser = lstUser.OrderBy(value => value.m_nAgenCode).ToList();

                List<CUserModel> lstUserModel = new List<CUserModel>();

                for (int i = 0; i < lstUser.Count; i++)
                {
                    CUserModel clsUserModel = lstUser[i].GetUserModel();
                    lstUserModel.Add(clsUserModel);
                }

                CUserInfoListS2APacket clsUserListPacket = new CUserInfoListS2APacket();
                clsUserListPacket.clsUserList = lstUserModel;
                clsUserListPacket.nTotalCnt = lstUserModel.Count;
                clsUserListPacket.nNewCnt = lstUserModel.FindAll(value => (CMyTime.GetMyTime() - CMyTime.ConvertStrToTime(value.m_strUserRegTime)).Days < 1).Count;
                clsUserListPacket.nBlockCnt = lstUserModel.FindAll(value => value.m_nUserState == 2).Count;
                clsUserListPacket.nOnlineCnt = lstUserModel.FindAll(value => value.m_nUserLogin == 1 && value.m_nUserState == 1).Count;

                sendPktData.strPktData = JsonConvert.SerializeObject(clsUserListPacket);
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : StoreUserList");
                sendPktData.strPktData = "-1";
            }

            SendPacket(sendPktData);
        }


        private void AdminUserGearList(string strPktData)
        {
            string[] param = strPktData.Split(":");
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_USERGEARLIST_RESPONE);

            try
            {
                string key = param[0];
                string condition = param[1];
                int nKind = Convert.ToInt32(param[2]);
                int nOnline = Convert.ToInt32(param[3]);
                int nUserCode = Convert.ToInt32(param[4]);
                int nStoreCode = Convert.ToInt32(param[5]);

                CUser clsUser = CGlobal.GetUserByCode(nUserCode);
                if (clsUser == null)
                    return;

                List<CBaseGear> lstGear = clsUser.GetUserGearList();
                List<CGearModel> lstAdminGearInfoPacket = new List<CGearModel>();

                for (int i = 0; i < lstGear.Count; i++)
                {
                    CGearModel clsGearModel = lstGear[i].GetGearModel();
                    lstAdminGearInfoPacket.Add(clsGearModel);
                }
                sendPktData.strPktData = JsonConvert.SerializeObject(lstAdminGearInfoPacket);
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminUserGearList");
                sendPktData.strPktData = "-1";
            }

            SendPacket(sendPktData);
        }

        public void AdminSendAnswer(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_ANSWER_RCEV);
            try
            {
                JToken json = JToken.Parse(strPktData);

                int nFaqCode = Convert.ToInt32(json["nFaqCode"]);
                int nUserCode = Convert.ToInt32(json["nUserCode"]);
                string strAnswer = Convert.ToString(json["strAnswer"]);
                int nAcid = Convert.ToInt32(json["nAcid"]);

                CUser user = CGlobal.GetUserByCode(nUserCode);
                if (user == null)
                    return;

                if (user.m_nUserLogin == 1)
                {
                    if (nAcid == 1)
                    {
                        if (strAnswer.Split(',').Length != 3)
                        {
                            sendPktData.strPktData = "계좌문의에 대한 답변을 정확히 입력해 주세요.";
                        }
                        else
                        {
                            CGlobal.SendAdminToUserMessage(nUserCode, strAnswer, nAcid);
                            sendPktData.strPktData = "success";
                        }
                    }
                    else
                    {
                        CGlobal.SendAdminToUserMessage(nUserCode, strAnswer, nAcid);
                        sendPktData.strPktData = "success";
                    }
                }
                else
                {
                    sendPktData.strPktData = user.m_strUserNick + "님은 오프라인입니다.";
                }
                
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminSendAnswer");
                sendPktData.strPktData = "-1";
            }

            SendPacket(sendPktData);

        }

        public void AdminSendChat(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_CHAT_RESPONSE);
            try
            {
                JToken json = JToken.Parse(strPktData);

                int nOnerCode = Convert.ToInt32(json["nOnerCode"]);
                int nUserCode = Convert.ToInt32(json["nUserCode"]);
                string strChat = Convert.ToString(json["strChat"]);

                CChatS2APacket chat = new CChatS2APacket();
                CUser user = CGlobal.GetUserByCode(nUserCode);
                if (user.m_nUserLogin == 1)
                {
                    string strTime = CMyTime.GetMyTimeStr();
                    string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);

                    chat.strTime = strTime;
                    chat.strNick = strOnerNick;
                    chat.strChat = strChat;
                    chat.flag = "success";
                    sendPktData.strPktData = JsonConvert.SerializeObject(chat);

                    CGlobal.SendSecurityChat(nOnerCode, nUserCode, 0, strChat);
                }
                else
                {
                    chat.flag = "failed";
                    chat.strChat = user.m_strUserNick + "님은 오프라인입니다.";
                    sendPktData.strPktData = JsonConvert.SerializeObject(chat);
                }
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminSendChat");
            }

            SendPacket(sendPktData);

        }

        private void AdminGearList(string strPktData)
        {

            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_GEARLIST_RESPONSE);

            try
            {
                string[] packet = strPktData.Split(',');
                int nGameCode = Convert.ToInt32(packet[0]);
                int nGearState = Convert.ToInt32(packet[1]);
                int nGearRun = Convert.ToInt32(packet[2]);
                int nTakeUser = Convert.ToInt32(packet[3]);
                int nSelectPage = Convert.ToInt32(packet[4]) - 1;
                if (nSelectPage < 0)
                    nSelectPage = 0;
                int nDataCount = 50;

                List<CGearModel> lstGearModel = new List<CGearModel>();

                List<CBaseGear> lstGear = CGlobal.GetGearList().OrderBy(value => value.m_nGearCode).ToList();
                if(nGameCode > 0)
                    lstGear = CGlobal.GetGearListByGameCode(nGameCode).OrderBy(value => value.m_nGearCode).ToList();

                if (nGearState > -1)
                {
                    if(nGearState == 2)
                    {
                        lstGear = lstGear.FindAll(value => value.m_nGearCheck == 1);
                    }
                    else
                    {
                        lstGear = lstGear.FindAll(value => value.m_nGearState == nGearState);
                    }
                    
                }
                if (nGearRun > -1)
                {
                    lstGear = lstGear.FindAll(value => value.m_nGearRun == nGearRun);
                }
                if(nTakeUser > -1)
                {
                    if (nTakeUser == 0)
                        lstGear = lstGear.FindAll(value => value.m_nTakeUser > 0 && value.m_nTakeRobot == 0);
                    else if (nTakeUser == 1)
                        lstGear = lstGear.FindAll(value => value.m_nTakeUser == 0 && value.m_nTakeRobot > 0);
                }

                int nCount = nSelectPage * nDataCount + nDataCount;
                if (nCount > lstGear.Count)
                    nCount = lstGear.Count;

                for (int i = nSelectPage * nDataCount; i < nCount; i++)
                {
                    CGearModel clsGearModel = lstGear[i].GetGearModel();
                    lstGearModel.Add(clsGearModel);
                }
                sendPktData.strPktData = JsonConvert.SerializeObject(lstGearModel);
            }
            catch (Exception ex)
            {
                sendPktData.strPktData = "-1";
                CGlobal.Log(ex.Message + " : AdminGearList");
            }

            SendPacket(sendPktData);
        }

        public void AdminRobotChat(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_S2O_GLOBAL_ROBOT_CHAT_RESPONSE);
            try
            {
                JToken json = JToken.Parse(strPktData);

                int nOnerCode = Convert.ToInt32(json["nOnerCode"]);
                int nRbCode = Convert.ToInt32(json["nRbCode"]);
                string strChat = Convert.ToString(json["strChat"]);

                CChatS2APacket chat = new CChatS2APacket();
                CRobot robot = CGlobal.GetRobotByCode(nRbCode);
                string strTime = CMyTime.GetMyTimeStr();
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);

                CGChatPacket chatPacket = new CGChatPacket();
                chatPacket.m_nCode = nRbCode;
                chatPacket.m_nOnerCode = nOnerCode;
                chatPacket.m_strMsg = strChat;
                chatPacket.m_strTime = strTime;

                CDataBase.InsertRobotChatToDB(chatPacket);
                chat.strTime = strTime;
                chat.strNick = strOnerNick;
                chat.strChat = strChat;
                chat.flag = "success";
                CGlobal.SendGroupChat(nRbCode, 2, strChat);

                sendPktData.strPktData = JsonConvert.SerializeObject(chat);

            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminRobotChat");
                sendPktData.strPktData = "-1";
            }

            SendPacket(sendPktData);

        }

        public void AdminSendNotice(string strPktData)
        {
            try
            {
                JToken json = JToken.Parse(strPktData);

                int nOnerCode = Convert.ToInt32(json["nOnerCode"]);
                string strNotice = Convert.ToString(json["strNotice"]);
                string strTitle = Convert.ToString(json["strTitle"]);

                CGlobal.SetNotice(strTitle, strNotice);
                string strValue = CGlobal.GetNotice();
                CGlobal.SendNoticeBroadCast(strValue);
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminSendNotice");
            }
        }

        public void AdminSendBroadChat(string strPktData)
        {
            try
            {
                JToken json = JToken.Parse(strPktData);

                int nOnerCode = Convert.ToInt32(json["nOnerCode"]);
                string strValue = Convert.ToString(json["strChat"]);
                CGlobal.SendGroupChat(nOnerCode, 0, strValue);
                return;

            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message + " : AdminSendBroadChat");
            }
        }

        private void ChatOnerLogin(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_O2S_CHAT_LOGIN);


            string[] packet = strPktData.Split(',');

            string strOnerID = packet[0].Trim();
            string strOnerPW = packet[1].Trim();

            CAdmin clsAdmin = CGlobal.GetAdminByAdminID(strOnerID);

            if (clsAdmin != null)
            {
                if(clsAdmin.m_bOnerLogin)
                {
                    sendPktData.nPktResult = CDefine.CTL_FAILED;
                    sendPktData.strPktMsg = "중복접속을 시도하였습니다.";
                }
                else if(clsAdmin.m_strOnerPW == strOnerPW)
                {
                    sendPktData.strPktData = JsonConvert.SerializeObject(clsAdmin);
                    sendPktData.nPktResult = CDefine.CTL_SUCCESS;

                    m_clsAdmin = clsAdmin;
                    m_clsAdmin.m_bOnerLogin = true;
                    CGlobal.AddChatSocket(this);
                }
                else
                {
                    sendPktData.nPktResult = CDefine.CTL_FAILED;
                    sendPktData.strPktMsg = "비번이 정확하지 않습니다.";
                }
            }
            else
            {
                sendPktData.nPktResult = CDefine.CTL_FAILED;
                sendPktData.strPktMsg = "아이디가 존재하지 않습니다.";
            }

            SendPacket(sendPktData);
        }

        private void ChatUserList(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_O2S_CHAT_USERLIST);

            List<CUserModel> lstUserModel = new List<CUserModel>();
            List<CUser> lstUser = CGlobal.GetUserList().FindAll(user => user != null && user.m_nUserLogin == 1);
            for (int i = 0; i < lstUser.Count; i++)
            {
                CUserModel clsUserModel = lstUser[i].GetUserModel();
                lstUserModel.Add(clsUserModel);
            }

            sendPktData.strPktData = JsonConvert.SerializeObject(lstUserModel);
            SendPacket(sendPktData);
        }

        private void ChatRobotList(string strPktData)
        {
            CPacket sendPktData = new CPacket(CDefine.PKT_HEADER_O2S_CHAT_ROBOTLIST);

            List<CRobot> lstRobot = CGlobal.GetRobotList().FindAll(robot => robot.m_nRbLogin == 1);
            sendPktData.strPktData = JsonConvert.SerializeObject(lstRobot);

            SendPacket(sendPktData);
        }

        private void ChatGroup(string strPktData)
        {
            CGChatPacket clsChatPacket = JsonConvert.DeserializeObject<CGChatPacket>(strPktData);
            CGlobal.SendGroupChat(clsChatPacket.m_nCode, clsChatPacket.m_nKind, clsChatPacket.m_strMsg);
        }

        private void ChatSecurity(string strPktData)
        {
            CSChatPacket clsChatPacket = JsonConvert.DeserializeObject<CSChatPacket>(strPktData);
            CGlobal.SendSecurityChat(clsChatPacket.m_nOnerCode, clsChatPacket.m_nUserCode, clsChatPacket.m_nType, clsChatPacket.m_strMsg);
        }

        private void ChatBlock(string strPktData)
        {
            CUserChatBlock clsPacket = JsonConvert.DeserializeObject<CUserChatBlock>(strPktData);
            CUser clsUser = CGlobal.GetUserByCode(clsPacket.m_nUserCode);
            if (clsUser == null)
                return;

            clsUser.m_nChatBlock = clsPacket.m_nChatBlock;
            clsUser.m_nChatBlockA = clsPacket.m_nChatBlockA;
            CDataBase.SaveUserInfoToDB(clsUser);

            CGlobal.SendUserInfoToClient(clsUser);
        }

        private void ChatLogout()
        {
            m_clsAdmin.m_bOnerLogin = false;
        }
    }
}
