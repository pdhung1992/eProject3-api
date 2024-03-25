using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/districts")]
    public class DistrictController : Controller
    {
        private readonly DBContext _dbContext;

        public DistrictController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<DistrictDTO> districts = _dbContext.Districts.Include(d => d.City)
                    .Select(d => new DistrictDTO()
                    {
                        Id = d.Id,
                        Name = d.Name,
                        CityName = d.City.Name
                    })
                    .ToList();
                return Ok(districts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("bycity/{id}")]
        public IActionResult GetDistrictInCity(int id)
        {
            try
            {
                List<DistrictDTO> districts = _dbContext.Districts.Include(d => d.City)
                    .Where(d => d.City.Id == id)
                    .Select(d => new DistrictDTO()
                    {
                        Id = d.Id,
                        Name = d.Name,
                        CityName = d.City.Name
                    })
                    .ToList();
                return Ok(districts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("details/{id}")]
        public IActionResult GetDistrictDetails(int id)
        {
            try
            {
                District district = _dbContext.Districts.Find(id);
                if (district == null)
                {
                    return NotFound("District not found.");
                }

                City city = _dbContext.Cities.Find(district.CityId);
                if (city == null)
                {
                    return NotFound("City not found.");
                }

                return Ok(new DistrictDTO()
                {
                    Id = district.Id,
                    Name = district.Name,
                    CityId = city.Id,
                    CityName = city.Name
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateDistrict(DistrictModel creModel)
        {
            if (ModelState.IsValid)
            {
                City city = _dbContext.Cities.Find(creModel.CityId);
                if (city == null)
                {
                    return NotFound("City not found.");
                }

                District newDist = new District()
                {
                    Name = creModel.Name,
                    CityId = city.Id
                };

                _dbContext.Districts.Add(newDist);
                _dbContext.SaveChanges();

                return Created("District created.", new DistrictDTO()
                {
                    Name = newDist.Name
                });
            }

            return BadRequest("Create district error.");
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult UpdateDistrict(int id, DistrictModel updateModel)
        {
            if (ModelState.IsValid)
            {
                District updateDist = _dbContext.Districts.Find(id);
                if (updateDist == null)
                {
                    return NotFound("District not found.");
                }
                
                City city = _dbContext.Cities.Find(updateModel.CityId);
                if (city == null)
                {
                    return NotFound("City not found.");
                }

                updateDist.Name = updateModel.Name;
                updateDist.CityId = city.Id;

                _dbContext.SaveChanges();
                return Ok(new DistrictDTO()
                {
                    Name = updateDist.Name
                });
            }
            return BadRequest("Create district error.");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteDistrict(int id)
        {
            try
            {
                District delDist = _dbContext.Districts.Find(id);
                if (delDist == null)
                {
                    return NotFound("Districy not found");
                }

                _dbContext.Districts.Remove(delDist);
                _dbContext.SaveChanges();
                return Ok("District deleted");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

