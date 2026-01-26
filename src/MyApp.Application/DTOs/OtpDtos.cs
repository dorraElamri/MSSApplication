using System;
using MyApp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Application.DTOs
{

    // --------------------
    // DTO POUR GENERER OTP
    // --------------------
    public class OtpRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Purpose is required")]
        public OtpPurpose Purpose { get; set; }
    }



    // --------------------
    // DTO POUR VERIFIER OTP
    // --------------------
    public class OtpVerifyDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be 6 digits")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Purpose is required")]
        public OtpPurpose Purpose { get; set; }
    }
}

