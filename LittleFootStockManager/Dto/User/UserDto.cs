using System.ComponentModel.DataAnnotations;

namespace LittleFootStockManager.Dto.User
{
    public class UserDto : LoginDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
