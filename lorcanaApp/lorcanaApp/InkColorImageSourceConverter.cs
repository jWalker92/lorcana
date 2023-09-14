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

        private static Dictionary<string, SvgImageSource> cache = new Dictionary<string, SvgImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CardColor color)
            {
                switch (color)
                {
                    case CardColor.Amber:
                        return SvgSource("lorcanaApp.Resources.Inks.Amber.svg");
                    case CardColor.Amethyst:
                        return SvgSource("lorcanaApp.Resources.Inks.Amethyst.svg");
                    case CardColor.Emerald:
                        return SvgSource("lorcanaApp.Resources.Inks.Emerald.svg");
                    case CardColor.Ruby:
                        return SvgSource("lorcanaApp.Resources.Inks.Ruby.svg");
                    case CardColor.Sapphire:
                        return SvgSource("lorcanaApp.Resources.Inks.Sapphire.svg");
                    case CardColor.Steel:
                        return SvgSource("lorcanaApp.Resources.Inks.Steel.svg");
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

