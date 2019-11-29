using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Blackboard.Util
{
    /// <summary>
    /// 获取当前屏幕图片，包括透明窗里
    /// </summary>
    public class DesktopImage
    {
        // GetSystemMetrics() codes
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern int GetDC(int hwnd);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern int GetDesktopWindow();

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern int CreateCompatibleDC(int hdc);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern int SelectObject(int hdc, int hObject);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern int BitBlt(
            int hDestDC,
            int x,
            int y,
            int nWidth,
            int nHeight,
            int hSrcDC,
            int xSrc,
            int ySrc,
            int dwRop
        );

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern int DeleteDC(int hdc);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC(int hwnd, int hdc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern int DeleteObject(int hObject);

        public enum TernaryRasterOperations
        {
            SRCCOPY = 0x00CC0020, // dest = source  
            SRCPAINT = 0x00EE0086, // dest = source OR dest
            SRCAND = 0x008800C6, // dest = source AND dest   
            SRCINVERT = 0x00660046, // dest = source XOR dest     
            SRCERASE = 0x00440328, // dest = source AND (NOT dest)      
            NOTSRCCOPY = 0x00330008, // dest = (NOT source)      
            NOTSRCERASE = 0x001100A6, // dest = (NOT src) AND (NOT dest)     
            MERGECOPY = 0x00C000CA, // dest = (source AND pattern)   
            MERGEPAINT = 0x00BB0226, // dest = (NOT source) OR dest      
            PATCOPY = 0x00F00021, // dest = pattern   
            PATPAINT = 0x00FB0A09, // dest = DPSnoo   
            PATINVERT = 0x005A0049, // dest = pattern XOR dest   
            DSTINVERT = 0x00550009, // dest = (NOT dest)   
            BLACKNESS = 0x00000042, // dest = BLACK   
            CaptureBlt = 1073741824,
        }

        public static Bitmap GetDesktopImage(bool cursorDisplay = false)
        {
            //In size variable we shall keep the size of the screen.  
            Size size = new Size();
            //Variable to keep the handle to bitmap.   
            int hBitmap;
            //Here we get the handle to the desktop device context.      
            int hDC = GetDC(GetDesktopWindow());
            //Here we make a compatible device context in memory for screen device context.   
            int hMemDC = CreateCompatibleDC(hDC);
            // We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.  
            size.Width = GetSystemMetrics(SM_CXSCREEN);
            //We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.      
            size.Height = GetSystemMetrics(SM_CYSCREEN);
            //We create a compatible bitmap of screen size using screen device context. 
            hBitmap = CreateCompatibleBitmap(hDC, size.Width, size.Height);
            //As hBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.       
            if (hBitmap != 0)
            {
                //Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.   
                int hOld = SelectObject(hMemDC, hBitmap);
                //We copy the Bitmap to the memory device context.     
                BitBlt(hMemDC, 0, 0, size.Width, size.Height, hDC, 0, 0, (int)(TernaryRasterOperations.SRCCOPY | TernaryRasterOperations.CaptureBlt));
                //We select the old bitmap back to the memory device context.   
                SelectObject(hMemDC, hOld);
                //We delete the memory device context.         
                DeleteDC(hMemDC);
                //We release the screen device context.   
                ReleaseDC(GetDesktopWindow(), hDC);
                //Image is created by Image bitmap handle and stored in local variable.    
                Bitmap bmp = System.Drawing.Image.FromHbitmap((IntPtr)hBitmap);
                //Release the memory to avoid memory leaks.      
                DeleteObject(hBitmap);

                // 画鼠标
                if (cursorDisplay)
                {
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        //graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));

                        //显示鼠标
                        DrawMouse(graphics);
                    }
                }
                //This statement runs the garbage collector manually. 
                //GC.Collect();
                //Return the bitmap    
                return bmp;
            }
            //   If hBitmap is null retunrn null.   
            return null;
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
