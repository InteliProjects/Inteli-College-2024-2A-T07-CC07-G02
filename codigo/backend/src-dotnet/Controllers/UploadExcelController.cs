using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using OfficeOpenXml;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UploadExcelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UploadExcelController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload-inventory")]
        public async Task<IActionResult> UploadInventoryExcelFile(IFormFile file)
        {
            if (file == null || !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Invalid file format. Please upload an Excel file.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Ensure the table is cleared before the new insertions
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Inventory");

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            var sql = new StringBuilder();
                            sql.Append("INSERT INTO Inventory (Sku, OfficeNum, Quantity) VALUES");

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var SKU = worksheet.Cells[row, 1].Text;
                                var CD = worksheet.Cells[row, 2].Text;
                                var QTDE = worksheet.Cells[row, 3].Text;
                                int quantity = int.TryParse(QTDE, out var q) ? q : 0;

                                SKU = string.IsNullOrWhiteSpace(SKU) ? "NULL" : $"'{SKU}'";
                                CD = string.IsNullOrWhiteSpace(CD) ? "NULL" : $"'{CD}'";
                                QTDE = string.IsNullOrWhiteSpace(QTDE) ? "NULL" : QTDE;

                                sql.Append($"({SKU}, {CD}, {QTDE}),");
                            }
                            sql.Length--;

                            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

                            // Commit transaction
                            await transaction.CommitAsync();
                            return Ok($"{rowCount - 1} records added to Inventory successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Rollback transaction if there's an error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }

        [HttpPost("upload-products")]
        public async Task<IActionResult> UploadProductsExcelFile(IFormFile file)
        {
            if (file == null || !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Invalid file format. Please upload an Excel file.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Ensure the table is cleared before the new insertions
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Product");

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            var sql = new StringBuilder();
                            sql.Append("INSERT INTO Product (Sku, ProductName, ImagePath) VALUES");

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var sku = worksheet.Cells[row, 1].Text;
                                var productName = worksheet.Cells[row, 2].Text;
                                var imagePath = worksheet.Cells[row, 3].Text;

                                sku = string.IsNullOrWhiteSpace(sku) ? "NULL" : $"'{sku}'";
                                productName = string.IsNullOrWhiteSpace(productName) ? "NULL" : $"'{productName}'";
                                imagePath = string.IsNullOrWhiteSpace(imagePath) ? "NULL" : $"'{imagePath}'";

                                sql.Append($"({sku}, {productName}, {imagePath}),");
                            }
                            sql.Length--;

                            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

                            // Commit transaction
                            await transaction.CommitAsync();
                            return Ok($"{rowCount - 1} records added to Product successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Rollback transaction if there's an error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }

        [HttpPost("upload-stores")]
        public async Task<IActionResult> UploadStoresExcelFile(IFormFile file)
        {
            if (file == null || !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Invalid file format. Please upload an Excel file.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Ensure the table is cleared before the new insertions
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Store");

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            var sql = new StringBuilder();
                            sql.Append("INSERT INTO Store (OfficeNum, Cep, Status, DeliveryTime) VALUES");

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var officeNum = worksheet.Cells[row, 1].Text;
                                var cep = worksheet.Cells[row, 2].Text;
                                var status = worksheet.Cells[row, 3].Text;
                                var deliveryTime = worksheet.Cells[row, 4].Text;

                                officeNum = string.IsNullOrWhiteSpace(officeNum) ? "NULL" : $"'{officeNum}'";
                                cep = string.IsNullOrWhiteSpace(cep) ? "NULL" : $"'{cep}'";
                                status = string.IsNullOrWhiteSpace(status) ? "NULL" : $"'{status}'";
                                deliveryTime = string.IsNullOrWhiteSpace(deliveryTime) ? "NULL" : $"'{deliveryTime}'";

                                sql.Append($"({officeNum}, {cep}, {status}, {deliveryTime}),");
                            }
                            sql.Length--;

                            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

                            // Commit transaction
                            await transaction.CommitAsync();
                            return Ok($"{rowCount - 1} records added to Store successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Rollback transaction if there's an error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }



        [HttpPost("upload-available-ceps")]
        public async Task<IActionResult> UploadAvailableCepsExcelFile(IFormFile file)
        {
            if (file == null || !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Invalid file format. Please upload an Excel file.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Ensure the table is cleared before the new insertions
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM AvailableCeps");

                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            var sql = new StringBuilder();
                            sql.Append("INSERT INTO AvailableCeps (OfficeNum, Cep) VALUES");

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var officeNum = worksheet.Cells[row, 1].Text;
                                var cep = worksheet.Cells[row, 2].Text;

                                officeNum = string.IsNullOrWhiteSpace(officeNum) ? "NULL" : $"'{officeNum}'";
                                cep = string.IsNullOrWhiteSpace(cep) ? "NULL" : $"'{cep}'";

                                sql.Append($"({officeNum}, {cep}),");
                            }
                            sql.Length--;

                            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

                            // Commit transaction
                            await transaction.CommitAsync();
                            return Ok($"{rowCount - 1} records added to AvailableCeps successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Rollback transaction if there's an error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
