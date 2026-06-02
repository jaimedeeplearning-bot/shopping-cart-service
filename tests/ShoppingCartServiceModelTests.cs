using NUnit.Framework;
using ShoppingCartService.Models;
using System;

namespace ShoppingCartService.Tests
{
    /// <summary>
    /// Tests para validar los modelos del carrito
    /// Cubre CartItem, Product, DiscountCode, CartSummary
    /// </summary>
    [TestFixture]
    public class ShoppingCartServiceModelTests
    {
        #region Product Model Tests

        [Test]
        public void test_product_creation_with_valid_data_success()
        {
            // Arrange & Act
            var product = new Product("PROD001", "Test Product", 99.99m);

            // Assert
            Assert.That(product.Id, Is.EqualTo("PROD001"));
            Assert.That(product.Name, Is.EqualTo("Test Product"));
            Assert.That(product.Price, Is.EqualTo(99.99m));
        }

        [Test]
        public void test_product_creation_with_zero_price()
        {
            // Arrange & Act
            var product = new Product("PROD001", "Free Product", 0m);

            // Assert
            Assert.That(product.Price, Is.EqualTo(0m));
        }

        [Test]
        public void test_product_creation_with_large_price()
        {
            // Arrange & Act
            var product = new Product("PROD001", "Expensive Product", 999999.99m);

            // Assert
            Assert.That(product.Price, Is.EqualTo(999999.99m));
        }

        [Test]
        public void test_product_creation_with_negative_price()
        {
            // Arrange & Act
            var product = new Product("PROD001", "Invalid Product", -50m);

            // Assert
            Assert.That(product.Price, Is.EqualTo(-50m));
        }

        #endregion

        #region CartItem Model Tests

        [Test]
        public void test_cart_item_creation_with_default_quantity()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);

            // Act
            var cartItem = new CartItem(product);

            // Assert
            Assert.That(cartItem.Product, Is.EqualTo(product));
            Assert.That(cartItem.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void test_cart_item_creation_with_custom_quantity()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);

            // Act
            var cartItem = new CartItem(product, 5);

            // Assert
            Assert.That(cartItem.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void test_cart_item_get_subtotal_calculation()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            var cartItem = new CartItem(product, 3);

            // Act
            var subtotal = cartItem.GetSubtotal();

            // Assert
            Assert.That(subtotal, Is.EqualTo(300m)); // 100 * 3
        }

        [Test]
        public void test_cart_item_get_subtotal_with_decimal_price()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 19.99m);
            var cartItem = new CartItem(product, 2);

            // Act
            var subtotal = cartItem.GetSubtotal();

            // Assert
            Assert.That(subtotal, Is.EqualTo(39.98m)); // 19.99 * 2
        }

        [Test]
        public void test_cart_item_get_subtotal_with_zero_quantity()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            var cartItem = new CartItem(product, 0);

            // Act
            var subtotal = cartItem.GetSubtotal();

            // Assert
            Assert.That(subtotal, Is.EqualTo(0m));
        }

        [Test]
        public void test_cart_item_get_subtotal_with_negative_quantity()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            var cartItem = new CartItem(product, -5);

            // Act
            var subtotal = cartItem.GetSubtotal();

            // Assert
            Assert.That(subtotal, Is.EqualTo(-500m)); // 100 * -5
        }

        #endregion

        #region DiscountCode Model Tests

        [Test]
        public void test_discount_code_creation_with_default_is_valid()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("SUMMER20", 20m);

            // Assert
            Assert.That(discountCode.Code, Is.EqualTo("SUMMER20"));
            Assert.That(discountCode.DiscountPercentage, Is.EqualTo(20m));
            Assert.That(discountCode.IsValid, Is.True);
        }

        [Test]
        public void test_discount_code_creation_with_explicit_is_valid_true()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("SUMMER20", 20m, isValid: true);

            // Assert
            Assert.That(discountCode.IsValid, Is.True);
        }

        [Test]
        public void test_discount_code_creation_with_explicit_is_valid_false()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("EXPIRED", 20m, isValid: false);

            // Assert
            Assert.That(discountCode.IsValid, Is.False);
        }

        [Test]
        public void test_discount_code_with_zero_percentage()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("NODISCOUNT", 0m);

            // Assert
            Assert.That(discountCode.DiscountPercentage, Is.EqualTo(0m));
        }

        [Test]
        public void test_discount_code_with_100_percent()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("FREE", 100m);

            // Assert
            Assert.That(discountCode.DiscountPercentage, Is.EqualTo(100m));
        }

        [Test]
        public void test_discount_code_with_decimal_percentage()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("PROMO", 12.5m);

            // Assert
            Assert.That(discountCode.DiscountPercentage, Is.EqualTo(12.5m));
        }

        [Test]
        public void test_discount_code_with_negative_percentage()
        {
            // Arrange & Act
            var discountCode = new DiscountCode("INVALID", -20m);

            // Assert
            Assert.That(discountCode.DiscountPercentage, Is.EqualTo(-20m));
        }

        #endregion

        #region CartSummary Model Tests

        [Test]
        public void test_cart_summary_creation_calculates_total_correctly()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 100m, discountAmount: 20m, taxAmount: 6.40m);
            // Expected: 100 - 20 + 6.40 = 86.40

            // Assert
            Assert.That(summary.Subtotal, Is.EqualTo(100m));
            Assert.That(summary.DiscountAmount, Is.EqualTo(20m));
            Assert.That(summary.TaxAmount, Is.EqualTo(6.40m));
            Assert.That(summary.Total, Is.EqualTo(86.40m));
        }

        [Test]
        public void test_cart_summary_with_zero_values()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 0m, discountAmount: 0m, taxAmount: 0m);

            // Assert
            Assert.That(summary.Total, Is.EqualTo(0m));
        }

        [Test]
        public void test_cart_summary_with_no_discount()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 100m, discountAmount: 0m, taxAmount: 8m);
            // Expected: 100 - 0 + 8 = 108

            // Assert
            Assert.That(summary.Total, Is.EqualTo(108m));
        }

        [Test]
        public void test_cart_summary_with_no_tax()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 100m, discountAmount: 20m, taxAmount: 0m);
            // Expected: 100 - 20 + 0 = 80

            // Assert
            Assert.That(summary.Total, Is.EqualTo(80m));
        }

        [Test]
        public void test_cart_summary_with_all_values_zero()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 0m, discountAmount: 0m, taxAmount: 0m);

            // Assert
            Assert.That(summary.Subtotal, Is.EqualTo(0m));
            Assert.That(summary.DiscountAmount, Is.EqualTo(0m));
            Assert.That(summary.TaxAmount, Is.EqualTo(0m));
            Assert.That(summary.Total, Is.EqualTo(0m));
        }

        [Test]
        public void test_cart_summary_calculation_with_large_numbers()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 10000m, discountAmount: 1500m, taxAmount: 680m);
            // Expected: 10000 - 1500 + 680 = 9180

            // Assert
            Assert.That(summary.Total, Is.EqualTo(9180m));
        }

        [Test]
        public void test_cart_summary_calculation_with_precise_decimals()
        {
            // Arrange & Act
            var summary = new CartSummary(subtotal: 100.99m, discountAmount: 15.15m, taxAmount: 6.87m);
            // Expected: 100.99 - 15.15 + 6.87 = 92.71

            // Assert
            Assert.That(summary.Total, Is.EqualTo(92.71m));
        }

        #endregion
    }
}
