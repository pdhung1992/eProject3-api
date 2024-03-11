using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/cities")]
    public class CityController : Controller
    {
        private readonly DBContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CityController(DBContext context, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateCity([FromForm] CityModel creModel)
        {
            if (ModelState.IsValid)
            {
                
                var imgPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
                

                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }

                var fileName = creModel.Thumbnail.FileName;
                var fileExtension = Path.GetExtension(fileName);
                var uniqFilename = $"{DateTime.Now.Ticks}{fileExtension}";

                var filePath = Path.Combine(imgPath, uniqFilename);

                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    creModel.Thumbnail.CopyToAsync(stream);
                }

                City newCity = new City()
                {
                    Name = creModel.Name,
                    Thumbnail = filePath
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
    }
}

