using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    public class Child 
    {
        [Key]
        public int ChildID { get; set; }
        public int MemberID { get; set; } //foreign key property
        public int AdressID { get; set; } //foreign key property
        public Member Member { get; set; } = new Member (); //Navigation Property in child class
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Mother_Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Cell_Phone { get; set; }
        public string Home_Phone { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
        public string Job { get; set; }
        public string Gender { get; set; }   
        public string Grade { get; set; }
        public string Birth_Place { get; set; }
        public string US_Arrival_Date { get; set; }
        public string Member_Start_Date { get; set; }
        public string Member_End_Date { get; set; }
    }
}




 


 


 
 
