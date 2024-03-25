namespace web_api.DTOs;

public class ComboDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public double DiscountRate { get; set; }
    
    public int ResId { get; set; }
    
    public int ServeId { get; set; }
    
    public string Serve { get; set; }
    
    public string Thumbnail { get; set; }
    
    public int ComboTagId { get; set; }
    
    public string Tag { get; set; }

    public List<FoodDTO> Foods { get; set; }
}