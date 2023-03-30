using LittleFootStockManager.Dto.Admin;
using Microsoft.AspNetCore.Identity;

namespace LittleFootStockManager.Contract
{
    public interface IAdminRepository
    {
        //Roles
        public Task<AdminRolesIndexDto> GetAllRoles();
        public Task<bool> CreateRole(RoleDto role);
        public Task<bool> UpdateRole(RoleInfoDto updateRole);
        public Task<bool> DeleteRole(string roleId);
        public Task<bool> ConfirmeDeleteRole(IdentityRole role);
    }
}
