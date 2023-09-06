using lorcana.Cards;

namespace lorcana
{

    internal class Program
    {

        static async Task Main(string[] args)
        {
            await CardLibrary.BuildLibrary();

            var json = File.ReadAllText("update.json");

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
                missing.WriteDisplay();
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
                twoCard.WriteDisplay();
                Console.WriteLine();
            }
            Console.WriteLine();

            var threeCards = cardsList.Where(x => x.Total == 3);
            Console.WriteLine("1 missing: " + threeCards.Count());
            foreach (var threeCard in threeCards)
            {
                threeCard.WriteDisplay();
                Console.WriteLine();
            }

            var tradeables = cardsList.Where(x => x.Total > 4);
            Console.WriteLine();
            Console.WriteLine("Total Tradeable Cards (>4): " + tradeables.Count());
            Console.WriteLine("Total Tradeable Card count: " + tradeables.Sum(x => x.Total - 4));
            foreach (var tradeable in tradeables)
            {
                tradeable.WriteDisplay();
                Console.WriteLine(": " + (tradeable.Total - 4));
            }
            Console.ReadKey();
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
