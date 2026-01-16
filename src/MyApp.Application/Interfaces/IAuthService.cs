using System;
using Domain.Entities;
using MyApp.Application.DTOs;

namespace MyApp.Application.Interfaces
{
	public interface IAuthService
	{
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        //Task<AuthResponseDto> GenerateTokens(ApplicationUser user);

    }
}

