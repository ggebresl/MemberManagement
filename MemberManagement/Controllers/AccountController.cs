using MemberManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MemberManagement.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MemberManagement.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _context;
    
     //   private readonly string _tigrayComBostonUrl = "http://www.tigraycom.com/";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        public IConfiguration _configuration { get; }


        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 AppDbContext ctx,
                                 ILogger<AccountController> logger, 
                                 IConfiguration config)
        {

            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._context = ctx;
            this._logger = logger;
            _configuration = config;

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {

            return View();
        }
        //Client validation useing Remote attribute for email

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsImailAddressInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        //Client validation useing Remote attribute for username

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserNameInUse(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"username  {username} is already in use");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //Members could be registerd either by themselves or by Admins
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Firstname = model.FirstName,
                    Lastname = model.LastName,
                    PhoneNumber = model.Phone
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var newUser = new Userlogin  //save userlogin  info in this table, use the LoginID as unique idetifier for the Website. 
                    {
                        //LoginID = loginId,  //When turning the “IDENTITY INSERT OFF”, the “PRIMARY KEY ID” MUST NOT be PRESENT into the insert
                        Username = model.Username,
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Phone = model.Phone,
                        Email = model.Email
                    };

                    _context.userlogin.Add(newUser);
                    _context.SaveChanges();

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);



                    _logger.Log(LogLevel.Warning, confirmationLink);

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        // Members are registered by Admins
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    // return RedirectToAction("Index", "payment"); or the next line
                    /*  else
                      {
                          // Members are registerd themselves
                          return RedirectToAction("Welcome");
                      }*/
                    ViewBag.ErrorTitle = "Registration successful.";
                    ViewBag.ErrorMessage = "Before you Login, please confirm your email, by clicking " +
                        "on the confirmation link we have emailed to you.";
                    return View("Error");
                }
                else
                { 
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("index", "payment");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User ID {userId} is invalid";
                return View("Notfound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }
       

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // var model = new LoginViewModel { ReturnUrl = returnUrl };
           // ModelState.Clear();
            // return View(model);
            return View();

        }


        [HttpPost]
       [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
              //  var result = await _signInManager.PasswordSignInAsync(model.Username,model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                var result = await _signInManager.PasswordSignInAsync(model.Username,model.Password,isPersistent:false, false);

                var user = await _userManager.FindByNameAsync(model.Username);

                if (result.Succeeded && user !=null)
                {
                    TempData["Email"] = user.Email;
                    HttpContext.Session.SetString("username", user.UserName);
              

                    //aoutmate this 
                    int aspNetUersCount = _context.UserRoles.Count();

                    if (aspNetUersCount == 0)
                    {
                        
                      var loginUser = _context.userlogin.Where(u => u.LoginID <= 1);
                      if(loginUser != null)
                      {
                          var userId = user.Id;
                          var Role = await _roleManager.FindByNameAsync("Admin");
                          if(Role.Id != null)
                          {
                              var ur = new Microsoft.AspNetCore.Identity.IdentityUserRole<string>();
                              ur.UserId = user.Id;
                              ur.RoleId = Role.Id;
                              var result2 = _context.UserRoles.Add(ur);
                              _context.SaveChanges();
                          }
                      }
                    }

                    
                    return RedirectToAction("index", "payment");

                    /*
                    if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                       Url.IsLocalUrl(model.ReturnUrl))
                    {
                        ModelState.Clear();
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        ModelState.Clear();
                        return RedirectToAction("index", "payment");
                    } */
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid username/password.");
            ModelState.AddModelError(string.Empty, "Please try to login with valid username/password.");
            return View(model);
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
                if(user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        ViewBag.Message = "your password has been successfully changed";
                        return View("ChangePasswordSucess");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var email = new Email();
             email.To = model.Email;
            email.RoleType = "Member";
            email.Subject = "Reset Password";
            List<string> emailList = new List<string>();

            _logger.Log(LogLevel.Information, "ForgotPassword: start semding email at: " + DateTimeOffset.UtcNow);
            emailList.Add(email.To);

            
           if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = model.Email, token = token }, Request.Scheme);
                    email.Body = $"Clink on the provided link to reset your passwod: \n" +
                        passwordResetLink;
                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    try
                    {
                        await email.SendEmail(email, emailList, _configuration);
                    }
                    catch (Exception ex)
                    {
                        string emailHost = _configuration.GetSection("SMTP").GetSection("Host").Value;
                        string passWord = _configuration.GetSection("SMTP").GetSection("Password").Value;

                        _logger.Log(LogLevel.Error, ex.Message);
                        _logger.Log(LogLevel.Error, $"SendEmail: {emailHost},and {passWord} not accepted");
                        _logger.Log(LogLevel.Error, $"SendEmail: Email cannot be sent");
                    }
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");


            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();

        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user !=null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", "Invalid password reset token");
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Welcome()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            //return new RedirectResult (_tigrayComBostonUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}

