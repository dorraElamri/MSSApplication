using System;
using Domain.Entities;
using System.Net.NetworkInformation;

namespace MyApp.Domain.Entities
{

    public enum OtpPurpose
    {
        ForgotPassword = 1,
        EmailVerification = 2
    }

    public enum OtpStatus
    {
        Pending = 0,   // Créé mais pas encore utilisé
        Used = 1,      // Consommé
        Expired = 2    // Expiré
    }




    public class OtpCode
    {
        public int IdOtp { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Code { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }

        public OtpStatus Status { get; set; }

        public OtpPurpose Purpose { get; set; }
    }

}

