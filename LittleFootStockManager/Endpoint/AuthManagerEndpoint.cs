using LittleFootStockManager.Contract;
using LittleFootStockManager.Dto.User;
using LittleFootStockManager.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Serilog;

namespace LittleFootStockManager.Endpoint
{
    public static class AuthManagerEndpoint
    {
        public static IServiceCollection AddAuthManagerService(this IServiceCollection services)
        {
            services.AddScoped<IAuthManagerRepository, AuthManagerRepository>();
            return services;
        }

        public static RouteGroupBuilder MapAuthManagerEndpoint(this RouteGroupBuilder builder)
        {
            builder.MapPost("/register", Register)
                .WithTags("AccountManagement")
                .Produces<UserDto>(201, "application/json")
                .Produces(400)
                .Produces(500);
            builder.MapPost("/login", Login)
                .WithTags("AccountManagement")
                .Produces<LoginDto>(200, "application/json")
                .Produces(400)
                .Produces(500);
            builder.MapDelete("/logout", Logout)
                .WithTags("AccountManagement")
                .Produces(200)
                .Produces(400)
                .Produces(500);
            builder.MapGet("/congirm-email", ConfirmEmail)
                .WithTags("AccountManagement")
                .Produces(200)
                .Produces(400)
                .Produces(500);
            builder.MapPost("/forgot-password", ForgotPassword)
                .WithTags("AccountManagement")
                .Produces(200)
                .Produces(400)
                .Produces(500);
            builder.MapPost("/Reset-password", ResetPassword)
                .WithTags("AccountManagement")
                .Produces(200)
                .Produces(400)
                .Produces(500);
            return builder;
        }

        private static async Task<IResult> Register(
            [FromServices] IAuthManagerRepository authManager,
            [FromBody] UserDto userDto
            )
        {
            var errors = await authManager.Register(userDto);
            if (errors.Any())
            {
                return Results.BadRequest(errors);
            }
            return Results.Ok("User created");
        }

        private static async Task<IResult> Login(
            [FromServices] IAuthManagerRepository authManager,
            [FromBody] LoginDto loginDto
            )
        {
            var response = await authManager.Login(loginDto);
            
            if(!response.IsAuthenticated)
            {
                return Results.BadRequest(response);
            }
            return Results.Ok(response);
        }

        private static async Task<IResult> Logout(
            [FromServices] IAuthManagerRepository authManager
            )
        {
            try
            {
                await authManager.Logout();
                return Results.Ok("you are deconnected");
            }
            catch (InvalidOperationException e) 
            {
                return Results.BadRequest(e.Message);
            }
        }
        private static async Task<IResult> ConfirmEmail(
            [FromServices] IAuthManagerRepository authManager,
            [FromQuery] string email,
            [FromQuery] string code
            )
        {
            var confirmationDto = await authManager.ConfirmEmail(email, code);

            if(!confirmationDto.IsSucccesFull)
            {
                return Results.BadRequest(confirmationDto);
            }
            return Results.Ok(confirmationDto);
        }
        private static async Task<IResult> ForgotPassword(
            [FromServices] IAuthManagerRepository authManager,
            ForgotPasswordDto forgotPassword
            )
        {
            var result = await authManager.ForgotPassword(forgotPassword);
            if(result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }
        private static async Task<IResult> ResetPassword(
            [FromServices] IAuthManagerRepository authManager,
            ResetPasswordDto resetPassword
            )
        {
            var result = authManager.ResetPassword(resetPassword);
            if(result.IsCompleted)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }
    }

}

