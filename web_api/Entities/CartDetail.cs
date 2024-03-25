using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class CartDetail
{
    public int Id { get; set; }
    
    public int CartId { get; set; }
    [ForeignKey("CartId")]
    public Order Order { get; set; }
    
    public int FoodId { get; set; }
    [ForeignKey("FoodId")]
    public Food Food { get; set; }
}