using System;
using System.Collections.Concurrent;

namespace YouiToolkit.Logging
{
    /// <summary>
    /// 线程安全队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ConcurrentQueueEx<T> : ConcurrentQueue<T>
    {
        /// <summary>
        /// 清除数据
        /// </summary>
        /// <returns>清除个数</returns>
        public int Clear()
        {
            int cleanCount = default;
            int commandCount = Count;

            if (commandCount > 0)
            {
                for (int i = default; i < commandCount; i++)
                {
                    if (Count > 0)
                    {
                        try
                        {
                            if (TryDequeue(out T _))
                            {
                                cleanCount++;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Warn(e.ToString());
                        }
                    }
                }
            }
            return cleanCount;
        }
    }
}
