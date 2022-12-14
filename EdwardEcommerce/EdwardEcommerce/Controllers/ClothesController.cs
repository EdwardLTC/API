using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using EcommerceAPI.Models.Models_Responsive;
using EdwardEcommerce.Models;
using EdwardEcommerce.Models.Models_Respon;
using EdwardEcommerce.Models.Models_Responsive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetClothes()
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            List<Clothe> list = await _context.Clothes.ToListAsync();
            List<ClothesRes> res = new List<ClothesRes>();
            foreach (Clothe clothes in list)
            {
                var maxPrices = _context.ClothesProperties.Where(o => o.Idclothes == clothes.Id).Max(o => o.Price);
                var total = _context.ClothesProperties.Where(o => o.Idclothes == clothes.Id).Select(o => o.Quantily).Sum();
                string categoryName = _context.Categories.Where(o => o.Id == clothes.IdCategory).Select(o => o.Name).SingleOrDefault();
                ClothesRes respon = new ClothesRes
                {
                    Id = clothes.Id,
                    Name = clothes.Name,
                    Idseller = clothes.Idseller,
                    Des = clothes.Des,
                    IdCategory = clothes.IdCategory,
                    quantily = (int)total,
                    maxPrice = maxPrices.ToString(),
                    CategoryName = categoryName
                };
                List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == clothes.Id).Select(u => u.ImgUrl1).ToListAsync();
                respon.imgsUrl = listImgUrls;
                res.Add(respon);
            }
            resGetListClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListClothes._ClothesRes = res;
            return Ok(resGetListClothes);
        }

        [Route("GetClothesWhere")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetClothes>>> GetClothe(int id)
        {
            ResGetClothes resGetClothes = new ResGetClothes();
            var clothe = await _context.Clothes.FindAsync(id);
            List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == id).Select(u => u.ImgUrl1).ToListAsync();
            if (clothe == null)
            {
                resGetClothes._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(resGetClothes);
            }
            var total = _context.ClothesProperties.Where(o => o.Idclothes == id).Select(o => o.Quantily).Sum();
            var maxPrices = _context.ClothesProperties.Where(o => o.Idclothes == id).Max(o => o.Price);
            string categoryName = _context.Categories.Where(o => o.Id == clothe.IdCategory).Select(o => o.Name).SingleOrDefault();
            var clothesRes = new ClothesRes
            {
                Id = clothe.Id,
                Name = clothe.Name,
                Idseller = clothe.Idseller,
                Des = clothe.Des,
                IdCategory = clothe.IdCategory,
                imgsUrl = listImgUrls,
                quantily = (int)total,
                maxPrice = maxPrices.ToString(),
                CategoryName = categoryName
            };
            resGetClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetClothes._ClothesRes = clothesRes;
            return Ok(resGetClothes);
        }

        [Route("GetClothesWhereCategory")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetClothesWhereCategory(int idCategoryReq)
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            List<ClothesRes> lisRes = new List<ClothesRes>();
            List<Clothe> clotheList = await _context.Clothes.Where(x => x.IdCategory == idCategoryReq).ToListAsync();

            if (clotheList == null)
            {
                resGetListClothes._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(resGetListClothes);
            }

            foreach (Clothe clothe in clotheList)
            {

                List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == clothe.Id).Select(u => u.ImgUrl1).ToListAsync();
                var total = _context.ClothesProperties.Where(o => o.Idclothes == clothe.Id).Select(o => o.Quantily).Sum();
                string categoryName = _context.Categories.Where(o => o.Id == clothe.IdCategory).Select(o => o.Name).SingleOrDefault();
                var clothesRes = new ClothesRes
                {
                    Id = clothe.Id,
                    Name = clothe.Name,
                    Idseller = clothe.Idseller,
                    Des = clothe.Des,
                    IdCategory = clothe.IdCategory,
                    imgsUrl = listImgUrls,
                    quantily = (int)total,
                    CategoryName = categoryName
                };

                lisRes.Add(clothesRes);
            }

            resGetListClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListClothes._ClothesRes = lisRes;
            return Ok(resGetListClothes);
        }

        [Route("GetClothesFromSellerAndCategory")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetClothesFromSellerAndCategory(int idSellerReq, int idCategoryReq)
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            List<ClothesRes> lisRes = new List<ClothesRes>();
            List<Clothe> clotheList = await _context.Clothes.Where(x => x.Idseller == idSellerReq && x.IdCategory == idCategoryReq).ToListAsync();

            if (clotheList == null)
            {
                resGetListClothes._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(resGetListClothes);
            }
            foreach (Clothe clothe in clotheList)
            {
                List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == clothe.Id).Select(u => u.ImgUrl1).ToListAsync();
                var total = _context.ClothesProperties.Where(o => o.Idclothes == clothe.Id).Select(o => o.Quantily).Sum();
                string categoryName = _context.Categories.Where(o => o.Id == clothe.IdCategory).Select(o => o.Name).SingleOrDefault();
                var clothesRes = new ClothesRes
                {
                    Id = clothe.Id,
                    Name = clothe.Name,
                    Idseller = clothe.Idseller,
                    Des = clothe.Des,
                    IdCategory = clothe.IdCategory,
                    imgsUrl = listImgUrls,
                    quantily = (int)total,
                    CategoryName = categoryName
                };
                lisRes.Add(clothesRes);
            }

            resGetListClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListClothes._ClothesRes = lisRes;
            return Ok(resGetListClothes);
        }

        [Route("GetClothesFrom")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetClothesFrom(int idSellerReq)
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            List<ClothesRes> lisRes = new List<ClothesRes>();
            List<Clothe> clotheList = await _context.Clothes.Where(x => x.Idseller == idSellerReq).ToListAsync();

            if (clotheList == null)
            {
                resGetListClothes._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(resGetListClothes);
            }
            foreach (Clothe clothe in clotheList)
            {
                List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == clothe.Id).Select(u => u.ImgUrl1).ToListAsync();
                var total = _context.ClothesProperties.Where(o => o.Idclothes == clothe.Id).Select(o => o.Quantily).Sum();
                string categoryName = _context.Categories.Where(o => o.Id == clothe.IdCategory).Select(o => o.Name).SingleOrDefault();
                var clothesRes = new ClothesRes
                {
                    Id = clothe.Id,
                    Name = clothe.Name,
                    Idseller = clothe.Idseller,
                    Des = clothe.Des,
                    IdCategory = clothe.IdCategory,
                    imgsUrl = listImgUrls,
                    quantily = (int)total,
                    CategoryName = categoryName
                };
                lisRes.Add(clothesRes);
            }

            resGetListClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListClothes._ClothesRes = lisRes;
            return Ok(resGetListClothes);
        }

        [Route("UpdateClothes")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PutClothe(ClothesReq clothesReq)
        {
            Clothe clothe = new Clothe
            {
                Id = clothesReq.Id,
                IdCategory = clothesReq.IdCategory,
                Idseller = clothesReq.Idseller,
                Des = clothesReq.Des,
                Name = clothesReq.Name
            };
            _context.Entry(clothe).State = EntityState.Modified;

            var query = (from img in _context.ImgUrls
                         where img.Idclothes == clothe.Id
                         select img).ToListAsync();
            foreach (var img in await query)
            {
                _context.ImgUrls.Remove(img);
                _context.SaveChanges();
            }
            foreach (String url in clothesReq.imgUrls)
            {
                _context.ImgUrls.Add(new ImgUrl { Idclothes = clothesReq.Id, ImgUrl1 = url });
            }
            await _context.SaveChangesAsync();
            var query1 = (from pro in _context.ClothesProperties
                          where pro.Idclothes == clothe.Id
                          select pro).ToListAsync();
            foreach (var pro in await query1)
            {
                _context.ClothesProperties.Remove(pro);
                _context.SaveChanges();
            }
            foreach (ClothesPropertiesReq pro in clothesReq.ClothesProperties)
            {
                _context.ClothesProperties.Add(new ClothesProperty { Idclothes = clothesReq.Id, Size = pro.Size, Quantily = pro.Quantily, Price = Convert.ToDouble(pro.Price) });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClotheExists(clothesReq.Id))
                {
                    return Ok(new Respon { respone_code = 404, Status = "not found" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Respon { respone_code = 200, Status = "Success" });
        }

        [Route("CreateClothes")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PostClothe(ClothesReq clothesReq)
        {


            Clothe clothe = new()
            {
                Idseller = clothesReq.Idseller,
                Name = clothesReq.Name,
                IdCategory = clothesReq.IdCategory,
                Des = clothesReq.Des,

            };
            _context.Clothes.Add(clothe);
            try
            {
                await _context.SaveChangesAsync();
                foreach (String url in clothesReq.imgUrls)
                {
                    _context.ImgUrls.Add(new ImgUrl { Idclothes = clothe.Id, ImgUrl1 = url });
                }
                await _context.SaveChangesAsync();
                foreach (ClothesPropertiesReq pro in clothesReq.ClothesProperties)
                {
                    _context.ClothesProperties.Add(new ClothesProperty { Idclothes = clothe.Id, Size = pro.Size, Quantily = pro.Quantily, Price = Convert.ToDouble(pro.Price) });
                }
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {

                return Ok(new Respon { respone_code = 400, Status = "create failed" });

            }

            return Ok(new Respon { respone_code = 200, Status = "Success" });
        }

        [Route("DeleteClothes")]
        [HttpPost]
        public async Task<IActionResult> DeleteClothe(int id)
        {
            Respon res = new();
            try
            {
                var clothe = await _context.Clothes.FindAsync(id);
                if (clothe == null)
                {
                    res.respone_code = 404;
                    res.Status = "Not Found";
                    return Ok(res);
                }

                _context.Clothes.Remove(clothe);
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

        [Route("GetClothesProperties")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListProperties>>> GetClothePro(int idClothes)
        {
            ResGetListProperties res = new ResGetListProperties();
            if (!ClotheExists(idClothes))
            {
                res._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(res);
            }
            List<ClothesPropertiesRes> listRes = new List<ClothesPropertiesRes>();
            List<ClothesProperty> properties = await _context.ClothesProperties.Where(x => x.Idclothes == idClothes).ToListAsync();
            if (properties == null)
            {
                res._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(res);
            }
            foreach (var property in properties)
            {
                var propertiRes = new ClothesPropertiesRes
                {

                    Quantily = property.Quantily,
                    Size = property.Size,
                    Price = property.Price.ToString(),
                };
                listRes.Add(propertiRes);
            }

            res._Respon = new Respon { respone_code = 200, Status = "Success" };
            res._ClothesPropertiesRes = listRes;
            return Ok(res);
        }

        [Route("GetClothesQuantily")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResClothesQuantily>>> GetClothesQuantily(int idClothes)
        {
            ResClothesQuantily res = new ResClothesQuantily();
            if (!ClotheExists(idClothes))
            {
                res._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(res);
            }
            var total = _context.ClothesProperties.Where(o => o.Idclothes == idClothes).Select(o => o.Quantily).Sum();
            res._Respon = new Respon { respone_code = 200, Status = "Success" };
            res.Quantily = (int)total;
            return Ok(res);
        }

        [Route("GetStatistical")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetStatistical>>> GetStatistical()
        {
            ResGetStatistical resGetStatistical = new ResGetStatistical();
            List<ClothesStatisticalRes> res = new List<ClothesStatisticalRes>();
            try
            {
                string query = "EXEC STATISTIC";
                List<Statistical> result = _context.GetStatisticals.FromSqlRaw(query).AsEnumerable().ToList();
                foreach (Statistical statistical in result)
                {
                    var clothe = await _context.Clothes.FindAsync(statistical.IDClothes);
                    List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == statistical.IDClothes).Select(u => u.ImgUrl1).ToListAsync();
                    var total = _context.ClothesProperties.Where(o => o.Idclothes == statistical.IDClothes).Select(o => o.Quantily).Sum();
                    var maxPrices = _context.ClothesProperties.Where(o => o.Idclothes == statistical.IDClothes).Max(o => o.Price);
                    string categoryName = _context.Categories.Where(o => o.Id == clothe.IdCategory).Select(o => o.Name).SingleOrDefault();
                    var clothesRes = new ClothesStatisticalRes()
                    {
                        Id = clothe.Id,
                        Name = clothe.Name,
                        Idseller = clothe.Idseller,
                        Des = clothe.Des,
                        IdCategory = clothe.IdCategory,
                        imgsUrl = listImgUrls,
                        quantily = (int)total,
                        maxPrice = maxPrices.ToString(),
                        CategoryName = categoryName,
                        BuyTime = statistical.Quantity
                    };
                    res.Add(clothesRes);
                }
                resGetStatistical._Respon = new Respon { respone_code = 200, Status = "Success" };
                resGetStatistical.ClothesStatisticalRes = res;
                return Ok(resGetStatistical);
            }
            catch
            {
                resGetStatistical._Respon = new Respon { respone_code = 400, Status = "bad request" };
                return Ok(resGetStatistical);
            }


        }

        private bool ClotheExists(int id)
        {
            return _context.Clothes.Any(e => e.Id == id);
        }
    }
}
