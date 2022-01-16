using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouiToolkit.Assist
{
    public class PointGridMap
    {
        //private static int SubMapStep = 10;
        //private static int SubMapMaxCount = 100;

        //private static int SubMapStep = 3;
        //private static int SubMapMaxCount = 9;

        private static int SubMapStep = 2;
        private static int SubMapMaxCount = 4;

        public int SubMapCount { get; private set; } = 0;          //子图个数
        public int[] SubPointCount { get; private set; } = null;   //每个子图中点的个数
        public PointGridMap[] SubMap { get; private set; } = null; //子图数组（根子图没有）
        public float MinX { get; private set; } = 0;
        public float MinY { get; private set; } = 0;
        public float MaxX { get; private set; } = 0;
        public float MaxY { get; private set; } = 0;
        public float Resolution { get; private set; } = 0;
        public bool RootLevel { get; private set; } = false;

        public PointGridMap(float x, float y, float res)
        {
            MinX = x;
            MinY = y;

            MaxX = x + SubMapStep * res;
            MaxY = y + SubMapStep * res;

            Resolution = res;

            if (Resolution <= 1)
            {
                RootLevel = true;
                SubPointCount = new int[SubMapMaxCount];
                for (int i = 0; i < SubMapMaxCount; i++)
                {
                    SubPointCount[i] = 0;
                }
            }
            else
            {
                SubMap = new PointGridMap[SubMapMaxCount];
                SubPointCount = new int[SubMapMaxCount];
                for (int i = 0; i < SubMapMaxCount; i++)
                {
                    SubPointCount[i] = 0;
                    SubMap[i] = null;
                }
            }
        }

        public bool InsertPoint(float x, float y)
        {
            if (x < MinX || x >= MaxX || y < MinY || y >= MaxY || x.Equals(float.NaN) || y.Equals(float.NaN))
            {
                return false;
            }

            int row = (int)Math.Floor((y - MinY) / Resolution);
            int col = (int)Math.Floor((x - MinX) / Resolution);

            int index = row * SubMapStep + col;

            if (RootLevel)
            {
                if (SubPointCount[index] <= 0)
                {
                    ++SubMapCount;
                }

                ++SubPointCount[index];
            }
            else
            {
                if (index >= SubPointCount.Length)
                {
                    return false;
                }

                if (SubPointCount[index] <= 0)
                {
                    SubMap[index] = new PointGridMap(MinX + col * Resolution, MinY + row * Resolution, Resolution / SubMapStep);
                    ++SubMapCount;
                }

                if (SubMap[index]?.InsertPoint(x, y) ?? false)
                {
                    ++SubPointCount[index];
                }
            }
            return true;
        }

        public bool RemovePoint(float x, float y)
        {
            if (x < MinX || x >= MaxX || y < MinY || y >= MaxY || x.Equals(float.NaN) || y.Equals(float.NaN))
            {
                return false;
            }

            int row = (int)Math.Floor((y - MinY) / Resolution);
            int col = (int)Math.Floor((x - MinX) / Resolution);

            int index = row * SubMapStep + col;

            if (SubPointCount[index] > 0)
            {
                if (RootLevel)
                {
                    --SubPointCount[index];
                    if (SubPointCount[index] <= 0)
                    {
                        --SubMapCount;
                    }
                }
                else
                {
                    --SubPointCount[index];
                    SubMap[index].RemovePoint(x, y);

                    if (SubPointCount[index] <= 0)
                    {
                        SubMap[index] = null;
                        --SubMapCount;
                    }
                }
            }
            return true;
        }

        public int GetSubMapStep()
        {
            return SubMapStep;
        }

        public int GetSubMapMaxCount()
        {
            return SubMapMaxCount;
        }
    }
}
