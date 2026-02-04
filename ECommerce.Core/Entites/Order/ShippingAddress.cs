using ECommerce.Core.Account.Entites;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Entites.Order
{
    public class ShippingAddress : BaseEntity<int>
    {
        public ShippingAddress()
        {
        }
        public ShippingAddress(string firstName, string lastName, string city, string codeZip, string street, string state)
        {
            FirstName = firstName;
            LastName = lastName;
            City = city;
            CodeZip = codeZip;
            Street = street;
            State = state;
        }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string CodeZip { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string State { get; set; } = null!;

    }
}