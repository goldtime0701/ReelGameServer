using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;

namespace ReelServer
{
    public partial class CUserSocket
    {
        private void UserReconnect(string strPktData)
        {
            CUser clsUser = CGlobal.GetUserByToken(strPktData);
            if (clsUser == null)
                return;
            this.m_clsUser = clsUser;

            if (clsUser.m_nUserLogin == 1)
            {
                clsUser.m_nUserLogin = 1;
                
                CGlobal.AddUserSocket(this);
                m_bApp = true;


                foreach (CSChatPacket chatPacket in clsUser.m_lstTempSecurityChat)
                {
                    SendSecurityChat(chatPacket);
                }
                clsUser.m_lstTempSecurityChat.Clear();

                foreach (CBaseGear clsgear in m_clsUser.m_lstGear)
                    clsgear.OnReconnect(this);

                CGlobal.Log(clsUser.m_strUserNick + " 재접속");
            }
            else
            {
                clsUser.m_nUserLogin = 0;
                List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value => value.m_nTakeUser == clsUser.m_nUserCode);
                for (int i = 0; i < lstGear.Count; i++)
                {
                    lstGear[i].LogoutGear();
                }

                SendUserInfo();
                this.m_clsUser = null;
            }
            
        }


        private void UserLogin(string strPktData)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_GLOBAL_LOGIN);
            CLoginPacket clsLoginSendPacket = new CLoginPacket();

            CUserInfoC2SPacket clsUserInfo = JsonConvert.DeserializeObject<CUserInfoC2SPacket>(strPktData);
            CUser clsUser = CGlobal.GetUserByID(clsUserInfo.m_strUserID);
            if(clsUser == null)
            {
                clsLoginSendPacket.nCode = CDefine.ERROR_LOGIN_AUTH;
                pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
                SendPacket(pktSendData);

                return;
            }
            if(clsUser.m_strUserPW != clsUserInfo.m_strUserPW)
            {
                clsLoginSendPacket.nCode = CDefine.ERROR_LOGIN_AUTH;
                pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
                SendPacket(pktSendData);
                return;
            }
            if(clsUser.m_nUserLogin == 1)
            {
                clsLoginSendPacket.nCode = CDefine.ERROR_LOGIN_ALREADY;
                pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
                SendPacket(pktSendData);
                return;
            }
            if (clsUser.m_nUserState == 0)
            {
                clsLoginSendPacket.nCode = CDefine.ERROR_LOGIN_WAIT;
                pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
                SendPacket(pktSendData);
                return;
            }
            if (clsUser.m_nUserState == 2 || clsUser.m_nUserState == 3)
            {
                clsLoginSendPacket.nCode = CDefine.ERROR_LOGIN_BLOCK;
                pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
                SendPacket(pktSendData);
                return;
            }

            clsUser.m_strIP = Convert.ToString(Context.UserEndPoint.Address);
            if(CGlobal.CheckBlockIP(clsUser.m_strIP))
            {
                clsLoginSendPacket.nCode = CDefine.ERROR_LOGIN_BLOCK;
                pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
                SendPacket(pktSendData);
                return;
            }

            new Thread(() => BroadCastLogin(clsUser.m_strUserNick)).Start();
           

            //처움가입하는 유저이라면 체험머니주기가 설정되여있으면 자동으로 체험머니와 쿠폰을 주어야 한다.
            if (clsUser.m_nUserLogCnt == 0 && clsUser.m_nChargeCnt == 0)
            {
                CBonusVirtual cls = CGlobal.GetBonusVirtual();
                if (cls.m_nVirtual == 1)
                {
                    clsUser.m_nVirtualCash = cls.m_nVirtualCash;
                    clsUser.m_nVirtualCupn = cls.m_nVirtualCupn;

                    string strRet = "환영합니다.\n";
                    if (cls.m_nVirtualCash > 0 && cls.m_nVirtualCupn > 0)
                    {
                        strRet += "체험머니 " + cls.m_nVirtualCash.ToString("N0") + "원과 체험상품권 " + cls.m_nVirtualCupn.ToString("N0") + "장을 지급 해드렸습니다.\n";
                        strRet += "100% 통기계 버전을 경험 해보세요 .\n정식 충전시 보너스 상품권도 푸짐히 준비하였습니다.\n감사합니다.";
                    }
                    else if (cls.m_nVirtualCash > 0)
                    {
                        strRet += "체험머니 " + cls.m_nVirtualCash.ToString("N0") + "원을 지급 해드렸습니다.\n";
                        strRet += "100% 통기계 버전을 경험 해보세요 .\n";
                        strRet += "정식 충전시 보너스 상품권도 푸짐히 준비하였습니다..\n감사합니다.";
                    }
                    else if (cls.m_nVirtualCupn > 0)
                    {
                        strRet = "체험 상품권 " + cls.m_nVirtualCupn.ToString("N0") + "장을 지급 해드렸습니다.\n";
                        strRet += "100 % 통기계 버전을 경험 해보세요 .\n정식 충전시 보너스 상품권도 준비하였습니다.\n감사합니다.";
                    }

                    CGlobal.SendOnerMessage(clsUser.m_nUserCode, strRet);
                }
            }

            clsUser.m_nUserLogin = 1;
            clsUser.m_nUserLogCnt++;
            clsUser.m_nMobile = clsUserInfo.m_nChatBlock;  //0-Old, 1-Mobile, 2-PC, 3-Web
            clsUser.m_strLogTime = CMyTime.GetMyTimeStr();
            CDataBase.SaveUserLogin(clsUser.m_nUserCode);
            clsUser.m_strToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            CDataBase.SaveUserInfoToDB(clsUser);
            

            m_clsUser = clsUser;
            CGlobal.AddUserSocket(this);
            m_bApp = true;
            CGlobal.SendUserInfoToChatAdmin(clsUser);

            clsUserInfo.m_strUserID = clsUser.m_strUserID;
            clsUserInfo.m_strUserNick = clsUser.m_strUserNick;
            clsUserInfo.m_strToken = clsUser.m_strToken;
            clsUserInfo.m_nAbsentCnt = clsUser.m_nAbsentCnt;
            clsUserInfo.m_nUserCash = clsUser.m_nUserCash + clsUser.m_nVirtualCash;
            clsUserInfo.m_nUserCupn = clsUser.m_nUserCupn + clsUser.m_nVirtualCupn;
            clsUserInfo.m_nChatBlock = clsUser.m_nChatBlock;

            clsLoginSendPacket.nCode = 0;
            clsLoginSendPacket.strData = JsonConvert.SerializeObject(clsUserInfo);
            pktSendData.strPktData = JsonConvert.SerializeObject(clsLoginSendPacket);
            pktSendData.strToken = clsUser.m_strToken;
            SendPacket(pktSendData);
            CGlobal.Log(clsUser.m_strUserNick + " 접속");

            if(CGlobal.GetShowNoticeImage() == 1)
            {
                SendShowNotice();
            }

            SendUserTakeGearList();

            return;
        }

        private void BroadCastLogin(string strUserNick)
        {
            Thread.Sleep(1000);
            string strLogMessage = string.Empty;

            string strTemp = string.Empty;
            for (int i = 0; i < CGlobal._lstNoticeHistory.Count; i++)
            {
                if (strTemp == CGlobal._lstNoticeHistory[i])
                    continue;

                strTemp = CGlobal._lstNoticeHistory[i];
                strLogMessage += CGlobal._lstNoticeHistory[i];
                if (i < CGlobal._lstNoticeHistory.Count - 1)
                    strLogMessage += "\n";
            }
            SendNotice(strLogMessage, true);
            Thread.Sleep(50);

            string strNotice = CGlobal.GetNotice();
            if (strNotice != string.Empty)
                SendNotice(strNotice, true);

            string strChat = "[C][09f909]운영자: " + strUserNick + " 님 입장을 환영합니다.[-]";
            CGlobal.SendNoticeBroadCast(strChat);

            for (int i = 0; i< 5; i++)
            {
                Thread.Sleep(1000);
                SendUserInfo();
            }
        }

        private void SendUserTakeGearList(bool bSelectGame = false)
        {
            if (m_clsUser == null)
                return;

            List<CBaseGear> lstGear = m_clsUser.m_lstGear;
            if (lstGear.Count == 0)
                return;

            List<CGearModel> lstGearInfoPacket = new List<CGearModel>();
            for (int i = 0; i < lstGear.Count; i++)
            {
                lstGear[i].m_nGearState = 1;
                //if(!bSelectGame)
                //    lstGear[i].m_nGearCheck = 0;
                if(lstGear[i].m_nGameCode == CDefine.GAME_DRAGON)
                    lstGear[i].MakeNormalScorelRoll(lstGear[i].m_nSlotCash * 10, true);
                else
                    lstGear[i].MakeNormalScorelRoll(lstGear[i].m_nSlotCash, true);

                CGearModel clsGearModel = lstGear[i].GetGearModel();
                lstGearInfoPacket.Add(clsGearModel);
                CDataBase.SaveGearInfoToDB(lstGear[i]);
            }

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_GLOBAL_TAKEGEARLIST);
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = JsonConvert.SerializeObject(lstGearInfoPacket);
            SendPacket(pktSendData);
        }

        private void RegistRepUserID(string strPktData)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_GLOBAL_REPID);

            CUserInfoC2SPacket clsUserInfo = JsonConvert.DeserializeObject<CUserInfoC2SPacket>(strPktData);
            CUser clsUser = CGlobal.GetUserByID(clsUserInfo.m_strUserID);
            if(clsUser == null)
            {
                pktSendData.strPktData = CDefine.ERROR_REGISTER_IDSUCCESS.ToString();
            }
            else
            {
                pktSendData.strPktData = CDefine.ERROR_REGISTER_IDFAILED.ToString();
            }
            SendPacket(pktSendData);
        }

        private void RegistRepUserNick(string strPktData)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_GLOBAL_REPNICK);

            CUserInfoC2SPacket clsUserInfo = JsonConvert.DeserializeObject<CUserInfoC2SPacket>(strPktData);
            CUser clsUser = CGlobal.GetUserByNick(clsUserInfo.m_strUserNick);
            if (clsUser == null)
            {
                pktSendData.strPktData = CDefine.ERROR_REGISTER_NICKSUCCESS.ToString();
            }
            else
            {
                pktSendData.strPktData = CDefine.ERROR_REGISTER_NICKFAILED.ToString();
            }
            SendPacket(pktSendData);
        }

        private void RegistUser(string strPktData)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_GLOBAL_REGISTER);

            CUserInfoC2SPacket clsUserInfo = JsonConvert.DeserializeObject<CUserInfoC2SPacket>(strPktData);
            CUser clsUser = CGlobal.GetUserByID(clsUserInfo.m_strUserID);
            if (clsUser != null)
            {
                pktSendData.strPktData = CDefine.ERROR_REGISTER_IDFAILED.ToString();
                SendPacket(pktSendData);
                return;
            }
            clsUser = CGlobal.GetUserByNick(clsUserInfo.m_strUserNick);
            if(clsUser != null)
            {
                pktSendData.strPktData = CDefine.ERROR_REGISTER_NICKFAILED.ToString();
                SendPacket(pktSendData);
                return;
            }

            List<CAgent> lstAgent = CGlobal.GetAgentList();
            try
            {
                if (lstAgent.Count > 0)
                {
                    //APP버전일때이다.
                    CAgent clsAgent = CGlobal.GetAgentList()[0];
                    if (clsUserInfo.m_strAgenCode != string.Empty)
                    {
                        clsAgent = CGlobal.GetAgentByMark(clsUserInfo.m_strAgenCode);
                        if (clsAgent == null || clsAgent.m_nIsStore == 1)
                        {
                            pktSendData.strPktData = CDefine.ERROR_REGISTER_AGENTFAILED.ToString();
                            SendPacket(pktSendData);
                            return;
                        }

                        //디비에 먼저 써야 한다.
                        string sql = "INSERT INTO tbl_user(userID, userPW, userNick, userPhone, agenCode, userRegTime, userState) ";
                        sql += "VALUES('" + clsUserInfo.m_strUserID + "', '" + clsUserInfo.m_strUserPW + "', ";
                        sql += "'" + clsUserInfo.m_strUserNick + "', '" + clsUserInfo.m_strPhone + "', ";
                        sql += clsAgent.m_nAgenCode + ", '" + CMyTime.GetMyTimeStr() + "', 1)";
                        clsUser = CDataBase.InsertUserToDB(sql, clsUserInfo.m_strUserID);
                        if (clsUser == null)
                        {
                            pktSendData.strPktData = CDefine.ERROR_REGISTER_FAILED.ToString();
                            SendPacket(pktSendData);

                            return;
                        }

                        //유저가 속한 총판객체에 유저를 추가한다.
                        clsAgent.m_lstUser.Add(clsUser);
                        clsAgent.m_nUserCount++;
                        CDataBase.SaveAgentInfoToDB(clsAgent);

                        pktSendData.strPktData = CDefine.ERROR_REGISTER_SUCCESS.ToString();
                        SendPacket(pktSendData);
                    }
                }
                else
                {
                    //WEB, 왕빠버전일때이다.
                    CStore clsStore = CGlobal.GetStoreList().Find(value => value.m_strStoreMark == clsUserInfo.m_strAgenCode.Trim());
                    if(clsStore == null)
                    {
                        pktSendData.strPktData = CDefine.ERROR_REGISTER_AGENTFAILED.ToString();
                        SendPacket(pktSendData);
                        return;
                    }

                    //디비에 먼저 써야 한다.
                    string sql = "INSERT INTO tbl_user(userID, userPW, userNick, userPhone, userRegTime, userState, storeCode) ";
                    sql += "VALUES('" + clsUserInfo.m_strUserID + "', '" + clsUserInfo.m_strUserPW + "', ";
                    sql += "'" + clsUserInfo.m_strUserNick + "', '" + clsUserInfo.m_strPhone + "', ";
                    sql += "0, '" + CMyTime.GetMyTimeStr() + "', 1, " + clsStore.m_nStoreCode + ")";
                    clsUser = CDataBase.InsertUserToDB(sql, clsUserInfo.m_strUserID);
                    if (clsUser == null)
                    {
                        pktSendData.strPktData = CDefine.ERROR_REGISTER_FAILED.ToString();
                        SendPacket(pktSendData);

                        return;
                    }
                    //유저가 속한 매장객체에 유저를 추가한다.
                    clsStore.m_lstUser.Add(clsUser);
                    clsStore.m_nUserCnt++;
                    CDataBase.SaveStoreInfoToDB(clsStore);

                    pktSendData.strPktData = CDefine.ERROR_REGISTER_SUCCESS.ToString();
                    SendPacket(pktSendData);
                }
            }
            catch(Exception e)
            {
                CGlobal.Log(e.Message);
                pktSendData.strPktData = CDefine.ERROR_REGISTER_FAILED.ToString();
                SendPacket(pktSendData);
            }
        }


        //기계선택창을 열었을때 해당게임에 속한 기어리스트를 얻어 내려보낸다.
        private void GetGearInfoList(string strPktData)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_GEARLIST_RESPONE);

            JToken json = JToken.Parse(strPktData);
            int nGameCode = Convert.ToInt32(json["nGameCode"]);
            List<CBaseGear> lstGear = CGlobal.GetGearListByGameCode(nGameCode);
            List<CGearModel> lstGearInfoPacket = new List<CGearModel>();
            for (int i = 0; i < lstGear.Count; i++)
            {
                CGearModel clsGearModel = lstGear[i].GetGearModel();
                lstGearInfoPacket.Add(clsGearModel);
            }
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = JsonConvert.SerializeObject(lstGearInfoPacket);
            SendPacket(pktSendData);
        }

        //유저가 기계를 선택하였을때 호출
        private void SelectGear(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_SELECTGEAR_RESPONE);
            pktSendData.strToken = m_clsUser.m_strToken;

            int nGameCode = Convert.ToInt32(json["nGameCode"]);
            int nGearNum = Convert.ToInt32(json["nGearNum"]);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (nGearCode > 0)
            {
                CBaseGear preGear = CGlobal.GetGearByCode(nGearCode);
                if(preGear != null)
                {
                    m_clsUser.m_lstGear.Remove(preGear);
                    preGear.m_nGearState = 0;
                    preGear.m_nTakeUser = 0;
                    preGear.m_nGearRun = 0;
                    preGear.m_nGearCheck = 0;
                    //디베에 유저 기어가 해제 되였다는것을 보관
                    CDataBase.SaveGearInfoToDB(preGear);
                }
            }
           

            CBaseGear clsGear = CGlobal.GetGearByGameCodeGearNum(nGameCode, nGearNum);
            if(clsGear == null)
            {
                return;
            }

            CGearModel clsGearModel = clsGear.GetGearModel();

            if (clsGear.m_nGearState == 0)
            {
                clsGear.m_nGearState = 1; //기어선택상태로 만든다.
                clsGear.m_nTakeUser = m_clsUser.m_nUserCode; //선택한 유저를 설정한다.

                
                pktSendData.strPktData = JsonConvert.SerializeObject(clsGearModel);
                m_clsUser.m_lstGear.Add(clsGear);
                //디베에 유저가 기어를 선택하였다는것을 보관한다.
                CDataBase.SaveGearInfoToDB(clsGear);
                CGlobal.Log(m_clsUser.m_strUserNick + " " + CGlobal.GetGameNameByGameCode(clsGear.m_nGameCode) + clsGear.m_nGearNum.ToString("00") + " 기계선택");
                SendPacket(pktSendData);
            }
            else
            {
                pktSendData.strPktData = "-1";
                CGlobal.Log(m_clsUser.m_strUserNick + " " + CGlobal.GetGameNameByGameCode(clsGear.m_nGameCode) + clsGear.m_nGearNum.ToString("00") + " 기계가 이미 할당되였습니다.");
                SendPacket(pktSendData);
            }
        }

        private void AppendCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_APPENDCASH_RESPONE);

            if (nGearCode == 0)
                return;
            
            string strGame = CGlobal.GetGearNickByGearCode(nGearCode);
            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            int nGameCode = clsGear.m_nGameCode;
            int nAppenCash = CGlobal.GetGameEngineByCode(nGameCode).m_nCashAppend;


            if (m_clsUser.m_nUserCash <= 0 && m_clsUser.m_nVirtualCash <= 0)
            {
                pktSendData.strPktData = "-1";
                CGlobal.Log(m_clsUser.m_strUserNick + "님 금액이 충분하지 못하므로 " + strGame + "기대에 만원넣기가 실패하였습니다");
                SendPacket(pktSendData);
                return;
            }

            if (m_clsUser.m_nChargeCnt > 0)
            {
                if (m_clsUser.m_nUserCash < nAppenCash)
                {
                    nAppenCash = m_clsUser.m_nUserCash;

                }
                m_clsUser.m_nGearAppendCash += nAppenCash;
                m_clsUser.m_nAppendCash += nAppenCash;

                string strNote = m_clsUser.m_strUserNick + "님이 " + CGlobal.GetGameNameByGameCode(clsGear.m_nGameCode) + clsGear.m_nGearNum + "기대에 " + nAppenCash.ToString("N0") + "원넣기 진행";
                CDataBase.InsertCacheToDB(m_clsUser.m_nUserCode, -nAppenCash, 0, m_clsUser.m_nUserCash, m_clsUser.m_nUserCupn, m_clsUser.m_nUserCash - nAppenCash, m_clsUser.m_nUserCupn, strNote);

                //배팅내역에 보관을 한다.
                CDataBase.InsertBettingToDB(m_clsUser.m_nUserCode, nAppenCash, m_clsUser.m_nUserCash, m_clsUser.m_nUserCash - nAppenCash, nGameCode, 0);

                //유저금액에서 만원을 던다.
                m_clsUser.m_nUserCash -= nAppenCash;
                CGlobal.GetGameEngineByCode(nGameCode).AddAppendPot(nAppenCash, m_clsUser.m_nUserCode);
            }
            else
            {
                if (m_clsUser.m_nVirtualCash < nAppenCash)
                {
                    nAppenCash = m_clsUser.m_nVirtualCash;
                }

                //유저금액에서 만원을 던다.
                m_clsUser.m_nVirtualCash -= nAppenCash;
            }

            CDataBase.SaveUserInfoToDB(m_clsUser);
            //기대의 슬롯머니를 만원증가시킨다.
            if (nGameCode == CDefine.GAME_DRAGON)
                clsGear.m_nSlotCash += Convert.ToInt32((Convert.ToInt32(nAppenCash / 10) / 5)) * 5;
            else
                clsGear.m_nSlotCash += nAppenCash;

            CDataBase.SaveGearInfoToDB(clsGear);
            CGearModel clsGearModel = clsGear.GetGearModel();
            pktSendData.strPktData = JsonConvert.SerializeObject(clsGearModel);
            SendPacket(pktSendData);
            CGlobal.Log(m_clsUser.m_strUserNick + " " + strGame + "기계에 " + nAppenCash.ToString("N0") + "원넣기 진행");

            //다음은 점수 배렬을 만든다.
            clsGear.MakeNormalScorelRoll(nAppenCash, false); //일반점수배렬만들기
            CGlobal.SendUserInfoToClient(m_clsUser);
        }


        private void GearStart(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            clsGear.OnGearStart(1);

            CGearModel clsGearModel = clsGear.GetGearModel();
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_GEARINFO_SEND);
            pktSendData.strPktData = JsonConvert.SerializeObject(clsGearModel);
            SendPacket(pktSendData);
        }


        private void GearStop(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;


            string strToken = Convert.ToString(json["strToken"]);
            
            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            clsGear.OnGearStart(0);

            CGearModel clsGearModel = clsGear.GetGearModel();
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_GEARINFO_SEND);
            pktSendData.strPktData = JsonConvert.SerializeObject(clsGearModel);
            SendPacket(pktSendData);
        }

        private void GroupChat(string strPktData)
        {
            if(m_clsUser.m_nChatBlockA == 0)
                CGlobal.SendGroupChat(m_clsUser.m_nUserCode, 1, strPktData);
        }

        private void AcidFaq(string strPktData)
        {
            int nUserCode = m_clsUser.m_nUserCode;
            int nStoreCode = m_clsUser.m_nStoreCode;
            string strTime = CMyTime.GetMyTimeStr();
            //디비에 신청내용을 보관한다.
            CDataBase.InsertUserFaqToDB(nUserCode, strPktData, strTime, 1);
            CGlobal.SendAdminToUserMessage(nUserCode, CGlobal.GetAcid(nStoreCode), 1);
        }

        public void ChargeRequest(string strPktData)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_CHARGE);
            pktSendData.strToken = m_clsUser.m_strToken;

            TimeSpan span = CMyTime.GetMyTime() - m_clsUser.m_dtChargeTime;
            if (span.Minutes < 1)
            {
                pktSendData.strPktData = CDefine.ERROR_CHARGE_TIME.ToString();
                SendPacket(pktSendData);
                return;
            }
            m_clsUser.m_dtChargeTime = CMyTime.GetMyTime();

            JToken json = JToken.Parse(strPktData);
            

            string strBankName = Convert.ToString(json["m_strBankName"]);
            string strBankAcid = Convert.ToString(json["m_strBankAcid"]);
            string strUserName = Convert.ToString(json["m_strUserName"]);
            string strUserPhone = Convert.ToString(json["m_strUserPhone"]);
            int nChargeCash = Convert.ToInt32(json["m_nChargeCash"]);
            int nUserCode = m_clsUser.m_nUserCode;
            string strChargeTime = CMyTime.GetMyTimeStr();

            //디비에 충전신청내용을 보관한다.
            CDataBase.InsertChargeToDB(strBankName, strBankAcid, strUserName, strUserPhone, nChargeCash, nUserCode, strChargeTime);

            string strUserNick = CGlobal.GetUserNickByCode(nUserCode);
            string strLog = strUserNick + "님이 " + nChargeCash.ToString() + "원 입금신청을 하였습니다.";
            CGlobal.Log(strLog);

            pktSendData.strPktData = "0";
            SendPacket(pktSendData);
        }

        public void ExchargeRequest(string strPktData)
        {
            TimeSpan span = CMyTime.GetMyTime() - m_clsUser.m_dtExChargeTime;
            if (span.Minutes < 5)
            {
                string strMsg = "시스템상 환전신청은 5분이상 간격으로만 가능하오니 잠시후 다시 신청해주시기 바랍니다.";
                SendOnerMessage(strMsg);
                return;
            }
            m_clsUser.m_dtExChargeTime = CMyTime.GetMyTime();

            JToken json = JToken.Parse(strPktData);
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_EXCHARGE);
            pktSendData.strToken = m_clsUser.m_strToken;

            string strBankName = Convert.ToString(json["m_strBankName"]);
            string strBankAcid = Convert.ToString(json["m_strBankAcid"]);
            string strUserName = Convert.ToString(json["m_strUserName"]);
            string strUserPhone = Convert.ToString(json["m_strUserPhone"]);
            int nExchargeCash = Convert.ToInt32(json["m_nChargeCash"]);
            int nUserCode = m_clsUser.m_nUserCode;
            string strExchargeTime = CMyTime.GetMyTimeStr();


            if (nExchargeCash > m_clsUser.m_nUserCash)
            {
                if (m_clsUser.m_nVirtualCash > 0)
                {
                    pktSendData.strPktData = CDefine.ERROR_EXCHARGE_VIRTUAL.ToString();
                }
                else
                {
                    pktSendData.strPktData = CDefine.ERROR_EXCHARGE_DRAWZERO.ToString();
                }
            }
            //else if (nExchargeCash < 10000 || nExchargeCash > 1000000)
            //{
            //    pktSendData.strPktData = CDefine.ERROR_EXCHARGE_LIMIT.ToString();
            //}
            //else if(m_clsUser.m_nAppendCash < m_clsUser.m_nChargeCash + m_clsUser.m_nBonusCash)
            //{
            //    pktSendData.strPktData = CDefine.ERROR_EXCHARGE_ROLLING.toString();
            //}
            else
            {
                //유저머니를 감소시킨다.
                m_clsUser.m_nUserCash -= nExchargeCash;
                CDataBase.SaveUserInfoToDB(m_clsUser);
                //디비에 충전신청내용을 보관한다.
                CDataBase.InsertExchargeToDB(strBankName, strBankAcid, strUserName, strUserPhone, nExchargeCash, nUserCode, strExchargeTime);

                string strUserNick = CGlobal.GetUserNickByCode(nUserCode);
                string strLog = strUserNick + "님이 " + nExchargeCash.ToString() + "원 환전신청을 하였습니다.";
                CGlobal.Log(strLog);

                pktSendData.strPktData = "0";
            }

            SendPacket(pktSendData);
            CGlobal.SendUserInfoToClient(m_clsUser);
        }

        public void ExcupnRate(string strPktData)
        {
            int nUnit = 4500;
            if (m_clsUser.m_nStoreCode > 0)
            {
                CStore clsStore = CGlobal.GetStoreByCode(m_clsUser.m_nStoreCode);
                if (clsStore != null)
                    nUnit = Convert.ToInt32(5000 * clsStore.m_nCupnPro / 100);
            }

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_EXCUPON_RATE);
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = nUnit.ToString();
            SendPacket(pktSendData);
        }

        public void ExcupnRequest(string strPktData)
        {
            int nExCupn = Int32.Parse(strPktData);
            if (nExCupn == 0)
                return;

            if (nExCupn > m_clsUser.m_nUserCupn)
                return;

            int nUnit = 4500;
            if(m_clsUser.m_nStoreCode > 0)
            {
                CStore clsStore = CGlobal.GetStoreByCode(m_clsUser.m_nStoreCode);
                if (clsStore != null)
                {
                    nUnit = Convert.ToInt32(5000 * clsStore.m_nCupnPro / 100);
                    int nExCupnReal = (5000 - nUnit) * nExCupn;
                    clsStore.m_nExcupnReal += nExCupnReal;
                    CDataBase.SaveStoreInfoToDB(clsStore);
                }
            }

            if (m_clsUser.m_nChargeCnt > 0)
            {
                string strNote = m_clsUser.m_strUserNick + "님이 " + nExCupn + "개의 상품권을 게임머니 " + (nExCupn * nUnit) + "원으로 전환";
                CDataBase.InsertCacheToDB(m_clsUser.m_nUserCode, (nExCupn * nUnit), -nExCupn, m_clsUser.m_nUserCash, m_clsUser.m_nUserCupn, m_clsUser.m_nUserCash + (nExCupn * nUnit), m_clsUser.m_nUserCupn - nExCupn, strNote);

                //배팅내역에 보관을 한다.
                CDataBase.InsertBettingToDB(m_clsUser.m_nUserCode, (nExCupn * nUnit), m_clsUser.m_nUserCash, m_clsUser.m_nUserCash + (nExCupn * nUnit), 0, 1);

                m_clsUser.m_nUserCupn -= nExCupn;
                m_clsUser.m_nUserCash += (nExCupn * nUnit);
            }
            else
            {
                if (nExCupn > m_clsUser.m_nVirtualCupn)
                    return;
                m_clsUser.m_nVirtualCupn -= nExCupn;
                m_clsUser.m_nVirtualCash += (nExCupn * nUnit);
            }

            CDataBase.SaveUserInfoToDB(m_clsUser);
            CGlobal.SendUserInfoToClient(m_clsUser);
        }

        public void SecurityChat(string strPktData)
        {
            CGlobal.SendSecurityChat(0, m_clsUser.m_nUserCode, 1, strPktData);
        }
        private void GearPostList(string strPacket)
        {
            int nGameCode = Convert.ToInt32(strPacket);

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_GEARPOSTLIST);
            pktSendData.strToken = m_clsUser.m_strToken;

            List<CBaseGear> lstGear = null;
            lstGear = m_clsUser.m_lstGear;
            List<CGearPostInfo> lstGearPost = new List<CGearPostInfo>();
            for (int i = 0; i < lstGear.Count; i++)
            {
                CGearPostInfo postInfo = new CGearPostInfo();
                postInfo.m_nGameCode = lstGear[i].m_nGameCode;
                postInfo.m_nGearCode = lstGear[i].m_nGearCode;
                postInfo.m_nGearNum = lstGear[i].m_nGearNum;
                postInfo.m_nGearCheck = lstGear[i].m_nGearCheck;

                lstGearPost.Add(postInfo);
            }

            pktSendData.strPktData = JsonConvert.SerializeObject(lstGearPost);
            SendPacket(pktSendData);
        }

        private void GearKeepList(string strPacket)
        {
            int nGameCode = Convert.ToInt32(strPacket);

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_GEARKEEPLIST);
            pktSendData.strToken = m_clsUser.m_strToken;

            List<CBaseGear> lstGear = null;
            lstGear = m_clsUser.m_lstGear;
            List<CGearKeepInfo> lstGearKeep = new List<CGearKeepInfo>();
            for (int i = 0; i < lstGear.Count; i++)
            {
                CGearKeepInfo info = new CGearKeepInfo();
                info.m_nGameCode = lstGear[i].m_nGameCode;
                info.m_nGearCode = lstGear[i].m_nGearCode;
                info.m_nGearNum = lstGear[i].m_nGearNum;
                info.m_nSlotCash = lstGear[i].m_nSlotCash;
                info.m_nGearPlay = m_clsUser.GetUserGameLogin(lstGear[i].m_nGameCode) ? 1 : 0;
                lstGearKeep.Add(info);
            }

            pktSendData.strPktData = JsonConvert.SerializeObject(lstGearKeep);
            SendPacket(pktSendData);
        }

        private void GearPost(string strPacket)
        {
            List<CGearPostInfo> lstGearPost = JsonConvert.DeserializeObject<List<CGearPostInfo>>(strPacket);
            if (lstGearPost == null)
                return;

            for (int i = 0; i < lstGearPost.Count; i++)
            {
                int nGearCode = lstGearPost[i].m_nGearCode;
                CBaseGear gear = m_clsUser.m_lstGear.Find(value => value.m_nGearCode == nGearCode);
                if (gear == null)
                    continue;
                gear.m_nGearCheck = lstGearPost[i].m_nGearCheck;
                gear.m_dtCheckTime = CMyTime.GetMyTime();

                CDataBase.SaveGearInfoToDB(gear);
            }
        }

        private void GearJackInfo(string strPacket)
        {
            JToken json = JToken.Parse(strPacket);
            int nGameCode = Convert.ToInt32(json["nGameCode"]);
            int nGearNum = Convert.ToInt32(json["nGearCode"]);
            int nPageNum = Convert.ToInt32(json["nValue"]);

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_JACKINFO_LIST);
            pktSendData.strToken = m_clsUser.m_strToken;

            CBaseGear gear = CGlobal.GetGearByGameCodeGearNum(nGameCode, nGearNum);
            if (gear == null)
                return;

            int nDelta = 0;
            if (gear.m_nGearJack == 1)
                nDelta = 1;
            int nUnit = 8;
            if (gear.m_nGameCode == CDefine.GAME_YMT)
                nUnit = 7;

            List<CJackInfoModel> lstJackInfo = null;
            if (gear.m_lstJackInfo.Count >= nPageNum * nUnit + nDelta + nUnit)
                lstJackInfo = gear.m_lstJackInfo.GetRange(nPageNum * nUnit + nDelta, nUnit);
            else if (gear.m_lstJackInfo.Count > nPageNum * nUnit + nDelta && gear.m_lstJackInfo.Count < nPageNum * nUnit + nDelta + nUnit)
                lstJackInfo = gear.m_lstJackInfo.GetRange(nPageNum * nUnit + nDelta, gear.m_lstJackInfo.Count - nPageNum * nUnit - nDelta);
            else if (gear.m_lstJackInfo.Count < nPageNum * nUnit + nDelta)
                lstJackInfo = new List<CJackInfoModel>();

            CJackInfo clsJackInfo = new CJackInfo();
            clsJackInfo.m_lstJackModel = lstJackInfo;
            if(gear.m_nGameCode == CDefine.GAME_YMT)
            {
                clsJackInfo.m_strYmtInfo = gear.m_nMajorCount + "," + gear.m_nMinorCount + "," + gear.m_nMiniCount;
                for(int i=0; i<10; i++)
                {
                    clsJackInfo.m_strYmtGraph += gear.m_nLstGraph[i];
                    if (i < 9)
                        clsJackInfo.m_strYmtGraph += ",";
                }
            }

            pktSendData.strPktData = JsonConvert.SerializeObject(clsJackInfo);
            SendPacket(pktSendData);
        }

        private void GearPostCheck(string strPacket)
        {
            JToken json = JToken.Parse(strPacket);
            int nGameCode = Convert.ToInt32(json["nGameCode"]);
            int nGearNum = Convert.ToInt32(json["nGearNum"]);

            CBaseGear gear = CGlobal.GetGearByGameCodeGearNum(nGameCode, nGearNum);
            if (gear == null)
                return;

            gear.m_nGearCheck = 1;
            gear.m_dtCheckTime = CMyTime.GetMyTime();

            CDataBase.SaveGearInfoToDB(gear);
        }

        private void ExitGamePage(string strPacket)
        {
            List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value => value.m_nTakeUser == m_clsUser.m_nUserCode);
            for(int i=0; i<lstGear.Count; i++)
            {
                lstGear[i].LogoutGear();
            }

            SendUserTakeGearList(true);
        }

        private void LogoutUser(string strPacket)
        {
            List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value => value.m_nTakeUser == m_clsUser.m_nUserCode);
            for (int i = 0; i < lstGear.Count; i++)
            {
                lstGear[i].LogoutGear();
            }

            m_clsUser.m_nUserLogin = 0;
            CGlobal.SendUserInfoToChatAdmin(m_clsUser);
        }


        private void AbsentList(string strPacket)
        {
            DateTime dtTime = CMyTime.GetMyTime();
            int nDay = dtTime.Day;

            if(m_clsUser.m_lstAsent[nDay - 1] == 0)
                m_clsUser.m_lstAsent[nDay - 1] = CGlobal.EnableAbsent(m_clsUser);
            CAbsentPacket clsAbsentPacket = new CAbsentPacket();
            clsAbsentPacket.m_lstAbsent = m_clsUser.m_lstAsent.ToList();

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ABSENT_LIST);
            pktSendData.strPktData = JsonConvert.SerializeObject(clsAbsentPacket);

            SendPacket(pktSendData);
        }

        private void AbsentCheck(string strPacket)
        {
            CGlobal.AbsentCheck(this);
            
        }

        private void AbsentCompleteCheck(string strPacket)
        {
            int nGiveCupn = CGlobal.GetAbsentGiveCupn();
            m_clsUser.m_nUserCupn += nGiveCupn;
            m_clsUser.m_nBonusCash += (nGiveCupn * 4500);

            string strNote = "출석미션완성 쿠폰 " + nGiveCupn + "개 지급";
            CDataBase.InsertCacheToDB(m_clsUser.m_nUserCode, m_clsUser.m_nUserCash, m_clsUser.m_nUserCupn, m_clsUser.m_nUserCash, m_clsUser.m_nUserCupn - nGiveCupn, m_clsUser.m_nUserCash, m_clsUser.m_nUserCupn, strNote);
        }

        private void GetItemPriceList(string strPacket)
        {
            List<CItemModel> list = CGlobal.GetItemModelList();

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_PRICE_LIST);
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = JsonConvert.SerializeObject(list);
            SendPacket(pktSendData);
        }

        private void GetItemKeepList(string strPacket)
        {
            List<CItem> listItem = CGlobal.GetUserItemListByUserCode(m_clsUser.m_nUserCode);
            List<CItemKeep> listKeep = new List<CItemKeep>();
            List<CItemModel> listModel = CGlobal.GetItemModelList();

            foreach(CItemModel model in listModel)
            {
                CItemKeep keep = new CItemKeep();
                keep.m_nItemModel = model.m_nItemCode;
                keep.m_nItemCount = listItem.Count(value=>value.m_nItemModel == keep.m_nItemModel);
                keep.m_nGameCode = model.m_nGameCode;

                listKeep.Add(keep);
            }

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_KEEP_LIST);
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = JsonConvert.SerializeObject(listKeep);
            SendPacket(pktSendData);
        }

        private void GetItemGearList(string strPacket)
        {
            JToken json = JToken.Parse(strPacket);
            int nGameCode = Convert.ToInt32(json["nValue"]);

            List<CBaseGear> list = CGlobal.GetGearListByGameCode(nGameCode).FindAll(value=>value.m_nGearJack < 1 && value.m_nTakeUser == m_clsUser.m_nUserCode);
            List<CGearModel> listModel = new List<CGearModel>();
            foreach(CBaseGear gear in list)
            {
                CGearModel model = gear.GetGearModel();
                listModel.Add(model);
            }

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_GEAR);
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = JsonConvert.SerializeObject(listModel);
            SendPacket(pktSendData);
        }

        private void BuyItemByUser(string strPacket)
        {
            JToken json = JToken.Parse(strPacket);
            int nItemCode = Convert.ToInt32(json["nValue"]);
            int nItemCount = Convert.ToInt32(json["nMini"]);
            CItemModel itemModel = CGlobal.GetItemModelByCode(nItemCode);

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_BUY);
            pktSendData.strToken = m_clsUser.m_strToken;

            if (itemModel == null)
            {
                pktSendData.nPktResult = 0x29;
                SendPacket(pktSendData);
                return;
            }

            //유저보유금액이 아이템가격보다 많은가를 검사한다.
            if(m_clsUser.m_nUserCash < itemModel.m_nPrice * nItemCount)
            {
                pktSendData.nPktResult = 0x2A;
                SendPacket(pktSendData);
                return;
            }

            //아이템사기를 진행한다.
            for(int i=0; i<nItemCount; i++)
            {
                CItem item = new CItem(m_clsUser.m_nUserCode, itemModel.m_nItemCode, itemModel.m_nPrice);
                CGlobal.AddUserItemToKeepList(item);
                CDataBase.BuyItemByUser(item);

                
            }

            //유저머니를 감소시킨다.
            int nPrice = itemModel.m_nPrice * nItemCount;
            m_clsUser.m_nUserCash -= nPrice;
            CDataBase.SaveUserInfoToDB(m_clsUser);
            CGlobal.SendUserInfoToClient(m_clsUser);

            //유저캐시내역에 로그를 남긴다.
            string strNote = $"{itemModel.m_nPrice} X {nItemCount} 아이템사기진행.";
            CDataBase.InsertCacheToDB(m_clsUser.m_nUserCode, -nPrice, 0, m_clsUser.m_nUserCash + nPrice, m_clsUser.m_nUserCupn, m_clsUser.m_nUserCash, m_clsUser.m_nUserCupn, strNote);


            pktSendData.nPktResult = 0x2B;
            SendPacket(pktSendData);
            return;
        }

        private void UseItemByUser(string strPacket)
        {
            JToken json = JToken.Parse(strPacket);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);
            int nItemModel = Convert.ToInt32(json["nValue"]);

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_USE);
            pktSendData.strToken = m_clsUser.m_strToken;

            CItem item = CGlobal.GetUserItemByItemModel(m_clsUser.m_nUserCode, nItemModel);
            if(item == null)
            {
                pktSendData.nPktResult = 0x2C;   //이미사용되었습니다.
                SendPacket(pktSendData);
                return;
            }

            CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
            if(gear == null)
            {
                pktSendData.nPktResult = 0x2D;   //기계를 선택하세요.
                SendPacket(pktSendData);
                return;
            }
            if(gear.m_nGearJack == 1)
            {
                pktSendData.nPktResult = 0x2D;   //잭팟중입니다.
                SendPacket(pktSendData);
                return;
            }

            gear.UseItemByUser(item);
            item.m_nUse = 1;
            item.m_nGearCode = nGearCode;
            item.m_strUseTime = CMyTime.GetMyTimeStr();
            item.m_strUseNote = $"{gear.m_strGame} {gear.m_nGearNum}번 기계에 사용.";
            CGlobal.RemoveUserItem(item);
            CDataBase.SaveItemToDB(item);

            CGlobal.Log(m_clsUser.m_strUserNick + " " + item.m_strUseNote);

            pktSendData.nPktResult = 0x2E;   //사용성공.
            SendPacket(pktSendData);
            return;
        }


        #region Web 처리부
        private void WebLogin(string strPacket)
        {
            JToken json = JToken.Parse(strPacket);
            int nGameCode = Convert.ToInt32(json["nGameCode"]);
            string strToken = Convert.ToString(json["strValue"]);

            strToken = strToken.Replace(' ', '+');
            CUser clsUser = CGlobal.GetUserByToken(strToken);
            if (clsUser == null)
                return;
            if (clsUser.GetUserGameLogin(nGameCode))
                return;

            m_nGameCode = nGameCode;
            m_clsUser = clsUser;
            if (m_clsUser.m_nUserLogin == 0)
                m_clsUser.m_nUserLogCnt++;
            m_clsUser.m_nUserLogin = 1;
            clsUser.m_nMobile = 3;
            m_clsUser.SetUserGameLogin(nGameCode);

            CUserModel clsUserInfo = new CUserModel();
            CGlobal.AddUserSocket(this);
            m_bApp = false;

            new Thread(() => BroadCastLogin(clsUser.m_strUserNick)).Start();

            clsUserInfo.m_strUserID = clsUser.m_strUserID;
            clsUserInfo.m_strUserNick = clsUser.m_strUserNick;
            clsUserInfo.m_nAbsentCnt = clsUser.m_nAbsentCnt;
            clsUserInfo.m_nUserCash = clsUser.m_nUserCash + clsUser.m_nVirtualCash;
            clsUserInfo.m_nUserCupn = clsUser.m_nUserCupn + clsUser.m_nVirtualCupn;
            clsUserInfo.m_strToken = clsUser.m_strToken;
            clsUserInfo.m_nAgenCode = clsUser.m_nAgenCode;
            clsUserInfo.m_nStoreCode = clsUser.m_nStoreCode;

            CPacket clsPacket = new CPacket(CDefine.PKT_HEADER_C2S_WEB_USERINFO);
            clsPacket.strPktData = JsonConvert.SerializeObject(clsUserInfo);
            SendPacket(clsPacket);
            SendUserWebTakeGearList(nGameCode);
            clsUser.m_strIP = Convert.ToString(Context.UserEndPoint.Address);

            string strLog = clsUserInfo.m_strUserNick + "   " + CDefine.STR_GAMENAME[nGameCode] + "  Web 방식으로 접속";
            CGlobal.Log(strLog);
        }


        private void SendUserWebTakeGearList(int nGameCode)
        {
            List<CBaseGear> lstGear = m_clsUser.m_lstGear.FindAll(value=>value.m_nGameCode == nGameCode);
            if (lstGear.Count == 0)
                return;

            List<CGearModel> lstGearInfoPacket = new List<CGearModel>();
            for (int i = 0; i < lstGear.Count; i++)
            {
                lstGear[i].m_nGearState = 1;
                lstGear[i].m_nGearCheck = 0;
                lstGear[i].MakeNormalScorelRoll(lstGear[i].m_nSlotCash, true);

                CGearModel clsGearModel = lstGear[i].GetGearModel();
                lstGearInfoPacket.Add(clsGearModel);
                CDataBase.SaveGearInfoToDB(lstGear[i]);
            }

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_WEB_TAKEGEARLIST);
            pktSendData.strToken = m_clsUser.m_strToken;
            pktSendData.strPktData = JsonConvert.SerializeObject(lstGearInfoPacket);
            SendPacket(pktSendData);
        }
        #endregion



        #region  바다이야기
        private void SEAStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CSeaGear clsGear = CGlobal.GetGearByCode(nGearCode) as CSeaGear;
            clsGear.OnStartSpin(this);
        }

        private void SEACreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CSeaGear gear = CGlobal.GetGearByCode(nGearCode) as CSeaGear;
            gear.OnCreateCoin(this);
        }

        private void SEASlotCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CSeaGear gear = CGlobal.GetGearByCode(nGearCode) as CSeaGear;
            
            gear.OnAddSlotCash(this, nAddCash);
        }

        private void SEAWaterTank(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nValue = Convert.ToInt32(json["nValue"]);
            int nKind = Convert.ToInt32(json["nMini"]);
            CSeaGear gear = CGlobal.GetGearByCode(nGearCode) as CSeaGear;

            gear.OnWaterTank(this, nKind, nValue);
        }
        #endregion

        #region 손오공
        private void SWKCreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (m_clsUser == null)
                return;

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CSwkGear gear = CGlobal.GetGearByCode(nGearCode) as CSwkGear;
            gear.OnCreateCoin(this);
        }

        private void SWKStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CSwkGear clsGear = CGlobal.GetGearByCode(nGearCode) as CSwkGear;
            clsGear.OnStartSpin(this);
        }

        private void SWKGiftCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CSwkGear gear = CGlobal.GetGearByCode(nGearCode) as CSwkGear;

            gear.OnAddGiftCash(this, nAddCash);
        }

        #endregion

        #region 알라딘
        private void ALDEndAni(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CAldGear gear = CGlobal.GetGearByCode(nGearCode) as CAldGear;
            gear.OnCreateCoin(this);
        }

        private void ALDStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nSpinCash = Convert.ToInt32(json["nValue"]);
            CAldGear clsGear = CGlobal.GetGearByCode(nGearCode) as CAldGear;
            clsGear.OnStartSpin(this);
        }

        private void ALDAddSlotCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CAldGear clsGear = CGlobal.GetGearByCode(nGearCode) as CAldGear;
            clsGear.OnAddSlotCash(this, -100);
        }

        private void ALDAddGiftCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            int nMini = Convert.ToInt32(json["nMini"]);
            CAldGear gear = CGlobal.GetGearByCode(nGearCode) as CAldGear;

            gear.OnAddGiftCash(this, nAddCash, nMini);
        }
        #endregion

        #region 알라딘2
        private void ALD2EndAni(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CAldGear2 gear = CGlobal.GetGearByCode(nGearCode) as CAldGear2;
            gear.OnCreateCoin(this);
        }

        private void ALD2StartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nSpinCash = Convert.ToInt32(json["nValue"]);
            CAldGear2 clsGear = CGlobal.GetGearByCode(nGearCode) as CAldGear2;
            clsGear.OnStartSpin(this);
        }

        private void ALD2AddSlotCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CAldGear2 clsGear = CGlobal.GetGearByCode(nGearCode) as CAldGear2;
            clsGear.OnAddSlotCash(this, -100);
        }

        private void ALD2AddGiftCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            int nMini = Convert.ToInt32(json["nMini"]);
            CAldGear2 gear = CGlobal.GetGearByCode(nGearCode) as CAldGear2;

            gear.OnAddGiftCash(this, nAddCash, nMini);
        }
        #endregion

        #region 5드래곤
        private void FDGStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            string strToken = Convert.ToString(json["strToken"]);
            int nSpinCash = Convert.ToInt32(json["nValue"]);
            CFdgGear gear = CGlobal.GetGearByCode(nGearCode) as CFdgGear;
            gear.OnStartSpin(this, nSpinCash);
        }

        //5드래곤 프리스핀시작
        public void FDGStartFreeSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nDrgIndex = Convert.ToInt32(json["nValue"]);
            CFdgGear gear = CGlobal.GetGearByCode(nGearCode) as CFdgGear;
            gear.OnStartFreeSpin(nDrgIndex);
        }

        //5드래곤 머니전환
        public void FDGExMoney(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CFdgGear gear = CGlobal.GetGearByCode(nGearCode) as CFdgGear;
            gear.OnExMoney();
        }

        //5드래곤기프트점수증가
        public void FDGGiftAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nCash = Convert.ToInt32(json["nValue"]);
            CFdgGear gear = CGlobal.GetGearByCode(nGearCode) as CFdgGear;
            gear.OnAddGiftCash(this, nCash);
        }
        #endregion

        #region 황금성
        public void GDCStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CGdcGear clsGear = CGlobal.GetGearByCode(nGearCode) as CGdcGear;
            clsGear.OnStartSpin(this);
        }

        public void GDCAddGiftCash(string strPktData)
        {
            //JToken json = JToken.Parse(strPktData);
            //int nGearCode = Convert.ToInt32(json["nGearCode"]);

            //if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
            //    return;

            //int nAddCash = Convert.ToInt32(json["nValue"]);
            //CGdcGear gear = CGlobal.GetGearByCode(nGearCode) as CGdcGear;

            //gear.OnAddGiftCash(this, nAddCash);
        }

        public void GDCreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CGdcGear gear = CGlobal.GetGearByCode(nGearCode) as CGdcGear;
            gear.OnCreateCoin(this);
        }


        public void GDCAddJachCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CGdcGear gear = CGlobal.GetGearByCode(nGearCode) as CGdcGear;
            gear.OnAddJashCash(this, nAddCash);
        }

        public void GDCWinJachCash(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CGdcGear gear = CGlobal.GetGearByCode(nGearCode) as CGdcGear;
            gear.OnWinJashCash(this);
        }
        #endregion

        #region 오션
        private void OCAStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            COcaGear clsGear = CGlobal.GetGearByCode(nGearCode) as COcaGear;
            clsGear.OnStartSpin(this);
        }

        private void OCACreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            COcaGear gear = CGlobal.GetGearByCode(nGearCode) as COcaGear;
            gear.OnCreateCoin(this);
        }

        private void OCAGiftCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            COcaGear gear = CGlobal.GetGearByCode(nGearCode) as COcaGear;

            gear.OnAddGiftCash(this, nAddCash, 1);
        }

        private void OCAFinishAnimation(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            COcaGear gear = CGlobal.GetGearByCode(nGearCode) as COcaGear;
            gear.OnFinishAnimation();
        }

        private void OCAWaterTank(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            COcaGear gear = CGlobal.GetGearByCode(nGearCode) as COcaGear;
            gear.OnWaterTank();
        }
        #endregion

        #region 신천지
        private void NWDStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CNwdGear clsGear = CGlobal.GetGearByCode(nGearCode) as CNwdGear;
            clsGear.OnStartSpin(this);
        }

        private void NWDCreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CNwdGear gear = CGlobal.GetGearByCode(nGearCode) as CNwdGear;
            gear.OnCreateCoin(this);
        }

        private void NWDGiftCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CNwdGear gear = CGlobal.GetGearByCode(nGearCode) as CNwdGear;

            gear.OnAddGiftCash(this, nAddCash, 1);
        }

        private void NWDFinishAnimation(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);
            int nKind = Convert.ToInt32(json["nValue"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CNwdGear gear = CGlobal.GetGearByCode(nGearCode) as CNwdGear;
            gear.OnFinishAnimation(nKind);
        }

        private void NWDWaterTank(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);
            int nLR = Convert.ToInt32(json["nValue"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CNwdGear gear = CGlobal.GetGearByCode(nGearCode) as CNwdGear;
            gear.OnWaterTank(nLR);
        }
        #endregion

        #region 야마토
        private void YMTStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CYmtGear clsGear = CGlobal.GetGearByCode(nGearCode) as CYmtGear;
            clsGear.OnStartSpin(this);
        }

        private void YMTAddGift(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CYmtGear clsGear = CGlobal.GetGearByCode(nGearCode) as CYmtGear;
            clsGear.OnAddGiftCash(this, 20000);
        }

        private void YMTExmoney(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CYmtGear clsGear = CGlobal.GetGearByCode(nGearCode) as CYmtGear;
            clsGear.OnExMoney(this);
        }
        #endregion

        #region 다빈치
        private void DVCStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CDvcGear clsGear = CGlobal.GetGearByCode(nGearCode) as CDvcGear;
            clsGear.OnStartSpin(this);
        }

        private void DVCCreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CDvcGear gear = CGlobal.GetGearByCode(nGearCode) as CDvcGear;
            gear.OnCreateCoin(this);
        }

        private void DVCGiftCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CDvcGear gear = CGlobal.GetGearByCode(nGearCode) as CDvcGear;

            gear.OnAddGiftCash(this, nAddCash, 1);
        }

        private void DVCFinishAnimation(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CDvcGear gear = CGlobal.GetGearByCode(nGearCode) as CDvcGear;
            gear.OnFinishAnimation();
        }

        #endregion

        #region 백경
        private void WHTStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CWhtGear clsGear = CGlobal.GetGearByCode(nGearCode) as CWhtGear;
            clsGear.OnStartSpin(this);
        }

        private void WHTCreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CWhtGear gear = CGlobal.GetGearByCode(nGearCode) as CWhtGear;
            gear.OnCreateCoin(this);
        }

        private void WHTGiftCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CWhtGear gear = CGlobal.GetGearByCode(nGearCode) as CWhtGear;

            gear.OnAddGiftCash(this, nAddCash, 1);
        }

        private void WHTFinishAnimation(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CWhtGear gear = CGlobal.GetGearByCode(nGearCode) as CWhtGear;
            gear.OnFinishAnimation();
        }

        private void WHTWaterTank(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CWhtGear gear = CGlobal.GetGearByCode(nGearCode) as CWhtGear;
            gear.OnWaterTank(this);
        }
        #endregion

        #region 양귀비
        private void YANStartSpin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CYanGear clsGear = CGlobal.GetGearByCode(nGearCode) as CYanGear;
            clsGear.OnStartSpin(this);
        }

        private void YANCreateCoin(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CYanGear gear = CGlobal.GetGearByCode(nGearCode) as CYanGear;
            gear.OnCreateCoin(this);
        }

        private void YANGiftCashAdd(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            int nAddCash = Convert.ToInt32(json["nValue"]);
            CYanGear gear = CGlobal.GetGearByCode(nGearCode) as CYanGear;

            gear.OnAddGiftCash(this, nAddCash, 1);
        }

        private void YANFinishAnimation(string strPktData)
        {
            JToken json = JToken.Parse(strPktData);
            int nGearCode = Convert.ToInt32(json["nGearCode"]);

            if (!m_clsUser.m_lstGear.Exists(value => value.m_nGearCode == nGearCode))
                return;

            CYanGear gear = CGlobal.GetGearByCode(nGearCode) as CYanGear;
            gear.OnFinishAnimation();
        }
        #endregion

    }
}
