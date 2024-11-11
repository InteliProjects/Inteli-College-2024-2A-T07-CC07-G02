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
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            return await _context.Inventory.ToListAsync();
        }

        // GET: Inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        // PUT: Inventory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return BadRequest();
            }

            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Inventory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventory", new { id = inventory.InventoryId }, inventory);
        }

        // DELETE: Inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventory.Any(e => e.InventoryId == id);
        }

        // GET: /inventory/sku/{sku}
        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetInventoryBySku(string sku)
        {
            var inventoryItem = await _context.Inventory
                                              .FirstOrDefaultAsync(i => i.Sku == sku);

            if (inventoryItem == null)
            {
                return NotFound(new { Message = $"Inventory item with SKU '{sku}' not found." });
            }

            return Ok(inventoryItem);
        }


        // POST: /inventory/process-cart
        [HttpPost("process-cart")]
        public async Task<IActionResult> ProcessCart([FromBody] CartDto cart)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var errors = new List<string>();

                // Loop through each item in the cart
                foreach (CartItem item in cart.Items)
                {
                    // Retrieve the inventory item matching Sku and OfficeNum
                    var inventoryItem = await _context.Inventory
                        .FirstOrDefaultAsync(i => i.Sku == item.Sku && i.OfficeNum == item.OfficeNum);

                    var product = await _context.Product
                        .FirstOrDefaultAsync(p => p.Sku == item.Sku);

                    if (inventoryItem == null)
                    {
                        errors.Add($"Item com nome '{item.Sku}' and OfficeNum '{item.OfficeNum}' não encontrado no estoque.");
                        continue;
                    }

                    if (item.Quantity > inventoryItem.Quantity)
                    {
                        errors.Add($"Estoque insuficiente para produto '{product.ProductName}' na loja com OfficeNum '{item.OfficeNum}'. Quantidade no carrinho: {item.Quantity}, Quantidade disponível: {inventoryItem.Quantity}");
                        continue;
                    }

                    // Deduct the requested quantity from inventory
                    inventoryItem.Quantity -= item.Quantity;

                    // Mark the inventory item as modified
                    _context.Inventory.Update(inventoryItem);
                }

                if (errors.Any())
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Algum item não pode ser processado",
                        errors = errors
                    });
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Cart processed successfully."
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("A concurrency conflict occurred. Please try again.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
