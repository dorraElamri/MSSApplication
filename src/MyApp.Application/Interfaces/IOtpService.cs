using System;
using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces.IOtpService
{
	public interface IOtpService
	{

        Task SendAsync(string to, string subject, string body);
        Task GenerateAndSendOtpAsync(string email, OtpPurpose purpose);
        Task<bool> VerifyOtpAsync(string email, string code, OtpPurpose purpose);
    }
}

