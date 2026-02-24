using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Infrastructure.Repositories
{
    public class UserInstanceRepository : IUserInstanceRepository
    {
        private readonly ApplicationDbContext _context;

        public UserInstanceRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsAsync(string userId, Guid instanceId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.UserInstances
                .AsNoTracking()
                .AnyAsync(ui => ui.UserId == userId && ui.InstanceId == instanceId);
        }

        public async Task<UserInstance?> GetLinkAsync(string userId, Guid instanceId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _context.UserInstances
                .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.InstanceId == instanceId);
        }

        public async Task AddAsync(UserInstance link)
        {
            if (link == null) throw new ArgumentNullException(nameof(link));

            await _context.UserInstances.AddAsync(link);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserInstance link)
        {
            if (link == null) throw new ArgumentNullException(nameof(link));

            _context.UserInstances.Remove(link);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Instance>> GetInstancesForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Array.Empty<Instance>();
            }

            var instances = await _context.UserInstances
                .AsNoTracking()
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.Instance)
                .ToListAsync();

            return instances.AsReadOnly();
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetUsersForInstanceAsync(Guid instanceId)
        {
            var users = await _context.UserInstances
                .AsNoTracking()
                .Where(ui => ui.InstanceId == instanceId)
                .Select(ui => ui.User)
                .ToListAsync();

            return users.AsReadOnly();
        }

        public async Task<bool> UserHasAnyLinkAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.UserInstances
                .AsNoTracking()
                .AnyAsync(ui => ui.UserId == userId);
        }

        public async Task<int> CountUsersForInstanceAsync(Guid instanceId)
        {
            return await _context.UserInstances
                .AsNoTracking()
                .CountAsync(ui => ui.InstanceId == instanceId);
        }
    }
}