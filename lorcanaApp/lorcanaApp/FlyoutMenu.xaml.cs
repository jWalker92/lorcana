using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using lorcana.Cards;
using Xamarin.Essentials;
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

        void Booster_Clicked(object sender, EventArgs e)
        {
            App.NavigationPageInstance.PushAsync(new BoosterOpenPage());
            App.FlyoutInstance.IsPresented = false;
        }

        async void Export_Clicked(System.Object sender, System.EventArgs e)
        {
            App.FlyoutInstance.IsPresented = false;
            var csvContent = CardCollection.ListToCsv(await Database.Instance.GetCardsAsync());

            var choice = await DisplayActionSheet("Export", "Cancel", null, "Share File", "To Clipboard");
            if (choice == "Share File")
            {
                var fn = "app_export.csv";
                var file = Path.Combine(FileSystem.CacheDirectory, fn);
                File.WriteAllText(file, csvContent);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Collection Export",
                    File = new ShareFile(file)
                });
            }
            else if (choice == "To Clipboard")
            {
                await Clipboard.SetTextAsync(csvContent);
            }
        }

        void Import_Clicked(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                App.FlyoutInstance.IsPresented = false;
                var importDataStr = string.Empty;
                var choice = await DisplayActionSheet("Import", "Cancel", null, "From File", "From Text");
                if (choice == "From File")
                {
                    var pickedFile = await FilePicker.PickAsync();
                    if (pickedFile != null)
                    {
                        importDataStr = File.ReadAllText(pickedFile.FullPath);
                    }
                }
                else if (choice == "From Text")
                {
                    importDataStr = await PasteTextPage.ShowDialog();
                }
                if (string.IsNullOrEmpty(importDataStr))
                {
                    return;
                }
                var importedCollection = new CardCollection();
                importedCollection.InitializeWithCsv(CardLibrary.List, importDataStr);
                foreach (var card in importedCollection.List)
                {
                    await Database.Instance.AddOrReplaceCardAsync(card);
                }
                Database.Instance.FireCollectionChanged();
            });
        }
    }
}

