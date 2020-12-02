using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee_App.Token
{
    public class CoffeeToken
    {
        static IConfiguration _config;

        public CoffeeToken(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken()
        {
            string key = _config.GetValue<string>("Jwt:SecretKey");
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            // create token
            var token = new JwtSecurityToken(
                issuer: _config.GetValue<string>("Jwt:ValidIssuer"),
                audience: _config.GetValue<string>("Jwt:ValidAudience"),
                expires: DateTime.Now.AddHours(12),
                signingCredentials: signingCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       /* public static string GetEquipmentIdFromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenObj = handler.ReadJwtToken(token);
            var username = tokenObj.Claims.First(claim => claim.Type == "EquipmentId").Value;
            return username;
        }

        public static string GetFeedbackIdFromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenObj = handler.ReadJwtToken(token);
            var username = tokenObj.Claims.First(claim => claim.Type == "FeedbackId").Value;
            return username;
        }*/
    }
}
