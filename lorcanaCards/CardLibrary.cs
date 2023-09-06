using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lorcana.Cards
{
    public static class CardLibrary
    {
        private static List<Card> allCardsInfo;

        public static List<Card> List { get => allCardsInfo; }

        public static async Task BuildLibrary()
        {
            string allCardsJson = await GetAllCards();
            var allNames = JsonConvert.DeserializeObject<List<string>>(allCardsJson);
            string allCardsInfoJson = await GetAllCardsInfos(string.Join(';', allNames));

            var allInfo = JsonConvert.DeserializeObject<List<JObject>>(allCardsInfoJson);

            allCardsInfo = new List<Card>();
            foreach (var item in allInfo)
            {
                int number = Helpers.GetPropertyValue<int>(item, "card-number");
                string name = Helpers.GetPropertyValue<string>(item, "name");
                string rarityStr = Helpers.GetPropertyValue<string>(item, "rarity");
                Card infoCard = new Card
                {
                    Number = number,
                    Title = name,
                    SubTitle = Helpers.GetPropertyValue<string>(item, "subtitle"),
                    Color = Helpers.ColorFromString(Helpers.GetPropertyValue<string>(item, "color")),
                    RarityStr = rarityStr,
                    Image = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "medium"),
                    SmallImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "small"),
                    ArtImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "art-crop")
                };
                allCardsInfo.Add(infoCard);
            }
            allCardsInfo = allCardsInfo.OrderBy(x => x.Number).ToList();
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
    }
}

