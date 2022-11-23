using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respone;
using System.Drawing;

namespace EcommerceAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly EdwardEcomerceContext _context;

        public BillsController(EdwardEcomerceContext context)
        {
            _context = context;
        }

        [Route("CreateBill")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> CreateBill(BillReq billReq)
        {
            foreach (var billDetailReq in billReq.ListBillDetailReq)
            {
                if (!ClotheExists(billDetailReq.Idclothes))
                {
                    return Ok(new Respon { respone_code = 404, Status = "not Found" });
                }
                var quantity = _context.ClothesProperties
                .Where(o => o.Size == billDetailReq.Size && o.Idclothes == billDetailReq.Idclothes)
                .Select(o => o.Quantily).FirstOrDefault();
                if (billDetailReq.Quantily > quantity)
                {
                    return Ok(new Respon { respone_code = 400, Status = "Bad Req" });
                }
            }
            Bill bill = new Bill
            {
                Iduser = billReq.Iduser,
                Idvoucher = billReq.Idvoucher,
                DateCreate = billReq.DateCreate,
                DateReceived = billReq.DateReceived,
                Status = billReq.Status,
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            foreach (var billDetailReq in billReq.ListBillDetailReq)
            {
                ClothesProperty clothesProperty = _context.ClothesProperties.Where(o => o.Idclothes == billDetailReq.Idclothes).SingleOrDefault();
                int temp = (int)clothesProperty.Quantily;
                clothesProperty.Quantily = temp - billDetailReq.Quantily;
                _context.Entry(clothesProperty).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                BillDetail billDetail = new BillDetail
                {
                    Idbill = bill.Id,
                    IdclotheProperties = clothesProperty.Id,
                    Quantity = billDetailReq.Quantily,
                    Price = billDetailReq.Quantily * clothesProperty.Price
                };
                _context.BillDetails.Add(billDetail);
                await _context.SaveChangesAsync();
            }

            return Ok(new Respon { respone_code = 200, Status = "success" });


        }
        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.Id == id);
        }

        private bool ClotheExists(int id)
        {
            return _context.Clothes.Any(e => e.Id == id);
        }

    }
}
