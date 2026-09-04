using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext dbcontext;

        public GenericRepository(GymDbContext _dbcontext)
        {
            dbcontext = _dbcontext;
        }
        public void Add(TEntity entity)
        {
            dbcontext.Set<TEntity>().Add(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            
            return await dbcontext.Set<TEntity>().AnyAsync(predicate, ct);
        }

        public async Task<int> completeAsync()
        {
            return await dbcontext.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var entity = dbcontext.Set<TEntity>().FirstOrDefault(p => p.Id == id);
            if (entity is not null)
            {
                dbcontext.Set<TEntity>().Remove(entity);
            }
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked, CancellationToken ct = default)
        {
            var entity = isTracked ? dbcontext.Set<TEntity>() : dbcontext.Set<TEntity>().AsNoTracking();
            return await entity.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAll(bool isTracked, CancellationToken ct = default)
        {
            var entitys = isTracked ? dbcontext.Set<TEntity>() : dbcontext.Set<TEntity>().AsNoTracking();
            return await entitys.ToListAsync(ct);
        }

        public Task<TEntity?> GetById(int id, CancellationToken ct = default)
        {
            var entity = dbcontext.Set<TEntity>().FirstOrDefaultAsync(p => p.Id == id, ct);
            return entity;
        }

        public void Update(TEntity entity)
        {
            dbcontext.Set<TEntity>().Update(entity);
        }
    }
}
