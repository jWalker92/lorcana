using lorcana.Cards;

namespace lorcana
{
	public class CardToDraw : Card
	{
		public string CountryCode;
		public int Amount;

        public CardToDraw(int set, string num, int amount, string countryCode)
        {
            SetNumber = set;
            Number = num;
            Amount = amount;
            CountryCode = countryCode;
        }
    }
}

