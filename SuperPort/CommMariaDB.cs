using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPort
{
    public class CommMariaDB
    {
        public static bool connectSuccessFlag = false;
        private string connectstr = "server=localhost;port=3306;database=youitoolkit;user=root;password=123456;SslMode = none;";
        private MySqlConnection conn;
        public CommMariaDB()
        {

        }
        public void ConnectMariaDB()
        {
            try
            {
                conn = new MySqlConnection(connectstr);
                conn.Open();
                connectSuccessFlag = true;
                Console.WriteLine("连接成功!!!");
            }
            catch (Exception e)
            {
                connectSuccessFlag = false;
                Console.WriteLine(e.ToString());
            }
        }
        public int DoExecuteNonQuery(string sqlString)
        {
            try
            {
                if (conn == null || ConnectionState.Open != conn.State)
                {
                    return -1;
                }
                MySqlCommand command = new MySqlCommand(sqlString, conn);
                int affectLines = command.ExecuteNonQuery();
                return affectLines;
            }
            catch { }
            return -1;
        }
        public DataTable DoSelect(string sqlString)
        {
            DataTable dataTable = new DataTable();
            try
            {
                if (conn != null && ConnectionState.Open == conn.State)
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlString, conn);
                    adapter.Fill(dataTable);
                }
            }
            catch { }
            return dataTable;
        }

    }
}
