
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;


namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/servetypes")]
    public class ServeTypeController : Controller
    {
        private readonly DBContext _dbContext;

        public ServeTypeController(DBContext context)
        {
            _dbContext = context;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<ServeTypeDTO> serveTypes = _dbContext.ServeTypes
                    .Select(t => new ServeTypeDTO()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Value = t.Value
                    })
                    .ToList();

                return Ok(serveTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("details/{id}")]
        public IActionResult GetServeTypeDetails(int id)
        {
            try
            {
                ServeType serveType = _dbContext.ServeTypes.Find(id);
                if (serveType == null)
                {
                    return NotFound("Type not found.");
                }

                return Ok(new ServeTypeDTO()
                {
                    Id = serveType.Id,
                    Name = serveType.Name,
                    Value = serveType.Value
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("restaurant/{id}")]
        public IActionResult GetRestaurantServeTypes( int id)
        {
            try
            {
                List<ServeTypeDTO> types = _dbContext.Foods
                    .Where(f => f.ResId == id)
                    .Select(f => new ServeTypeDTO()
                    {
                        Id = f.ServeType.Id,
                        Name = f.ServeType.Name,
                        Value = f.ServeType.Value
                    })
                    .Distinct()
                    .ToList();
                
                if (types.Any())
                {
                    return Ok(types);
                }
                else
                {
                    return NotFound("No serve types found for the restaurant.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}

