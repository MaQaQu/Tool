using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Models.Maintain
{
    public class PageMaintainModel : ViewModelBase
    {
        public bool videoPlayingFlag { get; set; }
        public PageMaintainModel()
        {
            videoPlayingFlag = false;
        }
    }
}
