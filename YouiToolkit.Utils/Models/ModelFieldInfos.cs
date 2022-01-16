using System;
using System.Collections.Generic;
using System.Reflection;

namespace YouiToolkit.Utils
{
    /// <summary>
    /// 实体无序属性集合
    /// </summary>
    /// <typeparam name="TModel">实体类</typeparam>
    public sealed class ModelFieldInfos<TModel> : Dictionary<string, object>
    {
        /// <summary>
        /// 实体
        /// </summary>
        private TModel model = default(TModel);

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModelFieldInfos()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="model">实体</param>
        public ModelFieldInfos(TModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="properties">属性数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo[] properties, Predicate predicate = null)
        {
            if (model == null || properties == null)
            {
                return;
            }

            foreach (FieldInfo property in properties)
            {
                Add(property, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="properties">属性数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo[] properties, Predicate<FieldInfo> predicate = null)
        {
            if (model == null || properties == null)
            {
                return;
            }

            foreach (FieldInfo property in properties)
            {
                Add(property, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo fieldInfo, Predicate predicate = null)
        {
            if (model == null || fieldInfo == null)
            {
                return;
            }

            string name = fieldInfo.Name;
            object value = fieldInfo.GetValue(model);

            if (ModelTypePredicate.Check(fieldInfo, predicate))
            {
                Add(name, value);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo fieldInfo, Predicate<FieldInfo> predicate = null)
        {
            if (model == null || fieldInfo == null)
            {
                return;
            }

            string name = fieldInfo.Name;
            object value = fieldInfo.GetValue(model);

            if (ModelTypePredicate.Check(fieldInfo, predicate))
            {
                Add(name, value);
            }
        }

        /// <summary>
        /// 转成通用辞典格式
        /// </summary>
        /// <returns>通用辞典类型对象</returns>
        public ModelFieldInfos<object> ToObjects()
        {
            ModelFieldInfos<object> props = this;
            return props;
        }

        /// <summary>
        /// 转成通用辞典格式
        /// </summary>
        /// <returns>通用辞典类型对象</returns>
        public ModelFieldInfos<TModel> FromObjects(ModelFieldInfos<object> self)
        {
            ModelFieldInfos<TModel> props = self;
            return props;
        }

        /// <summary>
        /// 重载赋值运算符：类型转换
        /// </summary>
        /// <param name="self">本类型对象</param>
        public static implicit operator ModelFieldInfos<object>(ModelFieldInfos<TModel> self)
        {
            ModelFieldInfos<object> props = new ModelFieldInfos<object>();

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
        public static implicit operator ModelFieldInfos<TModel>(ModelFieldInfos<object> normal)
        {
            ModelFieldInfos<TModel> props = new ModelFieldInfos<TModel>();

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
    public sealed class ModelSortFields<TModel> : List<KeyValuePair<string, object>>
    {
        /// <summary>
        /// 指定实体类型
        /// ※设定后将不从实体类<see cref="TModel"/>获取类型
        /// </summary>
        public Type Type = null;

        /// <summary>
        /// 实体
        /// </summary>
        private TModel model = default(TModel);

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
                    values.Add(property.Value.ToString());
                }

                return values.ToArray();
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModelSortFields()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="model">实体</param>
        public ModelSortFields(TModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fieldInfos">字段数组</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo[] fieldInfos, Predicate predicate = null)
        {
            if (model == null || fieldInfos == null)
            {
                return;
            }

            // 按照指定特性排序
            fieldInfos = ModelComparable<TModel>.Sort(fieldInfos);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Add(fieldInfo, predicate);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="predicate">类型断言</param>
        public void Add(FieldInfo fieldInfo, Predicate predicate = null)
        {
            if (model == null || fieldInfo == null)
            {
                return;
            }

            string name = fieldInfo.Name;
            object value = fieldInfo.GetValue(model);

            // 只添加数值和字符串类型
            if (ModelTypePredicate.Check(fieldInfo, predicate))
            {
                Add(new KeyValuePair<string, object>(name, value));
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="modelSortProperties">实体有序属性集合</param>
        /// <returns></returns>
        public static ModelSortFields<TModel> ToModelProperties(ModelSortFields<TModel> modelSortProperties)
        {
            ModelSortFields<TModel> modelProperties = modelSortProperties;
            return modelProperties;
        }

        /// <summary>
        /// 重载赋值运算符：类型转换
        /// </summary>
        /// <param name="modelSortProperties">实体有序属性集合</param>
        public static implicit operator ModelFieldInfos<TModel>(ModelSortFields<TModel> modelSortProperties)
        {
            ModelFieldInfos<TModel> modelProperties = new ModelFieldInfos<TModel>();

            foreach (var modelSortProperty in modelSortProperties)
            {
                modelProperties.Add(modelSortProperty.Key, modelSortProperty.Value);
            }

            return modelProperties;
        }
    }
}
