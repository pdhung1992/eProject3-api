using System.ComponentModel.DataAnnotations;

namespace web_api.Models;

public class AdminModel
{
    
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Full name is required.")]
    [MinLength(6, ErrorMessage = "Name must be at least 6 characters.")]
    public string Fullname { get; set; }
    
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Incorrect email.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Telephone number is required. ")]
    public string Telephone { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Role ID is required.")]
    public int RoleId { get; set; }
    
    public string? ResName { get; set; }
    
    public string? ResAddress { get; set; }
    
    public string? ResDescription { get; set; }
    
    public string? ResDeliveryHours { get; set; }
    
    public string? ResMinimumDelivery { get; set; }
    
    public IFormFile? ResThumbnail { get; set; }
    
    public IFormFile? ResBanner { get; set; }
    
    public int ResDistrictId { get; set; }
    
    public int ResCatId { get; set; }
    
}