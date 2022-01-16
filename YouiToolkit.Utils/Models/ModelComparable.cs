using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 数据模型比较对象
    /// </summary>
    /// <typeparam name="TModel">数据模型类</typeparam>
    public class ModelComparable<TModel> : IComparable
    {
        /// <summary>
        /// 比较源对象
        /// </summary>
        private object _object = default(object);

        /// <summary>
        /// 获取排序顺序类型
        /// ※将不从数据模型类<see cref="TModel"/>获取排序顺序
        /// </summary>
        private Type _type = null;

        /// <summary>
        /// 获取比较源对象
        /// </summary>
        public object Object => _object;

        /// <summary>
        /// 构造
        /// </summary>
        protected ModelComparable()
        {
        }

        /// <summary>
        /// 比较方法
        /// </summary>
        /// <param name="obj">比较目标对象</param>
        /// <returns>比较结果</returns>
        public int CompareTo(object obj)
        {
            int compareResult = 0;
            int? leftOrder = default(int?);
            int? rightOrder = default(int?);

            object left = _object;
            object right = (obj as ModelComparable<TModel>)?._object;

            if (left is PropertyInfo)
            {
                leftOrder = GetPropertyOrder(left);
            }
            else if (left is FieldInfo)
            {
                leftOrder = GetFieldOrder(left);
            }

            if (right is PropertyInfo)
            {
                rightOrder = GetPropertyOrder(right);
            }
            else if (right is FieldInfo)
            {
                rightOrder = GetFieldOrder(right);
            }

            if (leftOrder != null && rightOrder != null)
            {
                if (leftOrder > rightOrder)
                {
                    compareResult = 1;
                }
                else if (leftOrder < rightOrder)
                {
                    compareResult = -1;
                }
                else
                {
                    compareResult = 0;
                }
            }

            return compareResult;
        }

        /// <summary>
        /// 属性数组排序
        /// </summary>
        /// <param name="propertyInfos">属性数组</param>
        /// <returns>排序后数组</returns>
        public static PropertyInfo[] Sort(PropertyInfo[] propertyInfos, Type type = null)
        {
            ModelComparable<TModel>[] modelComparables = FromPropertyInfos(propertyInfos, type);
            Array.Sort(modelComparables);
            return ToPropertyInfos(modelComparables);
        }

        /// <summary>
        /// 字段数组排序
        /// </summary>
        /// <param name="propertyInfos">字段数组</param>
        /// <returns>排序后数组</returns>
        public static FieldInfo[] Sort(FieldInfo[] fieldInfos, Type type = null)
        {
            ModelComparable<TModel>[] modelComparables = FromFieldInfos(fieldInfos, type);
            Array.Sort(modelComparables);
            return ToFieldInfos(modelComparables);
        }

        /// <summary>
        /// 从属性数组构造
        /// </summary>
        /// <param name="propertyInfos">属性数组</param>
        /// <returns>自身对象数组</returns>
        public static ModelComparable<TModel>[] FromPropertyInfos(PropertyInfo[] propertyInfos, Type type = null)
        {
            List<ModelComparable<TModel>> modelComparableList = new List<ModelComparable<TModel>>();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                modelComparableList.Add(new ModelComparable<TModel>() { _object = propertyInfo, _type = type });
            }
            return modelComparableList.ToArray();
        }

        /// <summary>
        /// 从字段数组构造
        /// </summary>
        /// <param name="propertyInfos">字段数组</param>
        /// <returns>自身对象数组</returns>
        public static ModelComparable<TModel>[] FromFieldInfos(FieldInfo[] fieldInfos, Type type = null)
        {
            List<ModelComparable<TModel>> modelComparableList = new List<ModelComparable<TModel>>();

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                modelComparableList.Add(new ModelComparable<TModel>() { _object = fieldInfo, _type = type });
            }
            return modelComparableList.ToArray();
        }

        /// <summary>
        /// 输出为属性数组
        /// </summary>
        /// <param name="propertyInfos">自身对象数组</param>
        /// <returns>属性数组</returns>
        public static PropertyInfo[] ToPropertyInfos(ModelComparable<TModel>[] selfs)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            foreach (ModelComparable<TModel> self in selfs)
            {
                properties.Add(self._object as PropertyInfo);
            }
            return properties.ToArray();
        }

        /// <summary>
        /// 输出为字段数组
        /// </summary>
        /// <param name="propertyInfos">自身对象数组</param>
        /// <returns>字段数组</returns>
        public static FieldInfo[] ToFieldInfos(ModelComparable<TModel>[] selfs)
        {
            List<FieldInfo> fieldInfo = new List<FieldInfo>();

            foreach (ModelComparable<TModel> self in selfs)
            {
                fieldInfo.Add(self._object as FieldInfo);
            }
            return fieldInfo.ToArray();
        }

        /// <summary>
        /// 从属性获取排序顺序
        /// 排序设定参考<see cref="DataMemberAttribute.Order" cref="DataColumnAttribute.Order"/>
        /// </summary>
        /// <param name="propertyInfos">属性对象</param>
        /// <returns>排序顺序</returns>
        public int? GetPropertyOrder(object obj)
        {
            int? order = default;

            if (obj is PropertyInfo p)
            {
                order = ReflectionHelper.GetPropertyAttribute<DataMemberAttribute>(_type ?? typeof(TModel), p.Name)?.Order;
                if (order == null)
                {
                    try
                    {
                        var propertyInfo = (_type ?? typeof(TModel)).GetProperty(p.Name);

                        foreach (CustomAttributeData attrData in propertyInfo.CustomAttributes)
                        {
                            PropertyInfo propOrder = attrData.AttributeType.GetProperty("Order", BindingFlags.Public | BindingFlags.Instance);

                            if (propOrder != null && propOrder.PropertyType == typeof(int))
                            {
                                Attribute attrTarget = Attribute.GetCustomAttribute((_type ?? typeof(TModel)).GetProperty(p.Name), attrData.AttributeType);

                                order = (int?)propOrder.GetValue(attrTarget);
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return order;
        }

        /// <summary>
        /// 从字段获取排序顺序
        /// 排序设定参考<see cref="DataMemberAttribute.Order" cref="DataColumnAttribute.Order"/>
        /// </summary>
        /// <param name="propertyInfos">字段对象</param>
        /// <returns>排序顺序</returns>
        public int? GetFieldOrder(object obj)
        {
            int? order = default(int?);

            if (obj is FieldInfo)
            {
                FieldInfo p = obj as FieldInfo;

                order = ReflectionHelper.GetFieldAttribute<DataMemberAttribute>(_type ?? typeof(TModel), p.Name)?.Order;
                if (order == null)
                {
                    var propertyInfo = (_type ?? typeof(TModel)).GetProperty(p.Name);

                    foreach (CustomAttributeData attrData in propertyInfo.CustomAttributes)
                    {
                        FieldInfo propOrder = attrData.AttributeType.GetField("Order", BindingFlags.Public | BindingFlags.Instance);

                        if (propOrder != null && propOrder.FieldType == typeof(int))
                        {
                            Attribute attrTarget = Attribute.GetCustomAttribute((_type ?? typeof(TModel)).GetProperty(p.Name), attrData.AttributeType);

                            order = (int?)propOrder.GetValue(attrTarget);
                            break;
                        }
                    }
                }
            }
            return order;
        }
    }
}
