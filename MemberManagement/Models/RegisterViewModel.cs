using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MemberManagement.Models
{
    public class RegisterViewModel
    {
        
        [Required(ErrorMessage ="Please enter a username.")]
        [Remote(action: "IsUserNameInUse", controller: "Account")]
        public string Username { get; set; }

       // [Required]
        [EmailAddress]
        [Remote(action:"IsImailAddressInUse", controller:"Account")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Confirm password")]
        [Compare("Password",
            ErrorMessage ="Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Phone Number")]
        public string Phone { get; set; }
    }
}
