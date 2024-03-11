using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/accounts")]
    public class AdminController : Controller
    {
        private readonly DBContext _dbContext;

        public AdminController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<AccountDTO> accounts = _dbContext.Admins
                    .Include(ad => ad.Role)
                    .Select(ad => new AccountDTO()
                    {
                        Id = ad.Id,
                        Username = ad.Username,
                        Fullname = ad.FullName,
                        Email = ad.Email,
                        Role = ad.Role.Name
                    })
                    .ToList();
                return Ok(accounts);
            }
            catch (Exception e)
            {
                return BadRequest("Get accounts error");
            }
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateAccount(AdminModel newAccModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_dbContext.Admins.Any(adm => adm.Username == newAccModel.Username))
                    {
                        return Unauthorized("Username Already Exist.");
                    }

                    Role role = _dbContext.Roles.Find(newAccModel.RoleId);
                    if (role == null)
                    {
                        return NotFound("Role not found");
                    }

                    string salt = BCrypt.Net.BCrypt.GenerateSalt(10);

                    string hashedPwd = BCrypt.Net.BCrypt.HashPassword(newAccModel.Password, salt);

                    Admin newAcc = new Admin()
                    {
                        Username = newAccModel.Username,
                        FullName = newAccModel.Fullname,
                        Email = newAccModel.Email,
                        Telephone = newAccModel.Telephone,
                        Password = hashedPwd,
                        RoleId = role.Id
                    };

                    _dbContext.Add(newAcc);
                    _dbContext.SaveChanges();

                    return Created("Account created.", new AdminDTO()
                    {
                        Username = newAcc.Username,
                        FullName = newAcc.FullName,
                        Permissions = null,
                        Token = null
                    });
                }
                catch (Exception e)
                {
                    return Unauthorized("Create Account Error");
                }
            }

            return Unauthorized("Create account error");
        }

        [HttpGet]
        [Route("detail/{id}")]
        public IActionResult GetAccountDetail(int id)
        {
            try
            {
                Admin accDetail = _dbContext.Admins.Find(id);
                if (accDetail == null)
                {
                    return NotFound("Account not found");
                }

                AccountDTO detail = new AccountDTO()
                {
                    Id = accDetail.Id,
                    Fullname = accDetail.FullName,
                    Email = accDetail.Email,
                    Username = accDetail.Username,
                    Role = accDetail.Role.Name
                };

                return Ok(detail);

            }
            catch (Exception e)
            {
                return NotFound("Account not found.");
            }
        }
        
    }
}

