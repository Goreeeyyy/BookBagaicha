using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Models;

namespace StudentManagement.Services
{
    public class JWTService
    {

        private readonly JWTTokenInfo jwtTokenInfo; 

        public JWTService(IOptions<JWTTokenInfo> jwtTokenInfo)
        {
            this.jwtTokenInfo = jwtTokenInfo.Value;
        }


        public string GenerateToken()

        {
            string key = jwtTokenInfo.Key; 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); 

            // signing credentials object needs security key and an algorithm
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 



            // this is the object which stores token values. 
            var tokenObj = new JwtSecurityToken(

                issuer: jwtTokenInfo.Audience,

                audience: jwtTokenInfo.Issuer,

                expires: DateTime.UtcNow.AddMinutes(jwtTokenInfo.ExpiresInMinutes),

                // we need to pass object of signing credentials 
                signingCredentials: signingCredentials
               
                );


            //Token Generation 
            var token = new JwtSecurityTokenHandler().WriteToken(tokenObj);

            return token;
        
        }
    }
}
