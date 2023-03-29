using System.ComponentModel.DataAnnotations;

namespace LittleFootStockManager.Dto.User
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
