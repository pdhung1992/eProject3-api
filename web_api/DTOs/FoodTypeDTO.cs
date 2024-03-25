namespace web_api.DTOs;

public class FoodTypeDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int SortOrder { get; set; }
}