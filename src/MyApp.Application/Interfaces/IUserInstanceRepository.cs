using Domain.Entities;
using MyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Application.Interfaces
{
    public interface IUserInstanceRepository
    {
        Task<bool> ExistsAsync(string userId, Guid instanceId);
        Task AddAsync(UserInstance link);
        Task DeleteAsync(UserInstance link);
        Task<UserInstance?> GetLinkAsync(string userId, Guid instanceId);
        Task<IReadOnlyList<Instance>> GetInstancesForUserAsync(string userId);
        Task<IReadOnlyList<ApplicationUser>> GetUsersForInstanceAsync(Guid instanceId);
        Task<bool> UserHasAnyLinkAsync(string userId);
        Task<int> CountUsersForInstanceAsync(Guid instanceId);
    }
}