using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        //public string RoleId { get; set; } //use ViewBag to strore RoleId from the controller to the view
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
