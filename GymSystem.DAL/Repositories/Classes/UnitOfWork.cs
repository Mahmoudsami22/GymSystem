using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext dbContext;
        private readonly Dictionary<String, Object> _Repos = [];

        public UnitOfWork(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if(_Repos.TryGetValue(typeName, out object oldRepository))
            {
                return (IGenericRepository<TEntity>)oldRepository;
            }
            var newRepository = new GenericRepository<TEntity>(dbContext);
            _Repos[typeName] = newRepository;
            return newRepository;
        }
        public async Task<int> CompeleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
