
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using EdwardEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controller
{
    [Route("api/")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly EdwardEcomerceContext _context;

        public CategoriesController(EdwardEcomerceContext context)
        {
            _context = context;
        }

        [Route("GetAllCategory")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListCategory>>> GetCategories()
        {
            ResGetListCategory _res = new ResGetListCategory();
            List<Category> categories = await _context.Categories.ToListAsync();
            List<CategoryRes> resList = new List<CategoryRes>();
            foreach (Category category in categories)
            {
                resList.Add(new CategoryRes
                {
                    Id = category.Id,
                    Name = category.Name,
                });
            }

            _res._Respon = new Respon { respone_code = 200, Status = "Success" };
            _res._CategoryRes = resList;
            return Ok(_res);

        }

        [Route("GetCategoryWhere")]
        [HttpGet]
        public async Task<ActionResult<ResGetCategory>> GetCategory(int id)
        {
            ResGetCategory _resGetCategory = new();
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _resGetCategory._Respon = new Respon { respone_code = 404, Status = "Does not exist" };
                return Ok(_resGetCategory);
            }
            CategoryRes res = new()
            {
                Id = category.Id,
                Name = category.Name,
            };

            _resGetCategory._Respon = new Respon { respone_code = 200, Status = "Success" };
            _resGetCategory._CategoryRes = res;
            return Ok(_resGetCategory);

        }

        [Route("UpdateCategory")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PutCategory(CategoryReq categoryReq)
        {
            var _category = new Category
            {
                Id = categoryReq.Id,
                Name = categoryReq.Name
            };
            _context.Entry(_category).State = EntityState.Modified;
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(categoryReq.Id))
                {
                    return Ok(new Respon { respone_code = 404, Status = "not found" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Respon { respone_code = 200, Status = "Success" }); ;
        }

        [Route("CreateCategory")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PostCategory(CategoryReq categoryReq)
        {
            try
            {
                var category = new Category
                {
                    Name = categoryReq.Name
                };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return Ok(new Respon { respone_code = 400, Status = "bad Request" });
            }


            return Ok(new Respon { respone_code = 200, Status = "Success" });
        }

        [Route("DeleteCategory")]
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Respon res = new();
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    res.respone_code = 404;
                    res.Status = "Not Found";
                    return Ok(res);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch
            {
                res.respone_code = 400;
                res.Status = "bad Request";
                return Ok(res);
            }
            res.respone_code = 200;
            res.Status = "Success";
            return Ok(res);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
