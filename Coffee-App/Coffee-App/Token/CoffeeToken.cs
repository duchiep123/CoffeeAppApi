using Coffee_App.IRepositories;
using Coffee_App.Models;
using Coffee_App.RequestModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Coffee_App.Token
{
    public class CoffeeToken : ICoffeeToken
    {
        private IConfiguration _config;
        private IRefreshTokenRepository _refreshTokenRepository;
        private IJwtTokenRepository _jwtTokenRepository;


        public CoffeeToken(IConfiguration config, IRefreshTokenRepository refreshTokenRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _config = config;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenRepository = jwtTokenRepository;

        }

        public ResponseRegisterModel CreateToken(string userId)
        {

            string key = _config.GetValue<string>("Jwt:SecretKey");
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            // create token
            var issuer = _config.GetValue<string>("Jwt:ValidIssuer");
            var audience = _config.GetValue<string>("Jwt:ValidAudience");
            var exp = _config.GetValue<int>("Jwt:ExpiryTimeInMinutes");
            //Add claims 
            var claims = new List<Claim>();
            var jwtId = Guid.NewGuid().ToString();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jwtId)); // jwt id 
            claims.Add(new Claim("UserId", userId));
            //
            //Finally create a Token
            var createDateTime = DateTime.Now;
            var header = new JwtHeader(signingCredentials);

            var payLoad = new JwtPayload(issuer, audience, claims, null, expires: createDateTime.AddMinutes(exp), issuedAt: createDateTime);
            //
            var token = new JwtSecurityToken(header, payLoad);
            var handler = new JwtSecurityTokenHandler();

            // token to string
            var tokenString = handler.WriteToken(token);

            var jwt = new JwtToken()
            {
                Token = tokenString,
                CreateDate = createDateTime,
                ExpirationDate = createDateTime.AddMinutes(exp),
                Status = 1,
                UserId = userId

            };
            _jwtTokenRepository.Add(jwt);
            if (_jwtTokenRepository.SaveChanges() != 1)
            {
                return new ResponseRegisterModel() { Token = "", RefreshToken = "", Error = "Error add JwtToken to database", Status = 1 };
            }

            var refreshToken = CreateRefreshToken(jwtId, userId);
            if (refreshToken != null)
            {
                return new ResponseRegisterModel() { Token = jwt.Token, RefreshToken = refreshToken, Error = null, Status = 0 };
            }
            return new ResponseRegisterModel() { Token = "", RefreshToken = "", Error = "Error add RefreshToken to database", Status = 1 };

        }

        private string CreateRefreshToken(string JwtTokenId, string userId)
        {

            var refreshToken = new RefreshToken()
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = JwtTokenId,
                UserId = userId,
                CreateTime = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(2),
                Used = false,
                Invalidated = false

            };
            _refreshTokenRepository.Add(refreshToken);
            if (_refreshTokenRepository.SaveChanges() == 1)
            {
                return refreshToken.Token;
            }
            return null;

        }

        public string CheckRefreshToken(string token, string refrehToken)
        {
            if (!ValidateToken(token))
            {
                return "JwtToken is not validated."; // token not validated
            }
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenObj = handler.ReadJwtToken(token);
            // get exp of token 
            var expiryDateUnix = long.Parse(tokenObj.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            //
            var expiryDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
            if (DateTime.Compare(expiryDateTime, DateTime.Now) >= 0)
            {
                return "JwtToken hasn't expired yet !";
            }
            var jti = tokenObj.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value; // jwt id
            RefreshToken rt = _refreshTokenRepository.Get(refrehToken);
            if (rt == null)
            {
                return "The refresh token is not existed.";
            }
            if (DateTime.Compare(rt.ExpiryDate, DateTime.Now) < 0)
            {
                return "The refresh token has expired.";
            }
            if (rt.Invalidated)
            {
                return "The refresh token is invalidated.";
            }
            if (rt.Used)
            {
                return "The refresh token has been used.";
            }
            if (rt.JwtId != jti)
            {
                return "This refresh token did not match this JWT token.";
            }

            rt.Used = true;
            _refreshTokenRepository.Update(rt);
            if (_refreshTokenRepository.SaveChanges() != 1)
            {
                return "Server error";
            }
            return "OK";
        }

        private bool ValidateToken(string token)
        {

            var tokenHandlder = new JwtSecurityTokenHandler();
            var validParamaterToken = new TokenValidationParameters();
            validParamaterToken.ValidateLifetime = false;
            validParamaterToken.ValidateIssuer = true;
            validParamaterToken.ValidateAudience = true;
            validParamaterToken.ValidIssuer = _config.GetValue<string>("Jwt:ValidIssuer");
            validParamaterToken.ValidAudience = _config.GetValue<string>("Jwt:ValidAudience");
            validParamaterToken.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:SecretKey")));
            SecurityToken validToken;
            try
            {
                tokenHandlder.ValidateToken(token, validParamaterToken, out validToken);

            }
            catch (Exception)
            {
                return false;
            }
            return true;


        }
        public string GetUserIdFromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenObj = handler.ReadJwtToken(token);
            var userId = tokenObj.Claims.First(claim => claim.Type == "UserId").Value;
            return userId;
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
