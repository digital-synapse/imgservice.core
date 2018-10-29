using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lazyimgservice.core.Models
{
    public enum ImageSize
    {
        src = 0,
        h2048 = 2048,
        h1024 = 1024,
        h720 = 720,
        h480 = 480,
        h240 = 240,
        h128 = 128,
        h64 = 64,
        h32 = 32
    }
    public class ImageRecord
    {
        public ImageRecord()
        {
            Updated = DateTime.UtcNow;
        }
        public int Width { get; set; }
        public int Height { get; set; }

        [JsonIgnore]
        [Key]
        public int Id { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public string Description { get; set; }

        [JsonIgnore]
        public bool Active { get; set; }
        [JsonIgnore]
        public DateTime Updated { get; set; }

        [NotMapped]
        public string Url { get; set; }

        [JsonIgnore]
        [ForeignKey("File")]
        public int FileId { get; set; }

        [JsonIgnore]
        public virtual FileRecord File { get; set; }
        
        [JsonIgnore]
        public int ImageGroupId { get; set; }

        [JsonIgnore]
        public virtual ImageGroup ImageGroup { get; set; }

        [JsonIgnore]
        public ImageSize ImageSize { get; set; }
        
    }
}
