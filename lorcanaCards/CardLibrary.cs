using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            var allInfo = JsonConvert.DeserializeObject<List<JObject>>(AllCardsInfoJson);

            allCardsInfo = new List<Card>();
            foreach (var item in allInfo)
            {
                string numberStr = Helpers.GetPropertyValue<string>(item, "Card_Num");
                string nameAndSubtitle = Helpers.GetPropertyValue<string>(item, "Name");
                var nameSubSplit = nameAndSubtitle.Split(new string[]{ " - "}, StringSplitOptions.RemoveEmptyEntries);
                string name = nameAndSubtitle;
                string subtitle = null;
                if (nameSubSplit.Count() == 2)
                {
                    name = nameSubSplit.First();
                    subtitle = nameSubSplit.Last();
                }
                bool hasEnchantedVariant = false;
                bool hasCardVariants = false;
                string variantsStr = Helpers.GetPropertyValue<string>(item, "Card_Variants");
                if (variantsStr != null)
                {
                    var variants = variantsStr.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var variant in variants)
                    {
                        if (variant.ToLower().Contains("enchanted"))
                        {
                            hasEnchantedVariant = true;
                        }
                        if (variant.ToLower().Contains("card_num"))
                        {
                            hasCardVariants = true;
                        }
                    }
                }
                int setNum = Helpers.GetPropertyValue<int>(item, "Set_Num");
                string baseImage = Card.GetImageLink(numberStr, hasCardVariants ? "a" : null, setNum, countryCode);
                string rarityStr = Helpers.GetPropertyValue<string>(item, "Rarity");
                string typeStr = Helpers.GetPropertyValue<string>(item, "Type");
                var infoCard = new Card
                {
                    Number = numberStr,
                    Title = name,
                    SetNumber = setNum,
                    SubTitle = subtitle,
                    Color = Helpers.ColorFromString(Helpers.GetPropertyValue<string>(item, "Color")),
                    RarityStr = rarityStr,
                    TypeStr = typeStr,
                    Image = baseImage,
                    //SmallImage = Helpers.ReplaceLastOccurrence(baseImage, "large", "small"),
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

