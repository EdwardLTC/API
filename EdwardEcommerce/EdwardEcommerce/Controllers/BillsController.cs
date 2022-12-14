
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respon;
using EcommerceAPI.Models.Models_Respone;
using EcommerceAPI.Models.Models_Responsive;
using EdwardEcommerce.Models;
using EdwardEcommerce.Models.Models_Responsive;
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
                return Ok(new Respon { respone_code = 404, Status = "not found" });
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
                            status = sl.Status
                        };
            List<ResBill> resList = new List<ResBill>();
            foreach (var seller in query)
            {
                string userName = _context.People.Where(o => o.Id == seller.idSeller).Select(o => o.Name).FirstOrDefault();
                string userAddress = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Address).FirstOrDefault();
                ResBill nes = new ResBill
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status,
                    SellerName = userName,
                    UserAddress = userAddress,
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
            List<ResBill> resList = new List<ResBill>();
            foreach (var seller in query)
            {
                string sellerName = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Name).FirstOrDefault();
                string userAddress = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Address).FirstOrDefault();
                ResBill nes = new ResBill
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status,
                    SellerName = sellerName,
                    UserAddress = userAddress,
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
                return Ok(new Respon { respone_code = 404, Status = "not found" });
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
        public async Task<ActionResult<IEnumerable<BillRes>>> CreateBill(BillReq billReq)
        {

            BillRes res = new BillRes();
            foreach (var billDetailReq in billReq.ListBillDetailReq)
            {
                if (!ClotheExists(billDetailReq.Idclothes))
                {
                    res._Respon = new Respon { respone_code = 404, Status = $"not Found {billDetailReq.Idclothes}" };
                    return Ok(res);
                }
                int k = 0;
                var k1 = _context.ClothesProperties
                     .Where(o => o.Size == billDetailReq.Size && o.Idclothes == billDetailReq.Idclothes)
                     .Select(o => o.Quantily).FirstOrDefault();
                if (k1 != null)
                {
                    k = (int)k1;
                }
                if (billDetailReq.Quantily > k)
                {
                    res._Respon = new Respon { respone_code = 400, Status = $"Bad Req {billDetailReq.Idclothes} size {billDetailReq.Size}" };
                    return Ok(res);
                }
            }
            var percentDiscount = _context.Vouchers.Where(o => o.Id == billReq.Idvoucher).Select(o => o.Ratio).SingleOrDefault();

            Bill bill = new()
            {
                Iduser = billReq.Iduser,
                Idseller = billReq.Idseller,
                Idvoucher = billReq.Idvoucher,
                DateCreate = DateTime.UtcNow.Date,
                Status = billReq.Status,
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            foreach (var billDetailReq in billReq.ListBillDetailReq)
            {
                ClothesProperty clothesProperty = await _context.ClothesProperties.Where(o => o.Idclothes == billDetailReq.Idclothes && o.Size == billDetailReq.Size).SingleOrDefaultAsync();
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

            double s = double.Parse(totolPrice.ToString()) - ((double.Parse(totolPrice.ToString()) * test) / 100);
            res._Respon = new Respon { respone_code = 200, Status = "success" };
            res._DateCreate = DateTime.Now.ToString();
            res._TotolPrice = String.Format("{0:0.00}", s);
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


            var listIdClothesPropeties = _context.BillDetails.Where(o => o.Idbill == billId).Select(o => o.IdclotheProperties );
            List<ClothesRes> listRes = new List<ClothesRes>();
            foreach (var item in listIdClothesPropeties)
            {
             
                var result = from clthePro in _context.ClothesProperties
                             where clthePro.Id == item
                             select new
                             {
                                 idc = clthePro.Idclothes,
                                 size = clthePro.Size
                             };

                foreach (var idClothes in result)
                {
                    var clothe = await _context.Clothes.FindAsync(idClothes.idc);
                    var listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == (int)idClothes.idc).Select(u => u.ImgUrl1).ToListAsync();
                    var idPro = _context.ClothesProperties.Where(o => o.Idclothes == (int)idClothes.idc && o.Size == idClothes.size ).Select(o => o.Id).FirstOrDefault();
                    var total = _context.BillDetails.Where(o => o.IdclotheProperties == idPro && o.Idbill == billId).Select(o => o.Quantity).Sum();
                    var size = _context.ClothesProperties.Where(o => o.Id == item && o.Idclothes == (int)idClothes.idc).Select(o => o.Size).FirstOrDefault();
                    var price = _context.ClothesProperties.Where(o => o.Id == item && o.Idclothes == (int)idClothes.idc).Max(o => o.Price);

                    ClothesRes clothesRes = new ClothesRes
                    {
                        Id = (int)idClothes.idc,
                        Name = clothe.Name,
                        Idseller = clothe.Idseller,
                        Des = clothe.Des,
                        IdCategory = clothe.IdCategory,
                        imgsUrl = listImgUrls,
                        quantily = (int)total,
                        maxPrice = ((int)total * double.Parse(price.ToString())).ToString(),
                        size = size.ToString()
                    };
                    listRes.Add(clothesRes);
                }
            }

            resGetListClothes._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListClothes._ClothesRes = listRes;
            return Ok(resGetListClothes);
        }

        [Route("GetClothesFromBillCustomer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListClothes>>> GetClothesFromBillCustomer(int idCus, int billId )
        {
            ResGetListClothes resGetListClothes = new ResGetListClothes();
            List<ClothesRes> listRes = new List<ClothesRes>();
            if (!PersonExists(idCus) || !BillExists(billId))
            {
                resGetListClothes._Respon = new Respon { Status = "not found", respone_code = 404 };
                return Ok(resGetListClothes);
            }
            var listBillId = _context.Bills.Where(o => o.Id == billId && o.Iduser == idCus).Select(o => o.Id);
            foreach(var idbill in listBillId)
            {
                var listIdClothesPropeties = _context.BillDetails.Where(o => o.Idbill == idbill).Select(o => o.IdclotheProperties);
                foreach (var item in listIdClothesPropeties)
                {
                    var result = from clthePro in _context.ClothesProperties
                                 where clthePro.Id == item
                                 select new
                                 {
                                     idc = clthePro.Idclothes,
                                     size = clthePro.Size
                                 };
                    foreach (var idClothes in result)
                    {
                        var clothe = await _context.Clothes.FindAsync(idClothes.idc);
                        var listImgUrls = await _context.ImgUrls.Where(x => x.Idclothes == idClothes.idc).Select(u => u.ImgUrl1).ToListAsync();
                        var idPro = _context.ClothesProperties.Where(o => o.Idclothes == idClothes.idc && o.Size == idClothes.size).Select(o => o.Id).FirstOrDefault();
                        var total = _context.BillDetails.Where(o => o.IdclotheProperties == idPro && o.Idbill == idbill).Select(o => o.Quantity).Sum();
                        var size = _context.ClothesProperties.Where(o => o.Id == item && o.Idclothes == idClothes.idc).Select(o => o.Size).FirstOrDefault();
                        var price = _context.ClothesProperties.Where(o => o.Id == item && o.Idclothes == idClothes.idc).Max(o => o.Price);

                        ClothesRes clothesRes = new ClothesRes
                        {
                            Id = (int)idClothes.idc,
                            Name = clothe.Name,
                            Idseller = clothe.Idseller,
                            Des = clothe.Des,
                            IdCategory = clothe.IdCategory,
                            imgsUrl = listImgUrls,
                            quantily = (int)total,
                            maxPrice = ((int)total * double.Parse(price.ToString())).ToString(),
                            size = size.ToString()
                        };
                        listRes.Add(clothesRes);
                    }
                }

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
            List<ResBill> resList = new List<ResBill>();
            foreach (var seller in query)
            {
                var sellerName = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Name).FirstOrDefault();
                var userAddress = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Address).FirstOrDefault();
                ResBill nes = new ResBill
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    Idvoucher = seller.idVoucher,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status,
                    SellerName = sellerName,
                    UserAddress = userAddress,
                };
                resList.Add(nes);
            }
            resGetListBill._Respon = new Respon { Status = "Success", respone_code = 200 };
            resGetListBill._BillRes = resList;
            return Ok(resGetListBill);
        }

        [Route("GetBillWhereCompletedSeller")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListBill>>> GetBillWhereCompletedSeller(int idSeller)
        {
            ResGetListBill resGetListBill = new ResGetListBill();
            var query = from bill in _context.Bills
                        where bill.Status == "Bill Completed" && bill.Idseller == idSeller
                        select new
                        {
                            idBill = bill.Id,
                            idSeller = bill.Idseller,
                            idVoucher = bill.Idvoucher,
                            idUser = bill.Iduser,
                            dateCreate = bill.DateCreate,
                            status = bill.Status
                        };
            List<ResBill> resList = new List<ResBill>();
            foreach (var seller in query)
            {
                var sellerName = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Name).FirstOrDefault();
                var userAddress = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Address).FirstOrDefault();
                ResBill nes = new ResBill
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    Idvoucher = seller.idVoucher,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status,
                    SellerName = sellerName,
                    UserAddress = userAddress,
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
            List<ResBill> resList = new List<ResBill>();
            foreach (var seller in query)
            {
                var sellerName = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Name).FirstOrDefault();
                var userAddress = _context.People.Where(o => o.Id == seller.idUser).Select(o => o.Address).FirstOrDefault();
                ResBill nes = new ResBill
                {
                    Id = seller.idBill,
                    Iduser = seller.idUser,
                    Idseller = seller.idSeller,
                    Idvoucher = seller.idVoucher,
                    DateCreate = seller.dateCreate.ToString(),
                    Status = seller.status,
                    SellerName = sellerName,
                    UserAddress = userAddress,
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
            Bill billReq = await _context.Bills.Where(o => o.Id == idBill).FirstOrDefaultAsync();
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
            res._Payment = String.Format("{0:0.00}", s);
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
