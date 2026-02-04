using ECommerce.Core.Entites.Product;
using ECommerce.Core.interfaces;
using ECommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class CategoryRepositry : GenericRepositry<Category>, ICategoryRepositry
    {
       

        public CategoryRepositry(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
