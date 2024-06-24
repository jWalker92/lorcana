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
                case "Bernstein":
                    return CardColor.Amber;
                case "Amethyst":
                    return CardColor.Amethyst;
                case "Emerald":
                case "Smaragd":
                    return CardColor.Emerald;
                case "Ruby":
                case "Rubin":
                    return CardColor.Ruby;
                case "Sapphire":
                case "Saphir":
                    return CardColor.Sapphire;
                case "Steel":
                case "Stahl":
                    return CardColor.Steel;
                default:
                    return CardColor.Amber;
            }
        }

        public static string HexStringFromColor(CardColor color)
        {
            switch (color)
            {
                case CardColor.Amber:
                    return "#F4B400";
                case CardColor.Amethyst:
                    return "#81377B";
                case CardColor.Emerald:
                    return "#298A34";
                case CardColor.Ruby:
                    return "#D30A2E";
                case CardColor.Sapphire:
                    return "#0089C3";
                case CardColor.Steel:
                    return "#9FA9B3";
                default:
                    return "#A0A0A0";
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

        public static string NumberToSetcode(int number)
        {
            switch (number)
            {
                case 1:
                    return "TFC";
                case 2:
                    return "ROTF";
                case 3:
                    return "ITI";
                default:
                    return string.Empty;
            }
        }

        public static Rarity RarityFromString(string str)
        {
            switch (str)
            {
                case "Common":
                case "Häufig":
                    return Rarity.Common;
                case "Uncommon":
                case "Ungewöhnlich":
                    return Rarity.Uncommon;
                case "Rare":
                    return Rarity.Rare;
                case "Super Rare":
                case "Super Selten":
                    return Rarity.SuperRare;
                case "Legendary":
                case "Legendär":
                    return Rarity.Legendary;
                case "Enchanted":
                case "Verzaubert":
                    return Rarity.Enchanted;
                default:
                    return Rarity.Unknown;
            }
        }

        public static CardType CardTypeFromString(string str)
        {
            if (str == "Character") return CardType.Character;
            if (str == "Action") return CardType.Action;
            if (str == "Action - Song") return CardType.Song;
            if (str == "Item") return CardType.Item;
            if (str == "Location") return CardType.Location;
            return CardType.Unknown;
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

        public static string ReplaceLastOccurrence(string input, string searchString, string replaceString)
        {
            int lastIndexOfSearchString = input.LastIndexOf(searchString);

            if (lastIndexOfSearchString >= 0)
            {
                input = input.Remove(lastIndexOfSearchString, searchString.Length)
                                         .Insert(lastIndexOfSearchString, replaceString);
            }

            return input;
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

