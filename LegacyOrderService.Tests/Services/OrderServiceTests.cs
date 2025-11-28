using FluentAssertions;
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
        const string productName = "Widget";
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
        const string productName = "Gadget";
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
        const string productName = "Doohickey";
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
}

