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

        private static Dictionary<string, SvgImageSource> cache = new Dictionary<string, SvgImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rarity rarity)
            {
                switch (rarity)
                {
                    case Rarity.Common:
                        return SvgSource("lorcanaApp.Resources.Rarity.Common.svg");
                    case Rarity.Uncommon:
                        return SvgSource("lorcanaApp.Resources.Rarity.Uncommon.svg");
                    case Rarity.Rare:
                        return SvgSource("lorcanaApp.Resources.Rarity.Rare.svg");
                    case Rarity.SuperRare:
                        return SvgSource("lorcanaApp.Resources.Rarity.SuperRare.svg");
                    case Rarity.Legendary:
                        return SvgSource("lorcanaApp.Resources.Rarity.Legendary.svg");
                    default:
                        break;
                }
            }
            return value;
        }

        private SvgImageSource SvgSource(string source)
        {
            if (cache.ContainsKey(source))
            {
                return cache[source];
            }
            var svgImgSrc = SvgImageSource.FromResource(source, typeof(App));
            cache.Add(source, svgImgSrc);
            return svgImgSrc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

