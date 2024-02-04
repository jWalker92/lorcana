using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using lorcana.Cards;
using Newtonsoft.Json;
using SQLite;

namespace lorcanaApp
{
    public class Database
    {
        public SQLiteAsyncConnection connection;
        private static Database instance;

        public static Database Instance { get => instance ?? new Database(); }

        public event EventHandler<bool> DatabaseInitiated;

        private Database()
        {
            instance = this;
            //File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "lorcana.db3"));
            connection = new SQLiteAsyncConnection(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "lorcana.db3"));
        }

        public async Task Init()
        {
            try
            {
                await connection.CreateTableAsync<Card>();
                DatabaseInitiated?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                DatabaseInitiated?.Invoke(this, false);
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task ClearDB()
        {
            try
            {
                await connection.CreateTableAsync<Card>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<Card>> GetCardsAsync()
        {
            try
            {
                return await connection.Table<Card>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<Card> GetCardBySetAndNumberAsync(int set, int number)
        {
            try
            {
                return await connection.Table<Card>().FirstOrDefaultAsync(x => x.SetNumber == set && x.NumberAsInt == number);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task AddOrReplaceCardAsync(Card card)
        {
            try
            {
                Card c = ConvertCard(card);
                c.ID = c.ConstructKey();
                await connection.InsertOrReplaceAsync(c);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private Card ConvertCard(Card card)
        {
            if (card is AdjustableCard adjCard)
            {
                return JsonConvert.DeserializeObject<Card>(JsonConvert.SerializeObject(adjCard));
            }
            return card;
        }

        internal async Task DeleteCardAsync(Card card)
        {
            try
            {
                Card c = ConvertCard(card);
                await connection.DeleteAsync(c);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
