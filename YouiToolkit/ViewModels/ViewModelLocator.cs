using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace YouiToolkit.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
            }
            else
            {
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PageRobotViewModel>();
            SimpleIoc.Default.Register<PageMapViewModel>();
            SimpleIoc.Default.Register<PageMapRenderViewModel>();
            SimpleIoc.Default.Register<PageMapListViewModel>();
            SimpleIoc.Default.Register<PageAvoidObstacleViewModel>();
            SimpleIoc.Default.Register<PageMaintainViewModel>();
            SimpleIoc.Default.Register<PageMtChartRenderViewModel>();
            SimpleIoc.Default.Register<PageMtMapRenderViewModel>();
            SimpleIoc.Default.Register<PageMtChartRenderViewModel_Angle>();
            SimpleIoc.Default.Register<PageMtChartRenderViewModel_Navigation>();
            SimpleIoc.Default.Register<PageMtChartRenderViewModel_Speed>();
            SimpleIoc.Default.Register<PageMtChartRenderViewModel_Wheel>();
            SimpleIoc.Default.Register<PageMtChartRenderViewModel_Wlan>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public PageRobotViewModel PageRobot => ServiceLocator.Current.GetInstance<PageRobotViewModel>();
        public PageMapViewModel PageMap => ServiceLocator.Current.GetInstance<PageMapViewModel>();
        public PageMapRenderViewModel PageMapRender => ServiceLocator.Current.GetInstance<PageMapRenderViewModel>();
        public PageMapListViewModel PageMapList => ServiceLocator.Current.GetInstance<PageMapListViewModel>();
        public PageAvoidObstacleViewModel PageAvoidObstacle => ServiceLocator.Current.GetInstance<PageAvoidObstacleViewModel>();
        public PageMaintainViewModel PageMaintain => ServiceLocator.Current.GetInstance<PageMaintainViewModel>();
        public PageMtChartRenderViewModel PageMtChartRender => ServiceLocator.Current.GetInstance<PageMtChartRenderViewModel>();
        public PageMtMapRenderViewModel PageMtMapRender => ServiceLocator.Current.GetInstance<PageMtMapRenderViewModel>();
        public PageMtChartRenderViewModel_Angle PageMtChartRender_Angle => ServiceLocator.Current.GetInstance<PageMtChartRenderViewModel_Angle>();
        public PageMtChartRenderViewModel_Navigation PageMtChartRender_Navigation => ServiceLocator.Current.GetInstance<PageMtChartRenderViewModel_Navigation>();
        public PageMtChartRenderViewModel_Speed PageMtChartRender_Speed => ServiceLocator.Current.GetInstance<PageMtChartRenderViewModel_Speed>();
        public PageMtChartRenderViewModel_Wheel PageMtChartRender_Wheel => ServiceLocator.Current.GetInstance<PageMtChartRenderViewModel_Wheel>();
        public PageMtChartRenderViewModel_Wlan PageMtChartRender_Wlan => ServiceLocator.Current.GetInstance<PageMtChartRenderViewModel_Wlan>();

        public static void Cleanup()
        {
        }
    }
}
