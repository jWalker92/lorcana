using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using lorcanaCards;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lorcana.Cards
{
    public class CardLibrary
    {
        private List<Card> allCardsInfo;

        public List<Card> List { get => allCardsInfo; }

        public string AllCardsInfoJson { get; private set; }

        public async Task BuildLibrary(string allCardsInfoJson, string countryCode)
        {
            if (string.IsNullOrEmpty(allCardsInfoJson))
            {
                allCardsInfoJson = await GetAllCards();
            }
            AllCardsInfoJson = allCardsInfoJson;
            if (AllCardsInfoJson == null)
            {
                return;
            }

            var allInfo = JObject.Parse(AllCardsInfoJson);
            var cardsItems = allInfo["cards"].ToObject<List<JsonCard>>();

            allCardsInfo = new List<Card>();
            foreach (var item in cardsItems)
            {
                string numberStr = item.number.ToString();
                string name = item.name;
                string subtitle = item.version;
                string cardVariant = item.variant;
                bool couldParse = int.TryParse(item.setCode, out int setNum);
                if (!couldParse)
                {
                    continue;
                }
                string baseImage = item.images.full; //Card.GetImageLink(numberStr, cardVariant, setNum, countryCode);
                string rarityStr = item.rarity;
                string typeStr = item.type;
                var infoCard = new Card
                {
                    Number = numberStr,
                    Title = name,
                    SetNumber = setNum,
                    SubTitle = subtitle,
                    Color = Helpers.ColorFromString(item.color),
                    RarityStr = rarityStr,
                    TypeStr = typeStr,
                    Image = baseImage,
                    SmallImage = item.images.thumbnail,
                    FoilMaskImage = item.images.foilMask,
                    //ArtImage = Helpers.GetPropertyValue<string>(Helpers.GetPropertyValue<JObject>(item, "image-urls"), "art-crop"),
                    Strength = item.strength,
                    Willpower = item.willpower,
                    LoreValue = item.lore,
                    FlavorText = item.flavorText,
                    InkCost = item.cost,
                    Inkable = item.inkwell,
                    Artist = item.artistsText,
                    Body = item.fullText
                };
                allCardsInfo.Add(infoCard);
            }
            allCardsInfo = allCardsInfo.OrderBy(x => x.Number).ToList();
        }

        private static async Task<string> GetAllCards()
        {

            string apiUrl = $"https://lorcanajson.org/files/current/en/allCards.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
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
                    catch (Exception ex)
                    {
                        return string.Empty;
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

