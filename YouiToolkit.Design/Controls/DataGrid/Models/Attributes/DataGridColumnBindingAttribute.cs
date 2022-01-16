using System;
using System.Windows.Data;

namespace YouiToolkit.Design
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataGridColumnBindingAttribute : Attribute
    {
        /// <summary>
        /// Custom binding mode.
        /// </summary>
        public DataGridColumnBindingAttribute()
        {
            BindingMode = BindingMode.TwoWay;
            UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
        }

        /// <summary>
        /// Custom binding mode.
        /// </summary>
        public DataGridColumnBindingAttribute(BindingMode bindingMode)
        {
            BindingMode = bindingMode;
            UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
        }

        /// <summary>
        /// Custom binding mode.
        /// </summary>
        public DataGridColumnBindingAttribute(BindingMode bindingMode, UpdateSourceTrigger updateSourceTrigger)
        {
            BindingMode = bindingMode;
            UpdateSourceTrigger = updateSourceTrigger;
        }

        public BindingMode BindingMode { get; set; }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
    }
}
