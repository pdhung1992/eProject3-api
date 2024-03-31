using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/cities")]
    public class CityController : Controller
    {
        private readonly DBContext _dbContext;

        public CityController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<CityDTO> cities = _dbContext.Cities
                    .Select(city => new CityDTO()
                    {
                        Id = city.Id,
                        Name = city.Name,
                        Thumbnail = city.Thumbnail
                    })
                    .ToList();
                return Ok(cities);
            }
            catch (Exception e)
            {
                return NotFound("Cities not found.");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetCityDetails(int id)
        {
            try
            {
                City city = _dbContext.Cities.Find(id);

                if (city == null)
                {
                    return NotFound("City not found");
                }

                return Ok(new CityDTO()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Thumbnail = city.Thumbnail
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public IActionResult CreateCity([FromForm] CityModel creModel)
        {
            if (ModelState.IsValid)
            {

                var path = "wwwroot/images";

                var fileName = Guid.NewGuid().ToString() + Path.GetFileName(creModel.Thumbnail.FileName);

                var upload = Path.Combine(Directory.GetCurrentDirectory(), path, fileName);

                using (var stream = new FileStream(upload, FileMode.Create))
                {
                    creModel.Thumbnail.CopyTo(stream);
                }

                City newCity = new City()
                {
                    Name = creModel.Name,
                    Thumbnail = fileName
                };

                _dbContext.Cities.Add(newCity);
                _dbContext.SaveChanges();

                return Created("City created", new CityModel()
                {
                    Name = newCity.Name
                });
            }

            return BadRequest("Create city error");
        }

        [HttpPost]
        [Route("update/{id}")]
        [Authorize]
        public IActionResult UpdateCity([FromForm] CityModel updateCityModel, int id)
        {
            if (ModelState.IsValid)
            {
                City updateCity = _dbContext.Cities.Find(id);
                if (updateCity == null)
                {
                    return NotFound("City not found.");
                }
                
                if (updateCityModel.Thumbnail != null)
                {
                    var path = "wwwroot/images";

                    var oldThumb = Path.Combine(path, updateCity.Thumbnail);
                    System.IO.File.Delete(oldThumb);

                    var newFileName = Guid.NewGuid().ToString() + Path.GetFileName(updateCityModel.Thumbnail.FileName);

                    var upload = Path.Combine(Directory.GetCurrentDirectory(), path, newFileName);

                    using (var stream = new FileStream(upload, FileMode.Create))
                    {
                        updateCityModel.Thumbnail.CopyTo(stream);
                    }
                    updateCity.Thumbnail = newFileName;
                }
                
                updateCity.Name = updateCityModel.Name;
                _dbContext.SaveChanges();
                return Ok(new CityDTO()
                {
                    Name = updateCity.Name
                });
            }

            return BadRequest("Update city Error.");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize]
        public IActionResult DeleteCity(int id)
        {
            try
            {
                City delCity = _dbContext.Cities.Find(id);
                if (delCity == null)
                {
                    return NotFound("City not found.");
                }
                
                var path = "wwwroot/images";
                var imgPath = Path.Combine(path, delCity.Thumbnail);

                if (!System.IO.File.Exists(imgPath))
                {
                    return NotFound();
                }
            
                System.IO.File.Delete(imgPath);
                _dbContext.Cities.Remove(delCity);
                _dbContext.SaveChanges();
                return Ok("City deleted.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("search/key={key}")]
        public IActionResult SearchCity(string key)
        {
            try
            {
                string lowercaseKey = key.ToLower();
                
                List<CityDTO> cities = _dbContext.Cities
                    .Where(c => c.Name.ToLower().Contains(lowercaseKey))
                    .Select(c => new CityDTO()
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList();
                
                return Ok(cities);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

