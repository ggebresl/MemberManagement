using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MemberManagement.Models;


namespace MemberManagement.Controllers
{
   /* 
    [Authorize(Roles = "Admin")]
    [Area("Admin")] */

  
    public class UserController : Controller
    {
        private  UserManager<User> _userManager;
        private   RoleManager<IdentityRole> _rollManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> rollManager)
        {
            _userManager = userManager;
            _rollManager = rollManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<User> users = new List<User>();
            foreach (User user in _userManager.Users)
            {
                user.RoleNames = await _userManager.GetRolesAsync(user);
                users.Add(user);
            }
            UserViewModel model = new UserViewModel
            {
                Users = users,
                Roles = _rollManager.Roles
            };
            //   return View(model);
            return RedirectToAction("ChangePassword");
        }

       [HttpGet]
        public IActionResult Delete()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    string errorMessage = "";
                    foreach (IdentityError error in result.Errors)
                    {
                        errorMessage += error.Description + " | ";
                    }
                    TempData["message"] = errorMessage;
                }
            }
           return RedirectToAction("Index");
         }
        [HttpGet]
        public IActionResult ChangePassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var user = await _userManager.FindByNameAsync(model.Username);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model); // to User/ChangePassword view
        }
    }
}
