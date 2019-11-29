using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackboard.Core
{
    /// <summary>
    /// 画布接口
    /// </summary>
    public interface ICanvas
    {
        void Undo();

        void Redo();

        void MyUndo();

        void MyRedo();

        void Clear();
    }
}
