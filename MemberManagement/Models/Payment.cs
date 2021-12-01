using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    /*Author: Gerawork Gebreslassie             date: 01/25/2021
 * Create Property names corresponding to the Payment table colums
 */
    public class Payment
    {
       [Key]
        public int PaymentID { get; set; }
        public int? LoginID { get; set; }
        public int? AdressID { get; set; } //foreign key property

        [Required(ErrorMessage = "Please enter First Name.")]
        public string First_Name { get; set; }

        [Required(ErrorMessage = "Please enter Last Name.")]
        public string Last_Name { get; set; }

        public string Spouse_Name { get; set; }

        [Required(ErrorMessage = "Please enter phone.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter amount.")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Please enter payment reason.")]
        public string Payment_Reason { get; set; }

        [Required(ErrorMessage = "Please enter payment type")]
        public string Payment_Type { get; set; }

         
        public string Receipt_Number { get; set; }

        [Required(ErrorMessage = "Please enter name of the person who gives a receipt.")]
        public string ReceivedBy { get; set; }

        [Required(ErrorMessage = "Please enter payment date.")]
        public DateTime? Payment_Date { get; set; }

       
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter yes or no.")]
        public string IsMember { get; set; }

        [ForeignKey("LoginID")]
        public  Userlogin Userlogin { get; set; }
    }
}
