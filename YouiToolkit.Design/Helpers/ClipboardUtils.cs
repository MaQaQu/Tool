using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using YouiToolkit.Utils;

namespace YouiToolkit.Design
{
    public static class ClipboardUtils
    {
        /// <summary>
        /// 全局剪贴板数据对象
        /// </summary>
        public static DataObject DataObject = null;

        public static void Begin()
        {
            DataObject = new DataObject();
        }

        public static void End()
        {
            Clipboard.SetDataObject(DataObject);
        }

        public static void Clear()
        {
            DataObject = null;
        }

        public static void AddData(ClipboardForamt format, object data)
        {
            DataObject ??= new DataObject();
            if (data != null)
            {
                DataObject.SetData(ConvertForamt(format), data);
            }
        }

        public static void SetData(ClipboardForamt format, object data)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetData(ConvertForamt(format), data);
            Clipboard.SetDataObject(dataObject);
        }

        public static object GetData(ClipboardForamt format)
            => Clipboard.GetData(ConvertForamt(format));

        public static void SetBitmap(Bitmap bitmap)
            => SetData(ClipboardForamt.Bitmap, bitmap);

        public static void AddBitmap(Bitmap bitmap)
            => AddData(ClipboardForamt.Bitmap, bitmap);

        public static Bitmap GetBitmap()
            => (GetData(ClipboardForamt.Bitmap) as BitmapSource)?.ToBitmap();

        public static void AddBitmapBase64(Bitmap bitmap)
            => AddData(ClipboardForamt.StringFormat, ImageUtil.ConvertToBase64(bitmap, ImageFormat.Png));

        public static void SetBitmapBase64(Bitmap bitmap)
            => SetData(ClipboardForamt.StringFormat, ImageUtil.ConvertToBase64(bitmap));

        public static Bitmap GetBitmapBase64()
            => ImageUtil.GetFromBase64(GetData(ClipboardForamt.StringFormat) as string);

        [DebuggerStepThrough]
        internal static string ConvertForamt(ClipboardForamt format)
        {
            FieldInfo fieldInfo = typeof(ClipboardForamt).GetField(format.ToString(), BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo != null)
            {
                CommentAttribute attr = fieldInfo.GetCustomAttribute<CommentAttribute>();
                
                if (attr != null && !string.IsNullOrWhiteSpace(attr.Comment))
                {
                    return attr.Comment;
                }
            }
            return ClipboardForamt.Text.ToString();
        }
    }

    /// <summary>
    /// <seealso cref="DataFormats"/>
    /// </summary>
    [Obfuscation]
    public enum ClipboardForamt
    {
        /// <summary>
        /// <seealso cref="DataFormats.Text"/>
        /// </summary>
        [Comment("Text")]
        Text,

        /// <summary>
        /// <seealso cref="DataFormats.UnicodeText"/>
        /// </summary>
        [Comment("UnicodeText")]
        UnicodeText,

        /// <summary>
        /// <seealso cref="DataFormats.Dib"/>
        /// </summary>
        [Comment("DeviceIndependentBitmap")]
        Dib,

        /// <summary>
        /// <seealso cref="DataFormats.Bitmap"/>
        /// </summary>
        [Comment("Bitmap")]
        Bitmap,

        /// <summary>
        /// <seealso cref="DataFormats.EnhancedMetafile"/>
        /// </summary>
        [Comment("EnhancedMetafile")]
        EnhancedMetafile,

        /// <summary>
        /// <seealso cref="DataFormats.MetafilePicture"/>
        /// </summary>
        [Comment("MetaFilePict")]
        MetafilePicture,

        /// <summary>
        /// <seealso cref="DataFormats.SymbolicLink"/>
        /// </summary>
        [Comment("SymbolicLink")]
        SymbolicLink,

        /// <summary>
        /// <seealso cref="DataFormats.Dif"/>
        /// </summary>
        [Comment("DataInterchangeFormat")]
        Dif,

        /// <summary>
        /// <seealso cref="DataFormats.Tiff"/>
        /// </summary>
        [Comment("TaggedImageFileFormat")]
        Tiff,

        /// <summary>
        /// <seealso cref="DataFormats.OemText"/>
        /// </summary>
        [Comment("OEMText")]
        OemText,

        /// <summary>
        /// <seealso cref="DataFormats.Palette"/>
        /// </summary>
        [Comment("Palette")]
        Palette,

        /// <summary>
        /// <seealso cref="DataFormats.PenData"/>
        /// </summary>
        [Comment("PenData")]
        PenData,

        /// <summary>
        /// <seealso cref="DataFormats.Riff"/>
        /// </summary>
        [Comment("RiffAudio")]
        Riff,

        /// <summary>
        /// <seealso cref="DataFormats.WaveAudio"/>
        /// </summary>
        [Comment("WaveAudio")]
        WaveAudio,

        /// <summary>
        /// <seealso cref="DataFormats.FileDrop"/>
        /// </summary>
        [Comment("FileDrop")]
        FileDrop,

        /// <summary>
        /// <seealso cref="DataFormats.Locale"/>
        /// </summary>
        [Comment("Locale")]
        Locale,

        /// <summary>
        /// <seealso cref="DataFormats.Html"/>
        /// </summary>
        [Comment("HTML Format")]
        Html,

        /// <summary>
        /// <seealso cref="DataFormats.Rtf"/>
        /// </summary>
        [Comment("Rich Text Format")]
        Rtf,

        /// <summary>
        /// <seealso cref="DataFormats.CommaSeparatedValue"/>
        /// </summary>
        [Comment("CSV")]
        CommaSeparatedValue,

        /// <summary>
        /// <seealso cref="typeof(string).FullName"/>
        /// <seealso cref="DataFormats.StringFormat"/>
        /// </summary>
        [Comment("System.String")]
        StringFormat,

        /// <summary>
        /// <seealso cref="DataFormats.PersistentObject"/>
        /// </summary>
        [Comment("PersistentObject")]
        Serializable,

        /// <summary>
        /// <seealso cref="DataFormats.Xaml"/>
        /// </summary>
        [Comment("Xaml")]
        Xaml,

        /// <summary>
        /// <seealso cref="DataFormats.XamlPackage"/>
        /// </summary>
        [Comment("XamlPackage")]
        XamlPackage,
    }
}
