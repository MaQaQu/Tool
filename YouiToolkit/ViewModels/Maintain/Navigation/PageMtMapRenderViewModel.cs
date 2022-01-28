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

        //保存导航文件数据
        public bool DownloadNavData(string filename)
        {
            try
            {
                //接收导航数据，存放在缓存文件夹中(只保留1个)
                mo.strNavDataCacheFilePath = GetDirPath() + "\\" + filename;
                AddDownloadStep(25);
                //从缓存表中提取数据
                ArrayList sqlArrayList = commMariaDB.GetSqlFile(mo.strNavDataCacheFilePath);
                AddDownloadStep(50);
                //将缓存表数据更新到datatable
                UpdateNavData(sqlArrayList);
                AddDownloadStep(75);
                //更新画布数据源
                UpdateMapSource();
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
                ArrayList sqlArrayList = commMariaDB.GetSqlFile(fileName);
                UpdateNavData(sqlArrayList);
                UpdateMapSource();
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
                System.Threading.Thread.Sleep(500);//测试效果用
                AddDeleteStep(50);
                File.Delete(GetDirPath() + "\\" + filename);
                System.Threading.Thread.Sleep(500);//测试效果用
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
        private void UpdateNavData(ArrayList arrayList)
        {
            //处理sql文件数据
            mo.dtPointCloudData.Clear();
            DataRow dr;
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
            mo.dtPointCloudData.DefaultView.Sort = "CurrentTime Asc"; //按CreateTime升排序
            mo.dtPointCloudData = mo.dtPointCloudData.DefaultView.ToTable();//返回一个新的DataTable
            GC.Collect();
        }
        private void UpdateMapSource()
        {
            int count = (int)mo.dtPointCloudData.Rows[0]["CurrentCount"];
            int totalCount = (int)mo.dtPointCloudData.Rows[0]["TotalCount"];
            float[] Xarray = mo.dtPointCloudData.Rows[0]["XArray"].ToString().Split(' ').Select(x => Convert.ToSingle(x)).ToArray();
            float[] Yarray = mo.dtPointCloudData.Rows[0]["YArray"].ToString().Split(' ').Select(x => Convert.ToSingle(x)).ToArray();
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
            ChangeShowTypeToPlayBack();
            GC.Collect();
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
