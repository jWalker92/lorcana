using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lorcana.Cards
{
    public class CardCollection
    {
        private List<Card> cardsList = new List<Card>();

        public List<Card> List { get => cardsList; }

        class intVal
        {
            public int IntegerValue { get; set; }

            public static implicit operator int(intVal val) => val.IntegerValue;

            public override string ToString()
            {
                return IntegerValue.ToString();
            }
        }

        public void InitializeWithJson(List<Card> library, string updateJson)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, intVal>>(updateJson);

                foreach (var item in dict.OrderBy(x => x.Key))
                {
                    int chapter = 0;
                    string number = "";
                    bool foil = false;

                    var keyItems = item.Key.Split('-', ':');
                    if (keyItems.Length >= 2)
                    {
                        chapter = int.Parse(keyItems[0]);
                        number = keyItems[1];
                        if (keyItems.Length >= 3 && keyItems[2] == "foil")
                        {
                            foil = true;
                        }
                    }
                    Card card = cardsList.FirstOrDefault(x => x.Number == number);
                    if (card == null)
                    {
                        card = library.FirstOrDefault(x => x.Number == number);
                        if (card != null)
                        {
                            cardsList.Add(card);
                        }
                        else
                        {
                            //Enchanted / Promo
                            continue;
                        }
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
            }
            catch (System.Exception ex)
            {
            }
        }

        public void InitializeWithCsv(List<Card> library, string csv)
        {
            try
            {
                var lines = csv.Split('\n');

                int normalsIndex = -1;
                int foilsIndex = -1;
                int numberIndex = -1;
                int setIndex = -1;
                int nameIndex = -1;
                int colorIndex = -1;
                int rarityIndex = -1;
                for (int line = 0; line < lines.Length - 1; line++)
                {
                    try
                    {
                        var values = lines[line].Split(',');
                        if (line == 0)
                        {
                            normalsIndex = Array.FindIndex(values, x => x.ToLower() == "normal");
                            foilsIndex = Array.FindIndex(values, x => x.ToLower() == "foil");
                            numberIndex = Array.FindIndex(values, x => x.ToLower() == "card number");
                            setIndex = Array.FindIndex(values, x => x.ToLower() == "set");
                            nameIndex = Array.FindIndex(values, x => x.ToLower() == "name");
                            colorIndex = Array.FindIndex(values, x => x.ToLower() == "color");
                            rarityIndex = Array.FindIndex(values, x => x.ToLower() == "rarity");
                            if (normalsIndex == -1 || foilsIndex == -1 || numberIndex == -1)
                            {
                                break;
                            }
                            continue;
                        }
                        string number = values[numberIndex];
                        int.TryParse(values[setIndex], out int setCodeNumber);
                        Card card = cardsList.FirstOrDefault(x => x.Number == number && x.SetNumber == setCodeNumber);
                        if (card == null)
                        {
                            card = library.FirstOrDefault(x => x.Number == number && x.SetNumber == setCodeNumber);
                            if (card != null)
                            {
                                cardsList.Add(card);
                            }
                            else
                            {
                                string rarity = values[rarityIndex];
                                if (rarity == "Enchanted" || rarity == "Promo")
                                {
                                    continue;
                                }
                                string name = values[nameIndex];
                                string color = values[colorIndex];
                                string setCode = Helpers.NumberToSetcode(setCodeNumber);
                                card = new Card() { Title = name, SetNumber = setCodeNumber, Number = number, Color = Helpers.ColorFromString(color), RarityStr = rarity };
                                library.Add(card);
                                cardsList.Add(card);
                            }
                        }
                        card.Foils = int.Parse(values[foilsIndex]);
                        card.Normals = int.Parse(values[normalsIndex]);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                cardsList.AddRange(library.Where(x => !cardsList.Any(y => y.SetNumber == x.SetNumber && y.Number == x.Number)));
            }
            catch (System.Exception ex)
            {
            }
        }

    }
}

