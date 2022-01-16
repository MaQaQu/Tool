using System;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 命令代码连接器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    internal class AssistCmdCodeAttribute : Attribute
    {
        public Type Type { get; set; }
        public object Source { get; set; }
        public object Tag { get; set; }

        public AssistCmdCodeAttribute(object @object)
        {
            Source = @object;
        }
    }
}
