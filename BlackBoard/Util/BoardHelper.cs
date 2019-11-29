using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Blackboard.Util
{
    public static class BoardHelper
    {
        /// <summary>
        /// 黑板截屏默认保存文件夹
        /// </summary>
        public const string CameraDir = "Camera";

        /// <summary>
        /// 当前程序路径
        /// </summary>
        private static string ExeDir
        {
            get
            {
                return Path.Combine(
                    Assembly.GetEntryAssembly().Location.Substring(
                    0, Assembly.GetEntryAssembly().Location.LastIndexOf(
                    Path.DirectorySeparatorChar)));
            }
        }

        /// <summary>
        /// 桌面路径
        /// </summary>
        private static string DesktopDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); }
        }

        /// <summary>
        /// 保存屏幕截图
        /// </summary>
        public static string SaveCameraPicture()
        {
            string path = Path.Combine(ExeDir, CameraDir);
            //保存目标路径，创建路径失败则保存至桌面路径
            if (!CheckDir(path))
            {
                path = DesktopDir;
            }
            string name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";
            string fileName = Path.Combine(path, name);
            Bitmap bmp = DesktopImage.GetDesktopImage(true);
            bmp.Save(fileName, ImageFormat.Bmp);
            bmp.Dispose();
            bmp = null;

            return fileName;
        }

        /// <summary>
        /// 检查目录结构是否存在
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool CheckDir(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
