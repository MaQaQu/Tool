using System;

namespace SuperPort
{
    public static class InternalLogger
    {
        /// <summary>
        /// 解耦日志输出接口
        /// </summary>
        public static Action<int, object[]> Exposed = null;

        /// <summary>
        /// 设定日志输出接口
        /// </summary>
        public static void SetExposed(Action<int, object[]> exposed) => Exposed = exposed ?? throw new ArgumentNullException(nameof(exposed));
    }

    internal static class Logger
    {
        /// <summary>
        /// 内部日志接口
        /// </summary>
        public static Action<int, object[]> Exposed => InternalLogger.Exposed;

        /// <summary>
        /// 日志信息入队（IGNORE级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Ignore(params object[] logs) => Exposed?.Invoke((int)ELevel.Ignore, logs);

        /// <summary>
        /// 日志信息入队（DEBUG级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Debug(params object[] logs) => Exposed?.Invoke((int)ELevel.Debug, logs);

        /// <summary>
        /// 日志信息入队（INFO级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Info(params object[] logs) => Exposed?.Invoke((int)ELevel.Info, logs);

        /// <summary>
        /// 日志信息入队（WARN级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Warn(params object[] logs) => Exposed?.Invoke((int)ELevel.Warn, logs);

        /// <summary>
        /// 日志信息入队（ERROR级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Error(params object[] logs) => Exposed?.Invoke((int)ELevel.Error, logs);

        /// <summary>
        /// 日志信息入队（FATAL级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Fatal(params object[] logs) => Exposed?.Invoke((int)ELevel.Fatal, logs);

        /// <summary>
        /// 日志级别定义
        /// </summary>
        private enum ELevel
        {
            Ignore, // 忽略级别
            Debug,  // 调试级别
            Info,   // 信息级别
            Warn,   // 警告级别
            Error,  // 错误级别
            Fatal,  // 致命级别
        }
    }
}
