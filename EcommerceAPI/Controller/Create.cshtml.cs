using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controller
{
    public class CreateModel : PageModel
    {
        private readonly EcommerceAPI.Models.EdwardEcomerceContext _context;

        public CreateModel(EcommerceAPI.Models.EdwardEcomerceContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["IdCategory"] = new SelectList(_context.Categories, "Id", "Id");
        ViewData["Idseller"] = new SelectList(_context.People, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Clothe Clothe { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Clothes.Add(Clothe);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
