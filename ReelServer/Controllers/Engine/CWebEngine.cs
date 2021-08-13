using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ReelServer
{
    public class CWebResponse
    {
        public int nCode;             //결과코드
        public string strData;        //성공일때는 자료, 오유일때는 오유메세지
    }


    public static class CWebEngine
    {
        public static CWebResponse CheckAgentID(string strAgentID, ref CStore clsStore)
        {
            CWebResponse clsResponse = new CWebResponse();
            if (strAgentID == null || strAgentID == string.Empty)
            {
                clsResponse.nCode = 0x01;
                clsResponse.strData = "관리자아이디 누락";
            }
            else
            {
                clsStore = CGlobal.GetStoreByID(strAgentID);
                if (clsStore == null)
                {
                    clsResponse.nCode = 0x01;
                    clsResponse.strData = "관리자아이디가 존재하지 않음";
                }
            }

            return clsResponse;
        }

        public static CUser CheckUserID(CStore clsStore, string strUserID, CWebResponse clsResponse, string strIP="")
        {
            CUser clsUser = null;
            if (clsResponse == null)
                clsResponse = new CWebResponse();
            
            if(clsStore.m_nDemo == 1)
            {
                clsUser = CGlobal.GetUserList().Find(value => value.m_nStoreCode == clsStore.m_nStoreCode && value.m_strIP == strIP);
                if(clsUser == null)
                {
                    List<CUser> lstUser = CGlobal.GetUserList().FindAll(value => value.m_nStoreCode == clsStore.m_nStoreCode && value.m_nUserLogin == 0);
                    if (lstUser == null || lstUser.Count == 0)
                    {
                        clsResponse.nCode = 0x01;
                        clsResponse.strData = "사용불가능한 데모계정";
                    }
                    else
                    {
                        clsUser = CGlobal.RandomSelect(lstUser);
                    }
                }
            }
            else
            {
                if (strUserID == null || strUserID == string.Empty)
                {
                    clsResponse.nCode = 0x01;
                    clsResponse.strData = "유저아이디 누락";
                }
                else
                {
                    string strPlayerID = strUserID + clsStore.m_clsSubAdmin0.m_nCode.ToString("X") + clsStore.m_clsSubAdmin1.m_nCode.ToString("X") + clsStore.m_clsSubAdmin2.m_nCode.ToString("X") + clsStore.m_nStoreCode.ToString("X");
                    clsUser = CGlobal.GetUserByID(strPlayerID);

                    if (clsUser == null)
                    {
                        clsResponse.nCode = 0x01;
                        clsResponse.strData = "등록되지 않은 유저아이디";
                    }
                }
            }

            

            return clsUser;
        }

        public static CWebResponse CreatePlayer(CStore clsStore, string strUserID, string strUserNick, string strUserPW)
        {
            strUserID = strUserID.Trim();
            strUserNick = strUserNick.Trim();
            strUserNick = Uri.UnescapeDataString(strUserNick);

            CWebResponse clsResponse = new CWebResponse();

            if(string.IsNullOrEmpty(strUserID))
            {
                clsResponse.nCode = 0x01;
                clsResponse.strData = "유저아이디 누락";
            }
            else if(string.IsNullOrEmpty(strUserNick))
            {
                clsResponse.nCode = 0x01;
                clsResponse.strData = "유저닉네임 누락";
            }
            else if(string.IsNullOrEmpty(strUserPW))
            {
                clsResponse.nCode = 0x01;
                clsResponse.strData = "유저비번 누락";
            }
            else
            {
                string strPlayerID = strUserID + clsStore.m_clsSubAdmin0.m_nCode.ToString("X") + clsStore.m_clsSubAdmin1.m_nCode.ToString("X") + clsStore.m_clsSubAdmin2.m_nCode.ToString("X") + clsStore.m_nStoreCode.ToString("X");
                if (CGlobal.GetUserList().Exists(value=>value.m_strUserID == strPlayerID))
                {
                    clsResponse.nCode = 0x01;
                    clsResponse.strData = "유저아이디 증복";
                }
                else if(CGlobal.GetUserList().Exists(value => value.m_nStoreCode == clsStore.m_nStoreCode && value.m_strUserNick == strUserNick))
                {
                    clsResponse.nCode = 0x01;
                    clsResponse.strData = "유저닉네임 증복";
                }
                else
                {
                    //디비에 먼저 써야 한다.
                    string sql = "INSERT INTO tbl_user(userID, userNick, userPW, userMail, agenCode, storeCode, userRegTime, userState) ";
                    sql += "VALUES('" + strPlayerID + "', '" + strUserNick + "', '" + strUserPW + "', '" + strUserID + "', ";
                    sql += "0" + ", " + clsStore.m_nStoreCode + ", '" + CMyTime.GetMyTimeStr() + "', 1)";
                    CUser clsUser = CDataBase.InsertUserToDB(sql, strPlayerID);

                    if(clsUser == null)
                    {
                        clsResponse.nCode = 0x01;
                        clsResponse.strData = "회원등록실패";
                    }
                    else
                    {
                        //유저가 속한 매장객체에 유저를 추가한다.
                        clsStore.m_lstUser.Add(clsUser);
                        clsStore.m_nUserCnt++;

                        clsResponse.nCode = 0x00;
                        clsResponse.strData = "유저추가성공";
                    }
                }
            }

            return clsResponse;
        }

        public static CWebResponse GetPlayerToken(CStore clsStore, string strUserID, string strIP)
        {
            if (strUserID == null)
                strUserID = string.Empty;

            strUserID = strUserID.Trim();
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse, strIP);

            if(clsUser != null)
            {
                if (clsUser.m_nUserLogin == 0)
                    clsUser.m_strToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

                int nPort = CDefine.SRV_USER_SOCKET_PORT[CGlobal._nUserSocketIndex];
                CGlobal._nUserSocketIndex++;
                if (CGlobal._nUserSocketIndex >= CDefine.SRV_USER_SOCKET_PORT.Count)
                    CGlobal._nUserSocketIndex = 1;

                string strPacket = string.Empty;
                if (clsUser.m_nUserLogin == 0)
                {
                    while (true)
                    {
                        string strToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        strPacket = strToken + ":" + nPort;
                        strPacket = CEncrypt.Encrypt(strPacket);

                        if (!strPacket.Contains('+'))
                        {
                            clsUser.m_strToken = strToken;
                            break;
                        }
                    }
                    clsResponse.strData = strPacket;
                }
                else
                {
                    strPacket = clsUser.m_strToken + ":" + nPort;
                    strPacket = CEncrypt.Encrypt(strPacket);
                    clsResponse.strData = strPacket;
                }

                clsResponse.nCode = 0x00;
            }

            return clsResponse;
        }

        public static CWebResponse GetCash(CStore clsStore, string strUserID)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if(clsUser != null)
            {
                CUserCash clsUserCash = new CUserCash();
                clsUserCash.nUserCash = clsUser.m_nUserCash;
                clsUserCash.nUserCupn = clsUser.m_nUserCupn;
                clsUserCash.nVirtualCash = clsUser.m_nVirtualCash;
                clsUserCash.nVirtualCupn = clsUser.m_nVirtualCupn;

                clsResponse.nCode = 0x00;
                clsResponse.strData = JsonConvert.SerializeObject(clsUserCash);
            }
            
            return clsResponse;
        }

        public static CWebResponse ExchangeCupn(CStore clsStore, string strUserID, int nCupn)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if (clsUser != null)
            {
                if (clsUser.m_nChargeCnt > 0)
                {
                    if (nCupn > clsUser.m_nUserCupn)
                    {
                        clsResponse.nCode = 0x01;
                        clsResponse.strData = "신청한 쿠폰개수가 보유쿠폰개수보다 많습니다.";
                    }
                    else
                    {
                        string strNote = clsUser.m_strUserNick + "님이 " + nCupn + "개의 상품권을 게임머니 " + nCupn * 4500 + "원으로 전환";
                        CDataBase.InsertCacheToDB(clsUser.m_nUserCode, nCupn * 4500, -nCupn, clsUser.m_nUserCash, clsUser.m_nUserCupn, clsUser.m_nUserCash + nCupn * 4500, clsUser.m_nUserCupn, strNote);
                        clsUser.m_nUserCupn -= nCupn;
                        clsUser.m_nUserCash += (nCupn * 4500);

                        CUserCash clsUserCash = new CUserCash();
                        clsUserCash.nUserCash = clsUser.m_nUserCash;
                        clsUserCash.nUserCupn = clsUser.m_nUserCupn;

                        clsResponse.nCode = 0x00;
                        clsResponse.strData = JsonConvert.SerializeObject(clsUserCash);
                    }
                }
                else
                {
                    if (nCupn > clsUser.m_nVirtualCupn)
                    {
                        clsResponse.nCode = 0x01;
                        clsResponse.strData = "신청한 쿠폰개수가 보유체험쿠폰개수보다 많습니다.";
                    }
                    clsUser.m_nVirtualCupn -= nCupn;
                    clsUser.m_nVirtualCash += (nCupn * 4500);

                    CUserCash clsUserCash = new CUserCash();
                    clsUserCash.nVirtualCash = clsUser.m_nVirtualCash;
                    clsUserCash.nVirtualCupn = clsUser.m_nVirtualCupn;
                }
            }

            return clsResponse;
        }

        public static CWebResponse ChargeCash(CStore clsStore, string strUserID, int nCash)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if (clsUser != null)
            {
                //소속된 매장의 알을 감소시켜야한다.
                if(clsStore.m_nStoreCash < nCash)
                {
                    clsResponse.nCode = 0x01;
                    clsResponse.strData = "매장에 알이 모자랍니다. 알을 충전해주세요.";
                }
                else
                {
                    //우선 충전디비에 충전자료를 써넣어야 한다.
                    string sql = $"INSERT INTO tbl_storecharge(userCode, chgCash, chgTime, accTime, storeCode, chgCheck) VALUES({clsUser.m_nUserCode}, {nCash}, '{CMyTime.GetMyTimeStr()}', '{CMyTime.GetMyTimeStr()}', {clsStore.m_nStoreCode}, 1)";
                    CDataBase.PushOtherQuery(sql);


                    int nPreMoney = clsStore.m_nStoreCash;
                    clsStore.m_nStoreCash -= nCash;
                    CDataBase.SaveStoreInfoToDB(clsStore);
                    CDataBase.InsertStoreCash(clsStore.m_nStoreCode, -nCash, nPreMoney, clsStore.m_nStoreCash, 2, clsUser.m_strUserNick + "님 " + nCash.ToString("N0") + "원 충전되었습니다.");
                    clsUser.m_nUserCash += nCash;
                    clsUser.m_nUserCharge += nCash;
                    clsUser.m_nChargeCash += nCash;
                    clsUser.m_nChargeCnt++;

                    if (clsUser.m_nVirtualCupn > 0 || clsUser.m_nVirtualCash > 0 || (clsUser.m_nChargeCnt == 0 && clsUser.m_lstGear.Count > 0))
                    {
                        CGlobal.ClearUserVirtualInfo(clsUser);
                    }
                    CDataBase.SaveUserInfoToDB(clsUser);

                    CUserCash clsUserCash = new CUserCash();
                    clsUserCash.nUserCash = clsUser.m_nUserCash;
                    clsUserCash.nUserCupn = clsUser.m_nUserCupn;

                    clsResponse.nCode = 0x00;
                    clsResponse.strData = JsonConvert.SerializeObject(clsUserCash);

                    CGlobal.SendUserInfoToClient(clsUser);
                }
            }

            return clsResponse;
        }

        public static CWebResponse ExchargeCash(CStore clsStore, string strUserID, int nCash)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if (clsUser != null)
            {
                if(clsUser.m_nUserCash < nCash)
                {
                    clsResponse.nCode = 0x01;
                    clsResponse.strData = "환전신청금액이 보유금액보다 많습니다.";
                }
                else
                {
                    //우선 환전디비에 충전자료를 써넣어야 한다.
                    string sql = $"INSERT INTO tbl_storeexcharge(userCode, exCash, exTime, accTime, storeCode, exCheck) VALUES({clsUser.m_nUserCode}, {nCash}, '{CMyTime.GetMyTimeStr()}', '{CMyTime.GetMyTimeStr()}', {clsStore.m_nStoreCode}, 1)";
                    CDataBase.PushOtherQuery(sql);

                    clsUser.m_nUserCash -= nCash;
                    clsUser.m_nUserExcharge += nCash;
                    clsUser.m_nAppendCash = 0;
                    clsUser.m_nBonusCash = 0;
                    clsUser.m_nChargeCash = 0;
                    clsUser.m_nExchargeCnt++;
                    CDataBase.SaveUserInfoToDB(clsUser);

                    //매장의 알을 올려주어여야 한다.
                    int nPreMoney = clsStore.m_nStoreCash;
                    clsStore.m_nStoreCash += nCash;
                    CDataBase.SaveStoreInfoToDB(clsStore);
                    CDataBase.InsertStoreCash(clsStore.m_nStoreCode, nCash, nPreMoney, clsStore.m_nStoreCash, 3, clsUser.m_strUserNick + "님 " + nCash.ToString("N0") + "원 환전되었습니다.");

                    CUserCash clsUserCash = new CUserCash();
                    clsUserCash.nUserCash = clsUser.m_nUserCash;
                    clsUserCash.nUserCupn = clsUser.m_nUserCupn;

                    clsResponse.nCode = 0x00;
                    clsResponse.strData = JsonConvert.SerializeObject(clsUserCash);

                    CGlobal.SendUserInfoToClient(clsUser);
                }
            }

            return clsResponse;
        }

        public static CWebResponse GetChargeList(CStore clsStore, string strUserID, string strFromDate, string strToDate)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if (strFromDate == null)
                strFromDate = string.Empty;
            if (strToDate == null)
                strToDate = string.Empty;

            if(clsUser != null)
            {
                string sql = "SELECT tbl_user.userMail AS userID, tbl_user.userNick, tbl_storecharge.chgCash, tbl_storecharge.chgTime FROM tbl_storecharge ";
                sql += " LEFT JOIN tbl_user ON tbl_storecharge.userCode = tbl_user.userCode";
                sql += " WHERE tbl_user.userCode = " + clsUser.m_nUserCode + " AND tbl_storecharge.chgTime >= '" + strFromDate + "' AND tbl_storecharge.chgTime <= '" + strToDate + "' ORDER BY tbl_storecharge.chgTime DESC LIMIT 500";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if(list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }

                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }
            else
            {
                string sql = "SELECT tbl_user.userMail AS userID, tbl_user.userNick, tbl_storecharge.chgCash, tbl_storecharge.chgTime FROM tbl_storecharge ";
                sql += " LEFT JOIN tbl_user ON tbl_storecharge.userCode = tbl_user.userCode";
                sql += " WHERE tbl_user.storeCode = " + clsStore.m_nStoreCode + " AND tbl_storecharge.chgTime >= '" + strFromDate + "' AND tbl_storecharge.chgTime <= '" + strToDate + "' ORDER BY tbl_storecharge.chgTime DESC LIMIT 500";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }

                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }

            return clsResponse;
        }

        public static CWebResponse GetExchargeList(CStore clsStore, string strUserID, string strFromDate, string strToDate)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if (strFromDate == null)
                strFromDate = string.Empty;
            if (strToDate == null)
                strToDate = string.Empty;

            if (clsUser != null)
            {
                string sql = "SELECT tbl_user.userMail AS userID, tbl_user.userNick, tbl_storeexcharge.exCash, tbl_storeexcharge.exTime FROM tbl_storeexcharge ";
                sql += " LEFT JOIN tbl_user ON tbl_storeexcharge.userCode = tbl_user.userCode";
                sql += " WHERE tbl_user.userCode = " + clsUser.m_nUserCode + " AND tbl_storeexcharge.exTime >= '" + strFromDate + "' AND tbl_storeexcharge.exTime =< '" + strToDate + "' ORDER BY tbl_storeexcharge.exTime DESC LIMIT 500";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }

                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }
            else
            {
                string sql = "SELECT tbl_user.userMail AS userID, tbl_user.userNick, tbl_storeexcharge.exCash, tbl_storeexcharge.exTime FROM tbl_storeexcharge ";
                sql += " LEFT JOIN tbl_user ON tbl_storeexcharge.userCode = tbl_user.userCode";
                sql += " WHERE tbl_user.storeCode = " + clsStore.m_nStoreCode + " AND tbl_storeexcharge.exTime >= '" + strFromDate + "' AND tbl_storeexcharge.exTime =< '" + strToDate + "' ORDER BY tbl_storeexcharge.exTime DESC LIMIT 500";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }

                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }

            return clsResponse;
        }

        public static CWebResponse GetPrizeList(CStore clsStore, string strUserID, string strFromDate, string strToDate)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);

            if (strFromDate == null)
                strFromDate = string.Empty;
            if (strToDate == null)
                strToDate = string.Empty;

            if (clsUser != null)
            {
                string sql = "SELECT tbl_user.userMail AS userID, tbl_user.userNick, tbl_game.gameName, tbl_gear.gearNum, tbl_jackpot.jackAmount, tbl_jackpot.jackTime FROM tbl_jackpot ";
                sql += "LEFT JOIN tbl_user ON tbl_jackpot.userCode = tbl_user.userCode LEFT JOIN tbl_game ON tbl_jackpot.gameCode = tbl_game.gameCode ";
                sql += "LEFT JOIN tbl_gear ON tbl_jackpot.gearCode = tbl_gear.gearCode LEFT JOIN tbl_jackinfo ON tbl_jackpot.gameCode = tbl_jackinfo.gameCode AND tbl_jackpot.jackCont = tbl_jackinfo.jackCont ";
                sql += "WHERE tbl_jackpot.userCode = " + clsUser.m_nUserCode + " AND tbl_jackpot.jackTime >= '" + strFromDate + "' AND tbl_jackpot.jackTime <= '" + strToDate + "' ";
                sql += "ORDER BY tbl_jackpot.jackTime DESC LIMIT 500";

                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }

                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }

            else
            {
                string sql = "SELECT tbl_user.userMail AS userID, tbl_user.userNick, tbl_game.gameName, tbl_gear.gearNum, tbl_jackpot.jackAmount, tbl_jackpot.jackTime FROM tbl_jackpot ";
                sql += "LEFT JOIN tbl_user ON tbl_jackpot.userCode = tbl_user.userCode LEFT JOIN tbl_game ON tbl_jackpot.gameCode = tbl_game.gameCode ";
                sql += "LEFT JOIN tbl_gear ON tbl_jackpot.gearCode = tbl_gear.gearCode LEFT JOIN tbl_jackinfo ON tbl_jackpot.gameCode = tbl_jackinfo.gameCode AND tbl_jackpot.jackCont = tbl_jackinfo.jackCont ";
                sql += "WHERE tbl_user.storeCode = " + clsStore.m_nStoreCode + " AND tbl_jackpot.jackTime >= '" + strFromDate + "' AND tbl_jackpot.jackTime <= '" + strToDate + "' ";
                sql += "ORDER BY tbl_jackpot.jackTime DESC LIMIT 1000";

                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }

                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }

            return clsResponse;
        }

        public static CWebResponse GetAgentCash(CStore clsStore)
        {
            CWebResponse clsResponse = new CWebResponse();
            clsResponse.nCode = 0x00;
            clsResponse.strData = clsStore.m_nStoreCash.ToString();

            return clsResponse;
        }

        public static CWebResponse GetBettingList(CStore clsStore, string strUserID, int nDataIndex)
        {
            CWebResponse clsResponse = new CWebResponse();
            CUser clsUser = CheckUserID(clsStore, strUserID, clsResponse);


            if(clsUser != null)
            {
                string sql = $"SELECT tbl_user.userMail AS strUserID, tbl_gamename.gameName AS strGame, tbl_betting.* FROM tbl_betting LEFT JOIN tbl_user ON tbl_betting.nUserCode = tbl_user.userCode";
                sql += $" LEFT JOIN tbl_gamename ON tbl_betting.nGameCode = tbl_gamename.gameCode";
                sql += $" WHERE tbl_user.userCode = {clsUser.m_nUserCode} LIMIT 1000 OFFSET {nDataIndex}";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }
                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }
            else
            {
                string sql = $"SELECT tbl_user.userMail AS strUserID, tbl_gamename.gameName AS strGame, tbl_betting.* FROM tbl_betting LEFT JOIN tbl_user ON tbl_betting.nUserCode = tbl_user.userCode";
                sql += $" LEFT JOIN tbl_gamename ON tbl_betting.nGameCode = tbl_gamename.gameCode";
                sql += $" WHERE tbl_user.storeCode = {clsStore.m_nStoreCode} LIMIT 1000 OFFSET {nDataIndex}";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                string strData = string.Empty;
                if (list.Count > 0)
                {
                    strData = JsonConvert.SerializeObject(list[0].Table);
                }
                else
                {
                    strData = string.Empty;
                }
                clsResponse.nCode = 0x00;
                clsResponse.strData = strData;
            }

            return clsResponse;
        }
    }
    
}
