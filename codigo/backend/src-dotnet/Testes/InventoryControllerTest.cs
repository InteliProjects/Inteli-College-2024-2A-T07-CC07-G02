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
    public class InventoryControllerTests : IDisposable
    {
        private DbContextOptions<ApplicationDbContext> _contextOptions;
        private ApplicationDbContext _context;

        public InventoryControllerTests()
        {
            // Criar um banco de dados em memória
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_contextOptions);
        }

        public void Dispose()
        {
            _context.Inventory.RemoveRange(_context.Inventory);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetInventory_ReturnsInventoryList()
        {
            // Arrange
            _context.Inventory.Add(new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" });
            _context.Inventory.Add(new Inventory { InventoryId = 2, Sku = "B456", Quantity = 20, OfficeNum = "002" });
            await _context.SaveChangesAsync();

            var controller = new InventoryController(_context);

            // Act
            var result = await controller.GetInventory();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Inventory>>>(result);
            var inventory = Assert.IsAssignableFrom<IEnumerable<Inventory>>(actionResult.Value);
            Assert.Equal(2, inventory.Count());
        }

        [Fact]
        public async Task GetInventoryById_ReturnsInventory_WhenInventoryExists()
        {
            // Arrange
            var inventory = new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" };
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            var controller = new InventoryController(_context);

            // Act
            var result = await controller.GetInventory(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Inventory>>(result);
            var returnValue = Assert.IsType<Inventory>(actionResult.Value);
            Assert.Equal(1, returnValue.InventoryId);
        }

        [Fact]
        public async Task GetInventoryById_ReturnsNotFound_WhenInventoryDoesNotExist()
        {
            // Arrange
            var controller = new InventoryController(_context);

            // Act
            var result = await controller.GetInventory(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostInventory_AddsInventorySuccessfully()
        {
            // Arrange
            var controller = new InventoryController(_context);
            var inventory = new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" };

            // Act
            var result = await controller.PostInventory(inventory);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Inventory>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Inventory>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.InventoryId);
        }

        [Fact]
        public async Task PutInventory_UpdatesInventorySuccessfully()
        {
            // Arrange
            var inventory = new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" };
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            var controller = new InventoryController(_context);

            // Act
            inventory.Quantity = 15;
            var result = await controller.PutInventory(1, inventory);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedInventory = await _context.Inventory.FindAsync(1);
            Assert.Equal(15, updatedInventory.Quantity);
        }

        [Fact]
        public async Task PutInventory_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var inventory = new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" };
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            var controller = new InventoryController(_context);

            // Act
            var result = await controller.PutInventory(2, inventory);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteInventory_RemovesInventorySuccessfully()
        {
            // Arrange
            var inventory = new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" };
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            var controller = new InventoryController(_context);

            // Act
            var result = await controller.DeleteInventory(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Inventory.FindAsync(1));
        }

        [Fact]
        public async Task ProcessCart_ReturnsSuccess_WhenInventoryIsSufficient()
        {
            // Arrange
            _context.Inventory.Add(new Inventory { InventoryId = 1, Sku = "A123", Quantity = 10, OfficeNum = "001" });
            _context.Product.Add(new Products { Sku = "A123", ProductName = "Product A" });
            await _context.SaveChangesAsync();

            var controller = new InventoryController(_context);
            var cart = new CartDto
            {
                Items = new List<CartItem>
                {
                    new CartItem { Sku = "A123", OfficeNum = "001", Quantity = 5 }
                }
            };

            // Act
            var result = await controller.ProcessCart(cart);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Use dynamic typing to access anonymous type properties
            dynamic response = okResult.Value;
            Assert.Equal("success", response.status);  // Check if the status is "success"

            var inventoryItem = await _context.Inventory.FindAsync(1);
            Assert.Equal(5, inventoryItem.Quantity);
        }

    }
}
