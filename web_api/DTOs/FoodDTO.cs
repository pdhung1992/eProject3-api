namespace web_api.DTOs;

public class FoodDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int ResId { get; set; }
    
    public int TypeId { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    public double Price { get; set; }
    
    public int ServeID { get; set; }
    
    public string Serve { get; set; }
    
    public string Thumbnail { get; set; }
    
    public int FoodTagId { get; set; }
    
    public string Tag { get; set; }
}