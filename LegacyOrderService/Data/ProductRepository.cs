using LegacyOrderService.Constants;

namespace LegacyOrderService.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly Dictionary<string, double> _productPrices = new()
        {
            [ProductNames.Widget] = 12.99,
            [ProductNames.Gadget] = 15.49,
            [ProductNames.Doohickey] = 8.75
        };

        public double GetPrice(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be null or empty", nameof(productName));

            // Simulate an expensive lookup
            Thread.Sleep(500);

            if (_productPrices.TryGetValue(productName, out var price))
                return price;

            throw new Exception(ErrorMessages.ProductNotFound);
        }
    }
}
