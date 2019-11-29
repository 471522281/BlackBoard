using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using Blackboard.Command;

namespace Blackboard.Core
{
    /// <summary>
    /// 黑板真正画布
    /// </summary>
    public class EClassCanvas : InkCanvas, ICanvas
    {
        private int editingOperationCount;//画笔总数
        private StrokeCollectionConverter c = new StrokeCollectionConverter();

        private CommandStack cmdStack;
        /// <summary>
        /// 操作栈
        /// </summary>
        public CommandStack CmdStack
        {
            get { return cmdStack; }
        }

        public bool IsChanged { get; set; }

        public EClassCanvas()
        {
            cmdStack = new CommandStack(this.Strokes);
            cmdStack.StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            this.Strokes.StrokesChanged += Strokes_StrokesChanged;
            this.SelectionMoving += EClassCanvas_SelectionMoving;
            this.SelectionResizing += EClassCanvas_SelectionMoving;
            this.MouseUp += EClassCanvas_MouseUp;
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }

        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            StrokeCollection added = new StrokeCollection(e.Added);
            StrokeCollection removed = new StrokeCollection(e.Removed);

            CommandItem item = new StrokesAddedOrRemovedCI(cmdStack, this.EditingMode, added, removed, editingOperationCount);
            cmdStack.Enqueue(item);
        }

        private void EClassCanvas_SelectionMoving(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            Rect newRect = e.NewRectangle;
            Rect oldRect = e.OldRectangle;

            if (newRect.Top < 0d || newRect.Left < 0d)
            {
                double x = newRect.Left < 0d ? 0d : newRect.Left;
                double y = newRect.Top < 0d ? 0d : newRect.Top;

                Rect newRectItem = new Rect(x, y, newRect.Width, newRect.Height);
                e.NewRectangle = newRectItem;
            }

            CommandItem item = new SelectionMovedOrResizedCI(cmdStack, this.GetSelectedStrokes(), newRect, oldRect, editingOperationCount);
            cmdStack.Enqueue(item);
        }

        private void EClassCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            editingOperationCount++;
        }

        public void Undo()
        {
            cmdStack.Undo();
        }

        public void Redo()
        {
            cmdStack.Redo();
        }

        public void Clear()
        {
            cmdStack.Clear();
        }

        public void ClearAll()
        {
            cmdStack.ClearAll();
        }

        public void MyUndo()
        {
            Undo();
        }

        public void MyRedo()
        {
            Redo();
        }

        /// <summary>
        /// 保存笔迹数据到字符串
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string data = c.ConvertToString(this.Strokes);
            return data;
        }

        /// <summary>
        /// 加载字符串数据为笔迹数据
        /// </summary>
        /// <param name="data"></param>
        public void Load(string data)
        {
            this.Strokes = (StrokeCollection)c.ConvertFromString(data);
        }
    }
}
