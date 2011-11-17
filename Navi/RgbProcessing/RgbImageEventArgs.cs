using System;

namespace Navi.RgbProcessing
{
    public class RgbImageEventArgs : EventArgs
    {
        public RgbImageEventArgs(RgbImage image)
        {
            RgbImage = image;
        }

        public RgbImage RgbImage { get; set; }
    }
}
