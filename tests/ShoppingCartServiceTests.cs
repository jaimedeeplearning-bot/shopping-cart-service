using NUnit.Framework;
using Moq;
using ShoppingCartService.Interfaces;
using ShoppingCartService.Models;
using ShoppingCartService.Services;
using System.Threading.Tasks;

namespace ShoppingCartService.Tests
{
    [TestFixture]
    public class ShoppingCartServiceTests
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

        #region Scenario 1: Agregar producto al carrito vacío

        
        [Test]
        public void test_add_product_to_empty_cart_total_is_one_hundred()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.SetTaxRate(0); // Sin impuestos para este test

            // Act
            _cartService.AddProduct(product);

            // Assert
            Assert.That(_cartService.GetTotal(), Is.EqualTo(100m),
                "El total debe ser $100");
        }

        #endregion

        #region Scenario 2: Aplicar descuento válido

        [Test]
        public void test_apply_valid_discount_code_summer20_discount_percentage_is_20_percent()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var validDiscountCode = new DiscountCode("SUMMER20", 20m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("SUMMER20"))
                .ReturnsAsync(validDiscountCode);

            // Act
            var result = _cartService.ApplyDiscountCodeAsync("SUMMER20").Result;

            // Assert
            Assert.That(result, Is.True,
                "El código de descuento debe aplicarse exitosamente");
            Assert.That(_cartService.GetAppliedDiscount().DiscountPercentage, Is.EqualTo(20m),
                "El descuento debe aplicarse como 20%");
        }

        [Test]
        public void test_apply_valid_discount_code_summer20_total_is_eighty()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            var validDiscountCode = new DiscountCode("SUMMER20", 20m, isValid: true);
            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("SUMMER20"))
                .ReturnsAsync(validDiscountCode);

            // Act
            _cartService.ApplyDiscountCodeAsync("SUMMER20").Wait();

            // Assert
            Assert.That(_cartService.GetTotal(), Is.EqualTo(80m),
                "El total debe ser $80 después de aplicar descuento del 20%");
        }

        #endregion

        #region Scenario 3: Rechazar código de descuento inválido

        [Test]
        public void test_apply_invalid_discount_code_fake99_returns_false()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("FAKE99"))
                .ReturnsAsync((DiscountCode)null);

            // Act
            var result = _cartService.ApplyDiscountCodeAsync("FAKE99").Result;

            // Assert
            Assert.That(result, Is.False,
                "El código de descuento inválido no debe aplicarse");
        }

        [Test]
        public void test_apply_invalid_discount_code_fake99_total_remains_one_hundred()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(0);

            _mockDiscountRepository
                .Setup(x => x.GetDiscountCodeAsync("FAKE99"))
                .ReturnsAsync((DiscountCode)null);

            // Act
            _cartService.ApplyDiscountCodeAsync("FAKE99").Wait();

            // Assert
            Assert.That(_cartService.GetTotal(), Is.EqualTo(100m),
                "El total debe permanecer en $100 cuando el descuento es inválido");
        }

        #endregion

        #region Scenario 4: Calcular impuestos correctamente

        [Test]
        public void test_calculate_tax_with_eight_percent_tax_amount_is_eight()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(8m);

            // Act
            var taxAmount = _cartService.GetTaxAmount();

            // Assert
            Assert.That(taxAmount, Is.EqualTo(8m),
                "El impuesto debe ser $8 sobre un monto de $100 con tasa del 8%");
        }

        [Test]
        public void test_calculate_tax_with_eight_percent_total_is_one_hundred_eight()
        {
            // Arrange
            var product = new Product("PROD001", "Test Product", 100m);
            _cartService.AddProduct(product);
            _cartService.SetTaxRate(8m);

            // Act
            var total = _cartService.GetTotal();

            // Assert
            Assert.That(total, Is.EqualTo(108m),
                "El total final debe ser $108 (100 + 8 de impuesto)");
        }

        #endregion

        #region Scenario 5: Vaciar el carrito

        [Test]
        public void test_clear_cart_with_three_products_cart_is_empty()
        {
            // Arrange
            var product1 = new Product("PROD001", "Product 1", 50m);
            var product2 = new Product("PROD002", "Product 2", 75m);
            var product3 = new Product("PROD003", "Product 3", 25m);
            _cartService.AddProduct(product1);
            _cartService.AddProduct(product2);
            _cartService.AddProduct(product3);

            // Act
            _cartService.Clear();

            // Assert
            Assert.That(_cartService.IsEmpty(), Is.True,
                "El carrito debe estar vacío después de limpiar");
        }

        [Test]
        public void test_clear_cart_with_three_products_total_is_zero()
        {
            // Arrange
            var product1 = new Product("PROD001", "Product 1", 50m);
            var product2 = new Product("PROD002", "Product 2", 75m);
            var product3 = new Product("PROD003", "Product 3", 25m);
            _cartService.AddProduct(product1);
            _cartService.AddProduct(product2);
            _cartService.AddProduct(product3);
            _cartService.SetTaxRate(0);

            // Act
            _cartService.Clear();

            // Assert
            Assert.That(_cartService.GetTotal(), Is.EqualTo(0m),
                "El total debe ser $0 después de vaciar el carrito");
        }

        #endregion
    }
}
