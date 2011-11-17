using System;
using ManagedNiteEx;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace Navi.Util
{
    public static class Util
    {
        private static void CopyWritableBitmap(XnMMapMetaData imageMd, WriteableBitmap b)
        {
            int dataSize = (int)imageMd.DataSize;
            IntPtr data = imageMd.Data;

            var rect = new Int32Rect((int)imageMd.XOffset, (int)imageMd.YOffset,
                (int)imageMd.XRes, (int)imageMd.YRes);

            b.WritePixels(rect, data, dataSize, b.BackBufferStride);
            /*
                        b.Lock();
                        NativeMethods.RtlMoveMemory(b.BackBuffer, data, dataSize);
                        b.Unlock();
            */
        }

        public static void CopyWriteableBitmap(byte[] data, WriteableBitmap b)
        {
            int dataSize = data.Length;
            var rect = new Int32Rect(0, 0, b.PixelWidth, b.PixelHeight);
            b.WritePixels(rect, data, b.BackBufferStride, 0);
            /*
                        b.Lock();
                        NativeMethods.RtlMoveMemory(b.BackBuffer, data, dataSize);
                        b.Unlock();
            */

        }

        public static WriteableBitmap CreateWriteableBitmap(int xResolution, int yResolution, PixelFormat format)
        {
            return new WriteableBitmap(xResolution, yResolution, 96.0, 96.0, format, null);
        }

        public static PixelFormat MapPixelFormat(XnMPixelFormat xnMPixelFormat)
        {
            switch (xnMPixelFormat)
            {
                case XnMPixelFormat.Grayscale8Bit:
                    return PixelFormats.Gray8;
                case XnMPixelFormat.Grayscale16Bit:
                    return PixelFormats.Gray16;
                case XnMPixelFormat.Rgb24:
                    return PixelFormats.Rgb24;

                case XnMPixelFormat.Yuv422:
                default:
                    throw new NotSupportedException();
            }
        }

    }
}
