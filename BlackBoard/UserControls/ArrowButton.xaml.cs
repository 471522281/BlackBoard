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
using Orientation = Blackboard.Enum.Orientation;

namespace Blackboard.UserControls
{
    /// <summary>
    /// ArrowButton.xaml 的交互逻辑
    /// </summary>
    public partial class ArrowButton : UserControl
    {
        /// <summary>
        /// 点击切换事件
        /// </summary>
        public event EventHandler ExchangeEvent;

        public ArrowButton()
        {
            InitializeComponent();
        }

        public static DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(ArrowButton),
            new FrameworkPropertyMetadata(Orientation.Left, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// 箭头方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArrowButton btn = sender as ArrowButton;
            if (btn.Orientation == Orientation.Left)
            {
                btn.imgArrow.Source = (BitmapImage)btn.TryFindResource("img_leftArrow");
                btn.ToolTip = "上一个";
            }
            else if (btn.Orientation == Orientation.Right)
            {
                btn.imgArrow.Source = (BitmapImage)btn.TryFindResource("img_rightArrow");
                btn.ToolTip = "下一个";
            }
        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            ExchangeEvent?.Invoke(this, new EventArgs());
        }
    }
}
