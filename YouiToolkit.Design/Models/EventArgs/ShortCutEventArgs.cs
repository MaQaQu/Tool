using System;

namespace YouiToolkit.Design
{
    public class ShortCutEventArgs : EventArgs
    {
        public static new ShortCutEventArgs Empty => new ShortCutEventArgs(ShortCutType.None);
        public static ShortCutEventArgs Copy => new ShortCutEventArgs(ShortCutType.Copy);
        public static ShortCutEventArgs Paste => new ShortCutEventArgs(ShortCutType.Paste);
        public static ShortCutEventArgs Cut => new ShortCutEventArgs(ShortCutType.Cut);
        public static ShortCutEventArgs Add => new ShortCutEventArgs(ShortCutType.Add);
        public static ShortCutEventArgs Remove => new ShortCutEventArgs(ShortCutType.Remove);

        public ShortCutType Type = ShortCutType.None;

        public ShortCutEventArgs(ShortCutType type)
        {
            Type = type;
        }
    }

    public enum ShortCutType
    {
        None,
        Copy,
        Paste,
        Cut,
        Add,
        Remove,
        Clear,
    }
}
