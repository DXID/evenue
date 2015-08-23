using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Evenue.ClientApp.Models;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Evenue.ClientApp.Views
{
    /// <summary>
    /// A page that can be used on navigated to within a Frame.
    /// </summary>
    public sealed partial class EventInfo : Page
    {
        private Event _event { get; set; }

        public EventInfo()
        {
            this.InitializeComponent();
            LocationMap.Loaded += LocationMap_Loaded;
        }

        // Set the map to location specified in the Event Class
        private async void LocationMap_Loaded(object sender, RoutedEventArgs e)
        {
            string address = _event.Location;

            MapLocationFinderResult result =
               await MapLocationFinder.FindLocationsAsync(address, null, 1);

            if(result.Status == MapLocationFinderStatus.Success)
            {
                double latitude, longitude;
                MapIcon icon = new MapIcon();
                icon.NormalizedAnchorPoint = new Point(0.5, 1.0);

                // Set try catch to make sure the latitude and longitude values aren't out of range
                try
                {
                    latitude = result.Locations[0].Point.Position.Latitude;
                    longitude = result.Locations[0].Point.Position.Longitude;
                    
                    icon.Location = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    });
                    icon.Title = address;

                    LocationMap.Center = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    });
                    LocationMap.ZoomLevel = 14;
                    LocationMap.MapElements.Add(icon);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace.ToString());
                    UnknownLocationText.Visibility = Visibility.Visible;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _event = (Event) e.Parameter;
            base.OnNavigatedTo(e);
        }
    }
}
