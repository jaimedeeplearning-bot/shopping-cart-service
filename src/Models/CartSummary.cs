namespace ShoppingCartService.Models
{
    /// <summary>
    /// Resumen del carrito con todos los cálculos
    /// </summary>
    public class CartSummary
    {
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }

        public CartSummary(decimal subtotal, decimal discountAmount, decimal taxAmount)
        {
            Subtotal = subtotal;
            DiscountAmount = discountAmount;
            TaxAmount = taxAmount;
            Total = subtotal - discountAmount + taxAmount;
        }
    }
}
