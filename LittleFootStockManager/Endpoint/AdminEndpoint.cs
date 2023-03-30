using LittleFootStockManager.Contract;
using LittleFootStockManager.Dto.Admin;
using LittleFootStockManager.Repository;
using Microsoft.AspNetCore.Mvc;

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

            return builder;
        }

        private static async Task<IResult> GetAllRoles(
            [FromServices] IAdminRepository adminRepository
            )
        {
            var roles = await adminRepository.GetAllRoles();
            return Results.Ok(roles);
        }

        #region role
        private static async Task<IResult> CreateRole(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] RoleDto role
            )
        {
            var result = await adminRepository.CreateRole(role);
            if (result == true)
            {
                return Results.Ok(new { Messsage = "Le rôle à bien était créer" });
            }
            return Results.BadRequest(new { Messsage = "Une erreur c'est produite!" });
        }

        private static async Task<IResult> UpdateRole(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] RoleInfoDto role
            )
        {
            var updateRole = await adminRepository.UpdateRole(role);
            if (updateRole == true)
            {
                return Results.Ok(new { Messsage = "Le rôle à bien était modifié" });
            }
            return Results.BadRequest(new { Messsage = "Une erreur c'est produite!" });
        }

        private static async Task<IResult> DeleteRole(
            [FromServices] IAdminRepository adminRepository,
            [FromBody] string roleId
            )
        {
            var result = await adminRepository.DeleteRole(roleId);
            if (result == true)
            {
                return Results.Ok(new { Messsage = "Le rôle à bien était Supprimé" });
            }
            return Results.Ok(new { Messsage = "Une erreur c'est produite!" });
        }
        #endregion
    }
}
