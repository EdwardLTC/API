using EcommerceAPI.Models;
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<ResBill>>> CreateBill(BillReq billReq)
        {
            ResBill res = new ResBill();
            foreach (var billDetailReq in billReq.ListBillDetailReq)
            {
                if (!ClotheExists(billDetailReq.Idclothes))
                {
                    res._Respon = new Respon { respone_code = 404, Status = $"not Found {billDetailReq.Idclothes}" };
                    return Ok(res);
                }
                var quantity = _context.ClothesProperties
                .Where(o => o.Size == billDetailReq.Size && o.Idclothes == billDetailReq.Idclothes)
                .Select(o => o.Quantily).FirstOrDefault();
                if (billDetailReq.Quantily > quantity)
                {
                    res._Respon = new Respon { respone_code = 400, Status = "Bad Req" };
                    return Ok(res);
                }
            }
            var percentDiscount = _context.Vouchers.Where(o => o.Id == billReq.Idvoucher).Select(o => o.Ratio).SingleOrDefault();
            Bill bill = new()
            {
                Iduser = billReq.Iduser,
                Idvoucher = percentDiscount,
                DateCreate = DateTime.UtcNow.Date,
                Status = billReq.Status,
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            foreach (var billDetailReq in billReq.ListBillDetailReq)
            {
                ClothesProperty clothesProperty = await _context.ClothesProperties.Where(o => o.Idclothes == billDetailReq.Idclothes).SingleOrDefaultAsync();
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

            var totolPrice = _context.BillDetails.Where(o => o.Idbill == bill.Id).Sum(i => i.Price);
            double test = 0;
            try
            {
                test = double.Parse(percentDiscount.ToString());
            }
            catch
            {
                test = 0;
            }

            double s = double.Parse(totolPrice.ToString()) - ((double.Parse(totolPrice.ToString()) * test));
            res._Respon = new Respon { respone_code = 200, Status = "success" };
            res._DateCreate = DateTime.Now.ToString();
            return Ok(res);
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
