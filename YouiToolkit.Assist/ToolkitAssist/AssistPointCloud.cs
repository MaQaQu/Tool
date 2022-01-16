using SuperMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using YouiToolkit.Logging;

namespace YouiToolkit.Assist
{
    public class AssistPointCloud
    {
        public int MaxCount { get; private set; } = ushort.MaxValue;
        public int Count { get; private set; } = 0;
        public GraphPoint[] MapPoints { get; private set; }
        public GraphPoint[] MapPointsPrior { get; private set; } = null;
        public PointGridMap GridMapPrior { get; private set; }

        #region [先验点云图缓存数据]
        public object MapPointsPriorMonitorLock { get; private set; } = new object();
        public byte[] MapPointsPriorCache { get; private set; } = null;
        public int MapPointsPriorCacheIndex { get; private set; } = default;
        public float MapPointsPriorPX { get; private set; } = default;
        public float MapPointsPriorPY { get; private set; } = default;
        public float MapPointsPriorPA { get; private set; } = default;
        public float MapPointsPriorResolution { get; private set; } = default;
        #endregion

        public AssistPointCloud(int maxCount)
        {
            MaxCount = maxCount;
            MapPoints = new GraphPoint[MaxCount];
            GridMapPrior = new PointGridMap(-1310720, -1310720, 1310720);
        }

        public void SetCount(int count)
        {
            if (count >= 0 && count < MaxCount)
            {
                Count = count;
            }
        }

        public void CacheMapPriorConfig(float px, float py, float pa, float resolution)
        {
            MapPointsPriorPX = px;
            MapPointsPriorPY = py;
            MapPointsPriorPA = pa;
            MapPointsPriorResolution = resolution;
        }

        /// <summary>
        /// 导出地图文件路劲
        /// <remark>
        /// ※同时作为标志位：
        /// empty|null >> 无请求导出地图
        /// other      >> 请求导出地图
        /// </remark>
        /// </summary>
        public string ExportMapPriorPath = null;

        public void CacheMapPrior(byte[] data, int maxLength = -1)
        {
            if (maxLength > 0)
            {
                MapPointsPriorCache = new byte[maxLength];
                MapPointsPriorCacheIndex = 0;
            }
            if (MapPointsPriorCache != null)
            {
                try
                {
                    Buffer.BlockCopy(data, 0, MapPointsPriorCache, MapPointsPriorCacheIndex, data.Length);
                    MapPointsPriorCacheIndex += data.Length;

                    if (MapPointsPriorCacheIndex >= MapPointsPriorCache.Length)
                    {
                        Task.Run(() =>
                        {
                            if (string.IsNullOrEmpty(ExportMapPriorPath))
                            {
                                lock (MapPointsPriorMonitorLock)
                                {
                                    // 加载地图
                                    MapPointsPrior = LoadMapPrior(MapPointsPriorCache, MapPointsPriorPX, MapPointsPriorPY, MapPointsPriorPA, MapPointsPriorResolution);
                                }
                            }
                            else
                            {
                                // 导出地图
                                SaveMapPrior(ExportMapPriorPath, MapPointsPriorCache);
                                ExportMapPriorPath = null;
                            }
                            ClearMapPriorCache();
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Fatal(e.ToString());
                }
            }
        }

        private void ClearMapPriorCache()
        {
            MapPointsPriorCache = null;
            CacheMapPriorConfig(default, default, default, default);
        }

        public bool IsMapPriorEmpty()
        {
            lock (MapPointsPriorMonitorLock)
            {
                return MapPointsPrior == null;
            }
        }

        public void ClearMapPrior()
        {
            lock (MapPointsPriorMonitorLock)
            {
                MapPointsPrior = null;
            }
            Count = 0;
            MapPoints = new GraphPoint[MaxCount];
            GridMapPrior = new PointGridMap(-1310720, -1310720, 1310720);
            ClearMapPriorCache();
        }

        public bool SaveMapPrior(string path, byte[] png)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.WriteAllBytes(path, png);
                return true;
            }
            catch (Exception e)
            {
                Logger.Fatal(e.ToString());
            }
            return false;
        }

        public GraphPoint[] LoadMapPrior(byte[] png, float px, float py, float pa, float resolution)
        {
            List<GraphPoint> list = new List<GraphPoint>(png?.Length ?? default);

            try
            {
                ulong count = default;
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using var stream = new MemoryStream(png);
                using var image = new Bitmap(stream);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                Task.Run(() =>
                {
                    string appUserPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string path = $@"{Path.Combine(appUserPath, "YouiToolkit")}\MapPrior.png";
                    
                    File.WriteAllBytes(path, png);
                    Logger.Info($"[LoadMapPrior] Bitmap file `{path}` saved.");
                });
                
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                
                unsafe
                {
#if true
                    Parallel.For(0, data.Height, (y) =>
                    {
                        byte* ptr = (byte*)(data.Scan0) + y * data.Stride;

                        for (int x = 0; x < data.Width; x++)
                        {
                            if (*ptr == 0)
                            {
                                float ppx = x * resolution + px;
                                float ppy = y * resolution + py;

                                GridMapPrior.InsertPoint(ppx, ppy);
                                count++;
                            }
                            ptr += 3;
                        }
                    });
#else
                    byte* ptr = (byte*)(data.Scan0);

                    for (int y = 0; y < data.Height; y++)
                    {
                        for (int x = 0; x < data.Width; x++)
                        {
                            if (*ptr == 0)
                            {
                                float ppx = x * resolution + px;
                                float ppy = y * resolution + py;

                                GridMapPrior.InsertPoint(ppx, ppy);
                                count++;
                            }
                            ptr += 3;
                        }
                        ptr += data.Stride - data.Width * 3;
                    }
#endif
                }

                stopwatch.Stop();
                Logger.Info($"[LoadMapPrior] Length={png.Length}bytes, Pixel={image.Width}x{image.Height}, Points={count}, Elapsed={stopwatch.ElapsedMilliseconds / 1000f}s");
            }
            catch (Exception e)
            {
                Logger.Fatal(e.ToString());
            }
            return list.ToArray();
        }

        public void DebugMapPrior(string pngPath, float px = default, float py = default, float pa = default, float resolution = default)
        {
            try
            {
                byte[] png = File.ReadAllBytes(pngPath);

                lock (MapPointsPriorMonitorLock)
                {
                    MapPointsPrior = LoadMapPrior(png, px, py, pa, resolution);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }
    }
}
