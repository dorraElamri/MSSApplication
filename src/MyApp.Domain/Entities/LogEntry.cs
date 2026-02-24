namespace MyApp.Domain.Entities;
public class LogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string? Environment { get; set; }
    public string? Application { get; set; }
    public string? Service { get; set; }
    public string Message { get; set; } = string.Empty;

    // 🔥 AJOUT IMPORTANT
    public Guid InstanceId { get; set; }
    public Instance Instance { get; set; } = null!;

    public required SourceServer SourceServer { get; set; } = new();
    public required RequestInfo Request { get; set; } = new();
    public required ExceptionInfo Exception { get; set; } = new();

    public string? TraceId { get; set; }
    public string? CorrelationId { get; set; }



    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


public class SourceServer
{
    public string? Name { get; set; }
    public string? Ip { get; set; }
}

public class RequestInfo
{
    public string? Method { get; set; }
    public string? Endpoint { get; set; }
    public string? RequestId { get; set; }
    public int? DurationMs { get; set; }
}

public class ExceptionInfo
{
    public string? Type { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
}