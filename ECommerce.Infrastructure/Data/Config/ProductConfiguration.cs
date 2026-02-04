using ECommerce.Core.Entites.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Data.Config
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {


            builder.Property(p => p.Name).IsRequired().HasMaxLength(30);
            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
            builder.Property(p => p.NewPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.OldPrice).IsRequired(false).HasColumnType("decimal(18,2)");
            

          




        }
    }
}
