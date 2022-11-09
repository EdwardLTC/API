using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controller
{
    [Route("api/")]
    [ApiController]
    public class ClothesController : ControllerBase
    {
        private readonly EdwardEcomerceContext _context;

        public ClothesController(EdwardEcomerceContext context)
        {
            _context = context;
        }

        [Route("GetAllClothes")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clothe>>> GetClothes()
        {
            return await _context.Clothes.ToListAsync();
        }

        // GET: api/Clothes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clothe>> GetClothe(int id)
        {
            var clothe = await _context.Clothes.FindAsync(id);

            if (clothe == null)
            {
                return NotFound();
            }

            return clothe;
        }

        // PUT: api/Clothes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClothe(int id, Clothe clothe)
        {
            if (id != clothe.Id)
            {
                return BadRequest();
            }

            _context.Entry(clothe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClotheExists(id))
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

        // POST: api/Clothes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Clothe>> PostClothe(Clothe clothe)
        {
            _context.Clothes.Add(clothe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClothe", new { id = clothe.Id }, clothe);
        }

        // DELETE: api/Clothes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClothe(int id)
        {
            var clothe = await _context.Clothes.FindAsync(id);
            if (clothe == null)
            {
                return NotFound();
            }

            _context.Clothes.Remove(clothe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClotheExists(int id)
        {
            return _context.Clothes.Any(e => e.Id == id);
        }
    }
}
