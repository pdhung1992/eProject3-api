using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class Permission
{
    public int Id { get; set; }
    
    public int SortOrder { get; set; }
    
    public string Name { get; set; }
    
    public string Prefix { get; set; }
    
    public string FaIcon { get; set; }
    
    public int RoleId { get; set; }
    
    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}