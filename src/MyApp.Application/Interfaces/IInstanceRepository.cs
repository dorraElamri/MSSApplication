using System;
namespace MyApp.Application.Interfaces;


using MyApp.Domain.Entities;

public interface IInstanceRepository 
{
    Task<IEnumerable<Instance>> GetAllAsync();
    Task<Instance?> GetByIdAsync(Guid id);
    Task<Instance?> GetByApiKeyAsync(string apiKey);
    Task<IEnumerable<Instance>> GetInstancesForUserAsync(string userId);
    Task AddAsync(Instance instance);
    Task UpdateAsync(Instance instance);
    Task DeleteAsync(Guid id);

}


