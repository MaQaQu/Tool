using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Models
{
    public class PageMtChartRenderModel_Speed
    {
        private static PageMtChartRenderModel_Speed _PageMtChartRenderModel_Speed = null;
        static PageMtChartRenderModel_Speed()
        {
            _PageMtChartRenderModel_Speed = new PageMtChartRenderModel_Speed();
        }
        public static PageMtChartRenderModel_Speed CreateInstance()
        {
            return _PageMtChartRenderModel_Speed;
        }
        private PageMtChartRenderModel_Speed()
        {
            Speed = "";
            Speed_X = "";
            Speed_Y = "";
            Speed_W = "";
            DateTime_Current = DateTime.MinValue;
        }
        public string Speed { get; set; }
        public string Speed_X { get; set; }
        public string Speed_Y { get; set; }
        public string Speed_W { get; set; }
        public DateTime DateTime_Current { get; set; }
    }
}
