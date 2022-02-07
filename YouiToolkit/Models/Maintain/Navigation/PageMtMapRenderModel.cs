using SuperMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Models
{
    public class PageMtMapRenderModel
    {
        private static PageMtMapRenderModel _PageMtMapRenderModel = null;
        static PageMtMapRenderModel()
        {
            _PageMtMapRenderModel = new PageMtMapRenderModel();
        }
        private PageMtMapRenderModel()
        {
            MapPoints = new GraphPoint[MaxCount];
            ShowType = (int)MtNavDataShowType.RealTime;
        }
        public static PageMtMapRenderModel CreateInstance()
        {
            return _PageMtMapRenderModel;
        }
        public int MaxCount { get; set; } = ushort.MaxValue;
        public GraphPoint[] MapPoints { get; set; }
        public GraphPoint[] MapPointsPrior { get; set; } = null;
        public int Count { get; set; } = 0;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PlayTime { get; set; }
        public int ShowType { get; set; }
    }
    public enum MtNavDataDownloadConfirmType
    {
        downloadConfirm = 1,
        deleteConfirm = 2,
    }
    public enum MtNavDataShowType
    {
        RealTime = 0,
        PlayBack = 1,
    }
}
