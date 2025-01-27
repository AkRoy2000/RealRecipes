using RealRecipes;
using RealRecipes.Views;
using RealRecipes.Data;
using RealRecipes.Models;

namespace RealRecipes;

public partial class Settings : ContentPage
{
    public bool IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;
    public Settings()
	{
        InitializeComponent();

        if (IsDarkMode)
        {
            DarkModeSwitch.IsToggled = true;
        }
        else
        {
            DarkModeSwitch.IsToggled = false;
        }
    }

    protected override void OnAppearing()
    {
#if IOS
        UpdateTitle();
#endif
        base.OnAppearing();
    }

    private void UpdateTitle() //ios stuff
    {
        TodoListPage todolistapge = new TodoListPage();
        ListView listView = todolistapge.todolistlist;

        int totalItems = listView.ItemsSource?.Cast<object>().Count() ?? 0;
        string task = "Task";

        if (totalItems != 1)
        {
            task += "s";
        }
        Title = $"Real Recipies";
    }

    // Dark Mode SWITCH
    private void DarkMode(object sender, EventArgs e) //light to dark broken
    {
        try
        {
            if (DarkModeSwitch.IsToggled)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }
        catch (Exception ex)
        {
            // Handle ex
            Console.WriteLine($"An error occurred while toggling dark mode: {ex.Message}");
            // Revert Switch
            DarkModeSwitch.IsToggled = !DarkModeSwitch.IsToggled;
            // Error message
            DisplayAlert("Error", "An error occurred while toggling dark mode.", "Cancel");
        }
    }

    private static async Task MakeSampleData() //ok
        {
            RecipeDatabase database = await RecipeDatabase.Instance;

            for (int i = 0; i < 6; i++)
            {
                var item = new Models.MakeRecipe
                {
                    Name = $"Recipe {i + 1}",
                    Ingredients = $"Ingriedients {i + 1}",
                    Method = $"Ingriedients {i + 1}",
                };

                await database.SaveItemAsync(item);
            }

            // await UpdateListView();
        }

    private async void Generate_Sample_Data_Button_Pressed(System.Object sender, System.EventArgs e) //ok
    {
        // Alert
        var userConfirmed = await DisplayAlert("Generate Sample Data", "This action will generate sample data for testing. Are you sure you want to continue", "Yes", "No");

        await Settings.MakeSampleData();

        if (userConfirmed)
        {
            await DisplayAlert("Success", "Sample test data has been generated", "OK");
        }
    }
}