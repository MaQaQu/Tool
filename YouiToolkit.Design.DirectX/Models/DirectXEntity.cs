using SharpDX.Mathematics.Interop;

namespace YouiToolkit.Design.DirectX
{
    public class DirectXEntity
    {
        public float X { get; set; }
        public float Y { get; set; }

        public RawVector2 ToRawVector2() => new RawVector2(X, Y);

        public static implicit operator RawVector2(DirectXEntity self) => self.ToRawVector2();
    }
}
