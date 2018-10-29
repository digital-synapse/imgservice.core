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
    public class ImageGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime Updated { get; set; }

        public virtual ICollection<ImageRecord> Images { get; set; }

        /*
        [JsonIgnore]
        public string TagData { get; set; }

        [NotMapped]
        public string[] Tags
        {
            get { return TagData?.Split(' '); }
            set { TagData = string.Join(' ', value); }
        }
        */

        [JsonIgnore]
        public ICollection<TagLink> TagLinks { get; set; }

        [NotMapped]
        public string[] Tags { get; set; }

    }
}
