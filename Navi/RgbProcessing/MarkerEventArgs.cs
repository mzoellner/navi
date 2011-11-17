using System;

namespace Navi.RgbProcessing
{
    public class MarkerEventArgs : EventArgs
    {
        public MarkerEventArgs(Marker m)
        {
            Marker = m;
        }

        public Marker Marker { get; set; }
    }
}
