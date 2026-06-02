namespace ShoppingCartService.Models
{
    /// <summary>
    /// Representa un producto agregado al carrito con su cantidad
    /// </summary>
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity = 1)
        {
            Product = product;
            Quantity = quantity;
        }

        public decimal GetSubtotal()
        {
            return Product.Price * Quantity;
        }
    }
}
