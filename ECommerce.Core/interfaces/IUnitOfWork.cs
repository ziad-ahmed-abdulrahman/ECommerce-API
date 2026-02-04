using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.interfaces
{
    public interface IUnitOfWork 
    {
        public IPhotoRepositry PhotoRepositry { get; }  
        public ICategoryRepositry CategoryRepositry { get; }  
        public IProductRepositry ProductRepositry { get; }  
        public ICustomerBasketRepositry CustomerBasketRepositry { get; }
        public IAddressRepositry AddressRepositry { get; }



    }
}
