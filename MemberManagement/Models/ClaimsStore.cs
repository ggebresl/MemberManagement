using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    public static class ClaimsStore
    {
        public static IEnumerable<Claim> AllClaims = new List<Claim>()
        {
            /*new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role")*/
            
             new Claim("Create Role", "Create Role", ClaimValueTypes.String),
             new Claim("Edit Role", "Edit Role", ClaimValueTypes.String),
             new Claim("Delete Role", "Delete Role", ClaimValueTypes.String)

        };

    }
}
