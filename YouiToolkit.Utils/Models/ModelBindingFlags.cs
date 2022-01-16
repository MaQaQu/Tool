using System.Reflection;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 实体绑定标志
    /// </summary>
    public class ModelBindingFlags
    {
        #region [公有 常量字段]
        /// <summary>
        /// 默认绑定标志
        /// </summary>
        public const BindingFlags DEFAULT = BindingFlags.Default;

        /// <summary>
        /// 实例绑定标志
        /// </summary>
        public const BindingFlags INSTANCE = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// 非公有实例绑定标志
        /// </summary>
        public const BindingFlags INSTANCE_NON_PUBLIC = BindingFlags.Instance | BindingFlags.NonPublic;

        /// <summary>
        /// 静态绑定标志
        /// </summary>
        public const BindingFlags STATIC = BindingFlags.Static | BindingFlags.Public;

        /// <summary>
        /// 非公有静态绑定标志
        /// </summary>
        public const BindingFlags STATIC_NON_PUBLIC = BindingFlags.Static | BindingFlags.NonPublic;
        #endregion

        #region [私有 字段]
        /// <summary>
        /// 绑定标志
        /// </summary>
        private BindingFlags bindingFlags = BindingFlags.Default;
        #endregion

        #region [公有 构造方法]
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="bindingFlags">绑定标志</param>
        public ModelBindingFlags(BindingFlags bindingFlags = INSTANCE)
        {
            this.bindingFlags = bindingFlags;
        }
        #endregion

        #region [公有 运算符重载]
        /// <summary>
        /// 赋值运算符重载
        /// </summary>
        /// <param name="bindingFlags">绑定标志</param>
        public static implicit operator ModelBindingFlags(BindingFlags bindingFlags)
        {
            ModelBindingFlags newInstance = new ModelBindingFlags(bindingFlags);

            return newInstance;
        }

        /// <summary>
        /// 赋值运算符重载
        /// </summary>
        /// <param name="self">实体绑定标志</param>
        public static implicit operator BindingFlags(ModelBindingFlags self)
        {
            return self.bindingFlags;
        }
        #endregion
    }

    /// <summary>
    /// 实体数据类型
    /// </summary>
    public enum EModelType
    {
        eProperty,
        eField,
        eConstructor,
        eMethod,
        eEvent,
    }
}
