using System;
using System.Linq;
using Navi.RgbProcessing;

namespace Navi.Util
{
    public static class ObservableExtensions
    {
        public static IObservable<Marker> ToMarker(this IObservable<RgbImage> rgbImages)
        {
            MarkerTracker tracker = new MarkerTracker(640, 480, 24);
            return rgbImages.Select(image => tracker.GetMarker(image)).Where(marker => marker != null);
        }
    }
}
