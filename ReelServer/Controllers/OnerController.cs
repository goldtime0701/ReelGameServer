using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReelServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnerController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get(int nOnerCmd, int nOnerCode = 0, string strValue = "")
        {
            string strRetValue = string.Empty;

            switch (nOnerCmd)
            {
                case 1: //로그인
                    strRetValue = COnerEngine.OnerLogin(nOnerCode, strValue);
                    break;
                case 2: //가입승인
                    strRetValue = COnerEngine.OnerUpadteUserState(nOnerCode, strValue);
                    break;
                case 3: //로그아웃
                    strRetValue = COnerEngine.OnerLogout(nOnerCode);
                    break;
                case 4: //회원 추가/수정
                    strRetValue = COnerEngine.OnerSaveUserInfo(nOnerCode, strValue);
                    break;
                case 5: //잭팟주기
                    strRetValue = COnerEngine.OnerSWKCallPrize(nOnerCode, strValue);
                    break;
                case 6: //금액초기화
                    strRetValue = COnerEngine.OnerClearGearCash(nOnerCode, strValue);
                    break;
                case 7: //잠금해제
                    strRetValue = COnerEngine.OnerReleaseGear(nOnerCode, strValue);
                    break;
                case 8: //충전승인
                    strRetValue = COnerEngine.OnerAcceptCharge(nOnerCode, strValue);
                    break;
                case 9: //충전취소
                    strRetValue = COnerEngine.OnerCancelCharge(nOnerCode, strValue);
                    break;
                case 10: //환전승인
                    strRetValue = COnerEngine.OnerAcceptExCharge(nOnerCode, strValue);
                    break;
                case 11: //환전취소
                    strRetValue = COnerEngine.OnerCancelExCharge(nOnerCode, strValue);
                    break;
                case 12: //1:1문의 답변
                    strRetValue = COnerEngine.OnerAnswerSend(nOnerCode, strValue);
                    break;
                case 13: //로봇추가
                    strRetValue = COnerEngine.OnerAddRobot(nOnerCode, strValue);
                    break;
                case 14: //로봇수정
                    strRetValue = COnerEngine.OnerUpdateRobot(nOnerCode, strValue);
                    break;
                case 15: //로봇삭제
                    strRetValue = COnerEngine.OnerDeleteRobot(nOnerCode, strValue);
                    break;
                case 16: //제천대성 잭팟포트설정
                    strRetValue = COnerEngine.OnerSetJackPot(nOnerCode, strValue);
                    break;
                case 17: //전체채팅
                    strRetValue = COnerEngine.OnerGlobalChat(nOnerCode, strValue);
                    break;
                case 18: //공지사항보내기
                    strRetValue = COnerEngine.OnerGlobalNotice(nOnerCode, strValue);
                    break;
                case 19: //유저캐시설정
                    strRetValue = COnerEngine.OnerSetUserCash(nOnerCode, strValue);
                    break;
                case 20: //유저쿠폰설정
                    strRetValue = COnerEngine.OnerSetUserCupn(nOnerCode, strValue);
                    break;
                case 21: //총판추가
                    strRetValue = COnerEngine.OnerCreateAgent(nOnerCode, strValue);
                    break;
                case 22: //알라딘잭팟주기
                    strRetValue = COnerEngine.OneALDCallPrize(nOnerCode, strValue);
                    break;
                case 23: //출석설정
                    strRetValue = COnerEngine.OnerSetAbsent(nOnerCode, strValue);
                    break;
                case 24: //누적금설정
                    strRetValue = COnerEngine.OnerSetTotalPot(nOnerCode, strValue);
                    break;
                case 25: //환수률설정
                    strRetValue = COnerEngine.OnerSetGameRate(nOnerCode, strValue);
                    break;
                case 26: //기계상세정보설정
                    strRetValue = COnerEngine.OnerSetGearDetail(nOnerCode, strValue);
                    break;
                case 27: //총판정산시 유저사용금액을 0으로 초기화
                    strRetValue = COnerEngine.AcceptAgentExReal(nOnerCode, strValue);
                    break;
                case 28: //체험머니 적용
                    strRetValue = COnerEngine.SetVirtualCash(nOnerCode, strValue);
                    break;
                case 29: //체험쿠폰 적용
                    strRetValue = COnerEngine.SetVirtualCupn(nOnerCode, strValue);
                    break;
                case 30: //첫가입시 체험머니적용설정
                    strRetValue = COnerEngine.SetVirtual(nOnerCode, strValue);
                    break;
                case 31: //첫충전시 보너스적용설정
                    strRetValue = COnerEngine.SetBonus(nOnerCode, strValue);
                    break;
                case 32://5드래곤 잭팟적용
                    strRetValue = COnerEngine.OneFDGCallPrize(nOnerCode, strValue);
                    break;
                case 33: //출석회수설정
                    strRetValue = COnerEngine.SetAbsentCount(nOnerCode, strValue);
                    break;
                case 34: //바다이야기 잭팟적용
                    strRetValue = COnerEngine.OneSeaCallPrize(nOnerCode, strValue);
                    break;
                case 35: //누적금액초기화
                    strRetValue = COnerEngine.ResetTotalPot(nOnerCode, strValue);
                    break;
                case 36: //총판정산취소
                    strRetValue = COnerEngine.CancelAgentExReal(nOnerCode, strValue);
                    break;
                case 37: //매장추가
                    strRetValue = COnerEngine.OnerCreateStore(nOnerCode, strValue);
                    break;
                case 38: //매장삭제
                    strRetValue = COnerEngine.OnerDeleteStore(nOnerCode, strValue);
                    break;
                case 39: //총판삭제
                    strRetValue = COnerEngine.OnerDeleteAgent(nOnerCode, strValue);
                    break;
                case 40: //매장정산시 유저사용금액을 0으로 초기화
                    strRetValue = COnerEngine.AcceptStoreExReal(nOnerCode, strValue);
                    break;
                case 41: //매장금액초기화
                    strRetValue = COnerEngine.ClearStoreCash(nOnerCode, strValue);
                    break;
                case 42: //매장정산취소
                    strRetValue = COnerEngine.CancelStoreExReal(nOnerCode, strValue);
                    break;
                case 43: //총판정산신청
                    strRetValue = COnerEngine.RequestAgentExReal(nOnerCode, strValue);
                    break;
                case 44: //총판정산금액초기화
                    strRetValue = COnerEngine.ClearAgenRealCash(nOnerCode, strValue);
                    break;
                case 45: //매장정산금액초기화
                    strRetValue = COnerEngine.ClearStoreRealCash(nOnerCode, strValue);
                    break;
                case 46: //매장유저충전승인
                    strRetValue = COnerEngine.StoreAcceptCharge(nOnerCode, strValue);
                    break;
                case 47: //매장유저환전승인
                    strRetValue = COnerEngine.StoreAcceptExcharge(nOnerCode, strValue);
                    break;
                case 48: //매장알환전신청
                    strRetValue = COnerEngine.StoreRequestAlExcharge(nOnerCode, strValue);
                    break;
                case 49: //매장유저빼기/삭제
                    strRetValue = COnerEngine.StoreUserPopDel(nOnerCode, strValue);
                    break;
                case 50: //가상실시간출금현황을 사용하겠는가
                    strRetValue = COnerEngine.UseTempExcharge(nOnerCode, strValue);
                    break;
                case 51: //강제탈퇴기능
                    strRetValue = COnerEngine.CallUserLogout(nOnerCode, strValue);
                    break;
                case 52: //관리자페지에 유저리스트전송시작
                    strRetValue = COnerEngine.GetUserListFromAdminPage(nOnerCode, strValue);
                    break;
                case 53: //황금성 잭팟적용
                    strRetValue = COnerEngine.OneGdcCallPrize(nOnerCode, strValue);
                    break;
                case 54:
                    strRetValue = COnerEngine.OneUserRegister(nOnerCode, strValue);
                    break;
                case 55: //알라딘2잭팟주기
                    strRetValue = COnerEngine.OneALD2CallPrize(nOnerCode, strValue);
                    break;
                case 56: //오션 잭팟적용
                    strRetValue = COnerEngine.OneOcaCallPrize(nOnerCode, strValue);
                    break;
                case 57: //신천지 잭팟적용
                    strRetValue = COnerEngine.OneNwdCallPrize(nOnerCode, strValue);
                    break;
                case 58: //유저메모저장기능
                    strRetValue = COnerEngine.SetUserMemo(nOnerCode, strValue);
                    break;
                case 59: //로봇기계해제
                    strRetValue = COnerEngine.OnerReleaseRobot(nOnerCode, strValue);
                    break;
                case 60: //차단아이피추가
                    strRetValue = COnerEngine.OnerAddBlockIP(nOnerCode, strValue);
                    break;
                case 61: //차단아이피삭제
                    strRetValue = COnerEngine.OnerDeleteBlockIP(nOnerCode, strValue);
                    break;
                case 62: //계좌등록
                    strRetValue = COnerEngine.OnSetAcid(nOnerCode, strValue);
                    break;
                case 63: //야마토잭팟
                    strRetValue = COnerEngine.OnYmtCallPrize(nOnerCode, strValue);
                    break;
                case 64: //2차자동환수설정
                    strRetValue = COnerEngine.OnSetAutoPrize(nOnerCode, strValue);
                    break;
                case 65: //다빈치 잭팟적용
                    strRetValue = COnerEngine.OnDvcCallPrize(nOnerCode, strValue);
                    break;
                case 66: //백경 잭팟적용
                    strRetValue = COnerEngine.OnWhtCallPrize(nOnerCode, strValue);
                    break;
                case 67: //양귀비 잭팟적용
                    strRetValue = COnerEngine.OnYanCallPrize(nOnerCode, strValue);
                    break;
                case 68: //공지사항이미지 로그인할때 보여주겠는가
                    strRetValue = COnerEngine.OnSetShowNoticeImage(nOnerCode, strValue);
                    break;
                case 69: //공지사항을 현재 가입하고 있는 유저들에게 보여주겠는가
                    strRetValue = COnerEngine.OnShowNoticeImage(nOnerCode, strValue);
                    break;
                case 70:  //영본사 요율설정
                    strRetValue = COnerEngine.SetSubAdmin0Pro(nOnerCode, strValue);
                    break;
                case 71:  //부본사 요율설정
                    strRetValue = COnerEngine.SetSubAdmin1Pro(nOnerCode, strValue);
                    break;
                case 72:  //총판 요율설정
                    strRetValue = COnerEngine.SetSubAdmin2Pro(nOnerCode, strValue);
                    break;
                case 73:  //본사 요율설정
                    strRetValue = COnerEngine.SetSubAdminPro(nOnerCode, strValue);
                    break;
                case 74: //단계정산진행
                    strRetValue = COnerEngine.RelaseRealCash(nOnerCode, strValue);
                    break;
                //case 75: //영본사정산진행
                //    strRetValue = COnerEngine.RelaseRealCash0(nOnerCode, strValue);
                //    break;
                //case 76: //부본사정산진행
                //    strRetValue = COnerEngine.RelaseRealCash1(nOnerCode, strValue);
                //    break;
                //case 77: //총판정산진행
                //    strRetValue = COnerEngine.RelaseRealCash2(nOnerCode, strValue);
                //    break;
                case 78:  //영본사 총요율설정
                    strRetValue = COnerEngine.SetSubAdmin0TotalPro(nOnerCode, strValue);
                    break;
                case 79:  //부본사 총요율설정
                    strRetValue = COnerEngine.SetSubAdmin1TotalPro(nOnerCode, strValue);
                    break;
                case 80:  //총판 총요율설정
                    strRetValue = COnerEngine.SetSubAdmin2TotalPro(nOnerCode, strValue);
                    break;
                case 81: //영본사추가/수정
                    strRetValue = COnerEngine.SaveSubAdmin0(nOnerCode, strValue);
                    break;
                case 82: //부본사추가/수정
                    strRetValue = COnerEngine.SaveSubAdmin1(nOnerCode, strValue);
                    break;
                case 83: //총판추가/수정
                    strRetValue = COnerEngine.SaveSubAdmin2(nOnerCode, strValue);
                    break;
                case 84: //매장추가/수정
                    strRetValue = COnerEngine.SaveSubAdmin3(nOnerCode, strValue);
                    break;
                case 85: //관리자페지에서 회원가입(4번과 꼭 같은 기능이다. 새버전관리자에서 사용)
                    strRetValue = COnerEngine.SaveUserInfoFromAdmin(nOnerCode, strValue);
                    break;
                case 86: //총판페지에 유저리스트전송시작
                    strRetValue = COnerEngine.GetUserListFromStorePage(nOnerCode, strValue);
                    break;
                case 87: //매장에서 유저금액 넣기
                    strRetValue = COnerEngine.SaveUserChargeFormStore(nOnerCode, strValue);
                    break;
                case 88: //매장에서 유저금액 빼기
                    strRetValue = COnerEngine.SaveUserExchargeFormStore(nOnerCode, strValue);
                    break;
                case 89: //매장사이 알이동
                    strRetValue = COnerEngine.ExchangeAlBetweenStore(nOnerCode, strValue);
                    break;
                case 90: //매장쿠폰전환수익초기화
                    strRetValue = COnerEngine.ClearExcupnRealStore(nOnerCode, strValue);
                    break;
                case 91: //아이템가격수정
                    strRetValue = COnerEngine.SetItemPrice(nOnerCode, strValue);
                    break;
                case 92: //다운로드페지로그인
                    strRetValue = COnerEngine.UserPageLogin(nOnerCode, strValue);
                    break;
                case 93: //아이템엔진잭팟점수설정
                    strRetValue = COnerEngine.SetItemJackCash(nOnerCode, strValue);
                    break;
                case 94: //아이템엔진누적금액설정
                    strRetValue = COnerEngine.SetItemUseCash(nOnerCode, strValue);
                    break;
                case 95: //아이템엔진발동금액설정
                    strRetValue = COnerEngine.SetItemRaiseCash(nOnerCode, strValue);
                    break;

                default:
                    break;
            }

            return strRetValue;
        }
    }
}
