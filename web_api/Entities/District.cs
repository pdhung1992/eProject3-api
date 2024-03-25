using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class District
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int CityId { get; set; }
    [ForeignKey("CityId")]
    public City City { get; set; }
}