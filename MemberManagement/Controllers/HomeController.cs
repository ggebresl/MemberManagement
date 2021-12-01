using MemberManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MemberManagement.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MemberManagement.Controllers
{
    // [Authorize] // we can apply authorize at the contoll level so it can apply to all actions.
    //That means we have to login in order to use these actions
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       // private IUserloginRepository _userlogin;
        private AppDbContext _context;
       // private readonly IDbContextFactory<AppDbContext> _context;
    
        public IConfiguration _configuration { get; }


        public HomeController(AppDbContext ctx, ILogger<HomeController> logger, IConfiguration config)
        {
            _context = ctx;
            _logger = logger;
            _configuration = config;
           
           
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("login", "account");
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

