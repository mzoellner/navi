using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace Navi.KinectEngine
{
    public unsafe class DepthImage : INotifyPropertyChanged
    {
        #region private fields

        private WriteableBitmap _grayscaleBitmap;
        
        private WriteableBitmap _filteredBitmap;

        private DepthHistogram _histogram;

        private byte[] _unknownColor = new byte[] { 100, 100, 100, 255 };
        private byte[][] _objectColors = new byte[3][] { new byte[] { 3, 3, 216, 255 }, new byte[] { 3, 133, 216, 255 }, new byte[] { 3, 216, 163, 255 } };

        #endregion

        #region public properties

        #region XResolution

        private int _xResolution = 0;

        /// <summary>
        /// Gets or sets the XResolution property. This observable property 
        /// indicates ....
        /// </summary>
        public int XResolution
        {
            get { return _xResolution; }
            set
            {
                if (_xResolution != value)
                {
                    _xResolution = value;
                    RaisePropertyChanged("XResolution");
                }
            }
        }

        #endregion

        #region YResolution

        private int _yResolution = 0;

        /// <summary>
        /// Gets or sets the YResolution property. This observable property 
        /// indicates ....
        /// </summary>
        public int YResolution
        {
            get { return _yResolution; }
            set
            {
                if (_yResolution != value)
                {
                    _yResolution = value;
                    RaisePropertyChanged("YResolution");
                }
            }
        }

        #endregion

        #region XOffset

        private int _xOffset = 0;

        /// <summary>
        /// Gets or sets the XOffset property. This observable property 
        /// indicates ....
        /// </summary>
        public int XOffset
        {
            get { return _xOffset; }
            set
            {
                if (_xOffset != value)
                {
                    _xOffset = value;
                    RaisePropertyChanged("XOffset");
                }
            }
        }

        #endregion

        #region YOffset

        private int _yOffset = 0;

        /// <summary>
        /// Gets or sets the YOffset property. This observable property 
        /// indicates ....
        /// </summary>
        public int YOffset
        {
            get { return _yOffset; }
            set
            {
                if (_yOffset != value)
                {
                    _yOffset = value;
                    RaisePropertyChanged("YOffset");
                }
            }
        }

        #endregion

        #region PixelFormat

        private PixelFormat _pixelFormat = PixelFormats.Default;

        /// <summary>
        /// Gets or sets the PixelFormat property. This observable property 
        /// indicates ....
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
            set
            {
                if (_pixelFormat != value)
                {
                    _pixelFormat = value;
                    RaisePropertyChanged("PixelFormat");
                }
            }
        }

        #endregion

        #region FrameId

        private int _frameId = 0;

        /// <summary>
        /// Gets or sets the FrameId property. This observable property 
        /// indicates ....
        /// </summary>
        public int FrameId
        {
            get { return _frameId; }
            set
            {
                if (_frameId != value)
                {
                    _frameId = value;
                    RaisePropertyChanged("FrameId");
                }
            }
        }

        #endregion

        #region Pixels

        private DepthPixel[] _pixels = null;

        /// <summary>
        /// Gets or sets the Pixels property. This observable property 
        /// indicates ....
        /// </summary>
        public IEnumerable<DepthPixel> Pixels
        {
            get { return _pixels; }
        }

        #endregion

        #region FilterApplied

        private bool _filterApplied = false;

        /// <summary>
        /// Gets or sets the FilterApplied property. This observable property 
        /// indicates ....
        /// </summary>
        public bool FilterApplied
        {
            get { return _filterApplied; }
            set
            {
                if (_filterApplied != value)
                {
                    _filterApplied = value;
                    RaisePropertyChanged("FilterApplied");
                }
            }
        }

        #endregion

        #region DepthRanges

        private double[] _depthRanges = null;

        /// <summary>
        /// Gets or sets the DepthRanges property. This observable property 
        /// indicates ....
        /// </summary>
        public double[] DepthRanges
        {
            get { return _depthRanges; }
            set
            {

                _depthRanges = value;

            }
        }

        #endregion

        #region RangeWindowSize

        private int _rangeWindowSize = 20;

        /// <summary>
        /// Gets or sets the RangeWindowSize property. This observable property 
        /// indicates ....
        /// </summary>
        public int RangeWindowSize
        {
            get { return _rangeWindowSize; }
            set
            {
                if (_rangeWindowSize != value)
                {
                    _rangeWindowSize = value;
                    RaisePropertyChanged("RangeWindowSize");
                }
            }
        }

        #endregion

        #region RangeThreshold

        private int _rangeThreshold = 1000;

        /// <summary>
        /// Gets or sets the RangeThreshold property. This observable property 
        /// indicates ....
        /// </summary>
        public int RangeThreshold
        {
            get { return _rangeThreshold; }
            set
            {
                if (_rangeThreshold != value)
                {
                    _rangeThreshold = value;
                    RaisePropertyChanged("RangeThreshold");
                }
            }
        }

        #endregion

        #region VerticalSplits

        private int[] _verticalSplits = null;

        /// <summary>
        /// Gets or sets the VerticalSplits property. This observable property 
        /// indicates ....
        /// </summary>
        public int[] VerticalSplits
        {
            get { return _verticalSplits; }
            set
            {
                if (_verticalSplits != value)
                {
                    _verticalSplits = value;
                    RaisePropertyChanged("VerticalSplits");
                }
            }
        }

        #endregion

        #endregion

        #region ctor

        public DepthImage(int xResolution, int yResolution, params int[] verticalSplits)
        {
            this.XResolution = xResolution;
            this.YResolution = yResolution;

            VerticalSplits = verticalSplits;

            _histogram = new DepthHistogram(10000);

            int capacity = xResolution * yResolution;

            _pixels = new DepthPixel[capacity];

            int i = 0;
            for (int y = 0; y < yResolution; y++)
            {
                for (int x = 0; x < xResolution; x++)
                {
                    _pixels[i] = new DepthPixel(x + 1, y + 1, this);
                    i++;
                }
            }
        }

        #endregion

        #region public methods

        public WriteableBitmap GetFilteredBitmap()
        {
            if (_filteredBitmap == null)
            {
                _filteredBitmap = CreateImageBitmap();
            }


            _filteredBitmap.Lock();

            int nTexMapX = _filteredBitmap.BackBufferStride;

            byte* pTexRow = (byte*)_filteredBitmap.BackBuffer + YOffset * nTexMapX;

            int i = 1;

            for (int y = 0; y < YResolution; y++)
            {
                byte* pTex = pTexRow + XOffset;

                for (int x = 0; x < XResolution; x++)
                {
                    DepthPixel pixel = GetPixelAtIndex(i);

                    short depthValue = pixel.FilteredDepth;
                    double calculatedDepth = depthValue;
                    calculatedDepth /= 10000.0;
                    calculatedDepth *= 255.0;

                    pTex[0] = (byte)calculatedDepth;  // B
                    pTex[1] = (byte)calculatedDepth;  // G
                    pTex[2] = (byte)calculatedDepth;  // R
                    pTex[3] = 255;  // A

                    pTex += 4;
                    i++;
                }
                pTexRow += nTexMapX;
            }

            _filteredBitmap.AddDirtyRect(new Int32Rect(0, 0, _filteredBitmap.PixelWidth, _filteredBitmap.PixelHeight));
            _filteredBitmap.Unlock();

            return _filteredBitmap;
        }

        public WriteableBitmap GetGrayscaleBitmap()
        {
            if (_grayscaleBitmap == null)
            {
                _grayscaleBitmap = CreateImageBitmap();
            }

            _grayscaleBitmap.Lock();

            int nTexMapX = _grayscaleBitmap.BackBufferStride;

            byte* pTexRow = (byte*)_grayscaleBitmap.BackBuffer + YOffset * nTexMapX;

            int i = 1;

            for (int y = 0; y < YResolution; y++)
            {
                byte* pTex = pTexRow + XOffset;

                for (int x = 0; x < XResolution; x++)
                {
                    DepthPixel pixel = GetPixelAtIndex(i);

                    short depthValue = pixel.CurrentDepth;

                    if (depthValue == 0)
                    {
                        //use special color if depth value is zero
                        //pTex[0] = (byte)64;  // B
                        //pTex[1] = (byte)128;  // G
                        //pTex[2] = (byte)255;  // R
                        //pTex[3] = 255;  // A

                        for (int j = 0; j < 4; j++)
                        {
                            pTex[j] = _unknownColor[j];
                        }

                            
                    }
                    else
                    {
                        double calculatedDepth = depthValue;

                        double window = RangeWindowSize / 2.0;

                        //int bucket = getbucket from x value

                        double rangeDepth = DepthRanges[pixel.Bucket];
                        if ((rangeDepth - window) <= calculatedDepth && (calculatedDepth <= rangeDepth + window))
                        {
                            byte[] color;

                            if (rangeDepth < 1000)
                            {
                                color = _objectColors[0];
                            }
                            else if (rangeDepth < 2000)
                            {
                                color = _objectColors[1];
                            }
                            else
                            {
                                color = _objectColors[2];
                            }

                            for (int j = 0; j < 4; j++)
                            {
                                pTex[j] = color[j];
                            }

                            //paint in different color
                            //pTex[0] = (byte)255;  // B
                            //pTex[1] = (byte)128;  // G
                            //pTex[2] = (byte)64;  // R
                            //pTex[3] = 255;  // A
                        }
                        else
                        {
                            calculatedDepth /= 10000.0;
                            calculatedDepth *= 255.0;

                            pTex[0] = (byte)calculatedDepth;  // B
                            pTex[1] = (byte)calculatedDepth;  // G
                            pTex[2] = (byte)calculatedDepth;  // R
                            pTex[3] = 255;  // A
                        }
                    }



                    pTex += 4;
                    i++;
                }
                pTexRow += nTexMapX;
            }

            _grayscaleBitmap.AddDirtyRect(new Int32Rect(0, 0, _grayscaleBitmap.PixelWidth, _grayscaleBitmap.PixelHeight));
            _grayscaleBitmap.Unlock();

            return _grayscaleBitmap;
        }

        public IEnumerable<DepthPixel> GetDepthPixelsInRegion(int x, int y, int width, int height)
        {
            for (int i = x; (i < x + width) && i<XResolution; i++)
            {
                for (int j = y; (j < y + height) && j<YResolution; j++)
                {
                    yield return GetPixelFromCoordinates(i, j);
                }
            }
        }

        #endregion

        #region internal methods

        internal void AddDepthValue(int bucket, short depth)
        {
            _histogram.AddDepthValue(bucket, depth);
        }

        internal void UpdateDepthRanges()
        {
            DepthRanges = _histogram.GetClosestDepthValuesOverThreshold(RangeThreshold, RangeWindowSize);
        }

        internal void ClearHistogram()
        {
            _histogram.Clear();
        }

        internal DepthPixel GetPixelAtIndex(int index)
        {
            return _pixels[index - 1];
        }

        internal DepthPixel GetPixelFromCoordinates(int x, int y)
        {
            return _pixels[y * XResolution - (XResolution - x) - 1];
        }

        internal IEnumerable<DepthPixel> GetNeighbourhood(int x, int y, int horizontalNeighbours, int verticalNeighbours)
        {
            var upperLeftX = x - horizontalNeighbours;
            var upperLeftY = y - verticalNeighbours;

            if (upperLeftX < 1)
                upperLeftX = 1;
            if (upperLeftY < 1)
                upperLeftY = 1;

            var lowerRightX = x + horizontalNeighbours;
            var lowerRightY = y + verticalNeighbours;

            if (lowerRightX > XResolution)
                lowerRightX = XResolution;
            if (lowerRightY > YResolution)
                lowerRightY = YResolution;

            List<DepthPixel> pixels = new List<DepthPixel>();

            for (int yPos = upperLeftY; yPos <= lowerRightY; yPos++)
            {
                for (int xPos = upperLeftX; xPos <= lowerRightX; xPos++)
                {
                    //falsche berechnung hier !!!!
                    if (!(xPos == x && yPos == y))
                        pixels.Add(GetPixelFromCoordinates(xPos, yPos));
                }
            }

            return pixels;
        }

        #endregion

        #region private methods

        private WriteableBitmap CreateImageBitmap()
        {
            return new WriteableBitmap(XResolution, YResolution, 96.0, 96.0, PixelFormats.Pbgra32, null);
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
