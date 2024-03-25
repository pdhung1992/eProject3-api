using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.DTOs;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/ftags")]
    public class FoodTagController : Controller
    {
        private readonly DBContext _dbContext;

        public FoodTagController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<FoodTagDTO> foodTags = _dbContext.FoodTags
                    .Select(t => new FoodTagDTO()
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();

                return Ok(foodTags);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

