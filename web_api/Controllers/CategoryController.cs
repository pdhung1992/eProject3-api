using Microsoft.AspNetCore.Mvc;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;
using web_api.Models;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/categories")]
    public class CategoryController : Controller
    {
        private readonly DBContext _dbContext;

        public CategoryController(DBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<CategoryDTO> categories = _dbContext.Categories
                    .Select(cat => new CategoryDTO()
                    {
                        Id = cat.Id,
                        Name = cat.Name,
                        Description = cat.Description
                    })
                    .ToList();

                return Ok(categories);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("details/{id}")]
        public IActionResult GetCategoryDetails(int id)
        {
            try
            {
                Category category = _dbContext.Categories.Find(id);
                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                return Ok(new CategoryDTO()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                });
            }
            catch (Exception e)
            {
                return BadRequest("Get category details error.");
            }
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateCategory(CategoryModel creModel)
        {
            if (ModelState.IsValid)
            {
                bool categoryExists = _dbContext.Categories.Any(c => c.Name == creModel.Name);
                if (categoryExists)
                {
                    return BadRequest("Category name already exists.");
                }
                
                Category newCat = new Category()
                {
                    Name = creModel.Name,
                    Description = creModel.Description
                };

                _dbContext.Categories.Add(newCat);
                _dbContext.SaveChanges();

                return Created("Category created.",new CategoryDTO()
                {
                    Name = newCat.Name
                });
            }

            return BadRequest("Create category error.");
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult UpdateCategory(int id, CategoryModel updateModel)
        {
            if (ModelState.IsValid)
            {
                Category updateCat = _dbContext.Categories.Find(id);
                if (updateCat == null)
                {
                    return NotFound("Category not found.");
                }

                updateCat.Name = updateModel.Name;
                updateCat.Description = updateModel.Description;

                _dbContext.SaveChanges();

                return Ok(new CategoryDTO()
                {
                    Name = updateCat.Name
                });
            }

            return BadRequest("Update category error.");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                Category delCat = _dbContext.Categories.Find(id);

                if (delCat == null)
                {
                    return NotFound("Category not found.");
                }

                _dbContext.Categories.Remove(delCat);
                _dbContext.SaveChanges();

                return Ok("Category deleted");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}

