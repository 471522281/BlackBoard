using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Blackboard.Enum;

namespace Blackboard.Converter
{
    [ValueConversion(typeof(BoardOperation), typeof(Visibility))]
    public class DragModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visual = Visibility.Collapsed;
            BoardOperation mode = (BoardOperation)value;

            switch (mode)
            {
                case BoardOperation.Select:
                    visual = Visibility.Visible;
                    break;
                default:
                    break;
            }

            return visual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
