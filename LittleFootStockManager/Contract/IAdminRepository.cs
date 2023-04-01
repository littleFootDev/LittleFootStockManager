using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.Admin;
using Microsoft.AspNetCore.Identity;

namespace LittleFootStockManager.Contract
{
    public interface IAdminRepository
    {
        //Roles
        public Task<AdminRolesIndexDto> GetAllRoles();
        public Task<RoleDto> CreateRole(RoleDto role);
        public Task<RoleInfoDto> UpdateRole(RoleInfoDto updateRole);
        public Task<RoleInfoDto> DeleteRole(string roleId);
        public Task<bool> ConfirmeDeleteRole(IdentityRole role);

        //User
        public Task<AdminUserIndexDto> GetAllUsers();
        public Task<bool> DeleteUser(string userId);
        public Task<bool> ConfirmeDeleteUser(Users user);
        public Task<bool> RemoveRoleUser(string userId, string roleName);
        public Task<bool> AddRoleUser(string userId, string roleName);
        public Task<bool> AddClaimUser(string userId, string claimType, string claimValue);
        public Task<bool> RemoveClaimUser(string userId, string claimType);
        public Task<bool> GetUser(string userId);
        public Task<UpdateUserDto> UpdateUser(Users updateUserDto);
    }
}
