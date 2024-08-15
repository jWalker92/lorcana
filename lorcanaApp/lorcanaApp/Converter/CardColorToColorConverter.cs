using System;
using System.Globalization;
using lorcana.Cards;
using Xamarin.Forms;

namespace lorcanaApp
{
    public class CardColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CardColor color)
            {
                return Color.FromHex(Helpers.HexStringFromColor(color));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

