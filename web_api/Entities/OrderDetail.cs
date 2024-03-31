using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class OrderDetail
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
    
    public int FoodId { get; set; }
    [ForeignKey("FoodId")]
    public Food Food { get; set; }
    
    public double Price { get; set; }
}