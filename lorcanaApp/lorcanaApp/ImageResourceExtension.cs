using System;
using System.Collections.Generic;
using System.Reflection;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace lorcanaApp
{
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        private static Dictionary<string, SvgImageSource> cache = new Dictionary<string, SvgImageSource>();
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            return GetImageResource(Source);
        }

        public static ImageSource GetImageResource(string path)
        {
            if (path.EndsWith(".svg"))
            {
                return SvgSource(path);
            }
            var imageSource = ImageSource.FromResource(path, typeof(App).GetTypeInfo().Assembly);
            return imageSource;
        }

        private static SvgImageSource SvgSource(string source)
        {
            if (cache.ContainsKey(source))
            {
                return cache[source];
            }
            var svgImgSrc = SvgImageSource.FromResource(source, typeof(App));
            cache.Add(source, svgImgSrc);
            return svgImgSrc;
        }
    }
}
