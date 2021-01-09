using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Coffee_App.Repositories;
using Coffee_App.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Coffee_App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //jwt
            string key = Configuration.GetValue<string>("Jwt:SecretKey");
            //symmetric security key 
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            string issuer = Configuration.GetValue<string>("Jwt:ValidIssuer");
            string audience = Configuration.GetValue<string>("Jwt:ValidAudience");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    // what to validate
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    // set up validate data 
                    ValidIssuer = issuer, //The audience "aud" claim in a JWT is meant to refer to the Resource Servers that should accept the token.
                    ValidAudience = audience, //typically, the base address of the resource being accessed, such as https://contoso.com.
                    IssuerSigningKey = symmetricSecurityKey
                };

            });
            // Register the Swagger generator, defining 1 or more Swagger documents
            /* services.AddSwaggerGen(c =>
             {
                 c.SwaggerDoc("v1", new Info { Title = "AspNetCoreApiStarter", Version = "v1" });
                 // Swagger 2.+ support
                 c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                 {
                     In = "header",
                     Description = "Please insert JWT with Bearer into field",
                     Name = "Authorization",
                     Type = "apiKey"
                 });

                 c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
   {
     { "Bearer", new string[] { } }
   });
             });*/


            services.AddCors(option =>
            {
                option.AddPolicy("CoffeePolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddScoped<DbContext, CoffeeApplicationContext>();
            services.AddScoped<ICouponRepository, CouponRepository>(); // thua` ke BaseRepository
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ICoffeeToken, CoffeeToken>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CoffeePolicy");

            app.UseAuthentication(); // phai de dong nay truoc dong  app.UseAuthorization();
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
