using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class AmountAdjustView : ContentView
	{
        public static BindableProperty CardProperty = BindableProperty.Create(nameof(Card), typeof(AdjustableCard), typeof(AmountAdjustView), propertyChanged: OnCardChanged);

        private static void OnCardChanged(BindableObject bindable, object oldValue, object newValue)
        {
			if (bindable is AmountAdjustView view)
			{
                if (newValue is AdjustableCard card)
                {
                    view.BindingContext = card;
                    view.OnPropertyChanged(nameof(AdjustableCard.Number));
                    Device.BeginInvokeOnMainThread(() => {
                        view.normals.Text = card.Normals.ToString();
                        view.foils.Text = card.Foils.ToString();
                        view.FadeTo(1, 250); });
                    view.InputTransparent = false;
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => view.FadeTo(0, 250));
                    view.InputTransparent = true;
                }
            }
        }

        public AdjustableCard Card
        {
			get => (AdjustableCard)GetValue(CardProperty);
			set => SetValue(CardProperty, value);
		}

        public event EventHandler<AdjustableCard> OnAmountChanged;

		public AmountAdjustView()
		{
			InitializeComponent();
            Opacity = 0;
            InputTransparent = true;
		}

        void bgTapped(object sender, EventArgs e)
        {
            Card = null;
        }

        async void Normals_Add(object sender, EventArgs e)
        {
            await AdjustNormals(1);
        }

        async void Foils_Add(object sender, EventArgs e)
        {
            await AdjustFoils(1);
        }

        async void Normals_Sub(object sender, EventArgs e)
        {
            await AdjustNormals(-1);
        }

        async void Foils_Sub(object sender, EventArgs e)
        {
            await AdjustFoils(-1);
        }

        private async Task AdjustNormals(int adjustment)
        {
            Card.Normals += adjustment;
            if (Card.Normals < 0)
            {
                Card.Normals = 0;
            }
            await Database.Instance.AddOrReplaceCardAsync(Card);
            normals.Text = Card.Normals.ToString();
            OnAmountChanged?.Invoke(this, Card);
        }

        private async Task AdjustFoils(int adjustment)
        {
            Card.Foils += adjustment;
            if (Card.Foils < 0)
            {
                Card.Foils = 0;
            }
            await Database.Instance.AddOrReplaceCardAsync(Card);
            foils.Text = Card.Foils.ToString();
            OnAmountChanged?.Invoke(this, Card);
        }
    }
}

