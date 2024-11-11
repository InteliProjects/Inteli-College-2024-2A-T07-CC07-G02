using System.IO;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Xunit;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backend.Testes
{
    public class UploadExcelControllerTests : IDisposable
    {
        private DbContextOptions<ApplicationDbContext> _contextOptions;
        private ApplicationDbContext _context;

        public UploadExcelControllerTests()
        {
            // Usando SQLite em memória para testes, garantindo suporte a SQL relacional
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")  // Banco de dados SQLite em memória
                .Options;

            _context = new ApplicationDbContext(_contextOptions);
            _context.Database.OpenConnection(); // Abre a conexão com o banco em memória
            _context.Database.EnsureCreated();  // Garante que o schema do banco seja criado
        }

        public void Dispose()
        {
            _context.Database.CloseConnection(); // Fecha a conexão com o banco após o teste
        }

        [Fact]
        public async Task UploadStoresExcelFile_SuccessfullyUploadsStores()
        {
            // Arrange
            var controller = new UploadExcelController(_context);

            // Cria um arquivo Excel de exemplo em memória
            var file = GenerateExcelFileForStores();

            // Cria um mock do IFormFile para simular o upload
            var formFile = new FormFile(file, 0, file.Length, "file", "stores.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            // Act
            var result = await controller.UploadStoresExcelFile(formFile);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Verifica se é um OkObjectResult
            Assert.Equal(200, actionResult.StatusCode);  // Verifica se o código de status é 200 OK
            Assert.Equal("2 records added to Store successfully.", actionResult.Value);

            // Verifica se os dados foram inseridos corretamente no banco de dados
            Assert.Equal(2, _context.Store.Count());
        }

        [Fact]
        public async Task UploadInventoryExcelFile_SuccessfullyUploadsInventory()
        {
            // Arrange
            var controller = new UploadExcelController(_context);

            // Cria um arquivo Excel de exemplo em memória
            var file = GenerateExcelFileForInventory();

            // Cria um mock do IFormFile para simular o upload
            var formFile = new FormFile(file, 0, file.Length, "file", "inventory.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            // Act
            var result = await controller.UploadInventoryExcelFile(formFile);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Verifica se é um OkObjectResult
            Assert.Equal(200, actionResult.StatusCode);  // Verifica se o código de status é 200 OK
            Assert.Equal("2 records added to Inventory successfully.", actionResult.Value);

            // Verifica se os dados foram inseridos corretamente no banco de dados
            Assert.Equal(2, _context.Inventory.Count());
            Assert.Equal("SKU001", _context.Inventory.First().Sku);
            Assert.Equal(100, _context.Inventory.First().Quantity);
        }

        private MemoryStream GenerateExcelFileForStores()
        {
            var stream = new MemoryStream();

            // Cria um novo arquivo Excel
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Stores");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "OfficeNum";
                worksheet.Cells[1, 2].Value = "Cep";
                worksheet.Cells[1, 3].Value = "Status";
                worksheet.Cells[1, 4].Value = "DeliveryTime";

                // Dados de exemplo
                worksheet.Cells[2, 1].Value = "001";
                worksheet.Cells[2, 2].Value = "12345-678";
                worksheet.Cells[2, 3].Value = "Ativo";
                worksheet.Cells[2, 4].Value = "30";

                worksheet.Cells[3, 1].Value = "002";
                worksheet.Cells[3, 2].Value = "98765-432";
                worksheet.Cells[3, 3].Value = "Inativo";
                worksheet.Cells[3, 4].Value = "45";

                package.Save(); // Salva o pacote Excel
            }

            stream.Position = 0; // Reseta a posição para leitura
            return stream;
        }

        private MemoryStream GenerateExcelFileForInventory()
        {
            var stream = new MemoryStream();

            // Cria um novo arquivo Excel
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventory");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Sku";
                worksheet.Cells[1, 2].Value = "OfficeNum";
                worksheet.Cells[1, 3].Value = "Quantity";

                // Dados de exemplo
                worksheet.Cells[2, 1].Value = "SKU001";
                worksheet.Cells[2, 2].Value = "001";
                worksheet.Cells[2, 3].Value = "100";

                worksheet.Cells[3, 1].Value = "SKU002";
                worksheet.Cells[3, 2].Value = "002";
                worksheet.Cells[3, 3].Value = "200";

                package.Save(); // Salva o pacote Excel
            }

            stream.Position = 0; // Reseta a posição para leitura
            return stream;
        }
    }
}
