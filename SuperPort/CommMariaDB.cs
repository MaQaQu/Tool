using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPort
{
    public class CommMariaDB
    {
        public static bool connectSuccessFlag = false;
        private string connectstr = "server=localhost;port=3306;database=youitoolkit;user=root;password=123456;SslMode = none;";
        private string connectstr2 = "server=localhost;port=3306;user=root;password=123456;SslMode = none;";
        private MySqlConnection conn;
        public CommMariaDB()
        {

        }
        public void ConnectMariaDB()
        {
            try
            {
                connectSuccessFlag = true;
                Console.WriteLine("连接成功!!!");
            }
            catch (Exception e)
            {
                connectSuccessFlag = false;
                Console.WriteLine(e.ToString());
            }
        }

        private bool OpenConnection()
        {
            try
            {
                conn = new MySqlConnection(connectstr);
                conn.Open();
                return true;
            }
            catch { }
            return false;
        }

        private bool CloseConnection()
        {
            try
            {
                if (conn == null)
                {
                    return false;
                }
                if (conn.State == ConnectionState.Open || conn.State == ConnectionState.Connecting)
                {
                    conn.Close();
                    conn.Dispose();
                    return true;
                }
                else
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        return true;
                    }
                    if (conn.State == ConnectionState.Broken)
                    {
                        return false;
                    }
                }
            }
            catch { }
            return false;
        }

        public bool DoExecuteNonQuery(string sqlString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectstr))
                {
                    conn.Open();
                    if (ConnectionState.Open == conn.State)
                    {
                        MySqlCommand command = new MySqlCommand(sqlString, conn);
                        int affectLines = command.ExecuteNonQuery();
                    }
                    conn.Close();
                    conn.Dispose();
                    return true;
                }
            }
            catch { }
            return false;
        }
        public DataTable DoSelect(string sqlString)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectstr))
                {
                    conn.Open();
                    if (ConnectionState.Open == conn.State)
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sqlString, conn);
                        adapter.Fill(dataTable);
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch { }
            return dataTable;
        }

        public ArrayList GetSqlFile(string varFileName)
        {
            ArrayList alSql = new ArrayList();
            if (!File.Exists(varFileName))
            {
                return alSql;
            }
            //StreamReader rs = new StreamReader(varFileName, System.Text.Encoding.Default);//注意编码
            using (var rs = new StreamReader(varFileName, System.Text.Encoding.Default))
            {
                string varLine = "";
                string commtext = "";
                bool beValueLine = false;
                while (rs.Peek() > -1)
                {
                    varLine = rs.ReadLine();
                    if (varLine.Contains("INSERT INTO"))
                    {
                        //数据存放在下一行
                        beValueLine = true;
                    }
                    else if (beValueLine)
                    {
                        commtext = System.Text.RegularExpressions.Regex.Replace(varLine, @"(.*\()(.*)(\).*)", "$2"); //提取小括号()中数据
                        alSql.Add(commtext);
                        beValueLine = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                rs.Close();
                rs.Dispose();
                GC.Collect();
            }
            return alSql;
        }

        public bool CopySqlFile(string fromPath, string toPath, int eachReadLength)
        {
            //将源文件 读取成文件流
            FileStream fromFile = new FileStream(fromPath, FileMode.Open, FileAccess.Read);
            //已追加的方式 写入文件流
            FileStream toFile = new FileStream(toPath, FileMode.Append, FileAccess.Write);
            //实际读取的文件长度
            int toCopyLength = 0;
            //如果每次读取的长度小于 源文件的长度 分段读取
            if (eachReadLength < fromFile.Length)
            {
                byte[] buffer = new byte[eachReadLength];
                long copied = 0;
                while (copied <= fromFile.Length - eachReadLength)
                {
                    toCopyLength = fromFile.Read(buffer, 0, eachReadLength);
                    fromFile.Flush();
                    toFile.Write(buffer, 0, eachReadLength);
                    toFile.Flush();
                    //流的当前位置
                    toFile.Position = fromFile.Position;
                    copied += toCopyLength;
                }
                int left = (int)(fromFile.Length - copied);
                toCopyLength = fromFile.Read(buffer, 0, left);
                fromFile.Flush();
                toFile.Write(buffer, 0, left);
                toFile.Flush();
            }
            else
            {
                //如果每次拷贝的文件长度大于源文件的长度 则将实际文件长度直接拷贝
                byte[] buffer = new byte[fromFile.Length];
                fromFile.Read(buffer, 0, buffer.Length);
                fromFile.Flush();
                toFile.Write(buffer, 0, buffer.Length);
                toFile.Flush();
            }
            fromFile.Close();
            toFile.Close();
            GC.Collect();
            return true;
        }
    }
}
