using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class LoreCounterPage : ContentPage
	{
        int counterCount = Preferences.Get(nameof(counterCount), 2);
        private Label mirrorLabel;
        private LoreCounterView loreCounter1;
        private LoreCounterView loreCounter2;
        private LoreCounterView loreCounter3;
        private LoreCounterView loreCounter4;

        public LoreCounterPage ()
		{
			InitializeComponent ();
            loreCounter1 = new LoreCounterView();
            loreCounter1.SetId("1");
            loreCounter1.PlayerDisplay = "Player 1";
            loreCounter2 = new LoreCounterView();
            loreCounter2.SetId("2");
            loreCounter2.PlayerDisplay = "Player 2";
            loreCounter2.ContentRotation = 180;
            loreCounter3 = new LoreCounterView();
            loreCounter3.SetId("3");
            loreCounter3.PlayerDisplay = "Player 3";
            loreCounter3.ContentRotation = 0;
            loreCounter4 = new LoreCounterView();
            loreCounter4.SetId("4");
            loreCounter4.PlayerDisplay = "Player 4";
            loreCounter4.ContentRotation = 180;
            mirrorLabel = new Label
            {
                Rotation = 180,
                BindingContext = loreCounter1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 128,
                TextColor = Color.WhiteSmoke,
                FontAttributes = FontAttributes.Bold
            };
            mirrorLabel.SetBinding(Label.TextProperty, nameof(LoreCounterView.LoreValue));
            SetDisplayMode();
        }

        async void Menu_Tapped(object sender, EventArgs e)
        {
            var selection = await DisplayActionSheet(null, "Cancel", "Reset", "Player Count", "Randomize Colors");
            if (selection == "Reset")
            {
                loreCounter1.SetLoreValue(0);
                loreCounter2.SetLoreValue(0);
                loreCounter3.SetLoreValue(0);
                loreCounter4.SetLoreValue(0);
            }
            else if (selection == "Player Count")
            {
                var playerCountStr = await DisplayActionSheet("Player Count", "Cancel", null, "1", "2", "3", "4");
                SetPlayerCount(int.Parse(playerCountStr));
            }
            else if (selection == "Randomize Colors")
            {
                loreCounter1.SetBgColor();
                loreCounter2.SetBgColor();
                loreCounter3.SetBgColor();
                loreCounter4.SetBgColor();
            }
        }

        private void SetPlayerCount(int selectedCount)
        {
            counterCount = selectedCount;
            Preferences.Set(nameof(counterCount), counterCount);
            SetDisplayMode();
        }

        private void SetDisplayMode()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                counterGrid.Children.Clear();
                if (counterCount <= 2)
                {
                    counterGrid.ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star } };
                }
                else
                {
                    counterGrid.ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } };
                }
                counterGrid.Children.Add(loreCounter1, 0, 1);
                if (counterCount == 1)
                {
                    counterGrid.Children.Add(mirrorLabel, 0, 0);
                    return;
                }
                if (counterCount >= 2)
                {
                    counterGrid.Children.Add(loreCounter2, 0, 0);
                }
                if (counterCount >= 3)
                {
                    counterGrid.Children.Add(loreCounter3, 1, 1);
                }
                if (counterCount >= 4)
                {
                    counterGrid.Children.Add(loreCounter4, 1, 0);
                }
            });
        }
    }
}

