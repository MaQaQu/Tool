using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using YouiToolkit.Utils;

namespace YouiToolkit.Assist
{
    public class ToolkitAssistDialogBase : ObservableObjectBase, IToolkitAssistSubCtrl
    {
        /// <summary>
        /// 父控制器
        /// </summary>
        public ToolkitAssist Assist { get; set; } = null;

        protected readonly ConcurrentDictionary<string, bool> status = new ConcurrentDictionary<string, bool>();
        public ConcurrentDictionary<string, bool> Status => status;
        public override bool Async => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual string NewGuid() => System.Guid.NewGuid().ToString("N");

        protected override void RaisePropertyChanged(object oldValue, object newValue, string propertyName = null, bool? async = null)
        {
            SetDialogStatus(propertyName, true);
            base.RaisePropertyChanged(oldValue, newValue, propertyName, async);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetDialogStatus(string propertyName, bool value)
            => status[propertyName] = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetDialogStatusX(string propertyName, bool value)
        {
            status[propertyName] = value;
            if (!value)
            {
                // 移除UID
                status.TryRemove(propertyName, out _);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool GetDialogStatus(string propertyName)
            => status[propertyName];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool CallCore(string propertyName, Action trans, int? millisecondsTimeout = 4000)
        {
            SetDialogStatus(propertyName, false);
            trans.Invoke();
            bool spinFunc()
            {
                return GetDialogStatus(propertyName);
            }
            bool spinRet = SpinWaiter.SpinUntil(spinFunc, millisecondsTimeout);
            return spinRet;
        }
    }
}
