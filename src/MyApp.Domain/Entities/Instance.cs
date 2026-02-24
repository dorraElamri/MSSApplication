namespace MyApp.Domain.Entities;

//public class Instance
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public string Host { get; set; }
//    public string ApiKey { get; set; }
//    //+logpath

//    // ✅ AJOUTER ÇA
//    public ICollection<UserInstance> UserInstances { get; set; } = new List<UserInstance>();

//    public ICollection<LogEntry> Logs { get; set; } = new List<LogEntry>();
//}




public enum EnvironmentType
{
    Development = 0,
    Staging = 1,
    Production = 2
}

public class Instance
{
    public Guid Id { get; set; }

    //Infos générales

    public string? ApplicationName { get; set; }
    public string Host { get; set; } = string.Empty;
    public string? Description { get; set; }

    // 🔐 Sécurité
    public string ApiKey { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime ApiKeyCreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApiKeyLastUsedAt { get; set; }
    
    // ⚙️ Configuration du worker
    public string? LogPath { get; set; } // chemin fichier logs
    public EnvironmentType Environment { get; set; }

    // 🕒 Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // 🔗 Relations
    public ICollection<UserInstance> UserInstances { get; set; } = new List<UserInstance>();
    public ICollection<LogEntry> Logs { get; set; } = new List<LogEntry>();

}


