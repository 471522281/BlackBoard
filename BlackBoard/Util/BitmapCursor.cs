using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Blackboard.Util
{
    public class BitmapCursor : SafeHandle
    {
        public override bool IsInvalid
        {
            get
            {
                // test
                return handle == (IntPtr)(-1);
            }
        }


        public static Cursor CreateBmpCursor(Bitmap cursorBitmap)
        {
            var c = new BitmapCursor(cursorBitmap);

            return CursorInteropHelper.Create(c);
        }

        protected BitmapCursor(Bitmap cursorBitmap)
            : base((IntPtr)(-1), true)
        {
            handle = cursorBitmap.GetHicon();
        }


        protected override bool ReleaseHandle()
        {
            bool result = DestroyIcon(handle);

            handle = (IntPtr)(-1);

            return result;
        }

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public static Cursor CreateCrossCursor()
        {
            const int w = 25;
            const int h = 25;

            var bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.Default;
            g.InterpolationMode = InterpolationMode.High;
            var pen = new System.Drawing.Pen(Brushes.Black, 2);
            g.DrawLine(pen, new Point(12, 0), new Point(12, 8)); // vertical line 
            g.DrawLine(pen, new Point(12, 17), new Point(12, 25)); // vertical line
            g.DrawLine(pen, new Point(0, 12), new Point(8, 12)); // horizontal line 
            g.DrawLine(pen, new Point(16, 12), new Point(24, 12)); // horizontal line
            g.DrawLine(pen, new Point(12, 12), new Point(12, 13)); // Middle dot 
            g.Flush();
            g.Dispose();
            pen.Dispose();
            var c = CreateBmpCursor(bmp);
            bmp.Dispose();
            return c;
        }

        public static Cursor CreateCrossCursor(Brush brush, int radius)
        {
            int w = 20 + radius * 2;
            int p = w / 2;
            int h = 6;

            var bmp = new Bitmap(w, w);

            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.Default;
            g.InterpolationMode = InterpolationMode.High;

            var pen = new System.Drawing.Pen(Brushes.Black, 2);
            g.DrawLine(pen, new Point(p, 0), new Point(p, h)); // vertical line 
            g.DrawLine(pen, new Point(p, w - h), new Point(p, w)); // vertical line
            g.DrawLine(pen, new Point(0, p), new Point(h, p)); // horizontal line 
            g.DrawLine(pen, new Point(w - h, p), new Point(w, p)); // horizontal line

            g.FillEllipse(brush, p - radius, p - radius, radius * 2, radius * 2); // Middle dot 
            g.Flush();
            g.Dispose();
            pen.Dispose();

            var c = CreateBmpCursor(bmp);

            bmp.Dispose();

            return c;
        }

        /// <summary>
        /// BitmapSource转为Bitmap
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static Bitmap CreateBitmap(BitmapSource m)
        {
            Bitmap bmp = new Bitmap(m.PixelWidth, m.PixelHeight, PixelFormat.Format32bppPArgb);

            BitmapData data = bmp.LockBits(
            new Rectangle(Point.Empty, bmp.Size),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppPArgb);
            m.CopyPixels(
                System.Windows.Int32Rect.Empty,
                data.Scan0,
                data.Height * data.Stride,
                data.Stride);
            bmp.UnlockBits(data);

            return bmp;
        }

        /// <summary>
        /// 创建图片鼠标
        /// </summary>
        /// <param name="bmpImage"></param>
        /// <returns></returns>
        public static Cursor CreateImageCursor(BitmapImage bmpImage)
        {
            Bitmap curBmp = CreateBitmap(bmpImage);
            int x = 7;//鼠标笔尖左侧偏移量，固定不可更改
            int y = 5;//鼠标笔尖右侧偏移量，固定不可更改
            Bitmap bmp = new Bitmap(curBmp.Width / 2 * 3, curBmp.Height / 2 * 3);

            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.Default;
            g.InterpolationMode = InterpolationMode.High;

            Pen pen = new Pen(Brushes.Black, 2);
            g.DrawImage(curBmp, curBmp.Width / 2 + x, curBmp.Height / 2 + y, curBmp.Width - x, curBmp.Height - y);

            g.Flush();
            g.Dispose();
            pen.Dispose();

            var c = CreateBmpCursor(bmp);

            bmp.Dispose();
            curBmp.Dispose();

            return c;
        }

        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path">文件的完整路径</param>
        /// <returns>返回结果</returns>
        public static Boolean IsImage(string path)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
