using System;
using Newtonsoft.Json.Linq;

namespace lorcana.Cards
{
	public static class Helpers
    {
        public static CardColor ColorFromString(string str)
        {
            switch (str)
            {
                case "Amber":
                    return CardColor.Amber;
                case "Amethyst":
                    return CardColor.Amethyst;
                case "Emerald":
                    return CardColor.Emerald;
                case "Ruby":
                    return CardColor.Ruby;
                case "Sapphire":
                    return CardColor.Sapphire;
                case "Steel":
                    return CardColor.Steel;
                default:
                    return CardColor.Amber;
            }
        }

        public static string StringFromColor(CardColor color)
        {
            switch (color)
            {
                case CardColor.Amber:
                    return "Amber";
                case CardColor.Amethyst:
                    return "Amethyst";
                case CardColor.Emerald:
                    return "Emerald";
                case CardColor.Ruby:
                    return "Ruby";
                case CardColor.Sapphire:
                    return "Sapphire";
                case CardColor.Steel:
                    return "Steel";
                default:
                    return "?";
            }
        }

        public static Rarity RarityFromString(string str)
        {
            if (str == "Uncommon") return Rarity.Common;
            if (str == "Common") return Rarity.Uncommon;
            if (str == "Rare") return Rarity.Rare;
            if (str == "Super Rare") return Rarity.SuperRare;
            if (str == "Legendary") return Rarity.Legendary;
            if (str == "Enchanted") return Rarity.Enchanted;
            return Rarity.Common;
        }


        public static T GetPropertyValue<T>(JObject item, string v)
        {
            var token = FindProperty(item, v);
            if (token != null)
            {
                try
                {
                    return token.Value<T>();
                }
                catch (Exception ex)
                {

                }
            }
            return default;
        }

        public static JToken FindProperty(JToken token, string key)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    if (property.Name == key)
                    {
                        return property.Value;
                    }

                    var childResult = FindProperty(property.Value, key);
                    if (childResult != null)
                    {
                        return childResult;
                    }
                }
            }
            else if (token is JArray array)
            {
                foreach (var item in array)
                {
                    var childResult = FindProperty(item, key);
                    if (childResult != null)
                    {
                        return childResult;
                    }
                }
            }
            return null;
        }

        private const string filledCircle = "\u25CF";
        private const string filledRectangle = "\u25A0";
        private const string filledTriangle = "\u25B2";
        private const string filledSquare = "\u25C6";
        private const string filledPentagon = "\u2B23";
        private const string filledHexagon = "\u2B21";


        public static string StringFromRarity(Rarity rarity, bool unicode = false)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    return unicode ? filledCircle : "C";
                case Rarity.Uncommon:
                    return unicode ? filledRectangle : "U";
                case Rarity.Rare:
                    return unicode ? filledTriangle : "R";
                case Rarity.SuperRare:
                    return unicode ? filledSquare : "S";
                case Rarity.Legendary:
                    return unicode ? filledPentagon : "L";
                case Rarity.Enchanted:
                    return unicode ? filledHexagon : "E";
                default:
                    return "?";
            }
        }
    }
}

