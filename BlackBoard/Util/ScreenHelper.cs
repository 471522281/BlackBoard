using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Blackboard.Util
{
    public class ScreenHelper
    {
        /// <summary>
        /// 获取屏幕截图
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetWindow_screen(bool mouse)
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.Bicubic;
                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));

                //显示鼠标
                if (mouse)
                {
                    DrawMouse(graphics);
                }
            }
            return bmp;
        }



        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
            // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
            //    0             The cursor is hidden.
            //    CURSOR_SHOWING    The cursor is showing.
            public IntPtr hCursor;          // Handle to the cursor. 
            public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        private const Int32 CURSOR_SHOWING = 0x00000001;
        public const int DI_NORMAL = 0x0003;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon,
           int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw,
           int diFlags);

        public static void DrawMouse(Graphics g)
        {
            CURSORINFO pci = new CURSORINFO();
            pci.cbSize = Marshal.SizeOf(pci);
            GetCursorInfo(out pci);
            IntPtr dc = g.GetHdc();
            DrawIconEx(dc, pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor, 32, 32, 1, IntPtr.Zero, DI_NORMAL);
            g.ReleaseHdc();
        }
    }
}
