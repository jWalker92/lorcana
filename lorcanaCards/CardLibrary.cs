using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lorcana.Cards
{
    public static class CardLibrary
    {
        private static List<Card> allCardsInfo;

        public static List<Card> List { get => allCardsInfo; }

        public static string AllCardsJson { get; private set; }

        public static string AllCardsInfoJson { get; private set; }

        public static async Task BuildLibrary(string allCardsJson, string allCardsInfoJson)
        {
            if (string.IsNullOrEmpty(allCardsJson))
            {
                allCardsJson = await GetAllCards();
            }
            AllCardsJson = allCardsJson;
            var allNames = JsonConvert.DeserializeObject<List<string>>(AllCardsJson);

            if (string.IsNullOrEmpty(allCardsInfoJson))
            {
                allCardsInfoJson = await GetAllCardsInfos(string.Join(";", allNames));
            }
            AllCardsInfoJson = allCardsInfoJson;
            var allInfo = JsonConvert.DeserializeObject<List<JObject>>(AllCardsInfoJson);

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
                    Image = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "large"),
                    SmallImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "small"),
                    ArtImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "art-crop"),
                    Body = Helpers.GetPropertyValue<string>(item, "body-text")
                };
                allCardsInfo.Add(infoCard);
            }
            allCardsInfo = allCardsInfo.OrderBy(x => x.Number).ToList();
        }

        private static async Task<string> GetAllCardsInfos(string names)
        {

            string apiUrl = $"https://api.lorcana-api.com/strict/{names}";

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

            string apiUrl = $"https://api.lorcana-api.com/lists/names";

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

