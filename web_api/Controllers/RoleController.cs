using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.DTOs;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/roles")]
    public class RoleController : Controller
    {
        private readonly DBContext _dbContext;
        
        public RoleController (DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<RoleDTO> roles = _dbContext.Roles
                    .Select(role => new RoleDTO()
                    {
                        Id = role.Id,
                        Name = role.Name
                    })
                    .ToList();

                return Ok(roles);
            }
            catch (Exception e)
            {
                return BadRequest("Get Role error");
            }
            
        }
    }
}

