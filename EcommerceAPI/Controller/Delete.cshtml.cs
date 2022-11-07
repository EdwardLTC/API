using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controller
{
    public class DeleteModel : PageModel
    {
        private readonly EcommerceAPI.Models.EdwardEcomerceContext _context;

        public DeleteModel(EcommerceAPI.Models.EdwardEcomerceContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Clothe Clothe { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Clothes == null)
            {
                return NotFound();
            }

            var clothe = await _context.Clothes.FirstOrDefaultAsync(m => m.Id == id);

            if (clothe == null)
            {
                return NotFound();
            }
            else 
            {
                Clothe = clothe;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Clothes == null)
            {
                return NotFound();
            }
            var clothe = await _context.Clothes.FindAsync(id);

            if (clothe != null)
            {
                Clothe = clothe;
                _context.Clothes.Remove(Clothe);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
