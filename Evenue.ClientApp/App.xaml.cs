using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Evenue.ClientApp.Views;
using Microsoft.WindowsAzure.MobileServices;

namespace Evenue.ClientApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        // Set the mobile service url
        public static MobileServiceClient MobileService =
            new MobileServiceClient(
                "https://evenuebackendapi-code.azurewebsites.net",
                "https://evenuebackendapi6dd2d0d6d39b4996904d8c524ab5462f.azurewebsites.net",
                "cpMWMNajUuZNrqgHZBxDvpDOVoMaWE88"
            );
        // Data Source=tcp:sfjd8875i4.database.windows.net,1433;Initial Catalog=EvenueBackEndAPI_db;User ID=evenue@sfjd8875i4;Password=
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // This just gets in the way.
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Set the login page as the starting page
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(LoginPage));
            Window.Current.Content = rootFrame;
            LoginPage p = rootFrame.Content as LoginPage;

            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
