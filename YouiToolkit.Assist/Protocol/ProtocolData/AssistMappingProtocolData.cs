using SuperPortLibrary;
using System;
using System.Text;

namespace YouiToolkit.Assist
{
    /// <summary>
    /// 建图节点
    /// </summary>
    internal class AssistMappingProtocolData : ProviderProtocolData
    {
        /// <summary>
        /// 模块码
        /// </summary>
        private const AssistModelCode module = AssistModelCode.MappingNode;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parent">父对象</param>
        public AssistMappingProtocolData(ProtocolData parent) : base(parent)
        {
        }

        /// <summary>
        /// 编译：接收数据的编译
        /// </summary>
        protected override void OnCompileData()
        {
            int session = BitConverter.ToUInt16(BytesData, 0);
            AssistCmdCode cmd = (AssistCmdCode)BytesData[3];

            AssistAppData output = null;

            switch (cmd)
            {
                case AssistCmdCode.RspHeartbeat:
                    if (BytesLength == 6)
                    {
                        output = new AssistAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };
                    }
                    break;
                case AssistCmdCode.RspPoseList:
                    if (BytesLength >= 10)
                    {
                        var tmp = new AssistRspPoseListAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Count = BitConverter.ToUInt16(BytesData, 6);
                        if (BytesLength == 10 + 18 * tmp.Count)
                        {
                            tmp.PoseNodes = new AssistPoseNode[tmp.Count];
                            for (int i = default; i < tmp.Count; i++)
                            {
                                tmp.PoseNodes[i].Index = BitConverter.ToUInt16(BytesData, 8 + i * 18);
                                tmp.PoseNodes[i].Version = BitConverter.ToInt32(BytesData, 8 + i * 18 + 2);
                                tmp.PoseNodes[i].X = ReadFloat(BytesData, 8 + i * 18 + 6) * 1000f;
                                tmp.PoseNodes[i].Y = ReadFloat(BytesData, 8 + i * 18 + 10) * 1000f;
                                tmp.PoseNodes[i].A = ReadFloat(BytesData, 8 + i * 18 + 14);
                            }
                        }
                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspPoseGraph:
                    if (BytesLength >= 15)
                    {
                        var tmp = new AssistRspPoseGraphAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Index = BitConverter.ToUInt16(BytesData, 4);
                        tmp.Version = BitConverter.ToInt32(BytesData, 6);
                        tmp.TotalPack = BytesData[10];
                        tmp.CurrPack = BytesData[11];
                        tmp.Count = BitConverter.ToUInt16(BytesData, 12);

                        if (tmp.Count > 0)
                        {
                            if (BytesLength == 16 + tmp.Count * 8)
                            {
                                tmp.X = new float[tmp.Count];
                                tmp.Y = new float[tmp.Count];

                                for (int i = 0; i < tmp.Count; i++)
                                {
                                    tmp.X[i] = ReadFloat(BytesData, 14 + 8 * i) * 1000f;
                                    tmp.Y[i] = ReadFloat(BytesData, 18 + 8 * i) * 1000f;
                                }
                            }
                            else
                            {
                                tmp.Count = 0;
                            }
                        }

                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspPointCloud:
                    if (BytesLength >= 26)
                    {
                        var tmp = new AssistRspPointCloudAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.PX = ReadFloat(BytesData, 4) * 1000f;
                        tmp.PY = ReadFloat(BytesData, 8) * 1000f;
                        tmp.PA = ReadFloat(BytesData, 12);

                        tmp.TotalPack = BytesData[16];
                        tmp.CurrPack = BytesData[17];
                        tmp.StartIndex = ReadUshort(BytesData, 18);
                        tmp.TotalCount = ReadUshort(BytesData, 20);
                        tmp.CurrCount = ReadUshort(BytesData, 22);

                        if (tmp.CurrCount > 0)
                        {
                            if (BytesLength == 26 + 8 * tmp.CurrCount)
                            {
                                tmp.X = new float[tmp.CurrCount];
                                tmp.Y = new float[tmp.CurrCount];

                                for (int i = default; i < tmp.CurrCount; i++)
                                {
                                    tmp.X[i] = ReadFloat(BytesData, 24 + 8 * i) * 1000f;
                                    tmp.Y[i] = ReadFloat(BytesData, 28 + 8 * i) * 1000f;
                                }
                            }
                            else
                            {
                                tmp.CurrCount = 0;
                            }
                        }

                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspPointCloudPrior:
                    if (BytesLength >= 8)
                    {
                        var tmp = new AssistRspPointCloudPriorAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.TotalPack = BytesData[4];
                        tmp.CurrPack = BytesData[5];

                        if (tmp.CurrPack == 0)
                        {
                            tmp.PX = ReadFloat(BytesData, 6) * 1000f;
                            tmp.PY = ReadFloat(BytesData, 10) * 1000f;
                            tmp.PA = ReadFloat(BytesData, 14);
                            tmp.Resolution = ReadFloat(BytesData, 18) * 1000f;
                            tmp.PngMaxLength = ReadUint(BytesData, 22);
                            tmp.PngData = ReadBytes(BytesData, 26, BytesData.Length - 28);
                        }
                        else
                        {
                            tmp.PngData = ReadBytes(BytesData, 6, BytesData.Length - 8);
                        }

                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspMappingStatus:
                    if (BytesLength == 23)
                    {
                        var tmp = new AssistRspMappingStatusAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Valid = BytesData[4] != 0x00;
                        tmp.MappingStatus = BytesData[5];
                        tmp.LocalizationStatus = BytesData[6];
                        tmp.ErrorCode = BitConverter.ToInt16(BytesData, 7);
                        tmp.X = ReadFloat(BytesData, 9) * 1000f;
                        tmp.Y = ReadFloat(BytesData, 13) * 1000f;
                        tmp.A = ReadFloat(BytesData, 17);

                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspMappingName:
                    {
                        var tmp = new AssistRspMappingNameAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Length = BitConverter.ToInt16(BytesData, 4);
                        tmp.Name = ReadBytes(BytesData, 6, tmp.Length);
                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspMappingList:
                    {
                        var tmp = new AssistRspMappingListAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Length = BitConverter.ToInt16(BytesData, 4);
                        tmp.List = AssistRspMappingListAppData.ToList(ReadBytes(BytesData, 6, tmp.Length));
                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspMappingCtrl:
                    if (BytesLength == 8)
                    {
                        var tmp = new AssistRspMappingCtrlAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Result = BytesData[4];
                        tmp.ErrorCode = BytesData[5];
                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RspRelocalizationCtrl:
                    if (BytesLength == 8)
                    {
                        var tmp = new AssistRspRelocalizationCtrlAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.Result = BytesData[4];
                        tmp.ErrorCode = BytesData[5];
                        output = tmp;
                    }
                    break;
                case AssistCmdCode.RptError:
                    if (BytesLength == 11)
                    {
                        var tmp = new AssistRptErrorAppData()
                        {
                            Session = session,
                            Module = module,
                            Cmd = cmd,
                        };

                        tmp.ErrorCode = BytesData[4];
                        tmp.ErrorValue = BitConverter.ToInt32(BytesData, 5);

                        output = tmp;
                    }
                    break;
                default:
                    break;
            }

            if (output != null)
            {
                SetApplication(output);
            }
        }

        /// <summary>
        /// 反编译：发送数据的编译
        /// </summary>
        protected override void OnDecompileData()
        {
            AssistAppData app = ApplicationData as AssistAppData;

            byte[] output = null;

            switch (app.Cmd)
            {
                case AssistCmdCode.ReqHeartbeat:
                case AssistCmdCode.ReqPoseList:
                case AssistCmdCode.ReqPointCloud:
                case AssistCmdCode.ReqMappingStatus:
                case AssistCmdCode.ReqMappingName:
                case AssistCmdCode.ReqMappingList:
                    {
                        output = new byte[6];

                        WriteUshort(ref output, (ushort)app.Session, 0);
                        output[2] = (byte)app.Module;
                        output[3] = (byte)app.Cmd;
                        output[4] = 0x00;
                        output[5] = 0x00;
                    }
                    break;
                case AssistCmdCode.ReqPointCloudPrior:
                    {
                        var rapp = app as AssistReqPointCloudPriorAppData;

                        byte[] mapName = Encoding.UTF8.GetBytes(rapp?.MapName ?? string.Empty);

                        output = new byte[8 + mapName.Length];

                        WriteUshort(ref output, (ushort)app.Session, 0);
                        output[2] = (byte)app.Module;
                        output[3] = (byte)app.Cmd;
                        WriteUshort(ref output, (ushort)mapName.Length, 4);
                        WriteBytes(ref output, mapName, 6);
                    }
                    break;
                case AssistCmdCode.ReqPoseGraph:
                    {
                        var rapp = app as AssistReqPoseGraphAppData;

                        output = new byte[8];

                        WriteUshort(ref output, (ushort)app.Session, 0);
                        output[2] = (byte)app.Module;
                        output[3] = (byte)app.Cmd;
                        WriteUshort(ref output, (ushort)rapp.Index, 4);
                        output[6] = 0x00;
                        output[7] = 0x00;
                    }
                    break;
                case AssistCmdCode.ReqMappingCtrl:
                    {
                        var rapp = app as AssistReqMappingCtrlAppData;

                        byte[] mapName = Encoding.UTF8.GetBytes(rapp.MapName);

                        output = new byte[9 + mapName.Length];

                        WriteUshort(ref output, (ushort)app.Session, 0);
                        output[2] = (byte)app.Module;
                        output[3] = (byte)app.Cmd;
                        output[4] = rapp.CmdVal;
                        WriteUshort(ref output, (ushort)mapName.Length, 5);
                        WriteBytes(ref output, mapName, 7);
                    }
                    break;
                case AssistCmdCode.ReqRelocalizationCtrl:
                    {
                        var rapp = app as AssistReqRelocalizationCtrlAppData;

                        output = new byte[19];

                        WriteUshort(ref output, (ushort)app.Session, 0);
                        output[2] = (byte)app.Module;
                        output[3] = (byte)app.Cmd;
                        output[4] = rapp.CmdVal;
                        WriteFloat(ref output, rapp.X, 5);
                        WriteFloat(ref output, rapp.Y, 9);
                        WriteFloat(ref output, rapp.A, 13);
                        output[17] = 0x00;
                        output[18] = 0x00;
                    }
                    break;
                default:
                    break;
            }

            if (output != null)
            {
                SetBytes(output);
            }
        }
    }
}
