using ECommerce.Core.Entites;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Account.Entites
{
    public class Address : BaseEntity<int>
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? CodeZip { get; set; } = string.Empty;
        public string? Street { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;

        public string AppUserId { get; set; } 

        [ForeignKey(nameof(AppUserId))]

        public virtual AppUser AppUser { get; set; } = null!;

    }
}