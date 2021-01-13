using Coffee_App.Cache;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coffee_App.Middleware
{
    public class ChallengeMiddleware
    {
        private async Task writeErrorResponse(HttpContext Context, string jsonBody)
        {
            Context.Response.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes(jsonBody);
            // write bytes to stream
            await Context.Response.Body.WriteAsync(data, 0, data.Length); // ko write async se bi loi Synchronous operations are disallowed

        }

        private readonly RequestDelegate _request; // a function that can process HTTP request

        public ChallengeMiddleware(RequestDelegate RequestDelegate)
        {
            if (RequestDelegate == null)
            {
                throw new ArgumentNullException(nameof(RequestDelegate)
                    , nameof(RequestDelegate) + " is required");
            }

            _request = RequestDelegate;
        }

        public async Task InvokeAsync(HttpContext Context)
        {
            if (Context == null)
            {
                throw new ArgumentNullException(nameof(Context)
                    , nameof(Context) + " is required");
            }

            #region check token in blacklist
            string jwtToken = Context.Request.Headers["Authorization"];
            if (jwtToken != null)
            {
                if (jwtToken.StartsWith("Bearer"))
                {
                    jwtToken = jwtToken.Substring(7);
                }
                var jwtCache = JWTTokenCache.AppCache;
                DateTime createDate;
                bool result = jwtCache.TryGetValue(jwtToken, out createDate);
                if (!result)
                {
                    Context.Response.StatusCode = 401;

                }
            }
            #endregion

            //Handler Error
            if (Context.Response.StatusCode == 401)
            {

                string json = JsonConvert.SerializeObject(new { message = "Invalid token." });
                await writeErrorResponse(Context, json);
            }
            else // if no error
            {
                await _request(Context); // call next middleware
            }
        }
    }
}
