using SuperPortLibrary;

namespace YouiToolkit.Assist
{
    internal class AssistProtocolData : ProtocolData
    {
        /// <summary>
        /// 构造
        /// </summary>
        static AssistProtocolData()
        {
            AssistProtocolDataProvider.Register<AssistMappingProtocolData>(AssistModelCode.MappingNode);
            AssistProtocolDataProvider.Register<AssistAvoidObstacleProtocolData>(AssistModelCode.AvoidObstacleNode);
        }

        /// <summary>
        /// 编译：接收数据的编译
        /// </summary>
        protected override void OnCompileData()
        {
            if (BytesLength < 4) return;

            AssistModelCode module = (AssistModelCode)BytesData[2];
            AssistProtocolDataProvider.Resolve(this, module)?.CompileData();
        }

        /// <summary>
        /// 反编译：发送数据的编译
        /// </summary>
        protected override void OnDecompileData()
        {
            if (ApplicationData is AssistAppData app)
            {
                AssistModelCode module = app.Module;
                AssistProtocolDataProvider.Resolve(this, module)?.DecompileData();
            }
        }
    }
}
