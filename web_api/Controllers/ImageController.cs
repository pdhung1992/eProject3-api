using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/images")]
    public class ImageController : Controller
    {
        private readonly DBContext _dbContext;
        

        [HttpGet]
        [Route("{filename}")]
        public IActionResult GetImage(string filename)
        {
            var path = "wwwroot/images";

            var imgPath = Path.Combine(path, filename);

            if (!System.IO.File.Exists(imgPath))
            {
                return NotFound();
            }

            var imgBytes = System.IO.File.ReadAllBytes(imgPath);

            return File(imgBytes, "image/jpeg");
        }

        [HttpDelete]
        [Route("delete/{filename}")]
        public IActionResult DeleteImage(string filename)
        {
            var path = "wwwroot/images";
            var imgPath = Path.Combine(path, filename);

            if (!System.IO.File.Exists(imgPath))
            {
                return NotFound();
            }
            
            System.IO.File.Delete(imgPath);

            return Ok("Image deleted");

        }
        
    }
}

