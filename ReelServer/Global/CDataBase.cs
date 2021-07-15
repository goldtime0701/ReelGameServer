using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Data;

namespace ReelServer
{
    public static class CDataBase
    {
        private static Queue<string> _lstQueryUser = new Queue<string>();
        private static Queue<string> _lstQueryGear = new Queue<string>();
        private static Queue<string> _lstQueryNational = new Queue<string>();
        private static Queue<string> _lstQueryOther = new Queue<string>();

        public static void pushSqlToQueryUser(string strSql)
        {
            _lstQueryUser.Enqueue(strSql);
        }

        public static void pushSqlToQueryGear(string strSql)
        {
            _lstQueryGear.Enqueue(strSql);
        }

        public static void pushSqlToQueryNational(string strSql)
        {
            _lstQueryNational.Enqueue(strSql);
        }

        public static void pushSqlToQueryOther(string strSql)
        {
            _lstQueryOther.Enqueue(strSql);
        }

        public static void ThreadExcuteQueryUser()
        {
            MySqlConnection mysqlCon = new MySqlConnection(CMysql.GetDBConnectString());

            string sql = string.Empty;
            while (true)
            {
                if (_lstQueryUser.Count > 0)
                {
                    mysqlCon.Open();
                    try
                    {
                        lock (_lstQueryUser)
                            sql = _lstQueryUser.Dequeue();
                        new MySqlCommand(sql, mysqlCon).ExecuteNonQuery();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    mysqlCon.Close();
                }
                Thread.Sleep(70);
            }
        }

        public static void ThreadExcuteQueryGear()
        {
            MySqlConnection mysqlCon = new MySqlConnection(CMysql.GetDBConnectString());

            string sql = string.Empty;
            while (true)
            {
                if (_lstQueryGear.Count > 0)
                {
                    mysqlCon.Open();
                    try
                    {
                        lock (_lstQueryGear)
                            sql = _lstQueryGear.Dequeue();
                        new MySqlCommand(sql, mysqlCon).ExecuteNonQuery();
                    }
                    catch
                    {

                    }
                    mysqlCon.Close();
                }
                Thread.Sleep(70);
            }
        }

        public static void ThreadExcuteQueryNational()
        {
            MySqlConnection mysqlCon = new MySqlConnection(CMysql.GetDBConnectString());

            string sql = string.Empty;
            while (true)
            {
                if (_lstQueryNational.Count > 0)
                {
                    mysqlCon.Open();
                    try
                    {
                        lock (_lstQueryNational)
                            sql = _lstQueryNational.Dequeue();
                        new MySqlCommand(sql, mysqlCon).ExecuteNonQuery();
                    }
                    catch
                    {

                    }
                    
                    mysqlCon.Close();
                }
                Thread.Sleep(70);
            }
        }

        public static void ThreadExcuteQueryOther()
        {
            MySqlConnection mysqlCon = new MySqlConnection(CMysql.GetDBConnectString());

            string sql = string.Empty;
            while (true)
            {

                if (_lstQueryOther.Count > 0)
                {
                    mysqlCon.Open();
                    try
                    {
                        lock (_lstQueryOther)
                            sql = _lstQueryOther.Dequeue();
                        new MySqlCommand(sql, mysqlCon).ExecuteNonQuery();
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    mysqlCon.Close();
                }
                Thread.Sleep(70);
            }
        }

        public static void SaveGearInfoToDB(CBaseGear gear)
        {
            string strYmt = string.Empty;
            
            if(gear.m_nGameCode == CDefine.GAME_YMT)
            {
                strYmt = gear.m_nLstGraph[0].ToString();
                for (int i = 1; i < 10; i++)
                {
                    strYmt += ",";
                    strYmt += gear.m_nLstGraph[i].ToString();
                }
            }
            else
            {
                strYmt = gear.m_nLeftWater + "," + gear.m_nRightWater;
            }

            string sql = "UPDATE tbl_gear SET slotCash = " + gear.m_nSlotCash + ", giftCash = " + gear.m_nGiftCash + ", gearState = " + gear.m_nGearState + ", gdcCash=" + gear.m_nGdcCash;
            sql += ", gearJack = " + gear.m_nGearJack + ", takeUser = " + gear.m_nTakeUser + ", takeRobot = " + gear.m_nTakeRobot + ", orderUser = " + gear.m_nOrderUser + ", ymtRound = '" + strYmt + "'";
            sql += ", accuCash = " + gear.m_nAccuCash + ", jackCash = " + gear.m_nJackCash + ", gearCheck = " + gear.m_nGearCheck + ", checkTime = '" + gear.m_dtCheckTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            sql += ", topJackCash = " + gear.m_nTopJackCash + ", lastJackCash = " + gear.m_nLastJackCash + ", lastJackTime = '" + gear.m_dtLastJackTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            sql += ", grandCount = " + gear.m_nGrandCount + ", majorCount = " + gear.m_nMajorCount + ", minorCount = " + gear.m_nMinorCount + ", miniCount = " + gear.m_nMiniCount;
            sql += " WHERE gearCode = " + gear.m_nGearCode;
            lock (_lstQueryGear)
                _lstQueryGear.Enqueue(sql);
        }

        public static void SaveUserLogin(int nUserCode)
        {
            string strLogTime = CMyTime.GetMyTimeStr();
            string sql1 = "INSERT INTO tbl_userLog(userCode, loginTime) VALUES(" + nUserCode + ", '" + strLogTime + "')";
            lock (_lstQueryUser)
                _lstQueryUser.Enqueue(sql1);
        }

        public static void SaveAbsentCheck(int nUserCode, int nDay, int nValue)
        {
            string sql = "UPDATE tbl_absent SET day" + nDay + " = " + nValue + " WHERE userCode = " + nUserCode;
            lock (_lstQueryOther)
                _lstQueryUser.Enqueue(sql);
        }

        public static void PushOtherQuery(string sql)
        {
            lock (_lstQueryOther)
                _lstQueryUser.Enqueue(sql);
        }

        public static void InserUserInfoToDB(CUser user)
        {
            string sql = $"INSERT INTO tbl_user(userID, userPW, userNick, userPhone, userState, userRegTime, agenCode, storeCode) VALUES('{user.m_strUserID}', '{user.m_strUserPW}', ";
            sql += $"'{user.m_strUserNick}', '{user.m_strUserPhone}', {user.m_nUserState}, '{user.m_strUserRegTime}', {user.m_nAgenCode}, {user.m_nStoreCode})";
            CMysql.ExcuteQuery(sql);
            sql = $"SELECT userCode FROM tbl_user WHERE userID = '{user.m_strUserID}'";
            DataRow info = CMysql.GetDataQuery(sql)[0];
            user.m_nUserCode = Convert.ToInt32(info["userCode"]);

            CGlobal.AddUser(user);
        }

        public static void SaveUserInfoToDB(CUser user)
        {
            string sql = "UPDATE tbl_user SET userID='" + user.m_strUserID + "', userPW='" + user.m_strUserPW + "'";
            sql += ", userNick='" + user.m_strUserNick + "', userMail='" + user.m_strUserMail + "'";
            sql += ", userPhone='" + user.m_strUserPhone + "', userName='" + user.m_strUserName + "'";
            sql += ", bankName='" + user.m_strBankName + "', bankAcid='" + user.m_strBankAcid + "'";
            sql += ", userLevel=" + user.m_nUserLevel + ", userCash=" + user.m_nUserCash;
            sql += ", userCupn=" + user.m_nUserCupn + ", agenCode=" + user.m_nAgenCode;
            sql += ", userState=" + user.m_nUserState + ", eventState=" + user.m_nEventState;
            sql += ", userCharge=" + user.m_nUserCharge + ", userExcharge=" + user.m_nUserExcharge;
            sql += ", absentCnt=" + user.m_nAbsentCnt + ", userUseCash=" + user.m_nUserUseCash;
            sql += ", userWinCash=" + user.m_nUserWinCash + ", userRealCash=" + user.m_nUserBonusCash;
            sql += ", appendCash=" + user.m_nAppendCash + ", bonusCash=" + user.m_nBonusCash;
            sql += ", chargeCash=" + user.m_nChargeCash + ", virtualCash=" + user.m_nVirtualCash;
            sql += ", virtualCupn=" + user.m_nVirtualCupn + ", chargeCnt=" + user.m_nChargeCnt;
            sql += ", userLogCnt=" + user.m_nUserLogCnt + ", token='" + user.m_strToken + "'";
            sql += ", userLogTime='" + user.m_strLogTime + "', chatBlock=" + user.m_nChatBlock;
            sql += ", strMemo='" + user.m_strMemo + "', chatBlockA=" + user.m_nChatBlockA;
            sql += ", absentCheck=" + user.m_nAbsentCheck;
            sql += " WHERE userCode = " + user.m_nUserCode;

            lock (_lstQueryUser)
                _lstQueryUser.Enqueue(sql);
        }

        public static void SaveRobotInfoToDB(CRobot robot)
        {
            string sql = "UPDATE tbl_robot SET rbLogin = " + robot.m_nRbLogin + ", gearCode = " + robot.m_nGearCode + ", gameCode = " + robot.m_nGameCode + ", jackpot = " + robot.m_nJackpot + " WHERE rbCode = " + robot.m_nRbCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void UpdateFaqTableToDB(int nFaqCode, string strAnswer)
        {
            strAnswer = strAnswer.Replace("\\", "");
            string sql = "UPDATE tbl_faq SET ansContent = '" + strAnswer + "', faqCheck = 1 WHERE faqCode = " + nFaqCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSecurityChatToDB(CSChatPacket chatPacket)
        {
            if (chatPacket.m_strMsg == string.Empty)
                return;

            string strMsg = chatPacket.m_strMsg.Replace("\\", "");
            strMsg = strMsg.Replace("'", ""); ;
            strMsg = strMsg.Replace(",", " ");
            string sql = "INSERT INTO tbl_chat(onerCode, userCode, ty, chatTime, chatContent) VALUES(" + chatPacket.m_nOnerCode + ", " + chatPacket.m_nUserCode + ", " + chatPacket.m_nType;
            sql += ", '" + chatPacket.m_strTime + "', '" + strMsg + "')";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertRobotChatToDB(CGChatPacket chatPacket)
        {
            string strMsg = chatPacket.m_strMsg.Replace("\\", "");
            strMsg = strMsg.Replace("'", "");

            string sql = "INSERT INTO tbl_robotChat(rcTime, onerCode, rcValue, rbCode) VALUES('" + chatPacket.m_strTime + "', " + chatPacket.m_nOnerCode + ", '" + strMsg + "', " + chatPacket.m_nCode + ")";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertGroupChatToDB(CGChatPacket chatPacket)
        {
            string strMsg = chatPacket.m_strMsg.Replace("\\", "");
            strMsg = strMsg.Replace("'", "");


            string sql = "INSERT INTO tbl_groupchat(userCode, gcKind, gcTime, gcChat) VALUES(" + chatPacket.m_nCode + ", " + chatPacket.m_nKind;
            sql += ", '" + chatPacket.m_strTime + "', '" + strMsg + "')";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);

        }

        public static void UpdateNoticeToDB(string strTitle, string strNotice)
        {
            string sql = "UPDATE tbl_notice SET stTitle = '" + strTitle + "',  stNotice = '" + strNotice + "'";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertCacheToDB(int nUserCode, int nCash, int nCupn, int nCash1, int nCupn1, int nCash2, int nCupn2, string strNote)
        {
            string strTime = CMyTime.GetMyTimeStr();
            string sql = "INSERT INTO tbl_cache(cahTime, userCode, cahCash, cahCupn, cahNote, cahCash1, cahCupn1, cahCash2, cahCupn2) VALUES('" + strTime + "', " + nUserCode + ", ";
            sql += nCash + ", " + nCupn + ", '" + strNote + "', " + nCash1 + ", " + nCupn1 + ", " + nCash2 + ", " + nCupn2 + ")";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertBettingToDB(int nUserCode, int nCash, int nPreMoney, int nCurMoney, int nGameCode, int nTy)
        {
            string strTime = CMyTime.GetMyTimeStr();
            string sql = $"INSERT INTO tbl_betting(nTy, strTime, nCash, nPreMoney, nCurMoney, nUserCode, nGameCode) VALUES({nTy}, '{strTime}', {nCash}, {nPreMoney}, {nCurMoney}, {nUserCode}, {nGameCode})";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }


        public static void InsertNationalToDB(int nGearCode, int nCash, int nStep)
        {
            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            int nGameCode = clsGear.m_nGameCode;
            int nUserCode = clsGear.m_nTakeUser;
            string strTime = CMyTime.GetMyTimeStr();
            string strUserNick = CGlobal.GetUserNickByCode(nUserCode);
            string strGameName = CGlobal.GetGameNameByGameCode(clsGear.m_nGameCode);
            int nGearNum = clsGear.m_nGearNum;
            string strNote = strUserNick + "님이 사용하는 " + strGameName + nGearNum.ToString() + "기대에 " + nStep + "차환수금액 " + nCash.ToString("N0") + "원을 지급하였습니다.";

            string sql = "INSERT INTO tbl_national(nalTime, userCode, gameCode, gearCode, nalCash, nalNote, nalStep) VALUES('" + strTime + "', " + nUserCode + ", " + nGameCode + ", " + nGearCode + ", " + nCash + ", '" + strNote + "', " + nStep + ")";

            lock (_lstQueryNational)
                _lstQueryNational.Enqueue(sql);
        }

        public static void InsertJackpotToDB(int nGameCode, int nOrderCode, int nUserCode, int nGearCode, int nJackAmount, int nJackCont, int nNalStep, int nIsRobot)
        {
            if (nJackAmount == 0)
                return;

            string strGame = CGlobal.GetGameNameByGameCode(nGameCode);
            string strAdminNick = CGlobal.GetOnerNickByCode(nOrderCode);

            int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
            string strTime = CMyTime.GetMyTimeStr();

            if (nIsRobot == 0)
            {
                string strUserNick = CGlobal.GetUserNickByCode(nUserCode);
                string strJackNote = strAdminNick + "님이 " + strUserNick + "님이 사용하는 " + strGame + nGearNum.ToString("000") + "기대에 ";
                strJackNote += nJackAmount.ToString("N0") + "원의 잭팟을 주었습니다.";

                string sql = "INSERT INTO tbl_jackpot(gameCode, onerCode, userCode, gearCode, jackAmount, jackNote, jackTime, jackCont, nalStep) ";
                sql += "VALUES(" + nGameCode + ", " + nOrderCode + ", " + nUserCode + ", " + nGearCode + ", " + nJackAmount + ", ";
                sql += "'" + strJackNote + "', '" + strTime + "', " + nJackCont + ", " + nNalStep + ")";

                lock (_lstQueryNational)
                    _lstQueryNational.Enqueue(sql);

            }
            else
            {
                string strUserNick = CGlobal.GetRobotNickByCode(nUserCode);
                string strJackNote = strAdminNick + "님이 " + strUserNick + "로봇이 사용하는 " + strGame + nGearNum.ToString("000") + "기대에 ";
                strJackNote += nJackAmount.ToString("N0") + "원의 잭팟을 주었습니다.";

                string sql = "INSERT INTO tbl_jackpot(gameCode, onerCode, rbCode, gearCode, jackAmount, jackNote, jackTime, jackCont, nalStep) ";
                sql += "VALUES(" + nGameCode + ", " + nOrderCode + ", " + nUserCode + ", " + nGearCode + ", " + nJackAmount + ", ";
                sql += "'" + strJackNote + "', '" + strTime + "', " + nJackCont + ", " + nNalStep + ")";

                lock (_lstQueryNational)
                    _lstQueryNational.Enqueue(sql);
            }

            CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
            CJackInfoModel jackInfo = new CJackInfoModel();
            jackInfo.m_strGameName = CGlobal.GetGameNameByGameCode(nGameCode) + " " + nGearNum.ToString("000");
            jackInfo.m_strJackCash = nJackAmount.ToString("N0");
            jackInfo.m_strJackDate = CMyTime.GetMyTimeStr("yyyy-MM-dd");
            jackInfo.m_strJackName = CGlobal.GetJackNameByGameCode_JackCont(nGameCode, nJackCont);

            gear.m_lstJackInfo.Insert(0, jackInfo);
            if (gear.m_lstJackInfo.Count > 100)
                gear.m_lstJackInfo.RemoveAt(gear.m_lstJackInfo.Count - 1);

            //string sql1 = "UPDATE tbl_gear SET jackCash = jackCash + " + nJackAmount + " WHERE gearCode = " + nGearCode;
            //lock (_lstQueryNational)
            //    _lstQueryNational.Enqueue(sql1);
        }

        public static void GetGameListFromDB(List<CGameEngine> lstGame)
        {
            string sql = "SELECT * FROM tbl_game";
            DataRowCollection games = CMysql.GetDataQuery(sql);

            for (int i = 0; i < games.Count; i++)
            {
                int nGameCode = Convert.ToInt32(games[i]["gameCode"]);
                CGameEngine clsGame = new CGameEngine(nGameCode);

                clsGame.m_strGameID = Convert.ToString(games[i]["gameID"]);
                clsGame.m_strGameName = Convert.ToString(games[i]["gameName"]);
                clsGame.m_nGameUse = Convert.ToInt32(games[i]["gameUse"]);
                clsGame.m_nCashAppend = Convert.ToInt32(games[i]["cashAppend"]);
                clsGame.m_nAutoPrize = Convert.ToInt32(games[i]["autoPrize"]);

                clsGame.m_nGameCash = Convert.ToInt32(games[i]["gameCash"]);
                clsGame.m_nGameCash1 = Convert.ToInt32(games[i]["gameCash1"]);
                clsGame.m_nGameCash2 = Convert.ToInt32(games[i]["gameCash2"]);
                clsGame.m_nGameCash3 = Convert.ToInt32(games[i]["gameCash3"]);

                clsGame.m_nGameRate = Convert.ToInt32(games[i]["gameRate"]);
                clsGame.m_nGameRate1 = Convert.ToInt32(games[i]["gameRate1"]);
                clsGame.m_nGameRate2 = Convert.ToInt32(games[i]["gameRate2"]);
                clsGame.m_nGameRate3 = Convert.ToInt32(games[i]["gameRate3"]);

                clsGame.m_clsTotalPot.m_nFirst = Convert.ToInt32(games[i]["passCash1"]);
                clsGame.m_clsTotalPot.m_nSecond = Convert.ToInt32(games[i]["passCash2"]);
                clsGame.m_clsTotalPot.m_nThird = Convert.ToInt32(games[i]["passCash3"]);

                clsGame.m_clsGivePot.m_nAppenCash = Convert.ToInt32(games[i]["totalAppendCash"]);
                clsGame.m_clsGivePot.m_nGiveCash = Convert.ToInt32(games[i]["totalGiveCash"]);
                clsGame.m_clsGivePot.m_nZero = Convert.ToInt32(games[i]["giveCash"]);
                clsGame.m_clsGivePot.m_nFirst = Convert.ToInt32(games[i]["giveCash1"]);
                clsGame.m_clsGivePot.m_nSecond = Convert.ToInt32(games[i]["giveCash2"]);
                clsGame.m_clsGivePot.m_nThird = Convert.ToInt32(games[i]["giveCash3"]);

                lstGame.Add(clsGame);
            }

            CGlobal.Log("게임정보를 자료기지로 부터 로드하였습니다.");
        }

        public static void GetOnerListFromDB(List<CAdmin> lstOner)
        {
            string sql = "SELECT * FROM tbl_oner";
            DataRowCollection oners = CMysql.GetDataQuery(sql);
            for (int i = 0; i < oners.Count; i++)
            {
                CAdmin oner = new CAdmin();
                oner.m_nOnerCode = Convert.ToInt32(oners[i]["onerCode"]);
                oner.m_strOnerID = Convert.ToString(oners[i]["onerID"]);
                oner.m_strOnerPW = Convert.ToString(oners[i]["onerPW"]);
                oner.m_strOnerNick = Convert.ToString(oners[i]["onerNick"]);
                oner.m_strOnerMail = Convert.ToString(oners[i]["onerMail"]);
                oner.m_strOnerPhone = Convert.ToString(oners[i]["onerPhone"]);
                oner.m_nOnerLevel = Convert.ToInt32(oners[i]["onerLevel"]);
                oner.m_strOnerRegTime = Convert.ToString(oners[i]["onerRegTime"]);
                oner.m_strJackPw = Convert.ToString(oners[i]["jackPw"]);

                lstOner.Add(oner);
            }

            CGlobal.Log("관리자정보를 자료기지로 부터 로드하였습니다.");
        }

        public static void GetAgentListFromDB(List<CAgent> lstAgent)
        {
            string sql = "SELECT * FROM tbl_agent";
            DataRowCollection agents = CMysql.GetDataQuery(sql);
            for (int i = 0; i < agents.Count; i++)
            {
                CAgent agent = new CAgent();
                agent.m_nAgenCode = Convert.ToInt32(agents[i]["agenCode"]);
                agent.m_strAgenID = Convert.ToString(agents[i]["agenID"]);
                agent.m_strAgenPW = Convert.ToString(agents[i]["agenPW"]);
                agent.m_strAgenNick = Convert.ToString(agents[i]["agenNick"]);
                agent.m_strAgenMail = Convert.ToString(agents[i]["agenMail"]);
                agent.m_strAgenPhone = Convert.ToString(agents[i]["agenPhone"]);
                agent.m_strAgenMark = Convert.ToString(agents[i]["agenMark"]);
                agent.m_nAgenLevel = Convert.ToInt32(agents[i]["agenLevel"]);
                agent.m_nAgenCash = Convert.ToInt32(agents[i]["tempCash"]);
                agent.m_nAgenState = Convert.ToInt32(agents[i]["agenState"]);
                agent.m_fAgentPro = Convert.ToDecimal(agents[i]["agenPro"]);
                agent.m_strBankName = Convert.ToString(agents[i]["bankName"]);
                agent.m_strBankAcid = Convert.ToString(agents[i]["bankAcid"]);
                agent.m_strAgenRegTime = Convert.ToString(agents[i]["agenRegTime"]);
                agent.m_strDomain = Convert.ToString(agents[i]["agenDomain"]);
                agent.m_nIsStore = Convert.ToInt32(agents[i]["isStore"]);
                agent.m_nUserCount = Convert.ToInt32(agents[i]["userCount"]);

                lstAgent.Add(agent);
            }
            CGlobal.Log("총판정보를 자료기지로 부터 로드하였습니다.");
        }

        public static void GetSubAdminFromDB(CSubAdmin subAdmin)
        {
            string sql = "SELECT * FROM tbl_subadmin";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            foreach (DataRow row in list)
            {
                subAdmin.m_nCode = Convert.ToInt32(row["nCode"]);
                subAdmin.m_strSubID = Convert.ToString(row["strSubID"]);
                subAdmin.m_strSubNick = Convert.ToString(row["strSubNick"]);
                subAdmin.m_strSubPwd = Convert.ToString(row["strSubPwd"]);
                subAdmin.m_nTotalPro = Convert.ToInt32(row["nTotalPro"]);
                subAdmin.m_nSelfPro = Convert.ToInt32(row["nSelfPro"]);
                subAdmin.m_nOtherPro = Convert.ToInt32(row["nOtherPro"]);
                subAdmin.m_nRealCash = Convert.ToInt32(row["nRealCash"]);
                subAdmin.m_nEventCash = Convert.ToInt32(row["nEventCash"]);
                subAdmin.m_nState = Convert.ToInt32(row["nState"]);
                subAdmin.m_nChildCnt = Convert.ToInt32(row["nChildCnt"]);
                subAdmin.m_strRegTime = Convert.ToString(row["strRegTime"]);
                subAdmin.m_strPhone = Convert.ToString(row["strPhone"]);
                subAdmin.m_strMark = Convert.ToString(row["strMark"]);
                subAdmin.m_nTotalCharge = Convert.ToInt64(row["nTotalCharge"]);
                subAdmin.m_nTotalExcharge = Convert.ToInt64(row["nTotalExcharge"]);
                subAdmin.m_nTotalExReal = Convert.ToInt64(row["nTotalExReal"]);
                subAdmin.m_nExRealCash = Convert.ToInt64(row["nExRealCash"]);
                subAdmin.m_nExEventCash = Convert.ToInt64(row["nExEventCash"]);
            }
        }

        public static void GetSubAdmin0ListFromDB(List<CSubAdmin0> lstSubAdmin0)
        {
            string sql = "SELECT * FROM tbl_subadmin0";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            foreach(DataRow row in list)
            {
                CSubAdmin0 subAdmin0 = new CSubAdmin0();
                subAdmin0.m_nCode = Convert.ToInt32(row["nCode"]);
                subAdmin0.m_nSuperCode = Convert.ToInt32(row["nSuperCode"]);
                subAdmin0.m_strSubID = Convert.ToString(row["strSubID"]);
                subAdmin0.m_strSubNick = Convert.ToString(row["strSubNick"]);
                subAdmin0.m_strSubPwd = Convert.ToString(row["strSubPwd"]);
                subAdmin0.m_nTotalPro = Convert.ToInt32(row["nTotalPro"]);
                subAdmin0.m_nSelfPro = Convert.ToInt32(row["nSelfPro"]);
                subAdmin0.m_nOtherPro = Convert.ToInt32(row["nOtherPro"]);
                subAdmin0.m_nRealCash = Convert.ToInt32(row["nRealCash"]);
                subAdmin0.m_nEventCash = Convert.ToInt32(row["nEventCash"]);
                subAdmin0.m_nState = Convert.ToInt32(row["nState"]);
                subAdmin0.m_nChildCnt = Convert.ToInt32(row["nChildCnt"]);
                subAdmin0.m_strRegTime = Convert.ToString(row["strRegTime"]);
                subAdmin0.m_strPhone = Convert.ToString(row["strPhone"]);
                subAdmin0.m_strMark = Convert.ToString(row["strMark"]);
                subAdmin0.m_nExRealCash = Convert.ToInt64(row["nExRealCash"]);
                subAdmin0.m_nExEventCash = Convert.ToInt64(row["nExEventCash"]);

                lstSubAdmin0.Add(subAdmin0);
            }
            CGlobal.Log("영본사정보를 자료기지로 부터 로드하였습니다.");
        }

        public static void GetSubAdmin1ListFromDB(List<CSubAdmin1> lstSubAdmin1)
        {
            string sql = "SELECT * FROM tbl_subadmin1";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            foreach (DataRow row in list)
            {
                CSubAdmin1 subAdmin1 = new CSubAdmin1();
                subAdmin1.m_nCode = Convert.ToInt32(row["nCode"]);
                subAdmin1.m_nSuperCode = Convert.ToInt32(row["nSuperCode"]);
                subAdmin1.m_strSubID = Convert.ToString(row["strSubID"]);
                subAdmin1.m_strSubNick = Convert.ToString(row["strSubNick"]);
                subAdmin1.m_strSubPwd = Convert.ToString(row["strSubPwd"]);
                subAdmin1.m_nTotalPro = Convert.ToInt32(row["nTotalPro"]);
                subAdmin1.m_nSelfPro = Convert.ToInt32(row["nSelfPro"]);
                subAdmin1.m_nOtherPro = Convert.ToInt32(row["nOtherPro"]);
                subAdmin1.m_nRealCash = Convert.ToInt32(row["nRealCash"]);
                subAdmin1.m_nEventCash = Convert.ToInt32(row["nEventCash"]);
                subAdmin1.m_nState = Convert.ToInt32(row["nState"]);
                subAdmin1.m_nChildCnt = Convert.ToInt32(row["nChildCnt"]);
                subAdmin1.m_strRegTime = Convert.ToString(row["strRegTime"]);
                subAdmin1.m_strPhone = Convert.ToString(row["strPhone"]);
                subAdmin1.m_strMark = Convert.ToString(row["strMark"]);
                subAdmin1.m_nExRealCash = Convert.ToInt64(row["nExRealCash"]);
                subAdmin1.m_nExEventCash = Convert.ToInt64(row["nExEventCash"]);

                lstSubAdmin1.Add(subAdmin1);
            }
            CGlobal.Log("부본사정보를 자료기지로 부터 로드하였습니다.");
        }

        public static void GetSubAdmin2ListFromDB(List<CSubAdmin2> lstSubAdmin2)
        {
            string sql = "SELECT * FROM tbl_subadmin2";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            foreach (DataRow row in list)
            {
                CSubAdmin2 subAdmin2 = new CSubAdmin2();
                subAdmin2.m_nCode = Convert.ToInt32(row["nCode"]);
                subAdmin2.m_nSuperCode = Convert.ToInt32(row["nSuperCode"]);
                subAdmin2.m_strSubID = Convert.ToString(row["strSubID"]);
                subAdmin2.m_strSubNick = Convert.ToString(row["strSubNick"]);
                subAdmin2.m_strSubPwd = Convert.ToString(row["strSubPwd"]);
                subAdmin2.m_nTotalPro = Convert.ToInt32(row["nTotalPro"]);
                subAdmin2.m_nSelfPro = Convert.ToInt32(row["nSelfPro"]);
                subAdmin2.m_nOtherPro = Convert.ToInt32(row["nOtherPro"]);
                subAdmin2.m_nRealCash = Convert.ToInt32(row["nRealCash"]);
                subAdmin2.m_nEventCash = Convert.ToInt32(row["nEventCash"]);
                subAdmin2.m_nState = Convert.ToInt32(row["nState"]);
                subAdmin2.m_nChildCnt = Convert.ToInt32(row["nChildCnt"]);
                subAdmin2.m_strRegTime = Convert.ToString(row["strRegTime"]);
                subAdmin2.m_strPhone = Convert.ToString(row["strPhone"]);
                subAdmin2.m_strMark = Convert.ToString(row["strMark"]);
                subAdmin2.m_nExRealCash = Convert.ToInt64(row["nExRealCash"]);
                subAdmin2.m_nExEventCash = Convert.ToInt64(row["nExEventCash"]);

                lstSubAdmin2.Add(subAdmin2);
            }
            CGlobal.Log("파트너정보를 자료기지로 부터 로드하였습니다.");
        }


        public static void GetStoreListFromDB(List<CStore> lstStore)
        {
            string sql = "SELECT * FROM tbl_store";
            DataRowCollection stores = CMysql.GetDataQuery(sql);
            for(int i=0; i<stores.Count; i++)
            {
                CStore store = new CStore();
                store.m_nStoreCode = Convert.ToInt32(stores[i]["storeCode"]);
                store.m_strStoreID = Convert.ToString(stores[i]["storeID"]);
                store.m_strStorePW = Convert.ToString(stores[i]["storePW"]);
                store.m_strStoreNick = Convert.ToString(stores[i]["storeNick"]);
                store.m_nStoreCash = Convert.ToInt32(stores[i]["storeCash"]);
                store.m_nSuperCode = Convert.ToInt32(stores[i]["superCode"]);
                store.m_nStoreState = Convert.ToInt32(stores[i]["storeState"]);
                store.m_strStoreRegTime = Convert.ToString(stores[i]["storeRegTime"]);
                store.m_nStoreCharge = Convert.ToInt64(stores[i]["storeCharge"]);
                store.m_nStoreChargeCnt = Convert.ToInt32(stores[i]["storeChargeCnt"]);
                store.m_nStoreExcharge = Convert.ToInt64(stores[i]["storeExcharge"]);
                store.m_nStoreExchargeCnt = Convert.ToInt32(stores[i]["storeExchargeCnt"]);
                store.m_nUserCnt = Convert.ToInt32(stores[i]["userCnt"]);
                store.m_nStorePro = Convert.ToInt32(stores[i]["storePro"]);
                store.m_nStoreReal = Convert.ToInt32(stores[i]["storeReal"]);
                store.m_strLastTime = Convert.ToString(stores[i]["storeLastTime"]);
                store.m_nStoreExReal = Convert.ToInt32(stores[i]["storeExReal"]);
                store.m_nStoreExRealCnt = Convert.ToInt32(stores[i]["storeExRealCnt"]);
                store.m_nCupnPro = Convert.ToInt32(stores[i]["cupnPro"]);
                store.m_nExcupnReal = Convert.ToInt64(stores[i]["excupnReal"]);
                store.m_nDemo = Convert.ToInt32(stores[i]["demo"]);
                store.m_strStorePhone = Convert.ToString(stores[i]["strPhone"]);
                store.m_strStoreMark = Convert.ToString(stores[i]["storeMark"]);

                lstStore.Add(store);
            }
            CGlobal.Log("매장정보를 자료기지로 부터 로드하였습니다.");
        }



        public static void GetUserListFromDB(List<CUser> lstUser)
        {
            string sql = "SELECT * FROM tbl_user WHERE userState < 3";
            DataRowCollection users = CMysql.GetDataQuery(sql);
            for (int i = 0; i < users.Count; i++)
            {
                CUser user = new CUser();
                user.m_nUserCode = Convert.ToInt32(users[i]["userCode"]);
                user.m_strUserID = Convert.ToString(users[i]["userID"]);
                user.m_strUserPW = Convert.ToString(users[i]["userPW"]);
                user.m_strUserNick = Convert.ToString(users[i]["userNick"]);
                user.m_strUserMail = Convert.ToString(users[i]["userMail"]);
                user.m_strUserPhone = Convert.ToString(users[i]["userPhone"]);
                user.m_strUserName = Convert.ToString(users[i]["userName"]);
                user.m_strBankName = Convert.ToString(users[i]["bankName"]);
                user.m_strBankAcid = Convert.ToString(users[i]["bankAcid"]);
                user.m_nUserLevel = Convert.ToInt32(users[i]["userLevel"]);
                user.m_nUserCash = Convert.ToInt32(users[i]["userCash"]);
                user.m_nUserCupn = Convert.ToInt32(users[i]["userCupn"]);
                user.m_nAgenCode = Convert.ToInt32(users[i]["agenCode"]);
                user.m_nUserState = Convert.ToInt32(users[i]["userState"]);
                user.m_nEventState = Convert.ToInt32(users[i]["eventState"]);
                user.m_strUserRegTime = Convert.ToString(users[i]["userRegTime"]);
                user.m_nUserCharge = Convert.ToInt64(users[i]["userCharge"]);
                user.m_nUserExcharge = Convert.ToInt64(users[i]["userExcharge"]);
                user.m_nUserLogCnt = Convert.ToInt32(users[i]["userLogCnt"]);
                user.m_nAbsentCnt = Convert.ToInt32(users[i]["absentCnt"]);
                user.m_nUserUseCash = Convert.ToInt32(users[i]["userUseCash"]);
                user.m_nUserWinCash = Convert.ToInt32(users[i]["userWinCash"]);
                user.m_nUserBonusCash = Convert.ToInt32(users[i]["userRealCash"]);
                user.m_nChargeCnt = Convert.ToInt32(users[i]["chargeCnt"]);
                user.m_nExchargeCnt = Convert.ToInt32(users[i]["exchargeCnt"]);
                user.m_nAppendCash = Convert.ToInt32(users[i]["appendCash"]);
                user.m_nBonusCash = Convert.ToInt32(users[i]["bonusCash"]);
                user.m_nChargeCash = Convert.ToInt32(users[i]["chargeCash"]);
                user.m_nVirtualCash = Convert.ToInt32(users[i]["virtualCash"]);
                user.m_nVirtualCupn = Convert.ToInt32(users[i]["virtualCupn"]);
                user.m_strToken = Convert.ToString(users[i]["token"]);
                user.m_strLogTime = Convert.ToString(users[i]["userLogTime"]);
                user.m_nChatBlock = Convert.ToInt32(users[i]["chatBlock"]);
                user.m_nChatBlockA = Convert.ToInt32(users[i]["chatBlockA"]);
                user.m_nStoreCode = Convert.ToInt32(users[i]["storeCode"]);
                user.m_strMemo = Convert.ToString(users[i]["strMemo"]);
                user.m_nAbsentCheck = Convert.ToInt32(users[i]["absentCheck"]);

                if(user.m_nAgenCode > 0)
                {
                    CAgent clsAgent = CGlobal.GetAgentByCode(user.m_nAgenCode);
                    clsAgent.m_lstUser.Add(user);
                }
                else if(user.m_nStoreCode > 0)
                {
                    CStore clsStore = CGlobal.GetStoreByCode(user.m_nStoreCode);
                    clsStore.m_lstUser.Add(user);
                }
                


                sql = "SELECT * FROM tbl_absent WHERE userCode = " + user.m_nUserCode;
                DataRowCollection absents = CMysql.GetDataQuery(sql);
                if (absents == null || absents.Count == 0)
                {
                    sql = "INSERT INTO tbl_absent(userCode) VALUES(" + user.m_nUserCode + ")";
                    CMysql.ExcuteQuery(sql);
                    sql = "SELECT * FROM tbl_absent WHERE userCode = " + user.m_nUserCode;
                    absents = CMysql.GetDataQuery(sql);
                }

                for (int j = 0; j < 31; j++)
                {
                    user.m_lstAsent[j] = Convert.ToInt32(absents[0]["day" + (j + 1).ToString()]);
                }


                lstUser.Add(user);
            }

            CGlobal.Log("유저정보를 자료기지로 부터 로드하였습니다.");
        }

        public static void GetitemListFromDB(List<CItem> lstItem)
        {
            string sql = "SELECT * FROM tbl_itemcache WHERE nUse = 0";
            DataRowCollection list = CMysql.GetDataQuery(sql);

            foreach(DataRow data in list)
            {
                int nUserCode = Convert.ToInt32(data["nUserCode"]);
                int nItemModel = Convert.ToInt32(data["nItemModel"]);
                int nBuyPrice = Convert.ToInt32(data["nBuyPrice"]);
                CItem item = new CItem(nUserCode, nItemModel, nBuyPrice);
                item.m_nItemCode = Convert.ToInt32(data["nItemCode"]);
                item.m_strBuyTime = Convert.ToString(data["strBuyTime"]);

                lstItem.Add(item);
            }
        }

        public static void GetItemPriceListFromDB(List<CItemModel> lstItemPrice)
        {
            string sql = "SELECT * FROM tbl_item";
            DataRowCollection list = CMysql.GetDataQuery(sql);

            foreach(DataRow data in list)
            {
                CItemModel itemPrice = new CItemModel(Convert.ToInt32(data["nCode"]));
                itemPrice.m_nGameCode = Convert.ToInt32(data["nGameCode"]);
                itemPrice.m_nPrice = Convert.ToInt32(data["nPrice"]);
                itemPrice.m_strItem = Convert.ToString(data["strItem"]);
                itemPrice.m_strNote = Convert.ToString(data["strNote"]);
                itemPrice.m_nLosePro = Convert.ToInt32(data["nLosePro"]);

                lstItemPrice.Add(itemPrice);
            }
        }
        public static void GetGearListFromDB(List<CBaseGear> lstGear)
        {
            string sql = "UPDATE tbl_gear SET gearJack = 0";
            CMysql.ExcuteQuery(sql);

            sql = "SELECT * FROM tbl_gear";
            DataRowCollection gears = CMysql.GetDataQuery(sql);
            for (int i = 0; i < gears.Count; i++)
            {
                int nGameCode = Convert.ToInt32(gears[i]["gameCode"]);
                CBaseGear gear = null;
                if (nGameCode == CDefine.GAME_JECHON)
                    gear = new CSwkGear();
                else if (nGameCode == CDefine.GAME_ALADIN)
                    gear = new CAldGear();
                else if (nGameCode == CDefine.GAME_DRAGON)
                    gear = new CFdgGear();
                else if (nGameCode == CDefine.GAME_SEA)
                    gear = new CSeaGear();
                else if (nGameCode == CDefine.GAME_GDC)
                    gear = new CGdcGear();
                else if (nGameCode == CDefine.GAME_ALADIN2)
                    gear = new CAldGear2();
                else if (nGameCode == CDefine.GAME_OCEAN)
                    gear = new COcaGear();
                else if (nGameCode == CDefine.GAME_NWD)
                    gear = new CNwdGear();
                else if (nGameCode == CDefine.GAME_YMT)
                    gear = new CYmtGear();
                else if (nGameCode == CDefine.GAME_DVC)
                    gear = new CDvcGear();
                else if (nGameCode == CDefine.GAME_WHT)
                    gear = new CWhtGear();
                else if (nGameCode == CDefine.GAME_YAN)
                    gear = new CYanGear();

                if (CGlobal.GetGameEngineByCode(nGameCode) == null || CGlobal.GetGameEngineByCode(nGameCode).m_nGameUse == 0)
                {
                    continue;
                }

                gear.m_nGearCode = Convert.ToInt32(gears[i]["gearCode"]);
                gear.m_nGearNum = Convert.ToInt32(gears[i]["gearNum"]);
                gear.m_nSlotCash = Convert.ToInt32(gears[i]["slotCash"]);
                gear.m_nGiftCash = Convert.ToInt32(gears[i]["giftCash"]);
                gear.m_nGearState = Convert.ToInt32(gears[i]["gearState"]);
                gear.m_nGearJack = Convert.ToInt32(gears[i]["gearJack"]);
                gear.m_nTakeUser = Convert.ToInt32(gears[i]["takeUser"]);
                gear.m_nOrderUser = Convert.ToInt32(gears[i]["orderUser"]);
                gear.m_nAccuCash = Convert.ToInt32(gears[i]["accuCash"]);
                gear.m_nJackCash = Convert.ToInt32(gears[i]["jackCash"]);
                gear.m_nGdcCash = Convert.ToInt32(gears[i]["gdcCash"]);
                gear.m_nGameCode = Convert.ToInt32(gears[i]["gameCode"]);
                gear.m_nTakeRobot = Convert.ToInt32(gears[i]["takeRobot"]);
                gear.m_nGearCheck = Convert.ToInt32(gears[i]["gearCheck"]);
                gear.m_dtCheckTime = Convert.ToDateTime(gears[i]["checkTime"]);
                gear.m_nTopJackCash = Convert.ToInt32(gears[i]["topJackCash"]);
                gear.m_nLastJackCash = Convert.ToInt32(gears[i]["lastJackCash"]);
                try
                {
                    gear.m_dtLastJackTime = Convert.ToDateTime(gears[i]["lastJackTime"]);
                }
                catch
                {
                    gear.m_dtLastJackTime = CMyTime.GetMyTime();
                }
                gear.m_nGrandCount = Convert.ToInt32(gears[i]["grandCount"]);
                gear.m_nMajorCount = Convert.ToInt32(gears[i]["majorCount"]);
                gear.m_nMinorCount = Convert.ToInt32(gears[i]["minorCount"]);
                gear.m_nMiniCount = Convert.ToInt32(gears[i]["miniCount"]);

                if(nGameCode == CDefine.GAME_YMT)
                {
                    //야마토라면 그라프정보를 추가하여야 한다.
                    string strYmt = Convert.ToString(gears[i]["ymtRound"]);
                    if(strYmt != string.Empty)
                    {
                        string[] pa = strYmt.Split(',');
                        for (int j = 0; j < pa.Length; j++)
                        {
                            int nRound = Convert.ToInt32(pa[j]);
                            gear.m_nLstGraph[j] = nRound;
                        }
                        gear.m_nYmtRound = gear.m_nLstGraph[0];
                    }
                }
                else
                {
                    string strYmt = Convert.ToString(gears[i]["ymtRound"]);
                    if(strYmt != null && strYmt != string.Empty)
                    {
                        gear.m_nLeftWater = Convert.ToInt32(strYmt.Split(',')[0]);
                        gear.m_nRightWater = Convert.ToInt32(strYmt.Split(',')[1]);
                    }
                }
               

                if (gear.m_nTakeUser > 0)
                {
                    CGlobal.GetUserByCode(gear.m_nTakeUser).AddUserGear(gear);    //유저객체에 기어를 추가한다.
                }

                sql = "SELECT * FROM tbl_jackpot WHERE gearCode = " + gear.m_nGearCode + " ORDER BY jackTime DESC LIMIT 100";
                DataRowCollection list = CMysql.GetDataQuery(sql);
                foreach(DataRow info in list)
                {
                    CJackInfoModel jackInfo = new CJackInfoModel();
                    jackInfo.m_strGameName = CGlobal.GetGameNameByGameCode(nGameCode) + " " + gear.m_nGearNum.ToString("000");
                    jackInfo.m_strJackCash = Convert.ToInt32(info["jackAmount"]).ToString("N0");
                    jackInfo.m_strJackDate = Convert.ToString(info["jackTime"]).Substring(0, 10);
                    jackInfo.m_strJackName = CGlobal.GetJackNameByGameCode_JackCont(nGameCode, Convert.ToInt32(info["jackCont"]));

                    gear.m_lstJackInfo.Add(jackInfo);
                }

                lstGear.Add(gear);
            }

            CGlobal.Log("기계정보를 자료기지로 부터 로드하였습니다.");
        }


        public static void GetRobotListFromDB(List<CRobot> lstRobot)
        {
            string sql = "SELECT * FROM tbl_robot";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            for (int i = 0; i < rows.Count; i++)
            {
                CRobot robot = new CRobot();

                robot.m_nRbCode = Convert.ToInt32(rows[i]["rbCode"]);
                robot.m_strRbNick = Convert.ToString(rows[i]["rbNick"]);
                robot.m_nOnerCode = Convert.ToInt32(rows[i]["onerCode"]);
                robot.m_nGearCode = Convert.ToInt32(rows[i]["gearCode"]);
                robot.m_nGameCode = Convert.ToInt32(rows[i]["gameCode"]);
                robot.m_nRbLogin = Convert.ToInt32(rows[i]["rbLogin"]);
                robot.m_nJackpot = Convert.ToInt32(rows[i]["jackpot"]);
                robot.m_nAutoJack = Convert.ToInt32(rows[i]["autoJack"]);

                if(robot.m_nAutoJack == 1)
                {
                    robot.StartChangeGear();
                }

                lstRobot.Add(robot);

                if (robot.m_nJackpot == 1)
                {
                    robot.m_nJackpot = 0;
                    SaveRobotInfoToDB(robot);
                }


                if (robot.m_nGearCode > 0)
                {
                    CBaseGear gear = CGlobal.GetGearByCode(robot.m_nGearCode);
                    gear.m_nGearState = 1;
                    gear.m_nGearRun = 1;
                    gear.m_nGearJack = 0;
                    gear.m_nTakeRobot = robot.m_nRbCode;
                    SaveGearInfoToDB(gear);
                }
            }
        }

        public static void GetAbsentFromDB(CAbsent absent)
        {
            string sql = "SELECT * FROM tbl_notice";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            absent.m_nCheckCount = Convert.ToInt32(rows[0]["absentCnt"]);
            absent.m_nPreUseCash = Convert.ToInt32(rows[0]["absentCondition"]);
            absent.m_nGiveCupn = Convert.ToInt32(rows[0]["absentCupn"]);

            return;
        }

        public static void GetJackNameListFromDB(List<CJackNameModel> lstJackName)
        {
            string sql = "SELECT * FROM tbl_jackinfo";
            DataRowCollection list = CMysql.GetDataQuery(sql);

            foreach(DataRow info in list)
            {
                CJackNameModel jackNameModel = new CJackNameModel();
                jackNameModel.m_nGameCode = Convert.ToInt32(info["gameCode"]);
                jackNameModel.m_nJackCont = Convert.ToInt32(info["jackCont"]);
                jackNameModel.m_strJackName = Convert.ToString(info["jackName"]);

                lstJackName.Add(jackNameModel);
            }
        }

        public static void GetBonusVirtualFromDB(CBonusVirtual bonusVirtual)
        {
            string sql = "SELECT * FROM tbl_notice";
            DataRow row = CMysql.GetDataQuery(sql)[0];

            bonusVirtual.m_nVirtual = Convert.ToInt32(row["virtual"]);
            bonusVirtual.m_nVirtualCash = Convert.ToInt32(row["virtualCash"]);
            bonusVirtual.m_nVirtualCupn = Convert.ToInt32(row["virtualCupn"]);
            bonusVirtual.m_nBonus = Convert.ToInt32(row["bonus"]);
            bonusVirtual.m_nBonusPro = Convert.ToDouble(row["bonusPro"]);
            bonusVirtual.m_nBonusCupn = Convert.ToInt32(row["bonusCupn"]);
            bonusVirtual.m_nAutoExcharge = Convert.ToInt32(row["tempex"]);

            return;
        }

        public static int GetNoticeImage()
        {
            int nNotice = 0;
            string sql = "SELECT * FROM tbl_repair";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            if (rows.Count > 0)
            {
                nNotice = Convert.ToInt32(rows[0]["st"]);
            }

            return nNotice;
        }

        public static void SaveNoticeImageValue(int nValue)
        {
            string sql = $"UPDATE tbl_repair SET st = {nValue}";
            CMysql.ExcuteQuery(sql);
        }

        public static string GetNoticeFromDB()
        {
            string strNotice = string.Empty;
            string sql = "SELECT * FROM tbl_notice";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            if (rows.Count > 0)
            {
                strNotice = Convert.ToString(rows[0]["stNotice"]);
            }

            return strNotice;
        }

        public static string GetNoticeTitleFromDB()
        {
            string strNoticeTitle = string.Empty;
            string sql = "SELECT * FROM tbl_notice";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            if (rows.Count > 0)
            {
                strNoticeTitle = Convert.ToString(rows[0]["stTitle"]);
            }

            return strNoticeTitle;
        }

        public static string GetAcidFromDB(List<CAcidModel> lstAcid)
        {
            string str = string.Empty;
            string sql = "SELECT * FROM tbl_acid";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            if (rows.Count > 0)
            {
                for(int i = 0; i<rows.Count; i++)
                {
                    string strUserName = Convert.ToString(rows[i]["userName"]);
                    string strBankName = Convert.ToString(rows[i]["bankName"]);
                    string strAcid = Convert.ToString(rows[i]["acid"]);
                    int nStoreCode = Convert.ToInt32(rows[i]["storeCode"]);

                    str = strUserName + "," + strBankName + "," + strAcid;

                    CAcidModel cls = new CAcidModel();
                    cls.m_strAcid = str;
                    cls.m_nStoreCode = nStoreCode;

                    lstAcid.Add(cls);
                }
                
            }
            else
            {
                CAcidModel cls = new CAcidModel();
                cls.m_strAcid = "aaa,aaa,aaa";
                cls.m_nStoreCode = 0;

                lstAcid.Add(cls);
            }

            return str;
        }

        public static void ClearAsentCount(int nUserCode)
        {
            string sql = "UPDATE tbl_user SET absentCnt = 0 WHERE userCode = " + nUserCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void ClearAsent()
        {
            string sql = "UPDATE tbl_absent SET ";
            for (int i = 1; i <= 31; i++)
            {
                sql += "day" + i + " = 0";
                if (i < 31)
                    sql += ", ";
            }

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertVirtualExcargeToDB(CAutoExcharge autoExcharge, string strTime)
        {
            string sql = "INSERT INTO tbl_tempex(kind, accTime, cash, userNick) VALUES(1, '" + strTime + "', " + autoExcharge.m_nExCash + ", '" + CGlobal.GetRobotNickByCode(autoExcharge.m_nRbCode) + "')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void ClearGearWeekToDB()
        {
            string sql = "UPDATE tbl_gear SET jackCash = 0, grandCount=0, majorCount=0, minorCount=0, miniCount=0, jackCash=0 WHERE gameCode <> " + CDefine.GAME_YMT;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);

        }

        public static void ClearDayToDB()
        {
            CMysql.ExcuteStoredProceDayClear();

            //string strNextDay = CMyTime.AddTime(CMyTime.GetMyTime(), 0, 0, 0, 3).ToString("yyyy-MM-dd");
            //string strClearDay = CMyTime.AddTime(CMyTime.GetMyTime(), -15, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            //string strHisDay = CMyTime.AddTime(CMyTime.GetMyTime(), -100, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");

            //string sql = "INSERT INTO tbl_datecache(strDate) VALUES('" + strNextDay + "')";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_cache WHERE cahTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_groupchat WHERE gcTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_jackpot WHERE jackTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_national WHERE nalTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_onerlog WHERE lgTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_robotchat WHERE rcTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_tempex WHERE accTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_tempjack WHERE jackTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_userlog WHERE loginTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_storecache WHERE strTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_chat WHERE chatTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_faq WHERE faqTime < '" + strClearDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);

            //sql = "DELETE FROM tbl_agentex WHERE aexTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_charge WHERE accTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_excharge WHERE accTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_storecharge WHERE accTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_storeexcharge WHERE accTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_webcharge WHERE strChargeTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
            //sql = "DELETE FROM tbl_webexcharge WHERE strExchargeTime < '" + strHisDay + "'";
            //lock (_lstQueryOther)
            //    _lstQueryOther.Enqueue(sql);
        }

        public static void SaveWinCash(long nWinCash, string strTime)
        {
            string sql = "INSERT INTO tbl_wincash(winCash, winTime) VALUES(" + nWinCash + ", '" + strTime + "')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void ClearGearCash(int nGearCode)
        {
            string sql = "UPDATE tbl_gear SET slotCash = 0, giftCash = 0 WHERE gearCode = " + nGearCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void RemoveGearFromRobotToDB(int nRbCode)
        {
            string sql = "UPDATE tbl_robot SET gearCode = 0, gameCode = 0 WHERE rbCode = " + nRbCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static CUser InsertUserToDB(string sql, string strUserID)
        {
            CMysql.ExcuteQuery(sql);
            CUser user = GetUserInfoFromDB(strUserID);
            if (user.m_nAgenCode > 0)
            {
                sql = "UPDATE tbl_agent SET userCount = userCount + 1 WHERE agenCode = " + user.m_nAgenCode;
                lock (_lstQueryOther)
                    _lstQueryOther.Enqueue(sql);
            }
            else if(user.m_nStoreCode > 0)
            {
                sql = "UPDATE tbl_store SET userCnt = userCnt + 1 WHERE storeCode = " + user.m_nStoreCode;
                lock (_lstQueryOther)
                    _lstQueryOther.Enqueue(sql);
            }

            sql = "INSERT INTO tbl_absent(userCode) VALUES(" + user.m_nUserCode + ")";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);

            user.m_nUserState = 1;
            BackupUserToDB(user);

            CGlobal.Log("디비에 " + user.m_strUserNick + "님을 추가하였습니다.");

            return user;
        }

        public static CUser GetUserInfoFromDB(string strUserID)
        {
            //디비에 추가한 유저정보를 얻어온다.
            string sql = "SELECT * FROM tbl_user WHERE userID = '" + strUserID + "'";
            DataRowCollection lst = CMysql.GetDataQuery(sql);
            if (lst.Count == 0)
                return null;

            DataRow row = lst[0];
            CUser user = new CUser();
            user.m_nUserCode = Convert.ToInt32(row["userCode"]);
            user.m_strUserID = Convert.ToString(row["userID"]);
            user.m_strUserPW = Convert.ToString(row["userPW"]);
            user.m_strUserNick = Convert.ToString(row["userNick"]);
            user.m_strUserMail = Convert.ToString(row["userMail"]);
            user.m_strUserPhone = Convert.ToString(row["userPhone"]);
            user.m_strUserName = Convert.ToString(row["userName"]);
            user.m_strBankName = Convert.ToString(row["bankName"]);
            user.m_strBankAcid = Convert.ToString(row["bankAcid"]);
            user.m_nUserLevel = Convert.ToInt32(row["userLevel"]);
            user.m_nUserCash = Convert.ToInt32(row["userCash"]);
            user.m_nUserCupn = Convert.ToInt32(row["userCupn"]);
            user.m_nAgenCode = Convert.ToInt32(row["agenCode"]);
            user.m_nUserState = Convert.ToInt32(row["userState"]);
            user.m_nEventState = Convert.ToInt32(row["eventState"]);
            user.m_strUserRegTime = Convert.ToString(row["userRegTime"]);
            user.m_nUserCharge = Convert.ToInt64(row["userCharge"]);
            user.m_nUserExcharge = Convert.ToInt64(row["userExcharge"]);
            user.m_nUserLogCnt = Convert.ToInt32(row["userLogCnt"]);
            user.m_nAbsentCnt = Convert.ToInt32(row["absentCnt"]);
            user.m_nUserUseCash = Convert.ToInt32(row["userUseCash"]);
            user.m_nUserWinCash = Convert.ToInt32(row["userWinCash"]);
            user.m_nUserBonusCash = Convert.ToInt32(row["userRealCash"]);
            user.m_nChargeCnt = Convert.ToInt32(row["chargeCnt"]);
            user.m_nExchargeCnt = Convert.ToInt32(row["exchargeCnt"]);
            user.m_nAppendCash = Convert.ToInt32(row["appendCash"]);
            user.m_nBonusCash = Convert.ToInt32(row["bonusCash"]);
            user.m_nChargeCash = Convert.ToInt32(row["chargeCash"]);
            user.m_strToken = Convert.ToString(row["token"]);
            user.m_strLogTime = Convert.ToString(row["userLogTime"]);
            user.m_nChatBlock = Convert.ToInt32(row["chatBlock"]);
            user.m_nStoreCode = Convert.ToInt32(row["storeCode"]);

            CGlobal.AddUser(user);
            return user;
        }
        

        public static void BackupUserToDB(CUser user)
        {
            string sql = "INSERT INTO tbl_user_back(userID, userPW, userNick, userPhone, agenCode, userRegTime, userState, storeCode) ";
            sql += "VALUES('" + user.m_strUserID + "', '" + user.m_strUserPW + "', ";
            sql += "'" + user.m_strUserNick + "', '" + user.m_strUserPhone + "', ";
            sql += user.m_nAgenCode + ", '" + user.m_strUserRegTime + "', 1, " + user.m_nStoreCode + ")";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertUserFaqToDB(int nUserCode, string strFaq, string strTime, int nAcid = 0)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            string sql = string.Empty;
            if (user.m_nStoreCode == 0)
            {
                sql = "INSERT tbl_faq(userCode, faqContent, faqTime, acid, storeCode, faqCheck) VALUES(" + nUserCode.ToString() + ", '" + strFaq + "', '" + strTime + "', " + nAcid + ", " + user.m_nStoreCode + ", 1)";
            }
            else
            {
                sql = "INSERT tbl_storefaq(userCode, faqContent, faqTime, acid, storeCode, faqCheck) VALUES(" + nUserCode.ToString() + ", '" + strFaq + "', '" + strTime + "', " + nAcid + ", " + user.m_nStoreCode + ", 1)";
            }
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertChargeToDB(string strBankName, string strBankAcid, string strUserName, string strUserPhone,
                                            int nChargeCash, int nUserCode, string strChargeTime)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            string sql = string.Empty;
            if(user.m_nStoreCode == 0)
            {
                sql = "INSERT INTO tbl_charge(userCode, bankName, bankAcid, userName, userPhone, chgCash, chgTime, storeCode) ";
                sql += "VALUES(" + nUserCode + ",'" + strBankName + "', '" + strBankAcid + "', '" + strUserName + "', '" + strUserPhone + "', " + nChargeCash + ",'" + strChargeTime + "', " + user.m_nStoreCode + ")";
            }
            else
            {
                sql = "INSERT INTO tbl_storecharge(userCode, bankName, bankAcid, userName, userPhone, chgCash, chgTime, storeCode) ";
                sql += "VALUES(" + nUserCode + ",'" + strBankName + "', '" + strBankAcid + "', '" + strUserName + "', '" + strUserPhone + "', " + nChargeCash + ",'" + strChargeTime + "', " + user.m_nStoreCode + ")";
            }
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
            CGlobal.Log("충전신청 DB 보관");

        }

        public static void InsertExchargeToDB(string strBankName, string strBankAcid, string strUserName, string strUserPhone,
                                            int nExchargeCash, int nUserCode, string strExchargeTime)
        {
            CUser user = CGlobal.GetUserByCode(nUserCode);
            string sql = string.Empty;
            if(user.m_nStoreCode == 0)
            {
                sql = "INSERT INTO tbl_excharge(userCode, bankName, bankAcid, userName, userPhone, exCash, exTime, storeCode) ";
                sql += "VALUES(" + nUserCode + ",'" + strBankName + "', '" + strBankAcid + "', '" + strUserName + "', '" + strUserPhone + "', " + nExchargeCash + ", '" + strExchargeTime + "', " + user.m_nStoreCode +  ")";
            }
            else
            {
                sql = "INSERT INTO tbl_storeexcharge(userCode, bankName, bankAcid, userName, userPhone, exCash, exTime, storeCode) ";
                sql += "VALUES(" + nUserCode + ",'" + strBankName + "', '" + strBankAcid + "', '" + strUserName + "', '" + strUserPhone + "', " + nExchargeCash + ", '" + strExchargeTime + "', " + user.m_nStoreCode + ")";
            }
            

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
            CGlobal.Log("환전신청 DB 보관");

        }

        public static void SaveGamePotToDB(CGameEngine clsEngine)
        {
            string sql = "UPDATE tbl_game SET passCash1=" + clsEngine.m_clsTotalPot.m_nFirst + ", passCash2=" + clsEngine.m_clsTotalPot.m_nSecond + ", passCash3=" + clsEngine.m_clsTotalPot.m_nThird;
            sql += ", giveCash=" + clsEngine.m_clsGivePot.m_nZero + ", giveCash1=" + clsEngine.m_clsGivePot.m_nFirst;
            sql += ", giveCash2=" + clsEngine.m_clsGivePot.m_nSecond + ", giveCash3=" + clsEngine.m_clsGivePot.m_nThird + ", totalAppendCash=" + clsEngine.m_clsGivePot.m_nAppenCash;
            sql += ", totalGiveCash=" + clsEngine.m_clsGivePot.m_nGiveCash + " WHERE gameCode=" + clsEngine.m_nGameCode;

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertOnerLog(int nOnderCode, string strMsg)
        {
            string sql = "INSERT INTO tbl_onerlog(logTime, onerCode, lgNote) VALUES('" + CMyTime.GetMyTimeStr() + "', " + nOnderCode + ", '" + strMsg + "')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertRealCash(int nGameCode, int nRealCash)
        {
            string sql = "INSERT INTO tbl_wincash(winCash, winTime, gameCode) VALUES(" + nRealCash + ", '" + CMyTime.GetMyTimeStr() + "', " + nGameCode + ")";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);

            UpdateDateCache(nRealCash, 2);
        }

        public static void UpdateDateCache(int nCash, int nTy)
        {
            CheckDateCacheFromDB();
            string sql = string.Empty;
            if(nTy == 0)
            {
                sql = "UPDATE tbl_datecache SET nChargeCash = nChargeCash + " + nCash + " WHERE strDate = '" + CMyTime.GetMyTimeStr("yyyy-MM-dd") + "'";
            }
            else if(nTy == 1)
            {
                sql = "UPDATE tbl_datecache SET nExchargeCash = nExchargeCash + " + nCash + " WHERE strDate = '" + CMyTime.GetMyTimeStr("yyyy-MM-dd") + "'";
            }
            else if(nTy == 2)
            {
                sql = "UPDATE tbl_datecache SET nGameCash = nGameCash + " + nCash + " WHERE strDate = '" + CMyTime.GetMyTimeStr("yyyy-MM-dd") + "'";
            }
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void CheckDateCacheFromDB()
        {
            string strDay = CMyTime.GetMyTimeStr("yyyy-MM-dd");
            string sql = "SELECT * FROM tbl_datecache WHERE strDate = '" + strDay + "'";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            if(rows.Count == 0)
            {
                sql = "INSERT INTO tbl_datecache(strDate) VALUES('" + strDay + "')";
                lock (_lstQueryOther)
                    _lstQueryOther.Enqueue(sql);
            }
        }

        public static void GetBlockIPListFromDB(List<string> lstIP)
        {
            string sql = "SELECT * FROM tbl_blockip";
            DataRowCollection rows = CMysql.GetDataQuery(sql);
            foreach(DataRow row in rows)
            {
                string strIP = Convert.ToString(row["strIP"]);
                lstIP.Add(strIP);
            }
        }

        public static void SaveSubAdmin(CSubAdmin subAdmin)
        {
            string sql = $"UPDATE tbl_subadmin SET strSubID='{subAdmin.m_strSubID}', strSubNick='{subAdmin.m_strSubNick}', nSelfPro={subAdmin.m_nSelfPro}, ";
            sql += $"nOtherPro={subAdmin.m_nOtherPro}, strSubPwd='{subAdmin.m_strSubPwd}', nRealCash={subAdmin.m_nRealCash}, nTotalPro={subAdmin.m_nTotalPro}, nEventCash={subAdmin.m_nEventCash}, ";
            sql += $"nState={subAdmin.m_nState}, nChildCnt={subAdmin.m_nChildCnt}, strRegTime='{subAdmin.m_strRegTime}', nTotalCharge={subAdmin.m_nTotalCharge}, ";
            sql += $"nTotalExcharge={subAdmin.m_nTotalExcharge}, nTotalExReal={subAdmin.m_nTotalExReal}, nExRealCash={subAdmin.m_nExRealCash}, nExEventCash={subAdmin.m_nExEventCash} ";
            sql += $"WHERE nCode = {subAdmin.m_nCode}";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdmin0(CSubAdmin0 subAdmin)
        {
            string sql = $"INSERT INTO tbl_subadmin0(strSubID, strSubNick, nSuperCode, strSubPwd, strPhone, strMark, strRegTime, nState) VALUES('{subAdmin.m_strSubID}', '{subAdmin.m_strSubNick}', {subAdmin.m_nSuperCode}, ";
            sql += $"'{subAdmin.m_strSubPwd}', '{subAdmin.m_strPhone}', '{subAdmin.m_strMark}', '{subAdmin.m_strRegTime}', {subAdmin.m_nState})";

            CMysql.ExcuteQuery(sql);
            sql = $"SELECT nCode FROM tbl_subAdmin0 WHERE strSubID = '{subAdmin.m_strSubID}'";
            DataRow info = CMysql.GetDataQuery(sql)[0];
            subAdmin.m_nCode = Convert.ToInt32(info["nCode"]);
        }

        public static void SaveSubAdmin0(CSubAdmin0 subAdmin)
        {
            string sql = $"UPDATE tbl_subadmin0 SET strSubID='{subAdmin.m_strSubID}', strSubNick='{subAdmin.m_strSubNick}', nSelfPro={subAdmin.m_nSelfPro}, ";
            sql += $"nOtherPro={subAdmin.m_nOtherPro}, strSubPwd='{subAdmin.m_strSubPwd}', nRealCash={subAdmin.m_nRealCash}, nTotalPro={subAdmin.m_nTotalPro}, nEventCash={subAdmin.m_nEventCash}, ";
            sql += $"nState={subAdmin.m_nState}, nChildCnt={subAdmin.m_nChildCnt}, strRegTime='{subAdmin.m_strRegTime}', nExRealCash={subAdmin.m_nExRealCash}, nExEventCash={subAdmin.m_nExEventCash} ";
            sql += $"WHERE nCode = {subAdmin.m_nCode}";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdmin1(CSubAdmin1 subAdmin)
        {
            string sql = $"INSERT INTO tbl_subadmin1(strSubID, strSubNick, nSuperCode, strSubPwd, strPhone, strMark, strRegTime, nState) VALUES('{subAdmin.m_strSubID}', '{subAdmin.m_strSubNick}', {subAdmin.m_nSuperCode}, ";
            sql += $"'{subAdmin.m_strSubPwd}', '{subAdmin.m_strPhone}', '{subAdmin.m_strMark}', '{subAdmin.m_strRegTime}', {subAdmin.m_nState})";

            CMysql.ExcuteQuery(sql);
            sql = $"SELECT nCode FROM tbl_subAdmin1 WHERE strSubID = '{subAdmin.m_strSubID}'";
            DataRow info = CMysql.GetDataQuery(sql)[0];
            subAdmin.m_nCode = Convert.ToInt32(info["nCode"]);
        }

        public static void SaveSubAdmin1(CSubAdmin1 subAdmin)
        {
            string sql = $"UPDATE tbl_subadmin1 SET strSubID='{subAdmin.m_strSubID}', strSubNick='{subAdmin.m_strSubNick}', nSelfPro={subAdmin.m_nSelfPro}, ";
            sql += $"nOtherPro={subAdmin.m_nOtherPro}, strSubPwd='{subAdmin.m_strSubPwd}', nRealCash={subAdmin.m_nRealCash}, nTotalPro={subAdmin.m_nTotalPro}, nEventCash={subAdmin.m_nEventCash}, ";
            sql += $"nState={subAdmin.m_nState}, nChildCnt={subAdmin.m_nChildCnt}, strRegTime='{subAdmin.m_strRegTime}', nExRealCash={subAdmin.m_nExRealCash}, nExEventCash={subAdmin.m_nExEventCash} ";
            sql += $"WHERE nCode = {subAdmin.m_nCode}";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdmin2(CSubAdmin2 subAdmin)
        {
            string sql = $"INSERT INTO tbl_subadmin2(strSubID, strSubNick, nSuperCode, strSubPwd, strPhone, strMark, strRegTime, nState) VALUES('{subAdmin.m_strSubID}', '{subAdmin.m_strSubNick}', {subAdmin.m_nSuperCode}, ";
            sql += $"'{subAdmin.m_strSubPwd}', '{subAdmin.m_strPhone}', '{subAdmin.m_strMark}', '{subAdmin.m_strRegTime}', {subAdmin.m_nState})";

            CMysql.ExcuteQuery(sql);
            sql = $"SELECT nCode FROM tbl_subAdmin2 WHERE strSubID = '{subAdmin.m_strSubID}'";
            DataRow info = CMysql.GetDataQuery(sql)[0];
            subAdmin.m_nCode = Convert.ToInt32(info["nCode"]);
        }

        public static void InsertStoreInfoToDB(CStore store)
        {
            string sql = $"INSERT INTO tbl_store(storeID, storeNick, storePW, superCode, strPhone, storeMark, storeState, storeRegTime, cupnPro) VALUES('{store.m_strStoreID}', '{store.m_strStoreNick}', '{store.m_strStorePW}', ";
            sql += $"{store.m_nSuperCode}, '{store.m_strStorePhone}', '{store.m_strStoreMark}', {store.m_nStoreState}, '{store.m_strStoreRegTime}', {store.m_nCupnPro})";
            CMysql.ExcuteQuery(sql);

            sql = $"SELECT storeCode FROM tbl_store WHERE storeID = '{store.m_strStoreID}'";
            DataRow info = CMysql.GetDataQuery(sql)[0];
            store.m_nStoreCode = Convert.ToInt32(info["storeCode"]);

            sql = $"INSERT INTO tbl_acid(userName, bankName, acid, storeCode) VALUES('홍길동', '', '', {store.m_nStoreCode})";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }


        public static void SaveSubAdmin2(CSubAdmin2 subAdmin)
        {
            string sql = $"UPDATE tbl_subadmin2 SET strSubID='{subAdmin.m_strSubID}', strSubNick='{subAdmin.m_strSubNick}', nSelfPro={subAdmin.m_nSelfPro}, ";
            sql += $"nOtherPro={subAdmin.m_nOtherPro}, strSubPwd='{subAdmin.m_strSubPwd}', nRealCash={subAdmin.m_nRealCash}, nTotalPro={subAdmin.m_nTotalPro}, nEventCash={subAdmin.m_nEventCash}, ";
            sql += $"nState={subAdmin.m_nState}, nChildCnt={subAdmin.m_nChildCnt}, strRegTime='{subAdmin.m_strRegTime}', nExRealCash={subAdmin.m_nExRealCash}, nExEventCash={subAdmin.m_nExEventCash} ";
            sql += $"WHERE nCode = {subAdmin.m_nCode}";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubProLog(int nCode, int nKind, int nSuperCode, int nPreTotalPro, int nCurTotalPro, int nPreSelfPro, int nCurSelfPro, int nPreOtherPro, int nCurOtherPro, string strNote)
        {
            string sql = $"INSERT INTO tbl_subpro(nSubCode, nKind, nSuperCode, nPreTotalPro, nCurTotalPro, nPreSelfPro, nCurSelfPro, nPreOtherPro, nCurOtherPro, strNote, strTime) ";
            sql += $"VALUES({nCode}, {nKind}, {nSuperCode}, {nPreTotalPro}, {nCurTotalPro}, {nPreSelfPro}, {nCurSelfPro}, {nPreOtherPro}, {nCurOtherPro}, '{strNote}', '{CMyTime.GetMyTimeStr()}')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubProLog0(int nCode, int nKind, int nSuperCode, int nPreTotalPro, int nCurTotalPro, int nPreSelfPro, int nCurSelfPro, int nPreOtherPro, int nCurOtherPro, string strNote)
        {
            string sql = $"INSERT INTO tbl_subpro0(nSubCode, nKind, nSuperCode, nPreTotalPro, nCurTotalPro, nPreSelfPro, nCurSelfPro, nPreOtherPro, nCurOtherPro, strNote, strTime) ";
            sql += $"VALUES({nCode}, {nKind}, {nSuperCode}, {nPreTotalPro}, {nCurTotalPro}, {nPreSelfPro}, {nCurSelfPro}, {nPreOtherPro}, {nCurOtherPro}, '{strNote}', '{CMyTime.GetMyTimeStr()}')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubProLog1(int nCode, int nKind, int nSuperCode, int nPreTotalPro, int nCurTotalPro, int nPreSelfPro, int nCurSelfPro, int nPreOtherPro, int nCurOtherPro, string strNote)
        {
            string sql = $"INSERT INTO tbl_subpro1(nSubCode, nKind, nSuperCode, nPreTotalPro, nCurTotalPro, nPreSelfPro, nCurSelfPro, nPreOtherPro, nCurOtherPro, strNote, strTime) ";
            sql += $"VALUES({nCode}, {nKind}, {nSuperCode}, {nPreTotalPro}, {nCurTotalPro}, {nPreSelfPro}, {nCurSelfPro}, {nPreOtherPro}, {nCurOtherPro}, '{strNote}', '{CMyTime.GetMyTimeStr()}')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubProLog2(int nCode, int nKind, int nSuperCode, int nPreTotalPro, int nCurTotalPro, int nPreSelfPro, int nCurSelfPro, int nPreOtherPro, int nCurOtherPro, string strNote)
        {
            string sql = $"INSERT INTO tbl_subpro2(nSubCode, nKind, nSuperCode, nPreTotalPro, nCurTotalPro, nPreSelfPro, nCurSelfPro, nPreOtherPro, nCurOtherPro, strNote, strTime) ";
            sql += $"VALUES({nCode}, {nKind}, {nSuperCode}, {nPreTotalPro}, {nCurTotalPro}, {nPreSelfPro}, {nCurSelfPro}, {nPreOtherPro}, {nCurOtherPro}, '{strNote}', '{CMyTime.GetMyTimeStr()}')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void SaveAgentInfoToDB(CAgent agent)
        {
            string sql = $"UPDATE tbl_agent SET agenID = '{agent.m_strAgenID}', agenPW = '{agent.m_strAgenPW}', agenNick = '{agent.m_strAgenNick}', agenMail = '{agent.m_strAgenMail}', agenPhone = '{agent.m_strAgenPhone}', ";
            sql += $"agenMark = '{agent.m_strAgenMark}', agenLevel = {agent.m_nAgenLevel}, agenCash = '{agent.m_nAgenCash}', agenState = '{agent.m_nAgenState}', agenPro = {agent.m_fAgentPro}, bankName = '{agent.m_strBankName}', ";
            sql += $"bankAcid = '{agent.m_strBankAcid}', agenRegTime = '{agent.m_strAgenRegTime}', userCount = {agent.m_nUserCount}, agenDomain = '{agent.m_strDomain}' WHERE agenCode = {agent.m_nAgenCode}";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void SaveAgentRealCash()
        {
            string sql = "UPDATE tbl_agent SET agenCash = tempCash";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }
        public static void SaveAgentRealCash(CAgent clsAgent)
        {
            string sql = "UPDATE tbl_agent SET tempCash = " + clsAgent.m_nAgenCash + ", agenCash = " + clsAgent.m_nAgenCash + " WHERE agenCode = " + clsAgent.m_nAgenCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void SaveAgentTempCash(CAgent clsAgent)
        {
            string sql = "UPDATE tbl_agent SET tempCash = " + clsAgent.m_nAgenCash + " WHERE agenCode = " + clsAgent.m_nAgenCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }


        public static void AddAgentRealCashToDB(int nAgenCode, int nRealCash)
        {
            //정산을 진행할때는 즉시 정산금액이 오르내리는것을 보여주기 위해 디비의 값을 먼저 덜어준다.
            string sql = "UPDATE tbl_agent SET agenCash = agenCash + " + nRealCash + " WHERE agenCode = " + nAgenCode;
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }


        public static void SaveStoreInfoToDB(CStore clsStore)
        {
            string sql = "UPDATE tbl_store SET storeID='" + clsStore.m_strStoreID + "', storePW='" + clsStore.m_strStorePW + "', ";
            sql += "storeNick='" + clsStore.m_strStoreNick + "', storeCash=" + clsStore.m_nStoreCash + ", superCode=" + clsStore.m_nSuperCode + ", ";
            sql += "storeState=" + clsStore.m_nStoreState + ", storeRegTime='" + clsStore.m_strStoreRegTime + "', storeCharge=" + clsStore.m_nStoreCharge + ", ";
            sql += "storeChargeCnt=" + clsStore.m_nStoreChargeCnt + ", storeExcharge=" + clsStore.m_nStoreExcharge + ", storeExchargeCnt=" + clsStore.m_nStoreExchargeCnt + ", ";
            sql += "userCnt=" + clsStore.m_nUserCnt + ", storePro=" + clsStore.m_nStorePro + ", storeReal=" + clsStore.m_nStoreReal + ", ";
            sql += "storeLastTime='" + clsStore.m_strLastTime + "', storeExReal=" + clsStore.m_nStoreExReal + ", storeExRealCnt=" + clsStore.m_nStoreExRealCnt + ", ";
            sql += "storeMark='" + clsStore.m_strStoreMark + "', strPhone = '" + clsStore.m_strStorePhone + "', cupnPro=" + clsStore.m_nCupnPro + ", excupnReal=" + clsStore.m_nExcupnReal + " ";
            sql += "WHERE storeCode=" + clsStore.m_nStoreCode;

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertStoreCash(int nStoreCode, int nCash, int nPreMoney, int nCurMoney, int nType, string strNote)
        {
            string sql = $"INSERT INTO tbl_storecache(nStoreCode, strTime, nCash, nPreMoney, nCurMoney, nType, strNote) VALUES({nStoreCode}, '{CMyTime.GetMyTimeStr()}', {nCash}, {nPreMoney}, {nCurMoney}, {nType}, '{strNote}')";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdmin2Cache(int nCode, int nCash, int nPreMoney, int nCurMoney, int nType, int nKind, string strNote)
        {
            string sql = $"INSERT INTO tbl_subcache2(nSubCode, nCash, nPreMoney, nCurMoney, strTime, strNote, nType, nKind) VALUES({nCode}, {nCash}, {nPreMoney}, {nCurMoney}, '{CMyTime.GetMyTimeStr()}', '{strNote}', {nType}, {nKind})";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdmin1Cache(int nCode, int nCash, int nPreMoney, int nCurMoney, int nType, int nKind, string strNote)
        {
            string sql = $"INSERT INTO tbl_subcache1(nSubCode, nCash, nPreMoney, nCurMoney, strTime, strNote, nType, nKind) VALUES({nCode}, {nCash}, {nPreMoney}, {nCurMoney}, '{CMyTime.GetMyTimeStr()}', '{strNote}', {nType}, {nKind})";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdmin0Cache(int nCode, int nCash, int nPreMoney, int nCurMoney, int nType, int nKind, string strNote)
        {
            string sql = $"INSERT INTO tbl_subcache0(nSubCode, nCash, nPreMoney, nCurMoney, strTime, strNote, nType, nKind) VALUES({nCode}, {nCash}, {nPreMoney}, {nCurMoney}, '{CMyTime.GetMyTimeStr()}', '{strNote}', {nType}, {nKind})";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void InsertSubAdminCache(int nCode, int nCash, int nPreMoney, int nCurMoney, int nType, int nKind, string strNote)
        {
            string sql = $"INSERT INTO tbl_subcache(nSubCode, nCash, nPreMoney, nCurMoney, strTime, strNote, nType, nKind) VALUES({nCode}, {nCash}, {nPreMoney}, {nCurMoney}, '{CMyTime.GetMyTimeStr()}', '{strNote}', {nType}, {nKind})";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void SaveItemModel(CItemModel itemModel)
        {
            string sql = $"UPDATE tbl_item SET strItem = '{itemModel.m_strItem}', strNote = '{itemModel.m_strNote}', nPrice = {itemModel.m_nPrice}, nLosePro = {itemModel.m_nLosePro} WHERE nCode = {itemModel.m_nItemCode}";
            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void BuyItemByUser(CItem item)
        {
            string sql = $"INSERT INTO tbl_itemcache(nUserCode, nItemModel, strBuyTime, nBuyPrice) VALUES({item.m_nUserCode}, {item.m_nItemModel}, '{item.m_strBuyTime}', {item.m_nBuyPrice})";
            CMysql.ExcuteQuery(sql);

            sql = $"SELECT * FROM tbl_itemcache WHERE nUserCode = {item.m_nUserCode} AND nItemModel = {item.m_nItemModel} ORDER BY strBuyTime DESC LIMIT 1";
            DataRow row = CMysql.GetDataQuery(sql)[0];
            int nItemCode = Convert.ToInt32(row["nItemCode"]);
            item.m_nItemCode = nItemCode;
        }

        public static void SaveItemToDB(CItem item)
        {
            string sql = $"UPDATE tbl_itemcache SET nUserCode={item.m_nUserCode}, strBuyTime='{item.m_strBuyTime}', nUse={item.m_nUse}, nBuyPrice={item.m_nBuyPrice}, ";
            sql += $"nGearCode ={item.m_nGearCode}, strUseTime='{item.m_strUseTime}', strNote='{item.m_strUseNote}', nItemModel={item.m_nItemModel} WHERE nItemCode={item.m_nItemCode}";

            lock (_lstQueryOther)
                _lstQueryOther.Enqueue(sql);
        }

        public static void GetItemEngineFromDB(CItemEngine engine)
        {
            string sql = "SELECT * FROM tbl_itemengine";
            DataRowCollection list = CMysql.GetDataQuery(sql);

            for(int i=0; i<list.Count; i++)
            {
                engine.m_nItemCash[i] = Convert.ToInt32(list[i]["nItemCash"]);
                engine.m_nRaiseCash[i] = Convert.ToInt32(list[i]["nRaiseCash"]);
                engine.m_nJackCash[i] = Convert.ToInt32(list[i]["nJackCash"]);
                if(i == 0)
                    engine.m_nRemCash = Convert.ToInt32(list[i]["nRemCash"]);
            }
        }

        public static void SaveItemEngine(CItemEngine engine)
        {
            for(int i=0; i<3; i++)
            {
                string sql = $"UPDATE tbl_itemengine SET nItemCash={engine.m_nItemCash[i]}, nRaiseCash={engine.m_nRaiseCash[i]}, nJackCash={engine.m_nJackCash[i]}, nRemCash={engine.m_nRemCash} WHERE nCode = {i + 1}";
                lock (_lstQueryOther)
                    _lstQueryOther.Enqueue(sql);
            }
        }
    }
}
