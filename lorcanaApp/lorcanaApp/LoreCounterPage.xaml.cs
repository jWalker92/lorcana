using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class LoreCounterPage : ContentPage
	{
        bool singleMode = Preferences.Get(nameof(singleMode), false);
        private Label mirrorLabel;
        private LoreCounterView loreCounter1;
        private LoreCounterView loreCounter2;

        public LoreCounterPage ()
		{
			InitializeComponent ();
            loreCounter1 = new LoreCounterView();
            loreCounter1.SetId("1");
            loreCounter1.PlayerDisplay = "Player 1";
            loreCounter2 = new LoreCounterView();
            loreCounter2.SetId("2");
            loreCounter2.PlayerDisplay = "Player 2";
            loreCounter2.Rotation = 180;
            mirrorLabel = new Label();
            mirrorLabel.Rotation = 180;
            mirrorLabel.BindingContext = loreCounter1;
            mirrorLabel.HorizontalOptions = LayoutOptions.Center;
            mirrorLabel.VerticalOptions = LayoutOptions.Center;
            mirrorLabel.FontSize = 128;
            mirrorLabel.TextColor = Color.WhiteSmoke;
            mirrorLabel.FontAttributes = FontAttributes.Bold;
            mirrorLabel.SetBinding(Label.TextProperty, nameof(LoreCounterView.LoreValue));
            SetDisplayMode();
        }

        async void Menu_Tapped(object sender, EventArgs e)
        {
            string counterSwitch = singleMode ? "2 Counters" : "1 Counter";
            var selection = await DisplayActionSheet(null, "Cancel", "Reset", counterSwitch);
            if (selection == "Reset")
            {
                loreCounter1.SetLoreValue(0);
                loreCounter2.SetLoreValue(0);
            }
            else if (selection == counterSwitch)
            {
                singleMode = !singleMode;
                Preferences.Set(nameof(singleMode), singleMode);
                SetDisplayMode();
            }
        }

        private void SetDisplayMode()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                counterGrid.Children.Clear();
                counterGrid.Children.Add(loreCounter1, 0, 1);
                if (singleMode)
                {
                    counterGrid.Children.Add(mirrorLabel, 0, 0);
                }
                else
                {
                    counterGrid.Children.Add(loreCounter2, 0, 0);
                }
            });
        }
    }
}

