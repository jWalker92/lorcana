using System;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace lorcanaApp
{
	public class SvgImageButton : Frame
	{
        public static BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(SvgImageButton), default(ImageSource), propertyChanged: Source_Changed);

        private static void Source_Changed(BindableObject bindable, object oldValue, object newValue)
        {
			if (bindable is SvgImageButton btn)
			{
				btn._img.Source = newValue as ImageSource;
			}
        }

        public ImageSource Source
		{
			get => (ImageSource)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

        public static BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SvgImageButton), default(ICommand));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private readonly SvgCachedImage _img;

        public event EventHandler Clicked;

		public SvgImageButton()
		{
			Padding = 8;
            Content = _img = new SvgCachedImage();
            GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() =>
            {
                Opacity = .6f;
                this.FadeTo(1);
                Command?.Execute(null);
                Clicked?.Invoke(this, EventArgs.Empty);
            })});
        }
	}
}

