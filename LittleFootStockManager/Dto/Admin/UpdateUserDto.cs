using System.Security.Claims;

namespace LittleFootStockManager.Dto.Admin
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<Claim> UserClaims { get; set; }
    }
}
