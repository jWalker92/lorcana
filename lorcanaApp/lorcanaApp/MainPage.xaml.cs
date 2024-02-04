using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using lorcana.Cards;
using Newtonsoft.Json;
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
        private List<AdjustableCard> filteredList;
        private List<AdjustableCard> filteredAndSearchedList;
        private bool isLoading = false;

        public MainPage()
        {
            InitializeComponent();
            headerLabel.Text = "Loading...";
            listPicker.ItemsSource = pickerItems;
            listPicker.SelectedIndex = 0;
            listPicker.SelectedIndexChanged += ListPicker_SelectedIndexChanged;
            adjustView.OnAmountChanged += AdjustView_OnAmountChanged;
            Task.Run(async () => {
                if (isLoading) return;
                isLoading = true;
                await BuildLibraryAndCollection(false);
                await LoadData();
                isLoading = false;
            });
        }

        private void AdjustView_OnAmountChanged(object sender, AdjustableCard e)
        {
            int index = filteredAndSearchedList.IndexOf(e);
            if (index >= 0)
            {
                filteredAndSearchedList[index].OnPropertyChanged(nameof(AdjustableCard.Normals));
                filteredAndSearchedList[index].OnPropertyChanged(nameof(AdjustableCard.Foils));
            }
        }

        private void ListPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task.Run(async () => {
                if (isLoading) return;
                Device.BeginInvokeOnMainThread(() => headerLabel.Text = "Loading...");
                isLoading = true;
                await LoadData();
                isLoading = false;
            });
        }

        private async Task BuildLibraryAndCollection(bool forceRefresh)
        {
            try
            {
                string allCardsInfoJson = forceRefresh ? null : Preferences.Get(allCardsInfoCache, "");
                await CardLibrary.BuildLibrary(allCardsInfoJson, App.CountryCode);
                Preferences.Set(allCardsInfoCache, CardLibrary.AllCardsInfoJson);
                string contents = Preferences.Get("contents", "");
                collection = new CardCollection();
                collection.InitializeFromList(CardLibrary.List, await Database.Instance.GetCardsAsync(), Database.Instance.AddOrReplaceCardAsync);

            }
            catch (Exception ex)
            {

            }
        }

        private async Task LoadData()
        {
            try
            {
                filteredList = new List<AdjustableCard>();
                var adjustableCollectionList = collection.List.Select(x => JsonConvert.DeserializeObject<AdjustableCard>(JsonConvert.SerializeObject(x)));
                switch (listPicker.SelectedIndex)
                {
                    case 0:
                        filteredList = adjustableCollectionList.ToList();
                        break;
                    case 1:
                        filteredList = adjustableCollectionList.Where(x => x.Total >= 1).ToList();
                        break;
                    case 2:
                        filteredList = adjustableCollectionList.Where(x => x.Total == 1).ToList();
                        break;
                    case 3:
                        filteredList = adjustableCollectionList.Where(x => x.Total == 2).ToList();
                        break;
                    case 4:
                        filteredList = adjustableCollectionList.Where(x => x.Total == 3).ToList();
                        break;
                    case 5:
                        filteredList = adjustableCollectionList.Where(x => x.Total >= 4).ToList();
                        break;
                    case 6:
                        filteredList = adjustableCollectionList.Where(x => x.Total >= 5).ToList();
                        break;
                    case 7:
                        filteredList = adjustableCollectionList.Where(x => x.Total > 8).ToList();
                        break;
                    case 8:
                        filteredList = adjustableCollectionList.Where(x => x.Total < 4).ToList();
                        break;
                    case 9:
                        filteredList = adjustableCollectionList.Where(x => x.Total == 0).ToList();
                        break;
                }
                SetListData(SearchedList(filteredList.OrderBy(x => x.NumberAsInt).ThenBy(x => x.SetNumber), searchBar.Text));
            }
            catch (Exception ex)
            {

            }
        }

        void SetListData(IEnumerable<AdjustableCard> enumerable)
        {
            try
            {
                foreach (var card in enumerable)
                {
                    card.OnTap = new Command(() => {
                        int index = filteredAndSearchedList.IndexOf(card);
                        if (index >= 0)
                        {
                            Navigation.PushAsync(new CardDetailPage(filteredAndSearchedList, index));
                        }
                    });
                    card.OnTapAmounts = new Command(() =>
                    {
                        adjustView.Card = card;
                    });
                }
                filteredAndSearchedList = enumerable.ToList();
                Device.BeginInvokeOnMainThread(() => { cardsList.ItemsSource = enumerable; headerLabel.Text = enumerable.Count() + " Cards"; });
            }
            catch (Exception ex)
            {

            }
        }

        void Import_Clicked(object sender, EventArgs e)
        {
            grid.RaiseChild(importView);
            importView.IsVisible = true;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            var importContent = importEditor.Text;
            if (!string.IsNullOrWhiteSpace(importContent))
            {
                try
                {
                    var importedCollection = new CardCollection();
                    importedCollection.InitializeWithCsv(CardLibrary.List, importContent);
                    foreach (var card in importedCollection.List)
                    {
                        await Database.Instance.AddOrReplaceCardAsync(card);
                    }
                    collection = new CardCollection();
                    collection.InitializeFromList(CardLibrary.List, await Database.Instance.GetCardsAsync(), Database.Instance.AddOrReplaceCardAsync);
                    Preferences.Set("contents", importContent);
                    importEditor.Text = null;
                    importView.IsVisible = false;
                    await LoadData();
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

        void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetListData(SearchedList(filteredList, searchBar.Text));
        }

        private IEnumerable<AdjustableCard> SearchedList(IEnumerable<AdjustableCard> cardList, string searchText)
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
                card.SetCode,
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
                SetListData(new List<AdjustableCard>());
                isLoading = true;
                Device.BeginInvokeOnMainThread(() => headerLabel.Text = "Loading...");
                await BuildLibraryAndCollection(true);
                await LoadData();
                isLoading = false;
            });
        }

        void searchBar_FocusChanged(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            SetSearchBarWidth();
        }

        void SetSearchBarWidth()
        {
            if (searchBar.IsFocused)
            {
                searchBar.Animate("widthAnim", new Animation((d) => searchBar.WidthRequest = d, searchBar.WidthRequest, filterGrid.Width), 16, 160, Easing.CubicInOut);
                pickerFrame.FadeTo(0, 100);
            }
            else
            {
                searchBar.Animate("widthAnim", new Animation((d) => searchBar.WidthRequest = d, searchBar.WidthRequest, filterGrid.Width / 2), 16, 160, Easing.CubicInOut);
                pickerFrame.FadeTo(1, 100);
            }
        }

        void filterGrid_SizeChanged(System.Object sender, System.EventArgs e)
        {
            SetSearchBarWidth();
        }
    }

    public class AdjustableCard : Card, INotifyPropertyChanged
    {
        public ICommand OnTap { get; set; }
        public ICommand OnTapAmounts { get; set; }

        public AdjustableCard() : base()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


