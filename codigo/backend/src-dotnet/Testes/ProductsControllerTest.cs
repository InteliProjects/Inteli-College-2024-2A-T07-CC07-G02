using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.Models;
using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;

namespace Backend.Tests
{
    public class ProductsControllerTests : IDisposable
    {
        private DbContextOptions<ApplicationDbContext> _contextOptions;
        private ApplicationDbContext _context;

        public ProductsControllerTests()
        {
            // Criar um banco de dados em memória
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_contextOptions);
        }

        public void Dispose()
        {
            _context.Product.RemoveRange(_context.Product);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProducts_ReturnsProductList()
        {
            // Arrange
            _context.Product.Add(new Products { Sku = "A123", ProductName = "Product A" });
            _context.Product.Add(new Products { Sku = "B456", ProductName = "Product B" });
            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context);

            // Act
            var result = await controller.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Products>>>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Products>>(actionResult.Value);
            Assert.Equal(2, products.Count());
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var product = new Products { Sku = "A123", ProductName = "Product A" };
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context);

            // Act
            var result = await controller.GetProduct("A123");

            // Assert
            var actionResult = Assert.IsType<ActionResult<Products>>(result);
            var returnValue = Assert.IsType<Products>(actionResult.Value);
            Assert.Equal("A123", returnValue.Sku);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var controller = new ProductsController(_context);

            // Act
            var result = await controller.GetProduct("999");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProduct_AddsProductSuccessfully()
        {
            // Arrange
            var controller = new ProductsController(_context);
            var product = new Products { Sku = "A123", ProductName = "Product A" };

            // Act
            var result = await controller.PostProduct(product);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Products>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Products>(createdAtActionResult.Value);
            Assert.Equal("A123", returnValue.Sku);
        }

        [Fact]
        public async Task PutProduct_UpdatesProductSuccessfully()
        {
            // Arrange
            var product = new Products { Sku = "A123", ProductName = "Product A" };
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context);

            // Act
            product.ProductName = "Updated Product A";
            var result = await controller.UpdateProduct("A123", product);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedProduct = await _context.Product.FindAsync("A123");
            Assert.Equal("Updated Product A", updatedProduct.ProductName);
        }

        [Fact]
        public async Task PutProduct_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var product = new Products { Sku = "A123", ProductName = "Product A" };
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context);

            // Act
            var result = await controller.UpdateProduct("B456", product);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_RemovesProductSuccessfully()
        {
            // Arrange
            var product = new Products { Sku = "A123", ProductName = "Product A" };
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context);

            // Act
            var result = await controller.DeleteProduct("A123");

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Product.FindAsync("A123"));
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var controller = new ProductsController(_context);

            // Act
            var result = await controller.DeleteProduct("999");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
