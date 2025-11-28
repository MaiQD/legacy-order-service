using FluentAssertions;
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
        const string productName = "Widget";
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
        const string productName = "Gadget";
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
        const string productName = "Doohickey";
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
        act.Should().Throw<Exception>().WithMessage("Product not found");
    }
}

