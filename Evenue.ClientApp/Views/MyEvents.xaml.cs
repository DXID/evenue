using Evenue.ClientApp.Controls;
using Evenue.ClientApp.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Evenue.ClientApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyEvents : Page
    {
        private MobileServiceCollection<Event, Event> events;
        private IMobileServiceTable<Event> userEvents = App.MobileService.GetTable<Event>();                                

        public MyEvents()
        {
            this.InitializeComponent();
            RefreshEventList(null);
        }

        // Refresh the item list each time we navigate to this page
        private async void RefreshEventList(string query)
        {
            if (query == null)
            {
                try
                {
                    events = await userEvents.Where(tempevent => tempevent.UserId == App.MobileService.CurrentUser.UserId).ToCollectionAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    ErrorText.Text = "Connectivity error, please try again";
                    ErrorText.Visibility = Visibility.Visible;
                }
                finally
                {
                    ProgressRing.IsActive = false;
                    SearchBox.Visibility = Visibility.Visible;
                }
                eventListGridView.ItemsSource = events;
            }
            else
            {
                var MatchEvents = EventController.GetMatchingEvents(events, query).ToList<Event>();
                if (MatchEvents.Count > 0)
                {
                    ErrorText.Visibility = Visibility.Collapsed;
                    eventListGridView.ItemsSource = MatchEvents;
                }
                else
                {
                    eventListGridView.ItemsSource = null;
                    ErrorText.Text = "No Events Found";
                    ErrorText.Visibility = Visibility.Visible;
                }
            }
        }

        // Navigate to Event information page and send over the Event object
        private void eventListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(
                 typeof(UpdateEvent),
                 e.ClickedItem,
                 new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        // Event handler when user is typing a search query, show a list of suggestions
        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //We only want to get results when it was a user typing
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var matchingEvents = EventController.GetMatchingEvents(events, sender.Text);

                RefreshEventList(sender.Text);
                sender.ItemsSource = matchingEvents.ToList();
            }
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = (args.SelectedItem as Event).ToString();
        }
    }
}
