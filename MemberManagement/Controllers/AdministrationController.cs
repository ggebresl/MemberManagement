using MemberManagement.Models;
using MemberManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;



namespace MemberManagement.Controllers
{
    //[Authorize] //this is simnple log in. It Checks if the user is logged in
     //[Authorize(Roles = "Admin,Engineer")] //log in member can be either  "Admin" or "Engineer" roles for the entire controler and Acttion methods
     //uncomment the above code 
     [Authorize(Roles = "Admin")] // authorizes at the control and methods level
    //   [Authorize(Policy = "EditRolePolicy")]
     // [Authorize(Policy = "AdminRolePolicy")]
       
     //  [Authorize(Roles = "Admin")]   // login users must be in both roles ("Admin" and ("user")
    //   [Authorize(Roles = "Member")]
    //   [Authorize(Roles = "user")]
        

    public class AdministrationController : Controller
    {

        private readonly Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private ILogger<AdministrationController> logger { get; }
        private AppDbContext _context { get; set; }

      //  private readonly AppDbContext _context;

        public AdministrationController(Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager,
                                        Microsoft.AspNetCore.Identity.UserManager<User> userManager,
                                        ILogger<AdministrationController> logger,
                                        AppDbContext ctx)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this._context = ctx;

        }

        //Add Google search funcgionality
        [AllowAnonymous]
        public IActionResult GoogleSearchAPI()   
        {
           // string serachQuery = Request["Search"];
            
           // var request = Request.Crearte
         

            return View();
        }

      //  [AllowAnonymous]
        public IActionResult ListUsers()  //Retrieve all registered users in AspNetUsers
        {
      

            if (!User.IsInRole("Admin"))
            {

                 return RedirectToAction("index", "payment"); // make see only his only his payment not all user payments 
            }

            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var usrClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Claims = usrClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = (List<string>)userRoles
            };

            return View(model);


        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                // Get userLogin object  using  username and phone fields
                var userLogin = _context.userlogin.Where(u => u.Username == user.UserName &&
                                                        u.Phone == user.PhoneNumber).FirstOrDefault();
                //update user object with corresponding fields from model object
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.Phone;

                // Then update userLogin object corresponding values 
                userLogin.Username = model.UserName;
                userLogin.Email = model.Email;
                if (!string.IsNullOrWhiteSpace(model.Phone))
                {
                    userLogin.Phone = model.Phone;
                }
                if ((userLogin != null) && (userLogin.Phone == user.PhoneNumber))
                {
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        _context.userlogin.Update(userLogin);
                        _context.SaveChanges();
                        return RedirectToAction("ListUsers");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }
                return View(model);
            }
        }

        [HttpGet]
     //  [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRolesViewModel>();
            foreach(var role in roleManager.Roles)
            {
                var userRoelsViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if ( await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoelsViewModel.IsSelected = true;
                }
                else
                {
                    userRoelsViewModel.IsSelected = false;
                }
                model.Add(userRoelsViewModel);
            }
            return View(model);
        }

        [HttpPost]
       // [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = userId });

        }


        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = { userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                     ClaimType = claim.Type
                };
                //If the user has the claim, set isSelected property to true, so the checkbox
                //next to the claim is cheked out on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);

            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");

            }
            
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimAsync(user, (Claim)claims);


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }
            result = await userManager.AddClaimAsync(user,
                (Claim)model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" :"false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = model.UserId });
        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.IdentityRole identityRole = new Microsoft.AspNetCore.Identity.IdentityRole
                {
                    Name = model.RoleName
                };
                Microsoft.AspNetCore.Identity.IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                  
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);

        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult DeleteRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role {ex}");

                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users " +
                        $"in this role. If you want to delete this role, please remove the users from " +
                        $"the role and the delete";

                    return View("Error2");
                }
            }
        }

        [HttpGet]
      // [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);


        }

        [HttpPost]
       // [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
           ViewBag.roleId = roleId; //so we pass it to the View
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {roleId} cannot be found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                Microsoft.AspNetCore.Identity.IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);

                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });

                }
            }
            return RedirectToAction("EditRole", new { Id = roleId});

        }

        [HttpGet]

        [HttpGet]
        public IActionResult DeleteUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} cannot be found";
                return View("NotFound");
            }
            else
            {
               try
                {
                //First remove userLogin whith corresponding value for username and phone
                var userLogin = _context.userlogin.Where(u => u.Username == user.UserName && u.Phone == user.PhoneNumber).FirstOrDefault();

                if (userLogin != null)
                {
                    _context.userlogin.Remove(userLogin);
                    _context.SaveChanges();
                }
                var result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListUsers");
                }
               
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role {ex}");

                    ViewBag.ErrorTitle = $"{user.UserName} user is in use";
                    ViewBag.ErrorMessage = $"{user.UserName} user cannot be deleted as there are users " +
                        $"in this user. If you want to delete this user, please remove the users from " +
                        $"the userr and the delete";

                    return View("Error2");
                }
            }
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
    }
}
