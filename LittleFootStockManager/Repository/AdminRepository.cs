using LittleFootStockManager.Contract;
using LittleFootStockManager.Data;
using LittleFootStockManager.Dto.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LittleFootStockManager.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly LittleFootStockManagerDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(LittleFootStockManagerDbContext context, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._roleManager = roleManager;
        }
        #region role
        public async Task<bool> ConfirmeDeleteRole(IdentityRole role)
        {
            try
            {
                if (role is not null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (!result.Succeeded)
                    {
                        var error = result.Errors.FirstOrDefault()?.Description;
                        throw new Exception($"Failed to delete role: {error}");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while delete the role.{ex.Message}");
            }

        }

        public async Task<bool> CreateRole(RoleDto roleDto)
        {
            try
            {
                var role = new IdentityRole
                {
                    Name = roleDto.Name
                };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    var error = result.Errors.FirstOrDefault()?.Description;
                    throw new Exception($"Failed to create role: {error}");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating the role.{ex.Message}");
            }
        }

        public async Task<bool> DeleteRole(string roleId)
        {
            try
            {
                var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId);
                if (role is not null)
                {
                    if (role.Name != "Admin" || role.Name != "User")
                    {
                        var result = await ConfirmeDeleteRole(role);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the role.{ex.Message}");
            }
        }

        public async Task<AdminRolesIndexDto> GetAllRoles()
        {
            try
            {
                var role = await _context.Roles.AsNoTracking().ToArrayAsync();
                return new AdminRolesIndexDto
                {
                    Roles = role,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the roles. {ex.Message}");
            }
        }

        public async Task<bool> UpdateRole(RoleInfoDto updateRole)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(updateRole.Id);
                if (role == null)
                {
                    throw new Exception("Role not found.");
                }
                role.Name = updateRole.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    var error = result.Errors.FirstOrDefault()?.Description;
                    throw new Exception($"Failed to update role: {error}");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the role.{ex.Message}");
            }
        }
        #endregion
    }
}
