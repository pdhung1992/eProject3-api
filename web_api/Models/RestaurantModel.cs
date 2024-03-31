namespace web_api.Models;

public class RestaurantModel
{
    public string Name { get; set; }
    
    public string Address { get; set; }
    
    public string Description { get; set; }
    
    public string DeliveryHours { get; set; }
    
    public string MinimumDelivery { get; set; }
    
    public double PrePaidRate { get; set; }
    
    public IFormFile? Thumbnail { get; set; }
    
    public IFormFile? Banner { get; set; }
    
    public int DistrictId { get; set; }
    
    public int CatId { get; set; }
}