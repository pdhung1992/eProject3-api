using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/foods")]
    public class FoodController : Controller
    {
        private readonly DBContext _dbContext;

        public FoodController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        [Route("restaurant/{id}")]
        public IActionResult GetFoodByRestaurant(int id)
        {
            try
            {
                List<FoodDTO> foods = _dbContext.Foods
                    .Include(f => f.FoodType)
                    .Include(f => f.FoodTag)
                    .Include(f => f.ServeType)
                    .Where(f => f.ResId == id)
                    .Select(f => new FoodDTO()
                    {
                        Id = f.Id,
                        Name = f.Name,
                        TypeId = f.FoodType.Id,
                        Type = f.FoodType.Name,
                        Tag = f.FoodTag.Name,
                        Price = f.Price,
                        Serve = f.ServeType.Name,
                        Description = f.Description,
                        Thumbnail = f.Thumbnail
                    })
                    .ToList();
                return Ok(foods);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("restaurant/{id}/type/{tId}/serve/{sId}")]
        public IActionResult GetFoodByTypeAndServeType(int id, int tId, int sId)
        {
            try
            {
                List<FoodDTO> foods = _dbContext.Foods
                    .Include(f => f.FoodType)
                    .Include(f => f.FoodTag)
                    .Include(f => f.ServeType)
                    .Where(f => f.ResId == id && f.TypeId == tId && f.ServeId == sId)
                    .Select(f => new FoodDTO()
                    {
                        Id = f.Id,
                        Name = f.Name,
                        TypeId = f.FoodType.Id,
                        Type = f.FoodType.Name,
                        Tag = f.FoodTag.Name,
                        Price = f.Price,
                        Serve = f.ServeType.Name,
                        Description = f.Description,
                        Thumbnail = f.Thumbnail
                    })
                    .ToList();
                return Ok(foods);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("restaurant/{id}/type/{tid}")]
        public IActionResult GetFoodByType(int id, int tid)
        {
            try
            {
                List<FoodDTO> foods = _dbContext.Foods
                    .Include(f => f.FoodType)
                    .Include(f => f.FoodTag)
                    .Include(f => f.ServeType)
                    .Where(f => f.ResId == id && f.TypeId == tid)
                    .Select(f => new FoodDTO()
                    {
                        Id = f.Id,
                        Name = f.Name,
                        TypeId = f.FoodType.Id,
                        Type = f.FoodType.Name,
                        Tag = f.FoodTag.Name,
                        Price = f.Price,
                        Serve = f.ServeType.Name,
                        Description = f.Description,
                        Thumbnail = f.Thumbnail
                    })
                    .ToList();
                return Ok(foods);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("details/{id}")]
        public IActionResult GetFoodDetails(int id)
        {
            try
            {
                Food food = _dbContext.Foods
                    .Include(f => f.FoodType)
                    .Include(f => f.FoodTag)
                    .Include(f => f.ServeType)
                    .FirstOrDefault(f => f.Id == id);
                
                if (food == null)
                {
                    return NotFound("Food not found.");
                }

                return Ok(new FoodDTO()
                {
                    Id = food.Id,
                    Name = food.Name,
                    TypeId = food.FoodType.Id,
                    Type = food.FoodType.Name,
                    FoodTagId = food.FoodTag.Id,
                    Tag = food.FoodTag.Name,
                    Price = food.Price,
                    Serve = food.ServeType.Name,
                    Description = food.Description,
                    Thumbnail = food.Thumbnail
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
        public IActionResult CreateFood([FromForm] FoodModel creFoodModel)
        {
            if (ModelState.IsValid)
            {
                var path = "wwwroot/images";

                var fileName = Guid.NewGuid().ToString() + Path.GetFileName(creFoodModel.Thumbnail.FileName);

                var upload = Path.Combine(Directory.GetCurrentDirectory(), path, fileName);

                using (var stream = new FileStream(upload, FileMode.Create))
                {
                    creFoodModel.Thumbnail.CopyTo(stream);
                }
                
                var ad = HttpContext.User;

                var adId = int.Parse(ad.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);

                Restaurant restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.AdminId == adId);
                if (restaurant == null)
                {
                    return NotFound("Restaurant not found.");
                }

                ServeType serveType = _dbContext.ServeTypes.Find(creFoodModel.ServeId);
                if (serveType == null)
                {
                    return NotFound("Serve type not found.");
                }

                FoodType foodType = _dbContext.FoodTypes.Find(creFoodModel.TypeId);
                if (foodType == null)
                {
                    return NotFound("Type not found.");
                }

                FoodTag foodTag = _dbContext.FoodTags.Find(creFoodModel.FoodTagId);
                if (foodTag == null)
                {
                    return NotFound("Tag not found");
                }

                Food newFood = new Food()
                {
                    Name = creFoodModel.Name,
                    Description = creFoodModel.Description,
                    Thumbnail = fileName,
                    Price = creFoodModel.Price,
                    ServeId = serveType.Id,
                    TypeId = foodType.Id,
                    FoodTagId = foodTag.Id,
                    ResId = restaurant.Id
                };

                _dbContext.Foods.Add(newFood);
                _dbContext.SaveChanges();
                return Created("Food created.", new FoodDTO
                {
                    Name = newFood.Name
                });
            }

            return BadRequest("Create food error.");
        }

        [HttpPut]
        [Route("update/{id}")]
        [Authorize]
        public IActionResult UpdateFood([FromForm]FoodModel updateFoodModel, int id)
        {
            if (ModelState.IsValid)
            {
                Food updateFood = _dbContext.Foods.Find(id);
                if (updateFood == null)
                {
                    return NotFound("Food not found.");
                }
                
                ServeType serveType = _dbContext.ServeTypes.Find(updateFoodModel.ServeId);
                if (serveType == null)
                {
                    return NotFound("Serve type not found.");
                }

                FoodType type = _dbContext.FoodTypes.Find(updateFoodModel.TypeId);
                if (type == null)
                {
                    return NotFound("Type not found.");
                }

                FoodTag tag = _dbContext.FoodTags.Find(updateFoodModel.FoodTagId);
                if (tag == null)
                {
                    return NotFound("Food tag not found.");
                }

                updateFood.Name = updateFoodModel.Name;
                updateFood.Description = updateFoodModel.Description;
                updateFood.Price = updateFoodModel.Price;
                updateFood.ServeId = serveType.Id;
                updateFood.TypeId = type.Id;
                updateFood.FoodTagId = tag.Id;
                
                if (updateFoodModel.Thumbnail != null)
                {
                    var path = "wwwroot/images";
                    
                    var oldThumb = Path.Combine(path, updateFood.Thumbnail);
                    System.IO.File.Delete(oldThumb);
                    
                    
                    var newFileName = Guid.NewGuid().ToString() + Path.GetFileName(updateFoodModel.Thumbnail.FileName);

                    var upload = Path.Combine(Directory.GetCurrentDirectory(), path, newFileName);

                    using (var stream = new FileStream(upload, FileMode.Create))
                    {
                        updateFoodModel.Thumbnail.CopyTo(stream);
                    }
                    updateFood.Thumbnail = newFileName;
                }

                _dbContext.SaveChanges();
                return Ok(new FoodDTO()
                {
                    Name = updateFood.Name
                });
            }

            return BadRequest("Update food error.");
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteFood(int id)
        {
            try
            {
                Food delFood = _dbContext.Foods.Find(id);
                if (delFood == null)
                {
                    return NotFound("Food not found.");
                }
                
                var path = "wwwroot/images";
                var imgPath = Path.Combine(path, delFood.Thumbnail);

                if (!System.IO.File.Exists(imgPath))
                {
                    return NotFound("Thumbnail file not found.");
                }
                
                System.IO.File.Delete(imgPath);
                _dbContext.Foods.Remove(delFood);
                _dbContext.SaveChanges();

                return Ok("Food deleted.");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

