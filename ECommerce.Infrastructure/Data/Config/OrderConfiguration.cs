using ECommerce.Core.Entites.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress,
                n => { n.WithOwner(); });


            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.PaymentStatus).HasConversion(s => s.ToString(),
                s => (PaymentStatus)Enum.Parse(typeof(PaymentStatus) , s));

            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");

        }
    }
}
