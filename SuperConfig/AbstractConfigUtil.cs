namespace SuperConfig
{
    public abstract class AbstractConfigUtil
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 序列化(保存文件)
        /// </summary>
        /// <param name="obj">序列化实体</param>
        /// <returns>操作结果</returns>
        public abstract bool Serialize<T>(T obj);

        /// <summary>
        /// 反序列化(读取文件)
        /// </summary>
        /// <returns>反序列化实体</returns>
        public abstract T Deserialize<T>();
    }
}
