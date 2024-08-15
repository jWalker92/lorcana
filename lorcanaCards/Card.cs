
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

    public enum CardType
    {
        Character,
        Action,
        Song,
        Item,
        Location,
        Unknown
    }

    public class Card
    {
        [SQLite.PrimaryKey]
        public string ID { get; set; }
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
        public int NormalsOnImport { get; set; }
        public int FoilsOnImport { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string ArtImage { get; set; }
        public CardColor Color { get; set; }
        public string RarityStr { get; set; }
        public string RarityIcon;
        public string TypeStr { get; set; }

        [SQLite.Ignore]
        public CardType CardType => Helpers.CardTypeFromString(TypeStr);
        [SQLite.Ignore]
        public Rarity Rarity => Helpers.RarityFromString(RarityStr);
        [SQLite.Ignore]
        public int Total { get => Normals + Foils; }
        [SQLite.Ignore]
        public string NumberDisplay => "#" + Number;
        [SQLite.Ignore]
        public int NumberAsInt { get { return int.TryParse(Number, out int numAsInt) ? numAsInt : 0; } }
        [SQLite.Ignore]
        public string Display => Title + (!string.IsNullOrEmpty(SubTitle) ? " (" + SubTitle + ")" : "");
        [SQLite.Ignore]
        public string SetCode => Helpers.NumberToSetcode(SetNumber);

        public string FoilMaskImage { get; set; }

        public static string GetImageLink(string number, string numberAddition, int setNumber, string countryCode = "de")
        {
            int.TryParse(number, out int intNumber);
            if (numberAddition == null)
            {
                numberAddition = "";
            }
            return $"https://images.dreamborn.ink/cards/{countryCode}/{setNumber:D3}-{intNumber:D3}{numberAddition}_1468x2048.webp";
        }

        public string ConstructKey()
        {
            return CreateKey(SetNumber, Number);
        }

        public static string CreateKey(int setNumber, string number)
        {
            return setNumber + ":" + number;
        }

        public static string CreateKey(int setNumber, int number)
        {
            return CreateKey(setNumber, number.ToString());
        }
    }
}

