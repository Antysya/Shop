using Microsoft.AspNetCore.Mvc;
using Shop;
using System.Collections.Concurrent;

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

        public Product GetProduct(string productName)
        {
            return _products.First(p => p.Name == productName);
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

        //ДЗ3-Многопоточность

        //async-await

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1); //задаем количество разрешений
        public async Task<List<Product>> GetProductsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _products.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Product> GetProductAsync(string productName)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _products.First(p => p.Name == productName);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task AddProductsAsync(Product product)
        {
            await _semaphore.WaitAsync();
            try
            {
                _products.Add(product);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> RemoveProductsAsync(Product product)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _products.Remove(product);
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}


