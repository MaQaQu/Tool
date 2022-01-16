using System;
using System.Threading.Tasks;

namespace YouiToolkit.Logging
{
    /// <summary>
    /// 周期队列
    /// </summary>
    /// <typeparam name="T">队列数据类型</typeparam>
    public class CyclicQueue<T>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 任务ID
        /// </summary>
        public int TID { get; private set; } = int.MinValue;

        /// <summary>
        /// 任务
        /// </summary>
        public Task Task => TaskPool.GetTask(TID);

        /// <summary>
        /// 出队周期
        /// ※大于等于1才会周期触发
        /// </summary>
        public int Cycle { get; set; } = 10;

        /// <summary>
        /// 队列数量
        /// </summary>
        public int Count => queue.Count;

        /// <summary>
        /// (一个周期内)出队开始事件
        /// </summary>
        public event Action DequeueStarted = null;

        /// <summary>
        /// 出队事件
        /// </summary>
        public event Action<T> Dequeued = null;

        /// <summary>
        /// (一个周期内)出队完了事件
        /// </summary>
        public event Action DequeueCompleted = null;

        /// <summary>
        /// 队列（拥有高效并发与线程安全特效）
        /// </summary>
        private readonly ConcurrentQueueEx<T> queue = new ConcurrentQueueEx<T>();

        #region [公有 方法]
        /// <summary>
        /// 构造方法
        /// </summary>
        public CyclicQueue(string name = null)
        {
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// 启动周期任务
        /// </summary>
        public void Start()
        {
            try
            {
                TID = TaskPool.Start(Dequeue, Cycle, TID);
            }
            catch (Exception e)
            {
                Logger.Fatal(e.ToString());
            }
        }

        /// <summary>
        /// 停止周期任务
        /// </summary>
        public void Stop()
        {
            TaskPool.Stop(TID);
        }

        /// <summary>
        /// 注册取消周期任务回调方法
        /// </summary>
        /// <param name="callback">回调方法</param>
        public void CancelRegister(Action callback)
        {
            TaskPool.CancelRegister(TID, callback);
        }

        /// <summary>
        /// 出队
        /// </summary>
        public void Dequeue()
        {
            if (queue.Count > 0)
            {
                DequeueStarted?.Invoke();

                for (int i = default; i < queue.Count; i++)
                {
                    if (queue.TryDequeue(out T command))
                    {
                        Dequeued?.Invoke(command);
                    }
                }

                DequeueCompleted?.Invoke();
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        public void Enqueue(T command)
        {
            try
            {
                queue.Enqueue(command);
            }
            catch (Exception e)
            {
                Logger.Fatal(e.ToString());
            }
        }

        /// <summary>
        /// 清除所有队列数据
        /// </summary>
        /// <returns>清除个数</returns>
        public int Clear() => queue.Clear();
        #endregion
    }
}
