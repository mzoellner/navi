using Navi.KinectEngine;

namespace Navi.DepthProcessing
{
    public interface IDepthFilter
    {
        void Filter(DepthImage image);
    }
}
