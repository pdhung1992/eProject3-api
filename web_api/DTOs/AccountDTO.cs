namespace web_api.DTOs;

public class AccountDTO
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Fullname { get; set; }
    
    public string Email { get; set; }
    
    public string Telephone { get; set; }
    
    public int RoleId { get; set; }
    
    public string Role { get; set; }
}