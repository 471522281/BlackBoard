using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Blackboard.Util
{
    public class WindowHelper
    {
        /// <summary>
        /// 获取屏幕宽度
        /// </summary>
        /// <param name="isHaveTaskbar">包含任务栏</param>
        public static double GetScreenWidth(bool isHaveTaskbar = false)
        {
            return isHaveTaskbar ? SystemParameters.PrimaryScreenWidth : SystemParameters.WorkArea.Width;
        }

        /// <summary>
        /// 获取屏幕高度
        /// </summary>
        /// <param name="isHaveTaskbar">包含任务栏</param>
        public static double GetScreenHeight(bool isHaveTaskbar = false)
        {
            return isHaveTaskbar ? SystemParameters.PrimaryScreenHeight : SystemParameters.WorkArea.Height;
        }

        public static T ExistWindow<T>() where T : class
        {
            object win = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is T)
                    {
                        win = window;
                        break;
                    }
                }
            }));
            return (T)win;
        }

        public static T GetOrShowWindow<T>() where T : class
        {
            object win = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is T)
                    {
                        win = window;
                        break;
                    }
                }

                if (win == null)
                {
                    try
                    {
                        win = Activator.CreateInstance(typeof(T));
                    }
                    catch { }
                }
            }));
            return (T)win;
        }
    }
}
