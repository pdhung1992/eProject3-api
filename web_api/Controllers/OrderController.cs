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
    [Route("/api/orders")]
    public class OrderController : Controller
    {
        private readonly DBContext _dbContext;

        public OrderController(DBContext context)
        {
            _dbContext = context;
        }


        [HttpGet]
        [Route("restaurant")]
        [Authorize]
        public IActionResult GetOrdersOfRestaurant()
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
                    .Include(r => r.District)
                    .Include(r => r.Category)
                    .FirstOrDefault(r => r.AdminId == admin.Id);

                if (restaurant == null)
                {
                    return NotFound("Restaurant not found." + adId);
                }

                List<OrderDTO> orders = _dbContext.Orders
                    .Include(o => o.Status)
                    .Where(o => o.RestaurantId == restaurant.Id)
                    .Select(o => new OrderDTO()
                    {
                        Id = o.Id,
                        InvoiceNumber = o.InvoiceNumber,
                        OrderDate = o.OrderDate,
                        EventName = o.EventName,
                        DeliveryAddress = o.DeliveryAddress,
                        DeliveryDate = o.DeliveryDate,
                        TotalAmount = o.TotalAmount,
                        StatusId = o.Status.Id,
                        Status = o.Status.Name
                    })
                    .ToList();
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("user")]
        [Authorize]
        public IActionResult GetOrdersOfUser()
        {
            try
            {
                var u = HttpContext.User;

                var userId = int.Parse(u.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value);

                User user = _dbContext.Users.Find(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                List<OrderDTO> orders = _dbContext.Orders
                    .Include(o => o.Status)
                    .Where(o => o.UserId == user.Id)
                    .Select(o => new OrderDTO()
                    {
                        Id = o.Id,
                        InvoiceNumber = o.InvoiceNumber,
                        OrderDate = o.OrderDate,
                        EventName = o.EventName,
                        DeliveryAddress = o.DeliveryAddress,
                        DeliveryDate = o.DeliveryDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status.Name,
                        StatusId = o.Status.Id
                    })
                    .ToList();
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("details/{id}")]
        [Authorize]
        public IActionResult GetOrderDetails(int id)
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
                    .Include(r => r.District)
                    .Include(r => r.Category)
                    .FirstOrDefault(r => r.AdminId == admin.Id);

                if (restaurant == null)
                {
                    return NotFound("Restaurant not found." + adId);
                }

                Order details = _dbContext.Orders
                    .Include(o => o.Status)
                    .FirstOrDefault(o => o.Id == id && o.RestaurantId == restaurant.Id);

                if (details == null)
                {
                    return NotFound("Order not found.");
                }

                List<OrderDetailDTO> foods = _dbContext.OrderDetails
                    .Include(d => d.Food)
                    .Where(d => d.OrderId == details.Id)
                    .Select(d => new OrderDetailDTO()
                    {
                        Thumbnail = d.Food.Thumbnail,
                        Name = d.Food.Name,
                        Price = d.Price,
                        Serve = d.Food.ServeType.Name
                    })
                    .ToList();

                return Ok(new OrderDTO()
                {
                    Id = details.Id,
                    InvoiceNumber = details.InvoiceNumber,
                    OrderDate = details.OrderDate,
                    EventName = details.EventName,
                    DeliveryAddress = details.DeliveryAddress,
                    DeliveryDate = details.DeliveryDate,
                    DeliveryPerson = details.DeliveryPerson,
                    DeliveryPhone = details.DeliveryPhone,
                    AdditionalRequirement = details.AdditionalRequirement,
                    TotalAmount = details.TotalAmount,
                    PrePaid = details.PrePaid,
                    StatusId = details.StatusId,
                    Status = details.Status.Name,
                    Foods = foods
                });
            }
            catch (Exception e)
            {
                return BadRequest("Get order details error.");
            }
        }
        
        [HttpGet]
        [Route("user/details/{id}")]
        [Authorize]
        public IActionResult GetUserOrderDetails(int id)
        {
            try
            {
                var u = HttpContext.User;

                var userId = int.Parse(u.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value);

                User user = _dbContext.Users.Find(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }
                

                Order details = _dbContext.Orders
                    .Include(o => o.Status)
                    .FirstOrDefault(o => o.Id == id);

                if (details == null)
                {
                    return NotFound("Order not found.");
                }

                List<OrderDetailDTO> foods = _dbContext.OrderDetails
                    .Include(d => d.Food)
                    .Where(d => d.OrderId == details.Id)
                    .Select(d => new OrderDetailDTO()
                    {
                        Thumbnail = d.Food.Thumbnail,
                        Name = d.Food.Name,
                        Price = d.Price,
                        Serve = d.Food.ServeType.Name
                    })
                    .ToList();

                return Ok(new OrderDTO()
                {
                    Id = details.Id,
                    InvoiceNumber = details.InvoiceNumber,
                    OrderDate = details.OrderDate,
                    EventName = details.EventName,
                    DeliveryAddress = details.DeliveryAddress,
                    DeliveryDate = details.DeliveryDate,
                    DeliveryPerson = details.DeliveryPerson,
                    DeliveryPhone = details.DeliveryPhone,
                    AdditionalRequirement = details.AdditionalRequirement,
                    TotalAmount = details.TotalAmount,
                    PrePaid = details.PrePaid,
                    StatusId = details.StatusId,
                    Status = details.Status.Name,
                    Foods = foods
                });
            }
            catch (Exception e)
            {
                return BadRequest("Get order details error.");
            }
        }
        
        [HttpPost]
        [Route("create")]
        public IActionResult CreateOrder(OrderModel creOrderModel)
        {
            if (ModelState.IsValid)
            {
                string lastBillNum = _dbContext.Orders
                    .OrderByDescending(order => order.Id)
                    .Select(order => order.InvoiceNumber)
                    .FirstOrDefault();

                string nextBillNum;
                if (string.IsNullOrEmpty(lastBillNum))
                {
                    nextBillNum = "INV0000001";
                }
                else
                {
                    string numberPart = lastBillNum.Substring(3);
                        
                    int lastNumber = int.Parse(numberPart);
                        
                    nextBillNum = "INV" + (++lastNumber).ToString("D7");
                }

                User user = _dbContext.Users.Find(creOrderModel.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                Restaurant restaurant = _dbContext.Restaurants.Find(creOrderModel.RestaurantId);
                if (restaurant == null)
                {
                    return NotFound("Restaurant not found.");
                }

                Order newOrder = new Order()
                {
                    UserId = user.Id,
                    RestaurantId = restaurant.Id,
                    InvoiceNumber = nextBillNum,
                    OrderDate = DateTime.Now,
                    EventName = creOrderModel.EventName,
                    DeliveryAddress = creOrderModel.DeliveryAddress,
                    DeliveryDate = creOrderModel.DeliveryDate,
                    DeliveryPerson = creOrderModel.DeliveryPerson,
                    DeliveryPhone = creOrderModel.DeliveryPhone,
                    AdditionalRequirement = creOrderModel.AdditionalRequirement,
                    UnitPrice = creOrderModel.UnitPrice,
                    Quantity = creOrderModel.Quantity,
                    ComboDiscount = creOrderModel.ComboDiscount,
                    TotalAmount = creOrderModel.TotalAmount,
                    PrePaid = creOrderModel.PrePaid,
                    StatusId = creOrderModel.StatusId
                };

                _dbContext.Orders.Add(newOrder);
                _dbContext.SaveChanges();

                foreach (var detail in creOrderModel.Details)
                {
                    OrderDetail newOrderDetail = new OrderDetail()
                    {
                        OrderId = newOrder.Id,
                        FoodId = detail.FoodId,
                        Price = detail.Price
                    };
                    _dbContext.OrderDetails.Add(newOrderDetail);
                }

                _dbContext.SaveChanges();

                return Created("Order created", new OrderDTO()
                {
                    InvoiceNumber = newOrder.InvoiceNumber
                });
            }

            return BadRequest("Create order error.");
        }

        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateOrderStatus(int id, StatusModel updateStatus)
        {
            if (ModelState.IsValid)
            {
                Order updateOrder = _dbContext.Orders.Find(id);
                if (updateOrder == null)
                {
                    return NotFound("Order not found.");
                }

                Status status = _dbContext.Status.Find(updateStatus.StatusId);
                if (status == null)
                {
                    return NotFound("Status not found.");
                }

                updateOrder.StatusId = status.Id;
                _dbContext.SaveChanges();
                return Ok("Status updated.");
            }

            return BadRequest("Update status error.");
        }
        
    }
}

