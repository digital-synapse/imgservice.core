using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lazyimgservice.core.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        //[Index(IsUnique = true)]  // not available in core?
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<TagLink> TagLinks { get; set; }
    }

    public class TagLink
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }

        [ForeignKey("File")]
        public int? FileId { get; set; }

        [ForeignKey("ImageGroup")]
        public int? ImageGroupId { get; set; }

        public virtual FileRecord File { get; set; }
        public virtual ImageGroup ImageGroup { get; set; }
        public virtual Tag Tag { get; set; }

    }
}
