using System;
using System.Collections.Generic;
using System.Reflection;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 实体无序属性集合
    /// </summary>
    /// <typeparam name="TModel">实体类</typeparam>
    public sealed class ModelProperties<TModel> : Dictionary<string, object>
    {
        /// <summary>
        /// 实体
        /// </summary>
        private TModel model = default;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModelProperties()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="model">实体</param>
        public ModelProperties(TModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="properties">属性数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo[] properties, Predicate predicate = null)
        {
            if (model == null || properties == null)
            {
                return;
            }

            foreach (PropertyInfo property in properties)
            {
                Add(property, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="properties">属性数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo[] properties, Predicate<PropertyInfo> predicate = null)
        {
            if (model == null || properties == null)
            {
                return;
            }

            foreach (PropertyInfo property in properties)
            {
                Add(property, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo property, Predicate predicate = null)
        {
            if (model == null || property == null)
            {
                return;
            }

            string name = property.Name;
            object value = null;

            if (property.CanRead)
            {
                value = property.GetValue(model, null);
            }

            if (ModelTypePredicate.Check(property, predicate))
            {
                Add(name, value);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo property, Predicate<PropertyInfo> predicate = null)
        {
            if (model == null || property == null)
            {
                return;
            }

            string name = property.Name;
            object value = null;

            if (property.CanRead)
            {
                value = property.GetValue(model, null);
            }

            if (ModelTypePredicate.Check(property, predicate))
            {
                Add(name, value);
            }
        }

        /// <summary>
        /// 转成通用辞典格式
        /// </summary>
        /// <returns>通用辞典类型对象</returns>
        public ModelProperties<object> ToObjects()
        {
            ModelProperties<object> props = this;
            return props;
        }

        /// <summary>
        /// 转成通用辞典格式
        /// </summary>
        /// <returns>通用辞典类型对象</returns>
        public ModelProperties<TModel> FromObjects(ModelProperties<object> self)
        {
            ModelProperties<TModel> props = self;
            return props;
        }

        /// <summary>
        /// 重载赋值运算符：类型转换
        /// </summary>
        /// <param name="self">本类型对象</param>
        public static implicit operator ModelProperties<object>(ModelProperties<TModel> self)
        {
            ModelProperties<object> props = new ModelProperties<object>();

            foreach (var item in self)
            {
                props.Add(item.Key, item.Value);
            }
            return props;
        }

        /// <summary>
        /// 重载赋值运算符：类型转换
        /// </summary>
        /// <param name="normal">通用辞典类型对象</param>
        public static implicit operator ModelProperties<TModel>(ModelProperties<object> normal)
        {
            ModelProperties<TModel> props = new ModelProperties<TModel>();

            foreach (var item in normal)
            {
                props.Add(item.Key, item.Value);
            }
            return props;
        }
    }

    /// <summary>
    /// 实体有序属性集合
    /// </summary>
    /// <typeparam name="TModel">实体类</typeparam>
    public sealed class ModelSortProperties<TModel> : List<KeyValuePair<string, object>>
    {
        /// <summary>
        /// 指定实体类型
        /// ※设定后将不从实体类<see cref="TModel"/>获取类型
        /// </summary>
        public Type Type = null;

        /// <summary>
        /// 实体
        /// </summary>
        private TModel model = default;

        /// <summary>
        /// 获取所有字符串数组形式的已添加的数据
        /// </summary>
        public string[] StringValues
        {
            get
            {
                List<string> values = new List<string>();

                foreach (KeyValuePair<string, object> property in this)
                {
                    values.Add(property.Value == null ? string.Empty : property.Value.ToString());
                }

                return values.ToArray();
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModelSortProperties()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="model">实体</param>
        public ModelSortProperties(TModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="properties">属性数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo[] properties, Predicate predicate = null)
        {
            if (model == null || properties == null)
            {
                return;
            }

            // 按照指定特性排序
            properties = ModelComparable<TModel>.Sort(properties, Type);

            foreach (PropertyInfo property in properties)
            {
                Add(property, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo property, Predicate predicate = null)
        {
            if (model == null || property == null)
            {
                return;
            }

            string name = property.Name;
            object value = null;

            if (property.CanRead)
            {
                value = property.GetValue(model, null);
            }

            if (ModelTypePredicate.Check(property, predicate))
            {
                Add(new KeyValuePair<string, object>(name, value));
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="modelSortProperties">实体有序属性集合</param>
        /// <returns></returns>
        public static ModelProperties<TModel> ToModelProperties(ModelSortProperties<TModel> modelSortProperties)
        {
            ModelProperties<TModel> modelProperties = modelSortProperties;
            return modelProperties;
        }

        /// <summary>
        /// 重载赋值运算符：类型转换
        /// </summary>
        /// <param name="modelSortProperties">实体有序属性集合</param>
        public static implicit operator ModelProperties<TModel>(ModelSortProperties<TModel> modelSortProperties)
        {
            ModelProperties<TModel> modelProperties = new ModelProperties<TModel>();

            foreach (var modelSortProperty in modelSortProperties)
            {
                modelProperties.Add(modelSortProperty.Key, modelSortProperty.Value);
            }

            return modelProperties;
        }
    }

    /// <summary>
    /// 实体有序属性名集合
    /// </summary>
    /// <typeparam name="TModel">实体类</typeparam>
    public sealed class ModelSortNames<TModel> : List<string>
    {
        /// <summary>
        /// 指定实体类型
        /// ※设定后将不从实体类<see cref="TModel"/>获取类型
        /// </summary>
        public Type Type = null;

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="properties">属性数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo[] properties, Predicate predicate = null)
        {
            if (properties == null)
            {
                return;
            }

            // 按照指定特性排序
            properties = ModelComparable<TModel>.Sort(properties, Type);

            foreach (PropertyInfo property in properties)
            {
                Add(property, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(PropertyInfo property, Predicate predicate = null)
        {
            if (property == null)
            {
                return;
            }

            if (ModelTypePredicate.Check(property, predicate))
            {
                Add(property.Name);
            }
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fields">字段数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo[] fields, Predicate predicate = null)
        {
            if (fields == null)
            {
                return;
            }

            // 按照指定特性排序
            fields = ModelComparable<TModel>.Sort(fields, Type);

            foreach (FieldInfo field in fields)
            {
                Add(field, predicate);
            }
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo field, Predicate predicate = null)
        {
            if (field == null)
            {
                return;
            }

            if (ModelTypePredicate.Check(field, predicate))
            {
                Add(field.Name);
            }
        }
    }
}
