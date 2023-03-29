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
using System.Web;

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
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContext;

        public AuthManagerRepository(IMapper mapper, UserManager<Users> userManager, ILogger<AuthManagerRepository> logger, SignInManager<Users> signInManager, 
            IConfiguration configuration, IOptions<AdminOptions> adminOption, IEmailSender emailSender, IHttpContextAccessor httpContext
            )
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._logger = logger;
            this._signInManager = signInManager;
            this._configuration = configuration;
            this._adminOption = adminOption;
            this._emailSender = emailSender;
            this._httpContext = httpContext;
        }

        public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
        {
            var user = _mapper.Map<Users>(userDto);
            user.UserName = userDto.Email;
            user.EmailConfirmed = userDto.Email == _adminOption.Value.AdminEmail;
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if(result.Succeeded)
            {
                if (userDto.Email == _adminOption.Value.AdminEmail)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return result.Errors;
                }
                else
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var scheme= _httpContext.HttpContext.Request.Scheme;
                    var host = _httpContext.HttpContext.Request.Host.ToUriComponent();
                    var url = $"/Confirm-email?email={userDto.Email}&code={HttpUtility.UrlEncode(code)}";
                    var confirmUrl = $"{scheme}://{host}{url}";

                    await _emailSender.SendEmail(user.Email, "Confirmez votre email", "Copier-coller ce lien et accédez-y pour valider votre email : " + confirmUrl);
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

        public async Task<EmailConfirmationDto> ConfirmEmail(string email, string code)
        {
            code = HttpUtility.UrlDecode(code);
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
            {
                return new EmailConfirmationDto
                {
                    IsSucccesFull = false,
                    ErrrorMessage = $"User with email {email} not found."
                };
            }
            if(user.EmailConfirmed == false)
            {
                var confirmationResult = await _userManager.ConfirmEmailAsync(user, code);
                if(confirmationResult.Succeeded) 
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    
                    return new EmailConfirmationDto
                    {
                        IsSucccesFull = true,
                        UserId = user.Id,
                        Email = email,
                        ConfirmationDate = DateTime.UtcNow
                    };
                }
                return new EmailConfirmationDto
                {
                    IsSucccesFull = false,
                    ErrrorMessage = $"Unable to confirm email for user {user.Id}"
                };
            }
            else
            {
                return new EmailConfirmationDto
                {
                    IsSucccesFull = false,
                    ErrrorMessage = $"Unable to confirm email for user {user.Id}"
                };
            }
        }
    }
}
