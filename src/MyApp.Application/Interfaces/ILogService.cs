using MyApp.Application.DTOs;
using MyApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Application.Interfaces;

public interface ILogService 
{
    Task<ApiResponse<Guid>> CreateLogAsync(LogEntryDto dto);
    Task<ApiResponse<List<LogEntry>>> GetAllLogsAsync();
    Task<ApiResponse<List<LogEntry>>> GetRecentLogsAsync(int count = 50);
}