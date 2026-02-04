namespace ECommerce.Core.Entites.Order
{
    public class OrderItem : BaseEntity<int>
    {
        public OrderItem()
        {
        }
        public OrderItem(int productId, string productName, string productImage, decimal price, int quantity)
        {
            ProductId = productId;
            ProductName = productName;
            ProductImage = productImage;
            Price = price;
            Quantity = quantity;
        }

        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price {  get; set; }
        public int Quantity { get; set;}
    }
}