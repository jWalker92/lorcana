using System;
using System.Collections.Generic;
using FFImageLoading.Forms;
using lorcana.Cards;
using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class CardListView : ContentView
	{
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
                cardImage.Source = c.Image;
            }
            base.OnBindingContextChanged();
        }
    }
}

