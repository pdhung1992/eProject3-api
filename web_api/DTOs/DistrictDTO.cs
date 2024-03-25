namespace web_api.DTOs;

public class DistrictDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public int CityId { get; set; }
    
    public string CityName { get; set; }
}