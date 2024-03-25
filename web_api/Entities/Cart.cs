using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class Cart
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public int Quantity { get; set; }
    
}