using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/permissions")]
    public class PerMissionController : Controller
    {
        private readonly DBContext _dbContext;

        public PerMissionController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<PermissionDTO> permissions = _dbContext.Permissions.Include(p => p.Role)
                    .Select(p => new PermissionDTO()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Role = p.Role.Name,
                        Prefix = null,
                        FaIcon = null
                    })
                    .ToList();
                return Ok(permissions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

