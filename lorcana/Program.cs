﻿using System.Collections.Generic;
using System.Net;
using lorcana.Cards;
using SkiaSharp;

namespace lorcana
{
    internal class Program
    {
        const string allCardsInfoCache = "allCardsInfo.json";

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

            var csv = File.ReadAllText("export.csv");
            var csvCompare = File.ReadAllText("compare.csv");

            Console.WriteLine("Building Collection...");
            var collection = new CardCollection();
            collection.InitializeWithCsv(lib.List, csv);
            var cardsList = collection.List;

            CardLibrary libCompare = new CardLibrary();
            await libCompare.BuildLibrary(allCardsInfoJson, "de");
            var collectionCompare = new CardCollection();
            collectionCompare.InitializeWithCsv(libCompare.List, csvCompare);

            Console.Clear();

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

            //Console.WriteLine("Total Play Set Cards (>=2): " + cardsList.Where(x => x.Total >= 2).Count());
            //Console.WriteLine("Total Play Set Cards (>=3): " + cardsList.Where(x => x.Total >= 3).Count());
            //var playSet = cardsList.Where(x => x.Total >= 4);
            //Console.WriteLine("Total Play Set Cards (>=4): " + playSet.Count());
            //Console.WriteLine();

            //var missingCards = cardsList.Where(x => x.Total == 0);
            //Console.WriteLine("4 missing: " + missingCards.Count());
            //WriteList(missingCards);
            //Console.WriteLine();

            //var oneCards = cardsList.Where(x => x.Total == 1);
            //Console.WriteLine("3 missing: " + oneCards.Count());
            //WriteList(oneCards);
            //Console.WriteLine();

            //var twoCards = cardsList.Where(x => x.Total == 2);
            //Console.WriteLine("2 missing: " + twoCards.Count());
            //WriteList(twoCards);
            //Console.WriteLine();

            //var threeCards = cardsList.Where(x => x.Total == 3);
            //Console.WriteLine("1 missing: " + threeCards.Count());
            //WriteList(threeCards);
            //Console.WriteLine();

            //var noAlbum = cardsList.Where(x => x.Total == 4);
            //Console.WriteLine("Playable but not in album: " + noAlbum.Count());
            //WriteList(noAlbum);
            //Console.WriteLine();

            //int tradeWithholdValue = 8;

            //var tradeables = cardsList.Where(x => x.Total > tradeWithholdValue);
            //Console.WriteLine($"Total Tradeable Cards (>{tradeWithholdValue}): " + tradeables.Count());
            //Console.WriteLine("Total Tradeable Card count: " + tradeables.Sum(x => x.Total - tradeWithholdValue));
            //WriteList(tradeables, (c) => ": " + (c.Total - tradeWithholdValue));
            //Console.WriteLine();

            var allMissingCommonUncommon = cardsList.Where(x => x.Total > 4); //(x.Rarity == Rarity.Common || x.Rarity == Rarity.Uncommon) && 
            Console.WriteLine("Missing : " + allMissingCommonUncommon.Count());
            WriteList(allMissingCommonUncommon, (c) => ": " + (c.Total - 4));
            Console.WriteLine();
            
            //var allFoiled = cardsList.Where(x => x.Foils > 0);
            //Console.WriteLine("Foiled Cards: " + allFoiled.Count());
            //WriteList(allFoiled, (c) => ": " + (c.Foils));
            //Console.WriteLine();

            var allFoiledTradeable = cardsList.Where(x => (x.Rarity == Rarity.Common || x.Rarity == Rarity.Uncommon) && x.Foils > 0 && x.Total >= 5);
            Console.WriteLine("Foiled Cards Tradeable: " + allFoiledTradeable.Count());
            WriteList(allFoiledTradeable, (c) => ": " + (c.Foils));
            Console.WriteLine();

            var allFoiledTradeable1v1 = cardsList.Where(x => x.Foils > 0 && x.Total <= 4);
            Console.WriteLine("Foiled Cards Tradeable 1v1: " + allFoiledTradeable1v1.Count());
            WriteList(allFoiledTradeable1v1, (c) => ": " + (c.Foils));
            Console.WriteLine();

            //var legendariesIti = cardsList.Where(x => x.Rarity >= Rarity.Legendary && x.Total > 0);
            //Console.WriteLine("Legendaries: " + legendariesIti.Count());
            //WriteList(legendariesIti, (c) => ": Normals (" + c.Normals + ") Foils (" + c.Foils + ")");
            //Console.WriteLine();

            Console.WriteLine("Compared Cards: ");
            foreach (var card in cardsList)
            {
                var compareCard = collectionCompare.List.FirstOrDefault(x => x.ConstructKey() == card.ConstructKey());
                if (compareCard != null)
                {
                    if (compareCard.Normals != card.Normals || compareCard.Foils != card.Foils)
                    {
                        WriteCardDisplay(card);
                        Console.WriteLine($" {card.Normals} -> {compareCard.Normals} | {card.Foils} -> {compareCard.Foils}");
                    }
                }
                else
                {
                    WriteCardDisplay(card);
                    Console.WriteLine(" Not found in compareList");
                }
            }
            Console.WriteLine();

            //DrawListToImageFiles(legendariesIti, (c) => "N(" + c.Normals + ") F(" + c.Foils + ")", 12, 5, 250, 350, 4, null);

            Console.ReadKey();
        }

        private static void DrawListToImageFiles(IEnumerable<Card> cardList, Func<Card, string>? txtFunc, uint cols, uint rows, uint cardWidth, uint cardHeight, uint padding, Func<Card, int, string>? fileNameFunction)
        {
            Console.WriteLine($"Drawing {cardList.Count()} images...");
            for (int i = 0; i < cardList.Count(); i += (int)(cols * rows))
            {
                SKBitmap bmp = new SKBitmap((int)(cols * cardWidth),(int)(rows * cardHeight));
                using (SKCanvas canvas = new SKCanvas(bmp))
                {
                    canvas.Clear(SKColors.White);
                    for (int index = i; index < i + (cols * rows) && index < cardList.Count(); index++)
                    {
                        Card c = cardList.ElementAt(index);
                        int colIndex = (int)((index - i) % cols);
                        int rowIndex = (int)((index - i) / cols);
                        
                        float x = colIndex * cardWidth;
                        float y = rowIndex * cardHeight;

                        var img = DownloadImage(c.SetNumber, c.Number);
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
                        string filePath = Path.Combine(Environment.CurrentDirectory, $"{fileName}.png");
                        string? directoryName = Path.GetDirectoryName(filePath);

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
        private static SKBitmap? DownloadImage(int setNumber, string number)
        {
            HttpWebResponse response = null;
            string url = Card.GetImageLink(number, null, setNumber);
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
                    return SKBitmap.Decode(stream);
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
        }

        private static void WriteCardDisplay(Card c)
        {
            Console.Write(c.NumberDisplay);
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
                    return ConsoleColor.DarkGray;
                case Rarity.Uncommon:
                    return ConsoleColor.White;
                case Rarity.Rare:
                    return ConsoleColor.DarkRed;
                case Rarity.SuperRare:
                    return ConsoleColor.Gray;
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
