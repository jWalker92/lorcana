using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class App : Application
    {
        public static NavigationPage NavigationPageInstance { get; internal set; }

        public static FlyoutPage FlyoutInstance { get; internal set; }
        public static string Language { get => Preferences.Get("language", "English"); set => Preferences.Set("language", value); }
        public static string CountryCode => GetCountryCode(Language);

        private static string GetCountryCode(string language)
        {
            switch (language)
            {
                case "English":
                    return "en";
                case "German":
                    return "de";
                default:
                    return "en";
            }
        }

        public App ()
        {
            InitializeComponent();
            Task.Run(Database.Instance.Init);
            MainPage = FlyoutInstance = new MainFlyoutPage();
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

        internal static void ReInit()
        {
            CardListPage.CardLibrary = null;
            NavigationPageInstance = new NavigationPage(new CardListPage());
            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };
            gradient.GradientStops.Add(new GradientStop { Color = Color.FromHex("#2E3192"), Offset = 0.0f });
            gradient.GradientStops.Add(new GradientStop { Color = Color.FromHex("#540D6E"), Offset = 1.0f });
            NavigationPageInstance.BarBackground = gradient;
            NavigationPageInstance.BarTextColor = Color.WhiteSmoke;
            FlyoutInstance.Detail = NavigationPageInstance;
            FlyoutInstance.IsPresented = false;
        }
    }
}

