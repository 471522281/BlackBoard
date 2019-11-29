using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blackboard.Core;
using Blackboard.Enum;
using Blackboard.Util;

namespace Blackboard.UserControls
{
    /// <summary>
    /// BoardCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class BoardCanvas : UserControl
    {
        private Size boardSize = new Size(3, 3);
        /// <summary>
        /// 笔画粗细
        /// </summary>
        public Size BoardSize
        {
            get { return boardSize; }
            private set
            {
                boardSize = value;
                SelectOperation();
            }
        }

        /// <summary>
        /// 设置画笔颜色
        /// </summary>
        public Color BoardColor
        {
            get
            {
                return canvas.DefaultDrawingAttributes.Color;
            }
            private set
            {
                canvas.DefaultDrawingAttributes.Color = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        private NormalPen currentPen;
        /// <summary>
        /// 当前画笔
        /// </summary>
        public NormalPen CurrentPen
        {
            get { return currentPen; }
            set
            {
                currentPen = value;
                BoardColor = value.PenColor;
                BoardSize = value.PenSize;
            }
        }

        /// <summary>
        /// 内容是否已经更改
        /// </summary>
        public bool IsChange
        {
            get { return canvas.IsChanged; }
            set { canvas.IsChanged = value; }
        }

        /// <summary>
        /// 黑板是否有内容
        /// </summary>
        public bool IsHasContent
        {
            get
            {
                return hasContent || canvas.CmdStack.CanRedo || canvas.CmdStack.CanUndo;
            }
        }

        /// <summary>
        /// 当前操作
        /// </summary>
        public BoardOperation CurrentOperation { get; set; }

        private bool hasContent = false;

        public BoardCanvas()
        {
            InitializeComponent();

            canvas.DefaultDrawingAttributes.FitToCurve = true;
            canvas.DefaultDrawingAttributes.IgnorePressure = true;
        }

        private void SelectOperation()
        {
            switch (CurrentOperation)
            {
                case BoardOperation.Pen:
                    SelectPen();
                    break;
                case BoardOperation.Eraser:
                    SelectEraser();
                    break;
                case BoardOperation.Select:
                    break;
                default:
                    break;
            }
        }

        public StrokeCollection GetStrokes()
        {
            return this.canvas.Strokes;
        }

        public void SetStrokes(StrokeCollection st)
        {
            this.canvas.Strokes = st;
        }

        public void Select(bool isSelect)
        {
            if (isSelect)
            {
                CurrentOperation = BoardOperation.Select;
                canvas.EditingMode = InkCanvasEditingMode.None;
                Grid.SetZIndex(canvas, 0);
                canvas.UseCustomCursor = false;
            }
            else
            {
                canvas.UseCustomCursor = true;
                Grid.SetZIndex(canvas, 1);
            }
        }

        /// <summary>
        /// 选择画笔状态
        /// </summary>
        public void SelectPen()
        {
            CurrentOperation = BoardOperation.Pen;
            Select(false);

            canvas.EditingMode = InkCanvasEditingMode.Ink;
            canvas.DefaultDrawingAttributes.Height = BoardSize.Height;
            canvas.DefaultDrawingAttributes.Width = BoardSize.Width;
            canvas.DefaultDrawingAttributes.IsHighlighter = false;
            canvas.UseCustomCursor = true;

            //System.Drawing.SolidBrush colorBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(BoardColor.A, BoardColor.R, BoardColor.G, BoardColor.B));
            //canvas.Cursor = BitmapCursor.CreateCrossCursor(colorBrush, (int)canvas.DefaultDrawingAttributes.Width);
            canvas.Cursor = CreateCursorByPenType();
        }

        public void SelectEraser()
        {
            CurrentOperation = BoardOperation.Eraser;
            Select(false);
            //矩形橡皮
            //RectangleStylusShape point = new RectangleStylusShape(BoardSize.Width * 10, BoardSize.Height * 10);
            //canvas.EraserShape = point;
            //圆形橡皮
            EllipseStylusShape shape = new EllipseStylusShape(BoardSize.Width * 10, BoardSize.Height * 10);
            canvas.EraserShape = shape;
            //先设置操作模式为空在设置操作模式为橡皮，避免出现无法切换橡皮大小
            canvas.EditingMode = InkCanvasEditingMode.None;
            canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            canvas.UseCustomCursor = false;
        }

        /// <summary>
        /// 撤销操作
        /// </summary>
        public void Undo()
        {
            this.canvas.Undo();
        }

        /// <summary>
        /// 重做操作
        /// </summary>
        public void Redo()
        {
            this.canvas.Redo();
        }

        /// <summary>
        /// 根据画笔颜色创建对应的鼠标
        /// </summary>
        /// <returns></returns>
        private Cursor CreateCursorByPenType()
        {
            BitmapImage data;
            switch (CurrentPen.PenColorType)
            {
                case ColorPenType.Pen_White:
                    data = (BitmapImage)TryFindResource("cur_white");
                    break;
                case ColorPenType.Pen_Black:
                    data = (BitmapImage)TryFindResource("cur_black");
                    break;
                case ColorPenType.Pen_Blue:
                    data = (BitmapImage)TryFindResource("cur_blue");
                    break;
                case ColorPenType.Pen_Green:
                    data = (BitmapImage)TryFindResource("cur_green");
                    break;
                case ColorPenType.Pen_Red:
                    data = (BitmapImage)TryFindResource("cur_red");
                    break;
                case ColorPenType.Pen_Yellow:
                    data = (BitmapImage)TryFindResource("cur_yellow");
                    break;
                default:
                    data = (BitmapImage)TryFindResource("cur_white");
                    break;
            }

            Cursor cursor = BitmapCursor.CreateImageCursor(data);

            return cursor;
        }
    }
}
