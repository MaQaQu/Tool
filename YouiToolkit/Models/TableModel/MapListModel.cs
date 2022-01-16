using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using YouiToolkit.Design;
using static YouiToolkit.Design.DataGridButtonGroupColumn;

namespace YouiToolkit.Models
{
    [Obfuscation]
    public class MapListModel
    {
        [DataGridColumn("名称", Order = 1, ReadOnly = true, Width = "*")]
        public string Name { get; set; }

        [DataGridColumn("类型", Order = 2, ReadOnly = true, Width = "150")]
        public string MapTpye { get; set; }

        [DataGridColumn("操作", Order = 3, RenderType = typeof(DataGridButtonGroupColumn), RenderStyleType = typeof(MapListOperationStyle), Width = "326")]
        public string Operation { get; set; }

        /// <summary>
        /// 构造操作按钮们
        /// </summary>
        public static string CreateOperation(string[] operations) => string.Join(SplitComma.ToString(), operations);
        public static string CreateOperation()
        {
            return MapListModel.CreateOperation(new string[]
            {
                "&#xe90e; 编辑",
                "&#xe982; 录制",
                "&#xea0e; 导出地图",
                "&#xe927; 删除",
            });
        }

        /// <summary>
        /// 构造
        /// </summary>
        public MapListModel() : this(null)
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        public MapListModel(string name)
        {
            Name = name;
            MapTpye = "激光地图";
            Operation = MapListModel.CreateOperation();
        }

        /// <summary>
        /// 列名
        /// </summary>
        [DataGridColumnIgnore]
        public static string[] Names => TableModelHelper.GetNames();

        /// <summary>
        /// 列下标
        /// </summary>
        public static int IndexOf(string name) => TableModelHelper.IndexOf(name);

        /// <summary>
        /// 下标运算符重载
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        [DataGridColumnIgnore]
        public object this[int index]
        {
            get => TableModelHelper.Get(this, index);
            set => TableModelHelper.Set(this, index, value);
        }

        /// <summary>
        /// 下标运算符
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public object Get(string propName) => TableModelHelper.Get(this, propName);
        public void Set(string propName, object value) => TableModelHelper.Set(this, propName, value);
    }

    /// <summary>
    /// 地图操作
    /// </summary>
    internal enum MapListOperation
    {
        /// <summary>
        /// 编辑
        /// </summary>
        Edit,

        /// <summary>
        /// 录制
        /// </summary>
        Capture,
        
        /// <summary>
        /// 导出地图
        /// </summary>
        ExportMap,

        /// <summary>
        /// 删除
        /// </summary>
        Remove,
    }

    /// <summary>
    /// 操作按钮样式
    /// </summary>
    public class MapListOperationStyle : DataGridButtonGroupColumnStyle
    {
        public override CornerRadius CornerRadius { get; set; }
        public override IList<Brush> Backgrounds { get; set; }
        public override IList<Brush> Foregrounds { get; set; }
        public override IList<FontFamily> FontFamilies { get; set; }
        public override IList<Brush> HoverBrushes { get; set; }
        public override IList<int> Widths { get; set; }

        public MapListOperationStyle()
        {
            Brush brushB = BrushUtils.GetBrush("#5A94AD");
            Brush brushBh = BrushUtils.GetBrushHighlight(brushB);
            Brush brushG = BrushUtils.GetBrush("#5AA4AD");
            Brush brushGh = BrushUtils.GetBrushHighlight(brushG);
            Brush brushR = BrushUtils.GetBrush("#DA5F5F");
            Brush brushRh = BrushUtils.GetBrushHighlight(brushR);
            Brush brushW = BrushUtils.GetBrush("White");

            CornerRadius = new CornerRadius(5);
            Backgrounds = new List<Brush>()
            {
                brushB, // 编辑
                brushG, // 录制
                brushB, // 导出地图
                brushR, // 删除
            };
            Foregrounds = new List<Brush>()
            {
                brushW,
                brushW,
                brushW,
                brushW,
            };
            HoverBrushes = new List<Brush>()
            {
                brushBh,
                brushGh,
                brushBh,
                brushRh,
            };
            FontFamilies = new List<FontFamily>()
            {
            };
            Widths = new List<int>()
            {
                -1,
                -1,
                95,
                -1,
            };
        }
    }
}
