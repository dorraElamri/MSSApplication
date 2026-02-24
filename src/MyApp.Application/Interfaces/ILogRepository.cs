using MyApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Application.Interfaces.ILogRepository;

public interface ILogRepository : IGenericRepository<LogEntry>
{

}
