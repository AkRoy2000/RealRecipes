using RealRecipes.Views;
using Microsoft.Maui.Controls;

namespace RealRecipes
{
    public partial class App : Application
{
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TodoListPage());
        }
    }
}
