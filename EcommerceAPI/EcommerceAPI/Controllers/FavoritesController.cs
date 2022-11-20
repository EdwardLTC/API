using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using EcommerceAPI.Models.Models_Request;

namespace EcommerceAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly EdwardEcomerceContext _context;

        public FavoritesController(EdwardEcomerceContext context)
        {
            _context = context;
        }

        [Route("GetAllFavoritesOf")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetFavorites(int userID)
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            if (!PersonExists(userID))
            {
                resGetListClothes._Respon = new Respon { respone_code = 404, Status = "Not found" };
                return Ok(resGetListClothes);
            }
            
            List<ClothesRes> res = new List<ClothesRes>();
            var query = from favorite in _context.Favorites
                        where favorite.Iduser == userID
                        select new
                        {
                            idClosthes = favorite.Idclothes
                        };

            foreach (var favorite in query)
            {
                var clothe = await _context.Clothes.FindAsync(favorite.idClosthes);
                var imgs = from url in _context.ImgUrls
                           where url.Idclothes == favorite.idClosthes
                           select new
                           {
                               imgUrl = url.ImgUrl1
                           };
                List<string> urls = await _context.ImgUrls.Where(x => x.Idclothes == favorite.idClosthes).Select(u => u.ImgUrl1).ToListAsync();

                var clothesRes = new ClothesRes
                {
                    Id = clothe.Id,
                    Name = clothe.Name,
                    Idseller = clothe.Idseller,
                    Des = clothe.Des,
                    IdCategory = clothe.IdCategory,
                    imgsUrl = urls
                };
                res.Add(clothesRes);
            }

            resGetListClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListClothes._ClothesRes = res;
            return Ok(resGetListClothes);
        }

        [Route("AddToFavorite")]
        [HttpPost]
        public async Task<ActionResult<Favorite>> PostFavorite(FavoriteReq favoritereq)
        {
            Respon res = new Respon();
            Favorite favorite = new Favorite { Idclothes = favoritereq.Idclothes, Iduser = favoritereq.Iduser };
            if (!PersonExists(favoritereq.Iduser))
            {
                res.respone_code = 404;
                res.Status = "User not found";
                return Ok(res);
            }

            if (!ClotheExists(favoritereq.Idclothes))
            {
                res.respone_code = 404;
                res.Status = "Clothes not found";
                return Ok(res);
            }
          
            try
            {
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                res.respone_code = 200;
                res.Status = "Success";
                return Ok(res);
            }
            catch
            {
                res.respone_code = 400;
                res.Status = "Bad Reqest";
                return Ok(res);
            }
            res.respone_code = 400;
            res.Status = "Bad Reqest";
            return Ok(res);
        }

        [Route("RemoveFromFavorite")]
        [HttpPost()]
        public async Task<IActionResult> DeleteFavorite(FavoriteReq favoriteReq)
        {
            Respon res = new Respon();
            var favorite = await _context.Favorites.Where(x=> x.Iduser == favoriteReq.Iduser && x.Idclothes == favoriteReq.Idclothes).FirstOrDefaultAsync();
            if (favorite == null)
            {
                res.respone_code = 400;
                res.Status = "Bad Reqest";
                return Ok(res);
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            res.respone_code = 200;
            res.Status = "Success";
            return Ok(res);
        }

        private bool FavoriteExists(int id)
        {
            return _context.Favorites.Any(e => e.Id == id);
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }

        private bool ClotheExists(int id)
        {
            return _context.Clothes.Any(e => e.Id == id);
        }
    }
}
