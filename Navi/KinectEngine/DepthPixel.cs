namespace Navi.KinectEngine
{
    public class DepthPixel
    {
        //private Dictionary<int, IEnumerable<DepthPixel>> _neighbourHoods = new Dictionary<int, IEnumerable<DepthPixel>>();

        private DepthImage _image;

        public DepthPixel(int x, int y, DepthImage image)
        {
            X = x;
            Y = y;
            CurrentDepth = 0;
            FilteredDepth = 0;
            Bucket = 0;

            for (int i = 1; i < image.VerticalSplits.Length; i++ )
                if (X > image.VerticalSplits[i - 1] && X <= image.VerticalSplits[i])
                {
                    this.Bucket = i - 1;
                    break;
                }
            
            _image = image;
        }

        //public void InitNeighbourhoods(params int[] sizes)
        //{
        //    foreach (int size in sizes)
        //        _neighbourHoods[size] = createNeighbourhood(size);
        //}

        //public IEnumerable<DepthPixel> GetNeighbourhood(int size)
        //{
        //    if (!_neighbourHoods.ContainsKey(size))
        //        _neighbourHoods[size] = createNeighbourhood(size);

        //    return _neighbourHoods[size];
        //}

        //private IEnumerable<DepthPixel> createNeighbourhood(int size)
        //{
        //    return _image.GetNeighbourhood(this.X, this.Y, size, size);
        //}

        #region X

        //private int _x = 0;

        ///// <summary>
        ///// Gets or sets the X property. This observable property 
        ///// indicates ....
        ///// </summary>
        //public int X
        //{
        //    get { return _x; }
        //    set
        //    {
        //        if (_x != value)
        //        {
        //            _x = value;
        //            RaisePropertyChanged("X");
        //        }
        //    }
        //}
        public int X;

        #endregion

        #region Y

        //private int _y = 0;

        ///// <summary>
        ///// Gets or sets the Y property. This observable property 
        ///// indicates ....
        ///// </summary>
        //public int Y
        //{
        //    get { return _y; }
        //    set
        //    {
        //        if (_y != value)
        //        {
        //            _y = value;
        //            RaisePropertyChanged("Y");
        //        }
        //    }
        //}
        public int Y;

        #endregion

        #region CurrentDepth

        //private short _currentDepth = 0;

        ///// <summary>
        ///// Gets or sets the CurrentDepth property. This observable property 
        ///// indicates ....
        ///// </summary>
        //public short CurrentDepth
        //{
        //    get { return _currentDepth; }
        //    set
        //    {
        //        _currentDepth = value;
        //    }
        //}

        public short CurrentDepth;

        #endregion

        #region FilteredDepth

        //private short _filteredDepth = 0;

        ///// <summary>
        ///// Gets or sets the FilteredDepth property. This observable property 
        ///// indicates ....
        ///// </summary>
        //public short FilteredDepth
        //{
        //    get { return _filteredDepth; }
        //    set
        //    {
        //        if (_filteredDepth != value)
        //        {
        //            _filteredDepth = value;
        //            RaisePropertyChanged("FilteredDepth");
        //        }
        //    }
        //}

        public short FilteredDepth;

        #endregion

        #region FilterColor

        //private Color _filterColor = Colors.White;

        ///// <summary>
        ///// Gets or sets the FilterColor property. This observable property 
        ///// indicates ....
        ///// </summary>
        //public Color FilterColor
        //{
        //    get { return _filterColor; }
        //    set
        //    {
        //        if (_filterColor != value)
        //        {
        //            _filterColor = value;
        //            RaisePropertyChanged("FilterColor");
        //        }
        //    }
        //}

        #endregion

        #region Bucket

        //private int _bucket = 0;

        ///// <summary>
        ///// Gets or sets the Bucket property. This observable property 
        ///// indicates ....
        ///// </summary>
        //public int Bucket
        //{
        //    get { return _bucket; }
        //    set
        //    {
        //        if (_bucket != value)
        //        {
        //            _bucket = value;
        //            RaisePropertyChanged("Bucket");
        //        }
        //    }
        //}

        public int Bucket;

        #endregion

    }
}
