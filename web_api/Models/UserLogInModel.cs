using System.ComponentModel.DataAnnotations;

namespace web_api.Models;

public class UserLogInModel
{
    [Required(ErrorMessage = "Email is required. ")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Incorrect email format.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required. ")]
    public string Password { get; set; }
}