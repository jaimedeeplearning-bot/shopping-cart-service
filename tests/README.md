# Shopping Cart Service - Test Suite (TDD)

Suite de pruebas unitarias para el servicio de carrito de compras.

## 📋 Estructura de Tests

Los tests están organizados por escenarios Gherkin, siguiendo el patrón **Arrange-Act-Assert (AAA)**:

### Scenario 1: Agregar producto al carrito vacío
- `test_add_product_to_empty_cart_product_count_is_one()` - Verifica que el conteo sea 1
- `test_add_product_to_empty_cart_total_is_one_hundred()` - Verifica que el total sea $100

### Scenario 2: Aplicar descuento válido
- `test_apply_valid_discount_code_summer20_discount_percentage_is_20_percent()` - Verifica que el descuento es 20%
- `test_apply_valid_discount_code_summer20_total_is_eighty()` - Verifica que el total sea $80

### Scenario 3: Rechazar código de descuento inválido
- `test_apply_invalid_discount_code_fake99_returns_false()` - Verifica que retorna false
- `test_apply_invalid_discount_code_fake99_total_remains_one_hundred()` - Verifica que el total permanece en $100

### Scenario 4: Calcular impuestos correctamente
- `test_calculate_tax_with_eight_percent_tax_amount_is_eight()` - Verifica que el impuesto sea $8
- `test_calculate_tax_with_eight_percent_total_is_one_hundred_eight()` - Verifica que el total sea $108

### Scenario 5: Vaciar el carrito
- `test_clear_cart_with_three_products_cart_is_empty()` - Verifica que el carrito esté vacío
- `test_clear_cart_with_three_products_total_is_zero()` - Verifica que el total sea $0

## 🔧 Tecnologías

- **Framework**: NUnit 3.13.3
- **Mocking**: Moq 4.18.4
- **Target**: .NET 6.0

## 🚀 Ejecución

### Requisitos previos
```bash
dotnet --version  # Asegúrate de tener .NET 6.0+
```

### Ejecutar todos los tests
```bash
dotnet test tests/ShoppingCartService.Tests.csproj
```

### Ejecutar un test específico
```bash
dotnet test tests/ShoppingCartService.Tests.csproj --filter "test_add_product_to_empty_cart_product_count_is_one"
```

### Ejecutar con verbosidad
```bash
dotnet test tests/ShoppingCartService.Tests.csproj --verbosity normal
```

### Ejecutar con coverage
```bash
dotnet test tests/ShoppingCartService.Tests.csproj /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## 📊 Convenciones de Nombres

Todos los tests siguen el patrón: `test_[scenario_description]`

- `test_add_product_to_empty_cart_product_count_is_one`
- `test_apply_valid_discount_code_summer20_total_is_eighty`
- `test_calculate_tax_with_eight_percent_tax_amount_is_eight`

## 🏗️ Arquitectura de Tests

Cada test contiene:

1. **Setup (@SetUp)**: Inicializa mocks y el servicio
2. **Arrange**: Prepara los datos de entrada
3. **Act**: Ejecuta la acción a probar
4. **Assert**: Verifica los resultados esperados

### Ejemplo de estructura:
```csharp
[SetUp]
public void Setup()
{
    // Inicializar mocks
    _mockDiscountRepository = new Mock<IDiscountRepository>();
    _mockTaxService = new Mock<ITaxService>();
    
    // Crear instancia del servicio con mocks
    _cartService = new ShoppingCartService(
        _mockDiscountRepository.Object,
        _mockTaxService.Object
    );
}

[Test]
public void test_add_product_to_empty_cart_product_count_is_one()
{
    // Arrange
    var product = new Product("PROD001", "Test Product", 100m);
    
    // Act
    _cartService.AddProduct(product);
    
    // Assert
    Assert.That(_cartService.GetProductCount(), Is.EqualTo(1));
}
```

## 🎯 Mocks Utilizados

- **IDiscountRepository**: Simula el acceso a códigos de descuento en BD
- **ITaxService**: Simula el cálculo de impuestos desde servicio externo

## ✅ Checklist de Implementación (TDD)

Sigue estos pasos para implementar el código:

- [ ] Todos los tests deben fallar inicialmente (RED)
- [ ] Implementa las clases e interfaces necesarias
- [ ] Los tests deben pasar (GREEN)
- [ ] Refactoriza si es necesario (REFACTOR)

## 📝 Notas

- Los tests son **agnósticos** de la implementación
- Utilizan **mocks** para aislar el código bajo prueba
- Cada test valida un aspecto específico del comportamiento
- Los tests están listos para ejecutarse inmediatamente

---

**Autor**: Jaime Deep Learning Bot  
**Fecha**: 2026-06-02  
**Patrón**: Test-Driven Development (TDD)
