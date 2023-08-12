using Microsoft.AspNetCore.Mvc;
using Shop;

namespace Shop
{
    public class Catalog
    {
        private readonly List<Product> _products = new()
        {
            new("Чистый код", 500m),
            new("Код, помещающийся в голове", 1000m),
            new("Рефакторинг",  100m)
        };

        public Product[] GetProducts()
        {
            return _products.ToArray();
        }

        public void AddProducts(Product product)
        {
            _products.Add(product);
        }

        public void DelProducts(Product product)
        {
            _products.Remove(product);
        }

        public void PutProducts(string productName, Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Name == productName);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
            }
        }
    }
}


