using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project1.Models
{
    public class Category
    {
        [Key]
        public int Category_ID { get; set; }
        [Required]
        public string Name { get; set; }
        // Weight cannot be less than zero
        [Range(0, 10000)]
        [Required]
        public int Min { get; set; }
        [Required]
        public int Max { get; set; }
        //[Required]
        public byte[] PhotoFile { get; set; }
        public string ImageMimeType { get; set; }
        //Configure one to many relationship
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
