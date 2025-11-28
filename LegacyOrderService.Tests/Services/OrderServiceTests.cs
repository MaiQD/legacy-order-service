using FluentAssertions;
using FluentValidation;
using LegacyOrderService.Constants;
using LegacyOrderService.Data;
using LegacyOrderService.Models;
using LegacyOrderService.Services;
using Moq;

namespace LegacyOrderServiceTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task ProcessOrderAsync_ValidInputs_ReturnsOrderWithCorrectProperties()
    {
        // Arrange
        const string customerName = "John Doe";
        const string productName = ProductNames.Widget;
        const int quantity = 5;
        const double expectedPrice = 12.99;

        _productRepositoryMock
            .Setup(x => x.GetPrice(productName))
            .Returns(expectedPrice);

        // Act
        var result = await _orderService.ProcessOrderAsync(customerName, productName, quantity);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be(customerName);
        result.ProductName.Should().Be(productName);
        result.Quantity.Should().Be(quantity);
        result.Price.Should().Be(expectedPrice);
        result.Total.Should().Be(quantity * expectedPrice);
    }

    [Fact]
    public async Task ProcessOrderAsync_ValidInputs_CallsProductRepositoryGetPrice()
    {
        // Arrange
        const string customerName = "Jane Smith";
        const string productName = ProductNames.Gadget;
        const int quantity = 3;
        const double expectedPrice = 15.49;

        _productRepositoryMock
            .Setup(x => x.GetPrice(productName))
            .Returns(expectedPrice);

        // Act
        await _orderService.ProcessOrderAsync(customerName, productName, quantity);

        // Assert
        _productRepositoryMock.Verify(x => x.GetPrice(productName), Times.Once);
    }

    [Fact]
    public async Task ProcessOrderAsync_ValidInputs_CallsOrderRepositorySaveAsync()
    {
        // Arrange
        const string customerName = "Bob Johnson";
        const string productName = ProductNames.Doohickey;
        const int quantity = 10;
        const double expectedPrice = 8.75;

        _productRepositoryMock
            .Setup(x => x.GetPrice(productName))
            .Returns(expectedPrice);

        // Act
        await _orderService.ProcessOrderAsync(customerName, productName, quantity);

        // Assert
        _orderRepositoryMock.Verify(x => x.SaveAsync(It.Is<Order>(o =>
            o.CustomerName == customerName &&
            o.ProductName == productName &&
            o.Quantity == quantity &&
            o.Price == expectedPrice
        )), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ProcessOrderAsync_InvalidCustomerName_ThrowsValidationException(string? customerName)
    {
        // Arrange
        const string productName = ProductNames.Widget;
        const int quantity = 5;

        // Act
        var act = async () => await _orderService.ProcessOrderAsync(customerName!, productName, quantity);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ProcessOrderAsync_InvalidProductName_ThrowsValidationException(string? productName)
    {
        // Arrange
        const string customerName = "John Doe";
        const int quantity = 5;

        // Act
        var act = async () => await _orderService.ProcessOrderAsync(customerName, productName!, quantity);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ProcessOrderAsync_InvalidQuantity_ThrowsValidationException(int quantity)
    {
        // Arrange
        const string customerName = "John Doe";
        const string productName = ProductNames.Widget;

        // Act
        var act = async () => await _orderService.ProcessOrderAsync(customerName, productName, quantity);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}

