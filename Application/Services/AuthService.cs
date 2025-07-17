using Application.Interfaces;
using Domain.DTOs.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = configuration;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request)
        {
            var user = new ApplicationUser { Email = request.Email, UserName = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) return (false, result.Errors.Select(e => e.Description));

            if (!string.IsNullOrEmpty(request.Role))
            {
                if (!await _roleManager.RoleExistsAsync(request.Role))
                    await _roleManager.CreateAsync(new IdentityRole(request.Role));
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            // Exemplo claim customizada:
            await _userManager.AddClaimAsync(user, new Claim("DataRegistro", DateTime.UtcNow.ToString("yyyy-MM-dd")));

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string Token, IEnumerable<string> Roles)> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
                return (false, null, null);

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(userClaims);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return (true, tokenStr, roles);
        }
    }
}
