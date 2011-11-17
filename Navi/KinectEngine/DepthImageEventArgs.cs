using System;

namespace Navi.KinectEngine
{
    public class DepthImageEventArgs : EventArgs
    {
        public DepthImageEventArgs(DepthImage image)
        {
            DepthImage = image;
        }

        public DepthImage DepthImage { get; set; }
    }
}
