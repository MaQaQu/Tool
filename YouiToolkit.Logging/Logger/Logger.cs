using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace YouiToolkit.Logging
{
    /// <summary>
    /// 日志输出类
    /// </summary>
    public class Logger : CyclicQueue<string>
    {
        #region [私有 枚举体]
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
        #endregion

        #region [私有 字段]
        /// <summary>
        /// 日志路劲
        /// </summary>
        private string path = string.Empty;

        /// <summary>
        /// 日志文件路劲
        /// </summary>
        private string fileName = string.Empty;

        /// <summary>
        /// 备份日志文件路劲
        /// </summary>
        private string fileNameBak = string.Empty;

        /// <summary>
        /// 日志级别
        /// </summary>
        private ELevel level = default;

        /// <summary>
        /// 日志缓存区
        /// </summary>
        private readonly static StringBuilder buffer = new StringBuilder();
        #endregion

        #region [公有 静态属性]
        /// <summary>
        /// 日志工具人单例
        /// </summary>
        internal static Logger Instance { get; set; } = new Logger();
        #endregion

        #region [私有 构造方法]
        /// <summary>
        /// 构造方法
        /// </summary>
        private Logger()
        {
            try
            {
                Name = nameof(Logger);
                path = "logs";
                fileName = Path.Combine(path, DateTimeConvert.FromDateTime(DateTime.Now, "yyyyMMdd") + ".log");
                fileNameBak = Path.ChangeExtension(fileName, ".log.bak");
                Cycle = 200;
#if DEBUG
                level = ELevel.Debug;
#else
                level = ELevel.Info;
#endif

                DequeueStarted += () => buffer.Clear();
                Dequeued += (str) => buffer.Append(str);
                DequeueCompleted += () =>
                {
                    // 输出控制台
                    try
                    {
                        Trace.Write(buffer.ToString());
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.ToString());
                    }

                    // 输出日志文件
                    const ushort maxTryTime = 10;
                    ushort currentTryTime = default;

                    while (true)
                    {
                        bool successed = true;

                        try
                        {
                            AppendToFile(buffer.ToString());
                        }
                        catch (Exception e)
                        {
                            successed = false;
                            currentTryTime++;
                            Trace.WriteLine(e.ToString());
                        }

                        if (successed)
                        {
                            break;
                        }
                        else
                        {
                            // 超过最大尝试次数则丢弃日志
                            if (currentTryTime >= maxTryTime)
                            {
                                Trace.WriteLine(">>>!!!-LOG-FILE-LOST-!!!<<<");
                                break;
                            }

                            // 歇息4分之1个周期时间重新尝试
                            Thread.Sleep(Cycle / 4);
                        }
                    }
                };

                Start();
                CancelRegister(Flush);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
        #endregion

        #region [私有 方法]
        /// <summary>
        /// 日志信息入队
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="logs">日志对象</param>
        private void Enqueue(ELevel level, params object[] logs)
        {
            if (logs != null && level >= this.level)
            {
                try
                {
                    string str = RenderLog(level, logs);
                    Enqueue(str);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// 构建堆栈跟踪的可读表示.
        /// - 如果skipFrames为1，则获取上一层的方法（RenderLog）；
        /// - 如果是为2层，则获取的是上二层的方法（Log）。
        /// - 下面函数最后的skipFrames为3，则获取的上三层的方法（调用log的地方）
        /// </summary>
        /// <param name="skipFrames">堆栈上要跳过的帧数</param>
        /// <returns>堆栈跟踪的可读表示</returns>
        private string GetStackFrameInfo(int skipFrames)
        {
            StackFrame stackFrame = new StackFrame((skipFrames < 1) ? 1 : (skipFrames + 1), true);
            MethodBase method = stackFrame.GetMethod();

            if (method != null)
            {
                StringBuilder result = new StringBuilder();

                result.Append(Path.GetFileName(stackFrame.GetFileName()) ?? "<unknown>");
                result.Append(":");
                result.Append(stackFrame.GetFileLineNumber());
                result.Append("|");
                result.Append(method.Name);

                // 追加方法参数
                if (method is MethodInfo && ((MethodInfo)method).IsGenericMethod)
                {
                    Type[] genericArguments = ((MethodInfo)method).GetGenericArguments();
                    int i = 0;
                    bool flag = true;

                    result.Append("<");

                    while (i < genericArguments.Length)
                    {
                        if (!flag)
                        {
                            result.Append(",");
                        }
                        else
                        {
                            flag = false;
                        }

                        result.Append(genericArguments[i].Name);

                        i++;
                    }

                    result.Append(">");
                }

                return result.ToString();
            }
            else
            {
                return "<null>";
            }
        }

        /// <summary>
        /// 将参数呈现为字符串
        /// </summary>
        /// <param name="type">日志级别</param>
        /// <param name="objs">要记录的诊断消息或对象</param>
        /// <returns>呈现的布局字符串 </returns>
        private string RenderLog(ELevel level, object[] objs)
        {
            StringBuilder result = new StringBuilder();

            result.Append(DateTimeConvert.FromDateTime(DateTime.Now));
            result.Append("|");
            result.Append(Environment.UserName);
            result.Append("|");
            result.Append(RenderLevel(level));
            result.Append("|");
            result.Append(Thread.CurrentThread.ManagedThreadId.ToString("000"));
            result.Append("|");
            result.Append(GetStackFrameInfo(3));
            result.Append("|");

            if (objs != null && objs.Length > 0)
            {
                int count = 0;

                foreach (object item in objs)
                {
                    result.Append((count++ == 0) ? string.Empty : " ");
                    result.Append(item == null ? string.Empty : item.ToString());
                }
            }
            result.Append(Environment.NewLine);

            return result.ToString();
        }

        /// <summary>
        /// 将级别呈现为字符串
        /// </summary>
        /// <param name="type">日志级别</param>
        /// <returns>呈现的级别字符串 </returns>
        private string RenderLevel(ELevel level) => level.ToString().ToUpper().PadRight(5);

        /// <summary>
        /// 将日志消息附加到文件
        /// </summary>
        /// <param name="message">要附加的日志消息</param>
        /// <exception cref="Exception">日志文件附加写入失败</exception>
        private void AppendToFile(string message)
        {
            FileStream fileStream = null;

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

                if (fileStream.Length > 10485760)
                {
                    try
                    {
                        File.Copy(fileName, fileNameBak, true);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.ToString());
                    }

                    fileStream.SetLength(0);
                }

                byte[] bytes = Encoding.UTF8.GetBytes(message);

                fileStream.Seek(0, SeekOrigin.End);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                fileStream?.Dispose();
            }
        }
        #endregion

        #region [公有 方法]
        /// <summary>
        /// 关闭日志工具人
        /// </summary>
        public static void Close() => Instance.Stop();

        /// <summary>
        /// 日志信息入队（IGNORE级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Ignore(params object[] logs) => Instance.Enqueue(ELevel.Ignore, logs);

        /// <summary>
        /// 日志信息入队（DEBUG级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Debug(params object[] logs) => Instance.Enqueue(ELevel.Debug, logs);

        /// <summary>
        /// 日志信息入队（INFO级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Info(params object[] logs) => Instance.Enqueue(ELevel.Info, logs);

        /// <summary>
        /// 日志信息入队（WARN级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Warn(params object[] logs) => Instance.Enqueue(ELevel.Warn, logs);

        /// <summary>
        /// 日志信息入队（ERROR级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Error(params object[] logs) => Instance.Enqueue(ELevel.Error, logs);

        /// <summary>
        /// 日志信息入队（FATAL级别）
        /// </summary>
        /// <param name="logs">日志字符数组数据</param>
        public static void Fatal(params object[] logs) => Instance.Enqueue(ELevel.Fatal, logs);

        /// <summary>
        /// 手动冲洗日志
        /// </summary>
        public static void Flush() => Instance.Dequeue();

        /// <summary>
        /// 暴露接口
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="logs">日志字符数组数据</param>
        /// <remarks>
        /// 
        /// </remarks>
        public static void Exposed(int level, params object[] logs) => Instance.Enqueue((ELevel)level, logs);
        #endregion
    }
}
