using System.Windows;

namespace Navi.RgbProcessing
{
    public class Marker
    {
        public Marker(int id)
        {
            Id = id;
        }

        public static Marker Invalid = new Marker(-1);
        
        public int Id { get; set; }
        public float Confidence { get; set; }

        public bool Found { get; set; }
        public int NotFoundCount { get; set; }
        //public ArManWrap.ARMarkerInfo ArMarkerInfo { get; set; }
        public float[] PrevMatrix = new float[0];
        public float[] ModelViewMatrix = new float[0];
        public float[] ProjectionMatrix = new float[0];
        public Point TopLeft;
        public Point TopRight;
        public Point BottomLeft;
        public Point BottomRight;
        public Point Center;
        
    }
}
