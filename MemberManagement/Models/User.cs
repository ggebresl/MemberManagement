using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace MemberManagement.Models
{
    public class User : IdentityUser
    {
        //Inherits all IdentifyUser Properties
        [NotMapped] 
        public IList<string> RoleNames { get; set; }
        [NotMapped]
        public string Firstname { get; set; }
        [NotMapped]
        public string Lastname { get; set; }
    }
}
