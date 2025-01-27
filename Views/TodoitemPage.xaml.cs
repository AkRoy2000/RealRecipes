using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RealRecipes.Data;
using RealRecipes.Models;

namespace RealRecipes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoitemPage : ContentPage
    {
        public TodoitemPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            UpdateToolbarTitle();
        }

        private void UpdateToolbarTitle()
        {
            if (string.IsNullOrWhiteSpace(NameField.Text)) //header for new recipe
            {
                itemTitle.FormattedText = new FormattedString();
                itemTitle.FormattedText.Spans.Add(new Span { Text = "Create new Recipe", FontAttributes = FontAttributes.Bold });
            }
            else //edit header for existing recipe
            {
                itemTitle.FormattedText = new FormattedString();
                itemTitle.FormattedText.Spans.Add(new Span { Text = "", FontAttributes = FontAttributes.Bold });
                itemTitle.FormattedText.Spans.Add(new Span { Text = NameField.Text });
            }
        }

        private async void DeleteImageClicked(object sender, EventArgs e)
        {
            var todoItem = (MakeRecipe)BindingContext;
            RecipeDatabase database = await RecipeDatabase.Instance;
            // if null
            if (todoItem.Image == null)
            {
                await DisplayAlert("Error: No image found", "There is no image to be deleted!", "OK");
                return;
            }

            // Alert

            bool Confirmed = await DisplayAlert("Delete Image.", "Are you sure you want to delete this image?", "Yes", "No");

            if (Confirmed)
            {
                todoItem.Image = null;
                imagelabel.IsVisible = true;
                attachmentImage.Source = null;

                // Save the updated todoItem to the database
                await database.SaveItemAsync(todoItem);
            }
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameField.Text) || string.IsNullOrWhiteSpace(MethodField.Text) || string.IsNullOrWhiteSpace(IngField.Text))
            {
                HapticFeedback.Perform(HapticFeedbackType.LongPress);
                await DisplayAlert("Error: Cannot Create Recipe", "Recipe name, ingredients, and method must be filled", "OK");
                return;
            }

            if (NameField.Text.Length < 3 || MethodField.Text.Length < 3 || IngField.Text.Length < 3)
            {
                HapticFeedback.Perform(HapticFeedbackType.LongPress);
                await DisplayAlert("Error: Cannot Create Recipe", "Recipe name, ingredients, and method must be at least 3 characters long", "OK");
                return;
            }

            var todoItem = (MakeRecipe)BindingContext;
            todoItem.Name = NameField.Text;
            todoItem.Ingredients = IngField.Text;
            todoItem.Method = MethodField.Text;

            HapticFeedback.Perform(HapticFeedbackType.Click);
            RecipeDatabase database = await RecipeDatabase.Instance;
            await database.SaveItemAsync(todoItem);
            await Navigation.PopAsync();
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
            bool Confirmed = await DisplayAlert("Delete Recipe.", "Are you sure you want to delete this recipe?", "Yes", "No");

            if (Confirmed)
            {
                HapticFeedback.Perform(HapticFeedbackType.Click);
                var todoItem = (MakeRecipe)BindingContext;
                RecipeDatabase database = await RecipeDatabase.Instance;
                await database.DeleteItemAsync(todoItem);
                await Navigation.PopAsync();
            }
        }

        public async void TakePhoto(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, photo.FileName);

                    using (Stream sourceStream = await photo.OpenReadAsync())
                    {
                        using (FileStream localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        }
                    }

                    // show the photo in the UI
                    var todoItem = (MakeRecipe)BindingContext;
                    RecipeDatabase database = await RecipeDatabase.Instance;
                    todoItem.Image = File.ReadAllBytes(localFilePath);

                    await Task.Delay(1000);

                    attachmentImage.Source = ImageSource.FromStream(() => new MemoryStream(todoItem.Image));

                    imagelabel.IsVisible = false;
                }
            }
        }

        public async void UploadPhoto(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult uploadedImage = await MediaPicker.Default.PickPhotoAsync();

                if (uploadedImage != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, uploadedImage.FileName);

                    using (Stream sourceStream = await uploadedImage.OpenReadAsync())
                    {
                        using (FileStream localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        }
                    }

                    // show the photo in the UI
                    var todoItem = (MakeRecipe)BindingContext;
                    RecipeDatabase database = await RecipeDatabase.Instance;
                    todoItem.Image = File.ReadAllBytes(localFilePath);

                    Task.Delay(1000);

                    attachmentImage.Source = ImageSource.FromStream(() => new MemoryStream(todoItem.Image));

                    imagelabel.IsVisible = false;
                }
            }
        }

        public async void OpenMenu2(object sender, EventArgs e)
        {
            string addImage = "Take Image";
            string uploadImage = "Upload Image";
            string delete = "Delete Recipe";

            var action = await Application.Current.MainPage.DisplayActionSheet(null, "Cancel", null, new[] { addImage, uploadImage, delete });

            if (action != null && action.Equals(delete))
            {
                OnDeleteClicked(sender, e);
            }
            else if (action != null && action.Equals(addImage))
            {
                TakePhoto(sender, e);
            }
            else if (action != null && action.Equals(uploadImage))
            {
                UploadPhoto(sender, e);
            }
        }
    }
}