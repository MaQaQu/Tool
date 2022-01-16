using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouiToolkit.Utils;

namespace YouiToolkit.Models
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public static string VersionString => $"v{AssemblyHelper.GetAssemblyVersion(typeof(VersionInfo).Assembly)}";
    }
}
