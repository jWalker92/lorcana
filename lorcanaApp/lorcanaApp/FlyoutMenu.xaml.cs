using System;

using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class FlyoutMenu : ContentPage
	{	
		public FlyoutMenu ()
		{
			InitializeComponent ();
        }

        void Counter_Clicked(object sender, EventArgs e)
        {
            App.NavigationPageInstance.PushAsync(new LoreCounterPage());
            App.FlyoutInstance.IsPresented = false;
        }

        void Language_Clicked(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                var res = await DisplayActionSheet("Choose Images Language", "Cancel", null, "English", "German");
                if (res == null)
                {
                    return;
                }
                App.Language = res;
                App.NavigationPageInstance = new NavigationPage(new MainPage());
                var gradient = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                gradient.GradientStops.Add(new GradientStop { Color = Color.FromHex("#2E3192"), Offset = 0.0f });
                gradient.GradientStops.Add(new GradientStop { Color = Color.FromHex("#540D6E"), Offset = 1.0f });
                App.NavigationPageInstance.BarBackground = gradient;
                App.NavigationPageInstance.BarTextColor = Color.WhiteSmoke;
                App.FlyoutInstance.Detail = App.NavigationPageInstance;
                App.FlyoutInstance.IsPresented = false;
            });
        }
    }
}

