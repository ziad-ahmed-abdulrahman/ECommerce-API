namespace ECommerce.Core.Entites.Basket
{
    public class BasketItem : BaseEntity<int>
    {
        public string Name { get; set; } = null!; 

        public string? Image { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        

    }
}