using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity ,new();
        public Task<int> CompeleteAsync();
        public ISessionRepository SessionRepository { get;}
    }
}
