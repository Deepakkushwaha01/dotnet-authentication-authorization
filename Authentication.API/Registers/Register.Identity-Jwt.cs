using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.API.Registers
{
public static partial class Register
    {
        public static IServiceCollection AddIdentityWithJwt(this IServiceCollection services, IConfiguration config)
        {
            // ✅ Bind Jwt settings
            var jwtSection = config.GetSection("Jwt");
            var jwtKey = jwtSection["Key"];
            var jwtIssuer = jwtSection["Issuer"];
            var jwtAudience = jwtSection["Audience"];

            if (string.IsNullOrEmpty(jwtKey))
                throw new ArgumentNullException("JWT Key not found in configuration");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            // ✅ Add Identity
            services
                .AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false; // For demo; true in prod
                    options.Lockout.MaxFailedAccessAttempts = 5;
                })
                .AddEntityFrameworkStores<Data.AuthDbContext>()
                .AddDefaultTokenProviders();

            // ✅ Add JWT Authentication
            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false; // Dev only; use true in prod
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            // ✅ Add Authorization with sample policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
                options.AddPolicy("ITOnly", p => p.RequireClaim("Department", "IT"));
            });

            return services;
        }
    }
}
