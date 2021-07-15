using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Threading;

namespace ReelServer
{
    public static class COnerEngine
    {
        public static string OnerLogin(int nOnerCode, string strValue)
        {
            string[] packet = strValue.Split(',');
            if (packet.Length != 3)
                return "failed";

            string strOnerID = packet[0];
            string strOnerPW = packet[1];
            string strSessionID = packet[2];

            CAdmin oner = CGlobal.GetAdminByOnerID(strOnerID);
            if (oner == null)
                return "failed";
            oner.m_strSessionID = strSessionID;

            return "success";
        }

        public static string OnerUpadteUserState(int nOnerCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);
            if (oner == null)
                return "failed";
           
            string[] strValues = strValue.Split(',');
            if (strValues.Length != 2)
                return "failed";

            int nUserCode = Convert.ToInt32(strValues[0]);
            int nUserState = Convert.ToInt32(strValues[1]);

            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user == null)
                return "오류가 발생하였습니다.";

            user.m_nUserState = nUserState;
            CDataBase.SaveUserInfoToDB(user);

            if (nUserState == 3)
            {
                if(user.m_nStoreCode > 0)
                {
                    CStore store = CGlobal.GetStoreByCode(user.m_nStoreCode);
                    store.m_nUserCnt--;
                    if (store.m_nUserCnt < 0)
                        store.m_nUserCnt = 0;
                }
                else
                {
                    CAgent agent = CGlobal.GetAgentByCode(user.m_nAgenCode);
                    agent.m_nUserCount--;
                    if (agent.m_nUserCount < 0)
                        agent.m_nUserCount = 0;
                }
                for (int i = 0; i < user.m_lstGear.Count; i++)
                {
                    user.m_lstGear[i].m_nTakeUser = 0;
                    user.m_lstGear[i].m_nGearState = 0;
                    user.m_lstGear[i].m_nGearJack = 0;
                    user.m_lstGear[i].m_nGearRun = 0;
                }
                CGlobal.RemoveUser(user);
            }

            return "success";
        }

        public static string OnerLogout(int nOnerCode)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);
            if (oner == null)
                return "failed";
            oner.m_bOnerLogin = false;

            return "success";
        }

        public static string OnerSaveUserInfo(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 2)
                return "failed";

            string strKey = values[0];

            CUser user;
            DataRowCollection users;

            if (strKey == "add")
            {
                int nUserCode = Convert.ToInt32(values[1]);
                user = new CUser();
                users = CMysql.GetDataQuery("SELECT * FROM tbl_user WHERE userCode = " + nUserCode);
            }
            else
            {
                int nUserCode = Convert.ToInt32(values[1]);
                user = CGlobal.GetUserByCode(nUserCode);
                users = CMysql.GetDataQuery("SELECT * FROM tbl_user WHERE userCode = " + nUserCode);
            }

            int i = 0;
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
            user.m_nUserCharge = Convert.ToInt32(users[i]["userCharge"]);
            user.m_nUserExcharge = Convert.ToInt32(users[i]["userExcharge"]);
            user.m_nUserLogCnt = Convert.ToInt32(users[i]["userLogCnt"]);
            user.m_nStoreCode = Convert.ToInt32(users[i]["storeCode"]);

            if (strKey == "add")
            {
                if(user.m_nStoreCode > 0)
                {
                    CStore clsStore = CGlobal.GetStoreByCode(user.m_nStoreCode);
                    clsStore.m_lstUser.Add(user);
                    clsStore.m_nUserCnt++;
                }
                else
                {
                    CAgent clsAgent = CGlobal.GetAgentByCode(user.m_nAgenCode);
                    clsAgent.m_lstUser.Add(user);
                    clsAgent.m_nUserCount++;

                    CDataBase.BackupUserToDB(user);
                }
                CGlobal.AddUser(user);

                string sql = "INSERT INTO tbl_absent(userCode) VALUES(" + user.m_nUserCode + ")";
                CDataBase.pushSqlToQueryOther(sql);
            }

            return "success";
        }

        public static string SaveUserInfoFromAdmin(int nOnerCode, string strValue)
        {
            JToken info = JToken.Parse(strValue);
            int nUserCode = Convert.ToInt32(info["nUserCode"]);
            string strUserID = Convert.ToString(info["strUserID"]);
            string strUserNick = Convert.ToString(info["strUserNick"]);
            string strUserPW = Convert.ToString(info["strUserPW"]);
            string strUserPhone = Convert.ToString(info["strUserPhone"]);
            string strUserMark = Convert.ToString(info["strUserMark"]);
            int nUserState = Convert.ToInt32(info["nUserState"]);
            string strApp = Convert.ToString(info["strApp"]);

            List<CUser> list = CGlobal.GetUserList();
            if (list.Exists(value => value.m_strUserID == strUserID && value.m_nUserCode != nUserCode))
                return "회원아이디가 증복되었습니다.";

            if (list.Exists(value => value.m_strUserNick == strUserNick && value.m_nUserCode != nUserCode))
                return "회원닉네임이 증복되었습니다.";

            if(nUserCode == 0)
            {
                //추가
                CUser user = new CUser();
                user.m_strUserID = strUserID;
                user.m_strUserNick = strUserNick;
                user.m_strUserPW = strUserPW;
                user.m_strUserPhone = strUserPhone;
                user.m_nUserState = nUserState;
                user.m_strUserRegTime = CMyTime.GetMyTimeStr();

                if(strApp == "APP")
                {
                    CAgent agent = CGlobal.GetAgentList().Find(value => value.m_strAgenMark == strUserMark);
                    if (agent == null)
                        return "가입코드를 정확히 입력하세요.";
                    user.m_nAgenCode = agent.m_nAgenCode;
                    agent.m_lstUser.Add(user);
                    agent.m_nUserCount++;
                    CDataBase.SaveAgentInfoToDB(agent);
                }
                else if(strApp == "WEB")
                {
                    CStore store = CGlobal.GetStoreList().Find(value => value.m_strStoreMark == strUserMark);
                    if(store == null)
                        return "가입코드를 정확히 입력하세요.";
                    user.m_nStoreCode = store.m_nStoreCode;
                    store.m_lstUser.Add(user);
                    store.m_nUserCnt++;
                    CDataBase.SaveStoreInfoToDB(store);
                }  
                CDataBase.InserUserInfoToDB(user);
                CDataBase.BackupUserToDB(user);
                string sql = "INSERT INTO tbl_absent(userCode) VALUES(" + user.m_nUserCode + ")";
                CDataBase.pushSqlToQueryOther(sql);

                return "추가되었습니다.";
            }
            else
            {
                //수정
                CUser user = CGlobal.GetUserByCode(nUserCode);
                user.m_strUserID = strUserID;
                user.m_strUserNick = strUserNick;
                user.m_strUserPW = strUserPW;
                user.m_strUserPhone = strUserPhone;
                user.m_nUserState = nUserState;

                CDataBase.SaveUserInfoToDB(user);

                return "수정되었습니다.";
            }
        }

        public static string OnerSWKCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 9)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackTy = Convert.ToInt32(values[4]);
            int nJackAmount = Convert.ToInt32(values[5]);
            int nActScore = Convert.ToInt32(values[6]);
            int nEvalScore = Convert.ToInt32(values[7]);
            int nRbCode = Convert.ToInt32(values[8]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_JECHON)
                return "잭팟설정중 오류가 발생했습니다.";

            CSwkGear gear = clsGear as CSwkGear;
            if (gear.m_nGearRun == 0)
                return "기대가 정지상태입니다.";
            if (gear.m_nTakeUser != nUserCode)
                return "유저코드가 서버정보와 일치하지 않습니다.";
            if (gear.m_nGameCode != nGameCode)
                return "게임코드가 서버정보와 일치하지 않습니다.";

            gear.SetSWKJackpotInfo(nJackCont, nJackTy, nJackAmount, nActScore, nEvalScore);

            bool ret = gear.MakePrizeRoll();
            string strRet = string.Empty;
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (nUserCode > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetSWKJackpotInfo().m_nJackCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CUser user = CGlobal.GetUserByCode(nUserCode);

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetSWKJackpotInfo().m_nJackCash, nUserCode, nGameCode)).Start();
                    }

                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetSWKJackpotInfo().m_nJackCash, nJackCont, 0, 0);
                    CGlobal.Log(strRet);
                }
                else if (nRbCode > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetSWKJackpotInfo().m_nJackCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetSWKJackpotInfo().m_nJackCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlaySwkPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }


        public static string OneALDCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_ALADIN)
                return "잭팟설정중 오류가 발생했습니다.";

            CAldGear gear = clsGear as CAldGear;
            if (gear.m_nGearRun == 0)
                return "기대가 정지상태입니다.";
            if (gear.m_nTakeUser != nUserCode)
                return "유저코드가 서버정보와 일치하지 않습니다.";
            if (gear.m_nGameCode != nGameCode)
                return "게임코드가 서버정보와 일치하지 않습니다.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            gear.SetALDJackpotInfo(nJackCont, nJackAmount);
            bool ret = gear.MakePrizeRoll();

            string strRet = string.Empty;
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (nUserCode > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetRealPrizeCash().ToString("N0") + "원의 잭팟을 주었습니다.";
                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetRealPrizeCash(), nUserCode, nGameCode)).Start();
                    }

                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetRealPrizeCash(), nJackCont, 0, 0);
                    CGlobal.Log(strRet);
                }
                else if (nRbCode > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetRealPrizeCash().ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetRealPrizeCash(), nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayAldPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OneALD2CallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_ALADIN2)
                return "잭팟설정중 오류가 발생했습니다.";

            CAldGear2 gear = clsGear as CAldGear2;
            if (gear.m_nGearRun == 0)
                return "기대가 정지상태입니다.";
            if (gear.m_nTakeUser != nUserCode)
                return "유저코드가 서버정보와 일치하지 않습니다.";
            if (gear.m_nGameCode != nGameCode)
                return "게임코드가 서버정보와 일치하지 않습니다.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            gear.SetALDJackpotInfo(nJackCont, nJackAmount);
            bool ret = gear.MakePrizeRoll();

            string strRet = string.Empty;
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (nUserCode > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetRealPrizeCash().ToString("N0") + "원의 잭팟을 주었습니다.";
                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetRealPrizeCash(), nUserCode, nGameCode)).Start();
                    }

                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetRealPrizeCash(), nJackCont, 0, 0);
                    CGlobal.Log(strRet);
                }
                else if (nRbCode > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetRealPrizeCash().ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetRealPrizeCash(), nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayAld2Prize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OneOcaCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_OCEAN)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            COcaGear gear = clsGear as COcaGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetOcaPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayOcaPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OnDvcCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_DVC)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CDvcGear gear = clsGear as CDvcGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetDvcPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayDvcPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OnWhtCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_WHT)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CWhtGear gear = clsGear as CWhtGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetWhtPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayWhtPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OnYanCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_YAN)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CYanGear gear = clsGear as CYanGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetYanPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayYanPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OneNwdCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_NWD)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CNwdGear gear = clsGear as CNwdGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetNwdPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayNwdPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OnYmtCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_YMT)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CYmtGear gear = clsGear as CYmtGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetYmtPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetOnerPrizeCash(nJackAmount);
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayYmtPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }


        public static string OneFDGCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 7)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nJackType = Convert.ToInt32(values[5]);
            int nRbCode = Convert.ToInt32(values[6]);
            
            if(nUserCode == 0 && nRbCode == 0)
                return "잭팟설정중 오류가 발생했습니다.";

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_DRAGON)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CFdgGear gear = clsGear as CFdgGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            if(nJackCont == 1)
            {
                if (nJackAmount <= 0)
                    return "잭팟금액을 입력해주세요.";

                int nMinCash = 0;
                int nMaxCash = 0;
                if (gear.m_nSpeedCash == 25)
                {
                    nMinCash = 10000;
                    nMaxCash = 200000;
                }
                else if (gear.m_nSpeedCash == 30)
                {
                    nMinCash = 15000;
                    nMaxCash = 300000;
                }
                else if (gear.m_nSpeedCash == 50)
                {
                    nMinCash = 20000;
                    nMaxCash = 400000;
                }
                else if (gear.m_nSpeedCash == 60)
                {
                    nMinCash = 30000;
                    nMaxCash = 600000;
                }
                else if (gear.m_nSpeedCash == 125)
                {
                    nMinCash = 50000;
                    nMaxCash = 1000000;
                }
                else if (gear.m_nSpeedCash == 150)
                {
                    nMinCash = 70000;
                    nMaxCash = 1500000;
                }
                else if (gear.m_nSpeedCash == 250)
                {
                    nMinCash = 100000;
                    nMaxCash = 2500000;
                }
                else if (gear.m_nSpeedCash == 300)
                {
                    nMinCash = 150000;
                    nMaxCash = 3000000;
                }
                else if (gear.m_nSpeedCash == 500)
                {
                    nMinCash = 200000;
                    nMaxCash = 4000000;
                }
                else if (gear.m_nSpeedCash == 600)
                {
                    nMinCash = 400000;
                    nMaxCash = 5000000;
                }

                if(nUserCode > 0)
                {
                    if (nJackAmount < nMinCash || nJackAmount > nMaxCash)
                    {
                        return "스핀캐시가 " + gear.m_nSpeedCash + "원인 기계에서 5드래곤 잭팟은 " + nMinCash.ToString("N0") + "원부터 " + nMaxCash.ToString("N0") + "원이여야 합니다.";
                    }
                }
                
            }

            bool ret = gear.SetFdgPrizeInfo(nJackCont, nJackAmount, nJackType);

            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    CUser user = CGlobal.GetUserByCode(nUserCode);

                    nJackAmount = gear.GetPrizeInfo().m_nPrizeCash;
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += (nJackAmount * 10).ToString("N0") + "원의 잭팟을 주었습니다.";

                    if(CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(nJackAmount * 10, nUserCode, nGameCode)).Start();
                    }

                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, nJackAmount * 10, nJackCont, 0, 0);
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += nJackAmount.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, nJackAmount, nJackCont, 0, 1);
                    CGlobal.Log(strRet);
                    //로봇이 사용한다면
                    gear.ClearPrizeInfo();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }

        public static string OneSeaCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_SEA)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CSeaGear gear = clsGear as CSeaGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetSEAPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeCash, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeCash.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeCash, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlaySeaPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }

            return strRet;
        }


        public static string OneGdcCallPrize(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');
            if (values.Length != 6)
                return "잘못된 파케트 전송";

            int nUserCode = Convert.ToInt32(values[0]);
            int nGameCode = Convert.ToInt32(values[1]);
            int nGearCode = Convert.ToInt32(values[2]);
            int nJackCont = Convert.ToInt32(values[3]);
            int nJackAmount = Convert.ToInt32(values[4]);
            int nRbCode = Convert.ToInt32(values[5]);

            CBaseGear clsGear = CGlobal.GetGearByCode(nGearCode);
            if (clsGear.m_nGameCode != CDefine.GAME_GDC)
                return "잭팟설정중 오류가 발생했습니다.";

            string strRet = string.Empty;
            CGdcGear gear = clsGear as CGdcGear;

            if (gear.m_nTakeUser != nUserCode && gear.m_nTakeRobot != nRbCode)
                return "해당기계를 사용하는 유저(로봇)정보가 틀립니다. 페이지를 다시 로드해 주세요.";

            CUser user = CGlobal.GetUserByCode(nUserCode);

            bool ret = gear.SetGdcPrizeCall(nJackCont, nJackAmount);
            if (ret)
            {
                string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);
                if (gear.m_nTakeUser > 0)
                {
                    string strUserNick = CGlobal.GetUserNickByCode(gear.m_nTakeUser);

                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strUserNick + "님에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeMoney.ToString("N0") + "점의 잭팟을 주었습니다.";

                    if (CGlobal.GetLevelByUserCode(gear.m_nTakeUser) < 10 && user.m_nChargeCnt > 0)
                    {
                        gear.SetGiveStep(4);
                        new Thread(() => CGlobal.RemoveNotionalJackStep(gear.GetPrizeInfo().m_nPrizeMoney, nUserCode, nGameCode)).Start();
                    }
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeUser, nGearCode, gear.GetPrizeInfo().m_nPrizeMoney, nJackCont, 0, 0);
                    //유저에게 잭팟정보를 보낸다.
                    CGlobal.Log(strRet);
                }
                else if (gear.m_nTakeRobot > 0)
                {
                    string strRbNick = CGlobal.GetRobotNickByCode(gear.m_nTakeRobot);
                    strRet = strGameName + nGearNum.ToString() + "기계를 사용중인 " + strRbNick + " 로봇에게 ";
                    strRet += gear.GetPrizeInfo().m_nPrizeMoney.ToString("N0") + "원의 잭팟을 주었습니다.";
                    CDataBase.InsertJackpotToDB(nGameCode, nOnerCode, gear.m_nTakeRobot, nGearCode, gear.GetPrizeInfo().m_nPrizeMoney, nJackCont, 0, 1);
                    CGlobal.Log(strRet);

                    CRobot robot = CGlobal.GetRobotByCode(gear.m_nTakeRobot);
                    robot.PlayGdcPrize();
                }
            }
            else
            {
                string strGameName = CGlobal.GetGameNameByGameCode(nGameCode);
                int nGearNum = CGlobal.GetGearNumByCode(nGearCode);

                strRet = strGameName + nGearNum.ToString() + "잭팟설정중 오류가 발생하였습니다. ";
            }


            return strRet;
        }



        public static string OnerClearGearCash(int nOnerCode, string strValue)
        {
            string strRet = string.Empty;
            int nGearCode = Convert.ToInt32(strValue);
            CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
            int nUserCode = gear.m_nTakeUser;
            CUser user = CGlobal.GetUserByCode(nUserCode);

            int nCash = gear.m_nSlotCash + gear.m_nGiftCash;
            gear.m_nSlotCash = 0;
            gear.m_nGiftCash = 0;

            //만일 금액초기화를 할때 기어에 있던 금액을 유저에게 돌려준다고 하면
            //user.m_nUserCash += nCash;
            CDataBase.ClearGearCash(nGearCode);
            CGlobal.SendClientClearGearCash(gear);
            CGlobal.SendGearInfoToClient(gear);

            string strGame = CGlobal.GetGameNameByGameCode(gear.m_nGameCode);
            strRet = strGame + gear.m_nGearNum.ToString() + "기계금액을 초기화 하였습니다.";
            CGlobal.SendOnerMessage(nUserCode, strRet);
            return strRet;
        }

        public static void OnerRepairGear(int nGearCode)
        {
            CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
            gear.ClearGear();
            List<CUser> lstUser = CGlobal.GetUserList().FindAll(value => value.m_lstGear.Exists(value1 => value1.m_nGearCode == nGearCode));

            for (int i = 0; i < lstUser.Count; i++)
            {
                lstUser[i].m_lstGear.Remove(gear);
            }
        }



        public static string OnerReleaseGear(int nOnerCode, string strValue)
        {
            string strRet = string.Empty;
            int nGearCode = Convert.ToInt32(strValue);
            CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
            int nUserCode = gear.m_nTakeUser;
            if (nUserCode > 0)
            {
                CUser user = CGlobal.GetUserByCode(nUserCode);
                int nCash = gear.m_nSlotCash + gear.m_nGiftCash;
                if (user.m_nChargeCnt > 0)
                {
                    user.m_nUserCash += nCash;
                }
                else
                {
                    user.m_nVirtualCash += nCash;
                }

                user.m_lstGear.Remove(gear);

                CDataBase.SaveUserInfoToDB(user);
                CGlobal.SendClientReleaseGear(gear);

                OnerRepairGear(nGearCode);
            }
            else
            {
                int nRbCode = gear.m_nTakeRobot;
                CRobot robot = CGlobal.GetRobotByCode(nRbCode);
                if (robot != null)
                {
                    robot.m_nGearCode = 0;
                    robot.m_nGameCode = 0;
                }

                CDataBase.RemoveGearFromRobotToDB(nRbCode);
            }

            gear.m_nGearRun = 0;
            gear.m_nSlotCash = 0;
            gear.m_nGiftCash = 0;
            gear.m_nGearState = 0;
            gear.m_nTakeRobot = 0;
            gear.m_nTakeUser = 0;
            gear.m_nGearCheck = 0;

            CDataBase.SaveGearInfoToDB(gear);

            string strGame = CGlobal.GetGameNameByGameCode(gear.m_nGameCode);
            strRet = strGame + gear.m_nGearNum.ToString() + "번 기계를 열림상태로 하였습니다.";
            CGlobal.SendOnerMessage(gear.m_nTakeUser, strRet);

            return strRet;
        }


        public static string OnerAcceptCharge(int nOnerCode, string strValue)
        {
            string[] values = strValue.Split(',');

            int nChgCode = Convert.ToInt32(values[0]);
            int nBonus = Convert.ToInt32(values[1]);
            string sql = $"SELECT * FROM tbl_charge WHERE chgCode = {nChgCode}";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            if(list == null || list.Count == 0)
            {
                return "해당자료가 디비에 존재하지 않습니다.";
            }
            DataRow info = list[0];
            if(Convert.ToInt32(info["chgCheck"]) == 1)
            {
                return "이미 승인한 신청입니다.";
            }
            
            int nUserCode = Convert.ToInt32(info["userCode"]);
            int nStoreCode = Convert.ToInt32(info["storeCode"]);
            int nChargeCash = Convert.ToInt32(info["chgCash"]);
            if (nStoreCode > 0)
                nBonus = 0;

            string strOnerNick = CGlobal.GetOnerNickByCode(nOnerCode);
            string strNote = string.Empty;
            if (nUserCode > 0)
            {
                CUser user = CGlobal.GetUserByCode(nUserCode);
                int nFirstBonusCash = 0;
                int nFirstBonusCupn = 0;
                if (user.m_nChargeCnt == 0)
                {
                    CBonusVirtual cls = CGlobal.GetBonusVirtual();
                    if (cls.m_nBonus == 1)
                    {
                        nFirstBonusCash = Convert.ToInt32(nChargeCash * cls.m_nBonusPro / 100);
                        nFirstBonusCupn = cls.m_nBonusCupn;
                    }
                }

                //소속된 총판의 정산을 진행하여야 한다.
                CAgent clsAgent = CGlobal.GetAgentByCode(user.m_nAgenCode);
                int nRealCash = Convert.ToInt32(nChargeCash * clsAgent.m_fAgentPro / 100);
                clsAgent.m_nAgenCash += nRealCash;
                CDataBase.SaveAgentTempCash(clsAgent);

                strNote = strOnerNick + "님이 " + user.m_strUserNick + "님에게 충전 " + nChargeCash.ToString("N0") + "원";
                if (nBonus > 0)
                    strNote += ", 관리자 보너스 " + nBonus.ToString("N0") + "원";
                if (nFirstBonusCash > 0)
                    strNote += ", 첫충전 보너스 " + nFirstBonusCash.ToString("N0") + "원";
                if (nFirstBonusCupn > 0)
                    strNote += ", 첫충전 보너스 쿠폰 " + nFirstBonusCash.ToString("N0");

                strNote += "을 승인하였습니다.";

                CDataBase.UpdateDateCache(nChargeCash, 0);
                CDataBase.InsertCacheToDB(user.m_nUserCode, nChargeCash + nBonus + nFirstBonusCash, nFirstBonusCupn, user.m_nUserCash, user.m_nUserCupn, user.m_nUserCash + nChargeCash + nBonus + nFirstBonusCash, user.m_nUserCupn + nFirstBonusCupn, strNote);

                user.m_nUserCash += nChargeCash + nBonus + nFirstBonusCash;
                user.m_nUserCupn += nFirstBonusCupn;
                user.m_nUserCharge += nChargeCash;
                user.m_nChargeCash += nChargeCash;
                user.m_nUserBonusCash += nBonus + nFirstBonusCash + nFirstBonusCupn * 4500;
                user.m_nBonusCash += nBonus + nFirstBonusCash + nFirstBonusCupn * 4500;
                user.m_nChargeCnt++;

                if (user.m_nVirtualCupn > 0 || user.m_nVirtualCash > 0 || (user.m_nChargeCnt == 0 && user.m_lstGear.Count > 0))
                {
                    CGlobal.ClearUserVirtualInfo(user);
                }

                CDataBase.SaveUserInfoToDB(user);
                CGlobal.SendUserInfoToClient(user);

                string strRet = "충전 " + nChargeCash.ToString("N0") + "원\n";
                if (nBonus > 0)
                    strRet += "관리자 보너스 " + nBonus.ToString("N0") + "원\n";
                if (nFirstBonusCash > 0)
                    strRet += "첫충전 보너스 " + nFirstBonusCash.ToString("N0") + "원\n";
                if (nFirstBonusCupn > 0)
                    strRet += "첫충전 보너스 쿠폰 " + nFirstBonusCash.ToString("N0") + "개\n";

                strRet += "지급되었습니다.";

                CGlobal.SendOnerMessage(nUserCode, strRet, 2);
            }
            else if(nStoreCode > 0)
            {
                CStore store = CGlobal.GetStoreByCode(nStoreCode);
                int nPreStoreCash = store.m_nStoreCash;
                store.m_nStoreCash += nChargeCash;
                store.m_nStoreCharge += nChargeCash;
                store.m_nStoreChargeCnt++;
                CDataBase.SaveStoreInfoToDB(store);
                CDataBase.InsertStoreCash(store.m_nStoreCode, nChargeCash, nPreStoreCash, store.m_nStoreCash, 0, nChargeCash.ToString("N0") + "원 알충전이 승인되었습니다.");

                string strLog = string.Empty;
                //소속된 총판의 정산을 진행하여야 한다.
                CSubAdmin2 subAdmin2 = store.m_clsSubAdmin2;
                int nSubPreMoney = subAdmin2.m_nRealCash;
                int nSelfPro = subAdmin2.m_nSelfPro;
                int nRealCash = Convert.ToInt32(nChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin2.m_nRealCash += nRealCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdmin2Cache(subAdmin2.m_nCode, nRealCash, nSubPreMoney, subAdmin2.m_nRealCash, 0, 0, strLog);
                }
                
                int nSubPreEvent = subAdmin2.m_nEventCash;
                int nEventPro = subAdmin2.m_nOtherPro;
                int nEventCash = Convert.ToInt32(nChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin2.m_nEventCash += nEventCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nEventPro}%";
                    CDataBase.InsertSubAdmin2Cache(subAdmin2.m_nCode, nEventCash, nSubPreEvent, subAdmin2.m_nEventCash, 0, 1, strLog);
                }
                CDataBase.SaveSubAdmin2(subAdmin2);

                CSubAdmin1 subAdmin1 = store.m_clsSubAdmin1;
                nSubPreMoney = subAdmin1.m_nRealCash;
                nSelfPro = subAdmin1.m_nSelfPro - subAdmin2.m_nTotalPro;
                nRealCash = Convert.ToInt32(nChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin1.m_nRealCash += nRealCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdmin1Cache(subAdmin1.m_nCode, nRealCash, nSubPreMoney, subAdmin1.m_nRealCash, 0, 0, strLog);
                }

                nSubPreEvent = subAdmin1.m_nEventCash;
                nEventPro = subAdmin1.m_nOtherPro;
                nEventCash = Convert.ToInt32(nChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin1.m_nEventCash += nEventCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nEventPro}%";
                    CDataBase.InsertSubAdmin1Cache(subAdmin1.m_nCode, nEventCash, nSubPreEvent, subAdmin1.m_nEventCash, 0, 1, strLog);
                }
                CDataBase.SaveSubAdmin1(subAdmin1);


                CSubAdmin0 subAdmin0 = store.m_clsSubAdmin0;
                nSubPreMoney = subAdmin0.m_nRealCash;
                nSelfPro = subAdmin0.m_nSelfPro - subAdmin1.m_nTotalPro;
                nRealCash = Convert.ToInt32(nChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin0.m_nRealCash += nRealCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdmin0Cache(subAdmin0.m_nCode, nRealCash, nSubPreMoney, subAdmin0.m_nRealCash, 0, 0, strLog);
                }

                nSubPreEvent = subAdmin0.m_nEventCash;
                nEventPro = subAdmin0.m_nOtherPro;
                nEventCash = Convert.ToInt32(nChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin0.m_nEventCash += nEventCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nEventPro}%";
                    CDataBase.InsertSubAdmin0Cache(subAdmin0.m_nCode, nEventCash, nSubPreEvent, subAdmin0.m_nEventCash, 0, 1, strLog);
                }
                CDataBase.SaveSubAdmin0(subAdmin0);

                CSubAdmin subAdmin = store.m_clsSubAdmin;
                nSubPreMoney = subAdmin.m_nRealCash;
                nSelfPro = subAdmin.m_nSelfPro - subAdmin0.m_nSelfPro;
                nRealCash = Convert.ToInt32(nChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin.m_nRealCash += nRealCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdminCache(subAdmin.m_nCode, nRealCash, nSubPreMoney, subAdmin.m_nRealCash, 0, 0, strLog);
                }

                nSubPreEvent = subAdmin.m_nEventCash;
                nEventPro = subAdmin.m_nOtherPro;
                nEventCash = Convert.ToInt32(nChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin.m_nEventCash += nEventCash;
                    strLog = $"{store.m_strStoreNick}매장 {nChargeCash}원 알충전 요율 {nEventPro}%";
                    CDataBase.InsertSubAdminCache(subAdmin.m_nCode, nEventCash, nSubPreEvent, subAdmin.m_nEventCash, 0, 1, strLog);
                }
                CDataBase.SaveSubAdmin(subAdmin);


                strNote = strOnerNick + "님이 " + store.m_strStoreNick + "매장 알충전 " + nChargeCash.ToString("N0") + "원";
                strNote += "을 승인하였습니다.";

                CDataBase.UpdateDateCache(nChargeCash, 0);
                CGlobal.GetSubAdmin().m_nTotalCharge += nChargeCash;
            }

            sql = $"UPDATE tbl_charge SET chgCheck = 1, accTime = '{CMyTime.GetMyTimeStr()}', onerCode = {nOnerCode}, bonus = {nBonus} WHERE chgCode = {nChgCode}";
            CMysql.ExcuteQuery(sql);
            
            CDataBase.SaveSubAdmin(CGlobal.GetSubAdmin());

            return strNote;
        }

        public static string SaveUserChargeFormStore(int nOnerCode, string strValue)
        {
            string[] packet = strValue.Split(',');
            int nUserCode = Convert.ToInt32(packet[0]);
            int nStoreCode = Convert.ToInt32(packet[1]);
            int nCash = Convert.ToInt32(packet[2]);

            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            CUser user = CGlobal.GetUserByCode(nUserCode);
            //우선 매장에 알이 충분한가를 검사한다.
            if(nCash > store.m_nStoreCash)
            {
                return "매장보유 알보다 신청금액이 작아야 합니다.";
            }


            //우선 충전디비에 충전자료를 써넣어야 한다.
            string sql = $"INSERT INTO tbl_storecharge(userCode, chgCash, chgTime, accTime, storeCode, chgCheck) VALUES({nUserCode}, {nCash}, '{CMyTime.GetMyTimeStr()}', '{CMyTime.GetMyTimeStr()}', {nStoreCode}, 1)";
            CDataBase.PushOtherQuery(sql);

            //유저금액을 증가시킨다.
            user.m_nUserCash += nCash;
            user.m_nUserCharge += nCash;
            user.m_nChargeCash += nCash;
            user.m_nChargeCnt++;
            string strNote = $"{store.m_strStoreNick}매장 {user.m_strUserNick} 유저에게 {nCash}원 넣기진행";
            CDataBase.InsertCacheToDB(nUserCode, nCash, 0, user.m_nUserCash - nCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn, strNote);
            CDataBase.SaveUserInfoToDB(user);

            //매장에서 알금액을 감소시켜야 한다.
            store.m_nStoreCash -= nCash;
            CDataBase.InsertStoreCash(nStoreCode, -nCash, store.m_nStoreCash + nCash, store.m_nStoreCash, 2, strNote);
            CDataBase.SaveStoreInfoToDB(store);

            CGlobal.SendUserInfoToClient(user);
            CGlobal.SendOnerMessage(nUserCode, strNote, 2);

            return strNote;
        }

        public static string SaveUserExchargeFormStore(int nOnerCode, string strValue)
        {
            string[] packet = strValue.Split(',');
            int nUserCode = Convert.ToInt32(packet[0]);
            int nStoreCode = Convert.ToInt32(packet[1]);
            int nCash = Convert.ToInt32(packet[2]);

            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            CUser user = CGlobal.GetUserByCode(nUserCode);
            //우선 매장에 알이 충분한가를 검사한다.
            if (nCash > user.m_nUserCash)
            {
                return "유저보유금액보다 신청금액이 작아야 합니다.";
            }


            //우선 환전디비에 충전자료를 써넣어야 한다.
            string sql = $"INSERT INTO tbl_storeexcharge(userCode, exCash, exTime, accTime, storeCode, exCheck) VALUES({nUserCode}, {nCash}, '{CMyTime.GetMyTimeStr()}', '{CMyTime.GetMyTimeStr()}', {nStoreCode}, 1)";
            CDataBase.PushOtherQuery(sql);

            //유저금액을 감소시킨다.
            user.m_nUserCash -= nCash;
            user.m_nUserExcharge += nCash;
            user.m_nChargeCash = 0;
            user.m_nExchargeCnt++;
            string strNote = $"{store.m_strStoreNick}매장 {user.m_strUserNick} 유저에게 {nCash}원 빼기진행";
            CDataBase.InsertCacheToDB(nUserCode, nCash, 0, user.m_nUserCash + nCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn, strNote);
            CDataBase.SaveUserInfoToDB(user);

            //매장에서 알금액을 증가시켜야 한다.
            store.m_nStoreCash += nCash;
            CDataBase.InsertStoreCash(nStoreCode, -nCash, store.m_nStoreCash - nCash, store.m_nStoreCash, 3, strNote);
            CDataBase.SaveStoreInfoToDB(store);

            CGlobal.SendUserInfoToClient(user);
            CGlobal.SendOnerMessage(nUserCode, strNote, 2);

            return strNote;
        }


        public static string StoreAcceptCharge(int nOnerCode, string strValue)
        {
            int nChgCode = Convert.ToInt32(strValue);
            string sql = $"SELECT * FROM tbl_storecharge WHERE chgCode = {nChgCode}";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            if(list == null || list.Count == 0)
            {
                return "해당자료가 디비에 존재하지 않습니다.";
            }
            DataRow info = list[0];
            if(Convert.ToInt32(info["chgCheck"]) != 0 && Convert.ToInt32(info["chgCheck"]) != 3)
            {
                return "처리된 신청입니다.";
            }
            int nUserCode = Convert.ToInt32(info["userCode"]);
            int nStoreCode = Convert.ToInt32(info["storeCode"]);
            int nChargeCash = Convert.ToInt32(info["chgCash"]);

            CUser user = CGlobal.GetUserByCode(nUserCode);
            if(user.m_nStoreCode != nStoreCode)
            {
                return "유저정보가 변경되었습니다.";
            }
            //소속된 매장의 알을 감소시켜야한다.
            CStore clsStore = CGlobal.GetStoreByCode(user.m_nStoreCode);
            if (clsStore.m_nStoreCash < nChargeCash)
                return "매장에 알이 모자랍니다. 알을 충전해주세요.";

            int nPreMoney = clsStore.m_nStoreCash;
            clsStore.m_nStoreCash -= nChargeCash;
            CDataBase.SaveStoreInfoToDB(clsStore);
            CDataBase.InsertStoreCash(clsStore.m_nStoreCode, -nChargeCash, nPreMoney, clsStore.m_nStoreCash, 2, user.m_strUserNick + "님 " + nChargeCash.ToString("N0") + "원 충전이 승인되었습니다.");

            user.m_nUserCash += nChargeCash;
            user.m_nUserCharge += nChargeCash;
            user.m_nChargeCash += nChargeCash;
            user.m_nChargeCnt++;

            string strNote = $"{user.m_strUserName}님 {nChargeCash}원 충전신청이 승인되었습니다.";
            CDataBase.InsertCacheToDB(user.m_nUserCode, nChargeCash, 0, user.m_nUserCash - nChargeCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn, strNote);

            if (user.m_nVirtualCupn > 0 || user.m_nVirtualCash > 0 || (user.m_nChargeCnt == 0 && user.m_lstGear.Count > 0))
            {
                CGlobal.ClearUserVirtualInfo(user);
            }

            CDataBase.SaveUserInfoToDB(user);
            CGlobal.SendUserInfoToClient(user);

            sql = $"UPDATE tbl_storecharge SET chgCheck = 1, accTime = '{CMyTime.GetMyTimeStr()}' WHERE chgCode = {nChgCode}";
            CMysql.ExcuteQuery(sql);

            string strRet = "충전 " + nChargeCash.ToString("N0") + "원\n";
            strRet += "지급되었습니다.";

            CGlobal.SendOnerMessage(nUserCode, strRet, 2);
            return strRet;
        }

        public static string OnerCancelCharge(int nOnderCode, string strValue)
        {
            string[] values = strValue.Split(',');
            int nUserCode = Convert.ToInt32(values[0]);
            int nChargeCash = Convert.ToInt32(values[1]);

            string strRet = "충전 " + nChargeCash.ToString("N0") + "원 신청이 취소되였습니다.";
            if (nUserCode > 0)
            {
                CGlobal.SendOnerMessage(nUserCode, strRet);
            }
            
            return strRet;
        }

        public static string OnerSetUserCash(int nOnderCode, string strValue)
        {
            try
            {
                string[] values = strValue.Split(',');
                int nUserCode = Convert.ToInt32(values[0]);
                int nUserCash = Convert.ToInt32(values[1]);
                CUser user = CGlobal.GetUserByCode(nUserCode);

                string strOnerNick = CGlobal.GetOnerNickByCode(nOnderCode);
                string strUserNick = CGlobal.GetUserNickByCode(nUserCode);
                string strNote = strOnerNick + "님이 " + strUserNick + "님의 보유금액을 " + nUserCash + "원으로 하였습니다.";
                CDataBase.InsertCacheToDB(nUserCode, nUserCash, 0, user.m_nUserCash, user.m_nUserCupn, nUserCash, user.m_nUserCupn, strNote);

                //int nRealCash = user.m_nChargeCash + user.m_nUserWinCash - user.m_nUserUseCash; //실지 남아있어야 하는 유저금액
                int nBonus = nUserCash - user.m_nUserCash;
                user.m_nUserCash = nUserCash;
                user.m_nBonusCash = Math.Max(nBonus, 0);
                user.m_nUserBonusCash = Math.Max(nBonus, 0);

                if (user.m_nChargeCnt == 0)
                {
                    CGlobal.ClearUserVirtualInfo(user);
                    user.m_nChargeCnt = 1;
                }

                CDataBase.SaveUserInfoToDB(user);
                CGlobal.SendUserInfoToClient(user);
                return strNote;
            }
            catch (Exception ex)
            {
                CGlobal.Log(ex.Message);
                return "failed";
            }

        }

        public static string OnerSetUserCupn(int nOnderCode, string strValue)
        {
            try
            {
                string[] values = strValue.Split(',');
                int nUserCode = Convert.ToInt32(values[0]);
                int nUserCupn = Convert.ToInt32(values[1]);
                CUser user = CGlobal.GetUserByCode(nUserCode);

                string strOnerNick = CGlobal.GetOnerNickByCode(nOnderCode);
                string strUserNick = CGlobal.GetUserNickByCode(nUserCode);
                string strNote = strOnerNick + "님이 " + strUserNick + "님의 쿠폰을 " + nUserCupn + "개로 하였습니다.";
                CDataBase.InsertCacheToDB(nUserCode, 0, nUserCupn, user.m_nUserCash, user.m_nUserCupn, user.m_nUserCash, nUserCupn, strNote);

                user.m_nUserCupn = nUserCupn;
                user.m_nUserBonusCash = Math.Max(0, user.m_nUserBonusCash + (nUserCupn - user.m_nUserCupn) * 4500);
                user.m_nBonusCash = Math.Max(0, user.m_nBonusCash + (nUserCupn - user.m_nUserCupn) * 4500);

                if (user.m_nChargeCnt == 0)
                {
                    CGlobal.ClearUserVirtualInfo(user);
                    user.m_nChargeCnt = 1;
                }
                CDataBase.SaveUserInfoToDB(user);
                CGlobal.SendUserInfoToClient(user);
                return strNote;
            }
            catch (Exception ex)
            {
                CGlobal.Log(ex.Message);
                return "failed";
            }

        }

        public static string OnerAcceptExCharge(int nOnderCode, string strValue)
        {
            int nExCode = Convert.ToInt32(strValue);
            string sql = $"SELECT * FROM tbl_excharge WHERE exCode = {nExCode}";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            if(list == null || list.Count == 0)
            {
                return "해당자료가 디비에 존재하지 않습니다.";
            }
            DataRow info = list[0];
            if(Convert.ToInt32(info["exCheck"]) == 1)
            {
                return "이미 승인된 자료입니다.";
            }
            int nUserCode = Convert.ToInt32(info["userCode"]);
            int nStoreCode = Convert.ToInt32(info["storeCode"]);
            int nExChargeCash = Convert.ToInt32(info["exCash"]);

            string strRet = "환전 " + nExChargeCash.ToString("N0") + "원 신청이 승인되었습니다.";
            if (nUserCode > 0)
            {
                CUser user = CGlobal.GetUserByCode(nUserCode);
                user.m_nUserExcharge += nExChargeCash;
                user.m_nChargeCash = 0;
                user.m_nBonusCash = 0;
                user.m_nAppendCash = 0;
                user.m_nExchargeCnt++;
                CDataBase.SaveUserInfoToDB(user);

                //소속된 총판의 정산을 진행하여야 한다.
                CAgent clsAgent = CGlobal.GetAgentByCode(user.m_nAgenCode);
                int nRealCash = Convert.ToInt32(nExChargeCash * clsAgent.m_fAgentPro / 100);
                clsAgent.m_nAgenCash -= nRealCash;
                CDataBase.SaveAgentTempCash(clsAgent);

                string strOnerNick = CGlobal.GetOnerNickByCode(nOnderCode);
                string strNote = strOnerNick + "님이 " + user.m_strUserNick + "님이 신청한 환전 " + nExChargeCash + "원을 승인하였습니다.";
                CDataBase.InsertCacheToDB(nUserCode, -nExChargeCash, 0, user.m_nUserCash + nExChargeCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn, strNote);
                CDataBase.UpdateDateCache(nExChargeCash, 1);

                CGlobal.SendOnerMessage(nUserCode, strRet);
            }
            else if(nStoreCode > 0)
            {
                CStore clsStore = CGlobal.GetStoreByCode(nStoreCode);
                clsStore.m_nStoreCash -= nExChargeCash;
                clsStore.m_nStoreExcharge += nExChargeCash;
                clsStore.m_nStoreExchargeCnt++;
                CDataBase.SaveStoreInfoToDB(clsStore);

                int nPreMoney = clsStore.m_nStoreCash + nExChargeCash;
                if (clsStore.m_nStoreCash < 0)
                    clsStore.m_nStoreCash = 0;
                CDataBase.InsertStoreCash(clsStore.m_nStoreCode, -nExChargeCash, nPreMoney, clsStore.m_nStoreCash, 1, nExChargeCash.ToString("N0") + "원 알환전 승인되었습니다.");

                string strLog = string.Empty;

                //소속된 총판들의 정산을 진행하여야 한다.
                CSubAdmin2 subAdmin2 = clsStore.m_clsSubAdmin2; ;
                nPreMoney = subAdmin2.m_nRealCash;
                int nSelfPro = subAdmin2.m_nSelfPro;
                int nRealCash = Convert.ToInt32(nExChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin2.m_nRealCash -= nRealCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdmin2Cache(subAdmin2.m_nCode, -nRealCash, nPreMoney, subAdmin2.m_nRealCash, 0, 1, strLog);
                }

                int nPreEvent = subAdmin2.m_nEventCash;
                int nEventPro = subAdmin2.m_nOtherPro;
                int nEventCash = Convert.ToInt32(nExChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin2.m_nEventCash -= nEventCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nEventPro}%";
                    CDataBase.InsertSubAdmin2Cache(subAdmin2.m_nCode, -nEventCash, nPreMoney, subAdmin2.m_nEventCash, 1, 1, strLog);
                }
                CDataBase.SaveSubAdmin2(subAdmin2);

                CSubAdmin1 subAdmin1 = clsStore.m_clsSubAdmin1;
                nPreMoney = subAdmin1.m_nRealCash;
                nSelfPro = subAdmin1.m_nSelfPro - subAdmin2.m_nTotalPro;
                nRealCash = Convert.ToInt32(nExChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin1.m_nRealCash -= nRealCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdmin1Cache(subAdmin1.m_nCode, -nRealCash, nPreMoney, subAdmin1.m_nRealCash, 0, 1, strLog);
                }

                nPreEvent = subAdmin1.m_nEventCash;
                nEventPro = subAdmin1.m_nOtherPro;
                nEventCash = Convert.ToInt32(nExChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin1.m_nEventCash -= nEventCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nEventPro}%";
                    CDataBase.InsertSubAdmin1Cache(subAdmin1.m_nCode, -nEventCash, nPreMoney, subAdmin1.m_nEventCash, 1, 1, strLog);
                }
                CDataBase.SaveSubAdmin1(subAdmin1);

                CSubAdmin0 subAdmin0 = clsStore.m_clsSubAdmin0;
                nPreMoney = subAdmin0.m_nRealCash;
                nSelfPro = subAdmin0.m_nSelfPro - subAdmin1.m_nTotalPro;
                nRealCash = Convert.ToInt32(nExChargeCash * nSelfPro / 100);
                if(nRealCash > 0)
                {
                    subAdmin0.m_nRealCash -= nRealCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdmin0Cache(subAdmin0.m_nCode, -nRealCash, nPreMoney, subAdmin0.m_nRealCash, 0, 1, strLog);
                }
                
                nPreEvent = subAdmin0.m_nEventCash;
                nEventPro = subAdmin0.m_nOtherPro;
                nEventCash = Convert.ToInt32(nExChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin0.m_nEventCash -= nEventCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nEventPro}%";
                    CDataBase.InsertSubAdmin0Cache(subAdmin0.m_nCode, -nEventCash, nPreMoney, subAdmin0.m_nEventCash, 1, 1, strLog);
                }
                CDataBase.SaveSubAdmin0(subAdmin0);

                CSubAdmin subAdmin = clsStore.m_clsSubAdmin;
                nPreMoney = subAdmin.m_nRealCash;
                nSelfPro = subAdmin.m_nSelfPro - subAdmin0.m_nTotalPro;
                if(nRealCash > 0)
                {
                    nRealCash = Convert.ToInt32(nExChargeCash * nSelfPro / 100);
                    subAdmin.m_nRealCash -= nRealCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nSelfPro}%";
                    CDataBase.InsertSubAdminCache(subAdmin.m_nCode, -nRealCash, nPreMoney, subAdmin.m_nRealCash, 0, 1, strLog);
                }

                nPreEvent = subAdmin.m_nEventCash;
                nEventPro = subAdmin.m_nOtherPro;
                nEventCash = Convert.ToInt32(nExChargeCash * nEventPro / 100);
                if(nEventCash > 0)
                {
                    subAdmin.m_nEventCash -= nEventCash;
                    strLog = $"{clsStore.m_strStoreNick}매장 알환전 {nExChargeCash}원 요율 {nEventPro}%";
                    CDataBase.InsertSubAdminCache(subAdmin.m_nCode, -nEventCash, nPreMoney, subAdmin.m_nEventCash, 1, 1, strLog);
                }
                CDataBase.SaveSubAdmin(subAdmin);

                CGlobal.GetSubAdmin().m_nTotalExcharge += nExChargeCash;
            }

            sql = $"UPDATE tbl_excharge SET exCheck = 1, accTime = '{CMyTime.GetMyTimeStr()}', onerCode = {nOnderCode} WHERE exCode = {nExCode}";
            CMysql.ExcuteQuery(sql);
            CDataBase.SaveSubAdmin(CGlobal.GetSubAdmin());

            return strRet;
        }

        public static string StoreAcceptExcharge(int nOnderCode, string strValue)
        {
            int nExCode = Convert.ToInt32(strValue);
            string sql = $"SELECT * FROM tbl_storeexcharge WHERE exCode = {nExCode}";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            if(list == null || list.Count == 0)
            {
                return "해당자료가 디비에 존재하지 않습니다.";
            }
            DataRow info = list[0];
            if(Convert.ToInt32(info["exCheck"]) != 0 && Convert.ToInt32(info["exCheck"]) != 3)
            {
                return "이미 처리된 자료입니다.";
            }
            int nUserCode = Convert.ToInt32(info["userCode"]);
            int nStoreCode = Convert.ToInt32(info["storeCode"]);
            int nExChargeCash = Convert.ToInt32(info["exCash"]);

            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (nStoreCode != user.m_nStoreCode)
            {
                return "유저정보가 변경되었습니다.";
            }
            user.m_nUserExcharge += nExChargeCash;
            user.m_nChargeCash = 0;
            user.m_nBonusCash = 0;
            user.m_nAppendCash = 0;
            user.m_nExchargeCnt++;
            CDataBase.SaveUserInfoToDB(user);
            string strNote = user.m_strUserNick + "님이 신청한 환전 " + nExChargeCash + "원이 승인되었습니다.";
            CDataBase.InsertCacheToDB(nUserCode, -nExChargeCash, 0, user.m_nUserCash + nExChargeCash, user.m_nUserCupn, user.m_nUserCash, user.m_nUserCupn, strNote);

            string strRet = "환전 " + nExChargeCash.ToString("N0") + "원 신청이 승인되었습니다.";

            //매장의 알을 올려주어여야 한다.
            CStore clsStore = CGlobal.GetStoreByCode(user.m_nStoreCode);
            int nPreMoney = clsStore.m_nStoreCash;
            clsStore.m_nStoreCash += nExChargeCash;
            CDataBase.SaveStoreInfoToDB(clsStore);
            CDataBase.InsertStoreCash(clsStore.m_nStoreCode, nExChargeCash, nPreMoney, clsStore.m_nStoreCash, 3, user.m_strUserNick + "님 " + nExChargeCash.ToString("N0") + "원 환전 승인되었습니다.");
            CGlobal.SendOnerMessage(nUserCode, strRet);

            sql = $"UPDATE tbl_storeexcharge SET exCheck = 1, accTime = '{CMyTime.GetMyTimeStr()}' WHERE exCode = {nExCode}";
            CMysql.ExcuteQuery(sql);

            return strRet;
        }

        public static string StoreRequestAlExcharge(int nOnderCode, string strValue)
        {
            string[] values = strValue.Split(',');
            int nStoreCode = Convert.ToInt32(values[0]);
            int nExChargeCash = Convert.ToInt32(values[1]);

            CStore clsStore = CGlobal.GetStoreByCode(nStoreCode);
            clsStore.m_nStoreCash -= nExChargeCash;

            
            return "Success";
        }

        public static string StoreUserPopDel(int nOnderCode, string strValue)
        {
            string[] param = strValue.Split(',');
            int nStoreCode = Convert.ToInt32(param[0]);
            int nUserCode = Convert.ToInt32(param[1]);
            int nDel = Convert.ToInt32(param[2]);

            CUser user = CGlobal.GetUserByCode(nUserCode);
            if(user.m_nStoreCode != nStoreCode)
            {
                return "매장소속계정이 아닙니다.";
            }

            if(user.m_nUserLogin == 1)
            {
                return "계정이 온라인상태입니다.";
            }

            //유저가 보유하고 있는 머니와 기대들에 남아있는 머니들의 합을 구한다.
            int nUserCash = user.m_nUserCash + user.m_nUserCupn * 4500;
            nUserCash += user.m_lstGear.Sum(value => value.m_nSlotCash);
            nUserCash += user.m_lstGear.Sum(value => value.m_nGiftCash);
            //매장보유알을 증가시킨다.
            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            store.m_nStoreCash += nUserCash;
            CDataBase.SaveStoreInfoToDB(store);
            //매장유저환전내역에 보관한다.
            string strTime = CMyTime.GetMyTimeStr();
            string sql = "INSERT INTO tbl_storeexcharge(userCode, exTime, accTime, exCheck, exCash, storeCode) VALUES(" + nUserCode + ", '" + strTime + "', '" + strTime + "', " + 1 + ", " + nUserCash + ", " + nStoreCode + ")";
            CDataBase.PushOtherQuery(sql);
            //유저보유머니를 0으로 초기화한다
            user.m_nUserCash = 0;
            user.m_nUserCupn = 0;
            CDataBase.SaveUserInfoToDB(user);
            //보유하고 있던 기계들을 모두 초기화한다.
            List<CBaseGear> lstGear = CGlobal.GetGearList().FindAll(value=>value.m_nTakeUser == nUserCode);
            foreach(CBaseGear gear in lstGear)
            {
                gear.ClearGear();
                gear.LogoutGear();
            }
            user.m_lstGear.Clear();

            if(nDel == 1)
            {
                sql = "DELETE FROM tbl_user WHERE userCode = " + nUserCode;
                CDataBase.PushOtherQuery(sql);
                CGlobal.GetUserList().Remove(user);
                store.m_nUserCnt--;
                CDataBase.SaveStoreInfoToDB(store);

                return "유저가 삭제되었습니다.";
            }
            else
            {
                return "유저를 게임에서 빼내었습니다.";
            }
        }


        public static string OnerCancelExCharge(int nOnderCode, string strValue)
        {
            int nExCode = Convert.ToInt32(strValue);
            string sql = $"SELECT * FROM tbl_excharge WHERE exCode = {nExCode}";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            if(list == null || list.Count == 0)
            {
                return "환전자료가 디비에 존재하지 않습니다.";
            }
            DataRow info = list[0];
            int nUserCode = Convert.ToInt32(info["userCode"]);
            int nStoreCode = Convert.ToInt32(info["storeCode"]);
            int nExChargeCash = Convert.ToInt32(info["exCash"]);

            string strRet = "환전 " + nExChargeCash.ToString("N0") + "원 신청이 취소되었습니다.";
            if (nUserCode > 0)
            {
                CUser user = CGlobal.GetUserByCode(nUserCode);
                user.m_nUserCash += nExChargeCash;
                CGlobal.SendUserInfoToClient(user);
                CDataBase.SaveUserInfoToDB(user);
                CGlobal.SendOnerMessage(nUserCode, strRet);
            }
            else if(nStoreCode > 0)
            {
                CStore store = CGlobal.GetStoreByCode(nStoreCode);
                store.m_nStoreCash += nExChargeCash;
                CDataBase.SaveStoreInfoToDB(store);
            }
            sql = $"UPDATE tbl_excharge SET exCheck = 2, accTime = '{CMyTime.GetMyTimeStr()}', onerCode = {nOnderCode} WHERE exCode = {nExCode}";
            CMysql.ExcuteQuery(sql);

            return strRet;
        }

        public static string OnerAnswerSend(int nOnderCode, string strValue)
        {
            string[] values = strValue.Split(',');
            int nUserCode = Convert.ToInt32(values[0]);
            string strAnswer = values[1];
            CUser user = CGlobal.GetUserByCode(nUserCode);

            string strRet = string.Empty;
            if (user.m_nUserLogin == 1)
            {
                CGlobal.SendOnerMessage(nUserCode, strAnswer);
                strRet = "success";
            }
            else
            {
                strRet = "failed";
            }


            return strRet;
        }

        public static string OnerAddRobot(int nOnderCode, string strValue)
        {
            int nRbCode = Convert.ToInt32(strValue);

            DataRow row = CMysql.GetDataQuery("SELECT * FROM tbl_robot WHERE rbCode = " + nRbCode)[0];
            string strRbNick = Convert.ToString(row["rbNick"]);
            int nOnerCode = Convert.ToInt32(row["onerCode"]);
            int nRbLogin = Convert.ToInt32(row["rbLogin"]);
            int nGearCode = Convert.ToInt32(row["gearCode"]);
            int nGameCode = Convert.ToInt32(row["gameCode"]);

            CRobot robot = new CRobot();
            robot.m_nRbCode = nRbCode;
            robot.m_strRbNick = strRbNick;
            robot.m_nOnerCode = nOnderCode;
            robot.m_nRbLogin = nRbLogin;
            robot.m_nGearCode = nGearCode;
            robot.m_nGameCode = nGameCode;

            CGlobal.AddRobot(robot);

            if (nRbLogin == 1)
            {
                string strChat = "[C][09f909]운영자: " + robot.m_strRbNick + " 님 입장을 환영합니다.[-]";
                CGlobal.SendNoticeBroadCast(strChat);
            }

            if (nGearCode > 0)
            {
                CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
                gear.m_nGearState = 1;
                gear.m_nGearRun = nRbLogin;
                gear.m_nTakeRobot = nRbCode;
            }

            return "로봇을 추가하였습니다.";
        }

        public static string OnerUpdateRobot(int nOnderCode, string strValue)
        {
            int nRbCode = Convert.ToInt32(strValue);

            DataRow row = CMysql.GetDataQuery("SELECT * FROM tbl_robot WHERE rbCode = " + nRbCode)[0];
            string strRbNick = Convert.ToString(row["rbNick"]);
            int nOnerCode = Convert.ToInt32(row["onerCode"]);
            int nRbLogin = Convert.ToInt32(row["rbLogin"]);
            int nGearCode = Convert.ToInt32(row["gearCode"]);
            int nGameCode = Convert.ToInt32(row["gameCode"]);
            int nAutoJack = Convert.ToInt32(row["autoJack"]);

            CRobot robot = CGlobal.GetRobotByCode(nRbCode);
            if (robot == null)
            {
                return "서버에 해당로봇이 없습니다.";
            }
            robot.m_strRbNick = strRbNick;
            robot.m_nOnerCode = nOnderCode;
            robot.m_nGearCode = nGearCode;
            robot.m_nGameCode = nGameCode;
            robot.m_nAutoJack = nAutoJack;

            if(robot.m_nAutoJack == 1)
            {
                robot.StartChangeGear();
            }

            if (robot.m_nRbLogin == 0)
            {
                if (nRbLogin == 1)
                {
                    string strChat = "[C][09f909]운영자: " + robot.m_strRbNick + " 님 입장을 환영합니다.[-]";
                    CGlobal.SendNoticeBroadCast(strChat);
                }
            }
            robot.m_nRbLogin = nRbLogin;

            if (nGearCode > 0)
            {
                CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
                gear.m_nGearState = 1;

                gear.m_nGearRun = nRbLogin;
                gear.m_nTakeRobot = nRbCode;
            }

            CGlobal.SendRobotInfoToAdmin(robot);

            return "로봇을 수정하였습니다.";
        }

        public static string OnerDeleteRobot(int nOnderCode, string strValue)
        {
            int nRbCode = Convert.ToInt32(strValue);
            CRobot robot = CGlobal.GetRobotByCode(nRbCode);
            if (robot == null)
                return "삭제된 로봇입니다.";

            robot.m_nRbLogin = 0;
            CBaseGear clsGear = CGlobal.GetGearByCode(robot.m_nGearCode);
            if(clsGear != null)
            {
                clsGear.ClearGear();
                clsGear.m_nGearState = 0;
                clsGear.m_nTakeRobot = 0;
            }

            CGlobal.SendRobotInfoToAdmin(robot);
            CGlobal.RemoveRobotByCode(nRbCode);

            string sql = "DELETE FROM tbl_robot WHERE rbCode = " + nRbCode;
            CMysql.ExcuteQuery(sql);

            return "로봇을 삭제하였습니다.";
        }

        public static string OnerReleaseRobot(int nOnderCode, string strValue)
        {
            int nRbCode = Convert.ToInt32(strValue);
            CRobot robot = CGlobal.GetRobotByCode(nRbCode);
            if (robot == null)
                return "삭제된 로봇입니다.";

            CBaseGear clsGear = CGlobal.GetGearByCode(robot.m_nGearCode);
            if (clsGear != null)
            {
                clsGear.ClearGear();
                clsGear.m_nGearState = 0;
                clsGear.m_nTakeRobot = 0;
                CDataBase.SaveGearInfoToDB(clsGear);
            }
            robot.m_nGameCode = 0;
            robot.m_nGearCode = 0;
            robot.m_nJackpot = 0;

            CGlobal.SendRobotInfoToAdmin(robot);
            CDataBase.SaveRobotInfoToDB(robot);

            return "로봇기계를 해제하였습니다.";
        }

        public static string OnerAddBlockIP(int nOnderCode, string strValue)
        {
            CGlobal.AddBlockIP(strValue);
            string sql = "INSERT INTO tbl_blockip(strIP, strTime, nOnerCode) VALUES('" + strValue + "', '" + CMyTime.GetMyTimeStr() + "', " + nOnderCode + ")";
            CMysql.ExcuteQuery(sql);


            return "블록아이피를 추가하였습니다.";
        }

        public static string OnerDeleteBlockIP(int nOnderCode, string strValue)
        {
            CGlobal.RemoveBlockIP(strValue);
            string sql = "DELETE FROM tbl_blockip WHERE strIP = '" + strValue + "'";
            CMysql.ExcuteQuery(sql);


            return "블록아이피가 삭제되었습니다.";

        }

        public static string OnSetAcid(int nOnderCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            string userName = Convert.ToString(json["userName"]).Trim();
            string bankName = Convert.ToString(json["bankName"]).Trim();
            string sendAcid = Convert.ToString(json["sendAcid"]).Trim();
            int nStoreCode = Convert.ToInt32(json["storeCode"]);

            CGlobal.SetAcid(userName, bankName, sendAcid, nStoreCode);

            string sql = "DELETE FROM tbl_acid WHERE storeCode = " + nStoreCode;
            CMysql.ExcuteQuery(sql);

            sql = "INSERT tbl_acid(userName, bankName, acid, storeCode) VALUES('" + userName + "', '" + bankName + "', '" + sendAcid + "'," + nStoreCode + ")";
            CMysql.ExcuteQuery(sql);

            return "계좌가 등록되었습니다.";
        }

        public static string OnerSetJackPot(int nOnderCode, string strValue)
        {
            string[] packet = strValue.Split(',');
            int nGameCode = CDefine.GAME_DRAGON; //Convert.ToInt32(packet[0]);
            int nIdx = Convert.ToInt32(packet[1]);
            int nJackPot = Convert.ToInt32(packet[2]);

            CGameEngine clsEngine = CGlobal.GetGameEngineByCode(nGameCode);
            CGlobal.SetJackPot(nIdx, nJackPot);

            return "잭팟포트금액이 설정되었습니다.";
        }

        public static string OnerSetTotalPot(int nOnderCode, string strValue)
        {
            string[] packet = strValue.Split(',');
            int nGameCode = Convert.ToInt32(packet[0]);
            int nIdx = Convert.ToInt32(packet[1]);
            int nTotalPot = Convert.ToInt32(packet[2]);

            CTotalPot totalpot = CGlobal.GetTotalPot(nGameCode);
            if (nIdx == 0)
                totalpot.m_nFirst = nTotalPot;
            else if (nIdx == 1)
                totalpot.m_nSecond = nTotalPot;
            else if (nIdx == 2)
                totalpot.m_nThird = nTotalPot;

            return "자연환수금액이 설정되었습니다.";
        }


        public static string ResetTotalPot(int nOnerCode, string strValue)
        {
            int nGameCode = Int32.Parse(strValue);
            CGlobal.GetGameEngineByCode(nGameCode).ResetTotalJackCash();

            return "게임누적금액이 초기화되었습니다.";
        }

        public static string OnerGlobalChat(int nOnderCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnderCode);
            if (oner == null)
                return "failed";

            CGlobal.SendGroupChat(nOnderCode, 0, strValue);
            return strValue;
        }

        public static string OnSetShowNoticeImage(int nOnderCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnderCode);
            if (oner == null)
                return "failed";
            int nValue = Convert.ToInt32(strValue);
            CGlobal.SetShowNoticeImage(nValue);

            return "설정되었습니다.";
        }
        public static string OnShowNoticeImage(int nOnderCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnderCode);
            if (oner == null)
                return "failed";

            CGlobal.BroadCastShowNotice();

            return "조작이 성공되었습니다.";
        }

        public static string OnerGlobalNotice(int nOnderCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnderCode);
            if (oner == null)
                return "failed";

            CGlobal.SendNoticeBroadCast(strValue);

            return "공지사항을 보내였습니다.";
        }

        public static string OnerCreateAgent(int nOnderCode, string strValue)
        {
            if (strValue == "add")
            {
                DataRowCollection agents = CMysql.GetDataQuery("SELECT * FROM tbl_agent ORDER BY agenCode DESC LIMIT 1");
                if (agents.Count == 0)
                {
                    return "failed";
                }
                CAgent agent = new CAgent();
                int i = 0;
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
                agent.m_nIsStore = Convert.ToInt32(agents[i]["isStore"]);
                agent.m_strDomain = Convert.ToString(agents[i]["agenDomain"]);

                CGlobal.GetAgentList().Add(agent);

                return "success";

            }
            else
            {
                int nAgenCode = Convert.ToInt32(strValue);
                CAgent agent = CGlobal.GetAgentByCode(nAgenCode);

                DataRowCollection agents = CMysql.GetDataQuery("SELECT * FROM tbl_agent WHERE agenCode = " + nAgenCode);
                int i = 0;
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

                return "success";
            }

        }

        public static string OnerCreateStore(int nOnderCode, string strValue)
        {
            if (strValue == "add")
            {
                DataRowCollection stores = CMysql.GetDataQuery("SELECT * FROM tbl_store ORDER BY storeCode DESC LIMIT 1");
                if (stores.Count == 0)
                {
                    return "failed";
                }
                int i = 0;
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

                CGlobal.GetStoreList().Add(store);

                return "success";

            }
            else
            {
                int nStoreCode = Convert.ToInt32(strValue);
                CStore store = CGlobal.GetStoreByCode(nStoreCode);

                DataRowCollection stores = CMysql.GetDataQuery("SELECT * FROM tbl_store WHERE storeCode = " + nStoreCode);
                int i = 0;
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

                return "success";
            }

        }

        public static string OnerDeleteStore(int nOnerCode, string strValue)
        {
            int nStoreCode = Convert.ToInt32(strValue);
            CStore store = CGlobal.GetStoreByCode(nStoreCode);

            CGlobal.GetStoreList().Remove(store);
            return "Success";
        }

        public static string OnerDeleteAgent(int nOnderCode, string strValue)
        {
            int nAgenCode = Convert.ToInt32(strValue);
            CAgent agent = CGlobal.GetAgentByCode(nAgenCode);
            CGlobal.GetAgentList().Remove(agent);
            return "Success";
        }

        public static string OnerSetAbsent(int nOnerCode, string strValue)
        {
            string[] packet = strValue.Split(',');

            CAbsent absent = new CAbsent();
            absent.m_nCheckCount = Convert.ToInt32(packet[0]);
            absent.m_nPreUseCash = Convert.ToInt32(packet[1]);
            absent.m_nGiveCupn = Convert.ToInt32(packet[2]);

            CGlobal.SetAbsent(absent);
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);

            return oner.m_strOnerNick + "님이 출석조건을 설정하였습니다.";
        }

        public static string OnerSetGameRate(int nOnerCode, string strValue)
        {
            string[] packet = strValue.Split(',');

            int nGameCode = Convert.ToInt32(packet[8]);
            CGameEngine game = CGlobal.GetGameEngineByCode(nGameCode);
            game.m_nGameCash = Convert.ToInt32(packet[0]);
            game.m_nGameRate = Convert.ToInt32(packet[1]);
            game.m_nGameCash1 = Convert.ToInt32(packet[2]);
            game.m_nGameRate1 = Convert.ToInt32(packet[3]);
            game.m_nGameCash2 = Convert.ToInt32(packet[4]);
            game.m_nGameRate2 = Convert.ToInt32(packet[5]);
            game.m_nGameCash3 = Convert.ToInt32(packet[6]);
            game.m_nGameRate3 = Convert.ToInt32(packet[7]);

            CGlobal.ResetTotalCash(nGameCode);
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);

            return oner.m_strOnerNick + "님이 환수률을 설정하였습니다.";
        }


        public static string AcceptAgentExReal(int nOnderCode, string strValue)
        {
            string[] param = strValue.Split(',');
            int nAgenCode = Convert.ToInt32(param[0]);
            int nAexCash = Convert.ToInt32(param[1]);

            List<CUser> lstUser = CGlobal.GetUserList().FindAll(value => value.m_nAgenCode == nAgenCode);
            for (int i = 0; i < lstUser.Count; i++)
            {
                lstUser[i].m_nUserUseCash = 0;
                lstUser[i].m_nUserWinCash = 0;
            }

            string sql = "UPDATE tbl_datecache SET nAgentCash = nAgentCash + " + nAexCash + " WHERE strDate = '" + CMyTime.GetMyTimeStr("yyyy-MM-dd") + "'";
            CMysql.ExcuteQuery(sql);

            return "Success";
        }

        public static string ClearAgenRealCash(int nOnderCode, string strValue)
        {
            int nAgenCode = Convert.ToInt32(strValue);
            CAgent agent = CGlobal.GetAgentByCode(nAgenCode);
            if (agent == null)
                return "Failed";

            agent.m_nAgenCash = 0;
            CDataBase.SaveAgentRealCash(agent);


            return "Success";
        }

        public static string ClearStoreRealCash(int nOnderCode, string strValue)
        {
            int nStoreCode = Convert.ToInt32(strValue);
            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            store.m_nStoreReal = 0;
            CDataBase.SaveStoreInfoToDB(store);


            return "Success";
        }

        public static string AcceptStoreExReal(int nOnderCode, string strValue)
        {
            string[] param = strValue.Split(',');
            int nStoreCode = Convert.ToInt32(param[0]);
            int nSexCash = Convert.ToInt32(param[1]);

            List<CUser> lstUser = CGlobal.GetUserList().FindAll(value => value.m_nStoreCode == nStoreCode);
            for (int i = 0; i < lstUser.Count; i++)
            {
                lstUser[i].m_nUserUseCash = 0;
                lstUser[i].m_nUserWinCash = 0;
            }

            string sql = "UPDATE tbl_datecache SET nAgentCash = nAgentCash + " + nSexCash + " WHERE strDate = '" + CMyTime.GetMyTimeStr("yyyy-MM-dd") + "'";
            CMysql.ExcuteQuery(sql);

            return "Success";
        }

        public static string CancelAgentExReal(int nOnderCode, string strValue)
        {
            string[] param = strValue.Split(',');
            int nAgenCode = Convert.ToInt32(param[0]);
            int nAexCash = Convert.ToInt32(param[1]);
            CAgent agent = CGlobal.GetAgentByCode(nAgenCode);
            agent.m_nAgenCash += nAexCash;
            CDataBase.AddAgentRealCashToDB(nAgenCode, nAexCash);
            CDataBase.SaveAgentTempCash(agent);

            return "Success";
        }

        public static string CancelStoreExReal(int nOnderCode, string strValue)
        {
            string[] param = strValue.Split(',');
            int nStoreCode = Convert.ToInt32(param[0]);
            int nAexCash = Convert.ToInt32(param[1]);
            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            store.m_nStoreReal += nAexCash;
            CDataBase.SaveStoreInfoToDB(store);

            return "Success";
        }

        //총판페지에서 보내온 자료이므로 총판코드가 온다.
        public static string RequestAgentExReal(int nOnerCode, string strValue)
        {
            string[] param = strValue.Split(',');
            int nAgenCode = Convert.ToInt32(param[0]);
            int nStoreCode = Convert.ToInt32(param[1]);
            int nAexCash = Convert.ToInt32(param[2]);

            if(nAgenCode > 0)
            {
                CAgent agent = CGlobal.GetAgentByCode(nAgenCode);
                agent.m_nAgenCash -= nAexCash;
                CDataBase.AddAgentRealCashToDB(nAgenCode, -nAexCash);
                CDataBase.SaveAgentTempCash(agent);
            }
            else if(nStoreCode > 0)
            {
                CStore store = CGlobal.GetStoreByCode(nStoreCode);
                store.m_nStoreReal -= nAexCash;
                if (store.m_nStoreReal < 0)
                    store.m_nStoreReal = 0;
                CDataBase.SaveStoreInfoToDB(store);
            }

            return "Success";
        }

        public static string ClearStoreCash(int nOnderCode, string strValue)
        {
            int nStoreCode = Convert.ToInt32(strValue);
            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            if(store == null)
            {
                return "잘못된 파케트를 전송하였습니다.";
            }

            int nStoreCash = store.m_nStoreCash;
            store.m_nStoreCash = 0;
            CDataBase.SaveStoreInfoToDB(store);

            List<CUser> lstUser = store.m_lstUser;
            foreach(CUser user in lstUser)
            {
                user.m_nUserCupn = 0;
                user.m_nUserCash = 0;
                CDataBase.SaveUserInfoToDB(user);

                List<CBaseGear> lstGear = user.m_lstGear;
                foreach(CBaseGear gear in lstGear)
                {
                    gear.m_nGiftCash = 0;
                    gear.m_nSlotCash = 0;
                    gear.m_nGearJack = 0;

                    CDataBase.SaveGearInfoToDB(gear);
                }
            }
            CDataBase.InsertStoreCash(store.m_nStoreCode, -nStoreCash, nStoreCash, 0, 4, "관리자님이 매장알을 초기화하였습니다.");

            return store.m_strStoreNick + "매장금액이 초기화되었습니다.";
        }


        public static string OnerSetGearDetail(int nOnderCode, string strValue)
        {
            int nGearCode = Convert.ToInt32(strValue);

            string sql = "SELECT * FROM tbl_gear WHERE gearCode = " + nGearCode;
            DataRow gearInfo = CMysql.GetDataQuery(sql)[0];

            int nAccuCash = Convert.ToInt32(gearInfo["accuCash"]);
            int nJackCash = Convert.ToInt32(gearInfo["jackCash"]);
            int nTopJackCash = Convert.ToInt32(gearInfo["topJackCash"]);
            int nLastJackCash = Convert.ToInt32(gearInfo["lastJackCash"]);
            string strLastJackTime = Convert.ToString(gearInfo["lastJackTime"]);
            int nGrandCount = Convert.ToInt32(gearInfo["grandCount"]);
            int nMajorCount = Convert.ToInt32(gearInfo["majorCount"]);
            int nMinorCount = Convert.ToInt32(gearInfo["minorCount"]);
            int nMiniCount = Convert.ToInt32(gearInfo["miniCount"]);

            CBaseGear gear = CGlobal.GetGearByCode(nGearCode);
            gear.m_nAccuCash = nAccuCash;
            gear.m_nJackCash = nJackCash;
            gear.m_nTopJackCash = nTopJackCash;
            gear.m_nLastJackCash = nLastJackCash;
            try
            {
                gear.m_dtLastJackTime = Convert.ToDateTime(strLastJackTime);
            }
            catch
            {
                gear.m_dtLastJackTime = CMyTime.GetMyTime();
            }

            gear.m_nGrandCount = nGrandCount;
            gear.m_nMajorCount = nMajorCount;
            gear.m_nMinorCount = nMinorCount;
            gear.m_nMiniCount = nMiniCount;


            string strRet = CGlobal.GetGearNickByGearCode(nGearCode) + " 기계의 상세정보를 설정하였습니다.";
            return strRet;
        }

        public static string SetVirtualCash(int nOnderCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnderCode);

            string[] packet = strValue.Split(',');
            int nUserCode = Convert.ToInt32(packet[0].Trim());
            int nVirtualCash = Convert.ToInt32(packet[1].Trim());

            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user.m_nChargeCnt > 0 || user.m_nUserCash > 0 || user.m_nUserCupn > 0)
            {
                return "failed";
            }
            user.m_nVirtualCash = nVirtualCash;
            CDataBase.SaveUserInfoToDB(user);
            CGlobal.SendUserInfoToClient(user);

            return oner.m_strOnerNick + "님이 " + user.m_strUserNick + "님에게 체험머니 " + nVirtualCash.ToString("N0") + "원을 지급하였습니다.";
        }

        public static string SetVirtualCupn(int nOnderCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnderCode);

            string[] packet = strValue.Split(',');
            int nUserCode = Convert.ToInt32(packet[0].Trim());
            int nVirtualCupn = Convert.ToInt32(packet[1].Trim());

            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user.m_nChargeCnt > 0 || user.m_nUserCash > 0 || user.m_nUserCupn > 0)
            {
                return "failed";
            }
            user.m_nVirtualCupn = nVirtualCupn;
            CDataBase.SaveUserInfoToDB(user);
            CGlobal.SendUserInfoToClient(user);

            return oner.m_strOnerNick + "님이 " + user.m_strUserNick + "님에게 체험쿠폰 " + nVirtualCupn.ToString("N0") + "게를 지급하였습니다.";
        }

        public static string SetVirtual(int nOnerCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);

            string[] packet = strValue.Split(',');
            CBonusVirtual cls = CGlobal.GetBonusVirtual();
            cls.m_nVirtual = Convert.ToInt32(packet[0]);
            cls.m_nVirtualCash = Convert.ToInt32(packet[1]);
            cls.m_nVirtualCupn = Convert.ToInt32(packet[2]);

            return oner.m_strOnerNick + "님이 첫가입시 체험쿠폰적용을 설정하였습니다.";
        }

        public static string SetBonus(int nOnerCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);

            string[] packet = strValue.Split(',');
            CBonusVirtual cls = CGlobal.GetBonusVirtual();
            cls.m_nBonus = Convert.ToInt32(packet[0]);
            cls.m_nBonusPro = Convert.ToDouble(packet[1]);
            cls.m_nBonusCupn = Convert.ToInt32(packet[2]);

            return oner.m_strOnerNick + "님이 첫충시 보너스적용을 설정하였습니다.";
        }

        public static string SetAbsentCount(int nOnerCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);

            string[] packet = strValue.Split(',');
            int nUserCode = Convert.ToInt32(packet[0].Trim());
            int nAbsent = Convert.ToInt32(packet[1].Trim());

            CUser user = CGlobal.GetUserByCode(nUserCode);
            user.m_nAbsentCnt = nAbsent;

            return oner.m_strOnerNick + "님이 " + user.m_strUserNick + "의 출석일수가 " + nAbsent + "로 설정하였습니다.";
        }

        public static string UseTempExcharge(int nOnerCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);

            int nValue = Convert.ToInt32(strValue);
            CGlobal.SetUseTempExcharge(nValue);
            string str = nValue == 1 ? "활성화" : "비활성화";

            return oner.m_strOnerNick + "님이 가상출금현황자동입력이 " + str + "되었습니다.";
        }

        public static string CallUserLogout(int nOnerCode, string strValue)
        {
            CAdmin oner = CGlobal.GetAdminByCode(nOnerCode);
            int nUserCode = Convert.ToInt32(strValue);
            CUser user = CGlobal.GetUserByCode(nUserCode);
            if (user == null)
                return oner.m_strOnerNick + "님이 선택한 유저가 정확하지 않아 강제탈퇴기능이 실패되었습니다.";

            if(user.m_nUserLogin == 0)
                return oner.m_strOnerNick + "님이 선택한 유저가 오프라인상태입니다.";
            user.UserLogout();

            return oner.m_strOnerNick + "님이 " + user.m_strUserNick + "님을 강제탈퇴시키였습니다.";
        }

        public static string GetUserListFromAdminPage(int nOnerCode, string strValue)
        {
            List<CAdminSocket> lstAdminSocket = CGlobal.GetAdminPageSocketList();
            if (lstAdminSocket.Exists(value => value.m_clsAdmin.m_nOnerCode == nOnerCode))
            {
                lstAdminSocket.Find(value => value.m_clsAdmin.m_nOnerCode == nOnerCode).StartSendUserList(strValue);

                return "success";
            }
            else
            {
                return "조회실패";
            }
            
        }

        public static string GetUserListFromStorePage(int nOnerCode, string strValue)
        {
            string[] param = strValue.Split(":");
            int nStoreCode = Convert.ToInt32(param[5]);
            List<CAdminSocket> lstStoreSocket = CGlobal.GetStorePageSocketList();
            if (lstStoreSocket.Exists(value => value.m_clsStore.m_nStoreCode == nStoreCode))
            {
                lstStoreSocket.Find(value => value.m_clsStore.m_nStoreCode == nStoreCode).StartSendUserList(strValue);

                return "success";
            }
            else
            {
                return "조회실패";
            }

        }

        public static string UserPageLogin(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            string strUserID = Convert.ToString(json["strUserID"]);
            strUserID = Uri.UnescapeDataString(strUserID).Trim();
            string strUserPW = Convert.ToString(json["strUserPW"]);
            strUserPW = Uri.UnescapeDataString(strUserPW).Trim();

            CUser user = CGlobal.GetUserByID(strUserID);
            if(user == null)
            {
                return "아이디가 잘못되었습니다.";
            }
            if(user.m_strUserPW != strUserPW)
            {
                return "비번이 잘못되었습니다.";
            }

            return "Success";
        }

        public static string OneUserRegister(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            string userID = Convert.ToString(json["userID"]).Trim();
            userID = Uri.UnescapeDataString(userID);
            string userNick = Convert.ToString(json["userNick"]).Trim();
            userNick = Uri.UnescapeDataString(userNick);
            string userPW = Convert.ToString(json["userPW"]).Trim();
            userPW = Uri.UnescapeDataString(userPW);
            string userPhone = Convert.ToString(json["userPhone"]).Trim();
            string domain = Convert.ToString(json["domain"]).Trim();

            if (CGlobal.GetUserList().Exists(value => value.m_strUserID == userID))
                return "이미 존재하는 아이디입니다.";
            else if (CGlobal.GetUserList().Exists(value => value.m_strUserNick == userNick))
                return "이미 존재하는 닉네임입니다.";

            string sql = $"SELECT * FROM tbl_agent WHERE agenDomain LIKE '%{domain}%' AND agenState = 1";
            DataRowCollection list = CMysql.GetDataQuery(sql);
            if(list == null || list.Count == 0)
            {
                return "승인되지 않은 도메인입니다.";
            }
            int agenCode = Convert.ToInt32(list[0]["agenCode"]);
            CAgent clsAgent = CGlobal.GetAgentByCode(agenCode);
            if (clsAgent == null)
            {
                return "등록되지 않은 도메인입니다.";
            }
                
            sql = "INSERT INTO tbl_user(userID, userPW, userNick, userPhone, agenCode, userRegTime, userState) ";
            sql += "VALUES('" + userID + "', '" + userPW + "', ";
            sql += "'" + userNick + "', '" + userPhone + "', ";
            sql += agenCode + ", '" + CMyTime.GetMyTimeStr() + "', 1)";
            CUser clsUser = CDataBase.InsertUserToDB(sql, userID);
            if(clsUser == null)
            {
                return "가입실패 되었습니다.";
            }
            else
            {
                //유저가 속한 총판객체에 유저를 추가한다.
                clsAgent.m_lstUser.Add(clsUser);
                clsAgent.m_nUserCount++;
            }

            return "success";
        }

        public static string SetUserMemo(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nUserCode = Convert.ToInt32(json["nUserCode"]);
            string strMemo = Convert.ToString(json["strMemo"]).Trim();

            CUser clsUser = CGlobal.GetUserByCode(nUserCode);
            if (clsUser == null)
                return "해당유저가 존재하지 않습니다.";
            clsUser.m_strMemo = strMemo;
            CDataBase.SaveUserInfoToDB(clsUser);

            return "설정되었습니다.";
        }

        public static string OnSetAutoPrize(int nOnerCode, string strValue)
        {
            string[] param = strValue.Split(',');
            if (param.Length != 2)
                return "파라메터값이 틀립니다.";

            int nGameCode = Convert.ToInt32(param[0]);
            int nAutoPrize = Convert.ToInt32(param[1]);

            CGameEngine clsGame = CGlobal.GetEngine(nGameCode);
            clsGame.m_nAutoPrize = nAutoPrize;

            string sql = $"UPDATE tbl_game SET autoPrize = {nAutoPrize} WHERE gameCode = {nGameCode}";
            CDataBase.PushOtherQuery(sql);

            return "설정되었습니다.";
        }

        public static string SetSubAdminPro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nOtherPro = Convert.ToInt32(json["nOtherPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사, 2-부본사, 3-총판, 4-매장
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nOtherPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin subAdmin = CGlobal.GetSubAdmin();
            int nSelfPro = subAdmin.m_nTotalPro - nOtherPro;
            
            CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
            if (admin == null)
                return "오류! 관리자코드가 잘못되었습니다.";

            List<CSubAdmin0> list = CGlobal.GetSubAdmin0List().FindAll(value => value.m_nSuperCode == nSubCode);
            if (list != null && list.Count > 0)
            {
                int nMinPro = list.Max(value => value.m_nSelfPro);
                if (nSelfPro < nMinPro)
                {
                    return $"오류! 자체요율이 소속된 영본사 요율({nMinPro})보다 커야 합니다.";
                }
            }

            int nPreSelfPro = subAdmin.m_nSelfPro;
            int nPreOtherPro = subAdmin.m_nOtherPro;
            subAdmin.m_nSelfPro = nSelfPro;
            subAdmin.m_nOtherPro = nOtherPro;
            CDataBase.SaveSubAdmin(subAdmin);

            string strNote = $"관리자 {admin.m_strOnerNick}님이 본사의 요율 변경";
            CDataBase.InsertSubProLog(nSubCode, 0, nSuperCode, 100, 100, nPreSelfPro, subAdmin.m_nSelfPro, nPreOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }

        public static string SetSubAdmin0TotalPro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nTotalPro = Convert.ToInt32(json["nTotalPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사, 2-부본사, 3-총판, 4-매장
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nTotalPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin0 subAdmin = CGlobal.GetSubAdmin0ByCode(nSubCode);
            if (subAdmin == null)
                return "오류! 영본사코드가 잘못되었습니다.";

            CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
            if (admin == null)
                return "오류! 관리자코드가 잘못되었습니다.";

            CSubAdmin superAdmin = CGlobal.GetSubAdmin();
            if(superAdmin.m_nSelfPro < nTotalPro)
                return $"오류! 본사 자체요율{superAdmin.m_nSelfPro} 보다 작아야 합니다.";

            int nSelfPro = nTotalPro - subAdmin.m_nOtherPro;
            if (nSelfPro < 0)
            {
                return "오류! 자체요율이 0이상이여야 합니다.";
            }
            List<CSubAdmin1> list = CGlobal.GetSubAdmin1List().FindAll(value => value.m_nSuperCode == nSubCode);
            if (list != null && list.Count > 0)
            {
                int nMinPro = list.Max(value => value.m_nSelfPro);
                if (nSelfPro < nMinPro)
                {
                    return $"오류! 자체요율이 소속된 부본사 요율({nMinPro})보다 커야 합니다.";
                }
            }

            int nPreTotalPro = subAdmin.m_nTotalPro;
            int nPreSelfPro = subAdmin.m_nSelfPro;
            subAdmin.m_nTotalPro = nTotalPro;
            subAdmin.m_nSelfPro = nSelfPro;
            CDataBase.SaveSubAdmin0(subAdmin);

            string strNote = $"본사관리자 {admin.m_strOnerNick}님이 영본사 {subAdmin.m_strSubNick}님의 총요율 변경";
            CDataBase.InsertSubProLog0(nSubCode, nKind, nSuperCode, subAdmin.m_nTotalPro, nPreTotalPro, nPreSelfPro, subAdmin.m_nSelfPro, subAdmin.m_nOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }


        public static string SetSubAdmin0Pro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nOtherPro = Convert.ToInt32(json["nOtherPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nOtherPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin0 subAdmin = CGlobal.GetSubAdmin0ByCode(nSubCode);
            if (subAdmin == null)
                return "오류! 영본사코드가 잘못되었습니다.";

            int nSelfPro = subAdmin.m_nTotalPro - nOtherPro;
            if(nSelfPro < 0)
            {
                return "오류! 자체요율이 0이상이여야 합니다.";
            }

            List<CSubAdmin1> list = CGlobal.GetSubAdmin1List().FindAll(value => value.m_nSuperCode == nSubCode);
            if(list != null && list.Count > 0)
            {
                int nMinPro = list.Max(value => value.m_nSelfPro);
                if(nSelfPro < nMinPro)
                {
                    return $"오류! 자체요율이 소속된 부본사 요율({nMinPro})보다 커야 합니다.";
                }
            }

            
            string strNote = string.Empty;
            if(nKind == 0)
            {
                CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
                if (admin == null)
                    return "오류! 관리자코드가 잘못되었습니다.";
                strNote = $"본사관리자 {admin.m_strOnerNick}님이 영본사 {subAdmin.m_strSubNick}님의 이벤트요율 변경";
            }
            else
            {
                strNote = $"자체 이벤트요율 변경";
            }
            

            int nPreSelfPro = subAdmin.m_nSelfPro;
            int nPreOtherPro = subAdmin.m_nOtherPro;
            subAdmin.m_nSelfPro = nSelfPro;
            subAdmin.m_nOtherPro = nOtherPro;
            CDataBase.SaveSubAdmin0(subAdmin);

            CDataBase.InsertSubProLog0(nSubCode, nKind, nSuperCode, subAdmin.m_nTotalPro, subAdmin.m_nTotalPro, nPreSelfPro, subAdmin.m_nSelfPro, nPreOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }

        public static string SetSubAdmin1TotalPro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nTotalPro = Convert.ToInt32(json["nTotalPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사, 2-부본사, 3-총판, 4-매장
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nTotalPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin1 subAdmin = CGlobal.GetSubAdmin1ByCode(nSubCode);
            if (subAdmin == null)
                return "오류! 부본사코드가 잘못되었습니다.";
            

            CSubAdmin0 superAdmin = CGlobal.GetSubAdmin0ByCode(subAdmin.m_nSuperCode);
            if (superAdmin.m_nSelfPro < nTotalPro)
                return $"오류! 영본사 자체요율{superAdmin.m_nSelfPro} 보다 작아야 합니다.";

            int nSelfPro = nTotalPro - subAdmin.m_nOtherPro;
            if (nSelfPro < 0)
            {
                return "오류! 자체요율이 0이상이여야 합니다.";
            }
            List<CSubAdmin2> list = CGlobal.GetSubAdmin2List().FindAll(value => value.m_nSuperCode == nSubCode);
            if (list != null && list.Count > 0)
            {
                int nMinPro = list.Max(value => value.m_nSelfPro);
                if (nSelfPro < nMinPro)
                {
                    return $"오류! 자체요율이 소속된 총판 요율({nMinPro})보다 커야 합니다.";
                }
            }

            string strNote = string.Empty;
            if(nKind == 0)
            {
                CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
                if (admin == null)
                    return "오류! 관리자코드가 잘못되었습니다.";

                strNote = $"본사관리자 {admin.m_strOnerNick}님이 부본사 {subAdmin.m_strSubNick}님의 총요율 변경";
            }
            else if(nKind == 1)
            {
                strNote = $"영본사 {superAdmin.m_strSubNick}님이 부본사 {subAdmin.m_strSubNick}님의 총요율 변경";
            }


            int nPreTotalPro = subAdmin.m_nTotalPro;
            int nPreSelfPro = subAdmin.m_nSelfPro;
            subAdmin.m_nTotalPro = nTotalPro;
            subAdmin.m_nSelfPro = nSelfPro;
            CDataBase.SaveSubAdmin1(subAdmin);
            CDataBase.InsertSubProLog1(nSubCode, nKind, nSuperCode, subAdmin.m_nTotalPro, nPreTotalPro, nPreSelfPro, subAdmin.m_nSelfPro, subAdmin.m_nOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }

        public static string SetSubAdmin1Pro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nOtherPro = Convert.ToInt32(json["nOtherPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사, 2-부본사, 3-총판, 4-매장
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nOtherPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin1 subAdmin = CGlobal.GetSubAdmin1ByCode(nSubCode);
            if (subAdmin == null)
                return "오류! 부본사코드가 잘못되었습니다.";

            int nSelfPro = subAdmin.m_nTotalPro - nOtherPro;
            if (nSelfPro < 0)
            {
                return "오류! 자체요율이 0이상이여야 합니다.";
            }
            List<CSubAdmin2> list = CGlobal.GetSubAdmin2List().FindAll(value => value.m_nSuperCode == nSubCode);
            if (list != null && list.Count > 0)
            {
                int nMinPro = list.Max(value => value.m_nSelfPro);
                if (nSelfPro < nMinPro)
                {
                    return $"오류! 자체요율이 소속된 총판 요율({nMinPro})보다 커야 합니다.";
                }
            }

            string strNote = string.Empty;
            if (nKind == 0)
            {
                CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
                if (admin == null)
                    return "오류! 관리자코드가 잘못되었습니다.";

                strNote = $"본사관리자 {admin.m_strOnerNick}님이 부본사 {subAdmin.m_strSubNick}님의 이벤트요율 변경";
            }
            else if(nKind == 1)
            {
                CSubAdmin0 subAdmin0 = CGlobal.GetSubAdmin0ByCode(nSuperCode);
                if (subAdmin0 == null)
                    return "오류! 영본사코드가 잘못되었습니다.";

                strNote = $"영본사 {subAdmin0.m_strSubNick}님이 부본사 {subAdmin.m_strSubNick}님의 이벤트요율 변경";
            }
            else if (nKind == 2)
            {
                strNote = $"자체 이벤트요율 변경";
            }

            int nPreSelfPro = subAdmin.m_nSelfPro;
            int nPreOtherPro = subAdmin.m_nOtherPro;
            subAdmin.m_nSelfPro = nSelfPro;
            subAdmin.m_nOtherPro = nOtherPro;
            CDataBase.SaveSubAdmin1(subAdmin);

            CDataBase.InsertSubProLog1(nSubCode, nKind, nSuperCode, subAdmin.m_nTotalPro, subAdmin.m_nTotalPro, nPreSelfPro, subAdmin.m_nSelfPro, nPreOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }

        public static string SetSubAdmin2TotalPro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nTotalPro = Convert.ToInt32(json["nTotalPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사, 2-부본사, 3-총판, 4-매장
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nTotalPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin2 subAdmin = CGlobal.GetSubAdmin2ByCode(nSubCode);
            if (subAdmin == null)
                return "오류! 총판코드가 잘못되었습니다.";


            CSubAdmin1 superAdmin = CGlobal.GetSubAdmin1ByCode(subAdmin.m_nSuperCode);
            if (superAdmin.m_nSelfPro < nTotalPro)
                return $"오류! 영본사 자체요율{superAdmin.m_nSelfPro} 보다 작아야 합니다.";

            int nSelfPro = nTotalPro - subAdmin.m_nOtherPro;
            if (nSelfPro < 0)
            {
                return "오류! 자체요율이 0이상이여야 합니다.";
            }
            string strNote = string.Empty;
            if (nKind == 0)
            {
                CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
                if (admin == null)
                    return "오류! 관리자코드가 잘못되었습니다.";

                strNote = $"본사관리자 {admin.m_strOnerNick}님이 총판 {subAdmin.m_strSubNick}님의 총요율 변경";
            }
            else if (nKind == 1)
            {
                CSubAdmin0 subAdmin0 = CGlobal.GetSubAdmin0ByCode(nSuperCode);
                if(subAdmin0 == null)
                    return "오류! 영본사코드가 잘못되었습니다.";

                strNote = $"영본사 {subAdmin0.m_strSubNick}님이 총판 {subAdmin.m_strSubNick}님의 총요율 변경";
            }
            else if(nKind == 2)
            {
                strNote = $"부본사 {superAdmin.m_strSubNick}님이 총판 {subAdmin.m_strSubNick}님의 총요율 변경";
            }


            int nPreTotalPro = subAdmin.m_nTotalPro;
            int nPreSelfPro = subAdmin.m_nSelfPro;
            subAdmin.m_nTotalPro = nTotalPro;
            subAdmin.m_nSelfPro = nSelfPro;
            CDataBase.SaveSubAdmin2(subAdmin);
            CDataBase.InsertSubProLog2(nSubCode, nKind, nSuperCode, subAdmin.m_nTotalPro, nPreTotalPro, nPreSelfPro, subAdmin.m_nSelfPro, subAdmin.m_nOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }

        public static string SetSubAdmin2Pro(int nOnerCode, string strValue)
        {
            JToken json = JToken.Parse(strValue);
            int nSubCode = Convert.ToInt32(json["nSubCode"]);
            int nOtherPro = Convert.ToInt32(json["nOtherPro"]);
            int nKind = Convert.ToInt32(json["nKind"]);  //0-본사, 1-영본사, 2-부본사, 3-총판, 4-매장
            int nSuperCode = Convert.ToInt32(json["nSuperCode"]);  //변화시킨 상위 코드

            if (nOtherPro < 0)
                return "오류! 요율은 0이상이여야 합니다.";

            CSubAdmin2 subAdmin = CGlobal.GetSubAdmin2ByCode(nSubCode);
            if (subAdmin == null)
                return "오류! 총판코드가 잘못되었습니다.";

            int nSelfPro = subAdmin.m_nTotalPro - nOtherPro;
            if (nSelfPro < 0)
            {
                return "오류! 자체요율이 0이상이여야 합니다.";
            }
            string strNote = string.Empty;
            if (nKind == 0)
            {
                CAdmin admin = CGlobal.GetAdminByCode(nSuperCode);
                if (admin == null)
                    return "오류! 관리자코드가 잘못되었습니다.";

                strNote = $"본사관리자 {admin.m_strOnerNick}님이 총판 {subAdmin.m_strSubNick}님의 이벤트요율 변경";
            }
            else if (nKind == 1)
            {
                CSubAdmin0 subAdmin0 = CGlobal.GetSubAdmin0ByCode(nSuperCode);
                if (subAdmin0 == null)
                    return "오류! 영본사코드가 잘못되었습니다.";

                strNote = $"영본사 {subAdmin0.m_strSubNick}님이 총판 {subAdmin.m_strSubNick}님의 이벤트요율 변경";
            }
            else if (nKind == 2)
            {
                CSubAdmin0 subAdmin0 = CGlobal.GetSubAdmin0ByCode(nSuperCode);
                if (subAdmin0 == null)
                    return "오류! 부본사코드가 잘못되었습니다.";

                strNote = $"부본사 {subAdmin0.m_strSubNick}님이 총판 {subAdmin.m_strSubNick}님의 이벤트요율 변경";
            }
            else if (nKind == 3)
            {
                strNote = $"자체 이벤트요율 변경";
            }

            int nPreSelfPro = subAdmin.m_nSelfPro;
            int nPreOtherPro = subAdmin.m_nOtherPro;
            subAdmin.m_nSelfPro = nSelfPro;
            subAdmin.m_nOtherPro = nOtherPro;
            CDataBase.SaveSubAdmin2(subAdmin);

            CDataBase.InsertSubProLog2(nSubCode, nKind, nSuperCode, subAdmin.m_nTotalPro, subAdmin.m_nTotalPro, nPreSelfPro, subAdmin.m_nSelfPro, nPreOtherPro, subAdmin.m_nOtherPro, strNote);

            return "설정되었습니다.";
        }

        public static string RelaseRealCash(int nOnerCode, string strValue)
        {
            int nCode = Convert.ToInt32(strValue);
            string sql = $"SELECT * FROM tbl_subex WHERE nCode = {nCode}";
            DataRow info = CMysql.GetDataQuery(sql)[0];
            int nExCash = Convert.ToInt32(info["nExCash"]);
            int nStep = Convert.ToInt32(info["nStep"]);
            int nKind = Convert.ToInt32(info["nKind"]);
            int nSubCode = Convert.ToInt32(info["nSubCode"]);

            CSubAdmin superAdmin = CGlobal.GetSubAdmin();
            if(nStep == 0)
            {
                CSubAdmin subAdmin = CGlobal.GetSubAdmin();

                if (nKind == 0)
                {
                    int nPreMoney = subAdmin.m_nRealCash;
                    subAdmin.m_nRealCash -= nExCash;
                    subAdmin.m_nExRealCash += nExCash;
                    CDataBase.SaveSubAdmin(subAdmin);

                    string strNote = $"{nExCash}원 정립금정산진행";
                    CDataBase.InsertSubAdminCache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nRealCash, 2, 0, strNote);
                }
                else if (nKind == 1)
                {
                    int nPreMoney = subAdmin.m_nEventCash;
                    subAdmin.m_nEventCash -= nExCash;
                    subAdmin.m_nExEventCash += nExCash;
                    CDataBase.SaveSubAdmin(subAdmin);

                    string strNote = $"{nExCash}원 이벤트금정산진행";
                    CDataBase.InsertSubAdminCache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nEventCash, 2, 1, strNote);
                }
            }
            else if(nStep == 1)
            {
                CSubAdmin0 subAdmin = CGlobal.GetSubAdmin0ByCode(nSubCode);

                if (nKind == 0)
                {
                    int nPreMoney = subAdmin.m_nRealCash;
                    subAdmin.m_nRealCash -= nExCash;
                    subAdmin.m_nExRealCash += nExCash;
                    CDataBase.SaveSubAdmin0(subAdmin);

                    string strNote = $"{nExCash}원 정립금정산진행";
                    CDataBase.InsertSubAdmin0Cache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nRealCash, 2, 0, strNote);

                    superAdmin.m_nTotalExReal += nExCash;
                    CDataBase.SaveSubAdmin(superAdmin);
                }
                else if (nKind == 1)
                {
                    int nPreMoney = subAdmin.m_nEventCash;
                    subAdmin.m_nEventCash -= nExCash;
                    subAdmin.m_nExEventCash += nExCash;
                    CDataBase.SaveSubAdmin0(subAdmin);

                    string strNote = $"{nExCash}원 이벤트금정산진행";
                    CDataBase.InsertSubAdmin0Cache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nEventCash, 2, 1, strNote);
                }
            }
            else if (nStep == 2)
            {
                CSubAdmin1 subAdmin = CGlobal.GetSubAdmin1ByCode(nSubCode);

                if (nKind == 0)
                {
                    int nPreMoney = subAdmin.m_nRealCash;
                    subAdmin.m_nRealCash -= nExCash;
                    subAdmin.m_nExRealCash += nExCash;
                    CDataBase.SaveSubAdmin1(subAdmin);

                    string strNote = $"{nExCash}원 정립금정산진행";
                    CDataBase.InsertSubAdmin1Cache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nRealCash, 2, 0, strNote);

                    superAdmin.m_nTotalExReal += nExCash;
                    CDataBase.SaveSubAdmin(superAdmin);
                }
                else if (nKind == 1)
                {
                    int nPreMoney = subAdmin.m_nEventCash;
                    subAdmin.m_nEventCash -= nExCash;
                    subAdmin.m_nExEventCash += nExCash;
                    CDataBase.SaveSubAdmin1(subAdmin);

                    string strNote = $"{nExCash}원 이벤트금정산진행";
                    CDataBase.InsertSubAdmin1Cache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nEventCash, 2, 1, strNote);
                }
            }
            else if (nStep == 3)
            {
                CSubAdmin2 subAdmin = CGlobal.GetSubAdmin2ByCode(nSubCode);

                if (nKind == 0)
                {
                    int nPreMoney = subAdmin.m_nRealCash;
                    subAdmin.m_nRealCash -= nExCash;
                    subAdmin.m_nExRealCash += nExCash;
                    CDataBase.SaveSubAdmin2(subAdmin);

                    string strNote = $"{nExCash}원 정립금정산진행";
                    CDataBase.InsertSubAdmin2Cache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nRealCash, 2, 0, strNote);

                    superAdmin.m_nTotalExReal += nExCash;
                    CDataBase.SaveSubAdmin(superAdmin);
                }
                else if (nKind == 1)
                {
                    int nPreMoney = subAdmin.m_nEventCash;
                    subAdmin.m_nEventCash -= nExCash;
                    subAdmin.m_nExEventCash += nExCash;
                    CDataBase.SaveSubAdmin2(subAdmin);

                    string strNote = $"{nExCash}원 이벤트금정산진행";
                    CDataBase.InsertSubAdmin2Cache(subAdmin.m_nCode, -nExCash, nPreMoney, subAdmin.m_nEventCash, 2, 1, strNote);
                }
            }

            sql = $"UPDATE tbl_subex SET nOnerCode = {nOnerCode}, nCheck = 1, strAccTime = '{CMyTime.GetMyTimeStr()}' WHERE nCode = {nCode}";
            CMysql.ExcuteQuery(sql);

            return "정산되었습니다.";

        }


        public static string SaveSubAdmin0(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            string strSubID = Convert.ToString(packet["strSubID"]);
            string strSubNick = Convert.ToString(packet["strSubNick"]);
            string strSubPwd = Convert.ToString(packet["strSubPwd"]);
            string strPhone = Convert.ToString(packet["strPhone"]);
            string strMark = Convert.ToString(packet["strMark"]);
            int nState = Convert.ToInt32(packet["nState"]);

            if(nCode == 0)
            {
                //추가
                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CSubAdmin0> list = CGlobal.GetSubAdmin0List();
                if (list.Exists(value => value.m_strSubID == strSubID))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strSubNick == strSubNick))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strMark == strMark))
                    return "가입코드가 증복되었습니다.";

                CSubAdmin0 subAdmin = new CSubAdmin0();
                subAdmin.m_strSubID = strSubID;
                subAdmin.m_strSubNick = strSubNick;
                subAdmin.m_strSubPwd = strSubPwd;
                subAdmin.m_strPhone = strPhone;
                subAdmin.m_strMark = strMark;
                subAdmin.m_nState = nState;
                subAdmin.m_nSuperCode = 1;
                subAdmin.m_strRegTime = CMyTime.GetMyTimeStr();

                CDataBase.InsertSubAdmin0(subAdmin);
                list.Add(subAdmin);

                CSubAdmin superAdmin = CGlobal.GetSubAdmin();
                superAdmin.m_nChildCnt++;
                CDataBase.SaveSubAdmin(superAdmin);

                return "add";
            }
            else
            {
                //수정
                CSubAdmin0 subAdmin = CGlobal.GetSubAdmin0ByCode(nCode);
                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CSubAdmin0> list = CGlobal.GetSubAdmin0List();
                if (list.Exists(value => value.m_strSubID == strSubID && value.m_nCode != nCode))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strSubNick == strSubNick && value.m_nCode != nCode))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strMark == strMark && value.m_nCode != nCode))
                    return "가입코드가 증복되었습니다.";

                subAdmin.m_strSubID = strSubID;
                subAdmin.m_strSubNick = strSubNick;
                subAdmin.m_strSubPwd = strSubPwd;
                subAdmin.m_strPhone = strPhone;
                subAdmin.m_strMark = strMark;
                subAdmin.m_nState = nState;
                CDataBase.SaveSubAdmin0(subAdmin);

                return "update";
            }
        }

        public static string SaveSubAdmin1(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            string strSubID = Convert.ToString(packet["strSubID"]);
            string strSubNick = Convert.ToString(packet["strSubNick"]);
            string strSubPwd = Convert.ToString(packet["strSubPwd"]);
            string strPhone = Convert.ToString(packet["strPhone"]);
            string strMark = Convert.ToString(packet["strMark"]);
            string strSuperMark = Convert.ToString(packet["strSuperMark"]);
            int nState = Convert.ToInt32(packet["nState"]);

            CSubAdmin0 superAdmin = CGlobal.GetSubAdmin0List().Find(value => value.m_strMark == strSuperMark);
            if (superAdmin == null)
                return "상위코드가 잘못되었습니다.";
            if(superAdmin.m_nState == 0)
                return "상위영본사가 가입대기상태입니다.";
            if (superAdmin.m_nState == 2)
                return "상위영본사가 차단상태입니다.";

            int nSuperCode = superAdmin.m_nCode;

            if (nCode == 0)
            {
                //추가
                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CSubAdmin1> list = CGlobal.GetSubAdmin1List();
                if (list.Exists(value => value.m_strSubID == strSubID))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strSubNick == strSubNick))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strMark == strMark))
                    return "가입코드가 증복되었습니다.";

                CSubAdmin1 subAdmin = new CSubAdmin1();
                subAdmin.m_strSubID = strSubID;
                subAdmin.m_strSubNick = strSubNick;
                subAdmin.m_strSubPwd = strSubPwd;
                subAdmin.m_strPhone = strPhone;
                subAdmin.m_strMark = strMark;
                subAdmin.m_nState = nState;
               
                subAdmin.m_nSuperCode = nSuperCode;
                subAdmin.m_strRegTime = CMyTime.GetMyTimeStr();

                CDataBase.InsertSubAdmin1(subAdmin);
                list.Add(subAdmin);

                superAdmin.m_nChildCnt++;
                CDataBase.SaveSubAdmin0(superAdmin);

                return "add";
            }
            else
            {
                //수정
                CSubAdmin1 subAdmin = CGlobal.GetSubAdmin1ByCode(nCode);
                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CSubAdmin1> list = CGlobal.GetSubAdmin1List();
                if (list.Exists(value => value.m_strSubID == strSubID && value.m_nCode != nCode))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strSubNick == strSubNick && value.m_nCode != nCode))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strMark == strMark && value.m_nCode != nCode))
                    return "가입코드가 증복되었습니다.";

                subAdmin.m_strSubID = strSubID;
                subAdmin.m_strSubNick = strSubNick;
                subAdmin.m_strSubPwd = strSubPwd;
                subAdmin.m_strPhone = strPhone;
                subAdmin.m_strMark = strMark;
                subAdmin.m_nState = nState;
                CDataBase.SaveSubAdmin1(subAdmin);

                return "update";
            }
        }

        public static string SaveSubAdmin2(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            string strSubID = Convert.ToString(packet["strSubID"]);
            string strSubNick = Convert.ToString(packet["strSubNick"]);
            string strSubPwd = Convert.ToString(packet["strSubPwd"]);
            string strPhone = Convert.ToString(packet["strPhone"]);
            string strMark = Convert.ToString(packet["strMark"]);
            string strSuperMark = Convert.ToString(packet["strSuperMark"]);
            int nState = Convert.ToInt32(packet["nState"]);

            CSubAdmin1 superAdmin = CGlobal.GetSubAdmin1List().Find(value => value.m_strMark == strSuperMark);
            if (superAdmin == null)
                return "상위코드가 잘못되었습니다.";
            if (superAdmin.m_nState == 0)
                return "상위부본사가 가입대기상태입니다.";
            if (superAdmin.m_nState == 2)
                return "상위부본사가 차단상태입니다.";

            int nSuperCode = superAdmin.m_nCode;

            if (nCode == 0)
            {
                //추가
                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CSubAdmin2> list = CGlobal.GetSubAdmin2List();
                if (list.Exists(value => value.m_strSubID == strSubID))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strSubNick == strSubNick))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strMark == strMark))
                    return "가입코드가 증복되었습니다.";

                CSubAdmin2 subAdmin = new CSubAdmin2();
                subAdmin.m_strSubID = strSubID;
                subAdmin.m_strSubNick = strSubNick;
                subAdmin.m_strSubPwd = strSubPwd;
                subAdmin.m_strPhone = strPhone;
                subAdmin.m_strMark = strMark;
                subAdmin.m_nState = nState;
                subAdmin.m_nSuperCode = nSuperCode;
                subAdmin.m_strRegTime = CMyTime.GetMyTimeStr();

                CDataBase.InsertSubAdmin2(subAdmin);
                list.Add(subAdmin);

                superAdmin.m_nChildCnt++;
                CDataBase.SaveSubAdmin1(superAdmin);

                return "add";
            }
            else
            {
                //수정
                CSubAdmin2 subAdmin = CGlobal.GetSubAdmin2ByCode(nCode);

                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CSubAdmin2> list = CGlobal.GetSubAdmin2List();
                if (list.Exists(value => value.m_strSubID == strSubID && value.m_nCode != nCode))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strSubNick == strSubNick && value.m_nCode != nCode))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strMark == strMark && value.m_nCode != nCode))
                    return "가입코드가 증복되었습니다.";

                subAdmin.m_strSubID = strSubID;
                subAdmin.m_strSubNick = strSubNick;
                subAdmin.m_strSubPwd = strSubPwd;
                subAdmin.m_strPhone = strPhone;
                subAdmin.m_strMark = strMark;
                subAdmin.m_nState = nState;
                CDataBase.SaveSubAdmin2(subAdmin);

                return "update";
            }
        }

        public static string SaveSubAdmin3(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            string strSubID = Convert.ToString(packet["strSubID"]);
            string strSubNick = Convert.ToString(packet["strSubNick"]);
            string strSubPwd = Convert.ToString(packet["strSubPwd"]);
            string strPhone = Convert.ToString(packet["strPhone"]);
            string strMark = Convert.ToString(packet["strMark"]);
            int nCupnPro = Convert.ToInt32(packet["nCupnPro"]);
            string strSuperMark = Convert.ToString(packet["strSuperMark"]);
            int nState = Convert.ToInt32(packet["nState"]);

            CSubAdmin2 superAdmin = CGlobal.GetSubAdmin2List().Find(value => value.m_strMark == strSuperMark);
            if (superAdmin == null)
                return "상위코드가 잘못되었습니다.";
            if (superAdmin.m_nState == 0)
                return "상위총판이 가입대기상태입니다.";
            if (superAdmin.m_nState == 2)
                return "상위총판이 차단상태입니다.";

            int nSuperCode = superAdmin.m_nCode;

            if (nCode == 0)
            {
                //추가
                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CStore> list = CGlobal.GetStoreList();
                if (list.Exists(value => value.m_strStoreID == strSubID))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strStoreNick == strSubNick))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strStoreMark == strMark))
                    return "가입코드가 증복되었습니다.";

                CStore subAdmin = new CStore();
                subAdmin.m_strStoreID = strSubID;
                subAdmin.m_strStoreNick = strSubNick;
                subAdmin.m_strStorePW = strSubPwd;
                subAdmin.m_strStorePhone = strPhone;
                subAdmin.m_strStoreMark = strMark;
                subAdmin.m_nStoreState = nState;
                subAdmin.m_nCupnPro = nCupnPro;
                subAdmin.m_nSuperCode = nSuperCode;
                subAdmin.m_strStoreRegTime = CMyTime.GetMyTimeStr();

                CDataBase.InsertStoreInfoToDB(subAdmin);
                list.Add(subAdmin);

                superAdmin.m_nChildCnt++;
                CDataBase.SaveSubAdmin2(superAdmin);

                return "add";
            }
            else
            {
                //수정
                CStore subAdmin = CGlobal.GetStoreByCode(nCode);

                if (strMark == null || strMark.Trim() == string.Empty)
                    return "가입코드를 설정해주세요.";
                List<CStore> list = CGlobal.GetStoreList();
                if (list.Exists(value => value.m_strStoreID == strSubID && value.m_nStoreCode != nCode))
                    return "아이디가 이미 존재합니다.";
                if (list.Exists(value => value.m_strStoreNick == strSubNick && value.m_nStoreCode != nCode))
                    return "닉네임이 이미 존재합니다.";
                if (list.Exists(value => value.m_strStoreMark == strMark && value.m_nStoreCode != nCode))
                    return "가입코드가 증복되었습니다.";

                subAdmin.m_strStoreID = strSubID;
                subAdmin.m_strStoreNick = strSubNick;
                subAdmin.m_strStorePW = strSubPwd;
                subAdmin.m_strStorePhone = strPhone;
                subAdmin.m_strStoreMark = strMark;
                subAdmin.m_nStoreState = nState;
                subAdmin.m_nCupnPro = nCupnPro;
                CDataBase.SaveStoreInfoToDB(subAdmin);

                return "update";
            }
        }


        public static string ClearExcupnRealStore(int nOnerCode, string strValue)
        {
            int nStoreCode = Convert.ToInt32(strValue);
            CStore store = CGlobal.GetStoreByCode(nStoreCode);
            if(store == null)
            {
                return "매장코드가 잘못되었습니다.";
            }
            store.m_nExcupnReal = 0;
            CDataBase.SaveStoreInfoToDB(store);

            return "초기화되었습니다.";
        }

        public static string ExchangeAlBetweenStore(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nStoreCode = Convert.ToInt32(packet["nStoreCode"]);
            string strRevMark = Convert.ToString(packet["strRevMark"]);
            int nSendCash = Convert.ToInt32(packet["nSendCash"]);
            string strPassword = Convert.ToString(packet["strPassword"]);
            string strNote = Convert.ToString(packet["strNote"]);

            CStore sendStore = CGlobal.GetStoreByCode(nStoreCode);
            if(sendStore == null)
            {
                return "매장세션 오류! 다시 로그인해주세요.";
            }

            CStore recvStore = CGlobal.GetStoreList().Find(value => value.m_strStoreMark == strRevMark);
            if(recvStore == null)
            {
                return "받을 매장코드가 잘못되었습니다.";
            }

            if(sendStore.m_strStorePW != strPassword)
            {
                return "매장비번이 잘못되었습니다.";
            }
            string sql = $"SELECT CASE WHEN SUM(exCash) IS NULL THEN 0 ELSE SUM(exCash) END AS sumCash FROM tbl_excharge WHERE (exCheck = 0 OR exCheck = 3) AND storeCode = {nStoreCode}";
            int nSumExCash = Convert.ToInt32(CMysql.GetDataQuery(sql)[0]["sumCash"]);

            if(sendStore.m_nStoreCash < nSumExCash + nSendCash)
            {
                return "보내는 금액이 보유금액보다 작아야 합니다.";
            }

            //디비에 전환내역을 보관해야 한다
            sql = $"INSERT INTO tbl_alexchange(strTime, nSendCash, nSendStore, nRecvStore, strNote) VALUES('{CMyTime.GetMyTimeStr()}', {nSendCash}, {nStoreCode}, {recvStore.m_nStoreCode}, '{strNote}')";
            CDataBase.PushOtherQuery(sql);

            //전송하는 매장에서 알환전을 자동으로 한것으로 해야 한다.
            sendStore.m_nStoreCash -= nSendCash;
            string strLog = $"매장 {sendStore.m_strStoreNick}님이 매장 {recvStore.m_strStoreNick}에게 알 {nSendCash}원 보냄";
            CDataBase.InsertStoreCash(sendStore.m_nStoreCode, -nSendCash, sendStore.m_nStoreCash + nSendCash, sendStore.m_nStoreCash, 4, strLog);
            CDataBase.SaveStoreInfoToDB(sendStore);

            //받은 매장에서는 알충전을 자동으로 한것으로 해야 한다.
            recvStore.m_nStoreCash += nSendCash;
            strLog = $"매장 {recvStore.m_strStoreNick}님이 매장 {sendStore.m_strStoreNick}으로부터 알 {nSendCash}원 받음";
            CDataBase.InsertStoreCash(recvStore.m_nStoreCode, nSendCash, recvStore.m_nStoreCash - nSendCash, recvStore.m_nStoreCash, 4, strLog);
            CDataBase.SaveStoreInfoToDB(recvStore);


            //상위총판이 서로 다르다면 총판정산을 해주어야 한다.
            if (sendStore.m_nSuperCode == recvStore.m_nSuperCode)
            {
                return "알을 보내었습니다.";
            }
            CSubAdmin2 sendAdmin2 = sendStore.m_clsSubAdmin2;
            int nSubPreMoney = sendAdmin2.m_nRealCash;
            int nSelfPro = sendAdmin2.m_nSelfPro;
            int nRealCash = Convert.ToInt32(nSendCash * nSelfPro / 100);
            if (nRealCash > 0)
            {
                sendAdmin2.m_nRealCash -= nRealCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nSelfPro}%";
                CDataBase.InsertSubAdmin2Cache(sendAdmin2.m_nCode, -nRealCash, nSubPreMoney, sendAdmin2.m_nRealCash, 3, 0, strLog);
            }

            int nSubPreEvent = sendAdmin2.m_nEventCash;
            int nEventPro = sendAdmin2.m_nOtherPro;
            int nEventCash = Convert.ToInt32(nSendCash * nEventPro / 100);
            if (nEventCash > 0)
            {
                sendAdmin2.m_nEventCash -= nEventCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nEventPro}%";
                CDataBase.InsertSubAdmin2Cache(sendAdmin2.m_nCode, -nEventCash, nSubPreEvent, sendAdmin2.m_nEventCash, 3, 1, strLog);
            }
            CDataBase.SaveSubAdmin2(sendAdmin2);

            CSubAdmin2 recvAdmin2 = recvStore.m_clsSubAdmin2;
            nSubPreMoney = recvAdmin2.m_nRealCash;
            nSelfPro = recvAdmin2.m_nSelfPro;
            nRealCash = Convert.ToInt32(nSendCash * nSelfPro / 100);
            if (nRealCash > 0)
            {
                recvAdmin2.m_nRealCash += nRealCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nSelfPro}%";
                CDataBase.InsertSubAdmin2Cache(recvAdmin2.m_nCode, nRealCash, nSubPreMoney, recvAdmin2.m_nRealCash, 3, 0, strLog);
            }

            nSubPreEvent = recvAdmin2.m_nEventCash;
            nEventPro = recvAdmin2.m_nOtherPro;
            nEventCash = Convert.ToInt32(nSendCash * nEventPro / 100);
            if (nEventCash > 0)
            {
                recvAdmin2.m_nEventCash += nEventCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nEventPro}%";
                CDataBase.InsertSubAdmin2Cache(recvAdmin2.m_nCode, nEventCash, nSubPreEvent, recvAdmin2.m_nEventCash, 3, 1, strLog);
            }
            CDataBase.SaveSubAdmin2(recvAdmin2);


            //부본사가 다르다면 부본사정산을 진행해주어야 한다.
            if(sendStore.m_clsSubAdmin1.m_nCode == recvStore.m_clsSubAdmin1.m_nCode)
            {
                return "알을 보내었습니다.";
            }
            CSubAdmin1 sendAdmin1 = sendStore.m_clsSubAdmin1;
            nSubPreMoney = sendAdmin1.m_nRealCash;
            nSelfPro = sendAdmin1.m_nSelfPro - sendAdmin2.m_nTotalPro;
            nRealCash = Convert.ToInt32(nSendCash * nSelfPro / 100);
            if (nRealCash > 0)
            {
                sendAdmin1.m_nRealCash -= nRealCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nSelfPro}%";
                CDataBase.InsertSubAdmin1Cache(sendAdmin1.m_nCode, -nRealCash, nSubPreMoney, sendAdmin1.m_nRealCash, 3, 0, strLog);
            }

            nSubPreEvent = sendAdmin1.m_nEventCash;
            nEventPro = sendAdmin1.m_nOtherPro;
            nEventCash = Convert.ToInt32(nSendCash * nEventPro / 100);
            if (nEventCash > 0)
            {
                sendAdmin1.m_nEventCash -= nEventCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nEventPro}%";
                CDataBase.InsertSubAdmin1Cache(sendAdmin1.m_nCode, -nEventCash, nSubPreEvent, sendAdmin1.m_nEventCash, 3, 1, strLog);
            }
            CDataBase.SaveSubAdmin1(sendAdmin1);

            CSubAdmin1 recvAdmin1 = recvStore.m_clsSubAdmin1;
            nSubPreMoney = sendAdmin1.m_nRealCash;
            nSelfPro = recvAdmin1.m_nSelfPro - recvAdmin2.m_nTotalPro;
            nRealCash = Convert.ToInt32(nSendCash * nSelfPro / 100);
            if (nRealCash > 0)
            {
                recvAdmin1.m_nRealCash += nRealCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nSelfPro}%";
                CDataBase.InsertSubAdmin1Cache(recvAdmin1.m_nCode, nRealCash, nSubPreMoney, recvAdmin1.m_nRealCash, 3, 0, strLog);
            }

            nSubPreEvent = recvAdmin1.m_nEventCash;
            nEventPro = recvAdmin1.m_nOtherPro;
            nEventCash = Convert.ToInt32(nSendCash * nEventPro / 100);
            if (nEventCash > 0)
            {
                recvAdmin1.m_nEventCash += nEventCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nEventPro}%";
                CDataBase.InsertSubAdmin1Cache(recvAdmin1.m_nCode, nEventCash, nSubPreEvent, recvAdmin1.m_nEventCash, 3, 1, strLog);
            }
            CDataBase.SaveSubAdmin1(recvAdmin1);


            //영본사가 다르다면 영본사정산을 진행해주어야 한다.
            if(sendStore.m_clsSubAdmin0.m_nCode == recvStore.m_clsSubAdmin0.m_nCode)
            {
                return "알을 보내었습니다.";
            }

            CSubAdmin0 sendAdmin0 = sendStore.m_clsSubAdmin0;
            nSubPreMoney = sendAdmin0.m_nRealCash;
            nSelfPro = sendAdmin0.m_nSelfPro - sendAdmin1.m_nTotalPro;
            nRealCash = Convert.ToInt32(nSendCash * nSelfPro / 100);
            if (nRealCash > 0)
            {
                sendAdmin0.m_nRealCash -= nRealCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nSelfPro}%";
                CDataBase.InsertSubAdmin0Cache(sendAdmin0.m_nCode, -nRealCash, nSubPreMoney, sendAdmin0.m_nRealCash, 3, 0, strLog);
            }

            nSubPreEvent = sendAdmin0.m_nEventCash;
            nEventPro = sendAdmin0.m_nOtherPro;
            nEventCash = Convert.ToInt32(nSendCash * nEventPro / 100);
            if (nEventCash > 0)
            {
                sendAdmin0.m_nEventCash -= nEventCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nEventPro}%";
                CDataBase.InsertSubAdmin0Cache(sendAdmin0.m_nCode, -nEventCash, nSubPreEvent, sendAdmin0.m_nEventCash, 3, 1, strLog);
            }
            CDataBase.SaveSubAdmin0(sendAdmin0);

            CSubAdmin0 recvAdmin0 = recvStore.m_clsSubAdmin0;
            nSubPreMoney = recvAdmin0.m_nRealCash;
            nSelfPro = recvAdmin0.m_nSelfPro - recvAdmin1.m_nTotalPro;
            nRealCash = Convert.ToInt32(nSendCash * nSelfPro / 100);
            if (nRealCash > 0)
            {
                recvAdmin0.m_nRealCash += nRealCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nSelfPro}%";
                CDataBase.InsertSubAdmin0Cache(recvAdmin0.m_nCode, nRealCash, nSubPreMoney, recvAdmin0.m_nRealCash, 3, 0, strLog);
            }

            nSubPreEvent = recvAdmin0.m_nEventCash;
            nEventPro = recvAdmin0.m_nOtherPro;
            nEventCash = Convert.ToInt32(nSendCash * nEventPro / 100);
            if (nEventCash > 0)
            {
                recvAdmin0.m_nEventCash += nEventCash;
                strLog = $"{sendStore.m_strStoreNick}매장 {recvStore.m_strStoreNick}매장에로 {nSendCash}원 알전송 요율 {nEventPro}%";
                CDataBase.InsertSubAdmin0Cache(recvAdmin0.m_nCode, nEventCash, nSubPreEvent, recvAdmin0.m_nEventCash, 3, 1, strLog);
            }
            CDataBase.SaveSubAdmin0(recvAdmin0);

            return "알을 보내었습니다.";
        }

        public static string SetItemPrice(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nItemCode = Convert.ToInt32(packet["nCode"]);
            string strItem = Convert.ToString(packet["strItem"]);
            int nPrice = Convert.ToInt32(packet["nPrice"]);
            int nLosePro = Convert.ToInt32(packet["nLosePro"]);
            string strNote = Convert.ToString(packet["strNote"]);

            CItemModel itemModel = CGlobal.GetItemModelByCode(nItemCode);
            if (itemModel == null)
            {
                return "아이템코드가 잘못되었습니다.";
            }

            itemModel.m_strItem = strItem;
            itemModel.m_strNote = strNote;
            itemModel.m_nPrice = nPrice;
            itemModel.m_nLosePro = nLosePro;

            CDataBase.SaveItemModel(itemModel);

            return "수정되었습니다.";
        }

        public static string SetItemJackCash(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            int nCash = Convert.ToInt32(packet["nCash"]);

            CGlobal.SetItemEngineJackCash(nCode, nCash);
            return "수정되었습니다.";
        }

        public static string SetItemUseCash(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            int nCash = Convert.ToInt32(packet["nCash"]);

            CGlobal.SetItemEngineItemCash(nCode, nCash);
            return "수정되었습니다.";
        }

        public static string SetItemRaiseCash(int nOnerCode, string strValue)
        {
            JToken packet = JToken.Parse(strValue);
            int nCode = Convert.ToInt32(packet["nCode"]);
            int nCash = Convert.ToInt32(packet["nCash"]);

            CGlobal.SetItemEngineRaiseCash(nCode, nCash);
            return "수정되었습니다.";
        }
    }
    
}
