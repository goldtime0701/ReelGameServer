using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ReelServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [DisableCors]
        [HttpGet]
        public ActionResult<string> Get(int nOnerCmd, string strValue)
        {
            string strRetValue = string.Empty;

            switch (nOnerCmd)
            {
                case 1: //로그인
                    strRetValue = CUserEngine.UserLogin(strValue);
                    break;
                case 2: //유저소켓포트요청 및 버전확인
                    strRetValue = CUserEngine.UserSocket();
                    break;
            }

            return strRetValue;
        }

        

    }
}
