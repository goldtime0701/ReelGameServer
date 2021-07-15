using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReelServer
{
    public class CStoreModel
    {
        public int m_nStoreCode;            //코드
        public string m_strStoreID;         //아이디
        public string m_strStorePW;         //비번
        public string m_strStoreNick;       //닉네임
        public int m_nStoreCash;            //보유머니
        public int m_nStoreState;           //상태 0-대기, 1-정상, 2-차단
        public string m_strStoreRegTime;    //등록날자
        public long m_nStoreCharge;         //총 입금금액
        public long m_nStoreExcharge;       //총 환전금액
        public int m_nStoreChargeCnt;       //총 입금회수
        public int m_nStoreExchargeCnt;     //총 환전회수
        public int m_nUserCnt;              //소속회원수
        public int m_nStorePro;             //매장루징프로
        public int m_nStoreReal;            //적립금액
        public string m_strLastTime;        //마지막 정산시간
        public int m_nStoreExReal;          //정산받은 금액
        public int m_nStoreExRealCnt;       //정산받은 회수
        public int m_nCupnPro;              //쿠폰전환프로
        public int m_nDemo;                 //데모매장
        public int m_nSuperCode;            //총판코드
        public string m_strStorePhone;      //매장전하번호
        public string m_strStoreMark;       //매장가입코드
        public long m_nExcupnReal;          //왕빠에서 상품권교환수익누적
    }
}
