using System.Linq;
using Navi.KinectEngine;

namespace Navi.DepthProcessing
{
    public class RangeFilter : IDepthFilter
    {
        public int MaximumDepth
        {
            get;
            set;
        }

        public void Filter(DepthImage image)
        {
            var parallelPixels = ParallelEnumerable.AsParallel<DepthPixel>(image.Pixels);

            foreach (DepthPixel pixel in parallelPixels)
            {
                if (pixel.CurrentDepth <= MaximumDepth)
                    pixel.FilteredDepth = pixel.CurrentDepth;
                else
                    pixel.FilteredDepth = 0;
            }
        }
    }
}
