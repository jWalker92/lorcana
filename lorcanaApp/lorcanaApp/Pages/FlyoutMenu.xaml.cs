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

        void Language_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                var res = await DisplayActionSheet("Choose Images Language", "Cancel", null, "English", "German");
                if (res == null)
                {
                    return;
                }
                App.Language = res;
                App.ReInit();
            });
        }

        void Booster_Clicked(object sender, EventArgs e)
        {
            App.NavigationPageInstance.PushAsync(new BoosterOpenPage());
            App.FlyoutInstance.IsPresented = false;
        }

        void Export_Clicked(System.Object sender, System.EventArgs e)
        {
            ImportManager.ExportCards(this);
        }

        void Import_Clicked(System.Object sender, System.EventArgs e)
        {
            ImportManager.ImportCards(this);
        }

        void Delete_Clicked(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                var res = await DisplayAlert("Delete Database", "Do you really want to delete your database? There is no going back. You might want to export it first.", "Delete", "Cancel");
                if (res)
                {
                    await Database.Instance.ClearDB();
                    App.ReInit();
                }
            });
        }
    }
}

