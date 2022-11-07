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
    public class IndexModel : PageModel
    {
        private readonly EcommerceAPI.Models.EdwardEcomerceContext _context;

        public IndexModel(EcommerceAPI.Models.EdwardEcomerceContext context)
        {
            _context = context;
        }

        public IList<Clothe> Clothe { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Clothes != null)
            {
                Clothe = await _context.Clothes
                .Include(c => c.IdCategoryNavigation)
                .Include(c => c.IdsellerNavigation).ToListAsync();
            }
        }
    }
}
