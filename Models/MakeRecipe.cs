using SQLite; // Import SQLite library for database operations
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRecipes.Models
{
    // Define a class to represent a recipe 
    public class MakeRecipe
    {
        // Define properties for the recipe 

        // Primary key for the database, automatically incremented
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        // Name of the recipe 
        public string Name { get; set; }

        // Ingredients needed for the recipe  (e.g., for cooking)
        public string Ingredients { get; set; }

        // Method to make the recipe
        public string Method { get; set; }

        // Flag indicating whether the todo item is selected (e.g., for UI purposes)
        public bool IsSelected { get; set; }

        // Byte array to store image data of the todo item (if applicable)
        public byte[] Image { get; set; }

        // Flag indicating whether the todo item has an associated image
        public bool HasImage { get; set; }
    }
}
