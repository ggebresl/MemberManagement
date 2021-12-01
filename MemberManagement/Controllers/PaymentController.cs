using MemberManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MemberManagement.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MemberManagement.Controllers
{
    public class PaymentController : Controller
    {
       
            private readonly ILogger<PaymentController> _logger;
            private  IWebHostEnvironment _web;
        
            private AppDbContext _context;
           // private readonly IDbContextFactory<AppDbContext> _context;

        public IConfiguration _configuration { get; }
          

            public PaymentController(AppDbContext ctx, ILogger<PaymentController> logger, IConfiguration config, IWebHostEnvironment web)
            {
                _context = ctx;
                _logger = logger;
                _configuration = config;
                _web = web;

                 
            }


        [HttpGet]
          public IActionResult Index()
           {

            IQueryable<Payment> payer = null;
            string email = TempData["Email"] as string;
            string username = HttpContext.Session.GetString("username");
           

            if (email != null) 
                payer = _context.payment.Where(e => e.Email == email);

            if (payer != null)
            {
                return View(payer);
            }

            if (username != null)
                payer = _context.payment.Where(e => e.Userlogin.Username == username);

            if (payer != null)
            {
                return View(payer);
            }

            else
            {
                payer = _context.payment.OrderBy(e => e.Email);
                return View(payer);
            }
                
  

        }

        [HttpGet]
       public IActionResult UploadFiles()
        {
            List<string> Folders = new List<string>() { "audio", "doc", "image", "video", "other" };
            ViewBag.message = Folders;
            return View();

       }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> filestoupload, Folder folder)
        {

            string selectedFolder = folder.FolderType;
            if (filestoupload.Count == 0 || selectedFolder == null)
            {
                ViewBag.Message = "Please click \"Choose Files\" button to select at least one file and then \r\n";
                ViewBag.Message += "select a folder type from the drop down menu (Select folder Name)";

                return View("SavedFiles");
            }

            string path = Path.Combine(this._web.WebRootPath, "Uploads");
            string folderTypePath = Path.Combine(path, selectedFolder);


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(folderTypePath))
            {
                Directory.CreateDirectory(folderTypePath);
            }

            try
            {
                foreach (IFormFile file in filestoupload)
                {
                    var saveFile = Path.Combine(folderTypePath, file.FileName);
                    var filesSelected = new FileStream(saveFile, FileMode.Create);
                    await file.CopyToAsync(filesSelected);

                   // ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                }
                if (filestoupload.Count == 1)
                {
                    ViewBag.Message = "Only " + filestoupload.Count + " file is successfully saved on the destination folder (" + folder.FolderType + ")";
                    return View("SavedFiles");
                }
                else if (filestoupload.Count > 1)
                {
                    ViewBag.Message = filestoupload.Count + " files are successfully saved on the destination folder (" + folder.FolderType + ")";
                    return View("SavedFiles");
                }
                else
                {
                    ViewBag.Message = filestoupload.Count + " or no file is saved on the destination folder (" + folder.FolderType + ")";
                    return View("SavedFiles");
                }
            }
            catch(Exception ex)
            {
                string error = ex.InnerException.ToString();
            }
            
            return View("SavedFiles");
        }
         
                       
            public IActionResult SendEmail()
            {
                List<string> emailRecevers = new List<string>()  { "All Members", "One Member" };
                ViewBag.message = emailRecevers;
                return View();
            }
            
            [HttpPost]
            public async Task<IActionResult> SendEmail(Email email)
            {
                string to = email.To;
                List<string> emailList = new List<string>();
                _logger.Log(LogLevel.Information,"start semding email at: " + DateTimeOffset.UtcNow);

               if(email.To == null && email.RoleType == null && email.Subject == null && email.Body == null)
                {
                     return RedirectToAction("index", "payment");

                }
            if (email.To == null && email.Subject == null && email.Body == null)
            {
                return RedirectToAction("index", "payment");

            }

            if (email.To == null && email.Body == null)
            {
                return RedirectToAction("index", "payment");

            }

            if (email.RoleType.ToLower().StartsWith("one"))
                {
                    if(email.To == null)
                    {
                        return RedirectToAction("index", "payment");
                    }
                     emailList = email.GetEmailList(_context.payment.Where(e => e.Email == email.To).ToList(), null);
                 }
                else
                {
                    emailList = email.GetEmailList(_context.payment.OrderBy(i => i.PaymentID).ToList(), email.RoleType);
                 }
                try
                {
                         await email.SendEmail(email, emailList, _configuration);
                 }
               
                 catch (Exception ex)
                 {
                        string emailHost = _configuration.GetSection("SMTP").GetSection("Host").Value;
                        string passWord = _configuration.GetSection("SMTP").GetSection("Password").Value;

                        _logger.LogError(ex.Message);
                        _logger.LogError($"SendEmail: Email Authentication Error for  {ex}");

                        ViewBag.ErrorTitle = $"SendEmail: {emailHost},and {passWord} not accepted";
                        ViewBag.ErrorMessage = $"SendEmail: Email cannot be sent ";



                        ViewBag.ErrorMessage = $"SendEmail: {emailHost} : Email cannot be sent. Check your email configuration";
                       _logger.LogWarning(ex.ToString(), DateTimeOffset.UtcNow);
                        return View("NotFound");
                 } 
                
                    return RedirectToAction("index", "payment");
            }

            [HttpGet]
            public IActionResult SendSmsText()
            {
                TwilioSms msg = new TwilioSms();
               // List<string> msgRecevers = new List<string>() { "Member", "All", "Admin", "Audio_Admin", "Video_Admin", "Image_Admin" };
                List<string> msgRecevers = new List<string>() { "All Members", "One Member" };
                ViewBag.message = msgRecevers;
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> SendSmsText(TwilioSms smsText)
            {
                TwilioSms msg = new TwilioSms();
                List<string> phoneList = new List<string>();

                msg.Text = smsText.Text;
                if (smsText.Phone == null && smsText.RoleType == null && smsText.Text == null)
                 {
                    return RedirectToAction("index", "payment");
                 }

                if (smsText.RoleType.ToLower().StartsWith("one"))
                {
                     phoneList = smsText.GetPhoneList(_context.payment.Where(p => p.Phone == smsText.Phone).ToList());
                }
                else
                {
                    phoneList = smsText.GetPhoneList(_context.payment.OrderBy(p => p.Phone).ToList());
                }

            //await msg.sendText(smsText.Text, smsText.GetPhoneList(_context.payment.OrderBy(i => i.PaymentID).ToList()), _configuration);

                await msg.sendText(smsText.Text, phoneList, _configuration);

                return RedirectToAction("index", "payment");
            }

        public ViewResult List()
          {
             ViewBag.Info = "List of All Payers";
             var payer = _context.payment.OrderBy(i => i.PaymentID);

                return View(payer);
            }
        public ViewResult ListMemberPayers()
        {
            ViewBag.Info = "List of member payers";
            var payer = _context.payment.Where(m => m.IsMember.ToLower().StartsWith("y")).ToList();
            return View("List", payer);
        }
                              
        public ViewResult ListNonMemberPayers()
        {
            ViewBag.Info = "List of non member Payers";
            var payer = _context.payment.Where(p => p.IsMember.ToLower().StartsWith("n")).ToList();
            return View("List",payer);
        }

            [HttpGet]
            public IActionResult Add()
            {
                ViewBag.Action = "Add";
                //return View("Edit", new Payment());
                return View("Add", new Payment());

        }

        [HttpPost]
        public IActionResult Add(Payment payer)
        {

            if (ModelState.IsValid)
            {
                // var lginUser = _context.userlogin
                if (payer.IsMember.ToLower().StartsWith("y"))
                {
                    
                    //The payer should have either Phone or Email or both
                    var loginUser = _context.userlogin.Where(p => p.Phone == payer.Phone || p.Email == payer.Email).FirstOrDefault();

                    /*
                    if (loginUser != null)
                    {
                        int loginID = loginUser.LoginID;
                        payer.LoginID = loginID;
                    }*/

                }
                if (payer.PaymentID == 0)
                    _context.payment.Add(payer);
                else
                    _context.payment.Update(payer);
                _context.SaveChanges();
                return RedirectToAction("Index", "payment");
            }
            else
            {
                ViewBag.Action = (payer.PaymentID == 0) ? "Add" : "Edit";
                return View(payer);
            }

        }
        [HttpGet]
        public IActionResult SearchTotalAmountBy()
        {
            List<string> searchTypes = new List<string>() { "Members", "Non Members", "by Date (MM/dd/yyyy)", "Year" };
            ViewBag.message = searchTypes;
            return View();

        }

        [HttpPost]
        public IActionResult SearchTotalAmountBy(Search selected)
        {

            var payer = new Payment();
            float total = 0;
            string info = string.Empty;
                
            switch (selected.SearchType)
            {
                //retrieve member object  from database based on user selection
                case "Members":
                    total = _context.payment.Where(p => p.IsMember.ToLower().StartsWith("y")).Sum(p => p.Amount);
                   
                    info = "Total Amount payed by members =  ";
                    ViewBag.Total = info + total.ToString(); 
                    return View("Total");
                    

                case "Non Members":     
                    total = _context.payment.Where(p => p.IsMember.StartsWith("n")).Sum(p => p.Amount);
                    info = "Total Amount payed by  non members =  ";
                    ViewBag.Total = info + total.ToString();
                    
                    return View("Total");

                case "by Date (MM/dd/yyyy)":
                    DateTime sqlServerDate;
                    DateTime.TryParseExact(selected.Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out sqlServerDate);
                    string selectedDate = sqlServerDate.ToString("yyyy-MM-dd");

                    total = _context.payment.Where(m => m.Payment_Date.ToString().Substring(0,10) == selectedDate).Sum(p => p.Amount);
                    info = $"Total Amount payed by all members on { selected.Text } =  ";
                    ViewBag.Total = info + total.ToString();
                    return View("Total");

                 case "Year":
                    total = _context.payment.Where(m => m.Payment_Date.ToString().Substring(0, 4) == selected.Text).Sum(p => p.Amount);
                    info = $"Total Amount payed by all members in  { selected.Text } =  ";
                    ViewBag.Total = info + total.ToString();
                    return View("Total");

                default:
                    total = _context.payment.OrderBy(m => m.Payment_Date.ToString().Substring(0, 4)).Sum(p => p.Amount);
                    info = $"Total Amount payed by all members in  { selected.Text } =  ";
                    ViewBag.Total = info + total.ToString();
                    return View("Total");
                    
            }
        }
        [HttpGet]
        public IActionResult Sort()
        {
        
            List<DropDownBy> listDropDown = new List<DropDownBy>();
            var result = _context.payment.GroupBy(p => p.First_Name)
                    .Select(g => new { payer_firstname = g.Key, total = g.Sum(i => i.Amount) });

            foreach (var group in result)
            {
                var li = new DropDownBy();
                li.payer_firstname = group.payer_firstname;
                li.Amount = group.total;
                li.SortType = group.payer_firstname;
                listDropDown.Add(li);
                li = null;
            }
           // ViewBag.SoryBy = listDropDown;

            return View(listDropDown);
        }

            [HttpGet]
            public IActionResult Edit(int id)
            {
                ViewBag.Action = "Edit";
                var payer = _context.payment.Find(id);
                return View(payer);
            }
        [HttpPost]
        public IActionResult Sort(List<DropDownBy> dd)
         {
          
            foreach(var v in dd)
            {
                string firstNasme = v.payer_firstname;
                float amount = v.Amount;
                string sortType = v.SortType;

            }
            
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "payment");
            }
            else
            {
                return View(dd);
            }
            
        }


      
        [HttpPost]
            public IActionResult Edit(Payment payer)
            {
                var loginUser = _context.userlogin.Where(u => u.Email == payer.Email).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    if (payer.PaymentID == 0)
                        _context.payment.Add(payer);
                    else
                    {
                        if (loginUser != null)
                        {
                            int loginID = loginUser.LoginID;
                            payer.LoginID = loginID;
                        }
                        _context.payment.Update(payer);
                    }
                    _context.SaveChanges();
                    return RedirectToAction("Index", "payment");
                 }
                else
                {
                    ViewBag.Action = (payer.PaymentID == 0) ? "Add" : "Edit";
                    return View(payer);
                }

            }

        [HttpGet]
        public IActionResult Delete(int id)
            {
                var payer = _context.payment.Find(id);
                    return View(payer);
            }

            [HttpPost]
            public IActionResult Delete(Payment payer)
            {
                _context.payment.Remove(payer);
                _context.SaveChanges();
                return RedirectToAction("Index", "payment");

            }

            [HttpGet]
            public IActionResult Detail(int id)
            {
                var payer = _context.payment.Find(id);
                return View(payer);
                
            }
        }
}
