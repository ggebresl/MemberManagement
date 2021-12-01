using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberManagement.Models
{
    /*Author: Gerawork Gebreslassie                  date: 01/25/2021
   * Create Property names corresponding to the Member table colums
   */
    // Member Entity to be linked
    #region  Model
    public class Member 
    {
        [Key]
        public int MemberID { get; set; }
        public int LoginID { get; set; } //foreign  key property
        public int AdressID { get; set; } //foreign key property
      //  public int? ChildID { get; set; } //foreign  key property

        [Required(ErrorMessage = "Please enter first name.")]
        public string First_Name { get; set; }

        public string Middle_Name { get; set; }

        [Required(ErrorMessage = "Please enter last name.")]
        public string Last_Name { get; set; }
        public string Gender { get; set; }
        public string Job { get; set; }
        public string Age { get; set; }

        public string Birth_Date { get; set; }
        public string Cell_Phone { get; set; }

        public string Home_Phone { get; set; }
        public string Work_Phone { get; set; }
        public string Fax { get; set; }

        public string Email { get; set; }
        public string Text { get; set; }
        public string Member_Status { get; set; }
        public string Marital_Status { get; set; }

        [Required(ErrorMessage = "Please enter your citizenship.")]
        public string Citizen { get; set; }

        [Required(ErrorMessage = "Please enter your birth place.")]
        public string Birth_Place { get; set; }

        public string US_Arrival_Date { get; set; }
        public string Member_Start_Date { get; set; }
        public string Member_End_Date { get; set; }

      //  public List<Child> Children { get; set; } = new List<Child>(); //navigation property

        //Navigation Property to linking entity
        // public ICollection<MemberAddress> MemberAddresses { get; set; } //= new(); //navigation property

    }
    #endregion
}




