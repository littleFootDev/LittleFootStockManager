using LittleFootStockManager.Contract;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.Admin;
using LittleFootStockManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LittleFootStockManager.Endpoint
{
    public static class AdminEndpoint
    {
        public static IServiceCollection AddAdminService(this IServiceCollection services)
        {
            services.AddScoped<IAdminRepository, AdminRepository>();
            return services;
        }

        public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder builder)
        {
            //roles
            builder.MapGet("roles", GetAllRoles)
                .WithTags("AdminManagement")
                .Produces<AdminRolesIndexDto>(200, "application/json")
                .Produces(400);
            builder.MapPost("roles", CreateRole)
                .WithTags("AdminManagement")
                .Produces<RoleInfoDto>(201, "application/json")
                .Produces(400);
            builder.MapPut("roles/", UpdateRole)
                .WithTags("AdminManagement")
                .Produces(204)
                .Produces(400)
                .Produces(404);
            builder.MapDelete("roles", DeleteRole)
                .WithTags("AdminManagement")
                .Produces(204)
                .Produces(400);

            //User
            builder.MapGet("/users", GetAllUsers)
                .WithTags("AdminManagement")
                .Produces<AdminUserIndexDto>(200, "application/json")
                .Produces(400);
            builder.MapPut("users/{id}", UpdateUser)
                .WithTags("AdminManagement")
                .Produces(204)
                .Produces(400)
                .Produces(404);
            builder.MapDelete("users/{id}", DeleteUser)
                .WithTags("AdminManagement")
                .Produces(204)
                .Produces(400);
            builder.MapDelete("users/roles/{id}", DeleteRoleUser)
                .WithTags("AdminManagement")
                .Produces(204)
                .Produces(400);
            builder.MapPut("users/roles/{id}", UpdateRoleUser)
              .WithTags("AdminManagement")
              .Produces(204)
              .Produces(400)
              .Produces(404);
            builder.MapDelete("users/claims/{id}", DeleteClaimsUser)
                .WithTags("AdminManagement")
                .Produces(204)
                .Produces(400);
            builder.MapPut("users/claims/{id}", AddClaimsUser)
              .WithTags("AdminManagement")
              .Produces(204)
              .Produces(400)
              .Produces(404);

            return builder;
        }

        #region role
        private static async Task<IResult> GetAllRoles(
            [FromServices] IAdminRepository adminRepository
            )
        {
            var roles = await adminRepository.GetAllRoles();
            return Results.Ok(roles);
        }
        private static async Task<IResult> CreateRole(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] RoleDto role
            )
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(role, new ValidationContext(role), validationResults))
            {
                return Results.BadRequest(validationResults);
            }

            var createRole = await adminRepository.CreateRole(role);
            return Results.Ok(createRole);
        }

        private static async Task<IResult> UpdateRole(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] RoleInfoDto role
            )
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(role, new ValidationContext(role), validationResults))
            {
                return Results.BadRequest(validationResults);
            }

            var updateRole = await adminRepository.UpdateRole(role);
            return Results.Ok(updateRole);

        }

        private static async Task<IResult> DeleteRole(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] string roleId
            )
        {
            var result = await adminRepository.DeleteRole(roleId);
            if (result.ErrorMessage != null)
            {
                return Results.BadRequest(result.ErrorMessage);
            }

            return Results.NoContent();
        }
        #endregion
        #region user
        private static async Task<IResult> AddClaimsUser(
             [FromServices] IAdminRepository adminRepository,
             string claimType,
             string claimValue,
             [FromQuery] string userId
            )
        {
            var result = await adminRepository.AddClaimUser(claimType, claimValue, userId);
            if (result == true) 
            {
                return Results.Ok("The claim to property was added");
            }
            return Results.BadRequest("An error occured");
        }

        private static async Task<IResult> DeleteClaimsUser(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] string claimType,
            [FromQuery] string userId
            )
        {
            var result = await adminRepository.RemoveClaimUser(claimType, userId);
            if (result == true)
            {
                return Results.Ok("the claim to property was removed");
            }
            return Results.BadRequest("An error occured");
        }

        private static async Task<IResult> UpdateRoleUser(
             [FromServices] IAdminRepository adminRepository,
             [FromBody] string roleName,
             [FromQuery] string userId
            )
        {
            var result = await adminRepository.AddRoleUser(roleName, userId);
            if (result == true)
            {
                return Results.Ok("The role to good was added");
            }
            return Results.BadRequest("An error occured");
        }

        private static async Task<IResult> DeleteRoleUser(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] string roleName,
            [FromQuery] string userId
            )
        {
            var result = await adminRepository.RemoveRoleUser(roleName, userId);
            if (result == true)
            {
                return Results.Ok("The role to good was removed");
            }
            return Results.BadRequest("An error occured");
        }

        private static async Task<IResult> DeleteUser(
            [FromServices] IAdminRepository adminRepository,
            [FromQuery] string userId
            )
        {
            var result = await adminRepository.DeleteUser(userId);
            if (result == true)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }

        private static async Task<IResult> UpdateUser(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] Users user
            )
        {
            var result = await adminRepository.UpdateUser(user);
            if (result is not null)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }

        private static async Task<IResult> GetAllUsers(
            [FromServices] IAdminRepository adminRepository
            )
        {
            var result = await adminRepository.GetAllUsers();
            if (result is not null)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }
        #endregion
    }
}
