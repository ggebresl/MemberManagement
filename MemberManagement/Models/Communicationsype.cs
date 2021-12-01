using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Models
{
    [NotMapped]
    public class Communicationsype
    {
        public string Cell_phone { get; set; }
        public string Home_phone { get; set; }
        public string Work_phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
    }
}
