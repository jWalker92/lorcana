using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lorcana.Cards
{
    public class CardCollection
    {
        private const string CSV_NORMALS = "normal";
        private const string CSV_FOILS = "foil";
        private const string CSV_CARDNUMBER = "card number";
        private const string CSV_SET = "set";
        private const string CSV_NAME = "name";
        private const string CSV_COLOR = "color";
        private const string CSV_RARITY = "rarity";

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

        public static string ListToCsv(List<Card> list)
        {
            string csvOutput = string.Empty;
            csvOutput += CSV_SET + "," + CSV_CARDNUMBER + "," + CSV_NORMALS + "," + CSV_FOILS + Environment.NewLine;
            foreach (var item in list)
            {
                csvOutput += item.SetNumber + "," + item.NumberAsInt + "," + item.Normals + "," + item.Foils + Environment.NewLine;
            }
            return csvOutput;
        }

        public void InitializeWithCsv(List<Card> library, string csv)
        {
            try
            {
                var importedCards = new List<Card>();
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
                        lines[line] = lines[line].Replace(", ", ";;; ");
                        var values = lines[line].Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = values[i].Replace(";;; ", ", ");
                        }
                        if (line == 0)
                        {
                            normalsIndex = Array.FindIndex(values, x => x.ToLower() == CSV_NORMALS);
                            foilsIndex = Array.FindIndex(values, x => x.ToLower() == CSV_FOILS);
                            numberIndex = Array.FindIndex(values, x => x.ToLower() == CSV_CARDNUMBER);
                            setIndex = Array.FindIndex(values, x => x.ToLower() == CSV_SET);
                            nameIndex = Array.FindIndex(values, x => x.ToLower() == CSV_NAME);
                            colorIndex = Array.FindIndex(values, x => x.ToLower() == CSV_COLOR);
                            rarityIndex = Array.FindIndex(values, x => x.ToLower() == CSV_RARITY);
                            if (normalsIndex == -1 || foilsIndex == -1 || numberIndex == -1 || setIndex == -1)
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
                            if (library != null)
                            {
                                card = library.FirstOrDefault(x => x.Number == number && x.SetNumber == setCodeNumber);
                            }
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

                                string url = Card.GetImageLink(number, null, setCodeNumber);
                                card.Image = url;
                                if (library != null)
                                {
                                    library.Add(card);
                                }
                                cardsList.Add(card);
                            }
                        }
                        int foils = int.Parse(values[foilsIndex]);
                        int normals = int.Parse(values[normalsIndex]);
                        var alreadyImportedThisRun = importedCards.FirstOrDefault(x => x.ConstructKey() == card.ConstructKey());
                        if (alreadyImportedThisRun != null)
                        {
                            foils += alreadyImportedThisRun.Foils;
                            normals += alreadyImportedThisRun.Normals;
                        }
                        card.Foils = foils;
                        card.Normals = normals;
                        importedCards.Add(card);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //cardsList.AddRange(library.Where(x => !cardsList.Any(y => y.SetNumber == x.SetNumber && y.Number == x.Number)));
            }
            catch (System.Exception ex)
            {
            }
        }

        public void InitializeFromList(List<Card> list, List<Card> cards, Func<Card, Task> updateCardTask)
        {
            cardsList = cards;
            foreach (var libCard in list)
            {
                var listCard = cardsList.FirstOrDefault(x => x.ConstructKey() == libCard.ConstructKey());
                if (listCard != null)
                {
                    int normalsBackup = listCard.Normals;
                    int foilsBackup = listCard.Foils;
                    listCard = libCard;
                    listCard.Normals = normalsBackup;
                    listCard.Foils = foilsBackup;
                }
                else
                {
                    cardsList.Add(libCard);
                }
                if (updateCardTask != null)
                {
                    updateCardTask?.Invoke(libCard);
                }
            }
        }
    }
}

