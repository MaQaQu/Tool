using System;
using System.Runtime.CompilerServices;

namespace SuperPortLibrary
{
    public abstract class ProtocolData
    {
        public virtual int BytesLength { get; set; } = 0;
        public virtual byte[] BytesData { get; set; } = null;
        public virtual ApplicationData ApplicationData { get; set; } = null;
        public virtual bool Varified { get; set; } = true;

        public virtual object Source { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual object Tag { get; set; }

        /// <summary>
        /// 编译：接收数据的编译
        /// </summary>
        protected abstract void OnCompileData();

        /// <summary>
        /// 反编译：发送数据的编译
        /// </summary>
        protected abstract void OnDecompileData();

        internal void ApplicationChecked()
        {
            ApplicationData?.CheckData();
        }

        public void SetVarified(bool varified)
        {
            Varified = varified;
        }

        public void CompileData()
        {
            OnCompileData();
        }

        public void DecompileData()
        {
            OnDecompileData();
        }

        public void SetBytes(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                BytesData = new byte[data.Length];
                Buffer.BlockCopy(data, 0, BytesData, 0, data.Length);
            }
            else
            {
                BytesData = new byte[] { };
            }
            BytesLength = BytesData.Length;
        }

        public void SetApplication(ApplicationData data)
        {
            ApplicationData = data;

            if (BytesData == null)
            {
                DecompileData();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected byte[] ReadBytes(byte[] source, int offset, int length)
        {
            byte[] target = new byte[length];
            Buffer.BlockCopy(source, offset, target, 0, length);
            return target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ushort ReadUshort(byte[] source, int offset)
            => BitConverter.ToUInt16(source, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected uint ReadUint(byte[] source, int offset)
            => BitConverter.ToUInt32(source, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float ReadFloat(byte[] source, int offset)
            => BitConverter.ToSingle(source, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteUshort(ref byte[] target, ushort @ushort, int offset)
        {
            byte[] source = BitConverter.GetBytes(@ushort);
            Buffer.BlockCopy(source, 0, target, offset, source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteFloat(ref byte[] target, float @float, int offset)
        {
            byte[] source = BitConverter.GetBytes(@float);
            Buffer.BlockCopy(source, 0, target, offset, source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteBytes(ref byte[] target, byte[] bytes, int offset)
            => Buffer.BlockCopy(bytes, 0, target, offset, bytes.Length);
    }

    public class ApplicationData
    {
        public int DataMask { get; protected set; }
        public bool Checked { get; protected set; }

        public ApplicationData()
        {
            DataMask = 0;
            Checked = false;
        }

        protected virtual void OnCheckData()
        {
            Checked = true;
        }

        internal void CheckData()
        {
            OnCheckData();
        }
    }
}
