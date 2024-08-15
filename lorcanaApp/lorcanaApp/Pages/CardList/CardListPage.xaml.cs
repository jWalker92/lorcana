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
    public partial class CardListPage : ContentPage
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
        List<string> sortItems = new List<string> {
            "Number",
            "Set",
            "Amount"
        };
        private CardCollection collection;
        internal static CardLibrary CardLibrary;
        private List<AdjustableCard> filteredList;
        private List<AdjustableCard> filteredAndSearchedList;
        private bool isLoading = false;
        private bool _filterAmber;
        private bool _filterAmethyst;
        private bool _filterEmerald;
        private bool _filterRuby;
        private bool _filterSapphire;
        private bool _filterSteel;
        private bool _filterRare;
        private bool _filterUncommon;
        private bool _filterCommon;
        private bool _filterSuperRare;
        private bool _filterLegendary;

        public CardListPage()
        {
            isLoading = true;
            InitializeComponent();
            CardLibrary = new CardLibrary();
            Database.Instance.CollectionChanged += Instance_CollectionChanged;
            headerLabel.Text = "Loading...";
            listPicker.ItemsSource = pickerItems;
            listPicker.SelectedIndex = 0;
            listPicker.SelectedIndexChanged += ListPicker_SelectedIndexChanged;
            sortPicker.ItemsSource = sortItems;
            sortPicker.SelectedIndex = 0;
            sortPicker.SelectedIndexChanged += SortPicker_SelectedIndexChanged;
            adjustView.OnAmountChanged += AdjustView_OnAmountChanged;
            Task.Run(async () => {
                await BuildLibraryAndCollection(false);
                await LoadData();
                isLoading = false;
            });
        }

        private void Instance_CollectionChanged(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    collection = new CardCollection();
                    collection.InitializeFromList(CardLibrary.List, await Database.Instance.GetCardsAsync(), Database.Instance.AddOrReplaceCardAsync);
                    await LoadData();
                }
                catch (Exception ex)
                {

                }
            });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
            LoadData();
        }

        private void SortPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetListData(SearchedList(filteredList, searchBar.Text));
        }

        private void ReloadData()
        {
            Task.Run(async () => {
                if (isLoading) return;
                collection = new CardCollection();
                collection.InitializeFromList(CardLibrary.List, await Database.Instance.GetCardsAsync(), Database.Instance.AddOrReplaceCardAsync);
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
                filteredList = collection.List.Select(AdjustableCard.FromCard).ToList();

                if (_filterAmber || _filterAmethyst || _filterEmerald || _filterRuby || _filterSapphire || _filterSteel)
                {
                    if (!_filterAmber)
                    {
                        filteredList.RemoveAll(x => x.Color == CardColor.Amber);
                    }
                    if (!_filterAmethyst)
                    {
                        filteredList.RemoveAll(x => x.Color == CardColor.Amethyst);
                    }
                    if (!_filterEmerald)
                    {
                        filteredList.RemoveAll(x => x.Color == CardColor.Emerald);
                    }
                    if (!_filterRuby)
                    {
                        filteredList.RemoveAll(x => x.Color == CardColor.Ruby);
                    }
                    if (!_filterSapphire)
                    {
                        filteredList.RemoveAll(x => x.Color == CardColor.Sapphire);
                    }
                    if (!_filterSteel)
                    {
                        filteredList.RemoveAll(x => x.Color == CardColor.Steel);
                    }
                }

                if (_filterCommon || _filterUncommon || _filterRare || _filterSuperRare || _filterLegendary)
                {
                    if (!_filterCommon)
                    {
                        filteredList.RemoveAll(x => x.Rarity == Rarity.Common);
                    }
                    if (!_filterUncommon)
                    {
                        filteredList.RemoveAll(x => x.Rarity == Rarity.Uncommon);
                    }
                    if (!_filterRare)
                    {
                        filteredList.RemoveAll(x => x.Rarity == Rarity.Rare);
                    }
                    if (!_filterSuperRare)
                    {
                        filteredList.RemoveAll(x => x.Rarity == Rarity.SuperRare);
                    }
                    if (!_filterLegendary)
                    {
                        filteredList.RemoveAll(x => x.Rarity == Rarity.Legendary);
                    }
                }
                switch (listPicker.SelectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        filteredList.RemoveAll(x => x.Total == 0);
                        break;
                    case 2:
                        filteredList.RemoveAll(x => x.Total != 1);
                        break;
                    case 3:
                        filteredList.RemoveAll(x => x.Total != 2);
                        break;
                    case 4:
                        filteredList.RemoveAll(x => x.Total != 3);
                        break;
                    case 5:
                        filteredList.RemoveAll(x => x.Total < 4);
                        break;
                    case 6:
                        filteredList.RemoveAll(x => x.Total < 5);
                        break;
                    case 7:
                        filteredList.RemoveAll(x => x.Total < 9);
                        break;
                    case 8:
                        filteredList.RemoveAll(x => x.Total >= 4);
                        break;
                    case 9:
                        filteredList.RemoveAll(x => x.Total != 0);
                        break;
                }
                SetListData(SearchedList(filteredList, searchBar.Text));
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
                Device.BeginInvokeOnMainThread(() => {
                    filteredAndSearchedList = OrderList(enumerable).ToList();
                    cardsList.ItemsSource = filteredAndSearchedList;
                    headerLabel.Text = enumerable.Count() + " Cards";
                });
            }
            catch (Exception ex)
            {

            }
        }

        private IEnumerable<AdjustableCard> OrderList(IEnumerable<AdjustableCard> enumerable)
        {
            switch (sortPicker.SelectedIndex)
            {
                default:
                case 0:
                    return enumerable.OrderBy(x => x.NumberAsInt).ThenBy(x => x.SetNumber);
                case 1:
                    return enumerable.OrderBy(x => x.SetNumber).ThenBy(x => x.NumberAsInt);
                case 2:
                    return enumerable.OrderByDescending(x => x.Total);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (adjustView.Card != null)
            {
                adjustView.Card = null;
                return true;
            }
            return base.OnBackButtonPressed();
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
                card.TypeStr,
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

        void FilterButton_Clicked(object sender, EventArgs e)
        {
            filterView.IsVisible = !filterView.IsVisible;
            if (sender is SvgImageButton svgBtn)
            {
                svgBtn.Source = ImageResourceExtension.GetImageResource(filterView.IsVisible ? "lorcanaApp.Resources.filter_off.svg" : "lorcanaApp.Resources.filter.svg");
            }
        }

        private void SetFilter(ref bool filter, ContentView filterView)
        {
            filter = !filter;
            filterView.FadeTo(filter ? 1 : 0.5);
            LoadData();
        }

        void Amber_Clicked(object sender, EventArgs e)
        {
            SetFilter(ref _filterAmber, amberFilter);
        }

        void Amethyst_Clicked(object sender, EventArgs e)
        {
            SetFilter(ref _filterAmethyst, amethystFilter);
        }

        void Emerald_Clicked(object sender, EventArgs e)
        {
            SetFilter(ref _filterEmerald, emeraldFilter);
        }

        void Ruby_Clicked(object sender, EventArgs e)
        {
            SetFilter(ref _filterRuby, rubyFilter);
        }

        void Sapphire_Clicked(object sender, EventArgs e)
        {
            SetFilter(ref _filterSapphire, sapphireFilter);
        }

        void Steel_Clicked(object sender, EventArgs e)
        {
            SetFilter(ref _filterSteel, steelFilter);
        }

        void Common_Clicked(System.Object sender, System.EventArgs e)
        {
            SetFilter(ref _filterCommon, commonFilter);
        }

        void Uncommon_Clicked(System.Object sender, System.EventArgs e)
        {
            SetFilter(ref _filterUncommon, uncommonFilter);
        }

        void Rare_Clicked(System.Object sender, System.EventArgs e)
        {
            SetFilter(ref _filterRare, rareFilter);
        }

        void SuperRare_Clicked(System.Object sender, System.EventArgs e)
        {
            SetFilter(ref _filterSuperRare, superRareFilter);
        }

        void Legendary_Clicked(System.Object sender, System.EventArgs e)
        {
            SetFilter(ref _filterLegendary, legendaryFilter);
        }
    }
}


