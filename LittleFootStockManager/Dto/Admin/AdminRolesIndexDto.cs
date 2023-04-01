using Microsoft.AspNetCore.Identity;

namespace LittleFootStockManager.Dto.Admin
{
    public class AdminRolesIndexDto
    {
        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
