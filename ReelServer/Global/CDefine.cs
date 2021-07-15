using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public static class CDefine
    {
        public static int SRV_ADMIN_SOCKET_PORT = 0;
        public static int SRV_AGENT_SOCKET_PORT = 0;
        public static List<int> SRV_USER_SOCKET_PORT = new List<int>();
        public static int SRV_ADMIN_PAGE_PORT = 0;


        //게임종류
        public const int GAME_UNKNOWN = 0x00;
        public const int GAME_JECHON = 0x01;                  //손오공
        public const int GAME_ALADIN = 0x02;                  //알라딘
        public const int GAME_DRAGON = 0x03;                  //드라곤
        public const int GAME_SEA = 0x04;                     //바다이야기
        public const int GAME_GDC = 0x05;                     //황금성
        public const int GAME_ALADIN2 = 0x06;                 //알라딘2
        public const int GAME_OCEAN = 0x07;                   //오션
        public const int GAME_NWD = 0x08;                     //신천지
        public const int GAME_YMT = 0x09;                     //야마토
        public const int GAME_DVC = 0x0A;                     //다빈치
        public const int GAME_WHT = 0x0B;                     //백경
        public const int GAME_YAN = 0x0C;                     //양귀비

        public static string[] STR_GAMENAME = { "", "손오공", "알라딘(100)", "5드래곤", "바다이야기", "황금성", "알라딘(200)", "오션", "신천지", "야마토", "다빈치", "백경", "양귀비" };

        //응답 반환값 
        public const int CTL_ERROR = 0x02;                    //조작오유
        public const int CTL_SUCCESS = 0x01;                  //조작성공
        public const int CTL_FAILED = 0x00;                   //조작실패


        //챠트관리자파케트
        public const int PKT_HEADER_O2S_CHAT_LOGIN = 0x3001;                     //챠트관리자로그인
        public const int PKT_HEADER_O2S_CHAT_USERLIST = 0x3002;                  //유저리스트
        public const int PKT_HEADER_O2S_CHAT_ROBOTLIST = 0x3003;                 //로봇리스트
        public const int PKT_HEADER_O2S_CHAT_GROUP = 0x3004;                     //그룹채팅 브로드카스트
        public const int PKT_HEADER_O2S_CHAT_SECURITY = 0x3005;                  //개별채팅 
        public const int PKT_HEADER_O2S_CHAT_USERINFO = 0x3006;                  //유저정보전송
        public const int PKT_HEADER_O2S_CHAT_ROBOTINFO = 0x3007;                 //로봇정보전송 
        public const int PKT_HEADER_O2S_CHAT_SERVERTIME = 0x3008;                //서버시간전송
        public const int PKT_HEADER_O2S_CHAT_BLOCK = 0x3009;                     //채팅블록전송

        //서버 - 관리자 파케트
        public const int PKT_HEADER_S2A_GLOBAL_REAL_BOARDCAST = 0x0001;          //1초에 한번 진행되는 실시간 브로드카스트파케트(관리자페지에로 전송)
        public const int PKT_HEADER_O2S_GLOBAL_USERLIST_REQUEST = 0x2011;        //유저리스트 요청(관리자 -> 서버)
        public const int PKT_HEADER_S2O_GLOBAL_USERLIST_RESPONE = 0x2011;        //유저리스트 응답(서버- > 관리자)
        public const int PKT_HEADER_O2S_GLOBAL_USERGEARLIST_REQUEST = 0x2012;    //선택한 유저의 기어리스트 요청(관리자 -> 서버)
        public const int PKT_HEADER_S2O_GLOBAL_USERGEARLIST_RESPONE = 0x2012;    //선택한 유저의 기어리스트 응답(서버 -> 관리자)
        public const int PKT_HEADER_O2S_GLOBAL_ANSWER_SEND = 0x2013;             //1:1문의답변전송
        public const int PKT_HEADER_S2O_GLOBAL_ANSWER_RCEV = 0x2013;             //1:1문의답변받기
        public const int PKT_HEADER_O2S_GLOBAL_CHAT_REQUEST = 0x2014;            //챠트보내기
        public const int PKT_HEADER_S2O_GLOBAL_CHAT_RESPONSE = 0x2014;           //챠트보내기결과
        public const int PKT_HEADER_O2S_GLOBAL_GEARLIST_REQUEST = 0x2015;        //챠트보내기
        public const int PKT_HEADER_S2O_GLOBAL_GEARLIST_RESPONSE = 0x2015;       //챠트보내기결과
        public const int PKT_HEADER_O2S_GLOBAL_ROBOT_CHAT_REQUEST = 0x2016;      //로봇챠트보내기
        public const int PKT_HEADER_S2O_GLOBAL_ROBOT_CHAT_RESPONSE = 0x2016;     //로봇챠트보내기결과
        public const int PKT_HEADER_O2S_GLOBAL_NOTICE_REQUEST = 0x2017;          //공지사항보내기
        public const int PKT_HEADER_O2S_GLOBAL_BROADCHAT_REQUEST = 0x2018;       //전체채닝보내기
        public const int PKT_HEADER_O2S_GLOBAL_AUTH_REQUEST = 0x2000;            //인증파케트
        public const int PKT_HEADER_S2O_GLOBAL_AUTH_RESPONE = 0x2000;            //인증파케트
        public const int PKT_HEADER_O2S_GLOBAL_STOREAUTH_REQUEST = 0x2007;       //매장인증파케트
        public const int PKT_HEADER_S2O_GLOBAL_STOREAUTH_RESPONE = 0x2007;       //매장인증파케트
        public const int PKT_HEADER_S2O_GLOBAL_GAMESTATE_RSPONSE = 0x2019;       //관리상태전송



        //서버 - 유저 파케트
        public const int PKT_HEADER_S2C_GLOBAL_REAL_BOARDCAST = 0x0000;          //1초에 한번 진행되는 실시간 브로드카스트파케트(유저페지에로 전송)

        public const int PKT_HEADER_GLOBAL_LOGIN = 0x1001;                      //로그인 요청(유저 -> 서버)
        public const int PKT_HEADER_GLOBAL_RECONNECT = 0x1007;                  //통신오유시 재접속파켓
        public const int PKT_HEADER_GLOBAL_REPID = 0x1002;                      //유저아이디증복확인
        public const int PKT_HEADER_GLOBAL_REPNICK = 0x1003;                    //유저닉네임증복확인
        public const int PKT_HEADER_GLOBAL_REGISTER = 0x1004;                   //회원가입
        public const int PKT_HEADER_GLOBAL_TAKEGEARLIST = 0x1005;               //유저가 선택하고 있는 기계정보
        public const int PKT_HEADER_C2S_GLOBAL_CHART = 0x1011;                  //채팅 요청(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_ACID = 0x1074;                   //계좌문의 요청(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_CHARGE = 0x1051;                 //충전 요청(유저 -> 서버)
        public const int PKT_HEADER_S2C_GLOBAL_USER_SEND = 0x1062;              //유저정보전송(서버 -> 유저)
        public const int PKT_HEADER_C2S_GLOBAL_EXCHARGE = 0x1052;               //환전 요청(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_EXCUPON = 0x1053;                //쿠폰전환 요청(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_EXCUPON_RATE = 0x1059;           //쿠폰비율
        public const int PKT_HEADER_C2S_GLOBAL_SECURITY_CHAT = 0x1073;          //관리자챠트요청
        public const int PKT_HEADER_C2S_GLOBAL_GEARPOSTLIST = 0x1076;           //기계예약리스트(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_GEARKEEPLIST = 0x1078;           //기계보유리스트(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_GEARPOST = 0x1081;               //기계예약(서버 -> 유저)
        public const int PKT_HEADER_C2S_GLOBAL_BROADPRIZE = 0x1009;             //잭팟당첨정보 방송
        public const int PKT_HEADER_C2S_GLOBAL_EXIT = 0x1099;                   //게임선택창으로(통합버전에서 첫화면으로 넘어갈때이다.)
        public const int PKT_HEADER_C2S_GLOBAL_LOGOUT = 0x1098;                 //게임탈퇴
        public const int PKT_HEADER_C2S_GLOBAL_ABSENT_LIST = 0x10A0;            //출석정보요청
        public const int PKT_HEADER_C2S_GLOBAL_ABSENT_CHECK = 0x10A1;           //출석체크요청
        public const int PKT_HEADER_C2S_GLOBAL_ABSENT_COMPLETE = 0x10A2;        //출석체크완성
        public const int PKT_HEADER_C2S_GLOBAL_ABSENT_COMSEND = 0x10A3;         //출석체크완성확인
        public const int PKT_HEADER_C2S_GLOBAL_JACKINFO_LIST = 0x1018;          //기계잭팟내역
        public const int PKT_HEADER_C2S_GLOBAL_GEAR_CHECK = 0x1019;             //기계예약
        public const int PKT_HEADER_C2S_GLOBAL_SHOW_NOTICE = 0x1017;            //공지사항이미지 보이기

        public const int PKT_HEADER_C2S_GLOBAL_ITEM_PRICE_LIST = 0x10B1;        //아이템가격리스트
        public const int PKT_HEADER_C2S_GLOBAL_ITEM_KEEP_LIST = 0x10B2;         //보유아이템리스트
        public const int PKT_HEADER_C2S_GLOBAL_ITEM_BUY = 0x10B3;               //아이템사기
        public const int PKT_HEADER_C2S_GLOBAL_ITEM_USE = 0x10B4;               //아이템사용
        public const int PKT_HEADER_C2S_GLOBAL_ITEM_GEAR = 0x10B5;              //아이템을 사용할 기계목록



        //Web 통신
        public const int PKT_HEADER_C2S_WEB_USERINFO = 0x2001;                  //토큰을 가지고 유저정보를 얻어간다.
        public const int PKT_HEADER_C2S_WEB_TAKEGEARLIST = 0x2005;              //유저가 선택하고 있는 기계정보

        public const int PKT_HEADER_C2S_GLOBAL_GEARLIST_REQUEST = 0x1031;        //기어리스트 요청(유저 -> 서버)
        public const int PKT_HEADER_S2C_GLOBAL_GEARLIST_RESPONE = 0x1031;        //기어리스트 응답(서버 -> 유저)
        public const int PKT_HEADER_C2S_GLOBAL_SELECTGEAR_REQUEST = 0x1032;      //기어선택 요청(유저 -> 서버)
        public const int PKT_HEADER_S2C_GLOBAL_SELECTGEAR_RESPONE = 0x1032;      //기어선택 응답(서버 -> 유저)
        public const int PKT_HEADER_C2S_GLOBAL_APPENDCASH_REQUEST = 0x1041;      //만원넣기 요청(유저 -> 서버)
        public const int PKT_HEADER_S2C_GLOBAL_APPENDCASH_RESPONE = 0x1041;      //만원넣기 응답(서버 -> 유저)
        public const int PKT_HEADER_S2C_GLOBAL_ONER_MESSAGE_SEND = 0x1069;       //관리자메세지전송
        public const int PKT_HEADER_S2C_GLOBAL_ACID_MESSAGE_SEND = 0x1070;       //계좌문의답변전송
        public const int PKT_HEADER_S2C_GLOBAL_CHARGE_MESSAGE_SEND = 0x1075;     //충전승인메세지 전송
        public const int PKT_HEADER_S2C_GLOBAL_ONER_CHAT_SEND = 0x1072;          //관리자챠트정송
        public const int PKT_HEADER_S2C_GLOBAL_CHART_RESPONE = 0x1011;           //챠트 응답(서버 -> 유저)
        public const int PKT_HEADER_S2C_GLOBAL_NOTICE_SEND = 0x1012;             //공지사항보내기(서버 -> 유저)
        public const int PKT_HEADER_S2C_GLOBAL_GEARINFO_SEND = 0x1082;           //기계정보전송(서버 -> 유저)
        public const int PKT_HEADER_S2C_GLOBAL_CLEAR_GEAR_SEND = 0x1067;         //기계금액초기화(서버 -> 유저)
        public const int PKT_HEADER_S2C_GLOBAL_RELEASE_GEAR_SEND = 0x1068;       //기계잠금해제(서버 -> 유저)
        public const int PKT_HEADER_C2S_GLOBAL_GEARSTART_REQUEST = 0x1065;       //게임시작(유저 -> 서버)
        public const int PKT_HEADER_C2S_GLOBAL_GEARSTOP_REQUEST = 0x1066;        //게임정지(유저 -> 서버)

        public const int PKT_HEADER_S2C_GLOBA_SCOREINFO_SEND = 0x1055;           //릴돌기 스핀정보송신(서버 -> 유저)
        public const int PKT_HEADER_S2C_GLOBAL_EXCUPN_SEND = 0x1057;             //상품권교환진행


        //바다이야기
        public const int PKT_HEADER_C2S_SEA_STARTSPIN_RECV = 0x401;              //바다이야기 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_SEA_COINCREATE_RECV = 0x403;             //바다이야기 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_SEA_SLOTADD_RECV = 0x404;                //바다이야기 슬롯점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_SEA_WATERTANK_RECV = 0x405;              //수조물이차는점수보내기(유저 -> 서버)

        //손오공
        public const int PKT_HEADER_C2S_SWK_STARTSPIN_RECV = 0x101;              //손오공 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_SWK_GIFTADD_RECV = 0x102;                //손오공 기프트점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_SWK_COINCREATE_RECV = 0x103;             //손오공 코인생성(유저 -> 서버)

        //알라딘
        public const int PKT_HEADER_C2S_ALD_STARTSPIN_RECV = 0x201;              //알라딘 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_ALD_GIFTADD_RECV = 0x202;                //알라딘 기프트점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_ALD_ENDANI_RECV = 0x203;                 //알라딘 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_ALD_SLOTADD_RECV = 0x204;                //알라딘 슬롯점수증가(유저 -> 서버)

        //알라딘2
        public const int PKT_HEADER_C2S_ALD2_STARTSPIN_RECV = 0x601;              //알라딘 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_ALD2_GIFTADD_RECV = 0x602;                //알라딘 기프트점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_ALD2_ENDANI_RECV = 0x603;                 //알라딘 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_ALD2_SLOTADD_RECV = 0x604;                //알라딘 슬롯점수증가(유저 -> 서버)

        //5드래곤
        public const int PKT_HEADER_C2S_FDG_STARTSPIN_RECV = 0x301;              //5드래곤 스핀시작받기(유저 -> 서버)
        public const int PKT_HEADER_C2S_FDG_GIFTADD_RECV = 0x303;                //5드래곤 기프트머니증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_FDG_DRG_INFO_RECV = 0x304;               //5드래곤 드래곤잭팟선택사항전송(유저->서버)
        public const int PKT_HEADER_C2S_FDG_EXMONEY_RECV = 0x305;                //5드래곤 머니전환(유저->서버)

        //황금성
        public const int PKT_HEADER_C2S_GDC_STARTSPIN_RECV = 0x501;              //황금성 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_GDC_GIFTADD_RECV = 0x502;                //황금성 기프트점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_GDC_COINCREATE_RECV = 0x503;             //황금성 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_GDC_JACHADD_RECV = 0x504;                //황금성 JachCash 증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_GDC_WINJACK_RECV = 0x505;                //황금성 조커X4 당첨(유저 -> 서버)

        //오션
        public const int PKT_HEADER_C2S_OCA_STARTSPIN_RECV = 0x701;               //오션 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_OCA_COINCREATE_RECV = 0x703;              //오션 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_OCA_GIFTADD_RECV = 0x704;                 //오션 슬롯점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_OCA_FINISHANI_RECV = 0x705;               //오션 잭팟애니매션 끝(유저 -> 서버)
        public const int PKT_HEADER_C2S_OCA_WATERTANK_RECV = 0x706;               //오션 수조점수증가(유저 -> 서버)

        //신천지
        public const int PKT_HEADER_C2S_NWD_STARTSPIN_RECV = 0x801;               //오션 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_NWD_COINCREATE_RECV = 0x803;              //오션 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_NWD_GIFTADD_RECV = 0x804;                 //오션 슬롯점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_NWD_FINISHANI_RECV = 0x805;               //오션 잭팟애니매션 끝(유저 -> 서버)
        public const int PKT_HEADER_C2S_NWD_WATERTANK_RECV = 0x806;               //신천지 수조점수증가(유저 -> 서버)

        //야마토
        public const int PKT_HEADER_C2S_YMT_STARTSPIN_RECV = 0x901;               //야마토 스핀시작수신(유저 -> 서버)
        public const int PKT_HEADER_C2S_YMT_GIFTADD_RECV = 0x902;                 //야마토 기프트점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_YMT_EXMONEY_RECV = 0x903;                 //야마토 머니전환(유저->서버)

        //다빈치
        public const int PKT_HEADER_C2S_DVC_STARTSPIN_RECV = 0xA01;             //다빈치 스핀시작송신(유저 -> 서버)
        public const int PKT_HEADER_C2S_DVC_COINCREATE_RECV = 0xA03;            //다빈치 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_DVC_GIFTADD_RECV = 0xA04;               //다빈치 슬롯점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_DVC_FINISHANI_RECV = 0xA05;             //다빈치 잭팟애니매션 끝(유저 -> 서버)

        //백경
        public const int PKT_HEADER_C2S_WHT_STARTSPIN_RECV = 0xB01;             //백경 스핀시작송신(유저 -> 서버)
        public const int PKT_HEADER_C2S_WHT_COINCREATE_RECV = 0xB03;            //백경 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_WHT_GIFTADD_RECV = 0xB04;               //백경 슬롯점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_WHT_FINISHANI_RECV = 0xB05;             //백경 잭팟애니매션 끝(유저 -> 서버)
        public const int PKT_HEADER_C2S_WHT_WATERTANK_RECV = 0xB06;             //백경 수조점수증가(유저 -> 서버)

        //양귀비
        public const int PKT_HEADER_C2S_YAN_STARTSPIN_RECV = 0xC01;             //양귀비 스핀시작송신(유저 -> 서버)
        public const int PKT_HEADER_C2S_YAN_COINCREATE_RECV = 0xC03;            //양귀비 코인생성(유저 -> 서버)
        public const int PKT_HEADER_C2S_YAN_GIFTADD_RECV = 0xC04;               //양귀비 슬롯점수증가(유저 -> 서버)
        public const int PKT_HEADER_C2S_YAN_FINISHANI_RECV = 0xC05;             //양귀비 잭팟애니매션 끝(유저 -> 서버)


        //오유메세지코드
        public const int ERROR_LOGIN_ALREADY = 0x01;
        public const int ERROR_LOGIN_AUTH = 0x02;
        public const int ERROR_LOGIN_BLOCK = 0x03;
        public const int ERROR_LOGIN_WAIT = 0x04;
        public const int ERROR_REGISTER_IDEMP = 0x05;
        public const int ERROR_REGISTER_NICKEMP = 0x06;
        public const int ERROR_REGISTER_PWEMP = 0x07;
        public const int ERROR_REGISTER_AGENTEMP = 0x08;
        public const int ERROR_REGISTER_IDFAILED = 0x09;
        public const int ERROR_REGISTER_NICKFAILED = 0x0A;
        public const int ERROR_REGISTER_IDSUCCESS = 0x0B;
        public const int ERROR_REGISTER_NICKSUCCESS = 0x0C;
        public const int ERROR_REGISTER_FAILED = 0x0D;
        public const int ERROR_REGISTER_SUCCESS = 0x0E;
        public const int ERROR_REGISTER_AGENTFAILED = 0x0F;
        public const int ERROR_CHARGE_BANKNAME = 0x10;
        public const int ERROR_CHARGE_ACID = 0x11;
        public const int ERROR_CHARGE_USERNAME = 0x12;
        public const int ERROR_CHARGE_PHONE = 0x13;
        public const int ERROR_CHARGE_CASH = 0x14;
        public const int ERROR_CHARGE_TIME = 0x15;
        public const int ERROR_EXCHARGE_UNIT = 0x16;
        public const int ERROR_EXCHARGE_BANKNAME = 0x17;
        public const int ERROR_EXCHARGE_ACID = 0x18;
        public const int ERROR_EXCHARGE_USERNAME = 0x19;
        public const int ERROR_EXCHARGE_PHONE = 0x1A;
        public const int ERROR_EXCHARGE_BANKCASH = 0x1B;
        public const int ERROR_EXCHARGE_DRAWZERO = 0x1C;
        public const int ERROR_EXCHARGE_DRAWOVER = 0x1D;
        public const int ERROR_EXCHARGE_AMOUNT = 0x1E;
        public const int ERROR_EXCHARGE_LIMIT = 0x1F;
        public const int ERROR_EXCHARGE_ROLLING = 0x20;
        public const int ERROR_EXCHARGE_VIRTUAL = 0x21;
    }   
}
