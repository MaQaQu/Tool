using SuperPortLibrary;

namespace YouiToolkit.Assist
{
    internal class AssistAppData : ApplicationData
    {
        /// <summary>
        /// 会话标识
        /// </summary>
        public int Session { get; set; }

        /// <summary>
        /// 模块码
        /// </summary>
        public AssistModelCode Module { get; set; }

        /// <summary>
        /// 命令码
        /// </summary>
        public AssistCmdCode Cmd { get; set; }
    }
}
