namespace YouiToolkit.Design
{
    public class ThumbnailImageShortCutEventArgs : ShortCutEventArgs
    {
        public int Index { get; set; } = -1;

        public static new ThumbnailImageShortCutEventArgs Empty(int index) => new ThumbnailImageShortCutEventArgs(ShortCutType.None, index);
        public static new ThumbnailImageShortCutEventArgs Copy(int index) => new ThumbnailImageShortCutEventArgs(ShortCutType.Copy, index);
        public static new ThumbnailImageShortCutEventArgs Paste(int index) => new ThumbnailImageShortCutEventArgs(ShortCutType.Paste, index);
        public static new ThumbnailImageShortCutEventArgs Cut(int index) => new ThumbnailImageShortCutEventArgs(ShortCutType.Cut, index);
        public static new ThumbnailImageShortCutEventArgs Add(int index) => new ThumbnailImageShortCutEventArgs(ShortCutType.Add, index);
        public static new ThumbnailImageShortCutEventArgs Remove(int index) => new ThumbnailImageShortCutEventArgs(ShortCutType.Remove, index);

        public ThumbnailImageShortCutEventArgs(ShortCutType type, int index) : this(type)
        {
            Index = index;
        }

        public ThumbnailImageShortCutEventArgs(ShortCutType type) : base(type)
        {
        }
    }
}
