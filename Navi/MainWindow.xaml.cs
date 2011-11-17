using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using Navi.KinectEngine;
using Navi.Navigation;
using Navi.RgbProcessing;
using Navi.SpeechProcessing;
using Navi.VibrationProcessing;

namespace Navi
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region private fields

        private NaviManager _naviManager;
        private VibrationManager _vibrationManager;
        private DepthImage _currentDepthImage;
        private NavigationEngine _navigationEngine;
        private SpeechEngine _speechEngine;
        private NavigationTag _navigationTag;
        private NavigationHint _currentHint;

        #endregion

        #region ctor

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #endregion

        #region event handling

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _naviManager = new NaviManager();

            _vibrationManager = new VibrationManager("COM7");

            _navigationEngine = new NavigationEngine("tags.xml");

            _speechEngine = new SpeechEngine();
            _speechEngine.RepeatRecognized += OnRepeatRecognized;

            _naviManager.DepthImageReceived += OnDepthImageReceived;
            _naviManager.RgbImageReceived += OnRgbImageReceived;
            _naviManager.MarkerChanged += OnMarkerChanged;
            _naviManager.FrameRateChanged += OnFrameRateChanged;

            Delay0 = 0;
            Delay1 = 500;
            Delay2 = 5000;
            
            Depth0 = 1000;
            Depth1 = 2000;
            Depth2 = 3000;

        }

        void OnRepeatRecognized(object sender, EventArgs e)
        {
            if (_currentHint != null)
            {
                _speechEngine.Speak(_currentHint.Text);
                _currentHint.LastTimeSeen = DateTime.Now;
            }
        }

        private void OnFrameRateChanged(object sender, FrameRateEventArgs e)
        {
            fpsTxt.Text = Math.Ceiling(e.FrameRate) + " fps";
        }

        private void OnMarkerChanged(object sender, MarkerEventArgs e)
        {
            CurrentTag = e.Marker.Id;

            if (e.Marker.Id == -1)
            {
                TagModus = 0;
                markerOverlay.Visibility = Visibility.Hidden;
                return;
            }
            markerOverlay.Visibility = Visibility.Visible;

            MarkerPosition = new Point(e.Marker.Center.X, e.Marker.Center.Y);

            //markerPosition.Margin = new Thickness(e.Marker.Center.X, e.Marker.Center.Y, 0, 0);

            //markerPosition.Margin = new Thickness(e.Marker.TopLeft.X, e.Marker.TopLeft.Y, 0, 0);

            var tltr = new LineGeometry(e.Marker.TopLeft, e.Marker.TopRight);
            var trbr = new LineGeometry(e.Marker.TopRight, e.Marker.BottomRight);
            var brbl = new LineGeometry(e.Marker.BottomRight, e.Marker.BottomLeft);
            var bltl = new LineGeometry(e.Marker.BottomLeft, e.Marker.TopLeft);

            topLeftTopRightLine.Data = tltr;
            topRightBottomRightLine.Data = trbr;
            bottomRightBottomLeftLine.Data = brbl;
            bottomLeftTopLeftLine.Data = bltl;

            var xValues = new[]{e.Marker.TopLeft.X, e.Marker.TopRight.X, e.Marker.BottomLeft.X, e.Marker.BottomRight.X};
            var yValues = new[]{e.Marker.TopLeft.Y, e.Marker.TopRight.Y, e.Marker.BottomLeft.Y, e.Marker.BottomRight.Y};

            var minX = Math.Floor(xValues.Min());
            var maxX = Math.Floor(xValues.Max());
            var minY = Math.Floor(yValues.Min());
            var maxY = Math.Floor(yValues.Max());

            var width = maxX - minX;
            var height = maxY - minY;

            //boundingBox.Margin = new Thickness(minX, minY, 0, 0);
            //boundingBox.Width = width;
            //boundingBox.Height = height;

            var modus = _currentDepthImage.GetDepthPixelsInRegion((int)minX, (int)minY, (int)width, (int)height).Select(pixel => pixel.CurrentDepth).Where(depth => depth != 0).GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            TagModus = modus;

            if (_navigationTag == null || _navigationTag.Id != CurrentTag)
            {
                _navigationTag = _navigationEngine.GetTag(CurrentTag);
            }

            if (_navigationTag != null)
            {
                _currentHint = _navigationTag.Hints.Where(hint => TagModus <= hint.Depth).OrderBy(hint => hint.Depth).FirstOrDefault();

                if (_currentHint != null)
                {
                    var diff = (DateTime.Now - _currentHint.LastTimeSeen).TotalSeconds;

                    if (diff > 20)
                    {
                        _speechEngine.Speak(_currentHint.Text);
                        _currentHint.LastTimeSeen = DateTime.Now;
                    }
                }
            }
        }

        void OnDepthImageReceived(object sender, DepthImageEventArgs e)
        {
            _currentDepthImage = e.DepthImage;
            e.DepthImage.RangeWindowSize = WindowSize;
            e.DepthImage.RangeThreshold = Threshold;

            double l = e.DepthImage.DepthRanges.ElementAt(0);
            leftText.Text = l.ToString() + " mm";
            left.Height = l / 5000.0 * 480.0;

            int leftDelay = GetVibratorDelay(l);
            if (leftDelay != _vibrationManager.GetDelay(VibratorPosition.Left))
            {
                _vibrationManager.ChangeDelay(VibratorPosition.Left, leftDelay);
            }
            LeftDelay = GetDelayIndex(leftDelay);
            //double leftFreq = getVibratorFrequency(l);
            //if (leftFreq != leftVib.EnginePulsRate)
            //    leftVib.changeEnginePulse(leftFreq);

            double c = e.DepthImage.DepthRanges.ElementAt(1);
            centerText.Text = c.ToString() + " mm";
            center.Height = c / 10000.0 * 480.0;

            int centerDelay = GetVibratorDelay(c);
            if (centerDelay != _vibrationManager.GetDelay(VibratorPosition.Center))
            {
                _vibrationManager.ChangeDelay(VibratorPosition.Center, centerDelay);
            }
            CenterDelay = GetDelayIndex(centerDelay);
            //double centerFreq = getVibratorFrequency(c);
            //if (centerFreq != centerVib.EnginePulsRate)
            //    centerVib.changeEnginePulse(centerFreq);

            double r = e.DepthImage.DepthRanges.ElementAt(2);
            rightText.Text = r.ToString() + " mm";
            right.Height = r / 10000.0 * 480.0;

            int rightDelay = GetVibratorDelay(r);
            if (rightDelay != _vibrationManager.GetDelay(VibratorPosition.Right))
            {
                _vibrationManager.ChangeDelay(VibratorPosition.Right, rightDelay);
            }
            RightDelay = GetDelayIndex(rightDelay);
            //double rightFreq = getVibratorFrequency(r);
            //if (rightFreq != rightVib.EnginePulsRate)
            //    rightVib.changeEnginePulse(rightFreq);

            if (ShowDepthImage)
                DepthImage = e.DepthImage.GetGrayscaleBitmap();
        }

        void OnRgbImageReceived(object sender, RgbImageEventArgs e)
        {
            if (ShowRGBImage)
                RgbImage = e.RgbImage.GetImage();
        }

        #endregion

        #region private methods

        private int GetDelayIndex(int delay)
        {
            if (delay == Delay0)
                return 0;
            if (delay == Delay1)
                return 1;
            if (delay == Delay2)
                return 2;
            return 3;
        }

        private int GetVibratorDelay(double depth)
        {
            if (depth < 500)
                return Delay2;
            if (depth < Depth0)
                return Delay0;
            if (depth < Depth1)
                return Delay1;
            return Delay2;
        }

        #endregion

        #region public properties

        #region RgbImage

        private WriteableBitmap _rgbImage = null;

        /// <summary>
        /// Gets or sets the RgbImage property. This observable property 
        /// indicates ....
        /// </summary>
        public WriteableBitmap RgbImage
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

        #region DepthImage

        private WriteableBitmap _depthImage = null;

        /// <summary>
        /// Gets or sets the DepthImage property. This observable property 
        /// indicates ....
        /// </summary>
        public WriteableBitmap DepthImage
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

        #region CurrentTag

        private int _currentTag = -1;

        /// <summary>
        /// Gets or sets the CurrentTag property. This observable property 
        /// indicates ....
        /// </summary>
        public int CurrentTag
        {
            get { return _currentTag; }
            set
            {
                if (_currentTag != value)
                {
                    _currentTag = value;
                    RaisePropertyChanged("CurrentTag");
                }
            }
        }

        #endregion

        #region ShowRGBImage

        private bool _showRgbImage = true;

        /// <summary>
        /// Gets or sets the ShowRGBImage property. This observable property 
        /// indicates ....
        /// </summary>
        public bool ShowRGBImage
        {
            get { return _showRgbImage; }
            set
            {
                if (_showRgbImage != value)
                {
                    _showRgbImage = value;
                    RaisePropertyChanged("ShowRGBImage");
                }
            }
        }

        #endregion

        #region ShowDepthImage

        private bool _showDepthImage = true;

        /// <summary>
        /// Gets or sets the ShowDepthImage property. This observable property 
        /// indicates ....
        /// </summary>
        public bool ShowDepthImage
        {
            get { return _showDepthImage; }
            set
            {
                if (_showDepthImage != value)
                {
                    _showDepthImage = value;
                    RaisePropertyChanged("ShowDepthImage");
                }
            }
        }

        #endregion

        #region WindowSize

        private int _windowSize = 120;

        /// <summary>
        /// Gets or sets the WindowSize property. This observable property 
        /// indicates ....
        /// </summary>
        public int WindowSize
        {
            get { return _windowSize; }
            set
            {
                if (_windowSize != value)
                {
                    _windowSize = value;
                    RaisePropertyChanged("WindowSize");
                }
            }
        }

        #endregion

        #region Threshold

        private int _threshold = 4500;

        /// <summary>
        /// Gets or sets the Threshold property. This observable property 
        /// indicates ....
        /// </summary>
        public int Threshold
        {
            get { return _threshold; }
            set
            {
                if (_threshold != value)
                {
                    _threshold = value;
                    RaisePropertyChanged("Threshold");
                }
            }
        }

        #endregion

        #region MarkerPosition

        private Point _markerPosition;

        /// <summary>
        /// Gets or sets the MarkerPosition property. This observable property 
        /// indicates ....
        /// </summary>
        public Point MarkerPosition
        {
            get { return _markerPosition; }
            set
            {
                if (_markerPosition != value)
                {
                    _markerPosition = value;
                    RaisePropertyChanged("MarkerPosition");
                }
            }
        }

        #endregion

        #region Delay0

        private int _delay0 = 0;

        /// <summary>
        /// Gets or sets the Delay1 property. This observable property 
        /// indicates ....
        /// </summary>
        public int Delay0
        {
            get { return _delay0; }
            set
            {
                if (_delay0 != value)
                {
                    _delay0 = value;
                    RaisePropertyChanged("Delay0");
                }
            }
        }

        #endregion

        #region Delay1

        private int _delay1 = 0;

        /// <summary>
        /// Gets or sets the Delay1 property. This observable property 
        /// indicates ....
        /// </summary>
        public int Delay1
        {
            get { return _delay1; }
            set
            {
                if (_delay1 != value)
                {
                    _delay1 = value;
                    RaisePropertyChanged("Delay1");
                }
            }
        }

        #endregion

        #region Delay2

        private int _delay2 = 0;

        /// <summary>
        /// Gets or sets the Delay1 property. This observable property 
        /// indicates ....
        /// </summary>
        public int Delay2
        {
            get { return _delay2; }
            set
            {
                if (_delay2 != value)
                {
                    _delay2 = value;
                    RaisePropertyChanged("Delay2");
                }
            }
        }

        #endregion

        #region Depth0

        private int _depth0 = 0;

        /// <summary>
        /// Gets or sets the Depth0 property. This observable property 
        /// indicates ....
        /// </summary>
        public int Depth0
        {
            get { return _depth0; }
            set
            {
                if (_depth0 != value)
                {
                    _depth0 = value;
                    RaisePropertyChanged("Depth0");
                }
            }
        }

        #endregion

        #region Depth1

        private int _depth1 = 0;

        /// <summary>
        /// Gets or sets the Depth0 property. This observable property 
        /// indicates ....
        /// </summary>
        public int Depth1
        {
            get { return _depth1; }
            set
            {
                if (_depth1 != value)
                {
                    _depth1 = value;
                    RaisePropertyChanged("Depth1");
                }
            }
        }

        #endregion

        #region Depth2

        private int _depth2 = 0;

        /// <summary>
        /// Gets or sets the Depth0 property. This observable property 
        /// indicates ....
        /// </summary>
        public int Depth2
        {
            get { return _depth2; }
            set
            {
                if (_depth2 != value)
                {
                    _depth2 = value;
                    RaisePropertyChanged("Depth2");
                }
            }
        }

        #endregion

        #region Pulses

        private int[] _pulses = null;

        /// <summary>
        /// Gets or sets the Pulses property. This observable property 
        /// indicates ....
        /// </summary>
        public int[] Pulses
        {
            get { return _pulses; }
            set
            {
                if (_pulses != value)
                {
                    _pulses = value;
                    RaisePropertyChanged("Pulses");
                }
            }
        }

        #endregion

        #region RightDelay

        private int _rightDelay = 0;

        /// <summary>
        /// Gets or sets the RightDelay property. This observable property 
        /// indicates ....
        /// </summary>
        public int RightDelay
        {
            get { return _rightDelay; }
            set
            {
                if (_rightDelay != value)
                {
                    _rightDelay = value;
                    RaisePropertyChanged("RightDelay");
                }
            }
        }

        #endregion

        #region CenterDelay

        private int _centerDelay = 0;

        /// <summary>
        /// Gets or sets the CenterDelay property. This observable property 
        /// indicates ....
        /// </summary>
        public int CenterDelay
        {
            get { return _centerDelay; }
            set
            {
                if (_centerDelay != value)
                {
                    _centerDelay = value;
                    RaisePropertyChanged("CenterDelay");
                }
            }
        }

        #endregion

        #region LeftDelay

        private int _leftDelay = 0;

        /// <summary>
        /// Gets or sets the LeftDelay property. This observable property 
        /// indicates ....
        /// </summary>
        public int LeftDelay
        {
            get { return _leftDelay; }
            set
            {
                if (_leftDelay != value)
                {
                    _leftDelay = value;
                    RaisePropertyChanged("LeftDelay");
                }
            }
        }

        #endregion

        #region TagModus

        private int _tagModus = 0;

        /// <summary>
        /// Gets or sets the TagModus property. This observable property 
        /// indicates ....
        /// </summary>
        public int TagModus
        {
            get { return _tagModus; }
            set
            {
                if (_tagModus != value)
                {
                    _tagModus = value;
                    RaisePropertyChanged("TagModus");
                }
            }
        }

        #endregion



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
