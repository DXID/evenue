using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evenue.ClientApp.Models
{
    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string UserId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "startdate")]
        public string StartDate { get; set; }

        [JsonProperty(PropertyName = "enddate")]
        public string EndDate { get; set; }

        [JsonProperty(PropertyName = "desc")]
        public string Desc { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public int Fee { get; set; }

        [JsonProperty(PropertyName = "containerName")]
        public string ContainerName { get; set; }

        [JsonProperty(PropertyName = "resourceName")]
        public string ResourceName { get; set; }

        [JsonProperty(PropertyName = "sasQueryString")]
        public string SasQueryString { get; set; }

        [JsonProperty(PropertyName = "imageUri")]
        public string ImageUri { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1} ({2})", Title, Location, Category);
        }
    }
}
