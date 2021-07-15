using System;
using System.IO;
using System.Net;

namespace ReelServer
{
    public class CHttp
    {
        public static string GetResponseString(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (Exception err)
            {
                CGlobal.Log(err.Message);
                return "error";
            }
        }
    }
}
