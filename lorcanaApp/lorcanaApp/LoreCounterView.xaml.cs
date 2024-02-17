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

        public double ContentRotation { get => grid.Rotation; set => grid.Rotation = value; }

		public LoreCounterView()
		{
			InitializeComponent();
            btnAdd.FadeTo(.1);
            btnSub.FadeTo(.1);
            SetBgColor();
        }

        public void SetBgColor(Color? c = null)
        {
            if (!c.HasValue)
            {
                c = GetRandomColor();
            }
            bgFrame.BackgroundColor = c.Value;
            Preferences.Set(_id + "color", c.Value.ToHex());
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
            SetLoreValue(Preferences.Get(_id + "lore", 0));
            SetBgColor(Color.FromHex(Preferences.Get(_id + "color", GetRandomColor().ToHex())));
        }

        internal void SetLoreValue(int value)
        {
            int preLoreValue = LoreValue;
            LoreValue = value;
            Preferences.Set(_id + "lore", LoreValue);
            UpdateLoreText();
            if (LoreValue >= 20)
            {
                if (preLoreValue == 19 && LoreValue == 20)
                {
                    shimmerBg.Animate("shimmer", new Animation((d) => {
                        LinearGradientBrush calculatedBg = new LinearGradientBrush();
                        calculatedBg.StartPoint = new Point(0, 0);
                        calculatedBg.EndPoint = new Point(0.3, 1);
                        calculatedBg.GradientStops = new GradientStopCollection {
                        new GradientStop(Color.Transparent, 0),
                        new GradientStop(Color.Transparent, (float)d - 0.26f),
                        new GradientStop(Color.White, (float)d),
                        new GradientStop(Color.Transparent, (float)d + 0.1f),
                        new GradientStop(Color.Transparent, 1)
                    };
                        shimmerBg.Background = calculatedBg;
                        var opacity = (d < 0.5 ? d * 2 : 2 - d * 2) * 0.6;
                        shimmerBg.Opacity = opacity;
                    }, 0, 1, Easing.SinOut), 16, 700);
                }
                bgImg.ScaleTo(3, 1400, Easing.SinInOut);
            }
            else
            {
                shimmerBg.AbortAnimation("shimmer");
                shimmerBg.FadeTo(0);
                bgImg.ScaleTo(1.6, 500, Easing.SinInOut);
            }
        }
    }
}

