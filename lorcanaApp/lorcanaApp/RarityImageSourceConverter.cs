using System;
using System.Collections.Generic;
using System.Globalization;
using FFImageLoading.Svg.Forms;
using lorcana.Cards;
using Xamarin.Forms;

namespace lorcanaApp
{
    public class RarityImageSourceConverter : IValueConverter
	{


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rarity rarity)
            {
                switch (rarity)
                {
                    case Rarity.Common:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Rarity.Common.svg");
                    case Rarity.Uncommon:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Rarity.Uncommon.svg");
                    case Rarity.Rare:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Rarity.Rare.svg");
                    case Rarity.SuperRare:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Rarity.SuperRare.svg");
                    case Rarity.Legendary:
                        return ImageResourceExtension.GetImageResource("lorcanaApp.Resources.Rarity.Legendary.svg");
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

