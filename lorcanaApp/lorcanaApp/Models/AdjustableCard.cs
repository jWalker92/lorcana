using lorcana.Cards;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace lorcanaApp
{

    public class AdjustableCard : Card, INotifyPropertyChanged
    {
        private bool showAmounts = true;
        private bool foilBackground;

        public ICommand OnTap { get; set; }
        public ICommand OnTapAmounts { get; set; }

        public bool ShowAmounts { get => showAmounts; set { showAmounts = value; OnPropertyChanged(); } }
        public bool FoilBackground { get => foilBackground; set { foilBackground = value; OnPropertyChanged(); } }

        public AdjustableCard() : base()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static AdjustableCard FromCard(Card c)
        {
            return JsonConvert.DeserializeObject<AdjustableCard>(JsonConvert.SerializeObject(c));
        }
    }
}

