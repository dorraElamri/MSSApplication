using Microsoft.AspNetCore.Identity;
using MyApp.Domain.Entities;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    // Add any extra properties for your user
    public string? FullName { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<UserInstance> UserInstances { get; set; } = new List<UserInstance>();

}
