using lazyimgservice.core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace lazyimgservice.core.Controllers
{    
    public class ImageSet
    {
        [JsonProperty(Order = 1)]
        public int Id { get; set; }
        [JsonProperty(Order = 2)]
        public string Name { get; set; }
        [JsonProperty(Order = 3)]
        public string Description { get; set; }
        [JsonProperty(Order = 4)]
        public bool Active { get; set; }
        [JsonProperty(Order = 5)]
        public DateTime Updated { get; set; }
        [JsonProperty(Order = 6)]
        public string[] Tags { get; set; }

        [JsonProperty(Order = 7)]
        public virtual ImageRecord Image { get; set; }

        [JsonProperty(Order = 8)]
        public virtual ImageRecord Image_h2048 { get; set; }

        [JsonProperty(Order = 9)]
        public virtual ImageRecord Image_h1024 { get; set; }

        [JsonProperty(Order = 10)]
        public virtual ImageRecord Image_h720 { get; set; }

        [JsonProperty(Order = 11)]
        public virtual ImageRecord Image_h480 { get; set; }

        [JsonProperty(Order = 12)]
        public virtual ImageRecord Image_h240 { get; set; }

        [JsonProperty(Order = 13)]
        public virtual ImageRecord Image_h128 { get; set; }

        [JsonProperty(Order = 14)]
        public virtual ImageRecord Image_h64 { get; set; }

        [JsonProperty(Order = 15)]
        public virtual ImageRecord Image_h32 { get; set; }

        [JsonProperty(Order = 16)]
        public virtual ICollection<ImageRecord> Images { get; set; }
    }

    public class SummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
    }

    public class Info
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
    }
}
