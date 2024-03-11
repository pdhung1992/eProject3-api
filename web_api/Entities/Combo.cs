using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class Combo
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int ResId { get; set; }
    [ForeignKey("ResId")]
    public Restaurant Restaurant { get; set; }
}