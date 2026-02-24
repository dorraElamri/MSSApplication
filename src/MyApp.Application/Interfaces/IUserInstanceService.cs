using Domain.Entities;
using MyApp.Application.DTOs;
using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces
{
    public interface IUserInstanceService
    {
        Task<ApiResponse<bool>> HasAccessAsync(string userId, Guid instanceId);
        Task<ApiResponse<bool>> AssignUserAsync(string userId, Guid instanceId);
        Task<ApiResponse<bool>> RemoveUserAsync(string userId, Guid instanceId);
        Task<ApiResponse<IReadOnlyList<Instance>>> GetInstancesOfUserAsync(string userId);
        Task<ApiResponse<IReadOnlyList<ApplicationUser>>> GetUsersOfInstanceAsync(Guid instanceId);
        Task<ApiResponse<bool>> IsUserLinkedToAnyInstanceAsync(string userId);
        Task<ApiResponse<int>> GetInstanceUserCountAsync(Guid instanceId);
    }
}