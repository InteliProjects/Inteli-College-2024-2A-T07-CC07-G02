using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.Testes
{
    public class DeliveryTimeControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeliveryTimeController _controller;

        public DeliveryTimeControllerTests()
        {
            // Configura o contexto de banco de dados em memóri
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Inicializa o controlador com o contexto em memória
            _controller = new DeliveryTimeController(_context);

            // Popula o banco de dados com dados de teste
            SeedDatabase();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedDatabase()
        {
            // Adiciona inventário
            _context.Inventory.AddRange(new List<Inventory>
            {
                new Inventory { Sku = "SKU123", OfficeNum = "OFF001", Quantity = 10 },
                new Inventory { Sku = "SKU123", OfficeNum = "OFF002", Quantity = 15 }
            });

            // Adiciona lojas
            _context.Store.AddRange(new List<Stores>
            {
                new Stores { OfficeNum = "OFF001", Cep = "00000-000", Status = "Ativo", DeliveryTime = "30" },
                new Stores { OfficeNum = "OFF002", Cep = "00000-000", Status = "Ativo", DeliveryTime = "45" } // Mesmo CEP genérico, mas maior tempo de entrega
            });

            // Adiciona ceps disponíveis (associados a OfficeNum)
            _context.AvailableCeps.AddRange(new List<AvailableCeps>
            {
                new AvailableCeps { Id = 1, OfficeNum = "OFF001", Cep = "12345-678" },
                new AvailableCeps { Id = 2, OfficeNum = "OFF002", Cep = "12345-678" }
            });

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetDeliveryTime_ReturnsBadRequest_WhenSkuOrCepIsMissing()
        {
            // Arrange
            var dtoMissingSku = new DeliveryTimeDto { Sku = "", Cep = "12345-678" };
            var dtoMissingCep = new DeliveryTimeDto { Sku = "SKU123", Cep = "" };

            // Act
            var resultMissingSku = await _controller.GetDeliveryTime(dtoMissingSku);
            var resultMissingCep = await _controller.GetDeliveryTime(dtoMissingCep);

            // Assert
            var badRequestResult1 = Assert.IsType<BadRequestObjectResult>(resultMissingSku);
            var badRequestResult2 = Assert.IsType<BadRequestObjectResult>(resultMissingCep);
            Assert.Equal("Sku e CEP são obrigatórios!", ((dynamic)badRequestResult1.Value).Message);
            Assert.Equal("Sku e CEP são obrigatórios!", ((dynamic)badRequestResult2.Value).Message);
        }

        [Fact]
        public async Task GetDeliveryTime_ReturnsBadRequest_WhenCepIsInvalid()
        {
            // Arrange
            var dto = new DeliveryTimeDto { Sku = "SKU123", Cep = "invalid-cep" };

            // Act
            var result = await _controller.GetDeliveryTime(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro de formatação no CEP: CEP inválido, deve conter 8 dígitos numéricos", ((dynamic)badRequestResult.Value).Message);
        }

        [Fact]
        public async Task GetDeliveryTime_ReturnsNotFound_WhenNoStoreForSku()
        {
            // Arrange
            var dto = new DeliveryTimeDto { Sku = "SKU999", Cep = "12345-678" };  // SKU que não existe no banco

            // Act
            var result = await _controller.GetDeliveryTime(dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Nenhuma loja encontrada para o Sku fornecido.", ((dynamic)notFoundResult.Value).Message);
        }

        [Fact]
        public async Task GetDeliveryTime_ReturnsNotFound_WhenNoStoreWithCepAvailable()
        {
            // Arrange
            var dto = new DeliveryTimeDto { Sku = "SKU123", Cep = "99999-999" };  // CEP que não está disponível

            // Act
            var result = await _controller.GetDeliveryTime(dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Nenhuma loja encontrada com o Sku e CEP fornecidos.", ((dynamic)notFoundResult.Value).Message);
        }

        [Fact]
        public async Task GetDeliveryTime_ReturnsOk_WhenStoreWithCepIsFound()
        {
            // Arrange
            var dto = new DeliveryTimeDto { Sku = "SKU123", Cep = "12345-678" };

            // Act
            var result = await _controller.GetDeliveryTime(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value as dynamic;
            Assert.Equal("OFF001", returnValue.Loja);
            Assert.Equal("30", returnValue.TempoEntrega);
        }

        [Fact]
        public async Task GetDeliveryTime_ReturnsStoreWithLowestDeliveryTime_WhenMultipleStoresHaveSameSkuAndCepInAvailableCeps()
        {
            // Arrange
            var dto = new DeliveryTimeDto { Sku = "SKU123", Cep = "12345-678" };

            // Act
            var result = await _controller.GetDeliveryTime(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value as dynamic;

            // Verifica se a loja retornada é a que tem o menor tempo de entrega (OFF001 tem 30 minutos, OFF002 tem 45 minutos)
            Assert.Equal("OFF001", returnValue.Loja);
            Assert.Equal("30", returnValue.TempoEntrega);
        }
    }
}