using System;
using System.Text;

namespace YouiToolkit.Utils
{
    public class MessageCenterEventArgs : EventArgs
    {
        /// <summary>
        /// 消息
        /// </summary>
        private string innerMessage { get; set; }

        /// <summary>
        /// 消息名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 回调消息名称
        /// </summary>
        public string NameCallback { get; set; } = string.Empty;

        /// <summary>
        /// 是否命令执行时触发异常
        /// </summary>
        public bool HasException => CatchedException != null;

        /// <summary>
        /// 命令执行时触发异常记录
        /// </summary>
        public Exception CatchedException { get; private set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get
            {
                if (HasException)
                {
                    return CatchedException.Message;
                }
                return innerMessage;
            }
        }

        /// <summary>
        /// 参数
        /// </summary>
        public object Param { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="message">消息</param>
        public MessageCenterEventArgs(string name, string message)
        {
            Name = name;
            innerMessage = message;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="message">消息</param>
        /// <param name="param">参数</param>
        public MessageCenterEventArgs(string name, string message, object param)
        {
            Name = name;
            innerMessage = message;
            Param = param;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="message">消息</param>
        /// <param name="nameCallback">回调消息名称</param>
        /// <param name="param">参数</param>
        public MessageCenterEventArgs(string name, string message, string nameCallback, object param)
        {
            Name = name;
            innerMessage = message;
            Param = param;
            NameCallback = nameCallback;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="catchedException">异常</param>
        public MessageCenterEventArgs(string name, Exception catchedException)
        {
            Name = name;
            CatchedException = catchedException;
        }

        /// <summary>
        /// 实例值转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (CatchedException != null)
            {
                sb.Append(CatchedException.ToString());
                sb.Append($"\r\n");
            }

            sb.Append(Name);
            if (Param != null || NameCallback != null)
            {
                sb.Append($" (");

                if (Param != null)
                {
                    sb.Append($"param={Param}");
                }
                if (NameCallback != null)
                {
                    if (Param != null)
                    {
                        sb.Append($", ");
                    }
                    sb.Append($"callback={NameCallback}");
                }

                sb.Append($")");
            }

            return sb.ToString();
        }
    }
}
