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
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public PageRobotViewModel PageRobot => ServiceLocator.Current.GetInstance<PageRobotViewModel>();
        public PageMapViewModel PageMap => ServiceLocator.Current.GetInstance<PageMapViewModel>();
        public PageMapRenderViewModel PageMapRender => ServiceLocator.Current.GetInstance<PageMapRenderViewModel>();
        public PageMapListViewModel PageMapList => ServiceLocator.Current.GetInstance<PageMapListViewModel>();
        public PageAvoidObstacleViewModel PageAvoidObstacle => ServiceLocator.Current.GetInstance<PageAvoidObstacleViewModel>();
        public PageMaintainViewModel PageMaintain => ServiceLocator.Current.GetInstance<PageMaintainViewModel>();

        public static void Cleanup()
        {
        }
    }
}
