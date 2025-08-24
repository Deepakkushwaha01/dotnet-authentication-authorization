# **Repo:** https://github.com/Deepakkushwaha01/dotnet-authentication-authorization

# 🚀 Identity + JWT + SQL Server (Docker) — Full Hinglish Documentation (MVC-style)

Yeh documentation **beginner-friendly Hinglish** me likhi gayi hai. Goal:

- **ASP.NET Core Identity** ko **JWT** ke saath integrate karna
- **SQL Server (Docker)** use karke persistence setup karna
- **Modern MVC-style structure** maintain karna
- Har important code block ke **line-by-line explanations** dena

> Scope: .NET 8 Web API (Controllers), Identity + Roles + Claims, JWT Access Token, Security Stamp, Swagger JWT integration, Role/Claim policies, DB migrations, seeding.
> 

---

## 0) Prerequisites

- .NET 8 SDK installed
- Docker Desktop installed & running
- VS Code / Rider / Visual Studio
- Postman / curl for testing

---

---

## 🛠 Step 0.1: Create a Solution

First, let’s create a new solution named **authentication**:

```bash
dotnet new sln -o authentication

```

👉 The `-o` flag tells the CLI to create the output in a new folder named `authentication`.

Now, move into this folder:

```bash
cd authentication

```

---

## 🛠 Step 0.2: Create Web API with Controllers

Next, create a new **Web API** project inside this solution:

```bash
dotnet new webapi --use-controllers -o Authentication.API

```

This will generate a Web API project with the default controller structure.

Now, add this project to the solution:

```bash
dotnet sln add Authentication.API/Authentication.API.csproj

```

---

## 🛠 Step 0.3: Initialize Git & GitIgnore

It’s always a good practice to start with Git version control.

In the **root folder** (`authentication`):

```bash
git init
dotnet new gitignore

```

👉 This creates a `.gitignore` file with all the standard .NET exclusions.

---

## 🧹 Step 0.4: Clean Default Code

By default, .NET creates **demo files** like:

- `WeatherForecast.cs`
- `WeatherForecastController.cs`

📝 **Why remove them?**

These are just placeholder examples, not required in a real project. Cleaning them up keeps your codebase minimal and professional.

## 🧠 Step 0.5: Program.cs Simplified Structure

Here’s a clean `Program.cs` template:

```csharp
namespace Dotnet.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                var app = BuildApplication(builder);
                RunApplication(app);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private static WebApplication BuildApplication(WebApplicationBuilder builder)
        {
            return builder.Build();
        }

        private static void RunApplication(WebApplication app)
        {
            app.Run();
        }
    }
}

```

### 🔎 Explanation

- `WebApplication.CreateBuilder(args)` → Starts app configuration.
- `BuildApplication()` → Converts the builder into a `WebApplication`.
- `RunApplication()` → Runs the app.
- `try-catch` → Helps in production-grade error handling.

This keeps `Program.cs` clean and maintainable.

---

---

## 1) Solution & Project Structure (MVC-ish Modern)

```
authentication/                     ← solution root
├─ Authentication.API/              ← Web API (Controllers-based)
│  ├─ Controllers/
│  │   └─ AuthController.cs
│  ├─ Data/
│  │   └─ AppDbContext.cs
│  ├─ Domain/                       ← (optional) Domain models
│  ├─ Dtos/
│  │   ├─ Auth/
│  │   │   ├─ LoginDto.cs
│  │   │   └─ RegisterDto.cs
│  ├─ Infrastructure/
│  │   ├─ Identity/
│  │   │   ├─ IdentityConfig.cs      ← Identity + JWT registration
│  │   │   └─ RoleSeeder.cs          ← Seed roles/admin
│  │   └─ Settings/
│  │       ├─ JwtSettings.cs
│  │       └─ SwaggerSettings.cs
│  ├─ Middleware/
│  ├─ Registers/                     ← Extension methods (DI)
│  │   ├─ Register.Versioning.cs
│  │   ├─ Register.Swagger.cs
│  │   └─ Register.Database.cs
│  ├─ appsettings.json
│  ├─ Program.cs
│  └─ Authentication.API.csproj
├─ docker-compose.yml
└─ .env

```

**Why this structure?**

- **Controllers** → Only HTTP endpoints.
- **Infrastructure** → Identity/JWT setup, seeding, external services.
- **Data** → EF Core DbContext.
- **Dtos** → Request/response contracts (clean controllers).
- **Registers** → DI extension methods (Program.cs clean).
- **Settings** → Strongly-typed config binding (Jwt, Swagger).

---

## 2) Docker me SQL Server setup

**docker-compose.yml**

```yaml
services:
  mssqldb:
    container_name: system.mssql.db
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=${EXTERNAL_INTEGRATION_MSSQL_PASSWORD}
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - authentication-sql-data:/var/opt/mssql

volumes:
  authentication-sql-data:

```

**Explanation:**

- `image`: Official SQL Server 2022.
- `MSSQL_SA_PASSWORD`: `.env` se aayega (security ke liye code me hardcode nahi).
- `ports`: Host 1433 → container 1433.
- `volumes`: Data persistent rahe.

**.env** (root me)

```
EXTERNAL_INTEGRATION_MSSQL_PASSWORD=ChangeThisStrong!Passw0rd

```

**Run:** `docker-compose up -d`

---

## 3) Packages install karo (Identity + JWT + EF Core + Swagger + Versioning)

Project folder me:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
# optional utilities
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Mvc.Versioning

```

---

---

### ✅ 1. `Microsoft.EntityFrameworkCore.SqlServer`

- **Purpose:**
    
    Ye package **Entity Framework Core** ka SQL Server provider hai.
    
    Iska kaam hai .NET application ko SQL Server ke sath **ORM (Object-Relational Mapping)** ke through interact karwana.
    
- **Why we need it:**
    - Agar tumhara database **SQL Server** hai, toh yeh package mandatory hai.
    - Ye SQL commands internally generate karke database ke sath CRUD operations handle karta hai.
- **Use Case:**
    - Jab tum `DbContext` class banate ho aur migration run karte ho, SQL Server ke liye commands yahi se aate hai.

---

### ✅ 2. `Microsoft.EntityFrameworkCore.Tools`

- **Purpose:**
    
    Ye **developer tooling** ke liye hai, jisse hum **migrations** aur **database updates** command line se kar sakein.
    
- **Why we need it:**
    - Ye package `dotnet ef` CLI tools ke liye support provide karta hai.
    - Migrations create karne ke liye:
        
        ```bash
        dotnet ef migrations add InitialCreate
        dotnet ef database update
        
        ```
        
- **Use Case:**
    - Development time par jab tum DB schema update karte ho code changes ke sath.

---

### ✅ 3. `Microsoft.EntityFrameworkCore.Design`

- **Purpose:**
    
    Ye **design-time services** provide karta hai for EF Core, mainly for **scaffolding** (database se models generate karne) aur migrations.
    
- **Why we need it:**
    - Agar tum `dotnet ef` command se scaffolding kar rahe ho (Reverse Engineer DB → Models), toh ye package required hai.
    - Without this, design-time tools work nahi karenge.

---

### ✅ 4. `Microsoft.AspNetCore.Identity.EntityFrameworkCore`

- **Purpose:**
    
    Ye **ASP.NET Core Identity** ka implementation hai jo EF Core use karta hai.
    
    - Iska kaam: Identity tables ko create karna (Users, Roles, Claims, etc.) aur unke sath kaam karna via EF.
- **Why we need it:**
    - Agar tum **ASP.NET Core Identity** use kar rahe ho authentication/authorization ke liye, toh yeh package must hai.
    - Ye humare liye ready-made DB structure (tables like `AspNetUsers`, `AspNetRoles`) provide karta hai.
- **Use Case:**
    - User registration, login, password hashing, role-based access, claims-based security.

---

### ✅ 5. `Microsoft.AspNetCore.Authentication.JwtBearer`

- **Purpose:**
    - Ye JWT (**JSON Web Token**) based authentication enable karta hai.
    - Basically, tumhare app me agar **token-based login** karna hai toh ye package handle karega token validation.
- **Why we need it:**
    - Agar tum frontend ko token dekar authorize karte ho, toh backend ko tokens validate karne ka mechanism chahiye → ye wahi hai.
    - Without this, JWT middleware kaam nahi karega.
- **Use Case:**
    - API endpoints protect karna using `[Authorize]` attribute.
    - Validate JWT for each request.

---

### ✅ 6. `Swashbuckle.AspNetCore` (Optional)

- **Purpose:**
    
    Ye **Swagger/OpenAPI** ke liye use hota hai.
    
    - Swagger se tumhari API ke liye **interactive documentation UI** ban jata hai.
- **Why we need it:**
    - Developer-friendly interface for testing API endpoints without Postman.
    - API ko share karne ke liye automatic docs generate karna.

---

### ✅ 7. `Microsoft.AspNetCore.Mvc.Versioning` (Optional)

- **Purpose:**
    
    Ye API **versioning** ke liye hai.
    
    - Agar tum apni API ke multiple versions maintain karna chahte ho (v1, v2, etc.), toh ye package help karega.
- **Why we need it:**
    - Backward compatibility maintain karne ke liye.
    - Large-scale apps me jab new features aate hai aur old clients still old API use karte hai.

---

✅ Ye sab packages milke tumhe **Full Stack Secure Web API** banane me help karenge with:

- Database handling (EF Core + SQL Server)
- Identity Management (Users, Roles, Claims)
- Authentication via JWT
- Swagger docs for testing
- Versioning for scalability

---

---

## 4) App settings (Connection + Jwt + Swagger)

**Authentication.API/appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=AuthDb;User Id=sa;Password=ChangeThisStrong!Passw0rd;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Issuer": "auth.example.com",
    "Audience": "auth.example.com",
    "Key": "SuperSecretKey_Min32Chars_Long_For_HS256!!",
    "AccessTokenMinutes": 30
  },
  "Swagger": {
    "Title": "Authentication API",
    "Description": "Identity + JWT + SQL Server",
    "Version": "v1",
    "RoutePrefix": "swagger",
    "JsonEndpointUrl": "/swagger/v1/swagger.json"
  },
  "AllowedHosts": "*"
}

```

**Why each key?**

- `ConnectionStrings.DefaultConnection` → EF Core ke liye DB.
- `Jwt.Issuer/Audience` → Token validation ke 2 anchors (who issued, for whom).
- `Jwt.Key` → **Symmetric signing key** (≥ 32 chars recommended for HS256).
- `AccessTokenMinutes` → Expiry control (config-driven).
- `Swagger` → UI title/route.

> Security Tip: Prod me Jwt.Key ko Secret Manager / Key Vault ya env vars me rakho.
> 

---

## 5) EF Core DbContext (Identity ke saath)

**Data/AppDbContext.cs**

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}

```

**Explanation (line-by-line):**

- `IdentityDbContext<IdentityUser>`: Identity ke saare tables + IdentityUser mapping ready.
- `options` injected by DI with SQL Server connection.

**Interface dbContext**

```jsx
namespace Authentication.API.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public interface IAppDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        ChangeTracker ChangeTracker { get; }

        void HealthCheck();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
```

---

## 6) Strongly-typed settings

**Infrastructure/Settings/JwtSettings.cs**

```csharp
namespace Authentication.API.Infrastructure.Settings;

public sealed class JwtSettings
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 30;
}

```

**Settings/ConnectionStringSettings.cs**

```jsx
namespace Authentication.API.Settings
{
    public class ConnectionStringSettings(IConfiguration configuration)
    {
        public string IntegrationDbConnectionString { get; } = configuration.GetValue<string>("ConnectionStrings:IntegrationDbConnectionString")!;
    }
}
```

**Registers/Register.Database.cs**

```csharp
using Authentication.API.Data;
using Authentication.API.Settings;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Registers
{
    public static partial class Registers
    {
        public static IServiceCollection AddDatabaseRegister(this IServiceCollection services, IConfiguration configuration)
        {
            ConnectionStringSettings connectionStringSettings = new(configuration);
            services.AddScoped<IAppDbContext, AppDbContext>().AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionStringSettings.IntegrationDbConnectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                            sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(15),
                            errorNumbersToAdd: null);
                });
            });
            return services;
        }
    }
}
```

---

## 7) Identity + JWT configuration (DI extensions)

**Infrastructure/Identity/IdentityConfig.cs**

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.API.Registers
{
    public static class IdentityJwtRegister
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
                .AddEntityFrameworkStores<Data.AppDbContext>()
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

```

**Explanation highlights:**

- `AddIdentity<IdentityUser, IdentityRole>`: User + Role stores + token providers.
- `Password/Lockout` options: security posture.
- `AddJwtBearer`: Token validation rules (issuer/audience/key/expiry).
- `Authorization policies`: central place for role/claim rules.

---

## 8) Swagger + JWT support

**Registers/Register.Swagger.cs**

```csharp
using Microsoft.OpenApi.Models;

namespace Authentication.API.Registers;

public static partial class Register
{
    public static IServiceCollection RegisterSwagger(this IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = config["Swagger:Title"],
                Version = config["Swagger:Version"],
                Description = config["Swagger:Description"],
            });

            // JWT Bearer in Swagger UI
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'"
            };
            o.AddSecurityDefinition("Bearer", securityScheme);
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new List<string>() }
            });
        });
        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(config["Swagger:JsonEndpointUrl"], config["Swagger:Title"]);
            c.RoutePrefix = config["Swagger:RoutePrefix"];
        });
        return app;
    }
}

```

---

## 9) API Versioning register

**Registers/Register.Versioning.cs**

```csharp
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Registers;

public static partial class Register
{
    public static IServiceCollection RegisterVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        return services;
    }
}

```

---

## 10) Program.cs (Clean entrypoint)

**Program.cs**

```csharp
using Authentication.API.Infrastructure.Identity;
using Authentication.API.Registers;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.RegisterVersioning();
builder.Services.RegisterDatabase(builder.Configuration);
builder.Services.AddIdentityWithJwt(builder.Configuration);
builder.Services.RegisterSwagger(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
app.UseSwaggerDocumentation(app.Configuration);

app.UseHttpsRedirection();
app.UseAuthentication(); // IMPORTANT: before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();

```

**Why order matters?**

- `UseAuthentication()` **before** `UseAuthorization()` → pehle principal set hota hai, phir policies apply hoti hain.

---

## 11) DTOs (clean inputs)

**Dtos/Auth/RegisterDto.cs**

```csharp
namespace Authentication.API.Dtos.Auth;

public sealed record RegisterDto(string Username, string Email, string Password);

```

**Dtos/Auth/LoginDto.cs**

```csharp
namespace Authentication.API.Dtos.Auth;

public sealed record LoginDto(string Username, string Password);

```

---

## 12) Role Seeding (Admin/User)

**Infrastructure/Identity/RoleSeeder.cs**

```csharp
using Microsoft.AspNetCore.Identity;

namespace Authentication.API.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = ["Admin", "User"];
        foreach (var r in roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        // Admin user (demo)
        var adminEmail = "admin@example.com";
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new IdentityUser { UserName = "admin", Email = adminEmail, EmailConfirmed = true };
            await userMgr.CreateAsync(admin, "Admin#12345");
            await userMgr.AddToRoleAsync(admin, "Admin");
            await userMgr.AddClaimAsync(admin, new System.Security.Claims.Claim("Department", "IT"));
        }
    }
}

```

**Call seeding at startup (optional prod-safe):**

```csharp
// after builder.Build(); before app.Run();
await Authentication.API.Infrastructure.Identity.RoleSeeder.SeedAsync(app.Services);

```

---

## 13) Auth Controller (Register/Login + JWT)

**Controllers/AuthController.cs**

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.API.Dtos.Auth;
using Authentication.API.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtSettings _jwt;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IOptions<JwtSettings> jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt.Value;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(new { message = "User registered" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username) ??
                   await _userManager.FindByEmailAsync(model.Username);
        if (user is null) return Unauthorized("Invalid credentials");

        var pwOk = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!pwOk) return Unauthorized("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("sid", user.SecurityStamp ?? string.Empty)
        };
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        // custom claims (example): Department from DB user claims
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var token = GenerateJwt(claims);
        return Ok(new { access_token = token, token_type = "Bearer", expires_in_minutes = _jwt.AccessTokenMinutes });
    }

    private string GenerateJwt(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

```

**Important lines:**

- `new("sid", user.SecurityStamp)`: Security stamp claim add kiya → password/email change pe invalidate logic implement kar sakte ho.
- `GetRolesAsync` + `ClaimTypes.Role`: Role-based authorization ke liye.
- `GetClaimsAsync`: User-level custom claims include.

---

## 14) Protected endpoint examples

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DemoController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new { message = "You are authenticated" });

    [HttpGet("admin-area")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly() => Ok("Welcome, Admin!");

    [HttpGet("it-dept")]
    [Authorize(Policy = "ITOnly")]
    public IActionResult ItOnly() => Ok("Hello IT Department user");
}

```

---

## 15) Migrations & Database creation

Commands (from project folder):

```bash
dotnet ef migrations add InitialIdentity
dotnet ef database update

```

**Tables auto-created:**

- `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`
- `AspNetUserClaims`, `AspNetRoleClaims`
- `AspNetUserLogins`, `AspNetUserTokens`

---

## 16) Swagger me JWT token kaise pass karein

- Swagger UI open → **Authorize** button → `Bearer {your_token}` paste karo (Bearer + space + token).
- Ab protected endpoints call honge.

---

## 17) Frontend/Client examples

**curl (login):**

```bash
curl -X POST http://localhost:5080/api/v1.0/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin#12345"}'

```

**curl (authorized):**

```bash
curl http://localhost:5080/api/v1.0/demo/me \
  -H "Authorization: Bearer {ACCESS_TOKEN}"

```

---

## 18) Security Stamp — Why & How

- **Why:** Agar user ka password/email/2FA change hua → **purane tokens invalidate** karne chahiye.
- **How:**
    - Token generate karte waqt `sid = user.SecurityStamp` claim add karo.
    - Custom `JwtBearerEvents.OnTokenValidated` hook me DB se user ka current `SecurityStamp` compare karo. Agar mismatch → `context.Fail("Stamp mismatch")`.

**Add in AddJwtBearer:**

```csharp
o.Events = new JwtBearerEvents
{
    OnTokenValidated = async context =>
    {
        var userMgr = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var userId = context.Principal!.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var sid = context.Principal!.FindFirst("sid")?.Value;
        var user = await userMgr.FindByIdAsync(userId!);
        if (user == null || user.SecurityStamp != sid)
            context.Fail("Security stamp mismatch");
    }
};

```

**Result:** Password reset/critical change pe **force re-login**.

---

## 19) Common Pitfalls & Fixes

- ❌ `UseAuthorization()` without `UseAuthentication()` → 401/403 issues.
- ❌ Wrong `Issuer/Audience` → token rejected.
- ❌ Too-short JWT Key → runtime error / weak security.
- ❌ Missing Swagger security definition → token pass nahi hota.
- ❌ Not seeding roles before assigning → AddToRole fails.

---

## 20) Production Notes

- Always **HTTPS** enable + set `RequireHttpsMetadata = true`.
- JWT key rotate policy consider karo.
- Refresh Tokens implement karo (separate table: `UserRefreshTokens`) with rotation.
- Brute-force protection: lockout config + IP throttling (rate limiting middleware).
- Email confirmation mandatory + 2FA for Admins.
- Store secrets in Azure Key Vault / AWS Secrets Manager.

---

## 21) What to change for MVC (Server-rendered) apps?

- API ke jagah MVC controllers + views.
- JWT ki jagah **Cookie Authentication** (package: `Microsoft.AspNetCore.Authentication.Cookies`).
- Identity scaffolding UI use kar sakte ho (login/register views).
- Role/Policy logic same rehta hai.

---

## 22) Concept Recap (Cheat Sheet)

- **Identity** → User + Roles + Claims + Tokens + SecurityStamp.
- **JWT** → Stateless auth; signed payload; send via `Authorization: Bearer`.
- **Claims** → Fine-grained access (Department, Tenant, Permissions).
- **SecurityStamp** → Invalidate old tokens after sensitive changes.
- **Policies** → Centralized authorization (role/claim rules).
- **Order** → `UseAuthentication()` → `UseAuthorization()`.

---

## 23) End-to-End Flow (ASCII Diagram)

```
Client (React/Angular)
  │  POST /auth/register → {username,email,password}
  ▼
API (Identity): Create user → AspNetUsers
  │
  │  POST /auth/login → credentials
  ▼
Validate → Build Claims (Roles + UserClaims + sid:SecurityStamp)
  │
  │  ← JWT (exp, iss, aud, signature)
  ▼
Store token (localStorage/cookie)
  │
  │  GET /demo/me with Authorization: Bearer <token>
  ▼
JwtBearer middleware → validate signature/exp/iss/aud
  │ compare sid with DB SecurityStamp
  │ apply [Authorize] policies
  ▼
Controller executes → 200 OK 🔐

```

---

### ✅ You’re ready!

Is template ko copy karke apne project me apply karo. Agar chaho to **Refresh Token flow**, **Email confirmation**, **Forgot/Reset Password**, **External Logins (Google/AzureAD)** bhi add karwa deta hoon in the same structure.
