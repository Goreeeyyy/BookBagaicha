using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BookBagaicha.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BookBagaicha.Services
{
    public class JWTService
    {

        private readonly JWTTokenInfo jwtTokenInfo;
        private readonly UserManager<User> _userManager; // ADDED

        
        public JWTService(IOptions<JWTTokenInfo> jwtTokenInfo, UserManager<User> userManager)
        {
            this.jwtTokenInfo = jwtTokenInfo.Value;
            _userManager = userManager; // ADDED
        }

        
        public string GenerateToken(User user)
        {
            string key = jwtTokenInfo.Key;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // ADD CLAIMS - USER ID AND EMAIL
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // ADD USER ROLES AS CLAIMS
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Set the claims here
                Expires = DateTime.UtcNow.AddMinutes(jwtTokenInfo.ExpiresInMinutes),
                Issuer = jwtTokenInfo.Issuer,
                Audience = jwtTokenInfo.Audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor); // Create token from the descriptor

            return tokenHandler.WriteToken(token);
        }
    }
}
