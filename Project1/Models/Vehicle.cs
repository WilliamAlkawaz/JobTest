using System;
using System.ComponentModel.DataAnnotations;

namespace Project1.Models
{
    public enum Manufacturer
    {
        Mazda, Mercedes, Honda, Ferrari, Toyota
    }
    public class Vehicle
    {
        [Key]
        public int Vehicle_ID { get; set; }
        [Display(Name = "Owner's Name")]
        [Required]
        public string Owners_Name { get; set; }
        [Required]
        public Manufacturer Manufacturer { get; set; }
        [Display(Name = "Year of Manufacture")]
        [Required]
        public string Year_of_Manufacture { get; set; }
        [Display(Name = "Weight in Kilograms")]
        [Range(0, 10000)]
        [Required]
        public decimal Weight
        {
            get { return weight; }
            //Allow up to two decimal places
            set { weight = Math.Round(value, 2); }
        }
        decimal weight;
        //Configure one to many relationship 
        
        public Nullable<int> Category_ID { get; set; }
        public virtual Category Category { get; set; }
    }
}
