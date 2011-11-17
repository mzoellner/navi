using System;
using System.Windows.Data;
using System.Windows;

namespace Navi.View
{
    public class PulseConverter : DependencyObject, IValueConverter
    {

        private int[] _pulses;

        public PulseConverter()
        {
            _pulses = new int[] { 0, 500, 5000 };
        }



        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int pulse = (int)value;

            return _pulses[pulse];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
