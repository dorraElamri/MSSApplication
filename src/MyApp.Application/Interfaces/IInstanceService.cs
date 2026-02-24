using System;
using MyApp.Application.DTOs;
using MyApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MyApp.Application.Interfaces;

public interface IInstanceService 
 {
    //Task<IEnumerable<Instance>> GetAllAsync();
    //Task<Instance?> GetByIdAsync(Guid id);
    //Task<Instance?> GetByApiKeyAsync(string apiKey);
    //Task<IEnumerable<Instance>> GetInstancesForUserAsync(string userId);
    //Task AddAsync(Instance instance);
    //Task UpdateAsync(Instance instance);
    //Task DeleteAsync(Guid id);

    //Task<IEnumerable<Instance>> GetAllInstancesAsync();
    //Task<Instance?> GetInstanceByIdAsync(Guid id);
    //Task<Instance?> GetInstanceByApiKeyAsync(string apiKey);
    //Task<IEnumerable<Instance>> GetInstancesForUserAsync(string userId);
    //Task CreateInstanceAsync(Instance instance);
    //Task UpdateInstanceAsync(Instance instance);
    //Task DeleteInstanceAsync(Guid id);
    //Task AssignUserToInstanceAsync(string userId, Guid instanceId);
    //Task RemoveUserFromInstanceAsync(string userId, Guid instanceId);

    
        Task<ApiResponse<List<Instance>>> GetAllInstancesAsync();
        Task<ApiResponse<Instance>> GetInstanceByIdAsync(Guid id);
        Task<ApiResponse<Instance>> GetInstanceByApiKeyAsync(string apiKey);
        Task<ApiResponse<List<Instance>>> GetInstancesForUserAsync(string userId);
        Task<ApiResponse<Guid>> CreateInstanceAsync(CreateInstanceDto dto);
        Task<ApiResponse<bool>> UpdateInstanceAsync(Guid id, UpdateInstanceDto dto);
        Task<ApiResponse<bool>> DeleteInstanceAsync(Guid id);
        Task<ApiResponse<string>> RegenerateApiKeyAsync(Guid id);



}


