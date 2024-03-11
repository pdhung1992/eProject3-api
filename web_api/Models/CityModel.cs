using System.Net;

namespace web_api.Models;

public class CityModel
{
    public string Name { get; set; }
    
    public IFormFile Thumbnail { get; set; }
}