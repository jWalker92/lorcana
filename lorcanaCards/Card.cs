
using System;

namespace lorcana.Cards
{
    public enum CardColor
    {
        Amber,
        Amethyst,
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
        Enchanted,
        Unknown
    }

    public class Card
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Body { get; set; }
        public int SetNumber { get; set; }
        public string Number { get; set; }
        public string Artist { get; set; }
        public int? Strength { get; set; }
        public int? Willpower { get; set; }
        public int? LoreValue { get; set; }
        public int InkCost { get; set; }
        public bool Inkable { get; set; }
        public string FlavorText { get; set; }
        public int Normals { get; set; }
        public int Foils { get; set; }
        public int Total { get => Normals + Foils; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string ArtImage { get; set; }
        public CardColor Color { get; set; }
        public string RarityStr { get; set; }
        public Rarity Rarity => Helpers.RarityFromString(RarityStr);
        public string RarityIcon;

        public string NumberDisplay => "#" + Number;
        public int NumberAsInt { get { return int.TryParse(Number, out int numAsInt) ? numAsInt : 0; } }
        public string Display => Title + (!string.IsNullOrEmpty(SubTitle) ? " (" + SubTitle + ")" : "");
        public string SetCode => Helpers.NumberToSetcode(SetNumber);

        public static string GetImageLink(int number, int setNumber)
        {
            return $"https://images.dreamborn.ink/cards/en/{setNumber:D3}-{number:D3}_1468x2048.webp";
        }
    }
}

