using SuperPortLibrary;
using System;
using System.Diagnostics;
using YouiToolkit.Logging;

namespace YouiToolkit.Assist
{
    internal class AssistProtocol : Protocol
    {
        private const int maxLength = 1024 * 64;
        private const int headLength = 3;
        private byte[] headData = new byte[] { (byte)'U', (byte)'I', (byte)'A' };
        private int headPoint = 0;
        private int revState = 0;
        private int dataLength = 0;
        private int lengthCheck = 0;
        private int revCount = 0;
        private byte[] revBuffer = new byte[maxLength];
        private int dataCheck = 0;
        private int sysCheck = 0;

        static AssistProtocol()
        {
            SuperPort.InternalLogger.Exposed = Logger.Exposed;
        }

        protected override ProtocolData OnAutoWriteData()
        {
            return null;
        }

        protected override ProtocolData OnCompileData(byte data)
        {
            ProtocolData output = null;

            switch (revState)
            {
                case 0:
                    {
                        if (headPoint < headLength)
                        {
                            if (headData[headPoint] == data)
                            {
                                headPoint++;
                                if (headPoint == headLength)
                                {
                                    revState = 10;
                                }
                            }
                            else
                            {
                                headPoint = 0;
                            }
                        }
                        else
                        {
                            InitBuffer();
                        }
                    }
                    break;
                case 10:
                    {
                        dataLength = data;
                        revState = 11;
                    }
                    break;
                case 11:
                    {
                        dataLength += ((data << 8) & 0xFF00);
                        if (dataLength > 0 && dataLength < 64 * 1024)
                        {
                            revState = 15;
                        }
                        else
                        {
                            InitBuffer();
                        }
                    }
                    break;
                case 15:
                    {
                        lengthCheck = data;
                        revState = 16;
                    }
                    break;
                case 16:
                    {
                        lengthCheck += ((data << 8) & 0xFF00);
                        if (dataLength + lengthCheck == 0xFFFF)
                        {
                            revState = 20;
                        }
                        else
                        {
                            InitBuffer();
                        }
                    }
                    break;
                case 20:
                    {
                        if (revCount < dataLength)
                        {
                            revBuffer[revCount] = data;
                            revCount++;

                            if (revCount == dataLength)
                            {
                                revState = 30;

                                byte[] crcByte = DataCheck.GetCRC(revBuffer, 0, revCount - 1);
                                if (crcByte != null)
                                {
                                    sysCheck = crcByte[0] + ((crcByte[1] << 8) & 0xFF00);
                                }
                            }
                        }
                        else
                        {
                            InitBuffer();
                        }
                    }
                    break;
                case 30:
                    {
                        dataCheck = data;
                        revState = 31;
                    }
                    break;
                case 31:
                    {
                        dataCheck += ((data << 8) & 0xFF00);

                        if (sysCheck == dataCheck)
                        {
                            byte[] rdata = new byte[revCount];
                            Buffer.BlockCopy(revBuffer, 0, rdata, 0, revCount);
                            output = new AssistProtocolData();
                            output.SetBytes(rdata);
                            output.SetVarified(true);
                        }
                        InitBuffer();
                    }
                    break;
                default:
                    break;
            }

            return output;
        }

        protected override ProtocolData OnDecompileData(ApplicationData data)
        {
            AssistProtocolData pdata = new AssistProtocolData();
            pdata.SetApplication(data);
            return pdata;
        }

        protected override ProtocolData OnErrorWriteData()
        {
            return null;
        }

        protected override ProtocolData OnFailWriteData()
        {
            return null;
        }

        protected override void OnReadTick()
        {
        }

        protected override byte[] OnTransmitData(ProtocolData data)
        {
            if (data.BytesData == null) return null;
            if (data.BytesLength == 0) return null;

            byte[] tmp2;
            byte[] output = new byte[data.BytesLength + headLength + 6];

            for (int i = 0; i < headLength; i++)
            {
                output[i] = headData[i];
            }

            tmp2 = BitConverter.GetBytes(data.BytesLength);
            Buffer.BlockCopy(tmp2, 0, output, headLength, 2);

            tmp2 = BitConverter.GetBytes(0xFFFF - data.BytesLength);
            Buffer.BlockCopy(tmp2, 0, output, headLength + 2, 2);

            Buffer.BlockCopy(data.BytesData, 0, output, headLength + 4, data.BytesLength);

            byte[] crc = DataCheck.GetCRC(data.BytesData, 0, data.BytesLength - 1);
            Buffer.BlockCopy(crc, 0, output, headLength + data.BytesLength + 4, 2);

            DebugPrint(ref output);

            return output;
        }

        [Conditional("DEBUG")]
        private void DebugPrint(ref byte[] output)
        {
            switch (output[9])
            {
                case (byte)AssistCmdCode.ReqMappingCtrl:
                case (byte)AssistCmdCode.ReqPointCloudPrior:
                case (byte)AssistCmdCode.ReqMappingName:
                case (byte)AssistCmdCode.ReqMappingList:
                case (byte)AssistCmdCode.ReqRelocalizationCtrl:
                    Logger.Info($"[RequestDebug] {(AssistCmdCode)output[9]}: {HexHelper.ConvertToHex(output)}");
                    break;
            }
        }

        protected override void OnWriteTick()
        {
        }

        private void InitBuffer()
        {
            headPoint = 0;
            revState = 0;
            dataLength = 0;
            lengthCheck = 0;
            revCount = 0;
            dataCheck = 0;
            sysCheck = 0;
        }
    }
}
