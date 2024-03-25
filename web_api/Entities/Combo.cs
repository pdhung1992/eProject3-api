using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class Combo
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public double DiscountRate { get; set; }
    
    public int ResId { get; set; }
    [ForeignKey("ResId")]
    public Restaurant Restaurant { get; set; }
    
    public int ServeId { get; set; }
    [ForeignKey("ServeId")]
    public ServeType ServeType { get; set; }
    
    public string Thumbnail { get; set; }
    
    public int ComboTagId { get; set; }
    [ForeignKey("ComboTagId")]
    public FoodTag FoodTag { get; set; }
}