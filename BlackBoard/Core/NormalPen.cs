using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Blackboard.Enum;

namespace Blackboard.Core
{
    public class NormalPen : EClassPen
    {
        public new EClassPenType PenType
        {
            get { return EClassPenType.Pen_Normal; }
        }

        /// <summary>
        /// 当前画笔颜色色值
        /// </summary>
        public Color PenColor
        {
            get
            {
                return ChangeColorPenType(PenColorType);
            }
        }

        /// <summary>
        /// 画笔粗细度值
        /// </summary>
        public Size PenSize
        {
            get { return ChangeColorPenSize(PenSizeType); }
        }

        /// <summary>
        /// 根据画笔颜色匹配色值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Color ChangeColorPenType(ColorPenType type)
        {
            Color color;
            switch (type)
            {
                case ColorPenType.Pen_White:
                    color = Colors.White;
                    break;
                case ColorPenType.Pen_Black:
                    color = Colors.Black;
                    break;
                case ColorPenType.Pen_Blue:
                    color = Colors.Blue;
                    break;
                case ColorPenType.Pen_Green:
                    color = Colors.Green;
                    break;
                case ColorPenType.Pen_Red:
                    color = Colors.Red;
                    break;
                case ColorPenType.Pen_Yellow:
                    color = Colors.Yellow;
                    break;
                default:
                    color = Colors.White;
                    break;
            }

            return color;
        }

        /// <summary>
        /// 根据画笔粗细匹配粗细值
        /// </summary>
        /// <param name="size"></param>
        private Size ChangeColorPenSize(ColorPenSize size)
        {
            Size penSize;
            switch (size)
            {
                case ColorPenSize.Size_Thin:
                    penSize = new Size(1, 1);
                    break;
                case ColorPenSize.Size_Normal:
                    penSize = new Size(3, 3);
                    break;
                case ColorPenSize.Size_Thick:
                    penSize = new Size(5, 5);
                    break;
                default:
                    penSize = new Size(3, 3);
                    break;
            }

            return penSize;
        }
    }
}
