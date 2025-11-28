namespace LegacyOrderService.Models;

public record ProcessOrderRequest(string CustomerName, string ProductName, int Quantity);

