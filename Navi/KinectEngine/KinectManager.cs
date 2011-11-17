using System;
using System.Collections.Generic;
using System.Linq;
using ManagedNiteEx;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Navi.RgbProcessing;

namespace Navi.KinectEngine
{
    public unsafe class KinectManager : INotifyPropertyChanged
    {
        #region private fields

        /// <summary>
        /// The OpenNI wrapper class
        /// </summary>
        private XnMOpenNIContextEx _niContext;

        /// <summary>
        /// The Depth Generator
        /// </summary>
        private XnMDepthGenerator _depthNode;

        /// <summary>
        /// Metadata of the Depth Generator
        /// </summary>
        private XnMDepthMetaData _depthMeta;

        /// <summary>
        /// The Image Generator
        /// </summary>
        private XnMImageMetaData _imageMeta;

        /// <summary>
        /// Metadata of the Image Generator
        /// </summary>
        private XnMImageGenerator _imageNode;

        /// <summary>
        /// Needed for thread communication and handling
        /// </summary>
        private AsyncStateData _currentState;

        private Subject<DepthImage> _depthImageSubject = new Subject<DepthImage>();
        
        private Subject<RgbImage> _rgbImageSubject = new Subject<RgbImage>();

        private readonly FrameCounter _frameCounter = new FrameCounter();

        #endregion

        #region public properties

        #region DepthImages

        /// <summary>
        /// Gets or sets the DepthImages property. This observable property 
        /// indicates ....
        /// </summary>
        public IObservable<DepthImage> DepthImages
        {
            get { return _depthImageSubject.AsObservable(); }
        }

        #endregion

        #region RgbImages

        /// <summary>
        /// Gets or sets the RgbImages property. This observable property 
        /// indicates ....
        /// </summary>
        public IObservable<RgbImage> RgbImages
        {
            get { return _rgbImageSubject.AsObservable(); }
        }

        #endregion

        #region FrameId

        private int _frameId;

        /// <summary>
        /// Gets or sets the FrameId property. This observable property 
        /// indicates
        /// </summary>
        public int FrameId
        {
            get { return _frameId; }
            set
            {
                if (_frameId != value)
                {
                    int old = _frameId;
                    _frameId = value;
                    OnFrameIdChanged(old, value);
                    RaisePropertyChanged("FrameId");
                }
            }
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the FrameId property.
        /// </summary>
        protected virtual void OnFrameIdChanged(int oldFrameId, int newFrameId)
        {
        }

        #endregion

        #region DepthImage

        private DepthImage _depthImage = null;

        /// <summary>
        /// Gets or sets the DepthImage property. This observable property 
        /// indicates ....
        /// </summary>
        public DepthImage DepthImage
        {
            get { return _depthImage; }
            set
            {
                if (_depthImage != value)
                {
                    _depthImage = value;
                    RaisePropertyChanged("DepthImage");
                }
            }
        }

        #endregion

        #region RgbImage

        private RgbImage _rgbImage = null;

        /// <summary>
        /// Gets or sets the RgbImage property. This observable property 
        /// indicates ....
        /// </summary>
        public RgbImage RgbImage
        {
            get { return _rgbImage; }
            set
            {
                if (_rgbImage != value)
                {
                    _rgbImage = value;
                    RaisePropertyChanged("RgbImage");
                }
            }
        }

        #endregion

        #region FrameRate

        private double _frameRate = 0;

        /// <summary>
        /// Gets or sets the FrameRate property. This observable property 
        /// indicates ....
        /// </summary>
        public double FrameRate
        {
            get { return _frameRate; }
            set
            {
                if (_frameRate != value)
                {
                    _frameRate = value;
                    RaisePropertyChanged("FrameRate");
                }
            }
        }

        #endregion

        #endregion

        #region ctor

        public KinectManager()
        {
            init();
            updateMetadata();
            initDepthImage();
            initRgbImage();
        }

        #endregion

        #region private methods

        private void init()
        {
            //opening the openNi context using the xml config file
            _niContext = new XnMOpenNIContextEx();
            _niContext.InitFromXmlFile("openNi.xml");

            //request the image node and metadata
            _imageNode = (XnMImageGenerator)_niContext.FindExistingNode(XnMProductionNodeType.Image);
            _imageMeta = new XnMImageMetaData();

            //request the depth node and metadata
            _depthNode = (XnMDepthGenerator)_niContext.FindExistingNode(XnMProductionNodeType.Depth);
            _depthMeta = new XnMDepthMetaData();
            
        }

        private void initDepthImage()
        {
            _depthImage = new DepthImage((int)_depthMeta.XRes, (int)_depthMeta.YRes, 0,180,460,640);
            _depthImage.PixelFormat = Util.Util.MapPixelFormat(_depthMeta.PixelFormat);
        }

        private void initRgbImage()
        {
            _rgbImage = new RgbImage((int)_imageMeta.XRes, (int)_imageMeta.YRes);
        }

        private void updateMetadata()
        {
            _niContext.WaitAndUpdateAll();
            _imageNode.GetMetaData(_imageMeta);
            _depthNode.GetMetaData(_depthMeta);
        }

        private void updateDepthImage()
        {
            _depthImage.FrameId = (int)_depthMeta.FrameID;
            _depthImage.XOffset = (int)_depthImage.XOffset;
            _depthImage.YOffset = (int)_depthImage.YOffset;

            _depthImage.ClearHistogram();

            short* depthPointer = (short*)_depthMeta.Data;

            int maxPixels = _depthImage.XResolution * _depthImage.YResolution;

            for (int i = 1; i <= maxPixels; i++)
            {
                short depthValue = *depthPointer;
                DepthPixel pixel = _depthImage.GetPixelAtIndex(i);
                pixel.CurrentDepth = depthValue;
                
                if(depthValue != 0)
                    _depthImage.AddDepthValue(pixel.Bucket, depthValue);
                
                depthPointer++;
            }

            _depthImage.UpdateDepthRanges();
        }

        private void updateRgbImage()
        {
            byte[] imageBytes = new byte[(int)_imageMeta.DataSize];
            Marshal.Copy(_imageMeta.Data, imageBytes, 0, imageBytes.Length);
            _rgbImage.Data = imageBytes;
        }

        private void track(AsyncStateData asyncData)
        {
            asyncData.Running = true;

            _frameCounter.Reset();

            while (!asyncData.Canceled)
            {
                _frameCounter.AddFrame();

                updateMetadata();

                updateDepthImage();

                _depthImageSubject.OnNext(_depthImage);

                updateRgbImage();

                _rgbImageSubject.OnNext(_rgbImage);

                FrameRate = _frameCounter.FramesPerSecond;
            }

            asyncData.Running = false;
        }

        #endregion

        #region public methods

        public void StartTracking()
        {
            StopTracking();

            AsyncStateData asyncData = new AsyncStateData();

            Action<AsyncStateData> trackAction = track;
            trackAction.BeginInvoke(asyncData, trackAction.EndInvoke, null);

            _currentState = asyncData;
        }

        public void StopTracking()
        {
            if (_currentState != null && _currentState.Running)
                _currentState.Canceled = true;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
