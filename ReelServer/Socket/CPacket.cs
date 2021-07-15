using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CPacket
    {
        public int nPktHeader;
        public string strPktData;
        public string strToken;
        public int nPktResult;                               // 처리 결과
        public string strPktMsg;

        public CPacket()
        {

        }

        public CPacket(int nHeader)
        {
            nPktHeader = nHeader;
        }
    }

    public class CUserInfoListS2APacket
    {
        public List<CUserModel> clsUserList;
        public int nTotalCnt;
        public int nNewCnt;
        public int nBlockCnt;
        public int nOnlineCnt;
    }

    public class CGameStateS2APacket
    {
        public int nUserTotalCnt;                   //총유저수
        public int nUserNewCnt;                     //신규회원수
        public int nUserBlockCnt;                   //차단회원수
        public int nUserOnlineCnt;                  //가입회원수
        public int nUserChargeCnt;                  //충전신청중
        public int nUserExchargeCnt;                //환전신청중
        public int nAgentExchargeCnt;               //총판정산신청
        public int nUnderExchargeCnt;               //하부정산신청
        public int nStoreAlCash;                    //매장알금액

    }


    public class CChatS2APacket
    {
        public string flag;
        public string strTime;
        public string strNick;
        public string strChat;
    }

    public class CSChatPacket                   //개별채팅
    {
        public int m_nOnerCode;                 //관리자코드
        public int m_nUserCode;                 //유저코드
        public int m_nType;                     //0-관리자->유저, 1-유저->관리자
        public string m_strTime;                //시간
        public string m_strMsg;                 //채팅내용
        public string m_strNick;
    }

    public class CUserChatBlock
    {
        public int m_nUserCode;
        public int m_nChatBlock;
        public int m_nChatBlockA;
    }

    public class CAbsentPacket
    {
        public List<int> m_lstAbsent;
    }

    public class CGChatPacket                    //그룹채팅
    {
        public int m_nCode;                     //유저코드(로봇코드, 관리자코드)
        public int m_nKind;                     //0-관리자, 1-유저, 2-로봇
        public string m_strTime;                //채팅시간    
        public string m_strMsg;                 //채팅내용
        public string m_strNick;
        public int m_nOnerCode;                 //로봇채팅일때만 사용
    }

    public class CScorePacket
    {
        public int nGearCode;
        public string strPktData;
    }

    public class CUserInfoC2SPacket
    {
        public string m_strUserID;
        public string m_strUserNick;
        public string m_strToken;
        public string m_strUserPW;
        public string m_strAgenCode;
        public string m_strPhone;
        public int m_nUserCash;
        public int m_nUserCupn;
        public int m_nAbsentCnt;
        public int m_nChatBlock;
        public int m_nLogin;
    }

    public class CLoginPacket
    {
        public string strMsg;
        public int nCode;
        public string strData;
    }

    public class CGearPostInfo
    {
        public int m_nGearCode;
        public int m_nGearNum;
        public int m_nGearCheck;
        public int m_nGameCode;
    }

    public class CGearKeepInfo
    {
        public int m_nGearCode;
        public int m_nGearNum;
        public int m_nGameCode;
        public int m_nGearPlay;
        public int m_nSlotCash;
    }

    public class CPrizeInfoBroadCast
    {
        public string m_strUserNick;
        public int m_nGameCode;
        public int m_nGearNum;
        public int m_nGearCode;
        public int m_nPrizeKind;
        public int m_nPrizeCash;
        public int m_nPrizeType;  //5드레곤일때는 장군잭팟의 Grand, Major, Minor, Mini 를 나타낸다
    }
}
