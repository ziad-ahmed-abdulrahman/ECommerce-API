using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Account.Entites
{
    public class AppUser : IdentityUser
    {
        public Address Address { get; set; } 
        public bool IsActive { get; set; } = false;
        public string? OneTimeCode { get; set; }

        public DateTime? OneTimeCodeExpiry { get; set; }

        public string? CodeOperation { get; set; }


    }
}
