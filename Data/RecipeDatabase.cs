using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealRecipes.Models;

namespace RealRecipes.Data
{
    public class RecipeDatabase
    {
        static SQLiteAsyncConnection Database; // SQLite connection instance

        // Singleton pattern for database instance creation
        public static readonly AsyncLazy<RecipeDatabase> Instance =
            new AsyncLazy<RecipeDatabase>(async () =>
            {
                var instance = new RecipeDatabase();
                try
                {
                    // Create the table if it doesn't exist
                    CreateTableResult result = await Database.CreateTableAsync<MakeRecipe>();
                }
                catch (Exception)
                {
                    throw;
                }
                return instance;
            });

        // Constructor initializing the SQLite connection
        public RecipeDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        // Method to retrieve all items asynchronously
        public Task<List<MakeRecipe>> GetItemsAysnc()
        {
            return Database.Table<MakeRecipe>().ToListAsync();
        }

        // Method to retrieve a specific item asynchronously by its ID
        public Task<MakeRecipe> GetItemAsync(int id)
        {
            return Database.Table<MakeRecipe>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        // Method to refresh data asynchronously
        public Task<List<MakeRecipe>> RefreshDataAsync()
        {
            return Database.QueryAsync<MakeRecipe>("SELECT * FROM [TodoItem]");
        }

        // Method to save a new or existing item asynchronously
        public Task<int> SaveItemAsync(MakeRecipe item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        // Method to delete an item asynchronously
        public Task<int> DeleteItemAsync(MakeRecipe item)
        {
            return Database.DeleteAsync(item);
        }
    }
}
