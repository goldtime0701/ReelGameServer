using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CGearModel
    {
        public int m_nGearCode;     //기어코드
        public int m_nGearNum;      //기어번호
        public int m_nSlotCash;     //슬롯머니
        public int m_nGiftCash;     //기프트머니
        public int m_nGearState;    //기어상태 0-준비중, 1-선택, 3-잠금, 4-고장
        public int m_nGearJack;     //잭팟상태 0-일반돌기, 1-잭팟중, 2-1차환수금액
        public int m_nTakeUser;     //사용중인 유저
        public int m_nOrderUser;    //예약중인 유저
        public int m_nAccuCash;     //기대에서 사용된 금액
        public int m_nJackCash;     //기대에서 준 잭팟금액
        public int m_nGameCode;     //게임코드
        public int m_nGearRun = 0;  //0-정지, 1-가동
        public int m_nTakeRobot;    //사용중인 로봇
        public int m_nGearCheck;    //기계예약
        public DateTime m_dtCheckTime; //예약시간
        public int m_nTopJackCash;  //최대잭팟금액
        public int m_nLastJackCash; //가장 최근 잭팟금액
        public DateTime m_dtLastJackTime; //가장 최근 잭팟시간
        public int m_nGrandCount;   //그랜드 출현 회수
        public int m_nMajorCount;   //메이져 출현 회수
        public int m_nMinorCount;   //미너 출현 회수
        public int m_nMiniCount;    //미니 출현 회수
        public int m_nGdcCash;      //황금성에서 사용되는 jachcash
        public List<int> m_nLstGraph;  //야마토에서돌기회수정보를 나타내는 값
        public int m_nYmtRound;     //야마토에서는 현재 회전수를 나타낸다.
        public int m_nLeftWater;    //왼쪽수조
        public int m_nRightWater;   //오른쪽수조

        public int m_nSpeedCash;     //스핀한번회전하는데 드는 금액


        public string m_strGame { get { return CGlobal.GetGameNameByGameCode(m_nGameCode); } }
        public string m_strTakeUser { get { return CGlobal.GetUserNickByCode(m_nTakeUser); } }
        public string m_strTakeRobot { get { return CGlobal.GetRobotNickByCode(m_nTakeRobot); } }
        public string m_strLastJackTime { get { return m_dtLastJackTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
    }

    public class CJackInfo
    {
        public List<CJackInfoModel> m_lstJackModel;
        public string m_strYmtInfo;
        public string m_strYmtGraph;
    }

    public class CJackInfoModel
    {
        public string m_strGameName;
        public string m_strJackCash;
        public string m_strJackDate;
        public string m_strJackName;
    }

    public class CJackNameModel
    {
        public int m_nGameCode;
        public int m_nJackCont;
        public string m_strJackName;
    }
}
