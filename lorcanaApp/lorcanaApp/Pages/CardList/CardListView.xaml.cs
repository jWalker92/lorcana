using System.Windows.Input;
using lorcana.Cards;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class CardListView : ContentView
	{
        public static BindableProperty OnTapProperty = BindableProperty.Create(nameof(OnTap), typeof(ICommand), typeof(CardListView), propertyChanged: onTapChanged);
        private TapGestureRecognizer onTapRecognizer;
        private static void onTapChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CardListView view)
            {
                if (view.onTapRecognizer != null)
                {
                    view.GestureRecognizers.Remove(view.onTapRecognizer);
                }
                if (newValue is ICommand cmd)
                {
                    view.onTapRecognizer = new TapGestureRecognizer { Command = cmd };
                    view.GestureRecognizers.Add(view.onTapRecognizer);
                }
            }
        }

        public ICommand OnTap
        {
            get => (ICommand)GetValue(OnTapProperty);
            set => SetValue(OnTapProperty, value);
        }

        public static BindableProperty OnTapAmountsProperty = BindableProperty.Create(nameof(OnTapAmounts), typeof(ICommand), typeof(CardListView), propertyChanged: onTapAmountsChanged);
        private TapGestureRecognizer onAmountsRecognizer;
        private static void onTapAmountsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CardListView view)
            {
                if (view.onAmountsRecognizer != null)
                {
                    view.amountsTapArea.GestureRecognizers.Remove(view.onAmountsRecognizer);
                }
                if (newValue is ICommand cmd)
                {
                    view.onAmountsRecognizer = new TapGestureRecognizer { Command = cmd };
                    view.amountsTapArea.GestureRecognizers.Add(view.onAmountsRecognizer);
                }
            }
        }

        public ICommand OnTapAmounts
        {
            get => (ICommand)GetValue(OnTapAmountsProperty);
            set => SetValue(OnTapAmountsProperty, value);
        }

        public CardListView ()
		{
			InitializeComponent();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            cardImage.Source = null;
            if (BindingContext is Card c)
            {
                cardImage.Source = c.SmallImage;
            }
            base.OnBindingContextChanged();
        }
    }
}

