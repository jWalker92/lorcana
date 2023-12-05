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

        public static string AllCardsInfoJson { get; private set; }

        public static async Task BuildLibrary(string allCardsInfoJson)
        {
            if (string.IsNullOrEmpty(allCardsInfoJson))
            {
                allCardsInfoJson = await GetAllCards();
            }
            AllCardsInfoJson = allCardsInfoJson;
            var allInfo = JsonConvert.DeserializeObject<List<JObject>>(AllCardsInfoJson);

            allCardsInfo = new List<Card>();
            foreach (var item in allInfo)
            {
                string number = Helpers.GetPropertyValue<string>(item, "Card_Num");
                string name = Helpers.GetPropertyValue<string>(item, "Name");
                string rarityStr = Helpers.GetPropertyValue<string>(item, "Rarity");
                Card infoCard = new Card
                {
                    Number = number,
                    Title = name,
                    SetCode = Helpers.GetPropertyValue<string>(item, "Set_ID"),
                    SubTitle = Helpers.GetPropertyValue<string>(item, "Subtitle"),
                    Color = Helpers.ColorFromString(Helpers.GetPropertyValue<string>(item, "Color")),
                    RarityStr = rarityStr,
                    Image = Helpers.GetPropertyValue<string>(item, "Image"),
                    //SmallImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "small"),
                    //ArtImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "art-crop"),
                    Strength = Helpers.GetPropertyValue<int?>(item, "Strength"),
                    Willpower = Helpers.GetPropertyValue<int?>(item, "Willpower"),
                    LoreValue = Helpers.GetPropertyValue<int?>(item, "Lore"),
                    FlavorText = Helpers.GetPropertyValue<string>(item, "Flavor_Text"),
                    InkCost = Helpers.GetPropertyValue<int>(item, "Cost"),
                    Inkable = Helpers.GetPropertyValue<bool>(item, "Inkable"),
                    Artist = Helpers.GetPropertyValue<string>(item, "Artist"),
                    Body = Helpers.GetPropertyValue<string>(item, "Body_Text")
                };
                allCardsInfo.Add(infoCard);
            }
            allCardsInfo = allCardsInfo.OrderBy(x => x.Number).ToList();
        }

        private static async Task<string> GetAllCards()
        {

            string apiUrl = $"https://api.lorcana-api.com/cards/all";

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

