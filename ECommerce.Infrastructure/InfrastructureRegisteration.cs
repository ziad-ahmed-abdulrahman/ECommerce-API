using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Repositories.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

         
            // Register generic repository
            services.AddScoped(typeof(IGenericRepositry<>), typeof(GenericRepositry<>));

            // Apply Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultEComDatabase"));
            });

     
           

            return services;
        }
    }




}
