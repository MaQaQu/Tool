using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// 将yyyyMMddHHmmss格式字符串拼接成datetime格式字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <summary>
        public string StringToDate(string time)
        {
            string Time = string.Format("{0}-{1}-{2} {3}:{4}:{5}", time.Substring(0, 4), time.Substring(4, 2), time.Substring(6, 2), time.Substring(8, 2), time.Substring(10, 2), time.Substring(12, 2));
            return Time;
        }
        /// <summary>
        /// 判断时间是否在指定开始/结束时间范围内
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtmin"></param>
        /// <param name="dtmax"></param>
        /// <returns></returns>
        public bool IsInDate(DateTime dt, DateTime dtmin, DateTime dtmax)
        {
            try
            {
                return dt.CompareTo(dtmin) >= 0 && dt.CompareTo(dtmax) <= 0;
            }
            catch { }
            return false;
        }
        /// 强制令时间落在指定区间内
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtmin"></param>
        /// <param name="dtmax"></param>
        /// <returns></returns>
        public DateTime SetInDate(DateTime dt, DateTime dtmin, DateTime dtmax)
        {
            try
            {
                if (dt.CompareTo(dtmin) < 0)
                {
                    return dtmin;
                }
                else if (dt.CompareTo(dtmax) > 0)
                {
                    return dtmax;
                }
                else
                {
                    return dt;
                }
            }
            catch { }
            return dt;
        }
        /// <summary>
        /// 获取整个sql文件
        /// </summary>
        /// <param name="varFileName"></param>
        /// <returns></returns>
        public ArrayList GetSqlFile(string varFileName)
        {
            ArrayList alSql = new ArrayList();
            if (!File.Exists(varFileName))
            {
                return alSql;
            }
            //StreamReader rs = new StreamReader(varFileName, System.Text.Encoding.Default);//注意编码
            using (var rs = new StreamReader(varFileName, System.Text.Encoding.UTF8))
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
        /// <summary>
        /// 将源文件拷贝保存为目标文件
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        /// <param name="eachReadLength"></param>
        /// <returns></returns>
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

        #region 导航数据处理
        /// <summary>
        /// 获取导航数据并分割成缓存文件
        /// </summary>
        /// <param name="varFileName"></param>
        /// <returns></returns>
        public List<string> GetSqlFileAndSplit_Nav(string varFileName)
        {
            List<string> alSql = new List<string>();
            List<string> alSqlFirst = new List<string>();
            if (!File.Exists(varFileName))
            {
                return alSql;
            }
            bool getFirstList = false;
            ClearCachefile_Nav();
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
                        beValueLine = true;
                    }
                    else if (beValueLine)
                    {
                        commtext = System.Text.RegularExpressions.Regex.Replace(varLine, @"(.*\()(.*)(\).*)", "$2");
                        alSql.Add(commtext);
                        if (!getFirstList) alSqlFirst.Add(commtext);
                        if (SaveArraylistToFile_Nav(varFileName, alSql))
                        {
                            getFirstList = true;
                            alSql.Clear();
                        }
                        beValueLine = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (alSql.Count > 0) { SaveFile_Nav(varFileName, alSql); }
                rs.Close();
                rs.Dispose();
                GC.Collect();
            }
            //获取第一个文件数据
            return alSqlFirst;
        }
        /// <summary>
        /// 从缓存文件中获取对应时间数据
        /// </summary>
        /// <param name="currentTime">yyyyMMddhhmmss格式字符串</param>
        /// <returns></returns>
        public List<string> GetSqlFileByTime_Nav(string currentTime)
        {
            List<string> alSql = new List<string>();
            try
            {
                string strDir = string.Format(@"{0}\CacheData\CacheData_Nav", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
                string[] strDataFiles = Directory.GetFiles(strDir);
                if (strDataFiles.Count() > 0)
                {
                    foreach (var item in strDataFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(item);
                        string[] name = fileName.Split('_');
                        int indexStart = name.Length >= 2 ? name.Length - 2 : 0;
                        int indexEnd = name.Length >= 1 ? name.Length - 1 : 0;
                        DateTime start = Convert.ToDateTime(StringToDate(name[indexStart]));
                        DateTime end = Convert.ToDateTime(StringToDate(name[indexEnd]));
                        //判断输入日期是否在起止日期之间
                        DateTime current = Convert.ToDateTime(StringToDate(currentTime));//yyyyMMddHHmmss格式字符串
                        if (IsInDate(current, start, end))
                        {
                            using (var rs = new StreamReader(item, System.Text.Encoding.Default))
                            {
                                while (rs.Peek() > -1)
                                {
                                    string varLine = rs.ReadLine();
                                    if (!string.IsNullOrEmpty(varLine))
                                    {
                                        alSql.Add(varLine);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
            return alSql;
        }
        /// <summary>
        /// 获取导航数据开始/结束时间
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DateTime GetStartEndTime_Nav(int type)
        {
            DateTime datetime = DateTime.MinValue;
            try
            {
                string strDir = string.Format(@"{0}\CacheData\CacheData_Nav", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
                string[] strDataFiles = Directory.GetFiles(strDir);
                DateTime dtStart = DateTime.MaxValue, dtEnd = DateTime.MinValue;
                if (strDataFiles.Count() > 0)
                {
                    foreach (var item in strDataFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(item);
                        string[] name = fileName.Split('_');
                        int indexStart = name.Length >= 2 ? name.Length - 2 : 0;
                        int indexEnd = name.Length >= 1 ? name.Length - 1 : 0;
                        DateTime start = Convert.ToDateTime(StringToDate(name[indexStart]));
                        DateTime end = Convert.ToDateTime(StringToDate(name[indexEnd]));
                        if (start < dtStart) dtStart = start;
                        if (end > dtEnd) dtEnd = end;
                    }
                }
                switch (type)
                {
                    case 1:
                        datetime = dtStart;
                        break;
                    case 2:
                        datetime = dtEnd;
                        break;
                }
            }
            catch { }
            return datetime;
        }
        /// <summary>
        /// 清空导航数据缓存文件
        /// </summary>
        private void ClearCachefile_Nav()
        {
            string strDir = string.Format(@"{0}\CacheData\CacheData_Nav", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
            DirectoryInfo dir = new DirectoryInfo(strDir);
            if (!dir.Exists) { dir.Create(); }
            else
            {
                string[] strDataFiles = Directory.GetFiles(strDir);
                if (strDataFiles.Count() > 0)
                {
                    foreach (var item in strDataFiles)
                    {
                        File.Delete(item);
                    }
                }
            }
        }
        /// <summary>
        /// 判断保存导航数据缓存文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="alSql"></param>
        /// <returns></returns>
        private bool SaveArraylistToFile_Nav(string filePath, List<string> alSql)
        {
            try
            {
                if (alSql.Count >= 500)
                {
                    if (SaveFile_Nav(filePath, alSql))
                        return true;
                    else return false;
                }
            }
            catch { }
            return false;
        }
        /// <summary>
        /// 保存导航数据缓存文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="alSql"></param>
        /// <returns></returns>
        private bool SaveFile_Nav(string filePath, List<string> alSql)
        {
            try
            {
                string split = ", ";
                string[] data_Start = Regex.Split(alSql[0], split, RegexOptions.IgnoreCase);
                string startTime = data_Start[6].Replace("'", "").Replace("-", "").Replace(":", "").Replace(" ", "");
                string[] data_End = Regex.Split(alSql[alSql.Count - 1], split, RegexOptions.IgnoreCase);
                string endTime = data_End[6].Replace("'", "").Replace("-", "").Replace(":", "").Replace(" ", "");
                string strDir = string.Format(@"{0}\CacheData\CacheData_Nav", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string newFileName = strDir + string.Format("\\{0}_{1}_{2}.dt", fileName, startTime, endTime);
                FileStream fs = new FileStream(newFileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                // 使用StreamWriter来往文件中写入内容 
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < alSql.Count; i++) sw.WriteLine(alSql[i]);
                //关闭此文件t 
                sw.Flush();
                sw.Close();
                fs.Close();
                return true;
            }
            catch { }
            return false;
        }
        #endregion
        #region 速度数据处理
        public List<string> GetSqlFileAndSplit_Speed(string varFileName)
        {
            List<string> alSql = new List<string>();
            List<string> alSqlFirst = new List<string>();
            if (!File.Exists(varFileName))
            {
                return alSql;
            }
            bool getFirstList = false;
            string strDir = string.Format(@"{0}\CacheData\CacheData_Speed", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
            ClearCachefile(strDir);
            using (var rs = new StreamReader(varFileName, System.Text.Encoding.UTF8))
            {
                string varLine = "";
                string commtext = "";
                bool beValueLine = false;
                while (rs.Peek() > -1)
                {
                    varLine = rs.ReadLine();
                    if (varLine.Contains("INSERT INTO"))
                    {
                        beValueLine = true;
                    }
                    else if (beValueLine)
                    {
                        commtext = Regex.Replace(varLine, @"(.*\()(.*)(\).*)", "$2");
                        alSql.Add(commtext);
                        if (!getFirstList) alSqlFirst.Add(commtext);
                        if (SaveArraylistToFile(alSql))
                        {
                            string filePath = GetCacheFilePath_Speed(varFileName, alSql);
                            SaveFile(filePath, alSql);
                            getFirstList = true;
                            alSql.Clear();
                        }
                        beValueLine = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (alSql.Count > 0)
                {
                    string filePath = GetCacheFilePath_Speed(varFileName, alSql);
                    SaveFile(filePath, alSql);
                }
                rs.Close();
                rs.Dispose();
                GC.Collect();
            }
            //获取第一个文件数据
            return alSqlFirst;
        }
        public List<string> GetSqlFileByTime_Speed(string currentTime)
        {
            List<string> alSql = new List<string>();
            try
            {
                string strDir = string.Format(@"{0}\CacheData\CacheData_Speed", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
                alSql = GetSqlFileByTime(strDir, currentTime);
            }
            catch { }
            return alSql;
        }
        private string GetCacheFilePath_Speed(string filePath, List<string> alSql)
        {
            string path = "";
            try
            {
                string split = ", ";
                string[] data_Start = Regex.Split(alSql[0], split, RegexOptions.IgnoreCase);
                string startTime = data_Start[10].Replace("'", "").Replace("-", "").Replace(":", "").Replace(" ", "");
                string[] data_End = Regex.Split(alSql[alSql.Count - 1], split, RegexOptions.IgnoreCase);
                string endTime = data_End[10].Replace("'", "").Replace("-", "").Replace(":", "").Replace(" ", "");
                string strDir = string.Format(@"{0}\CacheData\CacheData_Speed", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                path = strDir + string.Format("\\{0}_{1}_{2}.dt", fileName, startTime, endTime);
            }
            catch { }
            return path;
        }
        #endregion
        private bool SaveArraylistToFile(List<string> alSql)
        {
            try
            {
                if (alSql.Count >= 500)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
        private bool SaveFile(string filePath, List<string> alSql)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                // 使用StreamWriter来往文件中写入内容 
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < alSql.Count; i++) sw.WriteLine(alSql[i]);
                //关闭此文件t 
                sw.Flush();
                sw.Close();
                fs.Close();
                return true;
            }
            catch { }
            return false;
        }
        private void ClearCachefile(string strDir)
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);
            if (!dir.Exists) { dir.Create(); }
            else
            {
                string[] strDataFiles = Directory.GetFiles(strDir);
                if (strDataFiles.Count() > 0)
                {
                    foreach (var item in strDataFiles)
                    {
                        File.Delete(item);
                    }
                }
            }
        }
        private List<string> GetSqlFileByTime(string strDir, string currentTime)
        {
            List<string> alSql = new List<string>();
            try
            {
                string[] strDataFiles = Directory.GetFiles(strDir);
                if (strDataFiles.Count() > 0)
                {
                    foreach (var item in strDataFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(item);
                        string[] name = fileName.Split('_');
                        int indexStart = name.Length >= 2 ? name.Length - 2 : 0;
                        int indexEnd = name.Length >= 1 ? name.Length - 1 : 0;
                        DateTime start = Convert.ToDateTime(StringToDate(name[indexStart]));
                        DateTime end = Convert.ToDateTime(StringToDate(name[indexEnd]));
                        //判断输入日期是否在起止日期之间
                        DateTime current = Convert.ToDateTime(StringToDate(currentTime));//yyyyMMddHHmmss格式字符串
                        if (IsInDate(current, start, end))
                        {
                            using (var rs = new StreamReader(item, System.Text.Encoding.UTF8))
                            {
                                while (rs.Peek() > -1)
                                {
                                    string varLine = rs.ReadLine();
                                    if (!string.IsNullOrEmpty(varLine))
                                    {
                                        alSql.Add(varLine);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
            return alSql;
        }
    }
}
