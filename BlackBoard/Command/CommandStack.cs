using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace Blackboard.Command
{
    public class CommandStack
    {
        public event StrokeCollectionChangedEventHandler UserStrokeChange;

        private Stack<CommandItem> undoStack;
        private Stack<CommandItem> redoStack;
        private bool disableChangeTracing;

        private StrokeCollection strokeCollection;

        public StrokeCollection StrokeCollection
        {
            get { return strokeCollection; }
        }

        public CommandStack(StrokeCollection stroke)
        {
            if (stroke == null)
            {
                return;
            }

            strokeCollection = stroke;
            undoStack = new Stack<CommandItem>();
            redoStack = new Stack<CommandItem>();
            disableChangeTracing = false;
        }

        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        public void Clear()
        {
            strokeCollection.Clear();
        }

        public void ClearAll()
        {
            strokeCollection.Clear();
            undoStack.Clear();
            redoStack.Clear();
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            CommandItem item = undoStack.Pop();
            disableChangeTracing = true;
            try
            {
                item.Undo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                disableChangeTracing = false;
            }
            redoStack.Push(item);
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                return;
            }

            CommandItem item = redoStack.Pop();
            disableChangeTracing = true;
            try
            {
                item.Redo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                disableChangeTracing = false;
            }
            undoStack.Push(item);
        }

        /// <summary>
        /// 添加动作到动作栈中
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(CommandItem item)
        {
            if (item == null)
            {
                return;
            }

            if (disableChangeTracing)
            {
                return;
            }

            bool merged = false;
            if (undoStack.Count > 0)
            {
                CommandItem prev = undoStack.Peek();
                merged = prev.Merge(item);
            }

            if (!merged)
            {
                undoStack.Push(item);
                if (UserStrokeChange != null)
                {
                    UserStrokeChange(null, null);
                }
            }

            //清除重做动作栈
            if (redoStack.Count > 0)
            {
                redoStack.Clear();
            }
        }
    }
}
