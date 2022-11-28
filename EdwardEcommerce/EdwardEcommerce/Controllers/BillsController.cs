
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using EcommerceAPI.Models.Models_Responsive;
using EdwardEcommerce.Models;
using EdwardEcommerce.Models.Models_Responsive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                return Ok (new Respon { respone_code = 404, Status = "not found"});
            }
            Bill bill = await _context.Bills.Where(o => o.Id == idBill).SingleOrDefaultAsync();
            bill.Status = status;
            _context.SaveChanges();
            return Ok(new Respon { respone_code = 200, Status = "Success" });
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
            
            var query = from sl in _context.Bills
                        where sl.Iduser == idUser
                        select new
                        {
                            idBill = sl.Id,
                            idSeller = sl.Idseller,
                            idUser = sl.Iduser,
                            dateCreate = sl.DateCreate,
                            status =sl.Status
                        };
            List<BillRes> resList = new List<BillRes>();
            foreach (var seller in query)
            {
                string sellerName = from sl in _context.People
                                    where sl.Id == seller.
                BillRes nes = new BillRes
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status
                };
                resList.Add(nes);
            }
            resGetListBill._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListBill._BillRes = resList;
            return Ok(resGetListBill);
        }

        [Route("GetBillOfSeller")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListBill>>> GetALlBillOfSeller(int idSeller)
        {
            ResGetListBill resGetListBill = new ResGetListBill();
            if (!PersonExists(idSeller))
            {
                resGetListBill._Respon = new Respon { Status = "not found", respone_code = 404 };
                return Ok(resGetListBill);
            }

            var query = from seller in _context.Bills
                        where seller.Idseller == idSeller
                        select new
                        {
                            idBill = seller.Id,
                            idSeller = seller.Idseller,
                            idUser = seller.Iduser,
                            dateCreate = seller.DateCreate,
                            status = seller.Status
                        };
            List<BillRes> resList = new List<BillRes>();
            foreach (var seller in query)
            {
                BillRes nes = new BillRes
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    DateCreate =  seller.dateCreate.ToString(),
                    Status = seller.status
                };
                resList.Add(nes);
            }
            resGetListBill._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListBill._BillRes = resList;
            return Ok(resGetListBill);
        }

        [Route("MarkBillComplete")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Respon>>> MarkBillComplete(int idBill)
        {
            if (!BillExists(idBill))
            {
                return Ok(new Respon { respone_code=404,Status ="not found"});
            }
            Bill bill = await _context.Bills.Where(o => o.Id == idBill).SingleOrDefaultAsync();
            //if (bill.DateReceived != null)
            //{
            //    return Ok(new Respon { respone_code = 400, Status = "not found" });
            //}
            bill.DateReceived = DateTime.UtcNow.Date;
            bill.Status = "Bill Completed";
            _context.SaveChanges();
            return Ok(new Respon { respone_code = 200, Status = "Success" });
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
                Idseller = billReq.Idseller,
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

            double s = double.Parse(totolPrice.ToString()) - ((double.Parse(totolPrice.ToString()) * test) / 100 );
            res._Respon = new Respon { respone_code = 200, Status = "success" };
            res._DateCreate = DateTime.Now.ToString();
            return Ok(res);
        }

        [Route("GetClothesFromFromBill")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetClothesFromFromBill(int billId)
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            if (!BillExists(billId))
            {
                resGetListClothes._Respon = new Respon { Status = "not found", respone_code = 404 };
                return Ok(resGetListClothes);
            }


            var listIdClothesPropeties =_context.BillDetails.Where(o => o.Idbill == billId).Select(o=>o.IdclotheProperties);
            
            List<int> listIdClothes = new List<int>();
            foreach (var item in listIdClothesPropeties)
            {
                var result = _context.ClothesProperties.Where(o => o.Id == item).Select(o => o.Idclothes);
                foreach (var id in result)
                {
                    listIdClothes.Add(id.Value);
                }
            }

            List<ClothesRes> listRes = new List<ClothesRes>();
            foreach (var item in listIdClothes)
            {
                var clothe = await _context.Clothes.FindAsync(item);
                List<string> listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == item).Select(u => u.ImgUrl1).ToListAsync();
                var total = _context.ClothesProperties.Where(o => o.Idclothes ==item).Select(o => o.Quantily).Sum();
                var clothesRes = new ClothesRes
                {
                    Id = clothe.Id,
                    Name = clothe.Name,
                    Idseller = clothe.Idseller,
                    Des = clothe.Des,
                    IdCategory = clothe.IdCategory,
                    imgsUrl = listImgUrls,
                    quantily = (int)total
                };
                listRes.Add(clothesRes);

            }
           
            resGetListClothes._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListClothes._ClothesRes = listRes;
            return Ok(resGetListClothes);
        }

        [Route("GetBillWhereCompleted")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListBill>>> GetBillWhereCompleted()
        {
            ResGetListBill resGetListBill = new ResGetListBill();
            var query = from bill in _context.Bills
                        where bill.Status == "Bill Completed"
                        select new
                        {
                            idBill = bill.Id,
                            idSeller = bill.Idseller,
                            idVoucher = bill.Idvoucher,
                            idUser = bill.Iduser,
                            dateCreate = bill.DateCreate,
                            status = bill.Status
                        };
            List<BillRes> resList = new List<BillRes>();
            foreach (var seller in query)
            {
                BillRes nes = new BillRes
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    Idvoucher = seller.idVoucher,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status
                };
                resList.Add(nes);
            }
            resGetListBill._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListBill._BillRes = resList;
            return Ok(resGetListBill);
        }

        [Route("GetBillWhereNotCompleted")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListBill>>> GetBillWhereNotCompleted()
        {
            ResGetListBill resGetListBill = new ResGetListBill();
            var query = from bill in _context.Bills
                        where bill.Status != "Bill Completed"
                        select new
                        {
                            idBill = bill.Id,
                            idSeller = bill.Idseller,
                            idVoucher = bill.Idvoucher,
                            idUser = bill.Iduser,
                            dateCreate = bill.DateCreate,
                            status = bill.Status
                        };
            List<BillRes> resList = new List<BillRes>();
            foreach (var seller in query)
            {
                BillRes nes = new BillRes
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    Idvoucher = seller.idVoucher,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status
                };
                resList.Add(nes);
            }
            resGetListBill._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListBill._BillRes = resList;
            return Ok(resGetListBill);
        }

        [Route("GetBillPayment")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetBillPayment>>> GetBillPayment(int idBill)
        {
           
            ResGetBillPayment res = new ResGetBillPayment();
            if (!BillExists(idBill))
            {
                res._Respon = new Respon { respone_code = 404, Status = "Not Found" };
            }
            Bill billReq = await _context.Bills.Where(o=>o.Id == idBill).FirstOrDefaultAsync();
            var percentDiscount = _context.Vouchers.Where(o => o.Id == billReq.Idvoucher).Select(o => o.Ratio).SingleOrDefault();
            var totolPrice = _context.BillDetails.Where(o => o.Idbill == idBill).Sum(i => i.Price);
            double test = 0;
            try
            {
                test = double.Parse(percentDiscount.ToString());
            }
            catch
            {
                test = 0;
            }
            
            double s = double.Parse(totolPrice.ToString()) - ((double.Parse(totolPrice.ToString()) * test / 100));
            Console.WriteLine(double.Parse(totolPrice.ToString()));
            Console.WriteLine(((double.Parse(totolPrice.ToString()) * test)));
            Console.Write(totolPrice);
            res._Respon = new Respon { Status = "success", respone_code = 200 };
            res._Payment = s.ToString();
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
