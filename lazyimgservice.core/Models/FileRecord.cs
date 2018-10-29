using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lazyimgservice.core.Models
{
    public enum FileType
    {
        Unknown =0,
        Image =1
    }
    public class FileRecord
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string FilePath { get; set; }
        public string FileExt { get; set; }
        public decimal SizeKb { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FileType FileType { get; set; }

        [NotMapped]
        public string Url { get; set; }

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
