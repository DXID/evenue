using Microsoft.Azure.Mobile.Server;
using System.Collections.Generic;

namespace Evenue.MobileAppService.DataObjects
{
    public class User : EntityData
    {
        public User()
        {
            Events = new List<Event>();
        }
        public ICollection<Event> Events { get; set; }
    }
}