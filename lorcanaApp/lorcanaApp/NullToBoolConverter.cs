using System;
using System.Globalization;
using Xamarin.Forms;

namespace lorcanaApp
{
    public class NullToBoolConverter : IValueConverter
	{
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (value is string str)
            {
                return string.IsNullOrEmpty(str);
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

