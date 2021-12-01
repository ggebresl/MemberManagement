using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    public class MockUserloginRepostiory : IUserloginRepository
    {
        private List<Userlogin> _userloginList;

        public MockUserloginRepostiory()
        {
            _userloginList = new List<Userlogin>()
            {
                new Userlogin() {LoginID = 1, Username = "ggebresl1", Password = "********", Firstname = "Gerawork1", Lastname = "Gebreslassie1", Phone = "7812907621", Email= "ggebresl1@yahoo.com"},
                new Userlogin() {LoginID = 2, Username = "ggebresl2", Password = "********", Firstname = "Gerawork2", Lastname = "Gebreslassie2", Phone = "7812907622", Email= "ggebresl2@yahoo.com"},
                new Userlogin() {LoginID = 3, Username = "ggebresl3", Password = "********", Firstname = "Gerawork3", Lastname = "Gebreslassie3", Phone = "7812907623", Email= "ggebresl3@yahoo.com"},
                new Userlogin() {LoginID = 4, Username = "ggebresl4", Password = "********", Firstname = "Gerawork4", Lastname = "Gebreslassie4", Phone = "7812907624", Email= "ggebresl4@yahoo.com"}

            };
        }

        public IEnumerable<Userlogin> GetAllUserloin()
        {
            return this._userloginList;
        }

        public Userlogin GetUserlogin(int id)
        {
            return this._userloginList.FirstOrDefault(u => u.LoginID == id);


        }
        
        public Userlogin Delete(Userlogin user)
        {
            
            if ((user != null) && (_userloginList.Count() > 0))
            {
                _userloginList.Remove(user); 
            }
            return user;
        }

        public Userlogin Save(Userlogin user)
        {
            Userlogin userlogin = null;
               if (user != null)
                {
                  userlogin = new Userlogin()
                    {
                        LoginID = user.LoginID,
                        Firstname = user.Firstname,
                        Lastname = user.Lastname,
                        Username = user.Username,
                        Password = user.Password,
                        Phone = user.Phone,
                        Email = user.Email
                    };
                _userloginList.Add(userlogin);
                }
          return userlogin;
        }

        public IEnumerable<Userlogin> Search(string searchTerm)
        {
             if (string.IsNullOrEmpty(searchTerm))
            {
                return _userloginList;
            }
            return _userloginList.Where(u => u.Firstname.Contains(searchTerm) ||
                                        u.Email.Contains(searchTerm));
        }
    }
}
