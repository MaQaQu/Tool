using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using YouiToolkit.Utils;

namespace YouiToolkit.Design
{
    [Obfuscation]
    public static class TableModelHelper
    {
        /// <summary>
        /// 列名
        /// </summary>
        public static string[] GetNames()
        {
            StackFrame stackFrame = new StackFrame(1, true);
            Type type = stackFrame.GetMethod().DeclaringType;
            return GetNames(type);
        }

        /// <summary>
        /// 列名
        /// </summary>
        public static string[] GetNames(Type type)
            => ReflectionHelper.GetModelSortNames(type).ToArray();

        /// <summary>
        /// 列下标
        /// </summary>
        public static int IndexOf(string name)
        {
            StackFrame stackFrame = new StackFrame(1, true);
            Type type = stackFrame.GetMethod().DeclaringType;
            List<string> names = new List<string>(GetNames(type));

            return names.IndexOf(name);
        }

        /// <summary>
        /// 下标运算符重载：获取
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static object Get(object model, int index)
        {
            Type type = model.GetType();
            string name = GetNames(type)[index];
            PropertyInfo prop = type.GetProperty(name);

            if (prop != null && prop.CanRead)
            {
                return prop.GetValue(model);
            }
            throw new AmbiguousMatchException();
        }

        /// <summary>
        /// 下标运算符重载：设定
        /// </summary>
        public static void Set(object model, int index, object value)
        {
            Type type = model.GetType();
            string name = GetNames(type)[index];
            PropertyInfo prop = type.GetProperty(name);

            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(model, value);
            }
        }

        /// <summary>
        /// 下标运算符重载：获取
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static object Get(object model, string propName)
        {
            Type type = model.GetType();
            PropertyInfo prop = type.GetProperty(propName);

            if (prop != null && prop.CanRead)
            {
                return prop.GetValue(model);
            }
            throw new AmbiguousMatchException();
        }

        /// <summary>
        /// 下标运算符重载：设定
        /// </summary>
        public static void Set(object model, string propName, object value)
        {
            Type type = model.GetType();
            PropertyInfo prop = type.GetProperty(propName);

            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(model, value);
            }
        }
    }
}
