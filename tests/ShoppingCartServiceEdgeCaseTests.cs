using NUnit.Framework;
using Moq;
using ShoppingCartService.Interfaces;
using ShoppingCartService.Models;
using ShoppingCartService.Services;
using System;
using System.Threading.Tasks;

namespace ShoppingCartService.Tests
{
    /// <summary>
    /// Tests para casos edge case, entrada inválida y límites
    /// Objetivo: Aumentar cobertura a 90%+
    /// </summary>
    [TestFixture]
    public class ShoppingCartServiceEdgeCaseTests
    {
        private ShoppingCartService.Services.ShoppingCartService _cartService;
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private Mock<ITaxService> _mockTaxService;

        [SetUp]
        public void Setup()
        {
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _mockTaxService = new Mock<ITaxService>();
            _cartService = new ShoppingCartService.Services.ShoppingCartService(
                _mockDiscountRepository.Object,
                _mockTaxService.Object
            );
        }

        #region Tests: Entrada Inválida - AddProduct

        [Test]
        public void test_add_product_with_null_product_throws_argument_null_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => _cartService.AddProduct(null));
        }

        [Test]
        public void test_add_product_with_negative_quantity_throws_argument_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _cartService.AddProduct(product, -5));
        }

        [Test]
        public void test_add_product_with_zero_quantity_throws_argument_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _cartService.AddProduct(product, 0));
        }

        [Test]
        public void test_add_product_with_negative_price_throws_argument_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", -100m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _cartService.AddProduct(product));
        }

        [Test]
        public void test_add_product_with_zero_price_success()
        {
            // Arrange
            var product = new Product("PROD001", "Free Product", 0m);
            _cartService.SetTaxRate(0);

            // Act
            _cartService.AddProduct(product);

            // Assert
            Assert.That(_cartService.GetTotal(), Is.EqualTo(0m));
            Assert.That(_cartService.GetProductCount(), Is.EqualTo(1));
        }

        #endregion

        #region Tests: Entrada Inválida - ApplyDiscountCodeAsync

        [Test]
        public void test_apply_discount_with_null_code_throws_argument_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Act & Assert
            Assert.Throws<ArgumentException>(async () => await _cartService.ApplyDiscountCodeAsync(null));
        }

        [Test]
        public void test_apply_discount_with_empty_string_throws_argument_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Act & Assert
            Assert.Throws<ArgumentException>(async () => await _cartService.ApplyDiscountCodeAsync(""));
        }

        [Test]
        public void test_apply_discount_with_whitespace_only_throws_argument_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Act & Assert
            Assert.Throws<ArgumentException>(async () => await _cartService.ApplyDiscountCodeAsync("   "));
        }

        [Test]
        public void test_apply_discount_with_percentage_above_100_throws_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var invalidDiscountCode = new DiscountCode("INVALID150", 150m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("INVALID150"))
                .ReturnsAsync(invalidDiscountCode);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                async () => await _cartService.ApplyDiscountCodeAsync("INVALID150")
            );
        }

        [Test]
        public void test_apply_discount_with_negative_percentage_throws_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var invalidDiscountCode = new DiscountCode("NEGATIVEDISCOUNT", -20m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("NEGATIVEDISCOUNT"))
                .ReturnsAsync(invalidDiscountCode);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                async () => await _cartService.ApplyDiscountCodeAsync("NEGATIVEDISCOUNT")
            );
        }

        [Test]
        public void test_apply_discount_100_percent_returns_true()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var maxDiscountCode = new DiscountCode("FREE100", 100m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("FREE100"))
                .ReturnsAsync(maxDiscountCode);

            // Act
            var result = _cartService.ApplyDiscountCodeAsync("FREE100").Result;

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_cartService.GetTotal(), Is.EqualTo(0m));
        }

        #endregion

        #region Tests: Casos Límite - Empty Cart

        [Test]
        public void test_empty_cart_get_total_is_zero()
        {
            // Arrange & Act
            var total = _cartService.GetTotal();

            // Assert
            Assert.That(total, Is.EqualTo(0m));
        }

        [Test]
        public void test_empty_cart_get_subtotal_is_zero()
        {
            // Arrange & Act
            var subtotal = _cartService.GetSubtotal();

            // Assert
            Assert.That(subtotal, Is.EqualTo(0m));
        }

        [Test]
        public void test_empty_cart_get_tax_amount_is_zero()
        {
            // Arrange
            _cartService.SetTaxRate(8m);

            // Act
            var taxAmount = _cartService.GetTaxAmount();

            // Assert
            Assert.That(taxAmount, Is.EqualTo(0m));
        }

        [Test]
        public void test_empty_cart_get_discount_amount_is_zero()
        {
            // Arrange & Act
            var discountAmount = _cartService.GetDiscountAmount();

            // Assert
            Assert.That(discountAmount, Is.EqualTo(0m));
        }

        [Test]
        public void test_empty_cart_is_empty_returns_true()
        {
            // Arrange & Act
            var isEmpty = _cartService.IsEmpty();

            // Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void test_empty_cart_get_product_count_is_zero()
        {
            // Arrange & Act
            var count = _cartService.GetProductCount();

            // Assert
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void test_empty_cart_clear_does_not_throw()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => _cartService.Clear());
        }

        [Test]
        public void test_empty_cart_get_cart_summary_all_zeros()
        {
            // Arrange & Act
            var summary = _cartService.GetCartSummary();

            // Assert
            Assert.That(summary.Subtotal, Is.EqualTo(0m));
            Assert.That(summary.DiscountAmount, Is.EqualTo(0m));
            Assert.That(summary.TaxAmount, Is.EqualTo(0m));
            Assert.That(summary.Total, Is.EqualTo(0m));
        }

        #endregion

        #region Tests: Casos Límite - Tax Rate

        [Test]
        public void test_set_tax_rate_negative_throws_argument_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => _cartService.SetTaxRate(-5m));
        }

        [Test]
        public void test_set_tax_rate_above_100_throws_argument_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => _cartService.SetTaxRate(150m));
        }

        [Test]
        public void test_set_tax_rate_zero_success()
        {
            // Arrange & Act
            _cartService.SetTaxRate(0m);
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Assert
            Assert.That(_cartService.GetTaxAmount(), Is.EqualTo(0m));
        }

        [Test]
        public void test_set_tax_rate_100_percent_success()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(100m);

            // Act
            var total = _cartService.GetTotal();

            // Assert
            Assert.That(total, Is.EqualTo(200m)); // 100 + 100 de impuesto
        }

        #endregion

        #region Tests: Secuencias Inesperadas

        [Test]
        public void test_apply_discount_twice_second_replaces_first()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var discount1 = new DiscountCode("SUMMER20", 20m, isValid: true);
            var discount2 = new DiscountCode("SUMMER30", 30m, isValid: true);

            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("SUMMER20"))
                .ReturnsAsync(discount1);

            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("SUMMER30"))
                .ReturnsAsync(discount2);

            // Act
            _cartService.ApplyDiscountCodeAsync("SUMMER20").Wait();
            var firstTotal = _cartService.GetTotal(); // 80

            _cartService.ApplyDiscountCodeAsync("SUMMER30").Wait();
            var secondTotal = _cartService.GetTotal(); // 70

            // Assert
            Assert.That(firstTotal, Is.EqualTo(80m));
            Assert.That(secondTotal, Is.EqualTo(70m));
        }

        [Test]
        public void test_remove_discount_restores_original_total()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var discountCode = new DiscountCode("SUMMER20", 20m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("SUMMER20"))
                .ReturnsAsync(discountCode);

            // Act
            _cartService.ApplyDiscountCodeAsync("SUMMER20").Wait();
            var totalWithDiscount = _cartService.GetTotal();

            _cartService.RemoveDiscount();
            var totalAfterRemoval = _cartService.GetTotal();

            // Assert
            Assert.That(totalWithDiscount, Is.EqualTo(80m));
            Assert.That(totalAfterRemoval, Is.EqualTo(100m));
        }

        [Test]
        public void test_clear_empty_cart_remains_empty()
        {
            // Arrange & Act
            _cartService.Clear();
            _cartService.Clear(); // Clear twice

            // Assert
            Assert.That(_cartService.IsEmpty(), Is.True);
            Assert.That(_cartService.GetTotal(), Is.EqualTo(0m));
        }

        [Test]
        public void test_clear_cart_resets_discount_and_tax()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(8m);

            var discountCode = new DiscountCode("SUMMER20", 20m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("SUMMER20"))
                .ReturnsAsync(discountCode);

            _cartService.ApplyDiscountCodeAsync("SUMMER20").Wait();

            // Act
            _cartService.Clear();

            // Assert
            Assert.That(_cartService.IsEmpty(), Is.True);
            Assert.That(_cartService.GetAppliedDiscount(), Is.Null);
            Assert.That(_cartService.GetTaxAmount(), Is.EqualTo(0m));
        }

        [Test]
        public void test_add_same_product_multiple_times_increases_quantity()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.SetTaxRate(0);

            // Act
            _cartService.AddProduct(product, 1);
            _cartService.AddProduct(product, 2);
            _cartService.AddProduct(product, 3);

            // Assert
            Assert.That(_cartService.GetProductCount(), Is.EqualTo(1));
            Assert.That(_cartService.GetTotalItemCount(), Is.EqualTo(6));
            Assert.That(_cartService.GetTotal(), Is.EqualTo(600m));
        }

        [Test]
        public void test_get_discount_with_no_applied_discount_returns_zero()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Act
            var discountAmount = _cartService.GetDiscountAmount();

            // Assert
            Assert.That(discountAmount, Is.EqualTo(0m));
        }

        #endregion

        #region Tests: Métodos Adicionales - UpdateProductQuantity

        [Test]
        public void test_update_product_quantity_with_null_id_throws_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(
                () => _cartService.UpdateProductQuantity(null, 5)
            );
        }

        [Test]
        public void test_update_product_quantity_with_empty_id_throws_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(
                () => _cartService.UpdateProductQuantity("", 5)
            );
        }

        [Test]
        public void test_update_product_quantity_with_negative_quantity_throws_exception()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => _cartService.UpdateProductQuantity("PROD001", -5)
            );
        }

        [Test]
        public void test_update_product_quantity_nonexistent_product_returns_false()
        {
            // Arrange & Act
            var result = _cartService.UpdateProductQuantity("NONEXISTENT", 5);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void test_update_product_quantity_to_higher_value_success()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product, 2);
            _cartService.SetTaxRate(0);

            // Act
            var result = _cartService.UpdateProductQuantity("PROD001", 5);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_cartService.GetTotalItemCount(), Is.EqualTo(5));
            Assert.That(_cartService.GetTotal(), Is.EqualTo(500m));
        }

        [Test]
        public void test_update_product_quantity_to_zero_removes_product()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product, 2);

            // Act
            var result = _cartService.UpdateProductQuantity("PROD001", 0);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_cartService.GetProductCount(), Is.EqualTo(0));
            Assert.That(_cartService.IsEmpty(), Is.True);
        }

        [Test]
        public void test_update_product_quantity_to_lower_value_success()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product, 5);
            _cartService.SetTaxRate(0);

            // Act
            var result = _cartService.UpdateProductQuantity("PROD001", 2);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_cartService.GetTotalItemCount(), Is.EqualTo(2));
            Assert.That(_cartService.GetTotal(), Is.EqualTo(200m));
        }

        #endregion

        #region Tests: Métodos Adicionales - RemoveProduct

        [Test]
        public void test_remove_product_with_null_id_throws_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(
                () => _cartService.RemoveProduct(null)
            );
        }

        [Test]
        public void test_remove_product_with_empty_id_throws_exception()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(
                () => _cartService.RemoveProduct("")
            );
        }

        [Test]
        public void test_remove_product_nonexistent_returns_false()
        {
            // Arrange & Act
            var result = _cartService.RemoveProduct("NONEXISTENT");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void test_remove_product_existing_returns_true()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);

            // Act
            var result = _cartService.RemoveProduct("PROD001");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_cartService.IsEmpty(), Is.True);
        }

        [Test]
        public void test_remove_product_from_cart_with_multiple_products()
        {
            // Arrange
            var product1 = new Product("PROD001", "Product 1", 100m);
            var product2 = new Product("PROD002", "Product 2", 200m);
            var product3 = new Product("PROD003", "Product 3", 300m);
            _cartService.AddProduct(product1);
            _cartService.AddProduct(product2);
            _cartService.AddProduct(product3);
            _cartService.SetTaxRate(0);

            // Act
            var result = _cartService.RemoveProduct("PROD002");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_cartService.GetProductCount(), Is.EqualTo(2));
            Assert.That(_cartService.GetTotal(), Is.EqualTo(400m)); // 100 + 300
        }

        #endregion

        #region Tests: Métodos Adicionales - GetItems

        [Test]
        public void test_get_items_from_empty_cart_returns_empty_collection()
        {
            // Arrange & Act
            var items = _cartService.GetItems();

            // Assert
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void test_get_items_returns_all_products()
        {
            // Arrange
            var product1 = new Product("PROD001", "Product 1", 100m);
            var product2 = new Product("PROD002", "Product 2", 200m);
            _cartService.AddProduct(product1);
            _cartService.AddProduct(product2);

            // Act
            var items = _cartService.GetItems();

            // Assert
            Assert.That(items, Has.Count.EqualTo(2));
        }

        #endregion

        #region Tests: Combinaciones Complejas

        [Test]
        public void test_complex_scenario_multiple_products_with_discount_and_tax()
        {
            // Arrange
            var product1 = new Product("PROD001", "Laptop", 1000m);
            var product2 = new Product("PROD002", "Mouse", 50m);
            var product3 = new Product("PROD003", "Keyboard", 100m);
            _cartService.AddProduct(product1, 1);
            _cartService.AddProduct(product2, 2);
            _cartService.AddProduct(product3, 1);

            _cartService.SetTaxRate(8m);

            var discountCode = new DiscountCode("BLACKFRIDAY", 15m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("BLACKFRIDAY"))
                .ReturnsAsync(discountCode);

            // Act
            var subtotal = _cartService.GetSubtotal(); // 1000 + 100 + 100 = 1200
            _cartService.ApplyDiscountCodeAsync("BLACKFRIDAY").Wait();
            var discountAmount = _cartService.GetDiscountAmount(); // 1200 * 0.15 = 180
            var taxAmount = _cartService.GetTaxAmount(); // (1200 - 180) * 0.08 = 81.6
            var total = _cartService.GetTotal(); // 1200 - 180 + 81.6 = 1101.6

            // Assert
            Assert.That(subtotal, Is.EqualTo(1200m));
            Assert.That(discountAmount, Is.EqualTo(180m));
            Assert.That(taxAmount, Is.EqualTo(81.6m));
            Assert.That(total, Is.EqualTo(1101.6m));
        }

        [Test]
        public void test_complex_scenario_modify_quantities_and_prices()
        {
            // Arrange
            var product1 = new Product("PROD001", "Item 1", 100m);
            var product2 = new Product("PROD002", "Item 2", 200m);
            _cartService.AddProduct(product1, 2); // 200
            _cartService.AddProduct(product2, 1); // 200
            _cartService.SetTaxRate(10m);

            // Act - Initial total
            var initialTotal = _cartService.GetTotal(); // (200 + 200) + 40 = 440

            // Modify quantities
            _cartService.UpdateProductQuantity("PROD001", 1); // 100
            var middleTotal = _cartService.GetTotal(); // (100 + 200) + 30 = 330

            // Remove a product
            _cartService.RemoveProduct("PROD002");
            var finalTotal = _cartService.GetTotal(); // 100 + 10 = 110

            // Assert
            Assert.That(initialTotal, Is.EqualTo(440m));
            Assert.That(middleTotal, Is.EqualTo(330m));
            Assert.That(finalTotal, Is.EqualTo(110m));
        }

        #endregion
    }
}
