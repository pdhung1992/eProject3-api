using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class Food
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int ResId { get; set; }
    [ForeignKey("ResId")]
    public Restaurant Restaurant { get; set; }
    
    public int TypeId { get; set; }
    [ForeignKey("TypeId")]
    public FoodType FoodType { get; set; }
    
    public string Description { get; set; }
    
    public double Price { get; set; }
    
    public string Thumbnail { get; set; }
}