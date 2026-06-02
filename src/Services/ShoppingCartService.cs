using ShoppingCartService.Interfaces;
using ShoppingCartService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartService.Services
{
    /// <summary>
    /// Servicio principal del carrito de compras
    /// Implementación siguiendo TDD (Test-Driven Development)
    /// </summary>
    public class ShoppingCartService
    {
        private readonly List<CartItem> _items;
        private readonly IDiscountRepository _discountRepository;
        private readonly ITaxService _taxService;
        private DiscountCode _appliedDiscount;
        private decimal _taxRate;

        public ShoppingCartService(IDiscountRepository discountRepository, ITaxService taxService)
        {
            _items = new List<CartItem>();
            _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
            _taxService = taxService ?? throw new ArgumentNullException(nameof(taxService));
            _appliedDiscount = null;
            _taxRate = 0;
        }

        /// <summary>
        /// Agrega un producto al carrito
        /// Basado en el test: test_add_product_to_empty_cart_product_count_is_one
        /// </summary>
        public void AddProduct(Product product, int quantity = 1)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "El producto no puede ser nulo");

            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(quantity));

            var existingItem = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem(product, quantity));
            }
        }

        /// <summary>
        /// Obtiene la cantidad de productos únicos en el carrito
        /// Basado en el test: test_add_product_to_empty_cart_product_count_is_one
        /// </summary>
        public int GetProductCount()
        {
            return _items.Count;
        }

        /// <summary>
        /// Obtiene la cantidad total de items (considerando cantidades)
        /// </summary>
        public int GetTotalItemCount()
        {
            return _items.Sum(i => i.Quantity);
        }

        /// <summary>
        /// Calcula el subtotal del carrito (sin descuentos ni impuestos)
        /// Basado en el test: test_add_product_to_empty_cart_total_is_one_hundred
        /// </summary>
        public decimal GetSubtotal()
        {
            return _items.Sum(i => i.GetSubtotal());
        }

        /// <summary>
        /// Aplica un código de descuento al carrito
        /// Basado en los tests: 
        /// - test_apply_valid_discount_code_summer20_discount_percentage_is_20_percent
        /// - test_apply_invalid_discount_code_fake99_returns_false
        /// </summary>
        public async Task<bool> ApplyDiscountCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("El código de descuento no puede estar vacío", nameof(code));

            var discountCode = await _discountRepository.GetDiscountCodeAsync(code);

            if (discountCode == null || !discountCode.IsValid)
            {
                return false;
            }

            _appliedDiscount = discountCode;
            return true;
        }

        /// <summary>
        /// Obtiene el descuento aplicado al carrito
        /// Basado en el test: test_apply_valid_discount_code_summer20_discount_percentage_is_20_percent
        /// </summary>
        public DiscountCode GetAppliedDiscount()
        {
            return _appliedDiscount;
        }

        /// <summary>
        /// Calcula el monto del descuento aplicado
        /// Basado en el test: test_apply_valid_discount_code_summer20_total_is_eighty
        /// </summary>
        public decimal GetDiscountAmount()
        {
            if (_appliedDiscount == null)
                return 0;

            var subtotal = GetSubtotal();
            return subtotal * (_appliedDiscount.DiscountPercentage / 100);
        }

        /// <summary>
        /// Establece la tasa de impuesto manualmente (para testing)
        /// Basado en el test: test_calculate_tax_with_eight_percent_tax_amount_is_eight
        /// </summary>
        public void SetTaxRate(decimal rate)
        {
            if (rate < 0)
                throw new ArgumentException("La tasa de impuesto no puede ser negativa", nameof(rate));

            _taxRate = rate;
        }

        /// <summary>
        /// Calcula el monto del impuesto
        /// Basado en el test: test_calculate_tax_with_eight_percent_tax_amount_is_eight
        /// </summary>
        public decimal GetTaxAmount()
        {
            var subtotal = GetSubtotal();
            var discountAmount = GetDiscountAmount();
            var taxableAmount = subtotal - discountAmount;

            return taxableAmount * (_taxRate / 100);
        }

        /// <summary>
        /// Obtiene el resumen completo del carrito
        /// </summary>
        public CartSummary GetCartSummary()
        {
            var subtotal = GetSubtotal();
            var discountAmount = GetDiscountAmount();
            var taxAmount = GetTaxAmount();

            return new CartSummary(subtotal, discountAmount, taxAmount);
        }

        /// <summary>
        /// Obtiene el total del carrito con impuestos y descuentos
        /// Basado en los tests:
        /// - test_add_product_to_empty_cart_total_is_one_hundred
        /// - test_apply_valid_discount_code_summer20_total_is_eighty
        /// - test_apply_invalid_discount_code_fake99_total_remains_one_hundred
        /// - test_calculate_tax_with_eight_percent_total_is_one_hundred_eight
        /// - test_clear_cart_with_three_products_total_is_zero
        /// </summary>
        public decimal GetTotal()
        {
            return GetCartSummary().Total;
        }

        /// <summary>
        /// Vacía el carrito
        /// Basado en el test: test_clear_cart_with_three_products_cart_is_empty
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _appliedDiscount = null;
            _taxRate = 0;
        }

        /// <summary>
        /// Verifica si el carrito está vacío
        /// Basado en el test: test_clear_cart_with_three_products_cart_is_empty
        /// </summary>
        public bool IsEmpty()
        {
            return _items.Count == 0;
        }
    }
}
