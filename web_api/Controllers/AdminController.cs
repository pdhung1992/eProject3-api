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
        public IActionResult CreateAccount([FromForm] AdminModel newAccModel)
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
                    
                    if (role.Id == 2)
                    {
                        
                        var path = "wwwroot/images";

                        var thumbName = Guid.NewGuid().ToString() + Path.GetFileName(newAccModel.ResThumbnail?.FileName);
                        var bannerName = Guid.NewGuid().ToString() + Path.GetFileName(newAccModel.ResBanner?.FileName);

                        var uploadThumb = Path.Combine(Directory.GetCurrentDirectory(), path, thumbName);
                        var uploadBanner = Path.Combine(Directory.GetCurrentDirectory(), path, bannerName);

                        using (var stream = new FileStream(uploadThumb, FileMode.Create))
                        {
                            newAccModel.ResThumbnail?.CopyTo(stream);
                        }
                        
                        using (var stream = new FileStream(uploadBanner, FileMode.Create))
                        {
                            newAccModel.ResBanner?.CopyTo(stream);
                        }
                        
                        
                        Restaurant newRestaurant = new Restaurant()
                        {
                            Name = newAccModel.ResName,
                            Address = newAccModel.ResAddress,
                            JoinDate = DateTime.Now,
                            Description = newAccModel.ResDescription,
                            DeliveryHours = newAccModel.ResDeliveryHours,
                            MinimumDelivery = newAccModel.ResMinimumDelivery,
                            Thumbnail = thumbName,
                            Banner = bannerName,
                            DistrictId = newAccModel.ResDistrictId,
                            CatId = newAccModel.ResCatId,
                            AdminId = newAcc.Id,
                        };

                        _dbContext.Add(newRestaurant);
                        _dbContext.SaveChanges();
                    }
                    
                    
                    return Created("Account created.", new AdminDTO()
                    {
                        Username = newAcc.Username,
                        FullName = newAcc.FullName,
                        Permissions = null,
                        Token = null,
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
        [Route("details/{id}")]
        public IActionResult GetAccountDetail(int id)
        {
            try
            {
                Admin accDetail = _dbContext.Admins.Find(id);
                if (accDetail == null)
                {
                    return NotFound("Account not found");
                }
                
                Role role = _dbContext.Roles.Find(accDetail.RoleId);
                if (role == null)
                {
                    return NotFound("Role not found.");
                }

                AccountDTO detail = new AccountDTO()
                {
                    Id = accDetail.Id,
                    Fullname = accDetail.FullName,
                    Email = accDetail.Email,
                    Telephone = accDetail.Telephone,
                    Username = accDetail.Username,
                    RoleId = role.Id,
                    Role = role.Name,
                };

                return Ok(detail);

            }
            catch (Exception e)
            {
                return BadRequest("Get account detail error.");
            }
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult UpdateAccount(int id, UpdateAdminModel updateModel)
        {
            if (ModelState.IsValid)
            {
                Admin updateAdmin = _dbContext.Admins.Find(id);
                if (updateAdmin == null)
                {
                    return NotFound("Account notfound.");
                }
                
                updateAdmin.Email = updateModel.Email;
                updateAdmin.Telephone = updateModel.Telephone;
                updateAdmin.FullName = updateModel.Fullname;

                _dbContext.SaveChanges();
                return Ok(new AdminDTO()
                {
                    FullName = updateModel.Fullname
                });
            }

            return BadRequest("Update Account error.");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteAccount(int id)
        {
            try
            {
                Admin delAcc = _dbContext.Admins.Find(id);
                if (delAcc == null)
                {
                    return NotFound("Account not found.");
                }
                
                _dbContext.Admins.Remove(delAcc);
                _dbContext.SaveChanges();

                if (delAcc.RoleId == 2)
                {
                    Restaurant delRes = _dbContext.Restaurants.FirstOrDefault(r => r.AdminId == delAcc.Id);
                    if (delRes == null)
                    {
                        return NotFound("Restaurant not found.");
                    }
                    
                    var path = "wwwroot/images";
                    var thumbPath = Path.Combine(path, delRes.Thumbnail);
                    var bannerPath = Path.Combine(path, delRes.Banner);

                    if (delRes.Thumbnail != "blank-restaurant.png" && System.IO.File.Exists(thumbPath))
                    {
                        System.IO.File.Delete(thumbPath);
                    }
                    if (delRes.Banner != "blank-restaurant-banner.png" && System.IO.File.Exists(bannerPath))
                    {
                        System.IO.File.Delete(bannerPath);
                    }
                    _dbContext.Remove(delRes);
                }
                
                return Ok("Account deleted.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}

