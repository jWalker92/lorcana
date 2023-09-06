﻿
namespace lorcana.Cards
{
    public enum CardColor
    {
        Amber,
        Amehtyst,
        Emerald,
        Ruby,
        Sapphire,
        Steel
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        SuperRare,
        Legendary,
        Enchanted
    }

    public class Card
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Number { get; set; }
        public int Normals { get; set; }
        public int Foils { get; set; }
        public int Total { get => Normals + Foils; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string ArtImage { get; set; }
        public CardColor Color { get; set; }
        public string RarityStr { get; set; }
        public Rarity Rarity => Helpers.RarityFromString(RarityStr);

        private string Display => "#" + Number + " " + Title + (!string.IsNullOrEmpty(SubTitle) ? " (" + SubTitle + ")" : "");
        public void WriteDisplay()
        {
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = Helpers.ConsoleColorFromColor(Color);
            Console.Write(Display);
            Console.ForegroundColor = preColor;
        }
    }
}

