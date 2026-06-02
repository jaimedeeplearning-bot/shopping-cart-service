namespace ShoppingCartService.Models
{
    /// <summary>
    /// Representa un producto en el sistema
    /// </summary>
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product(string id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
}
