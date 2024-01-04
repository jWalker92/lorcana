using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class LoreCounterPage : ContentPage
	{	
		public LoreCounterPage ()
		{
			InitializeComponent ();
            var loreCounter1 = new LoreCounterView();
            loreCounter1.SetId("1");
            loreCounter1.PlayerDisplay = "Player 1";
            var loreCounter2 = new LoreCounterView();
            loreCounter2.SetId("2");
            loreCounter2.PlayerDisplay = "Player 2";
            loreCounter1.Rotation = 180;
            counterGrid.Children.Add(loreCounter1, 0, 0);
            counterGrid.Children.Add(loreCounter2, 0, 1);
        }
	}
}

