using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class ComboDetail
{
    public int Id { get; set; }
    
    public int ComboId { get; set; }
    [ForeignKey("ComboId")]
    public Combo Combo { get; set; }
    
    public int FoodId { get; set; }
    [ForeignKey("FoodId")]
    public Food Food { get; set; }
}