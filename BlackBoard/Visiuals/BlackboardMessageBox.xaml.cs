using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Blackboard.Visiuals
{
    /// <summary>
    /// BlackboardMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class BlackboardMessageBox : Window
    {
        private string head;
        /// <summary>
        /// 标题
        /// </summary>
        public string Head
        {
            get { return head; }
            set
            {
                head = value;
                this.titleTxt.Content = value;
            }
        }

        private string message;
        /// <summary>
        /// 显示文字
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                this.showTxt.Text = Message;
            }
        }

        public BlackboardMessageBox()
        {
            InitializeComponent();
            this.Style = (Style)this.TryFindResource("shadowStyle");
        }

        public BlackboardMessageBox(string message)
            : this()
        {
            Message = message;
        }

        public BlackboardMessageBox(string title, string message)
            : this(message)
        {
            Head = title;
        }

        public BlackboardMessageBox(string message, bool isCancel)
            : this(message)
        {
            if (isCancel)
            {
                this.cancelBtn.Visibility = Visibility.Visible;
            }
            else
            {
                this.cancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MoveInLegalArea())
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// 判定移动区域是否合法
        /// </summary>
        /// <returns></returns>
        private bool MoveInLegalArea()
        {
            double width = SystemParameters.WorkArea.Width;
            double height = SystemParameters.WorkArea.Height;
            if (this.Left < 0)
            {
                this.Left = 0;
                return false;
            }

            if (this.Top < 0)
            {
                this.Top = 0;
                return false;
            }

            if (this.Left > width - this.ActualWidth)
            {
                this.Left = width - this.ActualWidth;
                return false;
            }

            if (this.Top > height - this.ActualHeight)
            {
                this.Top = height - this.ActualHeight;
                return false;
            }

            return true;
        }
    }
}
