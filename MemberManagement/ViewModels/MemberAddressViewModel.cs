using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemberManagement.Models;
namespace MemberManagement.ViewModels
{
    public class MemberAddressViewModel
    {
        public Member Member { get; set; }
        public Address Address { get; set; }
      // public Child Child { get; set; }
    }
}
