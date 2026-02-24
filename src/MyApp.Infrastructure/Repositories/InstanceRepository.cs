using System;
using MyApp.Domain.Entities;
using MyApp.Application.Interfaces;
using MyApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Infrastructure.Repositories
{
    public class InstanceRepository : IInstanceRepository
    {
        private readonly ApplicationDbContext _context;

        public InstanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instance>> GetAllAsync()
        {
            return await _context.Instances.ToListAsync();
        }

        public async Task<Instance?> GetByIdAsync(Guid id)
        {
            return await _context.Instances.FindAsync(id);
        }

        public async Task<Instance?> GetByApiKeyAsync(string apiKey)
        {
            return await _context.Instances.FirstOrDefaultAsync(i => i.ApiKey == apiKey);
        }

        public async Task<IEnumerable<Instance>> GetInstancesForUserAsync(string userId)
        {
            return await _context.UserInstances
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.Instance)
                .ToListAsync();
        }

        public async Task AddAsync(Instance instance)
        {
            await _context.Instances.AddAsync(instance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Instance instance)
        {
            _context.Instances.Update(instance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var instance = await GetByIdAsync(id);
            if (instance != null)
            {
                _context.Instances.Remove(instance);
                await _context.SaveChangesAsync();
            }
        }
    }
}


