using System;
using System.Collections.Generic;
using System.Globalization;
using FFImageLoading.Svg.Forms;
using lorcana.Cards;
using Xamarin.Forms;

namespace lorcanaApp
{
    public class InkColorImageSourceConverter : IValueConverter
	{

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CardColor color)
            {
                switch (color)
                {
                    case CardColor.Amber:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Inks.Amber.svg");
                    case CardColor.Amethyst:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Inks.Amethyst.svg");
                    case CardColor.Emerald:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Inks.Emerald.svg");
                    case CardColor.Ruby:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Inks.Ruby.svg");
                    case CardColor.Sapphire:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Inks.Sapphire.svg");
                    case CardColor.Steel:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Inks.Steel.svg");
                    default:
                        break;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

