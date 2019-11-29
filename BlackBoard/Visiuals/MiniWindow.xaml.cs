using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Blackboard.Util;

namespace Blackboard.Visiuals
{
    /// <summary>
    /// MiniWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MiniWindow : Window
    {
        public MiniWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = WindowHelper.GetScreenWidth();
            this.Height = WindowHelper.GetScreenHeight();
        }

        private void Thumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenBlackboard();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            ShowBySide();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Drag(e.HorizontalChange, e.VerticalChange);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            HideBySide();
        }

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowBySide();
        }

        private void Thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            HideBySide();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenBlackboard();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 获取位置信息

        private double GetInScreenRight(FrameworkElement control, double horizontalChange)
        {
            double right = Canvas.GetRight(control) - horizontalChange;
            right = right >= this.Width - control.Width ? this.Width - control.Width : right;
            right = right <= 0 ? 0 : right;
            return right;
        }

        private double GetInScreenButtom(FrameworkElement control, double verticalChange)
        {
            double bottom = Canvas.GetBottom(control) - verticalChange;
            bottom = bottom >= this.Height - control.Height ? this.Height - control.Height : bottom;
            bottom = bottom <= 0 ? 0 : bottom;
            return bottom;
        }

        private double GetInScreenLeft(FrameworkElement control, double horizontalChange)
        {
            double left = Canvas.GetLeft(control) + horizontalChange;
            left = left < 0 ? 0 : left;
            left = this.Width < (control.Width + left) ? (this.Width - control.Width) : left;
            return left;
        }

        private double GetInScreenTop(FrameworkElement control, double verticalChange)
        {
            double top = Canvas.GetTop(control) + verticalChange;
            top = top < 0 ? 0 : top;
            top = this.Height < (control.Height + top) ? (this.Height - control.Height) : top;
            return top;
        }

        #endregion

        private void Drag(double horizontalChange, double herticalChange)
        {
            double right = GetInScreenRight(logoThumb, horizontalChange);
            double bottom = GetInScreenButtom(logoThumb, herticalChange);
            Canvas.SetRight(logoThumb, right);
            Canvas.SetBottom(logoThumb, bottom);

            //做条调试
            //double x = Canvas.GetRight(logoThumb);
            //double y = Canvas.GetBottom(logoThumb);
            //this.point.Text = string.Format("({0}.{1})", x, y);
        }

        private void OpenBlackboard()
        {
            this.Hide();
            MainWindow dlg = WindowHelper.GetOrShowWindow<MainWindow>();
            dlg.ShowDialog();
        }

        private void ShowBySide()
        {
            double x = Canvas.GetRight(logoThumb);
            double y = Canvas.GetBottom(logoThumb);
            //贴边隐藏后启动拖动
            if (x <= 0)
            {
                Canvas.SetRight(logoThumb, 0);
            }

            if (x >= this.Width - logoThumb.Width)
            {
                Canvas.SetRight(logoThumb, this.Width - logoThumb.Width);
            }

            if (y <= 0)
            {
                Canvas.SetBottom(logoThumb, 0);
            }

            if (y >= this.Height - logoThumb.Height)
            {
                Canvas.SetBottom(logoThumb, this.Height - logoThumb.Height);
            }
        }

        public void HideBySide()
        {
            double x = Canvas.GetRight(logoThumb);
            double y = Canvas.GetBottom(logoThumb);
            //贴边隐藏
            if (x == 0)
            {
                Canvas.SetRight(logoThumb, -logoThumb.Width / 2);
            }

            if (x == this.Width - logoThumb.Width)
            {
                Canvas.SetRight(logoThumb, this.Width - logoThumb.Width / 2);
            }

            if (y == 0)
            {
                Canvas.SetBottom(logoThumb, -logoThumb.Height / 2);
            }

            if (y == this.Height - logoThumb.Height)
            {
                Canvas.SetBottom(logoThumb, this.Height - logoThumb.Height / 2);
            }
        }
    }
}
