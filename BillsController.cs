using EcommerceAPI.Models;
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using EcommerceAPI.Models.Models_Responsive;
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

        [Route("UpdateStatusBill")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Respon>>> UpdateStatusBill(string status, int idBill)
        {
            if (!BillExists(idBill))
            {
                return NotFound();
            }
            Bill bill = await _context.Bills.Where(o => o.Id == idBill).SingleOrDefaultAsync();
            bill.Status = status;
            _context.SaveChanges();
            return Ok();
        }

        [Route("GetBillOfUser")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListBill>>> GetALlBillOfUser(int idUser)
        {
            ResGetListBill resGetListBill = new ResGetListBill();
            if (!PersonExists(idUser))
            {
                resGetListBill._Respon = new Respon { Status = "not found", respone_code = 404 };
                return Ok(resGetListBill);
            }

            var query = from seller in _context.Bills
                        where seller.Iduser == idUser
                        select new
                        {
                            idBill = seller.Id,
                            idUser = seller.Iduser,
                            dateCreate = seller.DateCreate,
                            status =seller.Status
                        };
            List<BillRes> resList = new List<BillRes>();
            foreach (var seller in query)
            {
                BillRes nes = new BillRes
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    DateCreate = seller.dateCreate,
                    Status = seller.status
                };
                resList.Add(nes);
            }
            resGetListBill._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListBill._BillList = resList;
            return Ok(resGetListBill);
        }

        [Route("MarkBillComplete")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Respon>>> MarkBillComplete(int idBill)
        {
            if (!BillExists(idBill))
            {
                return NotFound();
            }
            Bill bill = await _context.Bills.Where(o => o.Id == idBill).SingleOrDefaultAsync();
            if (bill.DateReceived != null)
            {
                return BadRequest();
            }
            bill.DateReceived = DateTime.UtcNow.Date;
            bill.Status = "Bill Completed";
            _context.SaveChanges();
            return Ok();
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
        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }

    }
}
