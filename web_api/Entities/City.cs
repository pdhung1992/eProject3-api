using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class City
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Thumbnail { get; set; }
}