using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api.Contexts;
using web_api.DTOs;
using web_api.Entities;

namespace web_api.Controllers
{
    [ApiController]
    [Route("/api/reports")]
    [Authorize]
    public class ReportController: Controller
    {
        private readonly DBContext _dbContext;

        public ReportController(DBContext context)
        {
            _dbContext = context;
        }
        
        [HttpGet]
        [Route("month")]
        public IActionResult GetOrderByMonth(int month, int year)
        {
            try
            {
                var ad = HttpContext.User;

                var adId = int.Parse(ad.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);

                Admin admin = _dbContext.Admins.Find(adId);

                if (admin == null)
                {
                    return NotFound("Admin not found.");
                }
            
                Restaurant restaurant = _dbContext.Restaurants
                    .FirstOrDefault(r => r.AdminId == admin.Id);

                if (restaurant == null)
                {
                    return NotFound("Restaurant not found." + adId);
                }
                
                if (month < 1 || month > 12 || year < 1)
                {
                    return BadRequest("Invalid month");
                }

                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Date;
                endDate = endDate.AddDays(1).AddTicks(-1);

                List<ReportDTO> orders = _dbContext.Orders
                    .Include(o => o.Status)
                    .Where(o => o.RestaurantId == restaurant.Id && o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .Select(o => new ReportDTO()
                    {
                        Id = o.Id,
                        InvoiceNumber = o.InvoiceNumber,
                        OrderDate = o.OrderDate,
                        Status = o.Status.Name,
                        PrePaid = o.Status.Name.Equals("Canceled", StringComparison.OrdinalIgnoreCase) ? 0 : o.PrePaid,
                        TotalAmount = o.Status.Name.Equals("Canceled", StringComparison.OrdinalIgnoreCase) ? 0 : o.TotalAmount
                    })
                    .ToList();
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

