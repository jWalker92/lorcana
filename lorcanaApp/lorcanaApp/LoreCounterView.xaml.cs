using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class LoreCounterView : ContentView
	{
        private string _id;
        private int loreValue;

        public int LoreValue { get => loreValue; set { loreValue = value; OnPropertyChanged(); } }
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

        void Button_Add_Clicked(object sender, EventArgs e)
        {
            SetLoreValue(LoreValue + 1);
            btnAdd.Opacity = 1;
            btnAdd.FadeTo(.1);
        }

        void Button_Sub_Clicked(object sender, EventArgs e)
        {
            if (LoreValue == 0)
            {
                return;
            }
            SetLoreValue(LoreValue - 1);
            btnSub.Opacity = 1;
            btnSub.FadeTo(.1);
        }

        void UpdateLoreText()
        {
            loreDisplay.Text = LoreValue.ToString();
        }

        Color GetRandomColor()
        {
            var rd = new Random();
            return Color.FromHsla(rd.NextDouble(), 0.8, 0.4);
        }

        internal void SetId(string id)
        {
            _id = id;
            LoreValue = Preferences.Get(_id, 0);
            UpdateLoreText();
        }

        internal void SetLoreValue(int value)
        {
            LoreValue = value;
            Preferences.Set(_id, LoreValue);
            UpdateLoreText();
        }
    }
}

