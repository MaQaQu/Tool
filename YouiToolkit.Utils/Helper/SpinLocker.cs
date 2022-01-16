using System;
using System.Diagnostics;
using System.Threading;
using YouiToolkit.Logging;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 自选锁柜
    /// </summary>
    public class SpinLocker
    {
        /// <summary>
        /// 自旋锁
        /// </summary>
        private SpinLock spinLock = new SpinLock();

        /// <summary>
        /// 获取锁异常的默认处理
        /// </summary>
        private Action<Exception, bool> actionCatchDefault = (e, taken) => Logger.Info(e.ToString());

        /// <summary>
        /// 获取自旋锁并执行要求方法，最终会自动解锁
        /// </summary>
        /// <param name="actionTry">要求方法</param>
        /// <param name="actionCatch">出错处理方法＜异常, 自旋锁标志＞</param>
        /// <param name="millisecondsTimeout">获取自旋锁超时时间</param>
        /// <param name="actionFinally">最终处理方法</param>
        public void Lock(Action actionTry, Action<Exception, bool> actionCatch = null, int? millisecondsTimeout = null, Action actionFinally = null)
        {
            // 初始化自旋锁标志
            bool spinLockTaken = false;
            DateTime dateTime = DateTime.Now;

            try
            {
                // 进入自旋阻塞直到获取锁
                if (millisecondsTimeout != null)
                {
                    while (true)
                    {
                        try
                        {
                            // 有设定超时时间：超时后触发异常
                            spinLock.TryEnter(ref spinLockTaken);
                        }
                        catch
                        {
                            // 尝试获取锁一次失败
                        }

                        if (spinLockTaken)
                        {
                            break;
                        }
                        else if ((DateTime.Now - dateTime).TotalMilliseconds >= millisecondsTimeout)
                        {
                            throw new TimeoutException("Try enter spin lock timeout.");
                        }
                    }
                }
                else
                {
                    // 没有设定超时时间：永久等待
                    // ※意味着可能会造成死锁
                    spinLock.Enter(ref spinLockTaken);
                }

                // 执行要求方法
                actionTry?.Invoke();
            }
            catch (Exception e)
            {
                // 初始化异常处理方法
                if (actionCatch == null)
                {
                    actionCatch = actionCatchDefault;
                }

                // 执行异常处理方法
                actionCatch?.Invoke(e, spinLockTaken);
            }
            finally
            {
                // 执行最终处理方法
                try
                {
                    actionFinally?.Invoke();
                }
                finally
                {
                    try
                    {
                        // 检查获取锁情况并释放锁
                        if (spinLockTaken)
                        {
                            spinLock.Exit(false);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.ToString());
                    }
                    finally
                    {
                        // 初始化自旋锁标志
                        spinLockTaken = false;
                    }
                }
            }
        }
    }
}
