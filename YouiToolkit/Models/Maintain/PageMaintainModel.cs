using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Models
{
    public sealed class PageMaintainModel : ViewModelBase
    {
        private static PageMaintainModel _PageMaintainModel = null;
        static PageMaintainModel()
        {
            _PageMaintainModel = new PageMaintainModel();
        }
        private PageMaintainModel()
        {
            videoPlayingFlag = false;
            navDataDownloadStep = 0;
            downloadingFlag = false;
            navDataDeleteStep = 0;
            deletingFlag = false;
            overOperationFlag = false;
            InitDtNavFilesName();
            InitDtNavData();
            InitDtPointCloudData();
        }
        public static PageMaintainModel CreateInstance()
        {
            return _PageMaintainModel;
        }
        private void InitDtNavFilesName()
        {
            dtNavFilesName = new DataTable("simulatenavfilesname");
            //dt新增列
            dtNavFilesName.Columns.Add("ID", Type.GetType("System.Int32"));
            dtNavFilesName.Columns.Add("FilesName", Type.GetType("System.String"));
            dtNavFilesName.Columns.Add("Date", Type.GetType("System.String"));
            dtNavFilesName.Columns.Add("StartTime", Type.GetType("System.String"));
            dtNavFilesName.Columns.Add("EndTime", Type.GetType("System.String"));
            dtNavFilesName.Columns.Add("Remark", Type.GetType("System.String"));
        }
        private void InitDtNavData()
        {
            dtNavData = new DataTable("simulatednavdata");
            DataColumn dc = null;
            //dt新增列
            dc = dtNavData.Columns.Add("ID", Type.GetType("System.Int32")); //ID
            dc = dtNavData.Columns.Add("VehicleID", Type.GetType("System.Int32")); //VehicleID
            dc = dtNavData.Columns.Add("CurrentTime", Type.GetType("System.DateTime")); //CurrentTime
            dc = dtNavData.Columns.Add("NavData", Type.GetType("System.String")); //NavData
            dc = dtNavData.Columns.Add("Remark", Type.GetType("System.String")); //NavData
        }
        private void InitDtPointCloudData()
        {
            dtPointCloudData = new DataTable("simulatednavdata");
            DataColumn dc = null;
            //dt新增列
            dc = dtPointCloudData.Columns.Add("ID", Type.GetType("System.Int32"));
            dc = dtPointCloudData.Columns.Add("VehicleID", Type.GetType("System.Int32"));
            dc = dtPointCloudData.Columns.Add("TotalCount", Type.GetType("System.Int32"));
            dc = dtPointCloudData.Columns.Add("CurrentCount", Type.GetType("System.Int32"));
            dc = dtPointCloudData.Columns.Add("XArray", Type.GetType("System.String"));
            dc = dtPointCloudData.Columns.Add("YArray", Type.GetType("System.String"));
            dc = dtPointCloudData.Columns.Add("CurrentTime", Type.GetType("System.DateTime"));
        }

        public bool videoPlayingFlag { get; set; }
        public int navDataDownloadStep { get; set; }
        public bool downloadingFlag { get; set; }
        public int navDataDeleteStep { get; set; }
        public bool deletingFlag { get; set; }
        public bool overOperationFlag { get; set; }
        public DataTable dtNavData { get; set; }
        public DataTable dtPointCloudData { get; set; }
        public DataTable dtNavFilesName { get; set; }
        public string strNavDataCacheFilePath { get; set; }
    }
}
