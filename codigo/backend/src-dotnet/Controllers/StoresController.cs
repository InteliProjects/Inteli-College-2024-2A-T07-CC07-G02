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
    public class StoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stores>>> GetStores()
        {
            return await _context.Store.ToListAsync();
        }

        // GET: Stores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stores>> GetStore(string id)
        {
            var stores = await _context.Store.FindAsync(id);

            if (stores == null)
            {
                return NotFound();
            }

            return stores;
        }

        // PUT: Stores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(string id, Stores stores)
        {
            if (id != stores.OfficeNum)
            {
                return BadRequest();
            }

            _context.Entry(stores).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: Stores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Stores>> PostStore(Stores stores)
        {
            _context.Store.Add(stores);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StoreExists(stores.OfficeNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStores", new { id = stores.OfficeNum }, stores);
        }

        // DELETE: Stores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(string id)
        {
            var stores = await _context.Store.FindAsync(id);
            if (stores == null)
            {
                return NotFound();
            }

            _context.Store.Remove(stores);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoreExists(string id)
        {
            return _context.Store.Any(e => e.OfficeNum == id);
        }
    }
}
