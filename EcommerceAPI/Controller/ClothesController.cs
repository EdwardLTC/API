﻿using System;
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
            foreach(Clothe clothes in list)
            {
                ClothesRes respon =  new ClothesRes
                {
                    Id = clothes.Id,
                    Name = clothes.Name,
                    Idseller = clothes.Idseller,
                    Des = clothes.Des,
                    IdCategory = clothes.IdCategory,
                };
                List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == clothes.Id).Select(u => u.ImgUrl1).ToListAsync();
                respon.imgsUrl = listImgUrls;
                res.Add(respon);
            }
            resGetListClothes._Respon = new Respon {respone_code= 200, Status= "Success" };
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
                return Ok(resGetClothes) ;
            }

            var clothesRes = new ClothesRes
            {
                Id = clothe.Id,
                Name = clothe.Name,
                Idseller = clothe.Idseller,
                Des = clothe.Des,
                IdCategory = clothe.IdCategory,
                imgsUrl = listImgUrls
            };
            resGetClothes._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetClothes._ClothesRes = clothesRes;
            return Ok(resGetClothes);
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
            foreach(String url in clothesReq.imgUrls)
            {
                _context.ImgUrls.Add(new ImgUrl {Idclothes = clothesReq.Id,ImgUrl1 = url});
            }
            await _context.SaveChangesAsync();  
            foreach (ClothesPropertiesReq pro in clothesReq.ClothesProperties)
            {
                _context.ClothesProperties.Add(new ClothesProperty { Idclothes = clothesReq.Id, Size = pro.Size, Quantily = pro.Quantily, Price = pro.Price });
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
        public async Task <ActionResult<IEnumerable<Respon>>> PostClothe(ClothesReq clothesReq)
        {
           

            Clothe clothe = new Clothe
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
                    _context.ClothesProperties.Add(new ClothesProperty { Idclothes = clothe.Id, Size = pro.Size, Quantily = pro.Quantily, Price = pro.Price });
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
        public async Task<ActionResult<IEnumerable<ResGetClothes>>> GetClothePro(int id)
        {
            ResGetProperties res = new ResGetProperties();
            var properties = await _context.ClothesProperties.Where(x => x.Idclothes == id).FirstOrDefaultAsync();
            if (properties == null)
            {
                res._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(res);
            }

            var propertiRes = new ClothesPropertiesRes
            {
                Quantily = properties.Quantily,
                Size = properties.Size,
                Price = properties.Price,
            };
            res._Respon = new Respon { respone_code = 200, Status = "Success" };
            res._ClothesPropertiesRes = propertiRes;
            return Ok(res);
        }


        private bool ClotheExists(int id)
        {
            return _context.Clothes.Any(e => e.Id == id);
        }
    }
}
