using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces;              // ← Pour ILogRepository
using MyApp.Application.Interfaces.ILogRepository;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories
{
    public class LogRepository : GenericRepository<LogEntry>, ILogRepository
    {
        public LogRepository(ApplicationDbContext context)
            : base(context)
        {
        }

      
    }
}