using Evenue.ClientApp.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evenue.ClientApp.Controls
{
    class EventController
    {
        // returns an ordered list of location that matches the search query
        public static IEnumerable<Event> GetMatchingEvents(MobileServiceCollection<Event, Event> eventlist, string query)
        {
            return eventlist
                .Where(c => c.Title.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) > -1 ||
                            c.Location.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) > -1 ||
                            c.Category.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) > -1)
                .OrderByDescending(c => c.Title.StartsWith(query, StringComparison.CurrentCultureIgnoreCase))
                .ThenByDescending(c => c.Location.StartsWith(query, StringComparison.CurrentCultureIgnoreCase))
                .ThenByDescending(c => c.Category.StartsWith(query, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
