using SharpDX.Mathematics.Interop;
using System;

namespace YouiToolkit.Models
{
    public class PoseEstimateEventArgs : EventArgs
    {
        public float Theta { get; set; }
        public RawVector2 Start { get; set; }
        public RawVector2 End { get; set; }

        public PoseEstimateEventArgs()
        {
        }

        public override string ToString()
            => $"Start=({Start.X:0.000}, {Start.Y:0.000}) End=({End.X:0.000}, {End.Y:0.000}) Theta={Theta:0.000}";
    }
}
