using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YouiToolkit.Logging;

namespace YouiToolkit.Utils
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T">对象类</typeparam>
        /// <param name="type">对象类型</param>
        /// <param name="object[]">构造参数</param>
        /// <returns>对象实例</returns>
        public static T CreateInstance<T>(Type type, object[] param = null)
        {
            try
            {
                Type[] types = null;
                ConstructorInfo constructor = null;

                if (param != null)
                {
                    types = new Type[param.Length];

                    for (int i = 0; i < param.Length; i++)
                    {
                        types[i] = param[i].GetType();
                    }
                }

                types = types ?? (new List<Type>()).ToArray();

                constructor = type.GetConstructor(types);

                if (constructor != null)
                {
                    object instance = constructor.Invoke(param);

                    return (T)instance;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e.ToString());
            }
            return default;
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T">对象类</typeparam>
        /// <param name="object[]">构造参数</param>
        public static T CreateInstance<T>(object[] param = null)
        {
            return CreateInstance<T>(typeof(T), param);
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelProperties<T> GetModelProperties<T>(T t, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            ModelProperties<T> modelProperties = new ModelProperties<T>(t);
            PropertyInfo[] properties = t.GetType().GetProperties(bingdingFlags);

            modelProperties.Add(properties, predicate);
            return modelProperties;
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelProperties<T> GetModelProperties<T>(T t, BindingFlags bingdingFlags, Predicate<PropertyInfo> predicate)
        {
            ModelProperties<T> modelProperties = new ModelProperties<T>(t);
            PropertyInfo[] properties = t.GetType().GetProperties(bingdingFlags);

            modelProperties.Add(properties, predicate);
            return modelProperties;
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelProperties<object> GetModelProperties(Type type, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            return GetModelProperties(CreateInstance<object>(type), bingdingFlags, predicate);
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelProperties<object> GetModelProperties(Type type, BindingFlags bingdingFlags, Predicate<PropertyInfo> predicate)
        {
            return GetModelProperties(CreateInstance<object>(type), bingdingFlags, predicate);
        }

        /// <summary>
        /// 反射得到实体类的字段名称和类型
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <returns>字段字典</returns>
        public static Dictionary<string, Type> GetModelPropertyTypes(Type type, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE)
        {
            Dictionary<string, Type> modelProperties = new Dictionary<string, Type>();
            PropertyInfo[] properties = type.GetProperties(bingdingFlags);

            if (properties == null)
            {
                return modelProperties;
            }

            foreach (PropertyInfo property in properties)
            {
                modelProperties.Add(property.Name, property.PropertyType);
            }
            return modelProperties;
        }

        /// <summary>
        /// 反射得到实体类的有序字段名称和值
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelSortProperties<T> GetModelSortProperties<T>(T t, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            ModelSortProperties<T> modelSortProperties = new ModelSortProperties<T>(t);
            PropertyInfo[] properties = t.GetType().GetProperties(bingdingFlags);

            modelSortProperties.Add(properties, predicate);
            return modelSortProperties;
        }

        /// <summary>
        /// 反射得到实体类的有序字段名称和值
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelSortProperties<object> GetModelSortProperties(Type type, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            return GetModelSortProperties(CreateInstance<object>(type), bingdingFlags, predicate);
        }

        /// <summary>
        /// 反射得到实体类的有序字段名称
        /// </summary>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelSortNames<T> GetModelSortNames<T>(EModelType modelType = EModelType.eProperty, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            ModelSortNames<T> modelSortNames = new ModelSortNames<T>();

            switch (modelType)
            {
                case EModelType.eProperty:
                    PropertyInfo[] properties = typeof(T).GetProperties(bingdingFlags);
                    modelSortNames.Add(properties, predicate);
                    break;
                case EModelType.eField:
                    FieldInfo[] fields = typeof(T).GetFields(bingdingFlags);
                    modelSortNames.Add(fields, predicate);
                    break;
                default:
                    throw new Exception("Unsupported model type.");
            }

            return modelSortNames;
        }

        /// <summary>
        /// 反射得到实体类的有序字段名称
        /// </summary>
        /// <param name="type">实体类型</typeparam>
        /// <param name="modelType">目标类型</typeparam>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelSortNames<object> GetModelSortNames(Type type, EModelType modelType = EModelType.eProperty, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            ModelSortNames<object> modelSortNames = new ModelSortNames<object>() { Type = type };

            switch (modelType)
            {
                case EModelType.eProperty:
                    PropertyInfo[] properties = type.GetProperties(bingdingFlags);
                    modelSortNames.Add(properties, predicate);
                    break;
                case EModelType.eField:
                    FieldInfo[] fields = type.GetFields(bingdingFlags);
                    modelSortNames.Add(fields, predicate);
                    break;
                default:
                    throw new Exception("Unsupported model type.");
            }

            return modelSortNames;
        }

        /// <summary>
        /// 反射从字段名称和值字典赋值到实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="modelProperties">字段字典</param>
        /// <param name="bingdingFlags">绑定标志</param>
        public static void SetModelProperties<T>(ref T t, ModelProperties<T> modelProperties, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE)
        {
            try
            {
                // 字段字典为空则返回
                if (modelProperties == null || modelProperties.Count < 0)
                {
                    return;
                }

                // 实体实例不可为空
                if (t == null)
                {
                    // 使用默认构造方法构造实体实例
                    t = CreateInstance<T>();

                    // 实体实例不可为空
                    if (t == null)
                    {
                        return;
                    }
                }

                PropertyInfo[] properties = t.GetType().GetProperties(bingdingFlags);

                // 遍历所有属性赋值
                foreach (var modelProperty in modelProperties)
                {
                    try
                    {
                        // 找到同名属性用于赋值
                        int index = Array.FindIndex(properties, (porp) => porp.Name == modelProperty.Key);

                        if (InRange(index, properties))
                        {
                            var property = properties[index];

                            if (property != null && property.CanWrite)
                            {
                                object value = modelProperty.Value;

                                // 数据类型不匹配则尝试修复
                                value = ChangeType(value, property.PropertyType);

                                property.SetValue(t, value, null);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Debug(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
            }
        }

        /// <summary>
        /// 反射从字段名称和值字典赋值到实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="modelProperties">字段字典</param>
        /// <param name="bingdingFlags">绑定标志</param>
        public static void SetModelProperties<T>(ref T t, ModelProperties<object> modelProperties, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE)
        {
            ModelProperties<T> props = modelProperties;

            SetModelProperties(ref t, props, bingdingFlags);
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelFieldInfos<T> GetModelFields<T>(T t, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            ModelFieldInfos<T> modelFields = new ModelFieldInfos<T>(t);
            FieldInfo[] fields = t.GetType().GetFields(bingdingFlags);

            modelFields.Add(fields, predicate);
            return modelFields;
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t">实例化</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelFieldInfos<T> GetModelFields<T>(T t, BindingFlags bingdingFlags, Predicate<FieldInfo> predicate)
        {
            ModelFieldInfos<T> modelFields = new ModelFieldInfos<T>(t);
            FieldInfo[] fields = t.GetType().GetFields(bingdingFlags);

            modelFields.Add(fields, predicate);
            return modelFields;
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelFieldInfos<object> GetModelFields(Type type, BindingFlags bingdingFlags = ModelBindingFlags.INSTANCE, Predicate predicate = null)
        {
            return GetModelFields(CreateInstance<object>(type), bingdingFlags, predicate);
        }

        /// <summary>
        /// 反射得到实体类的字段名称和值
        /// </summary>
        /// <param name="type">实例类型</param>
        /// <param name="bingdingFlags">绑定标志</param>
        /// <param name="predicate">类型断言</param>
        /// <returns>字段字典</returns>
        public static ModelFieldInfos<object> GetModelFields(Type type, BindingFlags bingdingFlags, Predicate<FieldInfo> predicate)
        {
            return GetModelFields(CreateInstance<object>(type), bingdingFlags, predicate);
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">数据名称</param>
        /// <returns>特性</returns>
        public static TAttr GetPropertyAttribute<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetProperty(name), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">数据名称</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>特性</returns>
        public static TAttr GetPropertyAttribute<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetProperty(name, bindingFlags), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr GetMethodAttribute<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetMethod(name), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">数据名称</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>特性</returns>
        public static TAttr GetMethodAttribute<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetMethod(name, bindingFlags), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr GetEventAttribute<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetEvent(name), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr GetEventAttribute<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetEvent(name, bindingFlags), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取类型成员特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr GetMemberAttribute<TAttr>(Type type) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type, typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取值特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr GetFieldAttribute<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetField(name), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取值特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>特性</returns>
        public static TAttr GetFieldAttribute<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => (TAttr)Attribute.GetCustomAttribute(type.GetField(name, bindingFlags), typeof(TAttr)), default);
        }

        /// <summary>
        /// 获取多特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">数据名称</param>
        /// <returns>特性</returns>
        public static TAttr[] GetPropertyAttributes<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetProperty(name), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">数据名称</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>特性</returns>
        public static TAttr[] GetPropertyAttributes<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetProperty(name, bindingFlags), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr[] GetMethodAttributes<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetMethod(name), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">数据名称</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>特性</returns>
        public static TAttr[] GetMethodAttributes<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetMethod(name, bindingFlags), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr[] GetEventAttributes<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetEvent(name), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取方法特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr[] GetEventAttributes<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetEvent(name, bindingFlags), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取类型成员特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr[] GetMemberAttributes<TAttr>(Type type) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type, typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取值特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <returns>特性</returns>
        public static TAttr[] GetFieldAttributes<TAttr>(Type type, string name) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetField(name), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 获取值特性
        /// </summary>
        /// <typeparam name="TAttr">特性类型</typeparam>
        /// <param name="name">数据名称</param>
        /// <param name="type">类型</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns>特性</returns>
        public static TAttr[] GetFieldAttributes<TAttr>(Type type, string name, BindingFlags bindingFlags) where TAttr : Attribute
        {
            return Try(() => Attribute.GetCustomAttributes(type.GetField(name, bindingFlags), typeof(TAttr)).ChangeType<TAttr>(), new TAttr[] { });
        }

        /// <summary>
        /// 类型转换：数组类型转换
        /// </summary>
        /// <param name="src">数组</param>
        /// <returns>数组</returns>
        private static T[] ChangeType<T>(this Array src)
        {
            return Try(() =>
            {
                T[] tar = new T[src.Length];

                for (int i = 0; i < src.Length; i++)
                {
                    tar[i] = (T)src.GetValue(i);
                }
                return tar;
            }, default);
        }

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="value">原数据</param>
        /// <param name="conversionType">转换类型</param>
        /// <returns>转换数据</returns>
        private static object ChangeType(object value, Type conversionType)
        {
            object conversionValue = default;

            // 数据类型不匹配则开始尝试转换
            if (value != null && conversionType != null && value.GetType() != conversionType)
            {
                try
                {
                    // 检查原数据是否继承于可空接口
                    if (value.GetType() == Nullable.GetUnderlyingType(conversionType))
                    {
                        // 无需转换
                        conversionValue = value;
                        return conversionValue;
                    }

                    // 检查转换类型是否继承于可空接口
                    if (Nullable.GetUnderlyingType(conversionType) != null)
                    {
                        conversionType = Nullable.GetUnderlyingType(conversionType);

                        // 检查原数据为字符串且为空串
                        if (value.GetType() == typeof(string) && string.IsNullOrEmpty(value as string))
                        {
                            // 转换结果为空值
                            conversionValue = null;
                            return conversionValue;
                        }
                    }

                    // 尝试转换类型
                    conversionValue = Convert.ChangeType(value, conversionType);
                }
                catch
                {
                    try
                    {
                        // 尝试直接传参构造
                        conversionValue = CreateInstance<object>(conversionType, new object[] { value });
                    }
                    catch
                    {
                        // 转换失败
                    }
                }
            }
            else
            {
                conversionValue = value;
            }
            return conversionValue;
        }

        /// <summary>
        /// 判断数组索引范围是否合法
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="array">数组对象</param>
        /// <returns>是否合法范围</returns>
        private static bool InRange<T>(int index, T[] array) => InRange(index, array?.Length);

        /// <summary>
        /// 判断索引范围是否合法
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="count">对象个数/大小/长度</param>
        /// <returns>是否合法范围</returns>
        private static bool InRange(int index, int? count)
        {
            if (count == null)
            {
                return false;
            }
            if (index >= 0 && index < count)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试调用方法
        /// 如果调用抛出异常则返回规定的返回值
        /// </summary>
        /// <typeparam name="TReturn">任意类型</typeparam>
        /// <param name="action">调用方法</param>
        /// <param name="defaultReturnedValue">调用出异常时的返回值</param>
        /// <returns>调用方法返回值</returns>
        private static TReturn Try<TReturn>(ReturnedAction<TReturn> action, TReturn defaultReturnedValue)
        {
            try
            {
                return action.Invoke();
            }
            catch
            {
                return defaultReturnedValue;
            }
        }

        /// <summary>
        /// 尝试调用方法
        /// 如果调用抛出异常则返回规定的返回值
        /// </summary>
        /// <typeparam name="TReturn">任意返回值类型</typeparam>
        /// <typeparam name="TParam">任意参数类型</typeparam>
        /// <param name="action">调用方法</param>
        /// <param name="param">参数</param>
        /// <param name="defaultReturnedValue">调用出异常时的返回值</param>
        /// <returns>调用方法返回值</returns>
        private static TReturn Try<TReturn, TParam>(ReturnedAction<TReturn, TParam> action, TParam param, TReturn defaultReturnedValue)
        {
            try
            {
                return action.Invoke(param);
            }
            catch
            {
                return defaultReturnedValue;
            }
        }

        /// <summary>
        /// 尝试调用方法
        /// </summary>
        /// <param name="action">调用方法</param>
        /// <returns>异常信息</returns>
        private static Exception Try(Action action)
        {
            try
            {
                action.Invoke();
                return default;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
