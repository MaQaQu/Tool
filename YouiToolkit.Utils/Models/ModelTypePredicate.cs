using System;
using System.Reflection;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 实体属性类型断言
    /// </summary>
    public class ModelTypePredicate
    {
        #region [公有 静态只读字段]
        /// <summary>
        /// 默认属性类型断言方法
        /// </summary>
        public static readonly Predicate<object> Default = (info) =>
        {
            // 只添加数值类型和字符串类型
            if (info is FieldInfo)
            {
                FieldInfo fieldInfo = info as FieldInfo;
                return (fieldInfo.FieldType.IsValueType || fieldInfo.FieldType == typeof(string));
            }
            else if (info is PropertyInfo)
            {
                PropertyInfo property = info as PropertyInfo;
                return (property.PropertyType.IsValueType || property.PropertyType == typeof(string));
            }
            return false;
        };

        /// <summary>
        /// 不指定属性类型断言方法
        /// </summary>
        public static readonly Predicate All = () => true;
        #endregion

        #region [公有 静态方法]
        /// <summary>
        /// 检查断言结果
        /// <remark>如果指定断言方法为空则调用默认断言方法</remark>
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">指定断言方法</param>
        /// <returns>断言结果</returns>
        public static bool Check(PropertyInfo property, Predicate predicate = null)
        {
            if (predicate == null)
            {
                return Default.Invoke(property);
            }
            return predicate.Invoke();
        }

        /// <summary>
        /// 检查断言结果
        /// <remark>如果指定断言方法为空则调用默认断言方法</remark>
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">指定断言方法</param>
        /// <returns>断言结果</returns>
        public static bool Check(PropertyInfo property, Predicate<PropertyInfo> predicate = null)
        {
            if (predicate == null)
            {
                return Default.Invoke(property);
            }
            return predicate.Invoke(property);
        }

        /// <summary>
        /// 检查断言结果
        /// <remark>如果指定断言方法为空则调用默认断言方法</remark>
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">指定断言方法</param>
        /// <returns>断言结果</returns>
        public static bool Check(FieldInfo fieldInfo, Predicate predicate = null)
        {
            if (predicate == null)
            {
                return Default.Invoke(fieldInfo);
            }
            return predicate.Invoke();
        }


        /// <summary>
        /// 检查断言结果
        /// <remark>如果指定断言方法为空则调用默认断言方法</remark>
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">指定断言方法</param>
        /// <returns>断言结果</returns>
        public static bool Check(FieldInfo fieldInfo, Predicate<FieldInfo> predicate = null)
        {
            if (predicate == null)
            {
                return Default.Invoke(fieldInfo);
            }
            return predicate.Invoke(fieldInfo);
        }
        #endregion
    }
}
