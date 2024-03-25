using System.ComponentModel.DataAnnotations;

namespace web_api.Models;

public class ChangePwdModel
{
        [Required(ErrorMessage = "Current password is required.")]
        public string currentPassword { get; set; }
    
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string newPassword { get; set; }
}