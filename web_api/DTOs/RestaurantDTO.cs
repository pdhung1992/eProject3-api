namespace web_api.DTOs;

public class RestaurantDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string Address { get; set; }
    
    public DateTime JoinDate { get; set; }
    
    public string Description { get; set; }
    
    public string DeliveryHours { get; set; }
    
    public string MinimumDelivery { get; set; }
    
    public double PrePaidRate { get; set; }
    
    public string Thumbnail { get; set; }
    
    public string Banner { get; set; }
    
    public int DistrictId { get; set; }
    
    public string District { get; set; }
    
    public int CityId { get; set; }
    
    public string City { get; set; }
    
    public int CatId { get; set; }
    
    public string Category { get; set; }
    
    public int? AdminId { get; set; }
}