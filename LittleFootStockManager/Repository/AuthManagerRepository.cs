using AutoMapper;
using LittleFootStockManager.Configuration;
using LittleFootStockManager.Contract;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LittleFootStockManager.Repository
{
    public class AuthManagerRepository : IAuthManagerRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<AuthManagerRepository> _logger;
        private readonly SignInManager<Users> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IOptions<AdminOptions> _adminOption;

        public AuthManagerRepository(IMapper mapper, UserManager<Users> userManager, ILogger<AuthManagerRepository> logger, SignInManager<Users> signInManager, 
            IConfiguration configuration, IOptions<AdminOptions> adminOption
            )
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._logger = logger;
            this._signInManager = signInManager;
            this._configuration = configuration;
            this._adminOption = adminOption;
        }

        public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
        {
            var user = _mapper.Map<Users>(userDto);
            user.UserName = userDto.Email;
            
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if(result.Succeeded)
            {
                if (userDto.Email == _adminOption.Value.AdminEmail)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return result.Errors;
                }
            }

            return result.Errors;
        }
        public async Task<AuthReponseDto> Login(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                var token = await GenerateAccessTokenAsync(user);
                return new AuthReponseDto
                {
                    IsAuthenticated = true,
                    UserId = user.Id,
                    Token = token,
                };
            }

            return new AuthReponseDto
            {
                IsAuthenticated = false,
                ErrorMessage = "Invalid username or password"
            };
                
        }

        private async Task<string> GenerateAccessTokenAsync(Users user)
        {
            RSA _rsa = RSA.Create();
            _rsa.ImportRSAPrivateKey(File.ReadAllBytes("key.bin"), out var _);

            var securityKey = new RsaSecurityKey(_rsa);
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Email),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
            }
            .Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credential
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task Logout()
        {
           await _signInManager.SignOutAsync();
        }
    }
}
