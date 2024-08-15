using System;
using System.Net;
using lorcana.Cards;
using SkiaSharp;

namespace lorcana
{
    internal class Program
    {
        const string allCardsInfoCache = "allCardsInfo.json";

        private static Dictionary<string, SKBitmap> _imgCache = new Dictionary<string, SKBitmap>();

        static async Task Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Building Library...");

            string allCardsInfoJson = null;
            if (File.Exists(allCardsInfoCache))
            {
                allCardsInfoJson = File.ReadAllText(allCardsInfoCache);
            }
            CardLibrary lib = new CardLibrary();
            await lib.BuildLibrary(allCardsInfoJson, "de");
            File.WriteAllText(allCardsInfoCache, lib.AllCardsInfoJson);

            var collection = ImportCollection("export.csv", "Danny", lib);
            var cardsList = collection.List;
            var collectionOlli = ImportCollection("olli.csv", "olli", lib);
            var collectionIngo = ImportCollection("ingo.csv", "ingo", lib);
            var collectionNatalie = ImportCollection("natalie.csv", "natalie", lib);
            var collectionBine = ImportCollection("bine.csv", "BineKiki", lib);

            Console.WriteLine("INFOS: ");
            foreach (CardColor color in (CardColor[])Enum.GetValues(typeof(CardColor)))
            {
                var cardsInColor = lib.List.Where(x => x.Color == color);
                WriteColorNameColored(color);
                Console.WriteLine(": " + cardsInColor.Count());
            }
            Console.WriteLine();
            
            Console.WriteLine("YOUR COLLECTION:");
            Console.WriteLine("Total Unique Cards: " + cardsList.Where(x => x.Total > 0).Count());
            Console.WriteLine("Total Cards: " + cardsList.Sum(x => x.Total));
            Console.WriteLine("Total Foiled Cards: " + cardsList.Sum(x => x.Foils));
            Console.WriteLine();

            //WriteDecklist(cardsList.Where(x => x.Rarity == Rarity.Rare && x.NumberAsInt <= 204 && x.SetNumber == 4 && x.Total < 4), (card) => 4 - card.Total + "x ");


            bool filter(Card c)
            {
                return true;
            }
            
            WriteList(collectionBine.List.Where(x => x.Rarity >= Rarity.Rare && x.Total > 4));

            CompareCollections(collection, collectionBine, filter);
            CompareCollections(collectionBine, collection, filter);

            //DrawListToImageFiles("rares", cardsList.Where(x => x.Rarity == Rarity.Rare && x.NumberAsInt <= 204 && x.SetNumber == 4 && x.Total < 4), (c) => 4 - c.Total, null, 3, 3, 1500, 2092, 0, null);
            //DrawListToImageFiles("moritz", cardsList.Where(x => x.ConstructKey() == "1:18" || x.ConstructKey() == "4:70"), (c) => 5, null, "en", 3, 3, 1500, 2092, 0, null);
            //DrawListToImageFiles("auftrag", new List<CardToDraw> { new CardToDraw(1, "18", 3, "en"), new CardToDraw(3, "190", 1, "en"), new CardToDraw(4, "70", 4, "de"), new CardToDraw(4, "221", 1, "de") }, 3, 3, 1500, 2092, 0);
            //DrawListToImageFiles("enchanted_playset", cardsList.Where(x => x.Rarity == Rarity.Enchanted), (c) => 4, null, 3, 3, 1500, 2092, 0, null);
            //DrawListToImageFiles("ingo", cardsList.Where(x => x.), (c) => 1, null, 3, 3, 1500, 2092, 0, null);

            Console.ReadKey();
        }

        private static CardCollection ImportCollection(string filename, string title, CardLibrary library)
        {
            var csv = File.ReadAllText(filename);
            var collection = new CardCollection { Name = title };
            collection.InitializeWithCsv(library.List, csv, false);
            return collection;
        }

        private static void CompareCollections(CardCollection collection1, CardCollection collection2, Func<Card, bool>? filter = null)
        {
            Console.WriteLine(collection1.Name + "'s Extras: ");
            var list = filter == null ? collection1.List : collection1.List.Where(filter);
            foreach (var card in list)
            {
                var compareCard = collection2.List.FirstOrDefault(x => x.ConstructKey() == card.ConstructKey());
                int comparedCount = 0;
                if (compareCard != null)
                {
                    comparedCount = compareCard.Total;
                }
                if (comparedCount < 4 && card.Total > 4)
                {
                    WriteCardDisplay(card);
                    int spareCards = Math.Min(card.Total - 4, 4 - comparedCount);
                    Console.WriteLine($" {spareCards}x ({collection1.Name}: {card.Total} | {collection2.Name}: {comparedCount})");
                }
            }
            Console.WriteLine();
        }

        private static void DrawListToImageFiles(string folderName, IEnumerable<CardToDraw> list, uint cols, uint rows, uint cardWidth, uint cardHeight, uint padding)
        {
            Console.WriteLine($"Drawing {list.Sum(x => x.Amount)} images of {list.Count()} cards...");
            int cardIndex = 0;
            var currentCard = list.First();
            var cardImage = DownloadImage(currentCard.SetNumber, currentCard.Number, currentCard.CountryCode);
            int cardImagesDrawn = 0;
            int pageIndex = 0;
            int amountPerPage = (int)(cols * rows);
            for (int i = 0; i < list.Sum(x => x.Amount); i += amountPerPage)
            {
                SKBitmap bmp = new SKBitmap((int)(cols * cardWidth), (int)(rows * cardHeight));
                using (SKCanvas canvas = new SKCanvas(bmp))
                {
                    for (int r = 0; r < amountPerPage; r++)
                    {
                        if (cardImagesDrawn == currentCard.Amount)
                        {
                            cardIndex++;
                            currentCard = list.ElementAt(cardIndex);
                            cardImage = DownloadImage(currentCard.SetNumber, currentCard.Number, currentCard.CountryCode);
                            cardImagesDrawn = 0;
                        }

                        int colIndex = (int)((r - i) % cols);
                        int rowIndex = (int)((r - i) / cols);
                        float x = colIndex * cardWidth;
                        float y = rowIndex * cardHeight;

                        canvas.DrawBitmap(cardImage, new SKRect(x + padding, y + padding, x + cardWidth - padding, y + cardHeight - padding));
                        cardImagesDrawn++;
                    }
                }
                using (SKImage img = SKImage.FromBitmap(bmp))
                {
                    using (var encodedImage = img.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        string fileName = i.ToString();
                        string directory = Path.Combine(Environment.CurrentDirectory, folderName);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        string filePath = Path.Combine(directory, $"{fileName}.png");

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        using (var stream = File.OpenWrite(filePath))
                        {
                            encodedImage.SaveTo(stream);
                        }
                    }
                }
                pageIndex++;
            }
            Console.WriteLine($"Done.");
        }


        private static void DrawListToImageFiles(string folderName, IEnumerable<Card> list, Func<Card, int> amountFunc, Func<Card, string>? txtFunc, string countryCode, uint cols, uint rows, uint cardWidth, uint cardHeight, uint padding, Func<Card, int, string>? fileNameFunction)
        {
            var cardList = new List<Card>();
            foreach (var card in list)
            {
                int amount = amountFunc == null ? 1 : amountFunc.Invoke(card);
                for (int i = 0; i < amount; i++)
                {
                    cardList.Add(card);
                }
            }
            Console.WriteLine($"Drawing {cardList.Count()} images...");
            for (int i = 0; i < cardList.Count(); i += (int)(cols * rows))
            {
                SKBitmap bmp = new SKBitmap((int)(cols * cardWidth),(int)(rows * cardHeight));
                using (SKCanvas canvas = new SKCanvas(bmp))
                {
                    canvas.Clear(SKColors.Transparent);
                    for (int index = i; index < i + (cols * rows) && index < cardList.Count(); index++)
                    {
                        Card c = cardList.ElementAt(index);
                        int colIndex = (int)((index - i) % cols);
                        int rowIndex = (int)((index - i) / cols);
                        
                        float x = colIndex * cardWidth;
                        float y = rowIndex * cardHeight;

                        var img = DownloadImage(c.SetNumber, c.Number, countryCode);
                        canvas.DrawBitmap(img, new SKRect(x + padding, y + padding, x + cardWidth - padding, y + cardHeight - padding));
                        if (txtFunc != null)
                        {
                            canvas.DrawRect(new SKRect(x + padding, y + padding, x + cardWidth - padding, y + 40 - padding), new SKPaint { Color = SKColors.Black.WithAlpha(200) });
                            canvas.DrawText(txtFunc.Invoke(c), x + padding + cardWidth * 0.33f, y + padding + 20, new SKPaint { IsAntialias = true, Color = SKColors.White, TextSize = 24, FakeBoldText = true });
                        }
                    }
                }
                using (SKImage img = SKImage.FromBitmap(bmp))
                {
                    using (var encodedImage = img.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        string fileName = i.ToString();
                        if (fileNameFunction != null)
                        {
                            fileName = fileNameFunction.Invoke(cardList.ElementAt(i), i);
                        }
                        string directory = Path.Combine(Environment.CurrentDirectory, folderName);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        string filePath = Path.Combine(directory, $"{fileName}.png");

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        using (var stream = File.OpenWrite(filePath))
                        {
                            encodedImage.SaveTo(stream);
                        }
                    }
                }
            }
            Console.WriteLine($"Done.");
        }

        //https://images.dreamborn.ink/cards/de/001-001_1468x2048.webp
        private static SKBitmap? DownloadImage(int setNumber, string number, string countryCode)
        {
            string key = setNumber + "_" + number;
            if (_imgCache.ContainsKey(key))
            {
                return _imgCache[key];
            }
            HttpWebResponse response = null;
            string url = Card.GetImageLink(number, null, setNumber, countryCode);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    byte[] stream = null;
                    using (var webClient = new WebClient())
                    {
                        stream = webClient.DownloadData(url);
                    }
                    var imgDecoded = SKBitmap.Decode(stream);
                    _imgCache.Add(key, imgDecoded);
                    return imgDecoded;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return null;
        }

        private static void WriteList(IEnumerable<Card> list, Func<Card, string> postString = null)
        {
            string currentSetCode = string.Empty;
            foreach (var card in list.OrderBy(x => x.SetNumber))
            {
                if (currentSetCode != card.SetCode)
                {
                    currentSetCode = card.SetCode;
                    Console.WriteLine(currentSetCode);
                }
                WriteCardDisplay(card);
                Console.WriteLine(postString == null ? "" : postString.Invoke(card));
            }
            Console.WriteLine();
        }

        private static void WriteDecklist(IEnumerable<Card> list, Func<Card, string> amountFunc = null)
        {
            string currentSetCode = string.Empty;
            foreach (var card in list.OrderBy(x => x.SetNumber))
            {
                if (currentSetCode != card.SetCode)
                {
                    currentSetCode = card.SetCode;
                    Console.WriteLine(currentSetCode);
                }
                Console.Write(amountFunc == null ? "" : amountFunc.Invoke(card));
                Console.WriteLine(card.Title + (string.IsNullOrEmpty(card.SubTitle) ? "" : " - " + card.SubTitle));
            }
            Console.WriteLine();
        }

        private static void WriteCardDisplay(Card c)
        {
            Console.Write($"{c.SetNumber} | {c.NumberDisplay}");
            for (int i = 0; i < 5 - c.NumberDisplay.Length; i++)
            {
                Console.Write(" ");
            }
            WriteRarityColored(c.Rarity);
            Console.Write("\t");
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColorFromColor(c.Color);
            Console.Write(c.Display);
            Console.ForegroundColor = preColor;
        }

        private static void WriteColorNameColored(CardColor color)
        {
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColorFromColor(color);
            Console.Write(Helpers.StringFromColor(color));
            Console.ForegroundColor = preColor;
        }

        private static void WriteRarityColored(Rarity rarity)
        {
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColorFromRarity(rarity);
            Console.Write(Helpers.StringFromRarity(rarity, false));
            Console.ForegroundColor = preColor;
        }

        private static ConsoleColor ConsoleColorFromColor(CardColor color)
        {
            if (color == CardColor.Amber) return ConsoleColor.DarkYellow;
            if (color == CardColor.Amethyst) return ConsoleColor.DarkMagenta;
            if (color == CardColor.Emerald) return ConsoleColor.DarkGreen;
            if (color == CardColor.Ruby) return ConsoleColor.DarkRed;
            if (color == CardColor.Sapphire) return ConsoleColor.DarkBlue;
            if (color == CardColor.Steel) return ConsoleColor.Gray;
            return ConsoleColor.White;
        }

        private static ConsoleColor ConsoleColorFromRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    return ConsoleColor.Gray;
                case Rarity.Uncommon:
                    return ConsoleColor.White;
                case Rarity.Rare:
                    return ConsoleColor.DarkRed;
                case Rarity.SuperRare:
                    return ConsoleColor.Blue;
                case Rarity.Legendary:
                    return ConsoleColor.DarkYellow;
                case Rarity.Enchanted:
                    return ConsoleColor.Cyan;
                default:
                    return ConsoleColor.DarkGray;
            }
        }
    }
}
