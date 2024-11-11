using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeliveryTimeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeliveryTimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeliveryTime/
        [HttpPost]
        public async Task<IActionResult> GetDeliveryTime(DeliveryTimeDto dto)
        {
            var sku = dto.Sku;
            var cep = dto.Cep;

            if (string.IsNullOrEmpty(sku) || string.IsNullOrEmpty(cep))
            {
                return BadRequest(new { Message = "Sku e CEP são obrigatórios!" });
            }

            try
            {
                cep = FormatCep(cep);
            }
            catch (ArgumentException e)
            {
                return BadRequest(new { Message = $"Erro de formatação no CEP: {e.Message}" });
            }

            // 1. Buscar os OfficeNums da tabela Inventory
            var officeNums = await _context.Inventory
                .Where(i => i.Sku == sku)
                .Select(i => i.OfficeNum)
                .ToListAsync();

            if (!officeNums.Any())
            {
                return NotFound(new { Message = "Nenhuma loja encontrada para o Sku fornecido." });
            }

            // 2. Buscar as lojas e ordenar pelo menor tempo de entrega
            var lojas = await _context.Store
                .Where(s => officeNums.Contains(s.OfficeNum))
                .OrderBy(s => s.DeliveryTime)
                .ToListAsync();

            if (!lojas.Any())
            {
                return NotFound(new { Message = "Nenhuma loja encontrada com tempo de entrega disponível." });
            }

            // 3. Verificar a tabela AvailableCeps para encontrar a primeira loja com o CEP correspondente
            foreach (var loja in lojas)
            {
                var cepDisponivel = await _context.AvailableCeps
                    .AnyAsync(ac => ac.OfficeNum == loja.OfficeNum && ac.Cep == cep);

                if (cepDisponivel)
                {
                    return Ok(new
                    {
                        Loja = loja.OfficeNum,
                        TempoEntrega = loja.DeliveryTime
                    });
                }
            }

            return NotFound(new { Message = "Nenhuma loja encontrada com o Sku e CEP fornecidos." });
        }

        // Função para garantir que o CEP esteja no formato XXXXX-XXX
        private string FormatCep(string cep)
        {
            cep = cep.Replace("-", ""); // Remover traços
            if (cep.Length != 8 || !long.TryParse(cep, out _))
            {
                throw new ArgumentException("CEP inválido, deve conter 8 dígitos numéricos");
            }
            return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }
    }
}
