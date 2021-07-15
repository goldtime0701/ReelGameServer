using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ReelServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int nty)
        {
            List<CUser> lstUser = new List<CUser>();
            string sql = string.Empty;

            if(nty == 0)
            {
                sql = "SELECT * FROM tbl_user";
            }
            else if(nty == 1)
            {
                sql = "SELECT * FROM tbl_user_back";
            }
            else
            {
                return string.Empty;
            }

            DataRowCollection users = CMysql.GetDataQuery(sql);

            for (int i = 0; i < users.Count; i++)
            {
                CUser user = new CUser();
                user.m_nUserCode = Convert.ToInt32(users[i]["userCode"]);
                user.m_strUserID = Convert.ToString(users[i]["userID"]);
                user.m_strUserPW = Convert.ToString(users[i]["userPW"]);
                user.m_strUserNick = Convert.ToString(users[i]["userNick"]);
                user.m_strUserMail = Convert.ToString(users[i]["userMail"]);
                user.m_strUserPhone = Convert.ToString(users[i]["userPhone"]);
                user.m_strUserName = Convert.ToString(users[i]["userName"]);
                user.m_strBankName = Convert.ToString(users[i]["bankName"]);
                user.m_strBankAcid = Convert.ToString(users[i]["bankAcid"]);
                user.m_nUserLevel = Convert.ToInt32(users[i]["userLevel"]);
                user.m_nUserCash = Convert.ToInt32(users[i]["userCash"]);
                user.m_nUserCupn = Convert.ToInt32(users[i]["userCupn"]);
                user.m_nAgenCode = Convert.ToInt32(users[i]["agenCode"]);
                user.m_nUserState = Convert.ToInt32(users[i]["userState"]);
                user.m_nEventState = Convert.ToInt32(users[i]["eventState"]);
                user.m_strUserRegTime = Convert.ToString(users[i]["userRegTime"]);
                user.m_nUserCharge = Convert.ToInt64(users[i]["userCharge"]);
                user.m_nUserExcharge = Convert.ToInt64(users[i]["userExcharge"]);
                user.m_nUserLogCnt = Convert.ToInt32(users[i]["userLogCnt"]);
                user.m_nAbsentCnt = Convert.ToInt32(users[i]["absentCnt"]);
                user.m_nUserUseCash = Convert.ToInt32(users[i]["userUseCash"]);
                user.m_nUserWinCash = Convert.ToInt32(users[i]["userWinCash"]);
                user.m_nUserBonusCash = Convert.ToInt32(users[i]["userRealCash"]);
                user.m_nChargeCnt = Convert.ToInt32(users[i]["chargeCnt"]);
                user.m_nExchargeCnt = Convert.ToInt32(users[i]["exchargeCnt"]);
                user.m_nAppendCash = Convert.ToInt32(users[i]["appendCash"]);
                user.m_nBonusCash = Convert.ToInt32(users[i]["bonusCash"]);
                user.m_nChargeCash = Convert.ToInt32(users[i]["chargeCash"]);
                user.m_nVirtualCash = Convert.ToInt32(users[i]["virtualCash"]);
                user.m_nVirtualCupn = Convert.ToInt32(users[i]["virtualCupn"]);
                user.m_strToken = Convert.ToString(users[i]["token"]);
                user.m_strLogTime = Convert.ToString(users[i]["userLogTime"]);
                user.m_nChatBlock = Convert.ToInt32(users[i]["chatBlock"]);
                user.m_nChatBlockA = Convert.ToInt32(users[i]["chatBlockA"]);
                user.m_nStoreCode = Convert.ToInt32(users[i]["storeCode"]);
                user.m_strMemo = Convert.ToString(users[i]["strMemo"]);

                lstUser.Add(user);
            }

            return JsonConvert.SerializeObject(lstUser);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
