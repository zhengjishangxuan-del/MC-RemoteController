using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MCRemoteController
{
    /// <summary>
    /// 屏幕捕获类，使用 GDI BitBlt 捕获屏幕画面
    /// </summary>
    public class ScreenCapture : IDisposable
    {
        #region WinAPI

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private const int SRCCOPY = 0x00CC0020;

        #endregion

        private int _width;
        private int _height;
        private IntPtr _desktopDC;
        private IntPtr _memoryDC;
        private IntPtr _bitmap;
        private IntPtr _oldBitmap;
        private bool _disposed;

        /// <summary>
        /// 初始化屏幕捕获
        /// </summary>
        public ScreenCapture()
        {
            _width = Screen.PrimaryScreen.Bounds.Width;
            _height = Screen.PrimaryScreen.Bounds.Height;

            _desktopDC = GetWindowDC(GetDesktopWindow());
            _memoryDC = CreateCompatibleDC(_desktopDC);
            _bitmap = CreateCompatibleBitmap(_desktopDC, _width, _height);
            _oldBitmap = SelectObject(_memoryDC, _bitmap);
        }

        /// <summary>
        /// 捕获当前屏幕画面
        /// </summary>
        /// <returns>Bitmap 对象</returns>
        public Bitmap CaptureScreen()
        {
            if (_disposed) throw new ObjectDisposedException("ScreenCapture");

            BitBlt(_memoryDC, 0, 0, _width, _height, _desktopDC, 0, 0, SRCCOPY);

            Bitmap bmp = Image.FromHbitmap(_bitmap);
            return bmp;
        }

        /// <summary>
        /// 捕获屏幕并转换为 JPEG 字节数组
        /// </summary>
        /// <param name="quality">JPEG 质量 (1-100)</param>
        /// <returns>JPEG 字节数组</returns>
        public byte[] CaptureAsJpeg(int quality = 60)
        {
            using (Bitmap bmp = CaptureScreen())
            {
                return BitmapToJpeg(bmp, quality);
            }
        }

        /// <summary>
        /// 捕获屏幕并缩放后转换为 JPEG
        /// </summary>
        /// <param name="targetWidth">目标宽度</param>
        /// <param name="targetHeight">目标高度</param>
        /// <param name="quality">JPEG 质量</param>
        /// <returns>JPEG 字节数组</returns>
        public byte[] CaptureAsJpegScaled(int targetWidth, int targetHeight, int quality = 60)
        {
            using (Bitmap original = CaptureScreen())
            using (Bitmap scaled = new Bitmap(targetWidth, targetHeight))
            using (Graphics g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(original, 0, 0, targetWidth, targetHeight);
                return BitmapToJpeg(scaled, quality);
            }
        }

        /// <summary>
        /// Bitmap 转 JPEG 字节数组
        /// </summary>
        private byte[] BitmapToJpeg(Bitmap bmp, int quality)
        {
            ImageCodecInfo jpegCodec = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bmp.Save(ms, jpegCodec, encoderParams);
                return ms.ToArray();
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public int Width { get { return _width; } }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public int Height { get { return _height; } }

        public void Dispose()
        {
            if (!_disposed)
            {
                SelectObject(_memoryDC, _oldBitmap);
                DeleteObject(_bitmap);
                DeleteDC(_memoryDC);
                ReleaseDC(GetDesktopWindow(), _desktopDC);
                _disposed = true;
            }
        }
    }
}
