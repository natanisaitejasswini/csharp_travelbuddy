using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travelbuddy.Models
{
    public abstract class BaseEntity {}

    public class User : BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string first_name { get; set; }
            [Required]
            [MinLength(1)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string last_name { get; set; }
            [Required]
            [EmailAddress]
            public string email{ get; set; }
            [Display(Name = "Birthday Date")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessage = "Birthday is mandatory")]
            public DateTime birthday{ get; set; }
            [Required]
            [MinLength(3)]
            public string password { get; set; }
            [Required]
            [Compare("password")]
            public string confirm_password {get; set;}
            public DateTime created_at;
            public DateTime updated_at;
        }
        public class Trip :  BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            public string destination { get; set; }
            [Required]
            [MinLength(2)]
            public string plan {get; set;}
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessage = "TravelStart is mandatory")]
            public DateTime travelstart {get; set;}
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [Required(ErrorMessage = "TravelEnd is mandatory")]
            public DateTime travelend {get; set;}
            public DateTime created_at;
            public DateTime updated_at;
            public string first_name {get; set;}
            public int user_id {get; set;}
        }
        public class Traveller : BaseEntity
        {
            public int id;
            public int user_id {get; set;}
            public int trip_id {get; set;}
        }
}