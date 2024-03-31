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
    [Route("/api/restaurant")]
    public class RestaurantController : Controller
    {
        private readonly DBContext _dbContext;

        public RestaurantController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        [Route("admin")]
        [Authorize]
        public IActionResult RestaurantByAdminDetails()
        {
            var ad = HttpContext.User;

            var adId = int.Parse(ad.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);

            Admin admin = _dbContext.Admins.Find(adId);

            if (admin == null)
            {
                return NotFound("Admin not found.");
            }
            

            Restaurant restaurant = _dbContext.Restaurants
                .Include(r => r.District)
                .Include(r => r.Category)
                .FirstOrDefault(r => r.AdminId == admin.Id);

            if (restaurant == null)
            {
                return NotFound("Restaurant not found." + adId);
            }
            

            City city = _dbContext.Cities.Find(restaurant.District.CityId);
            if (city == null)
            {
                return NotFound("City not found.");
            }

            RestaurantDTO res = new RestaurantDTO()
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                JoinDate = restaurant.JoinDate,
                Description = restaurant.Description,
                DeliveryHours = restaurant.DeliveryHours,
                MinimumDelivery = restaurant.MinimumDelivery,
                PrePaidRate = restaurant.PrePaidRate,
                Thumbnail = restaurant.Thumbnail,
                Banner = restaurant.Banner,
                DistrictId = restaurant.District.Id,
                District = restaurant.District.Name,
                CityId = restaurant.District.CityId,
                City = city.Name,
                CatId = restaurant.Category.Id,
                Category = restaurant.Category.Name
            };
            return Ok(res);
        }
        
        [HttpGet]
        [Route("{id}")]
        public IActionResult RestaurantDetails(int id)
        {
            Restaurant restaurant = _dbContext.Restaurants
                .Include(r => r.District)
                .Include(r => r.Category)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound("Restaurant not found.");
            }

            City city = _dbContext.Cities.Find(restaurant.District.CityId);
            if (city == null)
            {
                return NotFound("City not found.");
            }

            RestaurantDTO res = new RestaurantDTO()
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                JoinDate = restaurant.JoinDate,
                Description = restaurant.Description,
                DeliveryHours = restaurant.DeliveryHours,
                MinimumDelivery = restaurant.MinimumDelivery,
                PrePaidRate = restaurant.PrePaidRate,
                Thumbnail = restaurant.Thumbnail,
                Banner = restaurant.Banner,
                DistrictId = restaurant.District.Id,
                District = restaurant.District.Name,
                City = city.Name,
                CatId = restaurant.Category.Id,
                Category = restaurant.Category.Name,
                AdminId = null
            };
            return Ok(res);
        }

        [HttpGet]
        [Route("city/{id}")]
        public IActionResult GetRestaurantsByCity(int id)
        {
            try
            {
                City city = _dbContext.Cities.Find(id);
                if (city == null)
                {
                    return NotFound("City not found.");
                }

                List<RestaurantDTO> restaurants = _dbContext.Restaurants
                    .Where(r => r.District.CityId == city.Id)
                    .Include(r => r.District)
                    .Include(r => r.Category)
                    .Select(r => new RestaurantDTO()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Category = r.Category.Name,
                        District = r.District.Name,
                        Thumbnail = r.Thumbnail,
                        DeliveryHours = r.DeliveryHours
                    })
                    .OrderBy(x => Guid.NewGuid())
                    .ToList();
                return Ok(restaurants);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("district/{id}")]
        public IActionResult GetRestaurantsByDistrict(int id)
        {
            try
            {
                District district = _dbContext.Districts.Find(id);
                if (district == null)
                {
                    return NotFound("City not found.");
                }

                List<RestaurantDTO> restaurants = _dbContext.Restaurants
                    .Where(r => r.District.Id == district.Id)
                    .Include(r => r.District)
                    .Include(r => r.Category)
                    .Select(r => new RestaurantDTO()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Category = r.Category.Name,
                        District = r.District.Name,
                        Thumbnail = r.Thumbnail,
                        DeliveryHours = r.DeliveryHours
                    })
                    .ToList();
                return Ok(restaurants);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("category/{id}")]
        public IActionResult GetRestaurantsByCategory(int id)
        {
            try
            {
                Category category = _dbContext.Categories.Find(id);
                if (category == null)
                {
                    return NotFound("City not found.");
                }

                List<RestaurantDTO> restaurants = _dbContext.Restaurants
                    .Where(r => r.Category.Id == category.Id)
                    .Include(r => r.District)
                    .Include(r => r.Category)
                    .Select(r => new RestaurantDTO()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Category = r.Category.Name,
                        District = r.District.Name,
                        Thumbnail = r.Thumbnail,
                        DeliveryHours = r.DeliveryHours
                    })
                    .ToList();
                return Ok(restaurants);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        [Authorize]
        public IActionResult UpdateRestaurant([FromForm] RestaurantModel updateRestaurantModel)
        {
            if (ModelState.IsValid)
            {
                var ad = HttpContext.User;

                var adId = int.Parse(ad.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);

                Admin admin = _dbContext.Admins.Find(adId);

                if (admin == null)
                {
                    return NotFound("Admin not found.");
                }
            

                Restaurant updateRestaurant = _dbContext.Restaurants
                    .Include(r => r.District)
                    .Include(r => r.Category)
                    .FirstOrDefault(r => r.AdminId == admin.Id);

                if (updateRestaurant == null)
                {
                    return NotFound("Restaurant not found.");
                }

                District district = _dbContext.Districts.Find(updateRestaurantModel.DistrictId);
                if (district == null)
                {
                    return NotFound("District not found.");
                }

                Category category = _dbContext.Categories.Find(updateRestaurantModel.CatId);
                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                updateRestaurant.Name = updateRestaurantModel.Name;
                updateRestaurant.Address = updateRestaurantModel.Address;
                updateRestaurant.Description = updateRestaurantModel.Description;
                updateRestaurant.DeliveryHours = updateRestaurantModel.DeliveryHours;
                updateRestaurant.PrePaidRate = updateRestaurantModel.PrePaidRate;
                updateRestaurant.MinimumDelivery = updateRestaurantModel.MinimumDelivery;
                updateRestaurant.DistrictId = district.Id;
                updateRestaurant.CatId = category.Id;
                
                if (updateRestaurantModel.Thumbnail != null)
                {
                    var path = "wwwroot/images";

                    if (updateRestaurant.Thumbnail != "blank-restaurant.png")
                    {
                        var oldThumb = Path.Combine(path, updateRestaurant.Thumbnail);
                        System.IO.File.Delete(oldThumb);
                    }
                    
                    var newFileName = Guid.NewGuid().ToString() + Path.GetFileName(updateRestaurantModel.Thumbnail.FileName);

                    var upload = Path.Combine(Directory.GetCurrentDirectory(), path, newFileName);

                    using (var stream = new FileStream(upload, FileMode.Create))
                    {
                        updateRestaurantModel.Thumbnail.CopyTo(stream);
                    }
                    updateRestaurant.Thumbnail = newFileName;
                }
                
                if (updateRestaurantModel.Banner != null)
                {
                    var path = "wwwroot/images";
                    
                    if (updateRestaurant.Banner != "blank-restaurant-banner.png")
                    {
                        var oldBanner = Path.Combine(path, updateRestaurant.Banner);
                        System.IO.File.Delete(oldBanner);
                    }

                    var newFileName = Guid.NewGuid().ToString() + Path.GetFileName(updateRestaurantModel.Banner.FileName);

                    var upload = Path.Combine(Directory.GetCurrentDirectory(), path, newFileName);

                    using (var stream = new FileStream(upload, FileMode.Create))
                    {
                        updateRestaurantModel.Banner.CopyTo(stream);
                    }
                    updateRestaurant.Banner = newFileName;
                }

                _dbContext.SaveChanges();
                return Ok(new RestaurantDTO()
                {
                    Name = updateRestaurant.Name
                });
            }

            return Unauthorized("Update restaurant error.");
        }
        
        
    }
}

