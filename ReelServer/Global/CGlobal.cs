using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ReelServer
{
    public static partial class CGlobal
    {
        public static int _nUserSocketIndex = 1;                    //소켓분산처리를 위한 인덱스
        public static Random _RND = new Random();
        public static bool _bLog = false;

        private static List<CAdmin> _lstAdmin;                  //관리자리스트
        private static List<CUser> _lstUser;                    //유저리스트
        private static List<CBaseGear> _lstGear;                //기어리스트
        private static List<CItemModel> _lstItemPrice;          //아이템가격리스트
        private static List<CItem> _lstUserItem;                //현재유저들이 보유하고 있는 아이템
        private static List<CRobot> _lstRobot;                  //로보트리스트
        private static List<CAgent> _lstAgent;                  //총판리스트
        private static List<CStore> _lstStore;                  //매장리스트
        private static CSubAdmin _clsSubAdmin;                  //본사
        private static List<CSubAdmin0> _lstSubAdmin0;          //영본사리스트
        private static List<CSubAdmin1> _lstSubAdmin1;          //부본사리스트
        private static List<CSubAdmin2> _lstSubAdmin2;          //총판리스트(왕빠, 웹버전)

        private static List<CJackNameModel> _lstJackName;       //잭팟이름리스트
        private static List<string> _lstBlockIP;                //아이피차단목록
        
        private static List<CUserSocket> _lstUserBroadCastSocket;  //유저 브로드카스트소켓
        private static List<CUserSocket> _lstUserSocket;        //접속한 클라이언트소켓객체 0-은 실시간처리를 위한 소켓
        private static List<CAdminSocket> _lstAdminSocket;      //접속한 채팅관리자 소켓객체 0-은 브로드카스트를 위한 소켓
        private static List<CAdminSocket> _lstAdminPageSocket;  //관리자페지소켓
        private static List<CAdminSocket> _lstStorePageSocket;  //총판페지페지소켓

        private static int _nNoticeImage;                       //로그인할때 공지사항이미지를 보여주겟는가
        private static string _strNotice;                       //공지사항
        private static string _strNoticeTitle;                  //공지사항제목
        private static List<CAcidModel> _lstAcid;               //충전계좌
        public static List<string> _lstNoticeHistory;           //시스템채팅현시내역(로그인시 이미 진행중인 시스템채팅내역을 현시하기 위해 보관)
        private static CBonusVirtual _clsBonusVirtual;          //첫회원가입시, 첫충시 이벤트설정 클라스
        private static CAbsent _clsAbsent;                      //출석부
        private static List<CAutoExcharge> _lstAutoExcharge;    //로봇자동출금내역을 가상적으로 생성하기 위한 리스트

        public static List<CGameEngine> _lstGameEngine;         //게임별엔진객체
        private static CItemEngine _clsItemEngine;              //아이템관리엔진


        public static CJackPot _clsJacPot;                      //시스템잭팟금액
        public static CJackPot _clsRJacPot;                     //자동로봇시스템잭팟금액

        //로봇자동잭팟관리를 위한 변수
        public static int _nGrandRJack = 0;
        public static int _nMajorRJack = 0;
        public static int _nMinorRJack = 0;
        public static int _nMiniRJack = 0;

        public static int _nGrandRRaise = 0;
        public static int _nMajorRRaise = 0;
        public static int _nMinorRRaise = 0;
        public static int _nMiniRRaise = 0;

        public static void OnStartGameServer()
        {
            _lstGameEngine = new List<CGameEngine>();
            _lstAdmin = new List<CAdmin>();
            _lstUser = new List<CUser>();
            _lstRobot = new List<CRobot>();
            _lstGear = new List<CBaseGear>();
            _lstItemPrice = new List<CItemModel>();
            _lstUserItem = new List<CItem>();
            _lstAgent = new List<CAgent>();
            _clsSubAdmin = new CSubAdmin();
            _lstSubAdmin0 = new List<CSubAdmin0>();
            _lstSubAdmin1 = new List<CSubAdmin1>();
            _lstSubAdmin2 = new List<CSubAdmin2>();
            _lstStore = new List<CStore>();
            _lstJackName = new List<CJackNameModel>();
            _lstBlockIP = new List<string>();
            _lstAcid = new List<CAcidModel>();

            _lstUserBroadCastSocket = new List<CUserSocket>();
            _lstUserSocket = new List<CUserSocket>();
            _lstAdminSocket = new List<CAdminSocket>();
            _lstAdminPageSocket = new List<CAdminSocket>();
            _lstStorePageSocket = new List<CAdminSocket>();
            _lstNoticeHistory = new List<string>();
            _lstAutoExcharge = new List<CAutoExcharge>();
            _clsBonusVirtual = new CBonusVirtual();
            _clsAbsent = new CAbsent();
            _clsJacPot = new CJackPot();
            _clsRJacPot = new CJackPot();
            _clsItemEngine = new CItemEngine();

            CDataBase.GetGameListFromDB(_lstGameEngine);
            CDataBase.GetOnerListFromDB(_lstAdmin);
            CDataBase.GetAgentListFromDB(_lstAgent);
            CDataBase.GetSubAdminFromDB(_clsSubAdmin);
            CDataBase.GetSubAdmin0ListFromDB(_lstSubAdmin0);
            CDataBase.GetSubAdmin1ListFromDB(_lstSubAdmin1);
            CDataBase.GetSubAdmin2ListFromDB(_lstSubAdmin2);
            CDataBase.GetStoreListFromDB(_lstStore);
            CDataBase.GetUserListFromDB(_lstUser);
            CDataBase.GetJackNameListFromDB(_lstJackName);
            CDataBase.GetGearListFromDB(_lstGear);
            CDataBase.GetItemPriceListFromDB(_lstItemPrice);
            CDataBase.GetitemListFromDB(_lstUserItem);
            CDataBase.GetRobotListFromDB(_lstRobot);
            CDataBase.GetAbsentFromDB(_clsAbsent);
            CDataBase.GetBonusVirtualFromDB(_clsBonusVirtual);
            CDataBase.CheckDateCacheFromDB();
            CDataBase.GetBlockIPListFromDB(_lstBlockIP);
            CDataBase.GetItemEngineFromDB(_clsItemEngine);
            _nNoticeImage = CDataBase.GetNoticeImage();
            _strNotice = CDataBase.GetNoticeFromDB();
            _strNoticeTitle = CDataBase.GetNoticeTitleFromDB();
            CDataBase.GetAcidFromDB(_lstAcid);
            SetVirtualGearValue();

            foreach (CGameEngine clsGame in _lstGameEngine)
                clsGame.ResetTotalJackCash();

            
        }

        public static void AddBroadCastSocket(CUserSocket clsUserSocket)
        {
            _lstUserBroadCastSocket.Add(clsUserSocket);
        }

        public static void RemoveBroadCastSocket(CUserSocket clsUserSocket)
        {
            _lstUserBroadCastSocket.Remove(clsUserSocket);
        }

        

        public static void AddUserSocket(CUserSocket clsUserSocket)
        {
            _lstUserSocket.Add(clsUserSocket);
        }

        public static void SendUserInfoToChatAdmin(CUser user)
        {
            if (_lstAdminSocket.Count > 0)
                _lstAdminSocket[0].SendUserInfo(user);
        }

        public static void InsertAdminSocket(CAdminSocket clsAdminSocket)
        {
            _lstAdminSocket.Insert(0, clsAdminSocket);
        }

        public static void AddAdminPageSocket(CAdminSocket clsSocket)
        {
            if(!_lstAdminPageSocket.Exists(value=>value.m_clsAdmin.m_nOnerCode == clsSocket.m_clsAdmin.m_nOnerCode))
                _lstAdminPageSocket.Add(clsSocket);
        }

        public static void AddStorePageSocket(CAdminSocket clsSocket)
        {
            if (!_lstStorePageSocket.Exists(value => value.m_clsStore.m_nStoreCode == clsSocket.m_clsStore.m_nStoreCode))
                _lstStorePageSocket.Add(clsSocket);
        }

        public static List<CAdminSocket> GetAdminPageSocketList()
        {
            return _lstAdminPageSocket;
        }

        public static List<CAdminSocket> GetStorePageSocketList()
        {
            return _lstStorePageSocket;
        }

        public static void RemoveUserSocket(CUserSocket clsUserSocket)
        {
            _lstUserSocket.Remove(clsUserSocket);
        }


        public static void RemoveAdminSocket(CAdminSocket clsAdminSocket)
        {
            _lstAdminSocket.Remove(clsAdminSocket);
        }

        public static void RemoveAdminPageSocket(CAdminSocket clsAdminSocket)
        {
            _lstAdminPageSocket.Remove(clsAdminSocket);
        }

        public static void RemoveStorePageSocket(CAdminSocket clsStoreSocket)
        {
            _lstStorePageSocket.Remove(clsStoreSocket);
        }


        public static T RandomSelect<T>(List<T> lst)
        {
            if (lst.Count == 0 || lst == null)
                return default(T);

            int nIndex = _RND.Next(lst.Count);
            return lst[nIndex];
        }

        //강제적으로 기대에 로봇을 넣는 함수
        public static void InsertRobotToGear(CBaseGear clsGear)
        {
            //기대를 선택하지 않은 로봇리스트를 얻는다.
            List<CRobot> lstRobot = _lstRobot.FindAll(value => value.m_nGearCode == 0);
            if (lstRobot.Count == 0)
                return;

            CRobot clsRobot = lstRobot[_RND.Next(lstRobot.Count)];
            clsGear.m_nTakeRobot = clsRobot.m_nRbCode;
            clsGear.m_nGearState = 1;
            clsGear.m_nGearRun = 1;
            clsRobot.m_nRbLogin = 1;
            clsRobot.m_nGearCode = clsGear.m_nGearCode;
            clsRobot.m_nGameCode = clsGear.m_nGameCode;

            CDataBase.SaveGearInfoToDB(clsGear);
            CDataBase.SaveRobotInfoToDB(clsRobot);

            Thread.Sleep(_RND.Next(5, 11) * 60 * 1000);
            int nPrizeCash = _RND.Next((clsGear.m_nAccuCash / 10000 - 18), (clsGear.m_nAccuCash / 10000 - 5)) * 10000;
            clsGear.m_nAccuCash -= nPrizeCash;
            Thread.Sleep(_RND.Next(30, 40) * 1000);

            clsGear.m_nTakeRobot = 0;
            clsGear.m_nGearState = 0;
            clsGear.m_nGearRun = 0;
            clsRobot.m_nRbLogin = 1;
            clsRobot.m_nGearCode = 0;
            clsRobot.m_nGameCode = 0;

            CDataBase.SaveGearInfoToDB(clsGear);
            CDataBase.SaveRobotInfoToDB(clsRobot);
        }

        public static void SendUserInfoToAdmin(CUser clsUser)
        {
            if(_lstAdminSocket.Count > 0)
                _lstAdminSocket[0].SendUserInfo(clsUser);
        }


        public static List<CUser> GetUserList()
        {
            return _lstUser;
        }

        public static CUser GetUserByToken(string strToken)
        {
            CUser clsUser = _lstUser.Find(value => value.m_strToken == strToken);
            return clsUser;
        }

        public static List<CUserSocket> GetUserSocketList()
        {
            return _lstUserSocket;
        }

        public static CUser GetUserByCode(int nUserCode)
        {
            CUser clsUser = _lstUser.Find(value => value.m_nUserCode == nUserCode);
            return clsUser;
        }

        public static CUser GetUserByID(string strUserID)
        {
            CUser clsUser = _lstUser.Find(value=>value.m_strUserID == strUserID);
            return clsUser;
        }

        public static CUser GetUserByNick(string strUserNick)
        {
            CUser clsUser = _lstUser.Find(value=>value.m_strUserNick == strUserNick);
            return clsUser;
        }

        public static CAgent GetAgentByCode(int nAgenCode)
        {
            CAgent clsAgent = _lstAgent.Find(value => value.m_nAgenCode == nAgenCode);
            return clsAgent;
        }

        public static CAgent GetAgentByMark(string strMark)
        {
            CAgent clsAgent = _lstAgent.Find(value=>value.m_strAgenMark == strMark);
            return clsAgent;
        }


        public static string GetUserNickByCode(int nUserCode)
        {
            CUser clsUser = GetUserByCode(nUserCode);
            if (clsUser == null)
                return string.Empty;
            else
                return clsUser.m_strUserNick;
        }


        public static CRobot GetRobotByCode(int nRobotCode)
        {
            CRobot clsRobot = _lstRobot.Find(value => value.m_nRbCode == nRobotCode);
            return clsRobot;
        }

        public static string GetRobotNickByCode(int nRobotCode)
        {
            CRobot clsRobot = GetRobotByCode(nRobotCode);
            if (clsRobot == null)
                return string.Empty;
            else
                return clsRobot.m_strRbNick;
        }

        public static CAdmin GetAdminByOnerID(string strID)
        {
            CAdmin admin = _lstAdmin.Find(value=>value.m_strOnerID == strID);
            return admin;
        }

        public static CAdmin GetAdminByCode(int nOnerCodde)
        {
            CAdmin clsAdmin = _lstAdmin.Find(value => value.m_nOnerCode == nOnerCodde);
            return clsAdmin;
        }

        public static string GetJackNameByGameCode_JackCont(int nGameCode, int nJackCont)
        {
            string strJackName = string.Empty;
            CJackNameModel jackName = _lstJackName.Find(value => value.m_nGameCode == nGameCode && value.m_nJackCont == nJackCont);
            if(jackName != null)
            {
                strJackName = jackName.m_strJackName;
            }

            return strJackName;
        }

        public static string GetOnerNickByCode(int nOnerCode)
        {
            CAdmin clsAdmin = GetAdminByCode(nOnerCode);
            if (clsAdmin == null)
                return string.Empty;
            else
                return clsAdmin.m_strOnerNick;
        }

        public static List<CBaseGear> GetGearListByGameCode(int nGameCode)
        {
            List<CBaseGear> lstGear = _lstGear.FindAll(value=>value.m_nGameCode == nGameCode);

            return lstGear;
        }

        public static List<CBaseGear> GetGearList()
        {
            return _lstGear;
        }


        public static string GetGameNameByGameCode(int nGameCode)
        {
            return CDefine.STR_GAMENAME[nGameCode];
        }

        public static CAdmin GetAdminByAdminID(string strAdminID)
        {
            CAdmin clsAdmin = _lstAdmin.Find(value => value.m_strOnerID == strAdminID);
            return clsAdmin;
        }

        public static List<CRobot> GetRobotList()
        {
            return _lstRobot;
        }


        public static CBaseGear GetGearByCode(int nGearCode)
        {
            CBaseGear clsGear = _lstGear.Find(value => value.m_nGearCode == nGearCode);
            return clsGear;
        }

        public static CBaseGear GetGearByGameCodeGearNum(int nGameCode, int nGearNum)
        {
            CBaseGear clsGear = _lstGear.Find(value => value.m_nGameCode == nGameCode && value.m_nGearNum == nGearNum);
            return clsGear;
        }

        //기계코드로부터 기어명(게임이름+기계인덱스) 얻기
        public static string GetGearNickByGearCode(int m_nGearCode)
        {
            CBaseGear clsGear = _lstGear.Find(gear => gear != null && gear.m_nGearCode == m_nGearCode);
            string strGameName = CDefine.STR_GAMENAME[clsGear.m_nGameCode];

            return strGameName + clsGear.m_nGearNum;
        }

        public static int GetGameRate(int nGameCode)
        {
            return _lstGameEngine.Find(value => value.m_nGameCode == nGameCode).m_nGameRate;
        }

        public static int GetGearNumByCode(int nGearCode)
        {
            CBaseGear clsGear = _lstGear.Find(value=>value.m_nGearCode == nGearCode);
            if (clsGear == null)
                return 0;
            else
                return clsGear.m_nGearNum;
        }

        public static CGameEngine GetGameEngineByCode(int nGameCode)
        {
            CGameEngine clsGameEngine = _lstGameEngine.Find(value => value.m_nGameCode == nGameCode);
            return clsGameEngine;
        }


        public static void SetNotice(string strTitle, string strNotice)
        {
            _strNoticeTitle = strTitle;
            _strNotice = strNotice;
            CDataBase.UpdateNoticeToDB(strTitle, strNotice);
        }

        public static string GetNotice()
        {
            string strValue = string.Empty;
            if (_strNoticeTitle.Trim() != string.Empty || _strNoticeTitle.Trim() != "")
            {
                strValue = _strNoticeTitle;
                if (_strNotice != string.Empty)
                    strValue += "\n" +  _strNotice;
            }
            else if (_strNotice.Trim() != string.Empty || _strNotice.Trim() != "")
                strValue = _strNotice;


            return strValue;
        }

        public static CGameEngine GetEngine(int nGameCode)
        {
            return _lstGameEngine.Find(value => value.m_nGameCode == nGameCode);
        }


        public static void RemoveUser(CUser user)
        {
            user.m_nUserState = 3;
            SendUserInfoToAdmin(user);
            _lstUser.Remove(user);
        }

        public static void AddUser(CUser user)
        {
            _lstUser.Add(user);
        }

        public static CBonusVirtual GetBonusVirtual()
        {
            return _clsBonusVirtual;
        }

        public static void SetAbsent(CAbsent asent)
        {
            _clsAbsent = asent;
        }

        public static List<CAgent> GetAgentList()
        {
            return _lstAgent;
        }

        public static CTotalPot GetTotalPot(int nGameCode)
        {
            CTotalPot clsTotalPot = _lstGameEngine.Find(value => value.m_nGameCode == nGameCode).m_clsTotalPot;
            return clsTotalPot;
        }

        public static void RemoveRobotByCode(int nRbCode)
        {
            CRobot robot = GetRobotByCode(nRbCode);
            robot.m_nRbLogin = 0;

            if (robot != null)
            {
                SendRobotInfoToAdmin(robot);
                _lstRobot.Remove(robot);
            }

        }
        public static void AddRobot(CRobot robot)
        {
            _lstRobot.Add(robot);
        }

        public static int Random(int nMin, int nMax)
        {
            return _RND.Next(nMin, nMax);
        }

        //JackPot정보얻기
        public static CJackPot GetJackPot()
        {
            return _clsJacPot;
        }

        public static void SetJackPot(int nIndex, int nCash)
        {
            if (nIndex == 0)
                _clsJacPot.m_nGrand = nCash;
            else if (nIndex == 1)
                _clsJacPot.m_nMajor = nCash;
            else if (nIndex == 2)
                _clsJacPot.m_nMinor = nCash;
            else if (nIndex == 3)
                _clsJacPot.m_nMini = nCash;
        }

        public static int GetLevelByUserCode(int nUserCode)
        {
            CUser clsUser = GetUserByCode(nUserCode);
            
            CAgent clsAgent = GetAgentByCode(clsUser.m_nAgenCode);
            if(clsAgent != null)
            {
                return clsAgent.m_nAgenLevel;
            }
            else
            {
                return 1;
            }
        }

        public static int GetAbsentGiveCupn()
        {
            return _clsAbsent.m_nGiveCupn;
        }

        public static CStore GetStoreByCode(int nStoreCode)
        {
            CStore clsStore = _lstStore.Find(value=>value.m_nStoreCode == nStoreCode);
            return clsStore;
        }

        public static string GetStoreNickByCode(int nStoreCode)
        {
            CStore clsStore = GetStoreByCode(nStoreCode);
            if (clsStore == null)
                return string.Empty;
            else
                return clsStore.m_strStoreNick;
        }

        public static CStore GetStoreByID(string strID)
        {
            CStore clsStore = _lstStore.Find(value => value.m_strStoreID == strID);
            return clsStore;
        }

        public static CStore GetStoreByNick(string strNick)
        {
            CStore clsStore = _lstStore.Find(value => value.m_strStoreNick == strNick);
            return clsStore;
        }

        public static List<CStore> GetStoreList()
        {
            return _lstStore;
        }

        public static void WriteLog()
        {
            if (CGlobal._bLog == false)
                return;
            StackTrace st = new StackTrace(true);
            if(st.FrameCount > 1)
            {
                StackFrame sf = st.GetFrame(1);
                string str = "File: " + sf.GetFileName() + ", Line: " + sf.GetFileLineNumber() + ", Method: " + sf.GetMethod();
                CGlobal.Log(str);
            }
        }

        public static void SetUseTempExcharge(int nValue)
        {
            _clsBonusVirtual.m_nAutoExcharge = nValue;
            if (nValue == 0)
                _lstAutoExcharge.Clear();
            else
                MakeVirtualExcargeData();
        }

        public static CUserSocket GetUserSocketByUserCode(int nUserCode)
        {
            CUserSocket clsSocket = _lstUserSocket.Find(value => value.GetUserObject() != null && value.GetUserObject().m_nUserCode == nUserCode);
            return clsSocket;
        }

        public static void Log(string strLog)
        {
            Console.WriteLine("#" + CMyTime.GetMyTimeStr() + "#  " + strLog);
        }

        public static bool CheckBlockIP(string strIP)
        {
            return _lstBlockIP.Exists(value => value == strIP);
        }

        public static void AddBlockIP(string strIP)
        {
            _lstBlockIP.Add(strIP);
        }

        public static void RemoveBlockIP(string strIP)
        {
            _lstBlockIP.RemoveAll(value=>value == strIP);
        }

        public static void SetAcid(string strUserName, string strBankName, string strSendAcid, int nStoreCode)
        {
            string strAcid = strUserName + "," + strBankName + "," + strSendAcid;
            if (_lstAcid.Exists(value => value.m_nStoreCode == nStoreCode))
                _lstAcid.Find(value => value.m_nStoreCode == nStoreCode).m_strAcid = strAcid;
            else
            {
                CAcidModel cls = new CAcidModel();
                cls.m_nStoreCode = nStoreCode;
                cls.m_strAcid = strAcid;
                _lstAcid.Add(cls);
            }
        }

        public static string GetAcid(int nStoreCode)
        {
            CAcidModel cls = _lstAcid.Find(value => value.m_nStoreCode == nStoreCode);
            if(cls == null)
            {
                return "aaa,aaa,aaa";
            }
            else
            {
                return cls.m_strAcid;
            }
        }

        public static void SetShowNoticeImage(int nValue)
        {
            _nNoticeImage = nValue;
            CDataBase.SaveNoticeImageValue(nValue);
        } 

        public static int GetShowNoticeImage()
        {
            return _nNoticeImage;
        }

        public static CSubAdmin GetSubAdmin()
        {
            return _clsSubAdmin;
        }

        public static CSubAdmin0 GetSubAdmin0ByCode(int nCode)
        {
            return _lstSubAdmin0.Find(value => value.m_nCode == nCode);
        }

        public static CSubAdmin1 GetSubAdmin1ByCode(int nCode)
        {
            return _lstSubAdmin1.Find(value => value.m_nCode == nCode);
        }

        public static CSubAdmin2 GetSubAdmin2ByCode(int nCode)
        {
            return _lstSubAdmin2.Find(value => value.m_nCode == nCode);
        }

        public static List<CSubAdmin0> GetSubAdmin0List()
        {
            return _lstSubAdmin0;
        }

        public static List<CSubAdmin1> GetSubAdmin1List()
        {
            return _lstSubAdmin1;
        }

        public static List<CSubAdmin2> GetSubAdmin2List()
        {
            return _lstSubAdmin2;
        }

        public static List<CItemModel> GetItemModelList()
        {
            return _lstItemPrice;
        }

        public static CItemModel GetItemModelByCode(int nCode)
        {
            return _lstItemPrice.Find(value => value.m_nItemCode == nCode);
        }

        public static List<CItem> GetUserItemList()
        {
            return _lstUserItem;
        }

        public static List<CItem> GetUserItemListByUserCode(int nUserCode)
        {
            return _lstUserItem.FindAll(value => value.m_nUserCode == nUserCode);
        }

        public static void AddUserItemToKeepList(CItem item)
        {
            _lstUserItem.Add(item);
        }

        public static CItem GetUserItemByCode(int nItemCode)
        {
            return _lstUserItem.Find(value => value.m_nItemCode == nItemCode);
        }

        public static CItem GetUserItemByItemModel(int nUserCode, int nItemModel)
        {
            return _lstUserItem.Find(value => value.m_nUserCode == nUserCode && value.m_nItemModel == nItemModel);
        }

        public static void RemoveUserItem(CItem item)
        {
            _lstUserItem.Remove(item);
        }

        public static int UseItemByUser(CItem item)
        {
            return _clsItemEngine.UseItem(item);
        }

        public static void SetItemEngineRemCash(int nCash)
        {
            _clsItemEngine.SetRemCash(nCash);
        }

        public static void SetItemEngineJackCash(int nCode, int nCash)
        {
            _clsItemEngine.SetItemJackCash(nCode, nCash);
        }

        public static void SetItemEngineItemCash(int nCode, int nCash)
        {
            _clsItemEngine.SetItemUseCash(nCode, nCash);
        }

        public static void SetItemEngineRaiseCash(int nCode, int nCash)
        {
            _clsItemEngine.SetItemRaiseCash(nCode, nCash);
        }
    }
}
