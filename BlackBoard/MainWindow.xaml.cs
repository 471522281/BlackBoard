using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
using Blackboard.UserControls;
using Blackboard.Util;
using Blackboard.Enum;
using Blackboard.Visiuals;

namespace Blackboard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardOperation currentMode = BoardOperation.Normal;
        /// <summary>
        /// 当前操作模式
        /// </summary>
        public BoardOperation CurrentMode
        {
            get { return currentMode; }
            set { currentMode = value; }
        }

        private NormalPen currentPen = new NormalPen();
        /// <summary>
        /// 当前画笔
        /// </summary>
        public NormalPen CurrentPen
        {
            get { return currentPen; }
            set { currentPen = value; }
        }

        private ObservableCollection<BoardCanvas> boardCanvas = new ObservableCollection<BoardCanvas>();
        /// <summary>
        /// 画板
        /// </summary>
        public ObservableCollection<BoardCanvas> BoardCanvas
        {
            get { return boardCanvas; }
            set { boardCanvas = value; }
        }

        /// <summary>
        /// 当前黑板
        /// </summary>
        public BoardCanvas CurrentBoard
        {
            get
            {
                if (BoardCanvas == null ||
                    BoardCanvas.Count == 0 ||
                    BoardCanvas.Count <= currentBoardIndex)
                {
                    return null;
                }

                return BoardCanvas[currentBoardIndex];
            }
        }

        private double oneWidth;
        private double oneHeight;
        private int currentBoardIndex;//当前画板序号
        private int currentPageIndex;//当前题目序号

        public MainWindow()
        {
            InitializeComponent();
            InitSize();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitEvent();
            InitCanvas();
            AddCanvas();
            DragMove_Click(this, null);
        }

        private void Window_UnLoaded(object sender, RoutedEventArgs e)
        {
            FreeEvent();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is GridSplitter)
            {
                return;
            }
            //自动浮现切换试题按钮
            Point p = e.GetPosition(this);
            if (p.X >= 0 && p.X <= this.turnLeftBtn.ActualWidth + 20)
            {
                this.turnLeftBtn.btn.Opacity = 1;
            }
            else
            {
                this.turnLeftBtn.btn.Opacity = 0;
            }

            if (p.X >= this.Width - this.turnRightBtn.ActualWidth - 20 && p.X <= this.Width)
            {
                this.turnRightBtn.btn.Opacity = 1;
            }
            else
            {
                this.turnRightBtn.btn.Opacity = 0;
            }

            if (e.LeftButton == MouseButtonState.Pressed &&
                CurrentMode == BoardOperation.Select)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// 初始化画布
        /// </summary>
        private void InitCanvas()
        {
            oneWidth = mainCanvas.ActualWidth;
            oneHeight = mainCanvas.ActualHeight;
            mainPanel.Width = oneWidth;
            mainPanel.Height = oneHeight;
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void InitEvent()
        {
            this.colorPenTool.ChangeColorPenTypeEvent += ChangeColorPenType;
            this.colorPenTool.ChangeColorPenSizeEvent += ChangeColorPenSize;
            this.colorPenTool.CloseEvent += ColorPenToolClose;
            this.colorPenTool.UndoEvent += ColorPenToolUndo;
            this.colorPenTool.RedoEvent += ColorPenToolRedo;
            this.turnLeftBtn.ExchangeEvent += TurnLeft;
            this.turnRightBtn.ExchangeEvent += TurnRight;
            BoardCanvas.CollectionChanged += BoardCanvas_CollectionChanged;
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        private void FreeEvent()
        {
            this.colorPenTool.ChangeColorPenTypeEvent -= ChangeColorPenType;
            this.colorPenTool.ChangeColorPenSizeEvent -= ChangeColorPenSize;
            this.colorPenTool.CloseEvent -= ColorPenToolClose;
            this.colorPenTool.UndoEvent -= ColorPenToolUndo;
            this.colorPenTool.RedoEvent -= ColorPenToolRedo;
            this.turnLeftBtn.ExchangeEvent -= TurnLeft;
            this.turnRightBtn.ExchangeEvent -= TurnRight;
            BoardCanvas.CollectionChanged -= BoardCanvas_CollectionChanged;
        }

        /// <summary>
        /// 创建一个画板并且添加
        /// </summary>
        private void AddCanvas()
        {
            BoardCanvas boardCanvas = new BoardCanvas();
            boardCanvas.Width = oneWidth;
            boardCanvas.Height = oneHeight;
            boardCanvas.CurrentOperation = CurrentMode;
            boardCanvas.CurrentPen = CurrentPen;
            boardCanvas.IsChange = true;

            mainPanel.Children.Add(boardCanvas);
            mainPanel.Width = (BoardCanvas.Count + 1) * oneWidth;

            Canvas.SetLeft(mainPanel, -BoardCanvas.Count * oneWidth);

            BoardCanvas.Add(boardCanvas);
            currentBoardIndex = BoardCanvas.Count - 1;
        }

        /// <summary>
        /// 初始化窗口大小
        /// </summary>
        private void InitSize()
        {
            double width = SystemParameters.WorkArea.Width;
            double height = SystemParameters.WorkArea.Height;
            this.Width = width;
            this.Height = height;
        }

        #region 按钮事件

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMessage("确定要退出黑板？"))
            {
                this.Close();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            BoardCanvas[currentBoardIndex].canvas.Clear();
        }

        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMode == BoardOperation.Eraser)
            {
                return;
            }

            if (CurrentMode == BoardOperation.Pen)
            {
                this.eraserbg.Visibility = Visibility.Visible;
                this.penbg.Visibility = Visibility.Visible;
                this.colorPenTool.Visibility = Visibility.Visible;
                this.dragbg.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.eraserbg.Visibility = Visibility.Visible;
                this.penbg.Visibility = Visibility.Collapsed;
                this.colorPenTool.Visibility = Visibility.Collapsed;
                this.dragbg.Visibility = Visibility.Collapsed;
            }

            CurrentMode = BoardOperation.Eraser;

            if (currentBoardIndex <= BoardCanvas.Count)
            {
                BoardCanvas[currentBoardIndex].SelectEraser();
            }
        }

        private void Pen_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMode == BoardOperation.Pen)
            {
                return;
            }

            ChangeColorPenType(CurrentPen.PenColorType);
        }

        private void DragMove_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMode == BoardOperation.Select)
            {
                ResetPostion();
                return;
            }

            CurrentMode = BoardOperation.Select;

            SetSelectImage(false);
            this.eraserbg.Visibility = Visibility.Collapsed;
            this.penbg.Visibility = Visibility.Collapsed;
            this.colorPenTool.Visibility = Visibility.Collapsed;
            this.dragbg.Visibility = Visibility.Visible;

            BoardCanvas.ToList().ForEach(p => p.Select(true));

            Panel.SetZIndex(this.mainCanvas, 0);
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            string fileName = BoardHelper.SaveCameraPicture();
            if (ShowMessage("保存成功，是否查看图片？"))
            {
                Process.Start("Explorer", "/select," + fileName);//打开目录并选中文件
            }
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isCancel"></param>
        /// <returns></returns>
        private bool ShowMessage(string message, bool isCancel = true)
        {
            BlackboardMessageBox dlg = new BlackboardMessageBox(message, isCancel);
            dlg.Owner = this;
            dlg.ShowDialog();

            return (bool)dlg.DialogResult;
        }

        /// <summary>
        /// 选择画笔模式
        /// </summary>
        private void SelectPen()
        {
            CurrentMode = BoardOperation.Pen;

            SetSelectImage(true);
            this.eraserbg.Visibility = Visibility.Collapsed;
            this.penbg.Visibility = Visibility.Visible;
            this.colorPenTool.Visibility = Visibility.Visible;
            this.dragbg.Visibility = Visibility.Collapsed;

            this.colorPenTool.SelectPen(CurrentPen);
            Panel.SetZIndex(this.mainCanvas, 1);
        }

        /// <summary>
        /// 重置黑板位置
        /// </summary>
        private void ResetPostion()
        {
            this.Left = 0;
            this.Top = 0;
        }

        /// <summary>
        /// 设置拖动图标
        /// </summary>
        /// <param name="isArrow"></param>
        private void SetSelectImage(bool isArrow)
        {
            BitmapImage resetImg;
            if (isArrow)
            {
                resetImg = (BitmapImage)this.TryFindResource("img_drag");
                this.txtMove.Content = "拖动";
            }
            else
            {
                resetImg = (BitmapImage)this.TryFindResource("img_reset");
                this.txtMove.Content = "归位";
            }

            if (resetImg != null)
            {
                this.imgMove.Source = resetImg;
            }
        }

        #endregion

        #region 更新画笔信息

        /// <summary>
        /// 画笔切换
        /// </summary>
        /// <param name="type"></param>
        private void ChangeColorPenType(ColorPenType type)
        {
            CurrentPen.PenColorType = type;
            if (type != ColorPenType.Pen_None)
            {
                BoardCanvas.ToList().ForEach(p => p.CurrentPen = CurrentPen);
                BoardCanvas.ToList().ForEach(p => p.SelectPen());
            }
            SelectPen();
        }

        /// <summary>
        /// 画笔粗细切换
        /// </summary>
        /// <param name="size"></param>
        private void ChangeColorPenSize(ColorPenSize size)
        {
            CurrentPen.PenSizeType = size;
            if (size != ColorPenSize.Size_None)
            {
                BoardCanvas.ToList().ForEach(p => p.CurrentPen = CurrentPen);
            }
        }

        /// <summary>
        /// 撤销动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPenToolUndo(object sender, EventArgs e)
        {
            if (currentBoardIndex <= BoardCanvas.Count)
            {
                BoardCanvas[currentBoardIndex].Undo();
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPenToolRedo(object sender, EventArgs e)
        {
            if (currentBoardIndex <= BoardCanvas.Count)
            {
                BoardCanvas[currentBoardIndex].Redo();
            }
        }

        /// <summary>
        /// 画笔关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPenToolClose(object sender, EventArgs e)
        {
            DragMove_Click(this, null);
        }

        /// <summary>
        /// 向左切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurnLeft(object sender, EventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
            }
        }

        /// <summary>
        /// 向右切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurnRight(object sender, EventArgs e)
        {
            currentPageIndex++;
        }

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
        }

        private void BoardCanvas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (BoardCanvas.Count > 1)
            //{
            //    this.turnLeftBtn.Visibility = Visibility.Visible;
            //    this.turnRightBtn.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    this.turnLeftBtn.Visibility = Visibility.Collapsed;
            //    this.turnRightBtn.Visibility = Visibility.Collapsed;
            //}
        }

        /// <summary>
        /// 开始屏幕录制
        /// </summary>
        private void Instance_RecordStartEvent()
        {
            Console.WriteLine("开始屏幕录制");
        }

        /// <summary>
        /// 停止屏幕录制
        /// </summary>
        private void Instance_RecordStopEvent()
        {
            Console.WriteLine("停止屏幕录制");
        }

        #endregion

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void ShowDlg()
        {
            this.Left = 0;
            this.Top = 0;
            //this.Topmost = true;
            this.Show();
            this.Activate();
        }

        public void ShowTop()
        {
            this.Left = 0;
            this.Top = 0;
            this.Topmost = true;
            //this.Show();
            //this.Activate();
            this.ShowDialog();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MiniWindow dlg = WindowHelper.GetOrShowWindow<MiniWindow>();
            dlg.Show();
            dlg.HideBySide();
        }

        private void SizeAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SizeReduce_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
