using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using YouiToolkit.Design;
using YouiToolkit.Logging;
using YouiToolkit.Utils;

namespace YouiToolkit
{
    internal class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            // 输出系统基本信息
            Logger.Info($"Main thread started OS {AppEnv.OSVersion}. Startup Path: \"{AppEnv.StartupPath}\". Current Directory: \"{Environment.CurrentDirectory}\".");

            // 初始化程序单例名：服务于后续各类检查动作
            AppWrapper.InitInstanceName(MethodBase.GetCurrentMethod().DeclaringType);

            // 检查启动路劲是否与运行文件所在路劲一致
            if (!AppEnv.IsCurrentPathEqualStartupPath)
            {
                try
                {
                    // 工作路劲切换到程序所在目录
                    AppEnv.ChangeDirectoryToStartupPath();
                }
                catch (Exception e)
                {
                    Logger.Fatal($"Change Directory to Startup Path failed. Detail: '{e}'.");
                    Environment.ExitCode = (int)EExitCode.eFailed;
                    return;
                }
            }

            // 检查程序是否允许多开
            if (!AppWrapper.IsMutilInstanceEnabled && AppWrapper.IsAnotherInstanceStarted)
            {
                Logger.Error("Another instance was already running.");

                // 连接到跨进程通讯IPC通道
                AppIpcObject client = AppIpc.CreateIpcClientChannel(AppWrapper.InstanceName);

                // 恢复并激活已启动的窗体
                client?.RestoreAndActivate();

                Environment.ExitCode = (int)EExitCode.eMultiInstance;
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                // 创建跨进程通讯IPC通道
                AppIpc.CreateIpcServerChannel(AppWrapper.InstanceName);
            }

            // 显示启动界面
            //SplashScreen.Show();

            App app = new App();
            app.Run();
        }
    }
}
