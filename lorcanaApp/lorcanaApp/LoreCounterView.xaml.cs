using System;
using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class LoreCounterView : ContentView
	{
        public int LoreValue { get; set; }
        public string PlayerDisplay { get => playerDisplay.Text; set => playerDisplay.Text = value; }

		public LoreCounterView()
		{
			InitializeComponent();
            btnAdd.FadeTo(.1);
            btnSub.FadeTo(.1);
            bgFrame.BackgroundColor = GetRandomColor();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (Height > 0)
            {
                bgImg.TranslationY = Height * 0.2;
            }
            if (Width > 0)
            {
                bgImg.TranslationX = Width * 0.2;
            }
        }

        void Button_Add_Clicked(System.Object sender, System.EventArgs e)
        {
            LoreValue++;
            UpdateLoreText();
            btnAdd.Opacity = 1;
            btnAdd.FadeTo(.1);
        }

        void Button_Sub_Clicked(System.Object sender, System.EventArgs e)
        {
            LoreValue--;
            UpdateLoreText();
            btnSub.Opacity = 1;
            btnSub.FadeTo(.1);
        }

        void UpdateLoreText()
        {
            loreDisplay.Text = LoreValue.ToString();
        }

        Color GetRandomColor()
        {
            Random rd = new Random();
            return Color.FromHsla(rd.NextDouble(), 0.8, 0.4);
        }
    }
}

