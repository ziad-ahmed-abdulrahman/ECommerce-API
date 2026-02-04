using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Product;
using ECommerce.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.interfaces
{
    public interface IProductRepositry : IGenericRepositry<Product>
    {
        public  Task<bool> DeleteProductWithPhotosAsync(int id);
        public Task<ReturnProductDto> GetAllAsync(ProductParams productParams); 


        // for future
    }
}
