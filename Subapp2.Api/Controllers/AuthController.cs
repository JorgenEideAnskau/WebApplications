using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Subapp2.Api.Dtos;

namespace Subapp2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string Secret = "ThisIsOnlyForCourseDemoAndMustBeLongEnough123!";
    private readonly IAntiforgery _antiforgery;

    public AuthController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    [AllowAnonymous]
    [HttpGet("csrf-token")]
    public ActionResult<object> GetCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }

    [ValidateAntiForgeryToken]
    [HttpPost("login")]
    public ActionResult<LoginResponseDto> Login(LoginRequestDto dto)
    {
        if (dto.Username != "student" || dto.Password != "Pass123!")
        {
            return Unauthorized();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "subapp2",
            audience: "subapp2",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return Ok(new LoginResponseDto(new JwtSecurityTokenHandler().WriteToken(token)));
    }

    public static SymmetricSecurityKey GetSigningKey() => new(Encoding.UTF8.GetBytes(Secret));
}
