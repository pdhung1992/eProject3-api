namespace web_api.Models;

public class ComboModel
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public double DiscountRate { get; set; }
    
    public int ServeId { get; set; }
    
    public IFormFile? Thumbnail { get; set; }
    
    public int ComboTagId { get; set; }
    
    public List<int> foods { get; set; }
}