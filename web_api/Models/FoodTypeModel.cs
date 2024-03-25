namespace web_api.Models;

public class FoodTypeModel
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public int SortOrder { get; set; }
}