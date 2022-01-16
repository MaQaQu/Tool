namespace YouiToolkit.Models
{
    /// <summary>
    /// 避障数据储存
    /// </summary>
    internal class AvoidObstacleStock
    {
        /// <summary>
        /// 存储区最大数量
        /// </summary>
        public const int StockMax = 16;

        /// <summary>
        /// 存储数组
        /// </summary>
        public AvoidObstacleDragPointSetBlock[] Stocks = null;

        /// <summary>
        /// 当前编辑中的储存下标
        /// </summary>
        public int CurrentIndex { get; private set; } = 0;

        /// <summary>
        /// 当前编辑中的储存对象
        /// </summary>
        public AvoidObstacleDragPointSetBlock Current
            => (CurrentIndex >= 0 && CurrentIndex < StockMax) ? Stocks[CurrentIndex] : null;

        /// <summary>
        /// 下标访问器
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>储存对象</returns>
        public AvoidObstacleDragPointSetBlock this[int index] => Stocks[index];

        /// <summary>
        /// 长度
        /// </summary>
        public int Length => Stocks.Length;

        /// <summary>
        /// 构造
        /// </summary>
        public AvoidObstacleStock()
        {
            Clear();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            Stocks = new AvoidObstacleDragPointSetBlock[StockMax];

            for (int i = default; i < StockMax; i++)
            {
                Stocks[i] = new AvoidObstacleDragPointSetBlock()
                {
                    Index = i,
                };
            }
        }

        /// <summary>
        /// 指定下标切换到
        /// ※进行数据往外拷贝操作
        /// </summary>
        /// <param name="target">往外拷贝目标</param>
        /// <param name="index">指定下标</param>
        public void SwitchTo(AvoidObstacleDragPointSetBlock target, int index)
        {
            CurrentIndex = index;
            this[index].CopyTo(target);
        }

        /// <summary>
        /// 指定下标切换自
        /// ※进行数据往内拷贝操作
        /// ※下标可空：下标为空时进行全拷贝
        /// </summary>
        /// <param name="source">往内拷贝数据源</param>
        /// <param name="index">指定下标</param>
        public void SwitchFrom(AvoidObstacleDragPointSetBlock source, int? index)
        {
            AvoidObstacleAreaTypeAs? typeAs = source.Actived?.TypeAs;

            if (typeAs != null)
            {
                source.DragPointSets[(int)typeAs].CopyTo(this[index ?? CurrentIndex].DragPointSets[(int)typeAs]);
            }
            else
            {
                source.CopyTo(this[CurrentIndex]);
            }
        }

        /// <summary>
        /// 转序列化中间对象
        /// </summary>
        public AvoidObstacleDragPointSerializeObject ToSerializeObject()
            => AvoidObstacleDragPointSerializeObject.Create(this);
    }
}
