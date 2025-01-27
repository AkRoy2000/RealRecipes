using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core.Platform;
using System;
using RealRecipes.Data;
using RealRecipes.Models;


namespace RealRecipes.Views
{
#if ANDROID
    [XamlCompilation(XamlCompilationOptions.Compile)]
#endif
    public partial class TodoListPage : ContentPage
    {
        // dark mode property
        private AppTheme darkmode = AppTheme.Dark;
        public TodoListPage()
        {
            InitializeComponent();
            todolistlist = listView;

        Application.Current.RequestedThemeChanged += (s, a) =>
            {
                if (Application.Current.RequestedTheme == darkmode)
                {
                    // set searchbar background color to DarkGH from Colors.xaml
                    SearchContainer.BackgroundColor = (Color)Application.Current.Resources["DarkGH"];
                }

            };
        }

        public ListView todolistlist;

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateListView();
            await GetItemsWithImage();
            await UpdateListView();
            await IsEmptyList();
        }

        public bool EmptyList { get; set; }
        private async Task IsEmptyList()
        {
            RecipeDatabase database = await RecipeDatabase.Instance;
            var items = await database.GetItemsAysnc();
            if (items.Count == 0)
            {
                VStack.IsVisible = true;
                listView.IsVisible = false;
                Console.WriteLine("List is empty");
            }
            else
            {
                listView.IsVisible = true;
                VStack.IsVisible = false;
                Console.WriteLine("List is not empty");
            }
        }

        private async Task GetItemsWithImage()
        {
            RecipeDatabase database = await RecipeDatabase.Instance;
            var items = await database.GetItemsAysnc();

            foreach (var item in items)
            {
                if (item.Image != null)
                {
                    item.HasImage = true;
                    await database.SaveItemAsync(item);
                }

                else
                {
                    item.HasImage = false;
                    await database.SaveItemAsync(item);
                }
            }
        }

        private async Task UpdateListView()
        {
            RecipeDatabase database = await RecipeDatabase.Instance;
            listView.ItemsSource = await database.GetItemsAysnc();
            UpdateTitle();
            Console.WriteLine("Listview Refreshed");
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);

            await Navigation.PushAsync(new TodoitemPage
            {
                BindingContext = new MakeRecipe()
            });
        }

        async void OpenSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }

        async void DeleteAllItems(object sender, EventArgs e)
        {
            bool userConfirmed = await DisplayAlert("Delete All Recipes", "Warning! Confirm you want to DELETE ALL Recipes?", "Yes", "No");

            if (userConfirmed)
            {
                RecipeDatabase database = await RecipeDatabase.Instance;
                var allitems = await database.GetItemsAysnc();

                foreach (var item in allitems)
                {
                    await database.DeleteItemAsync(item);
                }

                listView.ItemsSource = null;
                await IsEmptyList();
                await UpdateListView();
            }
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var todoItem = (MakeRecipe)BindingContext;
            RecipeDatabase database = await RecipeDatabase.Instance;
            await database.DeleteItemAsync(todoItem);
            await UpdateListView(); // Update ListView after deleting item.
            await Navigation.PopAsync();
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new TodoitemPage
                {
                    BindingContext = e.SelectedItem as MakeRecipe
                });
            }
        }

        private void UpdateTitle()
        {
        
            Title = $"Real Recipes";
            labeltitle.Text = $"Real Recipes";
        }
        private void OnCheckBoxChecked(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var todoItem = checkBox.BindingContext as MakeRecipe;
            if (todoItem != null)
            {
                todoItem.IsSelected = checkBox.IsChecked;
            }
        }

        private void OnCheckBoxUnchecked(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var todoItem = checkBox.BindingContext as MakeRecipe;
            if (todoItem != null)
            {
                todoItem.IsSelected = checkBox.IsChecked;
            }
        }
        async void DeleteSelectedItems(object sender, EventArgs e)
        {
            var selectedItems = listView.ItemsSource?.Cast<MakeRecipe>().Where(item => item.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                await DisplayAlert("Error: No Recipes Selected", "Please select items to delete", "OK");
            }
            else
            {
                bool Confirmed = await DisplayAlert("Delete Selected Tasks", "Warning! Are you sure you want to delete all selected recipes?", "Yes", "No");

                if (Confirmed)
                {
                    RecipeDatabase database = await RecipeDatabase.Instance;
                    foreach (var item in selectedItems)
                    {
                        await database.DeleteItemAsync(item);
                    }

                    // Refresh ListView
                    await IsEmptyList();
                    await UpdateListView();
                }
            }
        }
        public async void OpenMenu(object sender, EventArgs e)
        {
            string deleteall = "Delete all Recipes";
            string settings = "Settings";

            var action = await Application.Current.MainPage.DisplayActionSheet(null, "Return", null, new[] { deleteall, settings });

            if (action != null && action.Equals(deleteall))
            {
                DeleteAllItems(sender, e);
            }
            else if (action != null && action.Equals(settings))
            {
                OpenSettings(sender, e);
            }
        }
        //Searching
        async void SearchBar_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            var keyword = SearchBar.Text.ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Empty searchbar > show all
                listView.ItemsSource = ((IEnumerable<MakeRecipe>)listView.ItemsSource).ToList();
                await UpdateListView();
            }
            else
            {
                // Filter items based on the keyword
                var filteredItems = ((IEnumerable<MakeRecipe>)listView.ItemsSource)
                    .Where(item => item.Name.ToLower().Contains(keyword));

                // Update ListView with filtered items
                listView.ItemsSource = filteredItems.ToList();
            }
        }
        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            listView.IsRefreshing = true;
            await UpdateListView();
            await Task.Delay(1000); // Temporary fix, sometimes the refresh spinner doesn't disappear.
            listView.IsRefreshing = false;

        }


    }
}