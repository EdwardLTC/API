using EcommerceAPI.Models;
using EcommerceAPI.Models.Models_Request;
using EcommerceAPI.Models.Models_Respone;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EcommerceAPI.Controller
{
    [Route("api/")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly EdwardEcomerceContext _context;
        public PersonController(EdwardEcomerceContext context)
        {
            _context = context;
        }

        [Route("GetAllPerson")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetListPerson>>> GetPeople()
        {
            ResGetListPerson _resGetListPerson = new();
            List<Person> list = await _context.People.ToListAsync();
            List<PersonRes> listRes = new();
            foreach (Person person in list)
            {
                PersonRes res = new()
                {
                    Id = person.Id,
                    Name = person.Name,
                    Address = person.Address,
                    Role = person.Role,
                    Mail = person.Mail,
                    ImgUrl = person.ImgUrl,
                    PhoneNum = person.PhoneNum
                };
                listRes.Add(res);

            }
            _resGetListPerson._Respon = new Respon { respone_code = 200, Status = "Success" };
            _resGetListPerson._PersonRes = listRes;

            return Ok(_resGetListPerson);

        }

        [HttpGet]
        [Route("GetAllPersonLoadMore")]
        public async Task<ActionResult<IEnumerable<ResGetListPerson>>>  GetDataLoadMore(int currentIndex, int elementsNumber)
        {
            ResGetListPerson _resGetListPerson = new();
            var list = _context.People.Skip(currentIndex).Take(elementsNumber).ToListAsync();
            List<PersonRes> listRes = new();
            foreach (Person person in await list)
            {
                PersonRes res = new()
                {
                    Id = person.Id,
                    Name = person.Name,
                    Address = person.Address,
                    Role = person.Role,
                    Mail = person.Mail,
                    ImgUrl = person.ImgUrl,
                    PhoneNum = person.PhoneNum
                };
                listRes.Add(res);

            }

            _resGetListPerson._Respon = new Respon { respone_code = 200, Status = "Success" };
            _resGetListPerson._PersonRes = listRes;

            return Ok(listRes);
        }

        [Route("GetPersonWhere")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetPerson>>> GetPeople(int Id)
        {
            ResGetPerson _resGetPerson = new(); 
            var person = await _context.People.FindAsync(Id);
            if (person == null)
            {
                _resGetPerson._Respon = new Respon { respone_code = 404, Status = "Does not exist" };
                return Ok(_resGetPerson);
            }
            PersonRes res = new()
            {
                Id = person.Id,
                Name = person.Name,
                Address = person.Address,
                Role = person.Role,
                Mail = person.Mail,
                ImgUrl = person.ImgUrl,
                PhoneNum = person.PhoneNum
            };

            _resGetPerson._Respon = new Respon { respone_code = 200, Status = "Success" };
            _resGetPerson._PersonRes = res;
            return Ok(res);

        }

        [Route("CreatePerson")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> CreatePerson(PersonReq personReq)
        {
            try
            {
                var person = new Person
                {
                    Name = personReq.Name,
                    Address = personReq.Address,
                    Role = personReq.Role,
                    Mail = personReq.Mail,
                    ImgUrl = personReq.ImgUrl,
                    PhoneNum = personReq.PhoneNum,
                    Psw = personReq.Psw
                };
                _context.People.Add(person);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return Ok(new Respon { respone_code = 400, Status = "bad Request" });
            }
          
            return Ok(new Respon { respone_code = 200, Status = "Success" });
        }

        [Route("UpdatePerson")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> PutPerson(PersonReq personReq)
        {
            var person = new Person();
            int id = personReq.Id;
            person.Name = personReq.Name;
            person.Address = personReq.Address;
            person.Role = personReq.Role;
            person.Mail = personReq.Mail;
            person.ImgUrl = personReq.ImgUrl;
            person.PhoneNum = personReq.PhoneNum;
            try
            {
                _context.Entry(person).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
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

        [Route("DeletePerson")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Respon>>> DeletePerson(int id)
        {
            Respon res = new();
            try
            {
                var person = await _context.People.FindAsync(id);
                if (person == null)
                {
                    res.respone_code = 404;
                    res.Status = "Not Found";
                    return Ok(res);
                }

                _context.People.Remove(person);
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

        [Route("Login")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResGetPerson>>> Login(String _email,String _psw)
        {
            ResGetPerson _resGetPerson = new();

            var person = await _context.People.Where(x => x.Mail == _email && x.Psw == _psw)
                .Select(x => new PersonRes
                {
                    Id = x.Id,
                    Name = x.Name,
                    Mail = x.Mail,
                    PhoneNum = x.PhoneNum,
                    Role = x.Role,
                    ImgUrl = x.ImgUrl,
                    Address = x.Address
                }).FirstOrDefaultAsync();

            if(person == null)
            {
                _resGetPerson._Respon = new Respon { respone_code= 404, Status= "Does not exist" };
            }
            else
            {
                _resGetPerson._Respon = new Respon { respone_code = 200, Status = "Successs" };
                _resGetPerson._PersonRes = person;
            }

            return Ok(_resGetPerson);
        }

        private bool PersonExists(int id)   
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
