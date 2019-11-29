using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;

namespace Blackboard.Command
{
    public class StrokesAddedOrRemovedCI : CommandItem
    {
        private InkCanvasEditingMode editingMode;
        private StrokeCollection added;
        private StrokeCollection removed;
        private int editingOperationCount;

        public StrokesAddedOrRemovedCI(CommandStack stack, InkCanvasEditingMode editMode, StrokeCollection add, StrokeCollection remove, int editCount)
            : base(stack)
        {
            editingMode = editMode;
            added = add;
            removed = remove;
            editingOperationCount = editCount;
        }

        public override void Undo()
        {
            if (commandStack == null)
            {
                return;
            }

            if (added != null)
            {
                commandStack.StrokeCollection.Remove(added);
            }

            if (removed != null)
            {
                commandStack.StrokeCollection.Add(removed);
            }
        }

        public override void Redo()
        {
            if (commandStack == null)
            {
                return;
            }

            if (added != null)
            {
                commandStack.StrokeCollection.Add(added);
            }

            if (removed != null)
            {
                commandStack.StrokeCollection.Remove(removed);
            }
        }

        public override bool Merge(CommandItem newItem)
        {
            StrokesAddedOrRemovedCI strokeItem = (StrokesAddedOrRemovedCI)newItem;
            if (strokeItem != null ||
                strokeItem.editingMode != editingMode ||
                strokeItem.editingOperationCount != editingOperationCount)
            {
                return false;
            }

            if (editingMode != InkCanvasEditingMode.EraseByPoint ||
                strokeItem.editingMode != InkCanvasEditingMode.EraseByPoint)
            {
                return false;
            }

            foreach (Stroke stroke in strokeItem.removed)
            {
                if (added.Contains(stroke))
                {
                    added.Remove(stroke);
                }
                else
                {
                    removed.Add(stroke);
                }
            }

            added.Add(strokeItem.added);

            return true;
        }
    }
}
