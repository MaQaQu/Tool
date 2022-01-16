using System;

namespace YouiToolkit.Design
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataGridColumnAttribute : Attribute
    {
        public const string DisplayNameDefault = "Name";
        public string DisplayName { get; set; } = DisplayNameDefault;
        public string Width { get; set; } = "auto";
        public int MinWidth { get; set; } = 0;
        public bool ReadOnly { get; set; } = false;
        public int Order { get; set; } = 0;

        public bool CanUserReorder { get; set; } = false;
        public bool CanUserResize { get; set; } = true;
        public bool CanUserSort { get; set; } = false;

        public Type RenderType { get; set; } = null;
        public Type RenderStyleType { get; set; } = null;

        public Type EditType { get; set; } = null;
        public Type EditStyleType { get; set; } = null;

        public DataGridColumnAttribute(string displayName = null) : this()
        {
            DisplayName = displayName ?? DisplayNameDefault;
        }

        public DataGridColumnAttribute()
        {
        }
    }
}
