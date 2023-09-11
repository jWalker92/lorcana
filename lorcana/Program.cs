using lorcana.Cards;

namespace lorcana
{
    internal class Program
    {
        const string allCardsCache = "allCards.json";
        const string allCardsInfoCache = "allCardsInfo.json";

        static async Task Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Building Library...");

            string allCardsJson = null;
            if (File.Exists(allCardsCache))
            {
                allCardsJson = File.ReadAllText(allCardsCache);
            }
            string allCardsInfoJson = null;
            if (File.Exists(allCardsInfoCache))
            {
                allCardsInfoJson = File.ReadAllText(allCardsInfoCache);
            }
            await CardLibrary.BuildLibrary(allCardsJson, allCardsInfoJson);
            File.WriteAllText(allCardsCache, CardLibrary.AllCardsJson);
            File.WriteAllText(allCardsInfoCache, CardLibrary.AllCardsInfoJson);

            var json = File.ReadAllText("update.json");

            Console.WriteLine("Building Collection...");
            var collection = new CardCollection(CardLibrary.List, json);
            var cardsList = collection.List;

            Console.Clear();

            Console.WriteLine("INFOS: ");
            foreach (CardColor color in (CardColor[])Enum.GetValues(typeof(CardColor)))
            {
                var cardsInColor = CardLibrary.List.Where(x => x.Color == color);
                WriteColorNameColored(color);
                Console.WriteLine(": " + cardsInColor.Count());
            }
            Console.WriteLine();

            Console.WriteLine("YOUR COLLECTION:");
            Console.WriteLine("Total Unique Cards: " + cardsList.Count);
            Console.WriteLine("Total Cards: " + cardsList.Sum(x => x.Total));
            Console.WriteLine("Total Foiled Cards: " + cardsList.Sum(x => x.Foils));
            Console.WriteLine();

            Console.WriteLine("Total Play Set Cards (>=2): " + cardsList.Where(x => x.Total >= 2).Count());
            Console.WriteLine("Total Play Set Cards (>=3): " + cardsList.Where(x => x.Total >= 3).Count());
            var playSet = cardsList.Where(x => x.Total >= 4);
            Console.WriteLine("Total Play Set Cards (>=4): " + playSet.Count());
            Console.WriteLine();

            var missingCards = CardLibrary.List.Where(x => !cardsList.Any(y => y.Number == x.Number));
            Console.WriteLine("4 missing: " + missingCards.Count());
            WriteList(missingCards);
            Console.WriteLine();

            var oneCards = cardsList.Where(x => x.Total == 1);
            Console.WriteLine("3 missing: " + oneCards.Count());
            WriteList(oneCards);
            Console.WriteLine();

            var twoCards = cardsList.Where(x => x.Total == 2);
            Console.WriteLine("2 missing: " + twoCards.Count());
            WriteList(twoCards);
            Console.WriteLine();

            var threeCards = cardsList.Where(x => x.Total == 3);
            Console.WriteLine("1 missing: " + threeCards.Count());
            WriteList(threeCards);
            Console.WriteLine();

            var noAlbum = cardsList.Where(x => x.Total == 4);
            Console.WriteLine("Playable but not in album: " + noAlbum.Count());
            WriteList(noAlbum);
            Console.WriteLine();

            var tradeables = cardsList.Where(x => x.Total > 5);
            Console.WriteLine("Total Tradeable Cards (>5): " + tradeables.Count());
            Console.WriteLine("Total Tradeable Card count: " + tradeables.Sum(x => x.Total - 5));
            WriteList(tradeables, (c) => ": " + (c.Total - 5));
            Console.ReadKey();
        }

        private static void WriteList(IEnumerable<Card> list, Func<Card, string> postString = null)
        {
            foreach (var card in list)
            {
                WriteCardDisplay(card);
                Console.WriteLine(postString == null ? "" : postString.Invoke(card));
            }
        }

        private static void WriteCardDisplay(Card c)
        {
            Console.Write(c.NumberDisplay);
            Console.Write(" ");
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
            Console.Write(Helpers.StringFromRarity(rarity, true));
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
