using LittleFootStockManager.Contract;
using LittleFootStockManager.Data;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Tls;
using System.Security.Claims;

namespace LittleFootStockManager.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly LittleFootStockManagerDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Users> _userManager;

        public AdminRepository(LittleFootStockManagerDbContext context, RoleManager<IdentityRole> roleManager, UserManager<Users> userManager)
        {
            this._context = context;
            this._roleManager = roleManager;
            this._userManager = userManager;
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
        public async Task<RoleDto> CreateRole(RoleDto roleDto)
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
                return new RoleDto
                {
                    Name = roleDto.Name,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating the role.{ex.Message}");
            }
        }
        public async Task<RoleInfoDto> DeleteRole(string roleId)
        {
            try
            {
                var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId);
                if (role is not null)
                {
                    if (role.Name != "Admin" || role.Name != "User")
                    {
                        var result = await ConfirmeDeleteRole(role);
                        return new RoleInfoDto
                        {
                            Id = roleId,
                            Name = role.Name
                        };
                    }
                }
                return new RoleInfoDto
                {
                    Name= role.Name,
                    Id= roleId,
                    ErrorMessage = "The role could not be deleted!"
                };
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
        public async Task<RoleInfoDto> UpdateRole(RoleInfoDto updateRole)
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
                return new RoleInfoDto
                {
                    Name = updateRole.Name,
                    Id = updateRole.Id,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the role.{ex.Message}");
            }
        }

        #endregion
        #region User
        public async Task<bool> AddClaimUser(string userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new Exception($"User with id {userId} not found");

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.AddClaimAsync(user, claim);
                if (!result.Succeeded)
                    throw new Exception($"Error adding claim {claimType}:{claimValue} to user with id {userId}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding claim {claimType}:{claimValue} to user with id {userId}", ex);
            }
        }

        public async Task<bool> AddRoleUser(string userId, string roleName)
        {   try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new Exception($"User with id {userId} not found");

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role == null)
                    throw new Exception($"Role with name {roleName} not found");

                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                    throw new Exception($"Error adding role {roleName} to user with id {userId}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding role {roleName} to user with id {userId} : {ex.Message} ");
            }
        }
        public async Task<UpdateUserDto> UpdateUser(Users updateUserDto)
        {
            try
            {
                var newUser = new UpdateUserDto
                {
                    Id = updateUserDto.Id,
                    FirstName = updateUserDto.FirstName,
                    LastName = updateUserDto.LastName,
                };
                newUser.Roles = await _context.Roles.Select(r => r.Name).ToArrayAsync();
                newUser.UserRoles = await _userManager.GetRolesAsync(updateUserDto);
                newUser.UserClaims = await _userManager.GetClaimsAsync(updateUserDto);

                return newUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user with id {updateUserDto.Id}", ex);
            }
        }
        public async Task<AdminUserIndexDto> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.AsNoTracking().ToArrayAsync();
                return new AdminUserIndexDto
                {
                    Users = users,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all users : {ex.Message}");
            }
        }

        public async Task<bool> GetUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    throw new Exception($"User with id {userId} not found");

                var result = await UpdateUser(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user with id {userId} : {ex.Message}");
            }
        }

        public async Task<bool> RemoveClaimUser(string userId, string claimType)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user is null)
                    throw new Exception($"User with id {userId} not found");

                var claims = await _userManager.GetClaimsAsync(user);
                if (claims == null)
                    throw new Exception($"Claim with type {claimType} not found for user with id {userId}");

                if(claims.Count() > 0)
                {
                    var claim = claims.FirstOrDefault(c => c.Type == claimType);
                    var result = await _userManager.RemoveClaimAsync(user, claim);
                    if (!result.Succeeded)
                        throw new Exception($"Error removing claim {claimType} from user with id {userId}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing claim {claimType} from user with id {userId}", ex);
            }
        }

        public async Task<bool> RemoveRoleUser(string userId, string roleName)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new Exception($"User with id {userId} not found");

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role == null)
                    throw new Exception($"Role with name {roleName} not found");

                var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (!result.Succeeded)
                    throw new Exception($"Error removing role {roleName} from user with id {userId}");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing role {roleName} from user with id {userId} {ex.Message}");
            }
        }
        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new Exception($"User with id {userId} not found");

                var result = await ConfirmeDeleteUser(user);
                if(result == true)
                {
                    return true;
                }
                throw new Exception($"Error deleting user with id {userId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user with id {userId}: {ex.Message}");
            }
        }
        public async Task<bool> ConfirmeDeleteUser(Users user)
        {
            await _userManager.DeleteAsync(user);
            return true;
        }
        #endregion
    }
}
