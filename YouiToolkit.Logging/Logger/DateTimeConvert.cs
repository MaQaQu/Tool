using System;
using System.Globalization;

namespace YouiToolkit.Logging
{
    internal static class DateTimeConvert
    {
        /// <summary>
        /// 日期格式（通常）
        /// </summary>
        public const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// 日期格式（通常且无毫秒）
        /// </summary>
        public const string DATETIME_FORMAT_NO_MILLISECOND = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 日期格式（文件名）
        /// </summary>
        public const string DATETIME_FORMAT_FILE = "yyyyMMddHHmmssfff";

        /// <summary>
        /// 日期格式（文件名）
        /// </summary>
        public const string DATETIME_FORMAT_FILE_NO_MILLISECOND = "yyyyMMddHHmmss";

        /// <summary>
        /// 时间格式（通常）
        /// </summary>
        public const string TIME_FORMAT = "HH:mm:ss.fff";

        /// <summary>
        /// String转DateTime
        /// </summary>
        /// <param name="data">字符串对象</param>
        /// <param name="format">转换格式</param>
        /// <returns>日期时间</returns>
        public static DateTime? ToDateTime(this string self, string format = DATETIME_FORMAT)
        {
            if (string.IsNullOrEmpty(self))
            {
                return null;
            }
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo
            {
                ShortDatePattern = format,
            };
            if (DateTime.TryParse(self, dtFormat, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            return null;
        }

        /// <summary>
        /// DateTime转String
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="format">格式</param>
        /// <returns>字符串</returns>
        public static string FromDateTime(DateTime dateTime, string format = DATETIME_FORMAT) => dateTime.ToString(format);
    }
}
