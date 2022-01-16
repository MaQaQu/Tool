using System;
using System.Threading;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 自旋等待器
    /// </summary>
    public class SpinWaiter
    {
        /// <summary>
        /// 自旋
        /// </summary>
        /// <param name="condition">满足条件</param>
        /// <param name="millisecondsTimeout">超时时间(ms)</param>
        /// <returns>条件满足结果</returns>
        public static bool SpinUntil(Func<bool> condition, int? millisecondsTimeout = null)
        {
            // 再初始化超时时间参数
            millisecondsTimeout = millisecondsTimeout ?? -1;
            if (millisecondsTimeout < -1)
            {
                millisecondsTimeout = -1;
            }

            // 再初始化满足条件参数
            if (condition == null)
            {
                // 如果有设定超时时间则默认使用死循环自旋
                if (millisecondsTimeout == -1 || millisecondsTimeout > 0)
                {
                    condition = () => false;
                }
                else
                {
                    throw new ArgumentNullException(nameof(condition));
                }
            }

            // 开始自旋直到条件满足或超时
            // 返回值：条件在超时内满足则为真，否则为假
            return SpinWait.SpinUntil(condition, (int)millisecondsTimeout);
        }
    }
}
