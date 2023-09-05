using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lorcana
{

    internal class Program
    {
        class intVal
        {
            public int IntegerValue { get; set; }

            public static implicit operator int(intVal val) => val.IntegerValue;

            public override string ToString()
            {
                return IntegerValue.ToString();
            }
        }

        enum Color
        {
            Amber,
            Amehtyst,
            Emerald,
            Ruby,
            Sapphire,
            Steel
        }

        enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            SuperRare,
            Legendary,
            Enchanted
        }

        class Card
        {
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public int Number { get; set; }
            public int Normals { get; set; }
            public int Foils { get; set; }
            public int Total { get => Normals + Foils; }
            public Color Color { get; set; }
            public string RarityStr { get; set; }
            public Rarity Rarity => RarityFromString(RarityStr);

            private string Display => "#" + Number + " " + Title + (!string.IsNullOrEmpty(SubTitle) ? " (" + SubTitle + ")" : "");
            public void WriteDisplay()
            {
                var preColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColorFromColor(Color);
                Console.Write(Display);
                Console.ForegroundColor = preColor;
            }
        }

        static async Task Main(string[] args)
        {
            var json = File.ReadAllText("update.json");

            var dict = JsonConvert.DeserializeObject<Dictionary<string, intVal>>(json);

            var cardsList = new List<Card>();
            int i = 0;

            string allCardsJson = null;
            if (File.Exists("allCards.json"))
            {
                allCardsJson = File.ReadAllText("allCards.json");
            }
            else
            {
                allCardsJson = await GetAllCards();
                File.WriteAllText("allCards.json", allCardsJson);
            }
            var allNames = JsonConvert.DeserializeObject<List<string>>(allCardsJson);

            string allCardsInfoJson = null;
            if (File.Exists("allCardsInfo.json"))
            {
                allCardsInfoJson = File.ReadAllText("allCardsInfo.json");
            }
            else
            {
                allCardsInfoJson = await GetAllCardsInfos(string.Join(';', allNames));
                File.WriteAllText("allCardsInfo.json", allCardsInfoJson);
            }

            var allInfo = JsonConvert.DeserializeObject<List<JObject>>(allCardsInfoJson);

            List<Card> allCardsInfo = new List<Card>();
            foreach (var item in allInfo)
            {
                int number = GetPropertyValue<int>(item, "card-number");
                string name = GetPropertyValue<string>(item, "name");
                string rarityStr = GetPropertyValue<string>(item, "rarity");
                Card infoCard = new Card
                {
                    Number = number,
                    Title = name,
                    SubTitle = GetPropertyValue<string>(item, "subtitle"),
                    Color = ColorFromString(GetPropertyValue<string>(item, "color")),
                    RarityStr = rarityStr
                };
                allCardsInfo.Add(infoCard);
            }

            allCardsInfo = allCardsInfo.OrderBy(x => x.Number).ToList();

            foreach (var item in dict.OrderBy(x => x.Key))
            {

                int chapter = 0;
                int number = 0;
                bool foil = false;

                var keyItems = item.Key.Split('-', ':');
                if (keyItems.Length >= 2)
                {
                    chapter = int.Parse(keyItems[0]);
                    number = int.Parse(keyItems[1]);
                    if (keyItems.Length >= 3 && keyItems[2] == "foil")
                    {
                        foil = true;
                    }
                }
                Card card = cardsList.FirstOrDefault(x => x.Number == number);
                if (card == null)
                {
                    card = new Card { Number = number };
                    var cardInfo = allCardsInfo.FirstOrDefault(x => x.Number == number);
                    if (cardInfo != null)
                    {
                        card.Title = cardInfo.Title;
                        card.SubTitle = cardInfo.SubTitle;
                        card.Color = cardInfo.Color;
                        card.RarityStr = cardInfo.RarityStr;
                    }
                    else
                    {
                        //Enchanted / Promo
                        continue;
                    }
                    cardsList.Add(card);
                }
                else
                {

                }
                if (foil)
                {
                    card.Foils = item.Value;
                }
                else
                {
                    card.Normals = item.Value;
                }
            }

            Console.Clear();

            Console.WriteLine("Total Unique Cards: " + cardsList.Count);
            Console.WriteLine("Total Cards: " + cardsList.Sum(x => x.Total));
            Console.WriteLine("Total Foiled Cards: " + cardsList.Sum(x => x.Foils));
            var missingCards = allCardsInfo.Where(x => !cardsList.Any(y => y.Number == x.Number));
            Console.WriteLine();
            Console.WriteLine("Missing Cards: " + missingCards.Count());
            foreach (var missing in missingCards)
            {
                missing.WriteDisplay();
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Total Play Set Cards (>=2): " + cardsList.Where(x => x.Total >= 2).Count());
            Console.WriteLine("Total Play Set Cards (>=3): " + cardsList.Where(x => x.Total >= 3).Count());
            Console.WriteLine("Total Play Set Cards (>=4): " + cardsList.Where(x => x.Total >= 4).Count());
            Console.WriteLine();

            var twoCards = cardsList.Where(x => x.Total == 2);
            Console.WriteLine("2 times: " + twoCards.Count());
            foreach (var twoCard in twoCards)
            {
                twoCard.WriteDisplay();
                Console.WriteLine();
            }
            Console.WriteLine();

            var threeCards = cardsList.Where(x => x.Total == 3);
            Console.WriteLine("3 times: " + threeCards.Count());
            foreach (var threeCard in threeCards)
            {
                threeCard.WriteDisplay();
                Console.WriteLine();
            }

            var tradeables = cardsList.Where(x => x.Total > 4);
            Console.WriteLine();
            Console.WriteLine("Total Tradeable Cards (>4): " + tradeables.Count());
            Console.WriteLine("Total Tradeable Card count: " + tradeables.Sum(x => x.Total - 4));
            foreach (var tradeable in tradeables)
            {
                tradeable.WriteDisplay();
                Console.WriteLine(": " + (tradeable.Total - 4));
            }
            Console.ReadKey();
        }

        private static ConsoleColor ConsoleColorFromColor(Color color)
        {
            if (color == Color.Amber) return ConsoleColor.DarkYellow;
            if (color == Color.Amehtyst) return ConsoleColor.DarkMagenta;
            if (color == Color.Emerald) return ConsoleColor.DarkGreen;
            if (color == Color.Ruby) return ConsoleColor.DarkRed;
            if (color == Color.Sapphire) return ConsoleColor.DarkBlue;
            if (color == Color.Steel) return ConsoleColor.Gray;
            return ConsoleColor.White;
        }

        private static Color ColorFromString(string str)
        {
            if (str == "Amber") return Color.Amber;
            if (str == "Amethyst") return Color.Amehtyst;
            if (str == "Emerald") return Color.Emerald;
            if (str == "Ruby") return Color.Ruby;
            if (str == "Sapphire") return Color.Sapphire;
            if (str == "Steel") return Color.Steel;
            return Color.Amber;
        }

        private static Rarity RarityFromString(string str)
        {
            if (str == "Uncommon") return Rarity.Common;
            if (str == "Common") return Rarity.Uncommon;
            if (str == "Rare") return Rarity.Rare;
            if (str == "Super Rare") return Rarity.SuperRare;
            if (str == "Legendary") return Rarity.Legendary;
            if (str == "Enchanted") return Rarity.Enchanted;
            return Rarity.Common;
        }


        static T? GetPropertyValue<T>(JObject item, string v)
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

        private static async Task<string> GetAllCardsInfos(string names)
        {

            string apiUrl = $"http://api.lorcana-api.com/strict/{names}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Fehler beim Aufrufen der API. Statuscode: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private static async Task<string> GetAllCards()
        {

            string apiUrl = $"http://api.lorcana-api.com/lists/names";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Fehler beim Aufrufen der API. Statuscode: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        // Methode zum Suchen einer Eigenschaft in einem JToken
        static JToken FindProperty(JToken token, string key)
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
