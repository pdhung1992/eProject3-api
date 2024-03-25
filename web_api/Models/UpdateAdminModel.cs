using System.ComponentModel.DataAnnotations;

namespace web_api.Models;

public class UpdateAdminModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [MinLength(6, ErrorMessage = "Name must be at least 6 characters.")]
    public string Fullname { get; set; }
    
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Incorrect email.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Telephone number is required. ")]
    public string Telephone { get; set; }
}