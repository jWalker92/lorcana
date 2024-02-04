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
    }
}

