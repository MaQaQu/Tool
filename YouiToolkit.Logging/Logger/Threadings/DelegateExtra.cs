using System;

namespace YouiToolkit.Logging
{
    /// <summary>
    /// 封装方法：指定条件断言
    /// </summary>
    /// <returns>判断结果</returns>
    public delegate bool Predicate();

    /// <summary>
    /// 封装方法：不带参带返回值
    /// </summary>
    /// <typeparam name="TReturn">任意返回值类型</typeparam>
    public delegate TReturn ReturnedAction<TReturn>();

    /// <summary>
    /// 封装方法：带参带返回值
    /// </summary>
    /// <typeparam name="TReturn">任意返回值类型</typeparam>
    /// <typeparam name="TParam">任意参数类型</typeparam>
    /// <param name="param">参数</param>
    public delegate TReturn ReturnedAction<TReturn, TParam>(TParam param);

    /// <summary>
    /// 封装方法：带参无返回值
    /// </summary>
    /// <param name="param">参数对象</param>
    public delegate void ParameterizedAction(object param);

    /// <summary>
    /// 委托的扩展方法
    /// </summary>
    public class DelegateExtra
    {
        #region [公有 方法]
        /// <summary>
        /// Action<T>转换成Action类型
        /// </summary>
        /// <param name="action">Action<T>对象</param>
        /// <returns>Action对象</returns>
        public static Action ToAction<T>(Action<T> action, T param)
        {
            return () => action(param);
        }

        /// <summary>
        /// Action转换成Action<T>类型
        /// </summary>
        /// <param name="action">Action对象</param>
        /// <returns>Action<T>对象</returns>
        public static Action<T> FromAction<T>(Action action)
        {
            return (x) => action();
        }
        #endregion
    }
}
