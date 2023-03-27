using AutoMapper;
using LittleFootStockManager.Contract;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.User;
using Microsoft.AspNetCore.Identity;

namespace LittleFootStockManager.Repository
{
    public class AuthManagerRepository : IAuthManagerRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<AuthManagerRepository> _logger;
        private readonly SignInManager<Users> _signInManager;

        public AuthManagerRepository(IMapper mapper, UserManager<Users> userManager, ILogger<AuthManagerRepository> logger, SignInManager<Users> signInManager)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._logger = logger;
            this._signInManager = signInManager;
        }

        public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
        {
            var user = _mapper.Map<Users>(userDto);
            user.UserName = userDto.Email;
            
            var result = await _userManager.CreateAsync(user, userDto.Password);
            
            return result.Errors;
        }
        public async Task<AuthReponseDto> Login(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                return new AuthReponseDto
                {
                    IsAuthenticated = true,
                    UserId = user.Id
                };
            }

            return new AuthReponseDto
            {
                IsAuthenticated = false,
                ErrorMessage = "Invalid username or password"
            };
                
        }

        public async Task Logout()
        {
           await _signInManager.SignOutAsync();
        }
    }
}
