using System;

namespace MyApp.Application.DTOs;

public class LogEntryDto
{
    public DateTimeOffset? Timestamp { get; set; }
    public string? Level { get; set; }
    public string? Environment { get; set; }
    public string? Application { get; set; }
    public string? Service { get; set; }
    public string? Message { get; set; }
    public SourceServerDto? SourceServer { get; set; }
    public string? TraceId { get; set; }
    public string? CorrelationId { get; set; }
    public RequestInfoDto? Request { get; set; }
    public ExceptionInfoDto? Exception { get; set; }
}

public class SourceServerDto
{
    public string? Name { get; set; }
    public string? Ip { get; set; }
}

public class RequestInfoDto
{
    public string? Method { get; set; }
    public string? Endpoint { get; set; }
    public string? RequestId { get; set; }
    public int? DurationMs { get; set; }
}

public class ExceptionInfoDto
{
    public string? Type { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
}