using Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.IOtpService;
using MyApp.Domain.Entities;
namespace MyApp.Application.Services;

public class OtpService : IOtpService
{
    private readonly IOtpCodesRepository _otpCodesRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public OtpService(
        UserManager<ApplicationUser> userManager, IOtpCodesRepository otpCodesRepository)
    {
        _otpCodesRepository = otpCodesRepository;
        _userManager = userManager;
    }

    // -----------------------------------
    // SEND EMAIL (MailKit)
    // -----------------------------------
    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("no-reply@myapp.com"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, false);
        await smtp.AuthenticateAsync("YOUR_EMAIL@gmail.com", "APP_PASSWORD");
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    // -----------------------------------
    // GENERATE & SEND OTP
    // -----------------------------------
    public async Task GenerateAndSendOtpAsync(string email, OtpPurpose purpose)
    {
        var user = await _userManager.FindByEmailAsync(email)
                   ?? throw new Exception("User not found");

        // Invalidate old OTPs
        var oldOtps = await _otpCodesRepository.FindAsync(x =>
                x.UserId == user.Id &&
                x.Purpose == purpose &&
                x.Status == OtpStatus.Pending);

        foreach (var oldOtp in oldOtps)
            oldOtp.Status = OtpStatus.Expired;

        // Generate OTP
        var code = new Random().Next(100000, 999999).ToString();

        var otp = new OtpCode
        {
            UserId = user.Id,
            Code = code,
            Purpose = purpose,
            Status = OtpStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        await _otpCodesRepository.AddAsync(otp);

        await SendAsync(
            user.Email!,
            "Your OTP Code",
            $"""
            <h2>Verification Code</h2>
            <p>Your OTP code is:</p>
            <h1>{code}</h1>
            <p>This code expires in 10 minutes.</p>
            """
        );
    }

    // -----------------------------------
    // VERIFY OTP
    // -----------------------------------
    public async Task<bool> VerifyOtpAsync(string email, string code, OtpPurpose purpose)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;
        var otp = await _otpCodesRepository.GetSingleAsync<OtpCode>(
    predicate: x => x.UserId == user.Id &&
                    x.Code == code &&
                    x.Purpose == purpose &&
                    x.Status == OtpStatus.Pending &&
                    x.ExpirationDate > DateTime.UtcNow,
    orderBy: q => q.OrderByDescending(x => x.CreatedAt));


        if (otp == null)
            return false;

        otp.Status = OtpStatus.Used;
        await _otpCodesRepository.UpdateAsync(otp);

        return true;
    }
}
