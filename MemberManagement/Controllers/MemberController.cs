using MemberManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MemberManagement.ViewModels;


namespace MemberManagement.Controllers
{
    public class MemberController : Controller
    {

        private readonly ILogger<MemberController> _logger;

        private AppDbContext _context;
       // private readonly IDbContextFactory<AppDbContext> _context;

        public IConfiguration _configuration { get; }


        public MemberController(AppDbContext ctx, ILogger<MemberController> logger, IConfiguration config)
        {
            _context = ctx;
            _logger = logger;
            _configuration = config;
        }

        public IActionResult Index()
        {
            // var role = _context.role.OrderBy(i => i.RoleID).ToList();
            //   var person = _context.person.OrderBy(i => i.PersonID).ToList();
            // var address = _context.address.OrderBy(i => i.AdressID).ToList();
            //  var member = _context.member.OrderBy(i => i.LoginID).ToList();
            //var userLogin = _context.userlogin.OrderBy(i => i.LoginID).ToList();
            // var members = _context.MemberAddress.Find(1,1,2);

            var members = _context.member.OrderBy(i => i.MemberID).ToList();
            return View(members);

           // return View(member);
        }

        [HttpGet]
        public ViewResult ListRegisteredUsers()
        {
            var userLogin = _context.userlogin.OrderBy(i => i.LoginID).ToList();
            return View(userLogin);
        }
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var userLogin = _context.userlogin.Find(id);
            return View(userLogin);

        }

        [HttpGet]
        public IActionResult Add()
        {
            var role = new Role();
          //  var person = new Person();
            var address = new Address();

            //  public Role role { get; set; }      //Member.Role.RoleType
            ///  public Person person { get; set; }  //Member.Person.first_name
            // public Address address { get; set; } //Member.Address.street_number

            ViewBag.Action = "Add";
            return View("Edit", new Member());

        }
        [HttpGet]
        public IActionResult Create()
        {
            //var mac = new MemberAddressViewModel();
            // ViewBag.Address = mac.Address.StreetNumber + " " + mac.Address.StreetName + " " + mac.Address.Apt;
            //ViewBag.Address = string.Empty;
            return View();
        }

        [HttpPost]
        public IActionResult Create(MemberAddressViewModel model)
        {
            var newMember = model.Member;
            var addr = model.Address;
           
            //Check in case if the current user is alrady in the database
            var memberInDb = _context.member.Where(p => p.Cell_Phone == newMember.Cell_Phone && p.Email == newMember.Email).FirstOrDefault();
            
            if(memberInDb != null)
            {
                if (newMember.Cell_Phone == memberInDb.Cell_Phone && newMember.Email == memberInDb.Email)
                {
                    ViewBag.Info = "Your are using either the same email or phone in our database. Please try again";
                    //_context.address.Update(addr);
                    //  _context.member.Update(newMember);
                    //  _context.SaveChanges();
                    return RedirectToAction("SameInfo");

                }
            }

            if (newMember == null)
            {
                ViewBag.ErrorMessage = $"Member with Id = {model.Member.LoginID} cannot be found";
                return View("NotFound");

            };

            if (addr == null)
            {
                ViewBag.ErrorMessage = $"Address with Id = {model.Address.AdressID} cannot be found";
                return View("NotFound");

            };

            if ((newMember.LoginID == 0) && (newMember.AdressID == 0))
            {
                //Get loginID and AdressID of Member fields from userLogin  & address tables
              
                var userLogin = _context.userlogin.Where(p => p.Phone == newMember.Cell_Phone && p.Email == newMember.Email).FirstOrDefault();
                if (userLogin == null)
                {
                    ViewBag.ErrorMessage = $"Your username cannot be found. Please try with valid username";
                    return View("NotFound");
                }
                   

                if (userLogin != null)
                 {
                    _context.address.Add(addr);
                    _context.SaveChanges();

                    newMember.LoginID = userLogin.LoginID;
                  //  newMember.AdressID = _context.address.Max(i => i.AdressID);
                    newMember.AdressID = addr.AdressID;

                    _context.member.Add(newMember);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Member");

                }
                else 
                {
                    _context.address.Update(addr);
                    _context.member.Update(newMember);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Member");
                }
             }
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateChild( Int32 MemberID)
        {

           // TempData["IDS"] = MemberID;


            return View();
        }
        /*
        [HttpPost]
        public IActionResult CreateChild(Child  child)
        {
            MemberAddressViewModel model = new MemberAddressViewModel();
            model.Child = child;

            
           

            model.Member = _context.member.Where(m => m.MemberID == child.MemberID).FirstOrDefault();
            model.Address = _context.address.Where(p => p.AdressID == model.Member.AdressID).FirstOrDefault();
           
            try
            {
                if ((model.Member != null) && (model.Address != null) && (model.Child != null) &&
                      (model.Child.AdressID == 0))

                {
                    model.Child.MemberID = model.Member.MemberID;
                    model.Child.AdressID = model.Member.AdressID;

                   
                    model.Child.ChildID = _context.children.Max(i => i.ChildID);

                    _context.children.Add(model.Child);
                    _context.SaveChanges();

                    model.Member.ChildID = model.Child.ChildID;
                    _context.member.Update(model.Member);
                    _context.SaveChanges();
                }


            }
            catch(Exception ex)
            {
                string expression = ex.StackTrace;
            }



            //  model.Child = chil;
            // model.Child = _context.children.Where(c => c.ChildID == model.Member.ChildID).FirstOrDefault();







            // var member = model.Member;
            // var addr = model.Address;
            //var child = model.Child;



            return View(model);
        }
        */

        [HttpGet]
        public IActionResult SearchBy()
        {
    

            List<string> searchTypes = new List<string>() { "Phone", "Email", "Last Name" };
            ViewBag.SearchTypes = searchTypes;

            return View();

        }
        [HttpPost]
        public IActionResult SearchBy(Search selected)
        {
            Member mem = new Member();

          

            switch (selected.SearchType)
            {
                //retrieve member object  from database based on user selection
                case "Phone":
                     mem = _context.member.Where(m => m.Cell_Phone == selected.Text).FirstOrDefault();
                    break;

                case "Email":
                    mem = _context.member.Where(m => m.Email == selected.Text).FirstOrDefault();
                    break;
                default:
                    mem = _context.member.Where(m => m.Last_Name == selected.Text).FirstOrDefault();
                    break;
            }
            
            if(mem != null)
            {
               
                return RedirectToAction("GetMemberInfo", "Member", new Member()
                {
                    MemberID = mem.MemberID,
                    LoginID = mem.LoginID,
                    AdressID = mem.AdressID

                });
            }

            if (mem == null)
            {
                ViewBag.ErrorMessage = $"Member with {selected.SearchType} = {selected.Text} cannot be found";
                return View("NotFound");

            }

            /*
            List<string> searchTypes = new List<string>() { "Phone", "Email", "Last Name" };
            ViewBag.SearchTypes = searchTypes; */

            return View();
        }
        [HttpGet] 
        public IActionResult GetMemberInfo( Member mem)
        {
            MemberAddressViewModel model = new MemberAddressViewModel();
           
            model.Member = _context.member.Where(p => p.MemberID == mem.MemberID).FirstOrDefault();

            model.Address = _context.address.Where(p => p.AdressID == mem.AdressID).FirstOrDefault();

            return View(model);

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Action = "Edit";
            var member = _context.member.Find(id);
            return View(member);
        }

        [HttpPost]
        public IActionResult Edit(Member member)
        {
            int memberID = _context.member.Max(i => i.MemberID);

            var oldMemberData = _context.member.Find(memberID);

            member.LoginID = oldMemberData.LoginID;
            member.MemberID = oldMemberData.MemberID;
            member.AdressID = oldMemberData.AdressID;
            

            if (ModelState.IsValid)
            {
                if (member.LoginID == 0)
                {
                    _context.member.Add(member);
                    _context.SaveChanges();
                }

                else 

                {
                    _context.member.Remove(oldMemberData);
                    _context.member.Update(member);
                    _context.SaveChanges();

                }

                return RedirectToAction("Index", "Member");
            }
            else
            {
                ViewBag.Action = (member.LoginID == 0) ? "Add" : "Edit";
                return View(member);
            }

        }

        [HttpGet]
         
        public IActionResult Delete(int id)
        {
            var member = _context.member.Find(id);
            return View(member);
        }

        [HttpPost]
        public IActionResult Delete(Member member)
        {
            _context.member.Remove(member);
            _context.SaveChanges();
            return RedirectToAction("Index", "Member");
        }
    }
}
