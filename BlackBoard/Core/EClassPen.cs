using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackboard.Enum;

namespace Blackboard.Core
{
    /// <summary>
    /// 画笔
    /// </summary>
    public class EClassPen
    {
        private ColorPenType penColorType = ColorPenType.Pen_None;
        /// <summary>
        /// 画笔颜色类型
        /// </summary>
        public ColorPenType PenColorType
        {
            get { return penColorType; }
            set { penColorType = value; }
        }

        private ColorPenSize penSizeType = ColorPenSize.Size_None;
        /// <summary>
        /// 笔画大小
        /// </summary>
        public ColorPenSize PenSizeType
        {
            get { return penSizeType; }
            set { penSizeType = value; }
        }

        private EClassPenType penType = EClassPenType.Pen_Normal;
        /// <summary>
        /// 画笔类型
        /// </summary>
        public EClassPenType PenType
        {
            get { return penType; }
            set { penType = value; }
        }
    }
}
