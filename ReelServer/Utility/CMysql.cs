using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;

namespace ReelServer
{
    public static class CMysql
    {
        public static string SRV_DB_ADDRESS = "127.0.0.1";
        public static string SRV_DB_DATABASE = "reel";
        public static string SRV_DB_PORT = "1433";
        public static string SRV_DB_USERID = "traxex";
        public static string SRV_DB_PASSWORD = "pwdTraxex";

        public static string GetDBConnectString()
        {
            string db_server = "server=" + SRV_DB_ADDRESS + ";";
            db_server += "port=" + SRV_DB_PORT + ";";
            db_server += "database=" + SRV_DB_DATABASE + ";";
            db_server += "uid=" + SRV_DB_USERID + ";";
            db_server += "pwd=" + SRV_DB_PASSWORD + ";";
            db_server += "CharSet=utf8;";

            return db_server;
        }


        public static void ExcuteQuery(string sql)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(GetDBConnectString()))
            {
                mysqlCon.Open();
                new MySqlCommand(sql, mysqlCon).ExecuteNonQuery();
                
                mysqlCon.Close();
            }
        }


        public static DataRowCollection GetDataQuery(string sql)
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(GetDBConnectString()))
            {
                mysqlCon.Open();
                DataSet dataset = new DataSet();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = new MySqlCommand(sql, mysqlCon);
                adapter.Fill(dataset);
                mysqlCon.Close();

                return dataset.Tables[0].Rows;
            }
        }

        public static void ExcuteStoredProceDayClear()
        {
            string strNextDay = CMyTime.AddTime(CMyTime.GetMyTime(), 0, 0, 0, 3).ToString("yyyy-MM-dd");
            string strClearDay = CMyTime.AddTime(CMyTime.GetMyTime(), -30, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            string strHisDay = CMyTime.AddTime(CMyTime.GetMyTime(), -100, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");

            using (MySqlConnection mysqlCon = new MySqlConnection(GetDBConnectString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("DayClear", mysqlCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strNextDay", strNextDay);
                    cmd.Parameters.AddWithValue("@strClearDay", strClearDay);
                    cmd.Parameters.AddWithValue("@strHisDay", strHisDay);
                    cmd.Parameters.Add(new MySqlParameter("?nRet", MySqlDbType.Int32));
                    cmd.Parameters["?nRet"].Direction = ParameterDirection.Output;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    int nRet = Convert.ToInt32(cmd.Parameters["?nRet"].Value);

                }
            }
        }

        public static MySqlDataReader ReadQuery(MySqlConnection mysqlCon, string sql)
        {
            try
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, mysqlCon);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    return reader;
                }
                catch (Exception e)
                {
                    CGlobal.Log(e.Message);
                    throw (e);

                }
            }
            catch (Exception e)
            {
                CGlobal.Log(e.Message);
            }
            return null;
        }

        public static void LoadConfig()
        {
            string[] lines = File.ReadAllLines("config.ini");
            foreach (string line in lines)
            {
                string[] param = line.Split(':');
                if (param.Length != 2)
                    continue;
                if (param[0].Trim() == "server")
                {
                    CMysql.SRV_DB_ADDRESS = param[1].Trim();
                }
                else if (param[0].Trim() == "db")
                {
                    CMysql.SRV_DB_DATABASE = param[1].Trim();
                }
                else if (param[0].Trim() == "port")
                {
                    CMysql.SRV_DB_PORT = param[1].Trim();
                }
                else if (param[0].Trim() == "userid")
                {
                    CMysql.SRV_DB_USERID = param[1].Trim();
                }
                else if (param[0].Trim() == "password")
                {
                    CMysql.SRV_DB_PASSWORD = param[1].Trim();
                }
                else if (param[0].Trim() == "onerPage")
                {
                    CDefine.SRV_ADMIN_PAGE_PORT = Convert.ToInt32(param[1]);
                }
                else if (param[0].Trim() == "admin")
                {
                    CDefine.SRV_ADMIN_SOCKET_PORT = Convert.ToInt32(param[1]);
                }
                else if (param[0].Trim() == "agent")
                {
                    CDefine.SRV_AGENT_SOCKET_PORT = Convert.ToInt32(param[1]);
                }
                else if (param[0].Trim() == "client")
                {
                    string[] str = param[1].Split('-');
                    int nFrom = Convert.ToInt32(str[0]);
                    int nTo = Convert.ToInt32(str[1]);

                    for (int i = nFrom; i <= nTo; i++)
                    {
                        CDefine.SRV_USER_SOCKET_PORT.Add(i);
                    }
                }
                else if (param[0].Trim() == "log")
                {
                    int nLog = Convert.ToInt32(param[1]);
                    CGlobal._bLog = nLog == 1;
                }
            }
        }
    }
}
