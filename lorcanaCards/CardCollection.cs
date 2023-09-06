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

        public CardCollection(List<Card> library, string updateJson)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, intVal>>(updateJson);

            foreach (var item in dict.OrderBy(x => x.Key))
            {
                int chapter = 0;
                int number = 0;
                bool foil = false;

                var keyItems = item.Key.Split('-', ':');
                if (keyItems.Length >= 2)
                {
                    chapter = int.Parse(keyItems[0]);
                    number = int.Parse(keyItems[1]);
                    if (keyItems.Length >= 3 && keyItems[2] == "foil")
                    {
                        foil = true;
                    }
                }
                Card? card = cardsList.FirstOrDefault(x => x.Number == number);
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
	}
}

