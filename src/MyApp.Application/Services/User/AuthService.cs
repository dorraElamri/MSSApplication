using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using Serilog;

namespace MyApp.Application.Services.User;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new Exception("Invalid credentials");

            return await GenerateTokens(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Login failed for {Email}", dto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            return await GenerateTokens(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Refresh token failed");
            throw;
        }
    }

    private async Task<AuthResponseDto> GenerateTokens(ApplicationUser user)
    {
        var accessMinutes = int.Parse(_config["Jwt:AccessTokenDurationInMinutes"]!);
        var refreshDays = int.Parse(_config["Jwt:RefreshTokenDurationInDays"]!);
        var expiresAt = DateTime.UtcNow.AddMinutes(accessMinutes);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.Name, user.Email!),
          
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        user.RefreshToken = Guid.NewGuid().ToString();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken,
            AccessTokenExpiresAt = expiresAt
        };
    }
}
