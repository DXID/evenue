using Microsoft.Azure.Mobile.Server;

namespace Evenue.MobileAppService.DataObjects
{
    public class Event : EntityData
    {
        public User User { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Desc { get; set; }
        public string Category { get; set; }
        public int fee { get; set; }

        // For storing images data
        public string ContainerName { get; set; }
        public string ResourceName { get; set; }
        public string SasQueryString { get; set; }
        public string ImageUri { get; set; }
    }
}