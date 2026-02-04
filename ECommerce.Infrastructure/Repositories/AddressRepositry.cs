using ECommerce.Core.Account.Entites;
using ECommerce.Core.interfaces;
using ECommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class AddressRepositry : GenericRepositry<Address> , IAddressRepositry
    {
        public AddressRepositry(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<Address?> GetByUserIdAsync(string userId)
        {
            var address =await _appDbContext.Addresses.FirstOrDefaultAsync(i => i.AppUserId == userId);
            return address;
        }
    }
}
