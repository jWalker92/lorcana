using Newtonsoft.Json.Linq;

namespace lorcana.Cards
{
	public static class Helpers
    {
        public static CardColor ColorFromString(string str)
        {
            if (str == "Amber") return CardColor.Amber;
            if (str == "Amethyst") return CardColor.Amehtyst;
            if (str == "Emerald") return CardColor.Emerald;
            if (str == "Ruby") return CardColor.Ruby;
            if (str == "Sapphire") return CardColor.Sapphire;
            if (str == "Steel") return CardColor.Steel;
            return CardColor.Amber;
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


        public static T? GetPropertyValue<T>(JObject item, string v)
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
    }
}

