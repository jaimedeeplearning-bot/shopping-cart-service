namespace ShoppingCartService.Models
{
    /// <summary>
    /// Representa un código de descuento
    /// </summary>
    public class DiscountCode
    {
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsValid { get; set; }

        public DiscountCode(string code, decimal discountPercentage, bool isValid = true)
        {
            Code = code;
            DiscountPercentage = discountPercentage;
            IsValid = isValid;
        }
    }
}
