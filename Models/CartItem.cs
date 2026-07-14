namespace BestPriceStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        public int ProductImageId { get; set; }
        public ProductImage? ProductImage { get; set; }

        public int Quantity { get; set; }
    }
}
