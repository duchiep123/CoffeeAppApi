using Coffee_App.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.Token
{
    public interface ICoffeeToken
    {
        ResponseRegisterModel CreateToken(string userId);
        string CheckRefreshToken(string token, string refrehToken);
        string GetUserIdFromJWT(string token);
    }
}
