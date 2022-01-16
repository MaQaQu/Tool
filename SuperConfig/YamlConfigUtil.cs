using System;
using System.IO;
using YamlDotNet.Serialization;

namespace SuperConfig
{
    public class YamlConfigUtil : AbstractConfigUtil
    {
        /// <summary>
        /// 序列化(保存文件)
        /// </summary>
        /// <param name="obj">序列化实体</param>
        /// <returns>操作结果</returns>
        public override bool Serialize<T>(T obj)
        {
            bool ret = false;

            try
            {
                Serializer serializer = new Serializer();
                string str = serializer.Serialize(obj);

                using StreamWriter sw = File.CreateText(FileName);

                sw.Write(str);
                sw.Flush();
#if YAMLOUT && DEBUG
                Logger.Info($"Save to file `{FileName} :{Environment.NewLine}{str}");
#endif
                ret = true;
            }
            catch (Exception e)
            {
                if (FileName != null)
                {
                    Logger.Error($"Serialize file `'{FileName}' failed!", e);
                }
                else
                {
                    Logger.Error($"Serialize file named null failed!", e);
                }
            }
            return ret;
        }

        /// <summary>
        /// 反序列化(读取文件)
        /// </summary>
        /// <returns>反序列化实体</returns>
        public override T Deserialize<T>()
        {
            T info = default(T);
            StreamReader reader = null;

            try
            {
                Deserializer deserializer = new Deserializer();

                reader = new StreamReader(FileName);
                info = deserializer.Deserialize<T>(reader);
            }
            catch (Exception e)
            {
                if (FileName != null)
                {
                    Logger.Error($"Deserialize file '{FileName}' failed!", e);
                }
                else
                {
                    Logger.Error("Deserialize file named `'null' failed!", e);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return info;
        }
    }
}
