using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    //UserLogin Entity to be linked

    public class Userlogin
    {
        /*Author: Gerawork Gebreslassie       date: 01/25/2021
    * Create Property names corresponding to the to the Username table colums
    */
        [Key]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoginID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
       // public List<Payment> Payments { get; set; }

        //navigation property to linking entity
       // public ICollection<MemberAddress> MemberAddresses { get; set; }


    }
}
