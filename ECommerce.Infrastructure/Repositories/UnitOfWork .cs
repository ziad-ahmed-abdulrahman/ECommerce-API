using ECommerce.Core.Account.Entites;
using ECommerce.Core.Entites;
using ECommerce.Core.Entites.Order;
using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        
       
        public IPhotoRepositry PhotoRepositry { get; }    

        public ICategoryRepositry CategoryRepositry { get; }

        public IProductRepositry ProductRepositry { get; }

        public ICustomerBasketRepositry CustomerBasketRepositry { get; }

        public IAddressRepositry AddressRepositry { get; }






        public UnitOfWork(AppDbContext appDbContext,
            IImageManagementService imageManagementService,
            IConnectionMultiplexer connectionMultiplexer)
        {

            CategoryRepositry = new CategoryRepositry(appDbContext);
            PhotoRepositry = new PhotoRepositry(appDbContext);
            ProductRepositry = new ProductRepositry(appDbContext, imageManagementService);
            CustomerBasketRepositry = new CustomerBasketRepositry(connectionMultiplexer);
            AddressRepositry = new AddressRepositry(appDbContext);


        }
    }
}
