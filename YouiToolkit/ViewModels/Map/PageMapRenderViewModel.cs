using GalaSoft.MvvmLight;

namespace YouiToolkit.ViewModels
{
    public class PageMapRenderViewModel : ViewModelBase
    {
    }

    /// <summary>
    /// 立上目标
    /// </summary>
    public enum MapRenderReloadTarget
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,

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
