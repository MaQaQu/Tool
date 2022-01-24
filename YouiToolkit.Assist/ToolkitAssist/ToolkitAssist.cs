using SuperMath;
using SuperPortLibrary;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using YouiToolkit.Logging;

namespace YouiToolkit.Assist
{
    public class ToolkitAssist : IDisposable
    {
        private const int PortNo = 59488;

        public event Action<object, bool> ConnectStatusChanged;

        public ToolkitAssistDialog Dialog { get; private set; }
        public ToolkitAssistReporter Reporter { get; private set; }
        public ToolkitAssistStatus Status { get; private set; } = null;
        internal CommTCPClient TcpPort { get; set; } = null;

        private bool LoopRunning { get; set; } = false;
        private Thread LoopThread { get; set; } = null;
        private int LoopInterval { get; set; } = 10;
        private bool MapGenRunning { get; set; } = false;
        private Thread MapGenThread { get; set; } = null;
        private int MapGenInterval { get; set; } = 100;
        private int SessionSeed { get; set; } = 1;
        private int MappingStatusCounter { get; set; } = 10;
        private int PointCloudCounter { get; set; } = 0;
        private int PoseListCounter { get; set; } = 5;
        private int PoseGraphCounter { get; set; } = 0;
        private ConcurrentQueue<int> PoseChangedIds { get; set; } = new ConcurrentQueue<int>();

        private Thread MariaDBThread { get; set; } = null;
        public static SuperPort.CommMariaDB commMariaDB { get; private set; }

        public bool Disposed { get; private set; } = false;
        public bool HasIP { get; private set; } = false;
        public IPAddress IP { get; private set; } = IPAddress.None;
        public bool IsConnected { get; private set; } = false;

        public float BaseAxisLength { get; private set; } = 2000;

        private ToolkitAssist()
        {
            Status = new ToolkitAssistStatus();

            TcpPort = new CommTCPClient();
            TcpPort.SetProtocol(new AssistProtocol());
            TcpPort.DataReceived += TcpPort_DataReceived;
            TcpPort.ConnectionChanged += (s, e) =>
            {
                if (TcpPort.Connected)
                {
                    IsConnected = true;
                    ConnectStatusChanged?.Invoke(s, true);
                }
                else
                {
                    IsConnected = false;
                    ConnectStatusChanged?.Invoke(s, false);
                };
            };

            Dialog = new ToolkitAssistDialog(this);
            Reporter = new ToolkitAssistReporter(this);
        }

        public static ToolkitAssist GetInstance() => new ToolkitAssist();

        public void Activate()
        {
            StartThread();
        }

        public void SetIP(IPAddress ip)
        {
            IP = ip;
            HasIP = true;
            TcpPort.SetServerEndPoint(IP, PortNo);
        }

        public void SetIP(string ipstr = null)
        {
            if (IPAddress.TryParse(ipstr, out IPAddress ip))
            {
                SetIP(ip);
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;

                StopThread();
            }
        }

        public void SetBaseAxisLength(float length)
        {
            if (length > 0)
            {
                BaseAxisLength = length;
            }
        }

        private void StartThread()
        {
            LoopThread = new Thread(MainLoop);
            LoopThread.IsBackground = true;
            LoopRunning = true;
            LoopThread.Start();

            MapGenThread = new Thread(MapGenLoop);
            MapGenThread.IsBackground = true;
            MapGenRunning = true;
            MapGenThread.Start();

            MariaDBThread = new Thread(MariaDBInit);
            MariaDBThread.IsBackground = true;
            MariaDBThread.Start();
        }

        private void StopThread()
        {
            LoopRunning = false;
            LoopThread = null;

            MapGenRunning = false;
            MapGenThread = null;
        }

        private void MainLoop()
        {
            while (LoopRunning)
            {
                if (++MappingStatusCounter > 10)
                {
                    MappingStatusCounter = 0;
                    if (IsConnected)
                    {
                        TransReqMappingStatus();
                    }
                }

                if (++PointCloudCounter > 25)
                {
                    PointCloudCounter = 0;
                    if (IsConnected)
                    {
                        TransReqPointCloud();
                    }
                }

                if (++PoseListCounter > 50)
                {
                    PoseListCounter = 0;
                    if (IsConnected)
                    {
                        TransReqPoseList();
                    }
                }

                if (++PoseGraphCounter > 10)
                {
                    PoseGraphCounter = 0;
                    if (IsConnected)
                    {
                        for (int i = 0; i < Status.PoseGraphMaxCount; i++)
                        {
                            AssistPoseGraph g = Status.PoseGraphs[i];
                            if (g == null)
                            {
                                continue;
                            }
                            if (g.MapVersion != g.CurrVersion)
                            {
                                //System.Diagnostics.Trace.WriteLine($"[RequestPoseGraphs] Index={g.Index} MapVersion={g.MapVersion} CurrVersion={g.CurrVersion}");
                                TransReqPoseGraph(g.Index);
                                break;
                            }
                        }
                    }
                }

                Thread.Sleep(LoopInterval);
            }
        }

        private void MapGenLoop()
        {
            while (MapGenRunning)
            {
                while (!PoseChangedIds.IsEmpty)
                {
                    if (PoseChangedIds.TryDequeue(out int index))
                    {
                        if (index < Status.PoseGraphMaxCount)
                        {
                            AssistPoseGraph g = Status.PoseGraphs[index];
                            if (g != null)
                            {
                                Matrix poseToMap = GetTransformMatrix(g.X, g.Y, g.A);

                                lock (Status.GridMapMonitorLock)
                                {
                                    for (int i = 0; i < g.Count; i++)
                                    {
                                        Status.GridMap.RemovePoint((float)g.MapPoints[i].X, (float)g.MapPoints[i].Y);

                                        Matrix mbp = new Matrix(new double[,] { { g.BasePoints[i].X }, { g.BasePoints[i].Y }, { 1 } });
                                        Matrix mmp = poseToMap * mbp;
                                        GraphPoint mp = new GraphPoint(mmp[0, 0], mmp[1, 0]);
                                        g.MapPoints[i] = mp;

                                        Status.GridMap.InsertPoint((float)g.MapPoints[i].X, (float)g.MapPoints[i].Y);
                                    }
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(MapGenInterval);
            }
        }

        private void MariaDBInit()
        {
            while (true)
            {
                StartConnectMariaDB();
                if (SuperPort.CommMariaDB.connectSuccessFlag)
                {
                    Console.WriteLine("初始化完成!");
                    break;
                }
                Thread.Sleep(50);
            }
        }
        static void StartConnectMariaDB()
        {
            commMariaDB = new SuperPort.CommMariaDB();
            commMariaDB.ConnectMariaDB();
        }

        private void TcpPort_DataReceived(object sender, ProtocolDataEventArgs e)
        {
            if (!(e.Data.ApplicationData is AssistAppData))
            {
                return;
            }

            AssistAppData app = e.Data.ApplicationData as AssistAppData;
            switch (app.Cmd)
            {
                case AssistCmdCode.RspHeartbeat:
                    break;
                case AssistCmdCode.RspPoseList:
                    {
                        var rapp = app as AssistRspPoseListAppData;

                        foreach (AssistPoseNode node in rapp.PoseNodes)
                        {
                            if (node.Index < Status.PoseGraphMaxCount)
                            {
                                AssistPoseGraph g;
                                if (Status.PoseGraphValids[node.Index])
                                {
                                    g = Status.PoseGraphs[node.Index];
                                }
                                else
                                {
                                    g = new AssistPoseGraph(node.Index);
                                    Status.PoseGraphs[node.Index] = g;
                                    Status.PoseGraphValids[node.Index] = true;
                                }

                                g.MapVersion = node.Version;
                                if (g.SetGraphPose(node.X, node.Y, node.A))
                                {
                                    PoseChangedIds.Enqueue(g.Index);
                                }
                            }
                        }
                    }
                    break;
                case AssistCmdCode.RspPoseGraph:
                    {
                        var rapp = app as AssistRspPoseGraphAppData;

                        int index = rapp.Index;
                        if (index >= Status.PoseGraphMaxCount)
                        {
                            break;
                        }

                        AssistPoseGraph g = Status.PoseGraphs[index];
                        if (g != null && g.MapVersion != g.CurrVersion && rapp.Version == g.MapVersion)
                        {
                            g.CurrVersion = g.MapVersion;

                            lock (Status.GridMapMonitorLock)
                            {
                                for (int i = 0; i < g.Count; i++)
                                {
                                    Status.GridMap.RemovePoint((float)g.MapPoints[i].X, (float)g.MapPoints[i].Y);
                                }

                                g.SetGraphCount(rapp.Count);
                                Matrix poseToMap = GetTransformMatrix(g.X, g.Y, g.A);

                                for (int i = 0; i < rapp.Count; i++)
                                {
                                    GraphPoint bp = new GraphPoint(rapp.X[i], rapp.Y[i]);
                                    g.BasePoints[i] = bp;

                                    Matrix mbp = new Matrix(new double[,] { { bp.X }, { bp.Y }, { 1 } });
                                    Matrix mmp = poseToMap * mbp;
                                    GraphPoint mp = new GraphPoint(mmp[0, 0], mmp[1, 0]);
                                    g.MapPoints[i] = mp;

                                    Status.GridMap.InsertPoint((float)g.MapPoints[i].X, (float)g.MapPoints[i].Y);
                                }
                            }
                        }
                    }
                    break;
                case AssistCmdCode.RspPointCloud:
                    {
                        var rapp = app as AssistRspPointCloudAppData;

                        int totalCount = rapp.TotalCount;
                        int currCount = rapp.CurrCount;
                        int startIndex = rapp.StartIndex;

                        for (int i = 0; i < currCount; i++)
                        {
                            Status.LidarPointCloud.MapPoints[startIndex + i] = new GraphPoint(rapp.X[i], rapp.Y[i]);
                        }
                        if (totalCount != Status.LidarPointCloud.Count)
                        {
                            Status.LidarPointCloud.SetCount(totalCount);
                        }
                        string str = String.Join(" ", Array.ConvertAll<float, string>(rapp.X, delegate (float s) { return s.ToString("F4"); }));
                        string str1 = String.Join(" ", Array.ConvertAll<float, string>(rapp.Y, delegate (float s) { return s.ToString("F4"); }));
                        //Logger.Info($"[PointCloud] Pack cloud: x:{str} y:{str1}");
                    }
                    break;
                case AssistCmdCode.RspPointCloudPrior:
                    {
                        var rapp = app as AssistRspPointCloudPriorAppData;

                        Logger.Info($"[PointCloudPrior] Pack: {rapp.CurrPack + 1}/{rapp.TotalPack} Length: {rapp.PngData.Length}");
                        if (rapp.CurrPack == 0)
                        {
                            Status.LidarPointCloud.CacheMapPriorConfig(rapp.PX, rapp.PY, rapp.PA, rapp.Resolution);
                            Status.LidarPointCloud.CacheMapPrior(rapp.PngData, (int)rapp.PngMaxLength);
                            string str = BitConverter.ToString(rapp.PngData).Replace('-', ' ');
                            //Logger.Info($"[PointCloudPrior] Pack PngData: {str}/");
                        }
                        else
                        {
                            Status.LidarPointCloud.CacheMapPrior(rapp.PngData);
                        }
                    }
                    break;
                case AssistCmdCode.RspMappingStatus:
                    {
                        var rapp = app as AssistRspMappingStatusAppData;

                        Status.Valid = rapp.Valid;
                        Status.MappingStatus = (AssistCmdRspMappingStatus)rapp.MappingStatus;
                        Status.RelocalizationStatus = (AssistCmdRspMappingStatusRelocalization)rapp.LocalizationStatus;

                        Status.Position = new GraphPointA(rapp.X, rapp.Y, rapp.A);
                        //Logger.Info($"[MappingStatus] Pack Position: x:{rapp.X} y:{rapp.Y} a:{rapp.A}");

                        Matrix baseToMap = GetTransformMatrix(rapp.X, rapp.Y, rapp.A);

                        Matrix axisX = new Matrix(new double[,] { { BaseAxisLength }, { 0 }, { 1 } });
                        Matrix axisX_m = baseToMap * axisX;
                        Status.AxisX = new GraphPoint(axisX_m[0, 0], axisX_m[1, 0]);

                        Matrix axisY = new Matrix(new double[,] { { 0 }, { BaseAxisLength }, { 1 } });
                        Matrix axisY_m = baseToMap * axisY;
                        Status.AxisY = new GraphPoint(axisY_m[0, 0], axisY_m[1, 0]);
                    }
                    break;
                case AssistCmdCode.RspMappingName:
                    {
                        var rapp = app as AssistRspMappingNameAppData;

                        Dialog.MapName = Encoding.UTF8.GetString(rapp.Name);
                    }
                    break;
                case AssistCmdCode.RspMappingList:
                    {
                        var rapp = app as AssistRspMappingListAppData;

                        Dialog.MapList = rapp.List;
                    }
                    break;
                case AssistCmdCode.RspMappingCtrl:
                    {
                        var rapp = app as AssistRspMappingCtrlAppData;

                        // 注意一定要逆向开始赋值保证结果时序
                        Dialog.MappingCtrlErrorCode = (AssistCmdRspMappingCtrlErrorCode)rapp.ErrorCode;
                        Dialog.MappingCtrlResult = (AssistCmdRspMappingCtrlResult)rapp.Result;
                    }
                    break;
                case AssistCmdCode.RspRelocalizationCtrl:
                    {
                        var rapp = app as AssistRspRelocalizationCtrlAppData;

                        // 注意一定要逆向开始赋值保证结果时序
                        Dialog.RelocalizationCtrlErrorCode = (AssistCmdRspRelocalizationCtrlErrorCode)rapp.ErrorCode;
                        Dialog.RelocalizationCtrlResult = (AssistCmdRspRelocalizationCtrlResult)rapp.Result;
                    }
                    break;
                case AssistCmdCode.RptError:
                    {
                        var rapp = app as AssistRptErrorAppData;

                        // 注意一定要逆向开始赋值保证结果时序
                        Reporter.ErrorValue = (AssistCmdRptErrorValue)rapp.ErrorValue;
                        Reporter.ErrorCode = (AssistCmdRptErrorCode)rapp.ErrorCode;
                    }
                    break;
                default:
                    break;
            }
        }

        internal void TransReqHeartbeat()
        {
            var app = new AssistAppData()
            {
                Session = GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqHeartbeat,
            };

            TcpPort.DataTransmit(app);
        }

        private void TransReqPoseList()
        {
            var app = new AssistAppData()
            {
                Session = GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqPoseList,
            };

            TcpPort.DataTransmit(app);
        }

        private void TransReqPoseGraph(int index)
        {
            var app = new AssistReqPoseGraphAppData()
            {
                Session = GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqPoseGraph,
                Index = index,
            };

            TcpPort.DataTransmit(app);
        }

        private void TransReqPointCloud()
        {
            var app = new AssistAppData()
            {
                Session = GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqPointCloud,
            };

            TcpPort.DataTransmit(app);
        }

        private void TransReqMappingStatus()
        {
            var app = new AssistAppData()
            {
                Session = GetSession(),
                Module = AssistModelCode.MappingNode,
                Cmd = AssistCmdCode.ReqMappingStatus,
            };

            TcpPort.DataTransmit(app);
        }

        internal int GetSession()
        {
            ++SessionSeed;
            if (SessionSeed > 65500)
            {
                SessionSeed = 1;
            }
            return SessionSeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Matrix GetTransformMatrix(double x, double y, double theta)
        {
            double sin = Math.Sin(theta);
            double cos = Math.Cos(theta);

            Matrix rotate = new Matrix(new double[,] { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } });
            Matrix translate = new Matrix(new double[,] { { 1, 0, x }, { 0, 1, y }, { 0, 0, 1 } });

            return translate * rotate;
        }
    }
}
