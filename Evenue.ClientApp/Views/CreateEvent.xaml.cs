using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Evenue.ClientApp.Models;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Evenue.ClientApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateEvent : Page
    {
        private IMobileServiceTable<Event> eventTable = App.MobileService.GetTable<Event>();
        private StorageFile image = null;
        List<string> comboBoxItems = new List<string>()
        {
            "Music", "Food & Drink", "Sport", "Art & Culture", "Party", "Technology", "Other"
        };
       
        public CreateEvent()
        {
            this.InitializeComponent();
            categoryComboBox.ItemsSource = comboBoxItems;
        }

        // Validate all Fields
        private bool ValidateForm()
        {
            bool success = true;
            
            if (titleTextBox.Text == "")
            {
                titleTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                success = false;
            }
            else
            {
                titleTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
            }

            if (locationTextBox.Text == "")
            {
                locationTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                success = false;
            }
            else
            {
                locationTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
            }

            if (categoryComboBox.SelectedValue == null)
            {
                categoryComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                success = false;
            }
            else
            {
                categoryComboBox.BorderBrush = new SolidColorBrush(Colors.Black);
            }

            if (descTextBox.Text == "")
            {
                descTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                success = false;
            }
            else
            {
                descTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
            }

            if (feeTextBox.Text == "")
            {
                feeTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                success = false;
            }
            else
            {
                feeTextBox.BorderBrush = new SolidColorBrush(Colors.Black);
            }

            if (image == null)
            {
                BrowseButton.BorderBrush = new SolidColorBrush(Colors.Red);
                success = false;
            }
            else
            {
                BrowseButton.BorderBrush = new SolidColorBrush(Colors.Black);
            }

            if (!success)
            {
                ValidationErrorText.Visibility = Visibility.Visible;
            }

            return success;
        }

        // Reset all fields to empty box
        private void ResetFields()
        {
            titleTextBox.Text = "";
            locationTextBox.Text = "";
            startDatePicker.Date = System.DateTime.Now;
            endDatePicker.Date = System.DateTime.Now;
            descTextBox.Text = "";
            categoryComboBox.SelectedIndex = 0;
            feeTextBox.Text = "";
            image = null;


            // Set back image preview slot to no image
            Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage = 
                new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx://Evenue.ClientApp/Assets/noimage.jpg"));
            imagePreview.Source = bitmapImage;

            ValidationErrorText.Visibility = Visibility.Collapsed;
        }

        // Open the image picker
        private async void BrowseImage(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;

            // Set the file extensions
            picker.FileTypeFilter.Clear();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".png");

            image = await picker.PickSingleFileAsync();

            if (image != null)
            {
                // Open a stream for the selected file.
                Windows.Storage.Streams.IRandomAccessStream fileStream = await image.OpenAsync(Windows.Storage.FileAccessMode.Read);

                // Set the image source to the selected bitmap.
                Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();

                bitmapImage.SetSource(fileStream);
                imagePreview.Source = bitmapImage;
            }
        }

        // Insert event to database, clear all fields
        private async void InsertEvent(object sender, RoutedEventArgs e)
        {
            if(ValidateForm())
            {
                // Dismiss the save button, show progress ring
                SaveButton.Visibility = Visibility.Collapsed;
                progressRing.Visibility = Visibility.Visible;

                Event _event = new Event()
                {
                    UserId = App.MobileService.CurrentUser.UserId,
                    Id = Guid.NewGuid().ToString(),
                    Title = titleTextBox.Text,
                    Location = locationTextBox.Text,
                    StartDate = startDatePicker.Date.ToString("dd/MM/yyyy"),
                    EndDate = endDatePicker.Date.ToString("dd/MM/yyyy"),
                    Desc = descTextBox.Text,
                    Category = categoryComboBox.SelectedItem.ToString(),
                    Fee = Int32.Parse(feeTextBox.Text)
                };
                
                // Begin upload process
                string errorString = string.Empty;
                if (image != null)
                {
                    // Set blob properties of the event
                    _event.ContainerName = "eventimages";

                    // Use a unigue GUID to avoid collisions.
                    _event.ResourceName = Guid.NewGuid().ToString();
                }

                try
                {
                    await eventTable.InsertAsync(_event);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    MessageDialog msg = new MessageDialog("Problem occured, please retry again");
                    await msg.ShowAsync();
                }

                // If we have a returned SAS, then upload the blob
                if (!string.IsNullOrEmpty(_event.SasQueryString))
                {
                    // Get the URI generated that contains the SAS 
                    // and extract the storage credentials
                    StorageCredentials cred = new StorageCredentials(_event.SasQueryString);
                    var imageUri = new Uri(_event.ImageUri);

                    // Instantiate a Blob store container based on the info in the returned item
                    CloudBlobContainer container = new CloudBlobContainer(
                        new Uri(string.Format("https://{0}/{1}",
                            imageUri.Host, _event.ContainerName)), cred);

                    // Get the new image as a stream
                    using (var inputStream = await image.OpenReadAsync())
                    {
                        // Upload the new image as a BLOB from the stream
                        CloudBlockBlob blobFromSASCredential =
                            container.GetBlockBlobReference(_event.ResourceName);
                        await blobFromSASCredential.UploadFromStreamAsync(inputStream);
                    }
                    
                    MessageDialog msg = new MessageDialog("The Event is successfully created");
                    ResetFields();
                    await msg.ShowAsync();
                }
                else
                {
                    // Failed to upload image, delete the previously created event
                    await eventTable.DeleteAsync(_event);

                    MessageDialog msg = new MessageDialog("Problem occured, please retry again");
                    await msg.ShowAsync();
                }
                
                // Dismiss the progress ring, bring back the save button
                SaveButton.Visibility = Visibility.Visible;
                progressRing.Visibility = Visibility.Collapsed;
            }
        }
    }
}
