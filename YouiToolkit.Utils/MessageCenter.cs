using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using YouiToolkit.Logging;

namespace YouiToolkit.Utils
{
    public class MessageCenter
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static MessageCenter Instance { get; private set; } = new MessageCenter();

        /// <summary>
        /// 消息受付事件辞典
        /// </summary>
        public ConcurrentDictionary<string, List<Action<MessageCenterEventArgs>>> MessageRecived = new ConcurrentDictionary<string, List<Action<MessageCenterEventArgs>>>();

        /// <summary>
        /// 构造
        /// </summary>
        protected MessageCenter()
        {
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="eventArgs">消息事件参数</param>
        public static void Publish(string name, MessageCenterEventArgs eventArgs)
        {
            if (Instance.MessageRecived.ContainsKey(name))
            {
                List<Action<MessageCenterEventArgs>> actionList = Instance.MessageRecived[name];

                foreach (Action<MessageCenterEventArgs> action in actionList)
                {
                    try
                    {
                        action?.Invoke(eventArgs);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="message">消息</param>
        public static void Publish(string name, string message)
        {
            Publish(name, new MessageCenterEventArgs(name, message));
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="message">消息</param>
        /// <param name="param">参数</param>
        public static void Publish(string name, string message, object param)
        {
            Publish(name, new MessageCenterEventArgs(name, message, param));
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="message">消息</param>
        /// <param name="nameCallback">回调消息名称</param>
        /// <param name="param">参数</param>
        public static void Publish(string name, string message, string nameCallback, object param)
        {
            Publish(name, new MessageCenterEventArgs(name, message, nameCallback, param));
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="exception">异常消息</param>
        public static void Publish(string name, Exception exception)
        {
            Publish(name, new MessageCenterEventArgs(name, exception));
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="action">回调方法</param>
        public static void Subscribe(string name, Action<MessageCenterEventArgs> action)
        {
            if (!Instance.MessageRecived.ContainsKey(name))
            {
                Instance.MessageRecived.TryAdd(name, new List<Action<MessageCenterEventArgs>>());
            }
            foreach (var subscribedAction in Instance.MessageRecived[name])
            {
                if (subscribedAction == action)
                {
                    Logger.Error($"Subscribed action named \"{name}\":\"{action}\".");
                    return;
                }
            }
            Instance.MessageRecived[name].Add(action);
        }

        /// <summary>
        /// 取消订阅消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="action">回调方法</param>
        public static void Unsubscribe(string name, Action<MessageCenterEventArgs> action)
        {
            if (Instance.MessageRecived.ContainsKey(name))
            {
                foreach (var subscribedAction in Instance.MessageRecived[name])
                {
                    if (subscribedAction == action)
                    {
                        Instance.MessageRecived[name].Remove(subscribedAction);
                        return;
                    }
                }

                if (Instance.MessageRecived[name].Count <= 0)
                {
                    List<Action<MessageCenterEventArgs>> removedActionList = null;
                    bool ret = Instance.MessageRecived.TryRemove(name, out removedActionList);

                    removedActionList = null;
                }
            }
        }
    }
}
