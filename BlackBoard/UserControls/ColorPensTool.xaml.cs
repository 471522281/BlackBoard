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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blackboard.Core;
using Blackboard.Enum;

namespace Blackboard.UserControls
{
    public delegate void ChangeColorPenTypeEventHandler(ColorPenType penType);//定义更改画笔颜色委托
    public delegate void ChangeColorPenSizeEventHandler(ColorPenSize penSize);//定义更改画笔粗细委托
    /// <summary>
    /// ColorPens.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPensTool : UserControl
    {

        public event EventHandler CloseEvent;//关闭画笔事件
        public event EventHandler UndoEvent;//取消上一次操作事件
        public event EventHandler RedoEvent;//重做事件
        public event ChangeColorPenTypeEventHandler ChangeColorPenTypeEvent;//更改画笔颜色事件
        public event ChangeColorPenSizeEventHandler ChangeColorPenSizeEvent;//更改画笔粗细事件

        private ColorPenType currentPen = ColorPenType.Pen_None;
        /// <summary>
        /// 当前画笔
        /// </summary>
        public ColorPenType CurrentPen
        {
            get { return currentPen; }
            set { currentPen = value; }
        }

        private ColorPenSize currentPenSize = ColorPenSize.Size_None;
        /// <summary>
        /// 当前画笔粗细
        /// </summary>
        public ColorPenSize CurrentPenSize
        {
            get { return currentPenSize; }
            set { currentPenSize = value; }
        }

        public ColorPensTool()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void SelectPen(EClassPen pen)
        {
            //第一次默认选中时默认选中白色中等粗细画笔
            if (pen.PenColorType == ColorPenType.Pen_None)
            {
                SelectColorPen(ColorPenType.Pen_White);
                SelectColorPenSize(ColorPenSize.Size_Normal);
                return;
            }

            SelectColorPen(pen.PenColorType);
            SelectColorPenSize(pen.PenSizeType);
        }

        /// <summary>
        /// 更改画笔
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="type"></param>
        public void SelectColorPen(ColorPenType type)
        {
            if (CurrentPen == type)
            {
                return;
            }

            CurrentPen = type;
            InitPenType();
            switch (CurrentPen)
            {
                case ColorPenType.Pen_White:
                    this.whitepen.Margin = new Thickness(0, -20, 0, 20);
                    break;
                case ColorPenType.Pen_Black:
                    this.blackpen.Margin = new Thickness(0, -20, 0, 20);
                    break;
                case ColorPenType.Pen_Blue:
                    this.bluepen.Margin = new Thickness(0, -20, 0, 20);
                    break;
                case ColorPenType.Pen_Green:
                    this.greenpen.Margin = new Thickness(0, -20, 0, 20);
                    break;
                case ColorPenType.Pen_Red:
                    this.redpen.Margin = new Thickness(0, -20, 0, 20);
                    break;
                case ColorPenType.Pen_Yellow:
                    this.yellowpen.Margin = new Thickness(0, -20, 0, 20);
                    break;
                default:
                    this.whitepen.Margin = new Thickness(0, -20, 0, 20);
                    break;
            }
            //通知更改了画笔
            if (ChangeColorPenTypeEvent != null)
            {
                ChangeColorPenTypeEvent(CurrentPen);
            }
        }

        /// <summary>
        /// 更改画笔粗细
        /// </summary>
        /// <param name="size"></param>
        public void SelectColorPenSize(ColorPenSize size)
        {
            if (CurrentPenSize == size)
            {
                return;
            }

            CurrentPenSize = size;
            InitPenSize();
            switch (CurrentPenSize)
            {
                case ColorPenSize.Size_Thin:
                    this.imgthin.Visibility = Visibility.Visible;
                    break;
                case ColorPenSize.Size_Normal:
                    this.imgnormal.Visibility = Visibility.Visible;
                    break;
                case ColorPenSize.Size_Thick:
                    this.imgthick.Visibility = Visibility.Visible;
                    break;
                default:
                    this.imgthin.Visibility = Visibility.Visible;
                    break;
            }
            //通知主界面更改了画笔粗细
            if (ChangeColorPenSizeEvent != null)
            {
                ChangeColorPenSizeEvent(CurrentPenSize);
            }
        }

        /// <summary>
        /// 初始化画笔显示模式
        /// </summary>
        private void InitPenType()
        {
            this.whitepen.Margin = new Thickness(0);
            this.blackpen.Margin = new Thickness(0);
            this.bluepen.Margin = new Thickness(0);
            this.redpen.Margin = new Thickness(0);
            this.greenpen.Margin = new Thickness(0);
            this.yellowpen.Margin = new Thickness(0);
        }

        /// <summary>
        /// 初始化画笔粗细显示模式
        /// </summary>
        private void InitPenSize()
        {
            this.imgthin.Visibility = Visibility.Collapsed;
            this.imgnormal.Visibility = Visibility.Collapsed;
            this.imgthick.Visibility = Visibility.Collapsed;
        }

        private void WhitePen_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPen(ColorPenType.Pen_White);
        }

        private void Black_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPen(ColorPenType.Pen_Black);
        }

        private void Red_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPen(ColorPenType.Pen_Red);
        }

        private void Yellow_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPen(ColorPenType.Pen_Yellow);
        }

        private void Green_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPen(ColorPenType.Pen_Green);
        }

        private void Blue_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPen(ColorPenType.Pen_Blue);
        }

        private void SelectThin_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPenSize(ColorPenSize.Size_Thin);
        }

        private void SelectNormal_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPenSize(ColorPenSize.Size_Normal);
        }

        private void SelectThick_Click(object sender, RoutedEventArgs e)
        {
            SelectColorPenSize(ColorPenSize.Size_Thick);
        }


        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (UndoEvent != null)
            {
                UndoEvent(this, new EventArgs());
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (RedoEvent != null)
            {
                RedoEvent(this, new EventArgs());
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent(this, null);
            }
        }
    }
}
