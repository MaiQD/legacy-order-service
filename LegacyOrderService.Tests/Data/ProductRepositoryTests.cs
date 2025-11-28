using FluentAssertions;
using LegacyOrderService.Constants;
using LegacyOrderService.Data;

namespace LegacyOrderServiceTests.Data;

public class ProductRepositoryTests
{
    private readonly ProductRepository _productRepository;

    public ProductRepositoryTests()
    {
        _productRepository = new ProductRepository();
    }

    [Fact]
    public void GetPrice_Widget_ReturnsCorrectPrice()
    {
        // Arrange
        const string productName = ProductNames.Widget;
        const double expectedPrice = 12.99;

        // Act
        var result = _productRepository.GetPrice(productName);

        // Assert
        result.Should().Be(expectedPrice);
    }

    [Fact]
    public void GetPrice_Gadget_ReturnsCorrectPrice()
    {
        // Arrange
        const string productName = ProductNames.Gadget;
        const double expectedPrice = 15.49;

        // Act
        var result = _productRepository.GetPrice(productName);

        // Assert
        result.Should().Be(expectedPrice);
    }

    [Fact]
    public void GetPrice_Doohickey_ReturnsCorrectPrice()
    {
        // Arrange
        const string productName = ProductNames.Doohickey;
        const double expectedPrice = 8.75;

        // Act
        var result = _productRepository.GetPrice(productName);

        // Assert
        result.Should().Be(expectedPrice);
    }

    [Fact]
    public void GetPrice_NonExistentProduct_ThrowsException()
    {
        // Arrange
        const string productName = "NonExistentProduct";

        // Act
        var act = () => _productRepository.GetPrice(productName);

        // Assert
        act.Should().Throw<Exception>().WithMessage(ErrorMessages.ProductNotFound);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetPrice_InvalidProductName_ThrowsArgumentException(string? productName)
    {
        // Act
        var act = () => _productRepository.GetPrice(productName!);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("productName");
    }
}

