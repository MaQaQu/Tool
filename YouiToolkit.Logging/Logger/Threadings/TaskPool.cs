using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YouiToolkit.Logging
{
    public class TaskPool
    {
        #region [私有 字段]
        /// <summary>
        /// 任务字典
        /// </summary>
        private static readonly Dictionary<int, TaskInfo> tasks = new Dictionary<int, TaskInfo>();

        /// <summary>
        /// 线程列表
        /// </summary>
        private static List<Thread> threads = new List<Thread>();
        #endregion

        #region [私有 TASK方法]
        /// <summary>
        /// 确认TID是否存在
        /// </summary>
        /// <param name="tid">TID</param>
        /// <returns>是否存在</returns>
        private static bool ContainsTID(int tid)
        {
            if (tid != int.MinValue)
            {
                lock (tasks)
                {
                    if (tasks.ContainsKey(tid))
                    {
                        Logger.Warn($"Task start failed, because TID[{tid}] is existed.");
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 创建任务信息
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="cancellation">取消对象</param>
        /// <param name="cycle">周期</param>
        /// <returns>任务信息</returns>
        private static TaskInfo CreateTaskInfo(Task task, CancellationTokenSource cancellation, int cycle = default)
        {
            TaskInfo taskInfo = new TaskInfo();

            taskInfo.Cycle = cycle;

            if (task != null)
            {
                taskInfo.Task = task;
                taskInfo.CancellationTokenSource = cancellation;

                lock (tasks)
                {
                    tasks.Add(task.Id, taskInfo);
                }
                cancellation.Token.Register(() =>
                {
                    lock (tasks)
                    {
                        tasks.Remove(task.Id);
                    }
                });
            }
            return taskInfo;
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="action">运行方法</param>
        /// <param name="taskInfo">返回运行信息</param>
        /// <returns>任务</returns>
        private static Task Run(Action action, out TaskInfo taskInfo)
        {
            CancellationTokenSource cancellation = new CancellationTokenSource();
            Task task = Task.Run(action, cancellation.Token);

            taskInfo = CreateTaskInfo(task, cancellation);
            return task;
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="action">运行方法</param>
        /// <param name="cycle">运行周期</param>
        /// <param name="taskInfo">返回运行信息</param>
        /// <returns>任务</returns>
        private static Task Run(Action action, int cycle, out TaskInfo taskInfo)
        {
            CancellationTokenSource cancellation = new CancellationTokenSource();
            Task task = null;

            void TaskFunc()
            {
                try
                {
                    while (!cancellation.IsCancellationRequested)
                    {
                        action();

                        Thread.Sleep(cycle);
                    }

                    Logger.Debug($"Task[TID={task?.Id.ToString()}] was canceled.");
                }
                catch (AggregateException e)
                {
                    e.Handle(p => p is OperationCanceledException);

                    lock (tasks)
                    {
                        if (tasks.ContainsKey((int)task?.Id))
                        {
                            tasks[(int)task?.Id].AggregateException = e;
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (tasks)
                    {
                        if (tasks.ContainsKey((int)task?.Id))
                        {
                            tasks[(int)task?.Id].ErrorMessage = e.ToString();
                            tasks[(int)task?.Id].HasError = true;
                        }
                    }
                    Logger.Debug(e.ToString());
                }
            };

            task = Task.Run(TaskFunc, cancellation.Token);
            taskInfo = CreateTaskInfo(task, cancellation, cycle);

            return task;
        }
        #endregion

        #region [公有 TASK方法]
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="func">启动方法</param>
        /// <param name="msDelay">启动周期（≤0时为单任务）</param>
        /// <param name="tid">重复任务TID</param>
        /// <returns>任务TID</returns>
        /// <exception cref="Exception">任务线程初始化失败</exception>
        public static int Start(Action func, int cycle = default, int tid = int.MinValue)
        {
            // 防止重复启动先确认TID是否存在
            if (ContainsTID(tid))
            {
                return tid;
            }

            Task task = null;

            if (cycle <= default(int))
            {
                task = Run(func, out _);
            }
            else
            {
                task = Run(func, cycle, out _);
            }

            if (task == null)
            {
                throw new Exception("任务线程初始化失败。");
            }

            return task.Id;
        }

        /// <summary>
        /// 启动任务（带参无返回值）
        /// </summary>
        /// <param name="func">启动方法</param>
        /// <param name="msDelay">启动周期（≤0时为单任务）</param>
        /// <param name="tid">重复任务TID</param>
        /// <returns>任务TID</returns>
        /// <exception cref="Exception">任务线程初始化失败</exception>
        public static int Start<T>(Action<T> func, T param, int cycle = default, int tid = int.MinValue)
        {
            return Start(DelegateExtra.ToAction(func, param), cycle, tid);
        }

        /// <summary>
        /// 取消任务（仅仅向取消任务代理请求取消）
        /// </summary>
        /// <param name="taskId">任务TID</param>
        public static void Stop(int taskId)
        {
            lock (tasks)
            {
                if (tasks.ContainsKey(taskId))
                {
                    Task task = tasks[taskId].Task;
                    CancellationTokenSource tokenSrc = tasks[taskId].CancellationTokenSource;

                    if (task != null && tokenSrc != null)
                    {
                        if (tokenSrc.Token.CanBeCanceled)
                        {
                            tokenSrc.Cancel();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册取消任务回调方法
        /// </summary>
        /// <param name="taskId">任务TID</param>
        /// <param name="callback">回调方法</param>
        public static void CancelRegister(int taskId, Action callback)
        {
            lock (tasks)
            {
                if (tasks.ContainsKey(taskId))
                {
                    tasks[taskId].CancellationTokenSource.Token.Register(callback);
                }
            }
        }

        /// <summary>
        /// 注册取消任务回调方法
        /// </summary>
        /// <param name="taskId">任务TID</param>
        /// <param name="callback">回调方法</param>
        /// <param name="state">回调方法参数</param>
        public static void CancelRegister(int taskId, Action<object> callback, object state)
        {
            lock (tasks)
            {
                if (tasks.ContainsKey(taskId))
                {
                    tasks[taskId].CancellationTokenSource.Token.Register(callback, state);
                }
            }
        }

        /// <summary>
        /// 获取取消任务代理
        /// </summary>
        /// <param name="taskId">任务TID</param>
        /// <exception cref="ArgumentException">无效的TID参数</exception>
        public static CancellationToken CancelToken(int taskId)
        {
            lock (tasks)
            {
                if (tasks.ContainsKey(taskId))
                {
                    return tasks[taskId].CancellationTokenSource.Token;
                }
            }

            throw new ArgumentException("无效的TID参数");
        }

        /// <summary>
        /// 取得任务
        /// </summary>
        /// <param name="taskId">任务TID</param>
        public static Task GetTask(int taskId)
        {
            lock (tasks)
            {
                if (tasks.ContainsKey(taskId))
                {
                    return tasks[taskId].Task;
                }
            }
            return null;
        }

        /// <summary>
        /// 阻塞等待任务
        /// </summary>
        /// <param name="taskId">任务TID</param>
        /// <param name="millisecondsTimeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static void Wait(int taskId, int millisecondsTimeout)
        {
            try
            {
                lock (tasks)
                {
                    if (tasks.ContainsKey(taskId))
                    {
                        Task task = tasks[taskId].Task;

                        if (task != null)
                        {
                            task.Wait(millisecondsTimeout);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

        /// <summary>
        /// 销毁任务（只有正常取消任务后生效）
        /// </summary>
        /// <param name="taskId">任务TID</param>
        public static bool TaskDispose(int taskId)
        {
            try
            {
                lock (tasks)
                {
                    if (tasks.ContainsKey(taskId))
                    {
                        Task task = tasks[taskId].Task;

                        if (task != null)
                        {
                            task.Dispose();
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }

            return false;
        }
        #endregion

        #region [私有 Thread方法]
        /// <summary>
        /// 启动线程
        /// </summary>
        /// <param name="func">方法</param>
        /// <param name="isBackground">是否后台运行</param>
        /// <returns>线程</returns>
        private static Thread Run(ThreadStart func, bool isBackground)
        {
            Thread thread = new Thread(func);

            thread.IsBackground = isBackground;
            thread.Start();

            lock (threads)
            {
                threads.Add(thread);
            }
            return thread;
        }
        #endregion

        #region [公有 Thread方法]
        /// <summary>
        /// 启动线程
        /// </summary>
        /// <param name="func">方法</param>
        /// <param name="isBackground">是否后台运行</param>
        /// <returns>线程</returns>
        public static Thread Start(Action func, bool isBackground)
        {
            return Run(new ThreadStart(func), isBackground);
        }

        /// <summary>
        /// 启动线程（带参）
        /// </summary>
        /// <param name="func">带参方法</param>
        /// <param name="param"></param>
        /// <param name="isBackground">是否后台运行</param>
        /// <returns>线程</returns>
        public static Thread Start<T>(Action<T> func, T param, bool isBackground)
        {
            return Run(new ThreadStart(DelegateExtra.ToAction(func, param)), isBackground);
        }

        /// <summary>
        /// 终止线程
        /// ※仅仅对于调用托管的代码好使
        /// ※所以无法对非托管代码无法进行GC回收
        /// ※最终可能发生内存泄露以及对象死锁
        /// </summary>
        /// <param name="thread">线程</param>
        public static void Abort(Thread thread)
        {
            try
            {
                if (thread != null && thread.IsAlive)
                {
                    thread?.Abort();
                }

                lock (threads)
                {
                    threads.Remove(thread);
                }
                thread = null;
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
            }
        }

        /// <summary>
        /// 阻塞等待线程
        /// </summary>
        /// <param name="thread">线程</param>
        /// <param name="millisecondsTimeout">超时时间（毫秒）</param>
        public static void Wait(Thread thread, int millisecondsTimeout)
        {
            thread?.Join(millisecondsTimeout);
        }
        #endregion
    }
}
