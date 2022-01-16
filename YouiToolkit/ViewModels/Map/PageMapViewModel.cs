using GalaSoft.MvvmLight;

namespace YouiToolkit.ViewModels
{
    public class PageMapViewModel : ViewModelBase
    {
        public PageMapViewModel()
        {
        }
    }

    /// <summary>
    /// 立上目标
    /// </summary>
    public enum MapReloadTarget
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,

        /// <summary>
        /// 地图列表（默认）
        /// </summary>
        MapList,

        /// <summary>
        /// 地图录制
        /// </summary>
        MapCapture,

        /// <summary>
        /// 地图补建
        /// </summary>
        MapEdit,
    }
}
