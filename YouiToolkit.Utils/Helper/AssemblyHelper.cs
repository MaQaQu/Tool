using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 程序集帮助类
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// 获取指定类型首个程序集
        /// </summary>
        /// <typeparam name="TAssy">程序集类型</typeparam>
        /// <returns>程序集</returns>
        public static TAssy GetAssembly<TAssy>(Assembly assembly = null)
        {
            TAssy[] assemblies = GetAssemblies<TAssy>(assembly);

            if (assemblies.Length > 0)
            {
                return assemblies[0];
            }
            return default;
        }

        /// <summary>
        /// 获取指定类型程序集集合
        /// </summary>
        /// <typeparam name="TAssy">程序集类型</typeparam>
        /// <returns>程序集集合</returns>
        public static TAssy[] GetAssemblies<TAssy>(Assembly assembly = null)
        {
            object[] attributes = (assembly ?? Assembly.GetExecutingAssembly()).GetCustomAttributes(typeof(TAssy), false);
            List<TAssy> attributeList = new List<TAssy>();

            foreach (object attribute in attributes)
            {
                if (attribute is TAssy)
                {
                    attributeList.Add((TAssy)attribute);
                }
            }
            return attributeList.ToArray();
        }

        /// <summary>
        /// 获取程序集的版本属性
        /// </summary>
        public static string GetAssemblyVersion(this Assembly assembly, EVersionType type = EVersionType.Major | EVersionType.Minor | EVersionType.Build)
        {
            Version version = assembly.GetName().Version;
            StringBuilder sb = new StringBuilder();

            if (type.HasFlag(EVersionType.Major))
            {
                sb.Append(version.Major);
            }
            if (type.HasFlag(EVersionType.Minor))
            {
                if (sb.Length > 0)
                    sb.Append(".");
                sb.Append(version.Minor);
            }
            if (type.HasFlag(EVersionType.Build))
            {
                if (sb.Length > 0)
                    sb.Append(".");
                sb.Append(version.Build);
            }
            if (type.HasFlag(EVersionType.Revision))
            {
                if (sb.Length > 0)
                    sb.Append(".");
                sb.Append(version.Revision);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得程序集构建时间
        /// </summary>
        /// <returns>构建时间</returns>
        public static DateTime GetAssemblyDateTime(Assembly assembly = null)
        {
            Func<string, DateTime> GetLinkerTimeStamp = (filepath) =>
            {
                const int peHeaderOffset = 60;
                const int linkerTimestampOffset = 8;
                byte[] b = new byte[2048];
                using (Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    stream.Read(b, 0, 2048);
                }
                int i = BitConverter.ToInt32(b, peHeaderOffset);
                int secondsSince1970 = BitConverter.ToInt32(b, i + linkerTimestampOffset);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);

                dt = dt.AddSeconds(secondsSince1970);
                dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
                return dt;
            };

            return GetLinkerTimeStamp((assembly ?? Assembly.GetExecutingAssembly()).Location);
        }

        /// <summary>
        /// 获取程序集名称
        /// </summary>
        /// <returns>程序集名称</returns>
        public static string GetAssemblyTitle(Assembly assembly = null)
        {
            AssemblyTitleAttribute attr = GetAssembly<AssemblyTitleAttribute>(assembly);

            return attr?.Title ?? Path.GetFileNameWithoutExtension((assembly ?? Assembly.GetExecutingAssembly()).CodeBase);
        }

        /// <summary>
        /// 获取程序集的版权
        /// </summary>
        /// <returns>程序集版权</returns>
        public static string GetAssemblyCopyright(Assembly assembly = null) => GetAssembly<AssemblyCopyrightAttribute>(assembly).Copyright;
    }

    [Flags]
    public enum EVersionType
    {
        Major = 0,
        Minor = 1,
        Build = 2,
        Revision = 4,
    }
}
