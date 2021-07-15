using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReelServer
{
    public partial class CUserSocket
    {
        private void SendPacket(CPacket pktSendData)
        {
            if (pktSendData.strToken != "BroadCast" && m_clsUser != null)
                pktSendData.strToken = m_clsUser.m_strToken;

            string strPacket = JsonConvert.SerializeObject(pktSendData);
            SendPacket(strPacket);
        }

        //관리자메세지전송
        public void SendAdminToUserMessage(string strMessage, int nAcid = 0)
        {
            if (nAcid == 0)  //일반문의 답변
            {
                CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_ONER_MESSAGE_SEND);
                
                pktSendData.strPktData = strMessage;
                SendPacket(pktSendData);
            }
            else if (nAcid == 1)  //계좌문의 답변
            {
                CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_ACID_MESSAGE_SEND);
                
                pktSendData.strPktData = strMessage;
                SendPacket(pktSendData);
            }
            else if (nAcid == 2) //충전신청승인 메세지
            {
                CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_CHARGE_MESSAGE_SEND);
                
                pktSendData.strPktData = strMessage;
                SendPacket(pktSendData);
            }
        }

        public void SendSecurityChat(CSChatPacket chatPacket)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_ONER_CHAT_SEND);
            
            pktSendData.strPktData = JsonConvert.SerializeObject(chatPacket);
            SendPacket(pktSendData);
            CGlobal.Log(chatPacket.m_strNick + "->" + chatPacket.m_strMsg);
        }

        public void SendNotice(string strNotice, bool bLog=false)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_NOTICE_SEND);
            pktSendData.strToken = "BroadCast";
            pktSendData.strPktData = strNotice;
            if(bLog == false)
            {
                CGlobal._lstNoticeHistory.Add(strNotice);
                if (CGlobal._lstNoticeHistory.Count > 20)
                    CGlobal._lstNoticeHistory.RemoveAt(0);
            }

            SendPacket(pktSendData);
        }

        public void SendScoreInfo(CScorePacket scoreInfo)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBA_SCOREINFO_SEND);
            
            pktSendData.strPktData = JsonConvert.SerializeObject(scoreInfo);

            SendPacket(pktSendData);
        }

        public void SendExcupn(int nGearCode, int nCupn)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_EXCUPN_SEND);
            pktSendData.strPktData = nGearCode + "," + nCupn;

            SendPacket(pktSendData);
        }


        public void SendGearInfo(CBaseGear clsGear)
        {
            if (clsGear == null)
                return;

            CGearModel clsGearModel = clsGear.GetGearModel();

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_GEARINFO_SEND);
            
            pktSendData.strPktData = JsonConvert.SerializeObject(clsGearModel);

            SendPacket(pktSendData);
        }

       

        public void SendClientClearGearCash(CBaseGear gear)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_CLEAR_GEAR_SEND);
            
            pktSendData.strPktData = gear.m_nGearCode.ToString();

            SendPacket(pktSendData);
        }

        public void SendOnerMessage(string strMessage, int nAcid = 0)
        {
            if (nAcid == 0)  //일반문의 답변
            {
                CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_ONER_MESSAGE_SEND);
                
                pktSendData.strPktData = strMessage;
                SendPacket(pktSendData);
            }
            else if (nAcid == 1)  //계좌문의 답변
            {
                CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_ACID_MESSAGE_SEND);
                
                pktSendData.strPktData = strMessage;
                SendPacket(pktSendData);
            }
            else if (nAcid == 2) //충전신청승인 메세지
            {
                CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_CHARGE_MESSAGE_SEND);
                
                pktSendData.strPktData = strMessage;
                SendPacket(pktSendData);
            }
        }

        public void SendClientReleaseGear(CBaseGear gear)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_RELEASE_GEAR_SEND);
            
            pktSendData.strPktData = gear.m_nGearCode.ToString();

            SendPacket(pktSendData);
        }

        //유저정보를 클라이어트에 보내는 함수
        public void SendUserInfo()
        {
            if (m_clsUser == null)
                return;

            CUserInfoC2SPacket userInfoPacket = new CUserInfoC2SPacket();
            userInfoPacket.m_strUserID = m_clsUser.m_strToken;
            userInfoPacket.m_strUserNick = m_clsUser.m_strUserNick;
            userInfoPacket.m_strToken = m_clsUser.m_strToken;
            userInfoPacket.m_strUserPW = m_clsUser.m_strUserPW;
            userInfoPacket.m_strAgenCode = m_clsUser.m_strAgenMark;
            userInfoPacket.m_strPhone = m_clsUser.m_strUserPhone;
            userInfoPacket.m_nUserCash = m_clsUser.m_nUserCash + m_clsUser.m_nVirtualCash;
            userInfoPacket.m_nUserCupn = m_clsUser.m_nUserCupn + m_clsUser.m_nVirtualCupn;
            userInfoPacket.m_nAbsentCnt = m_clsUser.m_nAbsentCnt;
            userInfoPacket.m_nChatBlock = m_clsUser.m_nChatBlock;
            userInfoPacket.m_nLogin = m_clsUser.m_nUserLogin;

            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_S2C_GLOBAL_USER_SEND);
            pktSendData.strPktData = JsonConvert.SerializeObject(userInfoPacket);

            SendPacket(pktSendData);
        }

        public void SendShowNotice()
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_SHOW_NOTICE);
            pktSendData.strPktData = string.Empty;

            SendPacket(pktSendData);
        }

        public void SendAbsentComplete(int nGiveCupn)
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ABSENT_COMPLETE);
            pktSendData.strPktData = nGiveCupn.ToString();

            SendPacket(pktSendData);
        }

        public void SendAbsentCheck()
        {
            CPacket pktSendData = new CPacket(CDefine.PKT_HEADER_C2S_GLOBAL_ABSENT_CHECK);
            SendPacket(pktSendData);
        }

    }
}
