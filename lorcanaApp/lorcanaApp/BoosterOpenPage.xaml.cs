using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using lorcana.Cards;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class BoosterOpenPage : ContentPage
	{
        private ObservableCollection<AdjustableCard> cards;

        public BoosterOpenPage ()
		{
			InitializeComponent ();
            cards = new ObservableCollection<AdjustableCard>();
			cardsList.ItemsSource = cards;
		}
        
        async void Normals_Add(object sender, EventArgs e)
        {
            await AddCard((c) => c.Normals++, (c) => c.Normals--);
        }

        async void Foils_Add(object sender, EventArgs e)
        {
            await AddCard((c) => { c.Foils++; c.FoilBackground = true; }, (c) => c.Foils--);
        }

        async Task AddCard(Action<AdjustableCard> onAdd, Action<AdjustableCard> onRemove)
        {
            try
            {
                int setNum = int.Parse(setEntry.Text);
                int cardNum = int.Parse(numEntry.Text);
                var card = await Database.Instance.GetCardBySetAndNumberAsync(setNum, cardNum);
                if (card == null)
                {
                    await DisplayAlert("Error", "Card not found in Database", "Ok");
                    return;
                }
                var addedCard = AdjustableCard.FromCard(card);
                addedCard.ShowAmounts = false;
                onAdd?.Invoke(addedCard);
                await Database.Instance.AddOrReplaceCardAsync(addedCard);
                addedCard.OnTap = new Command(async () => {
                    onRemove?.Invoke(addedCard);
                    await Database.Instance.AddOrReplaceCardAsync(addedCard);
                    cards.Remove(addedCard);
                });
                cards.Add(addedCard);
                numEntry.Text = null;
                numEntry.Focus();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

