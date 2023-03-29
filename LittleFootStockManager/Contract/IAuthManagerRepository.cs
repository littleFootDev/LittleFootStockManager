using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.User;
using Microsoft.AspNetCore.Identity;

namespace LittleFootStockManager.Contract
{
    public interface IAuthManagerRepository
    {
        Task<IEnumerable<IdentityError>> Register(UserDto userDto);
        Task<AuthReponseDto> Login(LoginDto loginDto);
        Task Logout();
        Task<EmailConfirmationDto> ConfirmEmail(string email, string code);

    }
}
