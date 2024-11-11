using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers;  // Certifique-se de que este namespace está correto
using Backend.Models;       // Certifique-se de que este namespace está correto
using Backend.Data;         // Certifique-se de que este namespace está correto
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;                   // Certifique-se de que você tem o pacote Moq instalado
using Xunit;
using System;

namespace Backend.Testes
{
    public class StoresControllerTests : IDisposable
    {
        private DbContextOptions<ApplicationDbContext> _contextOptions;
        private ApplicationDbContext _context;

        public StoresControllerTests()
        {
            // Gerar um novo banco de dados em memória para cada teste
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Gera um nome único para cada teste
                .Options;

            _context = new ApplicationDbContext(_contextOptions);
        }

        // Método para limpar o banco de dados após cada teste
        public void Dispose()
        {
            _context.Store.RemoveRange(_context.Store);  // Remove todos os itens
            _context.SaveChanges();  // Salva as alterações
        }

        [Fact]
        public async Task GetStores_ReturnsStoresList()
        {
            // Arrange
            _context.Store.Add(new Stores { OfficeNum = "1", Cep = "12345-678", Status = "Ativo", DeliveryTime = "30" });
            _context.Store.Add(new Stores { OfficeNum = "2", Cep = "87654-321", Status = "Inativo", DeliveryTime = "45" });
            await _context.SaveChangesAsync();

            var controller = new StoresController(_context);

            // Act
            var result = await controller.GetStores();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Stores>>>(result);
            var stores = Assert.IsAssignableFrom<IEnumerable<Stores>>(actionResult.Value);
            Assert.Equal(2, stores.Count());
        }

        [Fact]
        public async Task GetStore_ReturnsNotFound_WhenStoreDoesNotExist()
        {
            // Arrange
            var controller = new StoresController(_context);

            // Act
            var result = await controller.GetStore("999");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetStore_ReturnsStore_WhenStoreExists()
        {
            // Arrange
            var store = new Stores { OfficeNum = "1", Cep = "12345-678", Status = "Ativo", DeliveryTime = "30" };
            _context.Store.Add(store);
            await _context.SaveChangesAsync();

            var controller = new StoresController(_context);

            // Act
            var result = await controller.GetStore("1");

            // Assert
            var actionResult = Assert.IsType<ActionResult<Stores>>(result);
            var returnValue = Assert.IsType<Stores>(actionResult.Value);
            Assert.Equal("1", returnValue.OfficeNum);
        }

        [Fact]
        public async Task PostStore_AddsStoreSuccessfully()
        {
            // Arrange
            var controller = new StoresController(_context);
            var store = new Stores { OfficeNum = "3", Cep = "54321-098", Status = "Ativo", DeliveryTime = "25" };

            // Act
            var result = await controller.PostStore(store);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Stores>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Stores>(createdAtActionResult.Value);
            Assert.Equal("3", returnValue.OfficeNum);
        }

        [Fact]
        public async Task DeleteStore_RemovesStoreSuccessfully()
        {
            // Arrange
            var store = new Stores { OfficeNum = "1", Cep = "12345-678", Status = "Ativo", DeliveryTime = "30" };
            _context.Store.Add(store);
            await _context.SaveChangesAsync();

            var controller = new StoresController(_context);

            // Act
            var result = await controller.DeleteStore("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Store.FindAsync("1"));
        }
    }
}
