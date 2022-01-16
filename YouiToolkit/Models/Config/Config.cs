using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperConfig;
using YamlDotNet.Serialization;

namespace YouiToolkit
{
    internal class Config
    {
        /// <summary>
        /// 全局单例
        /// </summary>
        public static Config Instance { get; set; } = ConfigCtrl.Init();

        /// <summary>
        /// 机器人IP地址
        /// </summary>
        public string IPAddr { get; set; } = string.Empty;

        /// <summary>
        /// 地图名称
        /// </summary>
        [YamlIgnore]
        public string[] MapNames = Array.Empty<string>();
    }
}
