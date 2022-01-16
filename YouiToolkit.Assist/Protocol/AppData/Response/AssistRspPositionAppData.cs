using System;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 响应定位数据
    /// </summary>
    [Obsolete]
    [AssistCmdCode(AssistCmdCode.RspPosition)]
    internal class AssistRspPositionAppData : AssistAppData
    {
        public bool Valid { get; set; }
        public int Status { get; set; }
        public int ErrorCode { get; set; }

        /// <summary>
        /// 相对于世界坐标X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 相对于世界坐标Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 角度A
        /// </summary>
        public double A { get; set; }
    }
}
