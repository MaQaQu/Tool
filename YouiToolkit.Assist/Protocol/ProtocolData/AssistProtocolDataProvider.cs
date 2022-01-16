using SuperPortLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// IoC构造者
    /// </summary>
    internal static class AssistProtocolDataProvider
    {
        /// <summary>
        /// IoC容器
        /// </summary>
        private readonly static Dictionary<AssistModelCode, Type> container = new();

        /// <summary>
        /// 注册IoC容器
        /// </summary>
        /// <typeparam name="T">注册类型</typeparam>
        /// <param name="protocolData">父对象</param>
        /// <param name="moduleCode">模块码</param>
        public static void Register<T>(AssistModelCode moduleCode) where T : ProviderProtocolData
        {
            container.Add(moduleCode, typeof(T));
        }

        /// <summary>
        /// 请求IoC对象
        /// </summary>
        /// <typeparam name="T">注册类型</typeparam>
        /// <param name="protocolData">父对象</param>
        /// <returns>对象</returns>
        public static ProviderProtocolData Resolve(AssistProtocolData protocolData, AssistModelCode moduleCode)
        {
            Type type = container[moduleCode];

            if (type != null)
            {
                ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(ProtocolData) });

                return ctor?.Invoke(new object[] { protocolData }) as ProviderProtocolData;
            }
            return null;
        }
    }
}
