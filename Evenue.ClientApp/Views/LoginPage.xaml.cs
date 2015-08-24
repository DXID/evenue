using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using Windows.Security.Credentials;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Evenue.ClientApp.Models;
using System.Diagnostics;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Evenue.ClientApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public static string provider;
        public static MobileServiceUser user = null;
        private IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();

        public LoginPage()
        {
            this.InitializeComponent();

            var view = ApplicationView.GetForCurrentView();

            // Changing the background and the foreground color
            view.TitleBar.BackgroundColor = Color.FromArgb(255, 0, 120, 215);
            view.TitleBar.ForegroundColor = Colors.White;

            // Changing other components such as color for button states
            view.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 0, 120, 215);
            view.TitleBar.ButtonForegroundColor = Colors.White;

            view.TitleBar.ButtonHoverBackgroundColor = Colors.Blue;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;

            view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 0, 120, 0);
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;

            view.TitleBar.ButtonInactiveBackgroundColor = Colors.DarkGray;
            view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;

            view.TitleBar.InactiveBackgroundColor = Colors.DarkBlue;
            view.TitleBar.InactiveForegroundColor = Colors.Gray;
        }

        // Log the user in with specified provider (Microsoft Account or Facebook)
        private async Task AuthenticateAsync()
        {
            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential = vault.FindAllByResource(provider).FirstOrDefault();
            }
            catch (Exception)
            {
                // do nothing
            }

            if (credential != null)
            {
                // Create a user from the stored credentials.
                user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                App.MobileService.CurrentUser = user;

                try
                {
                    // Try to return an item now to determine if the cached credential has expired.
                    await App.MobileService.GetTable<Event>().Take(1).ToListAsync();
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Remove the credential with the expired token.
                        vault.Remove(credential);
                        credential = null;
                    }
                }
            }
            else
            {
                try
                {
                    // Login with the identity provider.
                    user = await App.MobileService.LoginAsync(provider);

                    // Create and store the user credentials.
                    credential = new PasswordCredential(provider,
                        user.UserId, user.MobileServiceAuthenticationToken);
                    vault.Add(credential);
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }
        
        // Event handler for login button, try to login when clicked
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            provider = (sender as Button).Name;

            // Hide the buttons and show a progress ring
            LoginProgress.Visibility = Visibility.Visible;
            MicrosoftAccount.Visibility = Visibility.Collapsed;
            Facebook.Visibility = Visibility.Collapsed;

            // Try to log the user in
            try
            {
                await AuthenticateAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

            if (user != null)
            {
                try
                {
                    User newuser = new User() { Id = user.UserId };
                    await userTable.InsertAsync(newuser);
                }
                catch (Exception)
                {
                    // User already exist, do nothing
                }

                AppShell shell = Window.Current.Content as AppShell;

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (shell == null)
                {
                    // Create a AppShell to act as the navigation context and navigate to the first page
                    shell = new AppShell();

                    // Set the default language
                    shell.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                }

                // Place our app shell in the current Window
                Window.Current.Content = shell;

                if (shell.AppFrame.Content == null)
                {
                    // When the navigation stack isn't restored, navigate to the first page
                    // suppressing the initial entrance animation.
                    shell.AppFrame.Navigate(typeof(EventList), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
                }

                Window.Current.Activate();
            }
            else
            {
                LoginProgress.Visibility = Visibility.Collapsed;
                MicrosoftAccount.Visibility = Visibility.Visible;
                Facebook.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            user = null;
        }
    }
}
