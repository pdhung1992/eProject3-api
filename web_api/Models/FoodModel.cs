using web_api.Entities;

namespace web_api.Models;

public class FoodModel
{
    public string Name { get; set; }
    
    public int TypeId { get; set; }
    
    public string Description { get; set; }
    
    public double Price { get; set; }
    
    public int ServeId { get; set; }
    
    public IFormFile? Thumbnail { get; set; }
    
    public int FoodTagId { get; set; }
}