using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.DTOs;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/status")]
    public class StatusController : Controller
    {
        private readonly DBContext _dbContext;

        public StatusController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<StatusDTO> status = _dbContext.Status
                    .Select(s => new StatusDTO()
                    {
                        Id = s.Id,
                        Name = s.Name
                    })
                    .ToList();
                return Ok(status);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

