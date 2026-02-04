using ECommerce.Core.Account.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.interfaces
{
    public interface IAddressRepositry : IGenericRepositry<Address>
    {
        public Task<Address?> GetByUserIdAsync(string userId);


    }
}
