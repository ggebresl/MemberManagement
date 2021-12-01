using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    /*Author: Gerawork Gebreslassie                  date: 01/25/2021
    * Create Property names corresponding to the Role table colums
    */
    public class Role
    {
     //   [Key]
        public int RoleID { get; set; }
        public int LoginID { get; set; }
        public string RoleType { get; set; }
       
    }
}
