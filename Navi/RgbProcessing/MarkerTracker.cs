using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Navi.RgbProcessing
{
    public unsafe class MarkerTracker : IDisposable
    {
        private const int BitsPerByte = 8;
        private const ArManWrap.PIXEL_FORMAT PixelFormat = ArManWrap.PIXEL_FORMAT.PIXEL_FORMAT_RGB;
        private const string CameraCalibrationPath = "no_distortion.cal";
        
        private readonly int _screenwidth;
        private readonly int _screenheight;
        private readonly int _bytesPerPixel;
        private readonly byte[] _imageBytes;
        private readonly byte[] _flipY;
        private readonly float[] _modelViewMatrix;
        private readonly float[] _projMatrix;

        private IntPtr _tracker;

        public static bool RunTracking
        {
            get;
            set;
        }

        public byte[] TrackingImageData
        {
            get
            {
                return _flipY;
            }
        }

        public MarkerTracker(int screenwidth, int screenheight, int bitsPerPixel)
        {
            this._screenwidth = screenwidth;
            this._screenheight = screenheight;
            this._bytesPerPixel = bitsPerPixel / BitsPerByte;
            this.InitArToolKit();
            this._imageBytes = new byte[this._screenwidth * this._screenheight * this._bytesPerPixel];
            this._flipY = new byte[this._imageBytes.Length];
            this._modelViewMatrix = new float[16];
            this._projMatrix = new float[16];
        }

        private void InitArToolKit()
        {

            this._tracker = ArManWrap.ARTKPConstructTrackerSingle(-1, this._screenwidth, this._screenheight);

            if (this._tracker == IntPtr.Zero)
            {
                throw new MarkerTrackerException("Literals.ArToolKitInitError");
            }

            //get the Tracker description
            IntPtr ipDesc = ArManWrap.ARTKPGetDescription(_tracker);
            string desc = Marshal.PtrToStringAnsi(ipDesc);


            ArManWrap.ARTKPSetPixelFormat(this._tracker, (int)PixelFormat);

            int retInit = ArManWrap.ARTKPInit(this._tracker, CameraCalibrationPath, 1.0f, 2000.0f);

            if (retInit != 0)
            {
                throw new MarkerTrackerException("Literals.ArToolKitDataInitError");
            }

            ArManWrap.ARTKPSetMarkerMode(this._tracker, (int)ArManWrap.MARKER_MODE.MARKER_ID_BCH);
            ArManWrap.ARTKPSetBorderWidth(this._tracker, 0.125f);

            ArManWrap.ARTKPSetPatternWidth(this._tracker, 100);

            if (ArManWrap.ARTKPIsAutoThresholdActivated(this._tracker))
            {
                ArManWrap.ARTKPActivateAutoThreshold(this._tracker, true);
            }

            ArManWrap.ARTKPSetUndistortionMode(this._tracker, (int)ArManWrap.UNDIST_MODE.UNDIST_LUT);
            ArManWrap.ARTKPSetUseDetectLite(this._tracker, false);

        }

        private static int GetPixelOffset(int row, int col, int width, int bytesPerPixel)
        {
            return ((row * width) + col) * bytesPerPixel;
        }

        private void generateTrackingImage(RgbImage image)
        {
            Array.Copy(image.Data, _imageBytes, this._imageBytes.Length);

            for (int col = 0; col < image.XResolution; col++)
            {
                for (int row = 0; row < image.YResolution; row++)
                {
                    int srcPixOffset = GetPixelOffset(row, col, image.XResolution, image.BytesPerPixel);
                    int tarPixOffset = GetPixelOffset(image.YResolution - row - 1, col, image.XResolution, image.BytesPerPixel);
                    for (int j = 0; j < image.BytesPerPixel; j++)
                    {
                        this._flipY[tarPixOffset + j] = this._imageBytes[srcPixOffset + j];
                    }
                }
            }

        }

        private Marker getMarker()
        {
            int pattern = -1;
            bool updateMatrix = true;
            IntPtr markerInfos = IntPtr.Zero;
            int numMarkers;
            int markerId = ArManWrap.ARTKPCalc(_tracker, _flipY, pattern, updateMatrix, out markerInfos, out numMarkers);

            if (numMarkers == 1)
            {
                //marshal the MarkerInfo from native to managed
                ArManWrap.ARMarkerInfo armi = (ArManWrap.ARMarkerInfo)Marshal.PtrToStructure(markerInfos, typeof(ArManWrap.ARMarkerInfo));


                

                float[] center = new float[] { 0, 0 };
                float width = 50;
                var matrix = new float[16];

                float conf = ArManWrap.ARTKPGetConfidence(_tracker);

                ArManWrap.ARTKPGetModelViewMatrix(this._tracker, this._modelViewMatrix);
                ArManWrap.ARTKPGetProjectionMatrix(this._tracker, this._projMatrix);
                ArManWrap.ARTKPGetTransMat(this._tracker, markerInfos, center, width, matrix);
                Marshal.Release(markerInfos);

                Marker mmi = new Marker(armi.id);
                mmi.Confidence = conf;
                mmi.Found = true;
                mmi.NotFoundCount = 0;
                mmi.PrevMatrix = matrix;
                mmi.ModelViewMatrix = this._modelViewMatrix;
                mmi.ProjectionMatrix = this._projMatrix;

                float x = 0;
                float y = 0;


                //calculate Center Point
                float* centerPos = armi.pos;
                x = *centerPos;
                centerPos++;
                y = *centerPos;
                mmi.Center = new Point(_screenwidth - x, _screenheight-y);
                
                //calculate TopLeft
                float* vertice = armi.vertex;

                x = *vertice;
                vertice++;
                y = *vertice;
                vertice++;

                mmi.TopLeft = new Point(_screenwidth - x, _screenheight - y);

                x = *vertice;
                vertice++;
                y = *vertice;
                vertice++;

                mmi.TopRight = new Point(_screenwidth - x, _screenheight - y);

                x = *vertice;
                vertice++;
                y = *vertice;
                vertice++;

                mmi.BottomRight = new Point(_screenwidth - x, _screenheight - y);

                x = *vertice;
                vertice++;
                y = *vertice;

                mmi.BottomLeft = new Point(_screenwidth - x, _screenheight - y);

                return mmi;
            }
            else
                return Marker.Invalid;
        }

        public Marker GetMarker(RgbImage image)
        {
            generateTrackingImage(image);

            return getMarker();
        }

        public void Dispose()
        {
            ArManWrap.ARTKPCleanup(this._tracker, IntPtr.Zero);
        }
    }

}
