using System;
using System.Linq;
using System.ComponentModel;
using Navi.DepthProcessing;
using Navi.KinectEngine;
using Navi.RgbProcessing;

namespace Navi
{
    public class NaviManager
    {
        private readonly KinectManager _kinectManager;
        private readonly FilterManager _filterManager;
        private readonly MarkerTracker _markerTracker;

        private IDisposable _depthImageSubscription;
        private IDisposable _rgbImageSubscription;
        private IDisposable _markerTrackerSubscription;

        public event EventHandler<RgbImageEventArgs> RgbImageReceived;
        public event EventHandler<DepthImageEventArgs> DepthImageReceived;
        public event EventHandler<MarkerEventArgs> MarkerChanged;

        public event EventHandler<FrameRateEventArgs> FrameRateChanged;

        public NaviManager()
        {
            _kinectManager = new KinectManager();
            _filterManager = new FilterManager();
            _markerTracker = new MarkerTracker(640, 480, 24);
            

            _depthImageSubscription = _kinectManager.DepthImages.ObserveOnDispatcher().Subscribe(OnDepthImageReceived);
            _markerTrackerSubscription = _kinectManager.RgbImages.Sample(TimeSpan.FromMilliseconds(300)).Select(image => _markerTracker.GetMarker(image)).Where(marker => marker != null).ObserveOnDispatcher().Subscribe(OnMarkerChanged);
            _rgbImageSubscription = _kinectManager.RgbImages.ObserveOnDispatcher().Subscribe(OnRgbImageReceived);
            
            Observable.FromEvent<PropertyChangedEventArgs>(_kinectManager, "PropertyChanged").Where(evt => evt.EventArgs.PropertyName == "FrameRate").ObserveOnDispatcher().Subscribe(evt =>
            {
                if (FrameRateChanged != null)
                    FrameRateChanged(this, new FrameRateEventArgs(_kinectManager.FrameRate));
            });

            _kinectManager.StartTracking();
        }

        
        private void FilterDepthImage(DepthImage image)
        {
            if (_filterManager.IsAnyFilterActive)
                _filterManager.Filter(image);
            else
                image.FilterApplied = false;
        }

        private void OnDepthImageReceived(DepthImage image)
        {
            if (DepthImageReceived != null)
                DepthImageReceived(this, new DepthImageEventArgs(image));
        }

        private void OnRgbImageReceived(RgbImage image)
        {
            if (RgbImageReceived != null)
                RgbImageReceived(this, new RgbImageEventArgs(image));
        }

        private void OnMarkerChanged(Marker marker)
        {
            if (MarkerChanged != null)
                MarkerChanged(this, new MarkerEventArgs(marker));
        }

        public void AddFilter(IDepthFilter filter)
        {
            _filterManager.AddDepthFilter(filter);
        }

        public void RemoveFilter(IDepthFilter filter)
        {
            _filterManager.RemoveDepthFilter(filter);
        }



    }
}
