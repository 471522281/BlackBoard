using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Interop;
using System.IO;
using System.Drawing.Imaging;
using System.Resources;

namespace Blackboard.Util
{
    public static class ImageSourceHelper
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Bitmap转换为ImageSource
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <returns>ImageSource</returns>
        public static ImageSource GetImageSourceByBitmap(this System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }


        public static ImageSource GetImageSourceByIcoPath(string icoPath)
        {
            if (!File.Exists(icoPath))
            {
                throw new FileNotFoundException();
            }

            Icon icon = new Icon(icoPath);
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

        public static byte[] Bitmap2Byte(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }


        public static System.Windows.Controls.Image GetImage(string url)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            return image;
        }


        public static ImageSource GetImageSource(Uri url)
        {
            BitmapImage bitmapImage = new BitmapImage(url);
            return bitmapImage;
        }

        public static ImageSource GetImageSource(string imgFilePath)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imgFilePath, UriKind.RelativeOrAbsolute));
            return bitmapImage;
        }

        /// <summary>
        /// ImageSource转Bitmap
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap GetBitmapToBitmapSource(this BitmapSource s)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s.PixelWidth, s.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            s.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }


        /// <summary> 
        /// 按照指定大小缩放图片 
        /// </summary> 
        /// <param name="srcImage"></param> 
        /// <param name="iWidth"></param> 
        /// <param name="iHeight"></param> 
        /// <returns></returns> 
        public static Bitmap SizeImage(this Image srcImage, int iWidth, int iHeight)
        {
            try
            {
                if (srcImage == null)
                {
                    return null;
                }

                // 要保存到的图片 
                Bitmap b = new Bitmap(iWidth, iHeight);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量 
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(srcImage, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 按照图片大小等比例缩放
        /// </summary>
        /// <param name="srcImage"></param>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <returns></returns>
        public static Bitmap SizeImage_1(this Image srcImage, double scale)
        {
            try
            {
                if (srcImage == null)
                {
                    return null;
                }
                int iWidth = (int)(srcImage.Width * scale);
                int iHeight = (int)(srcImage.Height * scale);
                // 要保存到的图片 
                Bitmap b = new Bitmap(iWidth, iHeight);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量 
                g.InterpolationMode = InterpolationMode.Default;
                g.DrawImage(srcImage, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 图片加图片水印
        /// </summary>
        /// <param name="sourceBmp"></param>
        /// <param name="copyImage"></param>
        /// <returns></returns>
        public static Bitmap WaterMarkForBmp(Bitmap sourceBmp, Image copyImage)
        {
            Graphics g = Graphics.FromImage(sourceBmp);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(copyImage, new Rectangle(sourceBmp.Width - copyImage.Width, sourceBmp.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
            g.Dispose();
            return sourceBmp;
        }
        public static Bitmap WaterMarkForText(Bitmap sourceBmp, string value)
        {
            Graphics g = Graphics.FromImage(sourceBmp);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // g.DrawImage(sourceBmp, 0, 0, sourceBmp.Width, sourceBmp.Height);
            Font f = new Font("Verdana", 32);
            System.Drawing.Point p = new System.Drawing.Point(sourceBmp.Width - 300, sourceBmp.Height - 100);

            // 阴影
            System.Drawing.Brush TransparentBrush0 = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
            System.Drawing.Brush TransparentBrush1 = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
            g.DrawString(value, f, TransparentBrush0, p.X, p.Y + 1);
            g.DrawString(value, f, TransparentBrush0, p.X + 1, p.Y);

            System.Drawing.Brush b = new SolidBrush(System.Drawing.Color.White);
            string addText = value;
            g.DrawString(addText, f, b, p);
            g.Dispose();

            return sourceBmp;
        }


        /// <summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private static System.Drawing.Imaging.PixelFormat[] indexedPixelFormats = { System.Drawing.Imaging.PixelFormat.Undefined, System.Drawing.Imaging.PixelFormat.DontCare,
            System.Drawing.Imaging.PixelFormat.Format16bppArgb1555, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, System.Drawing.Imaging.PixelFormat.Format4bppIndexed,
            System.Drawing.Imaging.PixelFormat.Format8bppIndexed
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgPixelFormat"></param>
        /// <returns></returns>

        private static bool IsPixelFormatIndexed(System.Drawing.Imaging.PixelFormat imgPixelFormat)
        {
            foreach (System.Drawing.Imaging.PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }

            return false;
        }


        /// <summary>
        /// 图片的克隆
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public static Bitmap BitmapClone(this Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);
            if (IsPixelFormatIndexed(sourceBitmap.PixelFormat))
            {
                bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.DrawImage(bmp, 0, 0);
                }
            }
            Graphics g_1 = Graphics.FromImage(bmp);
            g_1.CompositingQuality = CompositingQuality.HighQuality;
            g_1.SmoothingMode = SmoothingMode.HighQuality;
            g_1.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g_1.DrawImage(sourceBitmap, 0, 0, sourceBitmap.Width, sourceBitmap.Height);
            g_1.Dispose();
            return bmp;
        }
        /// <summary>
        /// 将Bitmap图片转换成byte字节数组
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);
            ms.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 等比例缩放
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static System.Windows.Size GetScalingSize(System.Windows.Size from, System.Windows.Size area)
        {

            System.Windows.Size result = new System.Windows.Size(from.Width, from.Height);

            if (from.Width > area.Width)
            {
                double d = area.Width / from.Width;
                result.Width = from.Width * d;
                result.Height = from.Height * d;
            }

            if (result.Height > area.Height)
            {
                double d = area.Height / result.Height;
                result.Height = result.Height * d;
                result.Width = result.Width * d;
            }

            return result;

        }


        /// <summary>
        /// 将byte字节数组转换成Bitmap图片
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] bytes, string filepath)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();
            bmp.Save(filepath);
            return bmp;
        }

        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="Bytes">字节数组</param>
        /// <returns>图片</returns>
        public static System.Drawing.Image byteArrayToImage(byte[] Bytes, string filepath)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            Image outputImg = Image.FromStream(ms);
            outputImg.Save(filepath);
            return outputImg;
        }


        /// <summary>
        /// 将byte字节数组转换成Bitmap图片
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] bytes)//,bool issave=false)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Bitmap bmp = new Bitmap(ms);
            //if (issave)
            //{
            //    bmp.Save(@"D:\abc.jpg");
            //}
            ms.Close();
            return bmp;
        }

        /// <summary>
        /// 将BitmapImage图片转换成byte字节数组
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] BitmapImageToBytes(BitmapImage bmp)
        {
            byte[] bytes = null;
            Stream s = bmp.StreamSource;
            s.Position = 0; //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。 
            using (BinaryReader br = new BinaryReader(s))
            {
                bytes = br.ReadBytes((int)s.Length);
            }
            return bytes;
        }


        /// <summary>
        /// 将byte字节数组转换成BitmapImage图片
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static BitmapImage BytesToBitmapImage(byte[] bytes)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(bytes);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        /// <summary>
        /// 将Bitmap图片转换成BitmapImage图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(this Bitmap bitmap)
        {
            return BytesToBitmapImage(BitmapToBytes(bitmap));
        }

        /// <summary>
        /// 将BitmapImage图片转换成Bitmap图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(this BitmapImage bitmapImage)
        {
            return BytesToBitmap(BitmapImageToBytes(bitmapImage));
        }

        public static Image ByteToImage(byte[] byteArrayIn)
        {
            /// 二进制图片流
            if (byteArrayIn == null)
                return null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                ms.Flush();
                return returnImage;
            }
        }

        #region 新截图方式

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;

        /// <summary>
        /// GDI+截取全屏图片
        /// </summary>
        /// <returns></returns>
        public static Bitmap CaptureScreenFun(bool canResize, int DstWidth, int DstHeight)
        {
            Bitmap result = null;

            try
            {
                result = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(result);
                {
                    g.CopyFromScreen(0, 0, 0, 0, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                    if (GetCursorInfo(out pci))
                    {
                        if (pci.flags == CURSOR_SHOWING)
                        {
                            DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                            g.ReleaseHdc();
                        }
                    }

                    g.Dispose();
                    g = null;
                }
            }
            catch
            {
                result = null;
            }

            if (canResize && result != null)
            {
                if (DstWidth != result.Width || DstHeight != result.Height)
                {
                    return KiResizeImage(result, DstWidth, DstHeight);
                }
            }

            return result;
        }

        /// <summary>
        /// 重新调整图片大小
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="newW"></param>
        /// <param name="newH"></param>
        /// <returns></returns>
        private static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                g = null;

                return b;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
