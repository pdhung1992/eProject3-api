using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/combos")]
    public class ComboController : Controller
    {
        private readonly DBContext _dbContext;

        public ComboController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        [Route("restaurant/{id}")]
        public IActionResult GetCombosByRestaurant(int id)
        {
            try
            {
                List<ComboDTO> combos = _dbContext.Combos
                    .Include(c => c.ServeType)
                    .Include(c => c.FoodTag)
                    .Where(c => c.Restaurant.Id == id)
                    .Select(c => new ComboDTO()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        DiscountRate = c.DiscountRate,
                        Serve = c.ServeType.Name,
                        Thumbnail = c.Thumbnail,
                        Tag = c.FoodTag.Name
                    })
                    .ToList();
                return Ok(combos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("restaurant/{rId}/serve/{sId}")]
        public IActionResult GetCombosByServe(int rId, int sId)
        {
            try
            {
                List<ComboDTO> combos = _dbContext.Combos
                    .Include(c => c.ServeType)
                    .Include(c => c.FoodTag)
                    .Where(c => c.Restaurant.Id == rId && c.ServeId == sId)
                    .Select(c => new ComboDTO()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        DiscountRate = c.DiscountRate,
                        Serve = c.ServeType.Name,
                        Thumbnail = c.Thumbnail,
                        Tag = c.FoodTag.Name
                    })
                    .ToList();
                return Ok(combos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("details/{id}")]
        public IActionResult GetComboDetails(int id)
        {
            try
            {
                Combo combo = _dbContext.Combos
                    .Include(c => c.ServeType)
                    .Include(c => c.FoodTag)
                    .FirstOrDefault(c => c.Id == id);

                if (combo == null)
                {
                    return NotFound("Combo notfound");
                }

                List<FoodDTO> details = _dbContext.ComboDetails
                    .Include(d => d.Food)
                    .Where(d => d.ComboId == combo.Id)
                    .Select(d => new FoodDTO()
                    {
                        Id = d.Id,
                        Name = d.Food.Name,
                        TypeId = d.Food.FoodType.Id,
                        Type = d.Food.FoodType.Name,
                        FoodTagId = d.Food.FoodTag.Id,
                        Tag = d.Food.FoodTag.Name,
                        Price = d.Food.Price,
                        Serve = d.Food.ServeType.Name,
                        Description = d.Food.Description,
                        Thumbnail = d.Food.Thumbnail
                    })
                    .ToList();

                return Ok(new ComboDTO()
                {
                    Id = combo.Id,
                    Name = combo.Name,
                    Description = combo.Description,
                    DiscountRate = combo.DiscountRate,
                    Serve = combo.ServeType.Name,
                    Thumbnail = combo.Thumbnail,
                    Tag = combo.FoodTag.Name,
                    Foods = details
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Create")]
        [Authorize]
        public IActionResult CreateCombo(int id, [FromForm] ComboModel creComboModel)
        {
            if (ModelState.IsValid)
            {
                var path = "wwwroot/images";

                var fileName = Guid.NewGuid().ToString() + Path.GetFileName(creComboModel.Thumbnail.FileName);

                var upload = Path.Combine(Directory.GetCurrentDirectory(), path, fileName);

                using (var stream = new FileStream(upload, FileMode.Create))
                {
                    creComboModel.Thumbnail.CopyTo(stream);
                }
                
                var ad = HttpContext.User;

                var adId = int.Parse(ad.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);
                
                Restaurant restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.AdminId == adId);
                if (restaurant == null)
                {
                    return NotFound("Restaurant not found.");
                }

                ServeType serveType = _dbContext.ServeTypes.Find(creComboModel.ServeId);
                if (serveType == null)
                {
                    return NotFound("Serve type not found.");
                }

                FoodTag foodTag = _dbContext.FoodTags.Find(creComboModel.ComboTagId);
                if (foodTag == null)
                {
                    return NotFound("Tag not found");
                }

                Combo newCombo = new Combo()
                {
                    Name = creComboModel.Name,
                    Description = creComboModel.Description,
                    DiscountRate = creComboModel.DiscountRate,
                    ResId = restaurant.Id,
                    ServeId = serveType.Id,
                    ComboTagId = foodTag.Id,
                    Thumbnail = fileName
                };

                _dbContext.Combos.Add(newCombo);
                _dbContext.SaveChanges();

                foreach (var foodId in creComboModel.foods)
                {
                    ComboDetail newComboDetail = new ComboDetail()
                    {
                        ComboId = newCombo.Id,
                        FoodId = foodId
                    };

                    _dbContext.ComboDetails.Add(newComboDetail);
                }
                _dbContext.SaveChanges();

                return Created("Combo created", new ComboDTO()
                {
                    Name = newCombo.Name
                });
            }

            return BadRequest("Create Combo error.");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteCombo(int id)
        {
            try
            {
                Combo delCombo = _dbContext.Combos.Find(id);
                if (delCombo == null)
                {
                    return NotFound("Combo not found.");
                }

                List<ComboDetail> delComboDetails = _dbContext.ComboDetails
                    .Where(d => d.ComboId == delCombo.Id)
                    .ToList();

                foreach (var detail in delComboDetails)
                {
                    _dbContext.ComboDetails.Remove(detail);
                }

                _dbContext.SaveChanges();

                var path = "wwwroot/images";
                var imgPath = Path.Combine(path, delCombo.Thumbnail);

                if (!System.IO.File.Exists(imgPath))
                {
                    return NotFound("Thumbnail file not found.");
                }
                
                System.IO.File.Delete(imgPath);
                _dbContext.Combos.Remove(delCombo);
                _dbContext.SaveChanges();

                return Ok("Combo deleted.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}

