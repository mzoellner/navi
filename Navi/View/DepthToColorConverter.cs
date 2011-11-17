using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Navi.View
{
    public class DepthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = (int)value;

            if (val == 0)
                return Brushes.Red;
            else if (val == 1)
                return Brushes.Orange;
            else
                return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
