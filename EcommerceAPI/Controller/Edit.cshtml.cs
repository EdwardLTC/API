using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controller
{
    public class EditModel : PageModel
    {
        private readonly EcommerceAPI.Models.EdwardEcomerceContext _context;

        public EditModel(EcommerceAPI.Models.EdwardEcomerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Clothe Clothe { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Clothes == null)
            {
                return NotFound();
            }

            var clothe =  await _context.Clothes.FirstOrDefaultAsync(m => m.Id == id);
            if (clothe == null)
            {
                return NotFound();
            }
            Clothe = clothe;
           ViewData["IdCategory"] = new SelectList(_context.Categories, "Id", "Id");
           ViewData["Idseller"] = new SelectList(_context.People, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Clothe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClotheExists(Clothe.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ClotheExists(int id)
        {
          return _context.Clothes.Any(e => e.Id == id);
        }
    }
}
