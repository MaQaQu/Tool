using SuperMath;
using SuperPort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YouiToolkit.Assist;
using YouiToolkit.Models;

namespace YouiToolkit.ViewModels
{
    public class PageMtMapRenderViewModel
    {
        public Models.PageMaintainModel mo = Models.PageMaintainModel.CreateInstance();
        public Models.PageMtMapRenderModel mapModel = Models.PageMtMapRenderModel.CreateInstance();
        CommMariaDB commMariaDB = new CommMariaDB();
        public void GetFilesNameTable()
        {
            try
            {
                //mo.dtNavFilesName = commMariaDB.DoSelect("SELECT * FROM simulatenavfilesname ORDER BY DATE desc,starttime desc");
                string path = GetDirPath() + "\\simulatenavfilesname.sql";
                ArrayList sqlArrayList = commMariaDB.GetSqlFile(path);
                UpdateFilesName(sqlArrayList);
            }
            catch { }
        }

        public void GetAlarmData()
        {
            try
            {
                string path = GetDirPath() + "\\simulateagvalarmdata_1.sql";
                ArrayList sqlArrayList = commMariaDB.GetSqlFile(path);
                UpdateAlarmData(sqlArrayList);
            }
            catch { }
        }

        //保存导航文件数据
        public bool DownloadNavData(string filename)
        {
            try
            {
                //接收导航数据，存放在缓存文件夹中(只保留1个)
                mo.strNavDataCacheFilePath = GetDirPath() + "\\" + filename;
                AddDownloadStep(20);
                //从缓存表中提取数据
                List<string> sqlList = commMariaDB.GetSqlFileAndSplit(mo.strNavDataCacheFilePath);
                AddDownloadStep(40);
                //将缓存表数据更新到datatable
                UpdateNavData(sqlList);
                AddDownloadStep(60);
                //更新画布数据源
                UpdateMapSource("first");
                AddDownloadStep(80);
                SetNavDataTime();
                AddDownloadStep(100);
                return true;
            }
            catch
            {
                AddDownloadStep(100);
                GC.Collect();
            }
            return false;
        }
        public bool OpenNavDataSqlFile(string fileName)
        {
            try
            {
                mo.strNavDataCacheFilePath = fileName;
                List<string> sqlList = commMariaDB.GetSqlFileAndSplit(fileName);
                UpdateNavData(sqlList);
                UpdateMapSource("first");
                SetNavDataTime();
                return true;
            }
            catch { GC.Collect(); }
            return false;
        }
        public bool SaveNavDataSqlFile(string targetName)
        {
            try
            {
                int length = 5 * 1024 * 1024;
                commMariaDB.CopySqlFile(mo.strNavDataCacheFilePath, targetName, length);
                return true;
            }
            catch { }
            return false;
        }
        public bool DeleteNavData(string filename)
        {
            try
            {
                AddDeleteStep(25);
                System.Threading.Thread.Sleep(100);
                AddDeleteStep(50);
                File.Delete(GetDirPath() + "\\" + filename);
                AddDeleteStep(75);
                string[] s = filename.Split('_', '.');
                DeleteNavData("simulatenavfilesname", Convert.ToInt32(s[1]));
                AddDeleteStep(100);
                return true;
            }
            catch
            {
                AddDeleteStep(100);
                GC.Collect();
            }
            return false;
        }
        public bool PlayNavData(DateTime currentTime)
        {
            try
            {
                //根据历史缓存序号判断当前时间是否在数据缓存中，返回当前缓存序号
                int cacheNum = GetCacheNum(currentTime);//0-不存在 1-存在于第1缓存 2-存在于第2缓存
                string time = currentTime.ToString("yyyyMMddHHmmss");
                switch (cacheNum)
                {
                    case 0://重新加载数据缓存
                        List<string> sqlList = commMariaDB.GetSqlFileByTime(time);//yyyyMMddHHmmss格式字符串
                        UpdateNavData(sqlList);
                        break;
                    case 1://使用第1缓存数据，判断是否触发重新加载第2缓存线程

                        break;
                    case 2://使用第2缓存数据，判断是否触发重新加载第1缓存线程

                        break;
                }
                UpdateMapSource(time);
            }
            catch { }
            return false;
        }
        public bool IsInDate(DateTime dt, DateTime dt1, DateTime dt2)
        {
            return commMariaDB.IsInDate(dt, dt1, dt2);
        }
        public DateTime SetInDate(DateTime dt, DateTime dtmin, DateTime dtmax)
        {
            return commMariaDB.SetInDate(dt, dtmin, dtmax);
        }
        //MAP
        public void ChangeShowType()
        {
            switch (mapModel.ShowType)
            {
                case (int)MtNavDataShowType.RealTime:
                    ChangeShowTypeToPlayBack();
                    break;
                case (int)MtNavDataShowType.PlayBack:
                    ChangeShowTypeToRealTime();
                    break;
            }
        }
        public void ChangeShowTypeTo(MtNavDataShowType type)
        {
            switch (type)
            {
                case MtNavDataShowType.RealTime:
                    ChangeShowTypeToRealTime();
                    break;
                case MtNavDataShowType.PlayBack:
                    ChangeShowTypeToPlayBack();
                    break;
            }
        }

        private void UpdateFilesName(ArrayList arrayList)
        {
            mo.dtNavFilesName.Clear();
            DataRow dr;
            foreach (var arr in arrayList)
            {
                string split = ", ";
                string[] data = Regex.Split(arr.ToString(), split, RegexOptions.IgnoreCase);
                if (data.Count() >= 5)
                {
                    dr = mo.dtNavFilesName.NewRow();
                    dr["id"] = data[0];
                    dr["FilesName"] = data[1].Replace("'", "");
                    dr["Date"] = data[2].Replace("'", "");
                    dr["StartTime"] = data[3].Replace("'", "");
                    dr["EndTime"] = data[4].Replace("'", "");
                    dr["Remark"] = data[5];
                    mo.dtNavFilesName.Rows.Add(dr);
                }
            }
            arrayList.Clear();
            mo.dtNavFilesName.DefaultView.Sort = "Date Desc,StartTime Desc"; //按CreateTime升排序
            mo.dtNavFilesName = mo.dtNavFilesName.DefaultView.ToTable();//返回一个新的DataTable
            GC.Collect();
        }
        private void UpdateAlarmData(ArrayList arrayList)
        {
            mo.dtAlarmData.Clear();
            DataRow dr;
            foreach (var arr in arrayList)
            {
                string split = ", ";
                string[] data = Regex.Split(arr.ToString(), split, RegexOptions.IgnoreCase);
                if (data.Count() >= 5)
                {
                    dr = mo.dtAlarmData.NewRow();
                    dr["ID"] = data[0];
                    dr["VechicleID"] = data[1];
                    dr["AlarmCode"] = data[2];
                    dr["StartTime"] = data[3].Replace("'", "");
                    dr["EndTime"] = data[4].Replace("'", "");
                    mo.dtAlarmData.Rows.Add(dr);
                }
            }
            arrayList.Clear();
            mo.dtAlarmData.DefaultView.Sort = "StartTime Desc"; //按CreateTime升排序
            mo.dtAlarmData = mo.dtAlarmData.DefaultView.ToTable();//返回一个新的DataTable
            GC.Collect();
        }
        private void UpdateNavData(List<string> arrayList)
        {
            //处理sql文件数据
            mo.dtPointCloudData.Clear();
            DataRow dr;
            if (arrayList != null && arrayList.Count > 0)
            {
                foreach (var arr in arrayList)
                {
                    string split = ", ";
                    string[] data = Regex.Split(arr.ToString(), split, RegexOptions.IgnoreCase);
                    if (data.Count() >= 5)
                    {
                        dr = mo.dtPointCloudData.NewRow();
                        dr["id"] = data[0];
                        dr["VehicleID"] = data[1];
                        dr["TotalCount"] = data[2];
                        dr["CurrentCount"] = data[3];
                        dr["XArray"] = data[4].Replace("'", "");
                        dr["YArray"] = data[5].Replace("'", "");
                        dr["CurrentTime"] = data[6].Replace("'", "");
                        mo.dtPointCloudData.Rows.Add(dr);
                    }
                }
                arrayList.Clear();
            }
            mo.dtPointCloudData.DefaultView.Sort = "CurrentTime Asc"; //按CreateTime升排序
            mo.dtPointCloudData = mo.dtPointCloudData.DefaultView.ToTable();//返回一个新的DataTable
            GC.Collect();
        }
        private void UpdateMapSource(string currentTime)
        {
            int index = -1;
            if (currentTime == "first")
            {
                index = 0;
            }
            else
            {
                //根据当前时间匹配数据
                //遍历list数据，匹配每两条数据的时间是否包含当前时间，是则返回索引，直到返回最后一条数据
                DateTime current = Convert.ToDateTime(commMariaDB.StringToDate(currentTime));
                for (int i = 0; i < mo.dtPointCloudData.Rows.Count; i++)
                {
                    DateTime dt = Convert.ToDateTime(mo.dtPointCloudData.Rows[i]["CurrentTime"]);
                    if (i != mo.dtPointCloudData.Rows.Count - 1 && current < dt)
                        break;
                    else
                        index = i;
                }
            }
            int count = (int)mo.dtPointCloudData.Rows[index]["CurrentCount"];
            int totalCount = (int)mo.dtPointCloudData.Rows[index]["TotalCount"];
            float[] Xarray = mo.dtPointCloudData.Rows[index]["XArray"].ToString().Split(' ').Select(x => Convert.ToSingle(x)).ToArray();
            float[] Yarray = mo.dtPointCloudData.Rows[index]["YArray"].ToString().Split(' ').Select(x => Convert.ToSingle(x)).ToArray();
            mapModel.MapPoints = new GraphPoint[count];
            for (int i = 0; i < count; i++)
            {
                mapModel.MapPoints[i] = new GraphPoint(Xarray[i], Yarray[i]);
            }
            if (totalCount != mapModel.Count)
            {
                if (totalCount >= 0 && totalCount < mapModel.MaxCount)
                {
                    mapModel.Count = totalCount;
                }
            }
            GC.Collect();
        }
        private int GetCacheNum(DateTime currentTime)
        {
            int num = 0;
            try
            {
                //根据历史缓存编号，优先判断对应缓存表
                //检测当前时间是否在对应缓存表时间段内
                //如不存在则继续检测另一缓存表
                //如都不存在返回0
                DateTime start = Convert.ToDateTime(mo.dtPointCloudData.Rows[0]["CurrentTime"]);
                DateTime end = Convert.ToDateTime(mo.dtPointCloudData.Rows[mo.dtPointCloudData.Rows.Count - 1]["CurrentTime"]);
                if (IsInDate(currentTime, start, end))
                {
                    num = 1;
                }
            }
            catch { }
            return num;
        }
        private void AddDownloadStep(int n)
        {
            mo.navDataDownloadStep = n;
        }
        private void AddDeleteStep(int n)
        {
            mo.navDataDeleteStep = n;
        }
        private string GetDirPath()
        {
            string strDir = string.Format(@"{0}\CacheData\simNavData", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
            DirectoryInfo dir = new DirectoryInfo(strDir);
            if (!dir.Exists)
            {
                CreateDir(strDir);
            }
            return strDir;
        }
        private void CreateDir(string directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            dirInfo.Create();
        }
        private void ChangeShowTypeToRealTime()
        {
            mapModel.ShowType = (int)MtNavDataShowType.RealTime;
        }
        private void ChangeShowTypeToPlayBack()
        {
            mapModel.ShowType = (int)MtNavDataShowType.PlayBack;
        }
        private void SetNavDataTime()
        {
            mapModel.StartTime = commMariaDB.GetStartEndTime(1);
            mapModel.EndTime = commMariaDB.GetStartEndTime(2);
            mapModel.PlayTime = mapModel.StartTime;
        }

        #region MariaDB
        private DataTable GetNavData()
        {
            return SelectNavCacheTable();
        }
        //检查缓存表
        private bool CheckNavCacheTableExist(string tableName)
        {
            bool result = false;
            try
            {
                string sqlString = string.Format(" SELECT table_name FROM information_schema.tables WHERE table_name = '{0}' ", tableName);
                DataTable table = commMariaDB.DoSelect(sqlString);
                result = table.Rows.Count > 0;
            }
            catch { }
            return result;
        }
        //新建缓存表
        private bool CreateNewNavCacheTable(string tableName)
        {
            bool result = false;
            try
            {
                string sqlString = String.Format(@"CREATE TABLE `{0}` (
	`ID` INT(11) NULL DEFAULT NULL,
	`Remark1` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb3_general_ci',
	`Remark2` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb3_general_ci'
)
COMMENT='测试用，无意义'
COLLATE='utf8mb3_general_ci'
ENGINE=MyISAM", tableName);
                result = commMariaDB.DoExecuteNonQuery(sqlString);
            }
            catch { }
            return result;
        }
        //删除缓存表
        private bool DropNavCacheTableData(string tableName)
        {
            bool result = false;
            try
            {
                string sqlString = string.Format(" DROP TABLE {0} ", tableName);
                result = commMariaDB.DoExecuteNonQuery(sqlString);
            }
            catch { }
            return result;
        }
        //缓存表添加数据
        private bool InsertNavCacheTable()
        {
            try { }
            catch { }
            return false;
        }
        //查询缓存表
        private DataTable SelectNavCacheTable()
        {
            DataTable table = new DataTable();
            try
            {
                table = commMariaDB.DoSelect(" SELECT * FROM test ");
            }
            catch { }
            return table;
        }
        private bool DeleteNavData(string tableName, int ID)
        {
            string sqlString = string.Format("delete from {0} where id = {1}", tableName, ID);
            bool result = commMariaDB.DoExecuteNonQuery(sqlString);
            return result;

        }
        //CSV导出
        private bool ExportCSVFile()
        {
            try { }
            catch { }
            return false;
        }
        //CSV导入
        private bool InportCSVFile()
        {
            try { }
            catch { }
            return false;
        }
        #endregion

        public string ConvertWeekToChn(int week)
        {
            string chnWeek = "";
            switch (week)
            {
                case 0:
                    chnWeek = "星期日";
                    break;
                case 1:
                    chnWeek = "星期一";
                    break;
                case 2:
                    chnWeek = "星期二";
                    break;
                case 3:
                    chnWeek = "星期三";
                    break;
                case 4:
                    chnWeek = "星期四";
                    break;
                case 5:
                    chnWeek = "星期五";
                    break;
                case 6:
                    chnWeek = "星期六";
                    break;
                default:
                    chnWeek = "星期日";
                    break;
            }
            return chnWeek;
        }
    }
}
