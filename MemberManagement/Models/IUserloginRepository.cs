using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    public interface IUserloginRepository
    {
        Userlogin GetUserlogin(int id);
        IEnumerable<Userlogin> GetAllUserloin();
        Userlogin Delete(Userlogin user);
        Userlogin Save(Userlogin user);
        IEnumerable<Userlogin> Search(string searchTerm);
    }
}
