using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using YouiToolkit.Models.Maintain;

namespace YouiToolkit.ViewModels
{
    public class PageMaintainViewModel : ViewModelBase
    {
        /// <summary>
        /// 画直线
        /// </summary>
        public PageMaintainModel maintainModel { get; set; }

        public PageMaintainViewModel()
        {
            maintainModel = new PageMaintainModel();
        }
    }
}
