using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lorcana.Cards;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class MainPage : ContentPage
    {

        const string allCardsCache = "allCards.json";
        const string allCardsInfoCache = "allCardsInfo.json";

        List<string> pickerItems = new List<string> {
            "All",
            "Owned",
            "Owned Once",
            "Owned Twice",
            "Owned Thrice",
            "Full Play Set",
            "Tradeable (>=5)",
            "Tradeable (>8)",
            "Missing"
        };
        private CardCollection collection;
        private List<Card> filteredList;

        public MainPage()
        {
            InitializeComponent();
            listPicker.ItemsSource = pickerItems;
            listPicker.SelectedIndex = 0;
            listPicker.SelectedIndexChanged += ListPicker_SelectedIndexChanged;
            Task.Run(async () => { await BuildLibraryAndCollection(); await LoadData(); });
        }

        private void ListPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Run(LoadData);
        }

        private async Task BuildLibraryAndCollection()
        {
            try
            {

                string allCardsJson = Preferences.Get(allCardsCache, "");
                string allCardsInfoJson = Preferences.Get(allCardsInfoCache, "");
                await CardLibrary.BuildLibrary(allCardsJson, allCardsInfoJson);
                Preferences.Set(allCardsCache, CardLibrary.AllCardsJson);
                Preferences.Set(allCardsInfoCache, CardLibrary.AllCardsInfoJson);
                string contents = Preferences.Get("contents", "{}");
                collection = new CardCollection(CardLibrary.List, contents);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task LoadData()
        {
            try
            {
                filteredList = new List<Card>();
                switch (listPicker.SelectedIndex)
                {
                    case 0:
                        filteredList = collection.List.ToList();
                        filteredList.AddRange(CardLibrary.List.Where(x => !collection.List.Any(y => y.Number == x.Number)));
                        filteredList = filteredList.OrderBy(x => x.Number).ToList();
                        break;
                    case 1:
                        filteredList = collection.List.ToList();
                        break;
                    case 2:
                        filteredList = collection.List.Where(x => x.Total == 1).ToList();
                        break;
                    case 3:
                        filteredList = collection.List.Where(x => x.Total == 2).ToList();
                        break;
                    case 4:
                        filteredList = collection.List.Where(x => x.Total == 3).ToList();
                        break;
                    case 5:
                        filteredList = collection.List.Where(x => x.Total >= 4).ToList();
                        break;
                    case 6:
                        filteredList = collection.List.Where(x => x.Total >= 5).ToList();
                        break;
                    case 7:
                        filteredList = collection.List.Where(x => x.Total > 8).ToList();
                        break;
                    case 8:
                        filteredList = CardLibrary.List.Where(x => !collection.List.Any(y => y.Number == x.Number)).ToList();
                        break;
                }
                Device.BeginInvokeOnMainThread(() => cardsList.ItemsSource = filteredList);
            }
            catch (Exception ex)
            {

            }
        }

        async void Import_Clicked(System.Object sender, System.EventArgs e)
        {
            grid.RaiseChild(importView);
            importView.IsVisible = true;
        }

        void Save_Clicked(System.Object sender, System.EventArgs e)
        {
            var importContent = importEditor.Text;
            if (!string.IsNullOrWhiteSpace(importContent))
            {
                try
                {
                    collection = new CardCollection(CardLibrary.List, importContent);
                    Preferences.Set("contents", importContent);
                    Task.Run(LoadData);
                    importEditor.Text = null;
                    importView.IsVisible = false;
                }
                catch (Exception ex)
                {

                }
            }
        }

        void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            importEditor.Text = null;
            importView.IsVisible = false;
        }

        void cardsList_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (cardsList.SelectedItem == null)
            {
                return;
            }
            if (cardsList.SelectedItem is Card card)
            {
                int index = filteredList.IndexOf(card);
                Navigation.PushAsync(new CardDetailPage(filteredList, index));
            }
            cardsList.SelectedItem = null;
        }
    }
}


