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
    public class VouchersController : ControllerBase
    {
        private readonly EdwardEcomerceContext _context;

        public VouchersController(EdwardEcomerceContext context)
        {
            _context = context;
        }

        [Route("GetAllVoucher")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListVoucher>>> GetVouchers()
        {
            ResGetListVoucher resGetListVoucher = new ResGetListVoucher();
            List<Voucher> vouchers = await _context.Vouchers.ToListAsync();
            List<VoucherRes> vouchersRes = new List<VoucherRes>();
            foreach (Voucher voucher in vouchers)
            {
                VoucherRes voucherRes = new VoucherRes
                {
                    Idseller = voucher.Idseller,
                    Id = voucher.Id,
                    Ratio = voucher.Ratio
                };
                vouchersRes.Add(voucherRes);
            }
            resGetListVoucher._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListVoucher._VoucherRes = vouchersRes;
            return Ok(resGetListVoucher);
        }

        [Route("GetAllVoucherOf")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListVoucher>>> GetAllVoucherOf(int idSeller)
        {

            ResGetListVoucher resGetListVoucher = new ResGetListVoucher();
            if (!PersonExists(idSeller))
            {
                resGetListVoucher._Respon = new Respon { respone_code = 404, Status = "Seller Not Exists" };
                return Ok(resGetListVoucher);
            }

            List<Voucher> vouchers = await _context.Vouchers.Where(x => x.Idseller == idSeller).ToListAsync();
            List<VoucherRes> vouchersRes = new List<VoucherRes>();
            foreach (Voucher voucher in vouchers)
            {
                VoucherRes voucherRes = new VoucherRes
                {
                    Idseller = voucher.Idseller,
                    Id = voucher.Id,
                    Ratio = voucher.Ratio
                };
                vouchersRes.Add(voucherRes);
            }
            resGetListVoucher._Respon = new Respon { respone_code = 200, Status = "Success" };
            resGetListVoucher._VoucherRes = vouchersRes;
            return Ok(resGetListVoucher);
        }

        [Route("GetVoucherWhere")]
        [HttpGet]
        public async Task<ActionResult<ResGetVoucher>> GetVoucher(int id)
        {
            ResGetVoucher resGetVoucher = new ResGetVoucher();
            var voucher = await _context.Vouchers.FindAsync(id);

            if (voucher == null)
            {
                resGetVoucher._Respon = new Respon { respone_code = 404, Status = "Not Found" };
                return Ok(resGetVoucher);
            }

            VoucherRes voucherRes = new VoucherRes
            {
                Id = voucher.Id,
                Ratio = voucher.Ratio,
                Idseller = voucher.Idseller
            };

            resGetVoucher._VoucherRes = voucherRes;
            resGetVoucher._Respon = new Respon { respone_code = 200, Status = "Success" };
            return Ok(resGetVoucher);
        }

        [Route("UpdateVoucher")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PutVoucher(VoucherReq voucherReq)
        {

            Voucher voucher = new Voucher
            {
                Id = voucherReq.Id,
                Ratio = voucherReq.Ratio,
                Idseller = voucherReq.Idseller
            };
            _context.Entry(voucher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherExists(voucherReq.Id))
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

        [Route("CreateVoucher")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PostVoucher(VoucherReq voucherReq)
        {
            Voucher voucher = new Voucher
            {
                Idseller = voucherReq.Idseller,
                Ratio = voucherReq.Ratio
            };
            _context.Vouchers.Add(voucher);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                return Ok(new Respon { respone_code = 400, Status = "create failed" });

            }

            return Ok(new Respon { respone_code = 200, Status = "Success" });
        }

        [Route("DeleteVoucher")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> DeleteVoucher(int id)
        {
            Respon res = new();
            try
            {
                var voucher = await _context.Vouchers.FindAsync(id);
                if (voucher == null)
                {
                    res.respone_code = 404;
                    res.Status = "Not Found";
                    return Ok(res);
                }
                _context.Vouchers.Remove(voucher);
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

        private bool VoucherExists(int id)
        {
            return _context.Vouchers.Any(e => e.Id == id);
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
