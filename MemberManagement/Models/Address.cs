using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    //Address Entity to be linked
    public class Address
    {
        [Key]
        public int AdressID { get; set; }
        // public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Please enter your street number and street name.")]
        public string Street_Addres { get; set; }
       
        public string Apt { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter your state")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter your zip code")]
        public string Zip_Code { get; set; }

        public bool IsPrimaryAddress { get; set; }

        [Required(ErrorMessage = "Please enter your country")]
        public string Country { get; set; }
        //Navigation Property to linking entity
       // public ICollection<MemberAddress> MemberAddresses { get; set; }
    }
}

