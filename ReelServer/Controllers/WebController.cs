using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ReelServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebController : ControllerBase
    {
        // GET: api/Web/5
        [HttpGet("{strCmd}", Name = "Get")]
        public string Get(string strCmd)
        {
            string strAgentID = string.Empty, strUserID = string.Empty, strUserNick = string.Empty, strUserPW = string.Empty, strFromDate = string.Empty, strToDate = string.Empty, strIP = string.Empty;
            int nCash = 0, nCupn = 0, nDataIndex = 0;
            string strParams = Request.QueryString.Value.Substring(1);
            string[] strParam = strParams.Split('&');
            for(int i=0; i<strParam.Length; i++)
            {
                string strKey = strParam[i].Split('=')[0];
                string strValue = strParam[i].Split('=')[1];

                if (strKey == "strCmd")
                    strCmd = strValue;
                else if (strKey == "strAgentID")
                    strAgentID = strValue;
                else if (strKey == "strUserID")
                    strUserID = strValue;
                else if (strKey == "strUserNick")
                    strUserNick = strValue;
                else if (strKey == "strUserPW")
                    strUserPW = strValue;
                else if (strKey == "strFromDate")
                    strFromDate = strValue;
                else if (strKey == "strToDate")
                    strToDate = strValue;
                else if (strKey == "strIP")
                    strIP = strValue;
                else if (strKey == "nCash")
                    nCash = Convert.ToInt32(strValue);
                else if (strKey == "nCupn")
                    nCupn = Convert.ToInt32(strValue);
                else if (strKey == "nDataIndex")
                    nDataIndex = Convert.ToInt32(strValue);
            }



            CWebResponse clsRespnse;
            CStore clsStore = null;

            clsRespnse = CWebEngine.CheckAgentID(strAgentID, ref clsStore);
            if (clsStore == null)
                return JsonConvert.SerializeObject(clsRespnse);

            if(strCmd == "createPlayer")
            {
                clsRespnse = CWebEngine.CreatePlayer(clsStore, strUserID, strUserNick, strUserPW);
            }
            else if(strCmd == "getToken")
            {
                clsRespnse = CWebEngine.GetPlayerToken(clsStore, strUserID, strIP);
            }
            else if(strCmd == "getCash")
            {
                clsRespnse = CWebEngine.GetCash(clsStore, strUserID);
            }
            else if(strCmd == "exchangeCupn")
            {
                clsRespnse = CWebEngine.ExchangeCupn(clsStore, strUserID, nCupn);
            }
            else if(strCmd == "chargeCash")
            {
                clsRespnse = CWebEngine.ChargeCash(clsStore, strUserID, nCash);
            }
            else if(strCmd == "exchargeCash")
            {
                clsRespnse = CWebEngine.ExchargeCash(clsStore, strUserID, nCash);
            }
            else if(strCmd == "getChargeList")
            {
                clsRespnse = CWebEngine.GetChargeList(clsStore, strUserID, strFromDate, strToDate);
            }
            else if(strCmd == "getExchargeList")
            {
                clsRespnse = CWebEngine.GetExchargeList(clsStore, strUserID, strFromDate, strToDate);
            }
            else if(strCmd == "getPrizeList")
            {
                clsRespnse = CWebEngine.GetPrizeList(clsStore, strUserID, strFromDate, strToDate);
            }
            else if(strCmd == "getAgentCash")
            {
                clsRespnse = CWebEngine.GetAgentCash(clsStore);
            }
            else if(strCmd == "getBettingList")
            {
                clsRespnse = CWebEngine.GetBettingList(clsStore, strUserID, nDataIndex);
            }


            return JsonConvert.SerializeObject(clsRespnse);
        }
        
    }
}
