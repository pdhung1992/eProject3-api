using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : Controller
    {
        private readonly DBContext _dbContext;
        private IConfiguration _configuration;

        public AuthController(DBContext context, IConfiguration config)
        {
            _dbContext = context;
            _configuration = config;
        }
        
        //user register
        [HttpPost("user/register")]
        public IActionResult UserRegister(UserModel regModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_dbContext.Users.Any(user => user.Email == regModel.Email))
                    {
                        return Unauthorized("Email already exist.");
                    }

                    string salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                    string hashedPwd = BCrypt.Net.BCrypt.HashPassword(regModel.Password, salt);

                    User newUser = new User()
                    {
                        FullName = regModel.FullName,
                        Email = regModel.Email,
                        Telephone = regModel.Telephone,
                        Password = hashedPwd
                    };

                    _dbContext.Users.Add(newUser);
                    _dbContext.SaveChanges();
                    return Created("Registration success!", new UserDTO()
                    {
                        FullName = newUser.FullName,
                        Email = newUser.Email,
                        Token = null
                    });
                }
                catch (Exception e)
                {
                    return Unauthorized("Registration error.");
                }
            }

            return Unauthorized("Registration error.");
        }
        
        //user login
        [HttpPost("user/login")]
        public IActionResult UserLogIn(UserLogInModel logInModel)
        {
            if (ModelState.IsValid)
            { 
                var user = _dbContext.Users.SingleOrDefault(u => u.Email == logInModel.Email);

                if (user != null)
                {
                    bool pwdMatch = BCrypt.Net.BCrypt.Verify(logInModel.Password, user.Password);
                    
                    if (pwdMatch)
                    {
                        var payload = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                            new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"])
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Issuer"],
                            audience: _configuration["Audience"],
                            claims: payload,
                            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["LifeTime"])),
                            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                        );

                        return Ok(new UserDTO()
                        {
                            FullName = user.FullName,
                            Email = user.Email,
                            Token = new JwtSecurityTokenHandler().WriteToken(token)
                        });
                    }
                    return Unauthorized("Email or password incorrect. ");
                }

                return Unauthorized("Email or Password incorrect. ");
            }

            return Unauthorized("Email or Password incorrect. ");
        }
        
        //admin create account

        [HttpPost]
        [Route("admin/create")]
        public IActionResult CreateAdmAccount(AdminModel createModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_dbContext.Admins.Any(adm => adm.Username == createModel.Username))
                    {
                        return Unauthorized("Username Already Exist.");
                    }

                    Role role = _dbContext.Roles.Find(createModel.RoleId);
                    if (role == null)
                    {
                        return NotFound("Role not found");
                    }

                    string salt = BCrypt.Net.BCrypt.GenerateSalt(10);

                    string hashedPwd = BCrypt.Net.BCrypt.HashPassword(createModel.Password, salt);

                    Admin newAdminAcc = new Admin()
                    {
                        Username = createModel.Username,
                        FullName = createModel.Fullname,
                        Email = createModel.Email,
                        Telephone = createModel.Telephone,
                        Password = hashedPwd,
                        RoleId = role.Id
                    };

                    _dbContext.Add(newAdminAcc);
                    _dbContext.SaveChanges();

                    return Created("Account created.", new AdminDTO()
                    {
                        Username = newAdminAcc.Username,
                        FullName = newAdminAcc.FullName,
                        Permissions = null,
                        Token = null
                    });
                }
                catch (Exception e)
                {
                    return Unauthorized("Create Account Error");
                }
            }

            return Unauthorized("Create Account Error.");
        }
        
        //admin login

        [HttpPost]
        [Route("admin/login")]
        public IActionResult AdmLogin(AdminLoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var admin = _dbContext.Admins.Include(a => a.Role).SingleOrDefault(a => a.Username == loginModel.Username);

                if (admin != null)
                {
                    bool pwdMatch = BCrypt.Net.BCrypt.Verify(loginModel.Password, admin.Password);

                    if (pwdMatch)
                    {
                        var payload = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                            new Claim(ClaimTypes.Name, admin.Username),
                            new Claim("Role", admin.Role.Name),
                            new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                            new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"])
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Issuer"],
                            audience: _configuration["Audience"],
                            claims: payload,
                            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt: LifeTime"])),
                            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                            );

                        List<PermissionDTO> permissions = _dbContext.Permissions
                            .Where(p => p.RoleId == admin.RoleId)
                            .OrderBy(p => p.SortOrder)
                            .Select(p => new PermissionDTO()
                            {
                                Name = p.Name,
                                Prefix = p.Prefix,
                                FaIcon = p.FaIcon
                            })
                            .ToList();
                        
                        Restaurant restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.AdminId == admin.Id);
                        int? resId = restaurant != null ? restaurant.Id : null;

                        return Ok(new AdminDTO()
                        {
                            Id = admin.Id,
                            Username = admin.Username,
                            FullName = admin.FullName,
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Permissions = permissions,
                            ResId = resId
                        });
                    }
                    return Unauthorized("Username or password is not correct.");
                }
                
                return Unauthorized("Username or password is not correct.");
            }

            return Unauthorized("Username or password is not correct.");
        }
        
        [HttpPost]
        [Route("changepassword")]
        [Authorize]
        public IActionResult ChangePassword(ChangePwdModel changePwdModel)
        {
            var adm = HttpContext.User;
            var username = adm.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;

            Admin updateAdm = _dbContext.Admins.SingleOrDefault(e => e.Username == username);

            if (updateAdm != null)
            {
                bool passwordMatch = BCrypt.Net.BCrypt.Verify(changePwdModel.currentPassword, updateAdm.Password);

                if (passwordMatch)
                {
                    updateAdm.Password = BCrypt.Net.BCrypt.HashPassword(changePwdModel.newPassword);
                    _dbContext.SaveChanges();
                    return Ok( new
                        {
                            Message = "Password change",
                            username = username
                        }
                    );
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

