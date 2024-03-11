using System.ComponentModel.DataAnnotations;

namespace web_api.Models;

public class UserModel
{
    [Required(ErrorMessage = "Full name is required. ")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Full name must be at least 6 characters.")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Email is required. ")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Incorrect email.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Telephone number is required. ")]
    public string Telephone { get; set; }
    
    [Required(ErrorMessage = "Password is required. ")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }
}