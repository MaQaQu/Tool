using System;
using System.Text;

namespace YouiToolkit.Assist
{
    [AssistCmdCode(AssistCmdCode.RspMappingList)]
    internal class AssistRspMappingListAppData : AssistAppData
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        public const char SplitChar = '\n';

        /// <summary>
        /// 地图列表长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 地图列表
        /// </summary>
        public string[] List { get; set; }

        /// <summary>
        /// 地图列表初始化
        /// </summary>
        public static string[] ToList(byte[] names)
        {
            if (names != null && names.Length > 0)
            {
                string nameStrs = Encoding.UTF8.GetString(names);
                string[] name = nameStrs.Split(SplitChar);

                return name;
            }
            return Array.Empty<string>();
        }
    }
}
