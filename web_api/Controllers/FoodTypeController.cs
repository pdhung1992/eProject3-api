using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.DTOs;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/types")]
    public class FoodTypeController : Controller
    {
        private readonly DBContext _dbContext;

        public FoodTypeController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<FoodTypeDTO> foodTypes = _dbContext.FoodTypes
                    .Select(t => new FoodTypeDTO()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        SortOrder = t.SortOrder
                    })
                    .ToList();

                return Ok(foodTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

