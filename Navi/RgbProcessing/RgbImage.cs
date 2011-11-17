using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Navi.RgbProcessing
{
    public class RgbImage : INotifyPropertyChanged
    {
        private WriteableBitmap _image;

        public RgbImage(int xResolution, int yResolution)
        {
            XResolution = xResolution;
            YResolution = yResolution;

            _image = Util.Util.CreateWriteableBitmap(XResolution, YResolution, PixelFormats.Rgb24);
        }

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

        #region Data

        private byte[] _data = null;

        /// <summary>
        /// Gets or sets the Data property. This observable property 
        /// indicates ....
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    RaisePropertyChanged("Data");
                }
            }
        }

        #endregion

        #region BytesPerPixel

        private int _bytesPerPixel = 3;

        /// <summary>
        /// Gets or sets the BytesPerPixel property. This observable property 
        /// indicates ....
        /// </summary>
        public int BytesPerPixel
        {
            get { return _bytesPerPixel; }
            set
            {
                if (_bytesPerPixel != value)
                {
                    _bytesPerPixel = value;
                    RaisePropertyChanged("BytesPerPixel");
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
        
        public WriteableBitmap GetImage()
        {
            Util.Util.CopyWriteableBitmap(Data, _image);
            return _image;
        }
    }
}
