using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.API.Data;
using Authentication.API.Dtos.Auth;
using Authentication.API.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Authentication.API.Infrastructure.Configurations.Models;

namespace Authentication.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtSettings _jwt;
    private readonly AppDbContext _context;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IOptions<JwtSettings> jwt,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt.Value;
        _context = context;
    }

    [Authorize]
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("pong");

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };

        // 1️⃣ Create user first
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // 2️⃣ Generate tokens and save refresh token
        var tokens = await GenerateTokensAsync(user);

        return Ok(new
        {
            message = "User registered successfully",
            tokens
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username) ??
                   await _userManager.FindByEmailAsync(model.Username);
        if (user == null) return Unauthorized("Invalid credentials");

        var pwOk = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!pwOk) return Unauthorized("Invalid credentials");

        var tokens = await GenerateTokensAsync(user);

        return Ok(tokens);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var storedToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken == null || storedToken.IsExpired)
            return Unauthorized("Invalid or expired refresh token");

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null)
            return Unauthorized("User not found");

        // Delete old token
        _context.Remove(storedToken);
        await _context.SaveChangesAsync();

        var tokens = await GenerateTokensAsync(user);

        return Ok(tokens);
    }

    private async Task<object> GenerateTokensAsync(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("sid", user.SecurityStamp ?? string.Empty)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        // Generate Access Token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var accessToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
            signingCredentials: creds);

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

        // Generate Refresh Token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
        };

        await _context.Set<RefreshToken>().AddAsync(refreshToken);
        await _context.SaveChangesAsync(); // ✅ Save refresh token

        return new
        {
            access_token = accessTokenString,
            refresh_token = refreshToken.Token,
            token_type = "Bearer",
            expires_in_minutes = _jwt.AccessTokenMinutes
        };
    }
}
