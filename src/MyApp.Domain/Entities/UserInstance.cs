using Domain.Entities;

namespace MyApp.Domain.Entities;

public class UserInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public Guid InstanceId { get; set; }
    public Instance Instance { get; set; } = null!;
}
