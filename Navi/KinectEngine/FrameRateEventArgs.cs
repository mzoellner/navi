using System;

namespace Navi.KinectEngine
{
    public class FrameRateEventArgs : EventArgs
    {
        public FrameRateEventArgs(double frameRate) : base()
        {
            FrameRate = frameRate;
        }

        public double FrameRate { get; set; }
    }
}
