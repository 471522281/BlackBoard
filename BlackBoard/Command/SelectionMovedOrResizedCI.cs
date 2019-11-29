using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;


namespace Blackboard.Command
{
    public class SelectionMovedOrResizedCI : CommandItem
    {
        private StrokeCollection selection;
        private Rect newRect;
        private Rect oldRect;
        private int editingOperationCount;

        public SelectionMovedOrResizedCI(CommandStack stack, StrokeCollection selectionItem, Rect newRectItem, Rect oldRectItem, int editCount)
            : base(stack)
        {
            selection = selectionItem;
            newRect = newRectItem;
            oldRect = oldRectItem;
            editingOperationCount = editCount;
        }

        public override void Undo()
        {
            Matrix m = GetTransformFromRectToRect(newRect, oldRect);
            selection.Transform(m, false);
        }

        public override void Redo()
        {
            Matrix m = GetTransformFromRectToRect(oldRect, newRect);
            selection.Transform(m, false);
        }

        public override bool Merge(CommandItem newItem)
        {
            SelectionMovedOrResizedCI selItem = (SelectionMovedOrResizedCI)newItem;
            if (selItem == null ||
                selItem.editingOperationCount != editingOperationCount ||
                !StrokeCollectionsAreEqual(selItem.selection, selection))
            {
                return false;
            }

            newRect = selItem.newRect;

            return true;
        }

        private Matrix GetTransformFromRectToRect(Rect source, Rect target)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-source.X, -source.Y);
            m.Scale(target.Width / source.Width, target.Height / source.Height);
            m.Translate(+target.X, +target.Y);

            return m;
        }

        private bool StrokeCollectionsAreEqual(StrokeCollection a, StrokeCollection b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
