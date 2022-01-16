using System;
using System.Threading;
using System.Threading.Tasks;

namespace YouiToolkit.Logging
{
    /// <summary>
    /// 任务信息
    /// </summary>
    internal class TaskInfo
    {
        /// <summary>
        /// 任务对象
        /// </summary>
        public Task Task { get; set; }

        /// <summary>
        /// 取消任务代理
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// 任务周期（毫秒）
        /// </summary>
        public int Cycle { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 错误标志
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 总计异常
        /// </summary>
        public AggregateException AggregateException { get; set; } 

        /// <summary>
        /// 构造方法
        /// </summary>
        public TaskInfo()
        {
            Task = null;
            CancellationTokenSource = null;
            Cycle = default(int);
            ErrorMessage = string.Empty;
            HasError = false;
        }
    }
}
