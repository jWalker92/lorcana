using lorcana.Cards;

namespace lorcana
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Building Library...");
            await CardLibrary.BuildLibrary();

            var json = File.ReadAllText("update.json");

            Console.WriteLine("Building Collection...");
            var collection = new CardCollection(CardLibrary.List, json);
            var cardsList = collection.List;

            Console.Clear();

            Console.WriteLine("Total Unique Cards: " + cardsList.Count);
            Console.WriteLine("Total Cards: " + cardsList.Sum(x => x.Total));
            Console.WriteLine("Total Foiled Cards: " + cardsList.Sum(x => x.Foils));
            var missingCards = CardLibrary.List.Where(x => !cardsList.Any(y => y.Number == x.Number));
            Console.WriteLine();
            Console.WriteLine("Missing Cards: " + missingCards.Count());
            foreach (var missing in missingCards)
            {
                WriteCardDisplay(missing);
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Total Play Set Cards (>=2): " + cardsList.Where(x => x.Total >= 2).Count());
            Console.WriteLine("Total Play Set Cards (>=3): " + cardsList.Where(x => x.Total >= 3).Count());
            var playSet = cardsList.Where(x => x.Total >= 4);
            Console.WriteLine("Total Play Set Cards (>=4): " + playSet.Count());
            Console.WriteLine();

            var twoCards = cardsList.Where(x => x.Total == 2);
            Console.WriteLine("2 missing: " + twoCards.Count());
            foreach (var twoCard in twoCards)
            {
                WriteCardDisplay(twoCard);
                Console.WriteLine();
            }
            Console.WriteLine();

            var threeCards = cardsList.Where(x => x.Total == 3);
            Console.WriteLine("1 missing: " + threeCards.Count());
            foreach (var threeCard in threeCards)
            {
                WriteCardDisplay(threeCard);
                Console.WriteLine();
            }

            var tradeables = cardsList.Where(x => x.Total > 5);
            Console.WriteLine();
            Console.WriteLine("Total Tradeable Cards (>5): " + tradeables.Count());
            Console.WriteLine("Total Tradeable Card count: " + tradeables.Sum(x => x.Total - 5));
            foreach (var tradeable in tradeables)
            {
                WriteCardDisplay(tradeable);
                Console.WriteLine(": " + (tradeable.Total - 5));
            }
            Console.ReadKey();
        }

        private static void WriteCardDisplay(Card c)
        {
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColorFromColor(c.Color);
            Console.Write(c.Display);
            Console.ForegroundColor = preColor;
        }

        private static ConsoleColor ConsoleColorFromColor(CardColor color)
        {
            if (color == CardColor.Amber) return ConsoleColor.DarkYellow;
            if (color == CardColor.Amehtyst) return ConsoleColor.DarkMagenta;
            if (color == CardColor.Emerald) return ConsoleColor.DarkGreen;
            if (color == CardColor.Ruby) return ConsoleColor.DarkRed;
            if (color == CardColor.Sapphire) return ConsoleColor.DarkBlue;
            if (color == CardColor.Steel) return ConsoleColor.Gray;
            return ConsoleColor.White;
        }
    }
}
