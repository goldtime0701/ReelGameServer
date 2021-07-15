using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public partial class CUserSocket
    {
        private void RecvParsing(CPacket pktRecvData)
        {
            if(m_clsUser != null)
            {
                if (pktRecvData.nPktHeader != CDefine.PKT_HEADER_GLOBAL_LOGIN && m_clsUser.m_strToken != pktRecvData.strToken)
                    return;
            }
            else
            {
                if (pktRecvData.nPktHeader != CDefine.PKT_HEADER_GLOBAL_LOGIN && pktRecvData.nPktHeader != CDefine.PKT_HEADER_GLOBAL_REPID
                    && pktRecvData.nPktHeader != CDefine.PKT_HEADER_GLOBAL_REPNICK && pktRecvData.nPktHeader != CDefine.PKT_HEADER_GLOBAL_REGISTER
                    && pktRecvData.nPktHeader != CDefine.PKT_HEADER_C2S_WEB_USERINFO && pktRecvData.nPktHeader != CDefine.PKT_HEADER_GLOBAL_RECONNECT)
                    return;
            }

            switch (pktRecvData.nPktHeader)
            {
                case CDefine.PKT_HEADER_GLOBAL_LOGIN:                                       //로그인
                    UserLogin(pktRecvData.strPktData);
                    break;      
                case CDefine.PKT_HEADER_GLOBAL_RECONNECT:                                   //재접속
                    UserReconnect(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_GLOBAL_REPID:                                       //회원가입시 아이디 체크                                  
                    RegistRepUserID(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_GLOBAL_REPNICK:                                     //회원가입시 닉네임 체크
                    RegistRepUserNick(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_GLOBAL_REGISTER:                                    //회원가입
                    RegistUser(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_CHART:                                   //공개채팅
                    GroupChat(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ACID:                                    //계좌문이
                    AcidFaq(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_CHARGE:                                  //충전신청
                    ChargeRequest(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_EXCHARGE:                                //환전신청
                    ExchargeRequest(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_EXCUPON:                                 //쿠폰 머니로 교환
                    ExcupnRequest(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_EXCUPON_RATE:                            //쿠폰교환레이트
                    ExcupnRate(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_SECURITY_CHAT:                           //관리자단독채팅
                    SecurityChat(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEARPOSTLIST:                            //기계목록요청
                    GearPostList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEARKEEPLIST:                            //예약한기계목록
                    GearKeepList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEARPOST:                                //기계예약진행
                    GearPost(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_EXIT:                                    //게임선택창으로 이동
                    ExitGamePage(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_LOGOUT:                                  //로그아웃
                    LogoutUser(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_WEB_USERINFO:                                   //웹버전유저로그인
                    WebLogin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ABSENT_LIST:                             //출석정보리스트
                    AbsentList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ABSENT_CHECK:                            //출석체크
                    AbsentCheck(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ABSENT_COMSEND:                          //출석미션완료체크
                    AbsentCompleteCheck(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_JACKINFO_LIST:                           //잭팟리스트
                    GearJackInfo(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEAR_CHECK:                              //기계예약기계체크
                    GearPostCheck(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_PRICE_LIST:                         //아이템가격리스트를 내려보낸다.
                    GetItemPriceList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_KEEP_LIST:                          //보유아이템리스트
                    GetItemKeepList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_BUY:                                //아이템사기
                    BuyItemByUser(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_USE:                                //아이템사용
                    UseItemByUser(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_ITEM_GEAR:                               //아이템을 사용할수 있는 기계목록
                    GetItemGearList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEARLIST_REQUEST:                        //기어정보리스트
                    GetGearInfoList(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_SELECTGEAR_REQUEST:                      //기어선택
                    SelectGear(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_APPENDCASH_REQUEST:                      //만원넣기
                    AppendCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SEA_STARTSPIN_RECV:                             //바다이야기 스핀돌기시작
                    SEAStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SEA_COINCREATE_RECV:                            //바다이야기 코인생성
                    SEACreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SEA_SLOTADD_RECV:                               //바다이야기 슬롯머니 증가
                    SEASlotCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SEA_WATERTANK_RECV:
                    SEAWaterTank(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEARSTART_REQUEST:                       //게임시작      
                    GearStart(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GLOBAL_GEARSTOP_REQUEST:                        //게임정지     
                    GearStop(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SWK_COINCREATE_RECV:                            //손오공코인생성      
                    SWKCreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SWK_STARTSPIN_RECV:                             //손오공 스핀돌기시작
                    SWKStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_SWK_GIFTADD_RECV:                               //손오공 Gift 증가
                    SWKGiftCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD_ENDANI_RECV:                                //알라딘 God 애니매션 끝신호      
                    ALDEndAni(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD_STARTSPIN_RECV:                             //알라딘 스핀돌기시작
                    ALDStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD_SLOTADD_RECV:                               //알라딘 슬롯캐시 감소
                    ALDAddSlotCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD_GIFTADD_RECV:                               //알라딘 기프트머니 증가
                    ALDAddGiftCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD2_ENDANI_RECV:                                //알라딘2 God 애니매션 끝신호      
                    ALD2EndAni(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD2_STARTSPIN_RECV:                             //알라딘2 스핀돌기시작
                    ALD2StartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD2_SLOTADD_RECV:                               //알라딘2 슬롯캐시 감소
                    ALD2AddSlotCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_ALD2_GIFTADD_RECV:                               //알라딘2 기프트머니 증가
                    ALD2AddGiftCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_FDG_STARTSPIN_RECV:
                    FDGStartSpin(pktRecvData.strPktData);                                   //5드래곤 스핀시작                                  
                    break;
                case CDefine.PKT_HEADER_C2S_FDG_DRG_INFO_RECV:
                    FDGStartFreeSpin(pktRecvData.strPktData);                               //5드래곤 프리돌기시작    
                    break;
                case CDefine.PKT_HEADER_C2S_FDG_EXMONEY_RECV:
                    FDGExMoney(pktRecvData.strPktData);                                     //5드래곤 머니전환      
                    break;
                case CDefine.PKT_HEADER_C2S_FDG_GIFTADD_RECV:
                    FDGGiftAdd(pktRecvData.strPktData);                                     //5드래곤 기프트점수증가    
                    break;
                case CDefine.PKT_HEADER_C2S_GDC_STARTSPIN_RECV:                             //황금성스핀시작
                    GDCStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GDC_GIFTADD_RECV:                               //황금성기프트머니증가
                    GDCAddGiftCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GDC_COINCREATE_RECV:                            //황금성 코인생성
                    GDCreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_GDC_JACHADD_RECV:                               //황금성잭크포트점수증가
                    GDCAddJachCash(pktRecvData.strPktData);
                    break;          
                case CDefine.PKT_HEADER_C2S_GDC_WINJACK_RECV:                               //황금성 잭크포트점수당첨
                    GDCWinJachCash(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_OCA_STARTSPIN_RECV:                             //오션 스핀돌기시작
                    OCAStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_OCA_COINCREATE_RECV:                            //오션 코인생성
                    OCACreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_OCA_GIFTADD_RECV:                               //오션 슬롯머니 증가
                    OCAGiftCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_OCA_FINISHANI_RECV:                             //오션 애니매션 끝
                    OCAFinishAnimation(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_OCA_WATERTANK_RECV:                             //오션물탱크정보
                    OCAWaterTank(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_NWD_STARTSPIN_RECV:                             //신천지 스핀돌기시작
                    NWDStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_NWD_COINCREATE_RECV:                            //신천지 코인생성
                    NWDCreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_NWD_GIFTADD_RECV:                               //신천지 슬롯머니 증가
                    NWDGiftCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_NWD_FINISHANI_RECV:                             //신천지 애니매션 끝
                    NWDFinishAnimation(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_NWD_WATERTANK_RECV:                             //신천지 물탱크
                    NWDWaterTank(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YMT_STARTSPIN_RECV:                             //야마토 스핀돌기시작
                    YMTStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YMT_GIFTADD_RECV:                               //야마토 기프트머니증가
                    YMTAddGift(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YMT_EXMONEY_RECV:                               //야마토 머니전환
                    YMTExmoney(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_DVC_STARTSPIN_RECV:                             //다빈치 스핀돌기시작
                    DVCStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_DVC_COINCREATE_RECV:                            //다빈치 코인생성
                    DVCCreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_DVC_GIFTADD_RECV:                               //다빈치 슬롯머니 증가
                    DVCGiftCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_DVC_FINISHANI_RECV:                             //다빈치 애니매션 끝
                    DVCFinishAnimation(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_WHT_STARTSPIN_RECV:                             //백경 스핀돌기시작
                    WHTStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_WHT_COINCREATE_RECV:                            //백경 코인생성
                    WHTCreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_WHT_GIFTADD_RECV:                               //백경 슬롯머니 증가
                    WHTGiftCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_WHT_FINISHANI_RECV:                             //백경 애니매션 끝
                    WHTFinishAnimation(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_WHT_WATERTANK_RECV:                             //백경 수조증가
                    WHTWaterTank(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YAN_STARTSPIN_RECV:                             //양귀비 스핀돌기시작
                    YANStartSpin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YAN_COINCREATE_RECV:                            //양귀비 코인생성
                    YANCreateCoin(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YAN_GIFTADD_RECV:                               //양귀비 슬롯머니 증가
                    YANGiftCashAdd(pktRecvData.strPktData);
                    break;
                case CDefine.PKT_HEADER_C2S_YAN_FINISHANI_RECV:                             //양귀비 애니매션 끝
                    YANFinishAnimation(pktRecvData.strPktData);
                    break;
                default:
                    break;
            }
        }
    }
}
