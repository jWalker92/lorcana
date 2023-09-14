using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace lorcanaApp
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent();
            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            gradient.GradientStops.Add(new GradientStop { Color = Color.FromHex("#2E3192"), Offset = 0.0f });
            gradient.GradientStops.Add(new GradientStop { Color = Color.FromHex("#540D6E"), Offset = 1.0f });

            MainPage = new NavigationPage(new MainPage()) { BarBackground = gradient, BarTextColor = Color.WhiteSmoke };
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

