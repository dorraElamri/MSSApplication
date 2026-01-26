using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces;
using MyApp.Infrastructure.Data;   // <-- Namespace de ApplicationDbContext

namespace MyApp.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context; // <-- DbContext CONCRET
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context) // <-- injection correcte
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<TResult?> GetSingleAsync<TResult>(
    Expression<Func<T, bool>>? predicate = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    Expression<Func<T, TResult>>? selector = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync();

            return await query.Cast<TResult>().FirstOrDefaultAsync(); // fallback to full entity
        }
    }
}
