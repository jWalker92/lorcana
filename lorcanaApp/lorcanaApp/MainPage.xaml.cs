using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using lorcana.Cards;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class MainPage : ContentPage
    {
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
            "Missing for Play Set",
            "Missing"
        };
        private CardCollection collection;
        private List<Card> filteredList;
        private List<Card> filteredAndSearchedList;
        private bool isLoading = false;

        public MainPage()
        {
            InitializeComponent();
            headerLabel.Text = "Loading...";
            listPicker.ItemsSource = pickerItems;
            listPicker.SelectedIndex = 0;
            listPicker.SelectedIndexChanged += ListPicker_SelectedIndexChanged;
            Task.Run(async () => {
                if (isLoading) return;
                isLoading = true;
                await BuildLibraryAndCollection(false);
                await LoadData();
                isLoading = false;
            });
        }

        private void ListPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Run(async () => {
                if (isLoading) return;
                isLoading = true;
                headerLabel.Text = "Loading...";
                await LoadData();
                isLoading = false;
            });
        }

        private async Task BuildLibraryAndCollection(bool forceRefresh)
        {
            try
            {
                string allCardsInfoJson = forceRefresh ? null : Preferences.Get(allCardsInfoCache, "");
                await CardLibrary.BuildLibrary( allCardsInfoJson);
                Preferences.Set(allCardsInfoCache, CardLibrary.AllCardsInfoJson);
                string contents = Preferences.Get("contents", "");
                collection = new CardCollection();
                collection.InitializeWithCsv(CardLibrary.List, contents);
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
                        filteredList = filteredList.OrderBy(x => x.NumberAsInt).ToList();
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
                        filteredList = collection.List.Where(x => x.Total < 4).ToList();
                        filteredList = filteredList.OrderBy(x => x.Number).ToList();
                        break;
                    case 9:
                        filteredList = collection.List.Where(x => x.Total == 0).ToList();
                        break;
                }
                SetListData(SearchedList(filteredList, searchBar.Text));
            }
            catch (Exception ex)
            {

            }
        }

        private void SetListData(IEnumerable<Card> enumerable)
        {
            filteredAndSearchedList = enumerable.ToList();
            Device.BeginInvokeOnMainThread(() => { cardsList.ItemsSource = enumerable; headerLabel.Text = enumerable.Count() + " Cards"; });
        }

        async void Import_Clicked(object sender, EventArgs e)
        {
            grid.RaiseChild(importView);
            importView.IsVisible = true;
        }

        void Save_Clicked(object sender, EventArgs e)
        {
            var importContent = importEditor.Text;
            if (!string.IsNullOrWhiteSpace(importContent))
            {
                try
                {
                    collection = new CardCollection();
                    collection.InitializeWithJson(CardLibrary.List, importContent);
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

        void Cancel_Clicked(object sender, EventArgs e)
        {
            importEditor.Text = null;
            importView.IsVisible = false;
        }

        void cardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cardsList.SelectedItem == null)
            {
                return;
            }
            if (cardsList.SelectedItem is Card card)
            {
                int index = filteredAndSearchedList.IndexOf(card);
                if (index >= 0)
                {
                    Navigation.PushAsync(new CardDetailPage(filteredAndSearchedList, index));
                }
            }
            cardsList.SelectedItem = null;
        }

        void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetListData(SearchedList(filteredList, searchBar.Text));
        }

        private IEnumerable<Card> SearchedList(IEnumerable<Card> cardList, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return cardList;
            }
            else
            {
                return cardList.Where(x => MatchSearchPhrase(x, searchText));
            }
        }

        private bool MatchSearchPhrase(Card card, string searchPhrase)
        {
            List<string> substrings = new List<string>();

            string pattern = @"[^\s""']+|""([^""]*)""|'([^']*)'";
            Regex regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(searchPhrase);

            foreach (Match match in matches)
            {
                substrings.Add(match.Value.Trim('"'));
            }

            List<string> strChecks = new List<string>
            {
                card.Title,
                card.SubTitle,
                card.Body,
                Helpers.StringFromColor(card.Color),
                card.RarityStr,
                card.NumberDisplay,
                card.Artist,
                card.Inkable ? "%%inkable" : "%%uninkable",
                "%%cost:" + card.InkCost
            };
            if (card.Strength.HasValue)
            {
                strChecks.Add("%%str:" + card.Strength.Value);
            }
            if (card.Willpower.HasValue)
            {
                strChecks.Add("%%will:" + card.Willpower.Value);
            }
            if (card.LoreValue.HasValue)
            {
                strChecks.Add("%%lore:" + card.LoreValue.Value);
            }

            foreach (var subStr in substrings)
            {
                if (!strChecks.Where(x => x != null).Any(x => x.StartsWith("%%") ? x.Remove(0, 2).ToLower().Equals(subStr.ToLower()) : x.ToLower().Contains(subStr.ToLower())))
                {
                    return false;
                }
            }
            return true;
        }

        void Rebuild_Clicked(object sender, EventArgs e)
        {
            Task.Run(async () => {
                if (isLoading) return;
                SetListData(new List<Card>());
                isLoading = true;
                await BuildLibraryAndCollection(true);
                await LoadData();
                isLoading = false;
            });
        }
    }
}


