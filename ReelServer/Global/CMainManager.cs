using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public static partial class CGlobal
    {
        private static string _strDate = CMyTime.GetMyTimeStr("yyyy-MM-dd");


        public static void SetVirtualGearValue()
        {
            DateTime curTime = CMyTime.GetMyTime();
            int nCount = (int)curTime.DayOfWeek + 1;
            //선택되지 않은 기계들의 누적금액을 가상적으로 설정을 한다.
            List<CBaseGear> lstGear = GetGearList().FindAll(value => value.m_nTakeUser == 0 && value.m_nGearCheck == 0);
            for (int i = 0; i < lstGear.Count; i++)
            {
                if (lstGear[i].m_nGameCode == CDefine.GAME_YMT)
                    continue;

                int nAccuCash = _RND.Next(500, 3001) * 100;
                lstGear[i].m_nAccuCash = nAccuCash;
                //lstGear[i].m_nJackCash = 0;
                lstGear[i].m_nGrandCount = _RND.Next(0, nCount);
                //lstGear[i].m_nJackCash += lstGear[i].m_nGrandCount * _RND.Next(70, 101) * 10000;
                lstGear[i].m_nMajorCount = _RND.Next(0, nCount);
                //lstGear[i].m_nJackCash += lstGear[i].m_nMajorCount * _RND.Next(50, 70) * 10000;
                lstGear[i].m_nMinorCount = _RND.Next(0, nCount);
                //lstGear[i].m_nJackCash += lstGear[i].m_nMinorCount * _RND.Next(20, 50) * 10000;
                lstGear[i].m_nMiniCount = _RND.Next(0, nCount);
                //lstGear[i].m_nJackCash += lstGear[i].m_nMiniCount * _RND.Next(5, 20) * 10000;

                if (lstGear[i].m_nGrandCount > 0)
                {
                    lstGear[i].m_nTopJackCash = _RND.Next(70, 101) * 10000;
                }
                else if (lstGear[i].m_nMajorCount > 0)
                {
                    lstGear[i].m_nTopJackCash = _RND.Next(50, 70) * 10000;
                }
                else if (lstGear[i].m_nMinorCount > 0)
                {
                    lstGear[i].m_nTopJackCash = _RND.Next(20, 50) * 10000;
                }
                else if (lstGear[i].m_nMiniCount > 0)
                {
                    lstGear[i].m_nTopJackCash = _RND.Next(5, 20) * 10000;
                }

                int nDay = _RND.Next(0, 5);
                int nHour = _RND.Next(0, 24);
                int nMin = _RND.Next(0, 60);
                int nSec = _RND.Next(0, 60);

                DateTime dtTime = CMyTime.GetMyTime().AddDays(-nDay).AddHours(-nHour).AddMinutes(-nMin).AddSeconds(-nSec);
                if (lstGear[i].m_dtLastJackTime < dtTime)
                {
                    lstGear[i].m_dtLastJackTime = dtTime;
                    lstGear[i].m_nLastJackCash = _RND.Next(5, 101) * 10000;
                }
                else if (lstGear[i].m_nLastJackCash == 0)
                {
                    lstGear[i].m_nLastJackCash = _RND.Next(5, 101) * 10000;
                }


                CDataBase.SaveGearInfoToDB(lstGear[i]);
            }
        }

        public static void OnStartManagerThread()
        {
            new Thread(OnStartManager).Start();
        }

        private static void OnStartManager()
        {
            for (int i = 0; i < _lstGameEngine.Count; i++)
            {
                if (_lstGameEngine[i].m_nGameUse == 1)
                    new Thread(_lstGameEngine[i].RealTime).Start();
            }

            //로봇자동로그인기능추가
            new Thread(RobotAutoLogin).Start();
            //실시간적으로 기계의 선택상태를 체크한다.
            //new Thread(CheckGearList).Start();

            if (_clsBonusVirtual.m_nAutoExcharge == 1)
                MakeVirtualExcargeData();


            while (true)
            {
                //로봇에 대한 실시간 계산을 진행한다.
                CalcRobotJackPot();

                //실시간적으로 시스템잭팟금액을 브로드카스트한다.
                BoardCastRealTimeJackPot();

                

               
                //기계예약을 위한 부분
                List<CBaseGear> lstGear = _lstGear.FindAll(value => value.m_nGearCheck == 1 && (CMyTime.GetMyTime() - value.m_dtCheckTime).TotalHours >= 24);
                if (lstGear != null)
                {
                    for (int i = 0; i < lstGear.Count; i++)
                    {
                        lstGear[i].m_nGearCheck = 0;

                        if (lstGear[i].m_nSlotCash > 0 || lstGear[i].m_nGiftCash > 0)
                            continue;

                        CUser user = CGlobal.GetUserByCode(lstGear[i].m_nTakeUser);
                        if(user != null)
                            user.m_lstGear.Remove(lstGear[i]);
                        lstGear[i].m_nTakeUser = 0;
                        lstGear[i].m_nGearState = 0;
                        CDataBase.SaveGearInfoToDB(lstGear[i]);

                        if (lstGear[i].m_nAccuCash >= 50 * 10000)
                        {
                            //로봇넣기 함수 기대누적금액이 50만점 이상이면 자동적으로 로봇을 기대에 넣는다.
                            lstGear[i].m_nAccuCash = _RND.Next(100, 300);
                            lstGear[i].m_nAccuCash *= 100;
                            //new Thread(() => InsertRobotToGear(lstGear[i])).Start();
                        }
                        else if(lstGear[i].m_nAccuCash < -(50 * 10000))
                        {
                            lstGear[i].m_nAccuCash = _RND.Next(100, 300);
                            lstGear[i].m_nAccuCash *= 100;
                        }
                    }
                }

                
                DateTime curTime = CMyTime.GetMyTime();

                //관리자페지에서 입출금가상내역을 자동으로 넣는 부분
                if (_clsBonusVirtual.m_nAutoExcharge == 1 &&  _lstAutoExcharge.Count > 0)
                {
                    CAutoExcharge autoExcharge = _lstAutoExcharge.Find(value => value.m_nHour == curTime.Hour && value.m_nMinute == curTime.Minute && value.m_nSecond == curTime.Second);
                    if (autoExcharge != null)
                    {
                        CDataBase.InsertVirtualExcargeToDB(autoExcharge, curTime.ToString("yyyy-MM-dd HH:mm:ss"));

                        _lstAutoExcharge.Remove(autoExcharge);
                    }
                }

                if (curTime.ToString("yyyy-MM-dd") == _strDate)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                //날자가 바뀔때 실행이 된다. ----------------------------------------------------------------------
                _strDate = curTime.ToString("yyyy-MM-dd");

                //출석부를 위한 기능
                ClearAbsentInfo();
                
                //총판들의 정산금액을 디비에 써넣는다.
                CDataBase.SaveAgentRealCash();

                //로봇들의 가상입출금내역추가
                if (_clsBonusVirtual.m_nAutoExcharge == 1)
                    MakeVirtualExcargeData();

                //하루에 한번씩 디비에서 지나간 자료는 지운다.
                CDataBase.ClearDayToDB();

                //야마토에서는 날자가 바뀔때 오늘 잭팟출현회수를 수정하여야 한다.
                ModifyYmatGearJackCpunt();

                //일요일 23:59:59 이면 기계의 주간 누적금액과 잭팟출현회수를 초기화 해주어야 한다.
                if (curTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    for (int i = 0; i < _lstGear.Count; i++)
                    {
                        if(_lstGear[i].m_nGameCode != CDefine.GAME_YMT)
                        {
                            //_lstGear[i].m_nJackCash = 0;
                            _lstGear[i].m_nGrandCount = 0;
                            _lstGear[i].m_nMajorCount = 0;
                            _lstGear[i].m_nMinorCount = 0;
                            _lstGear[i].m_nMiniCount = 0;
                        }

                        if (_lstGear[i].m_nAccuCash > 10000000)
                            _lstGear[i].m_nAccuCash = 5000000;
                        else if(_lstGear[i].m_nAccuCash < -5000000)
                            _lstGear[i].m_nAccuCash = 0;
                    }

                    CDataBase.ClearGearWeekToDB();
                }

                Thread.Sleep(1000);
            }
        }

        private static void BoardCastRealTimeJackPot()
        {
            if (_lstAdminSocket.Count > 0)
            {
                _lstAdminSocket[0].StartRealTimeBoardCast(_clsJacPot);
            }

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_REAL_BOARDCAST);
            pktSendData.strToken = "BroadCast";

            pktSendData.strPktData = _clsJacPot.m_nGrand + "," + _clsJacPot.m_nMajor + "," + _clsJacPot.m_nMinor + "," + _clsJacPot.m_nMini;
            string strPacket = JsonConvert.SerializeObject(pktSendData);

            UserBroadCastPacket(strPacket);
        }

        //로봇자동로그인함수
        public static void RobotAutoLogin()
        {
            while (true)
            {
                int nTime = _RND.Next(15 * 60 * 1000, 40 * 60 * 1000);
                Thread.Sleep(nTime);

                List<CRobot> lstRobot = _lstRobot.FindAll(value => value.m_nJackpot == 0);
                if (lstRobot == null || lstRobot.Count == 0)
                    continue;

                CRobot robot = RandomSelect(lstRobot);

                string strChat = "[C][09f909]운영자: " + robot.m_strRbNick + " 님 입장을 환영합니다.[-]";


                SendNoticeBroadCast(strChat);
            }

        }

        private static void MakeVirtualExcargeData()
        {
            _lstAutoExcharge.Clear();

            List<CRobot> lstRobot = _lstRobot.FindAll(value => value.m_nAutoJack == 1);
            List<CAutoExcharge> lstExcharge = new List<CAutoExcharge>();
            int nExChargeCnt = _RND.Next(15, 20);
            for (int i = 0; i < nExChargeCnt; i++)
            {
                if (lstRobot.Count == 0)
                    break;

                CAutoExcharge autoExcharge = new CAutoExcharge();
                autoExcharge.m_nHour = _RND.Next(0, 24);
                autoExcharge.m_nMinute = _RND.Next(0, 60);
                autoExcharge.m_nSecond = _RND.Next(0, 60);
                autoExcharge.m_nExCash = _RND.Next(5, 30) * 10000;
                CRobot robot = lstRobot[_RND.Next(lstRobot.Count)];
                lstRobot.Remove(robot);
                autoExcharge.m_nRbCode = robot.m_nRbCode;

                lstExcharge.Add(autoExcharge);
            }

            _lstAutoExcharge = lstExcharge.OrderBy(value => value.m_nHour).ToList().OrderBy(value => value.m_nMinute).ToList().OrderBy(value => value.m_nSecond).ToList();
        }

        public static void RemoveNotionalJackStep(int nJackAmount, int nUserCode, int nGameCode)
        {
            CGameEngine engine = _lstGameEngine.Find(value => value.m_nGameCode == nGameCode);
            engine.RemoveNotionalJackStep(nJackAmount, nUserCode);
        }

        public static void ResetTotalCash(int nGameCode)
        {
            CGameEngine engine = _lstGameEngine.Find(value => value.m_nGameCode == nGameCode);
            engine.ResetTotalCash();
        }

        public static void SendSecurityChat(int nOnerCode, int nUserCode, int nType, string strMsg)
        {
            CSChatPacket chatPacket = new CSChatPacket();
            chatPacket.m_nOnerCode = nOnerCode;
            chatPacket.m_nUserCode = nUserCode;
            chatPacket.m_nType = nType;
            chatPacket.m_strMsg = strMsg;
            chatPacket.m_strTime = CMyTime.GetMyTimeStr();
            CDataBase.InsertSecurityChatToDB(chatPacket);

            if (nType == 0)
            {
                chatPacket.m_strNick = "관리자";
            }
            else
            {
                chatPacket.m_strNick = GetUserNickByCode(nUserCode);
            }


            if (_lstAdminSocket.Count > 0)
                _lstAdminSocket[0].SendSecurityChat(chatPacket);

            if (_lstUserSocket.Exists(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == nUserCode))
            {
                CUserSocket clsUserSocket = _lstUserSocket.Find(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == nUserCode);
                clsUserSocket.SendSecurityChat(chatPacket);
            }
            else
            {
                GetUserByCode(nUserCode).m_lstTempSecurityChat.Add(chatPacket);
            }
        }

        public static void SendGroupChat(int nCode, int nKind, string strMsg)
        {
            CGChatPacket chatPacket = new CGChatPacket();
            chatPacket.m_nCode = nCode;
            chatPacket.m_nKind = nKind;
            chatPacket.m_strMsg = strMsg;
            chatPacket.m_strTime = CMyTime.GetMyTimeStr();

            if (nKind == 0)
                chatPacket.m_strNick = "관리자";
            else if (nKind == 1)
                chatPacket.m_strNick = GetUserNickByCode(nCode);
            else if (nKind == 2)
            {
                chatPacket.m_strNick = GetRobotNickByCode(nCode);
                CDataBase.InsertRobotChatToDB(chatPacket);
            }

            CDataBase.InsertGroupChatToDB(chatPacket);

            if (_lstAdminSocket.Count > 0)
                _lstAdminSocket[0].SendGroupChat(chatPacket);

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_CHART_RESPONE);
            pktSendData.strToken = "BroadCast";
            pktSendData.strPktData = JsonConvert.SerializeObject(chatPacket);
            string strPacket = JsonConvert.SerializeObject(pktSendData);

            UserBroadCastPacket(strPacket);
        }

        public static void SendAdminToUserMessage(int nUserCode, string strMsg, int nAcid = 0)
        {
            CUserSocket clsUserSocket = _lstUserSocket.Find(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == nUserCode && value.IsApp());
            if (clsUserSocket != null)
                clsUserSocket.SendAdminToUserMessage(strMsg, nAcid);
        }

        public static void SendNoticeBroadCast(string strNotice)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_NOTICE_SEND);
            pktSendData.strToken = "BroadCast";
            pktSendData.strPktData = strNotice;
            string strPacket = JsonConvert.SerializeObject(pktSendData);

            _lstNoticeHistory.Add(strNotice);
            if (_lstNoticeHistory.Count > 20)
                _lstNoticeHistory.RemoveAt(0);

            UserBroadCastPacket(strPacket);
        }
        public static void AddChatSocket(CAdminSocket clsAdminScoket)
        {
            _lstAdminSocket.Add(clsAdminScoket);
        }


        public static void SendClientClearGearCash(CBaseGear clsGear)
        {
            int nUserCode = clsGear.m_nTakeUser;
            CUserSocket clsSocket = GetUserSocketByUserCode(nUserCode);

            if (clsSocket != null)
                clsSocket.SendClientClearGearCash(clsGear);
        }

        public static void SendGearInfoToClient(CBaseGear clsGear)
        {
            int nUserCode = clsGear.m_nTakeUser;
            CUserSocket clsSocket = GetUserSocketByUserCode(nUserCode);

            if (clsSocket != null)
                clsSocket.SendGearInfo(clsGear);
        }

        public static void SendOnerMessage(int nUserCode, string strMsg, int nAcid = 0)
        {
            CUserSocket client = _lstUserSocket.Find(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == nUserCode && value.IsApp());
            if (client != null)
                client.SendOnerMessage(strMsg, nAcid);
        }

        public static void SendClientReleaseGear(CBaseGear clsGear)
        {
            int nUserCode = clsGear.m_nTakeUser;
            CUserSocket clsSocket = GetUserSocketByUserCode(nUserCode);

            if (clsSocket != null)
                clsSocket.SendClientReleaseGear(clsGear);
        }

        public static void SendRobotInfoToAdmin(CRobot robot)
        {
            if (_lstAdminSocket.Count > 0)
                _lstAdminSocket[0].SendRobotInfo(robot);
        }


        public static void SendUserInfoToClient(CUser user)
        {
            List<CUserSocket> lstClient = _lstUserSocket.FindAll(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == user.m_nUserCode);
            if (lstClient != null && lstClient.Count > 0)
            {
                foreach(CUserSocket client in lstClient)
                    client.SendUserInfo();
            }
        }

        public static void UserBroadCastPacket(string strPacket)
        {
            foreach (CUserSocket socket in _lstUserBroadCastSocket)
            {
                socket.UserBroadCastPacket(strPacket);
            }
        }


        public static void BroadCastPrizeInfo(CPrizeInfoBroadCast clsPrizeInfoPacket)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_BROADPRIZE);
            pktSendData.strPktData = JsonConvert.SerializeObject(clsPrizeInfoPacket);
            pktSendData.strToken = "BroadCast";

            UserBroadCastPacket(JsonConvert.SerializeObject(pktSendData));
        }

        public static void BroadCastShowNotice()
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_SHOW_NOTICE);
            pktSendData.strPktData = string.Empty;
            pktSendData.strToken = "BroadCast";

            UserBroadCastPacket(JsonConvert.SerializeObject(pktSendData));
        }

        public static void CalcRobotJackPot()
        {
            List<CRobot> lstRobot = CGlobal.GetRobotList().FindAll(value => value.m_nAutoJack == 1 && value.m_nGearCode > 0 && value.m_nRbLogin == 1 && value.m_nJackpot == 0);
            if (lstRobot.Count == 0)
                return;

            int nAdd = 10 * lstRobot.Count;
            CalcShowJackPot(nAdd, 1);

            if (_nGrandRJack == 0)
            {
                _nGrandRJack = CGlobal._RND.Next(100, 201) * 10000;
                _nGrandRRaise = CGlobal._RND.Next(100, 201) * 10000;
            }

            if (_nMajorRJack == 0)
            {
                _nMajorRJack = CGlobal._RND.Next(50, 101) * 10000;
                _nMajorRRaise = CGlobal._RND.Next(50, 101) * 10000;
            }

            if (_nMinorRJack == 0)
            {
                _nMinorRJack = CGlobal._RND.Next(20, 51) * 10000;
                _nMinorRRaise = CGlobal._RND.Next(20, 51) * 10000;
            }

            if (_nMiniRJack == 0)
            {
                _nMiniRJack = CGlobal._RND.Next(5, 10) * 10000;
                _nMiniRRaise = CGlobal._RND.Next(5, 10) * 10000;
            }


            CRobot robot = lstRobot[CGlobal._RND.Next(lstRobot.Count)];
            CBaseGear gear = CGlobal.GetGearByCode(robot.m_nGearCode);

            int nJackCnt = CGlobal.GetRobotList().Count(value => value.m_nJackpot == 1);

            if (_nGrandRRaise <= _clsRJacPot.m_nGrand && nJackCnt < 4)
            {
                //로봇그랜드잭팟발생
                _clsRJacPot.m_nGrand = 0;
                gear.RaiseJackPotR(_nGrandRJack);
                _nGrandRJack = 0;
                _nGrandRRaise = 0;
            }


            if (_nMajorRRaise <= _clsRJacPot.m_nMajor && nJackCnt < 4)
            {
                //로봇메이저잭팟발생
                _clsRJacPot.m_nMajor = 0;
                gear.RaiseJackPotR(_nMajorRJack);
                _nMajorRJack = 0;
                _nMajorRRaise = 0;
            }

            if (_nMinorRRaise <= _clsRJacPot.m_nMinor)
            {
                //로봇미너잭팟발생
                _clsRJacPot.m_nMinor = 0;
                if (nJackCnt < 4)
                    gear.RaiseJackPotR(_nMinorRJack);
                _nMinorRJack = 0;
                _nMinorRRaise = 0;
            }

            if (_nMiniRRaise <= _clsRJacPot.m_nMini)
            {
                //로봇미니잭팟발생
                _clsRJacPot.m_nMini = 0;
                if (nJackCnt < 4)
                    gear.RaiseJackPotR(_nMiniRJack);
                _nMiniRJack = 0;
                _nMiniRRaise = 0;
            }
        }

        public static void CalcShowJackPot(int nCash, int nRobot)
        {
            int nMini = (CGlobal._RND.Next(3, 10) * nCash / 100);
            int nMinor = (CGlobal._RND.Next(1, 7) * nCash / 100);
            int nMajor = (CGlobal._RND.Next(5, 13) * nCash / 100);
            int nGrand = (CGlobal._RND.Next(3, 8) * nCash / 100);

            lock (_clsJacPot)
            {
                _clsJacPot.m_nMini += nMini;
                _clsJacPot.m_nMini = Math.Min(_clsJacPot.m_nMini, 9 * 10000);
                _clsJacPot.m_nMini = Math.Max(_clsJacPot.m_nMini, 1 * 10000);

                _clsJacPot.m_nMinor += nMinor;
                _clsJacPot.m_nMinor = Math.Min(_clsJacPot.m_nMinor, 70 * 10000);
                _clsJacPot.m_nMinor = Math.Max(_clsJacPot.m_nMinor, 10 * 10000);

                _clsJacPot.m_nMajor += nMajor;
                _clsJacPot.m_nMajor = Math.Min(_clsJacPot.m_nMajor, 500 * 10000);
                _clsJacPot.m_nMajor = Math.Max(_clsJacPot.m_nMajor, 100 * 10000);

                _clsJacPot.m_nGrand += nGrand;
                _clsJacPot.m_nGrand = Math.Min(_clsJacPot.m_nGrand, 3000 * 10000);
                _clsJacPot.m_nGrand = Math.Max(_clsJacPot.m_nGrand, 1000 * 10000);
            }

            if (nRobot == 1)
            {
                lock (_clsRJacPot)
                {
                    _clsRJacPot.m_nMini += (nMini * 2);
                    _clsRJacPot.m_nMini = Math.Min(_clsRJacPot.m_nMini, 5000000);

                    _clsRJacPot.m_nMinor += nMinor;
                    _clsRJacPot.m_nMinor = Math.Min(_clsRJacPot.m_nMinor, 5000000);

                    _clsRJacPot.m_nMajor += nMajor;
                    _clsRJacPot.m_nMajor = Math.Min(_clsRJacPot.m_nMajor, 5000000);

                    _clsRJacPot.m_nGrand += nGrand;
                    _clsRJacPot.m_nGrand = Math.Min(_clsRJacPot.m_nGrand, 5000000);
                }
            }
        }


        public static void CalculateJackPotCash(int nCash, int nTy = 0)
        {
            if(nTy == 0)
            {
                if (nCash >= 500000)
                {
                    _clsJacPot.m_nGrand = Math.Max(_clsJacPot.m_nGrand - nCash, 0);
                }
                else if (nCash >= 300000)
                {
                    _clsJacPot.m_nMajor = Math.Max(_clsJacPot.m_nMajor - nCash, 0);
                }
                else if (nCash >= 100000)
                {
                    _clsJacPot.m_nMinor = Math.Max(_clsJacPot.m_nMinor - nCash, 0);
                }
                else
                {
                    _clsJacPot.m_nMini = Math.Max(_clsJacPot.m_nMini - nCash, 0);
                }
            }
            else
            {
                if (nTy == 4)
                {
                    _clsJacPot.m_nGrand = Math.Max(_clsJacPot.m_nGrand - nCash, 1000 * 10000);
                }
                else if (nTy == 3)
                {
                    _clsJacPot.m_nMajor = Math.Max(_clsJacPot.m_nMajor - nCash, 100 * 10000);
                }
                else if (nTy == 2)
                {
                    _clsJacPot.m_nMinor = Math.Max(_clsJacPot.m_nMinor - nCash, 10 * 10000);
                }
                else if (nTy == 1)
                {
                    _clsJacPot.m_nMini = Math.Max(_clsJacPot.m_nMini - nCash, 1 * 10000);
                }
            }
        }

        public static int EnableAbsent(CUser clsUser)
        {
            DateTime dtTime = CMyTime.GetMyTime();
            int nDay = dtTime.Day;

            if (clsUser.m_nGearAppendCash < _clsAbsent.m_nPreUseCash)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public static void AbsentCheck(CUserSocket clsSocket)
        {
            CUser clsUser = clsSocket.GetUserObject();
            DateTime dtTime = CMyTime.GetMyTime();
            int nDay = dtTime.Day;
            if (clsUser.m_nGearAppendCash < _clsAbsent.m_nPreUseCash)
            {
                return;
            }
            if (clsUser.m_lstAsent[nDay - 1] == 2)
                return;
            clsUser.m_lstAsent[nDay - 1] = 2;
            clsUser.m_nAbsentCheck = 1;
            clsUser.m_nAbsentCnt++;

            if (clsUser.m_nAbsentCnt >= _clsAbsent.m_nCheckCount)
            {
                clsUser.m_nAbsentCnt = 0;
                clsSocket.SendAbsentComplete(_clsAbsent.m_nGiveCupn);
            }
            CDataBase.SaveAbsentCheck(clsUser.m_nUserCode, nDay, 2);
            CDataBase.SaveUserInfoToDB(clsUser);

            clsSocket.SendAbsentCheck();
        }

        private static void ClearAbsentInfo()
        {
            DateTime dtTime = CMyTime.GetMyTime();

            for (int i = 0; i < _lstUser.Count; i++)
            {
                _lstUser[i].m_nGearAppendCash = 0;

                if (_lstUser[i].m_nAbsentCheck != 1)
                {
                    _lstUser[i].m_nAbsentCnt = 0;
                    CDataBase.ClearAsentCount(_lstUser[i].m_nUserCode);
                }
                
                for (int j = 0; j < 31; j++)
                {
                    if (dtTime.Day == 1)
                    {
                        //한달에 한번 출석자료를 초기화한다.
                        _lstUser[i].m_lstAsent[j] = 0;
                    }
                    else
                    {
                        if(_lstUser[i].m_lstAsent[j] == 1)
                            _lstUser[i].m_lstAsent[j] = 0;
                    }
                }

                _lstUser[i].m_nGearAppendCash = 0;
            }

            if (dtTime.Day == 1)
            {
                CDataBase.ClearAsent();
            }
        }

        public static void ClearUserVirtualInfo(CUser user)
        {
            user.m_nVirtualCash = 0;
            user.m_nVirtualCupn = 0;

            while (user.m_lstGear.Count > 0)
            {
                CBaseGear gear = user.m_lstGear[0];
                user.m_lstGear.Remove(gear);
                CGlobal.SendClientReleaseGear(gear);
                gear.m_nGearRun = 0;
                gear.m_nSlotCash = 0;
                gear.m_nGiftCash = 0;
                gear.m_nGearState = 0;
                gear.m_nTakeUser = 0;
                gear.m_nTakeRobot = 0;
                CDataBase.SaveGearInfoToDB(gear);
            }
        }

        private static void ModifyYmatGearJackCpunt()
        {
            List<CBaseGear> lstGear = _lstGear.FindAll(value => value.m_nGameCode == CDefine.GAME_YMT);

            foreach(CBaseGear clsGear in lstGear)
            {
                clsGear.m_nMiniCount = clsGear.m_nMinorCount;
                clsGear.m_nMinorCount = clsGear.m_nMajorCount;
                clsGear.m_nMajorCount = 0;

                CDataBase.SaveGearInfoToDB(clsGear);
            }
        }
    }
}
