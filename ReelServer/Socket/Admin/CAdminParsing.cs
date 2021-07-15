using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CAdminSocket
    {
        private void RecvParsing(CPacket pktRecvData)
        {
            switch (pktRecvData.nPktHeader)
            {
                case CDefine.PKT_HEADER_O2S_GLOBAL_AUTH_REQUEST:                //인증파케트
                    AdminAuth(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_STOREAUTH_REQUEST:         //매장인증파케트
                    StoreAuth(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_USERLIST_REQUEST:            //매장회원리스트
                    StoreUserList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_USERGEARLIST_REQUEST:       //유저가 선택한 기어리스트
                    //AdminUserGearList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_ANSWER_SEND:
                    AdminSendAnswer(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_CHAT_REQUEST:
                    AdminSendChat(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_GEARLIST_REQUEST:
                    AdminGearList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_ROBOT_CHAT_REQUEST:
                    AdminRobotChat(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_NOTICE_REQUEST:
                    AdminSendNotice(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_GLOBAL_BROADCHAT_REQUEST:
                    AdminSendBroadChat(pktRecvData.strPktData);
                    break;


                case CDefine.PKT_HEADER_O2S_CHAT_LOGIN:
                    ChatOnerLogin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_CHAT_USERLIST:
                    ChatUserList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_CHAT_ROBOTLIST:
                    ChatRobotList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_CHAT_GROUP:
                    ChatGroup(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_CHAT_SECURITY:
                    ChatSecurity(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_O2S_CHAT_BLOCK:
                    ChatBlock(pktRecvData.strPktData);
                    break;
            }
            
        }
    }
}
