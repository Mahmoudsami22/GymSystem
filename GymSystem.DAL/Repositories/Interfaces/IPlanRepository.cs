using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAll( bool isTracked , CancellationToken ct = default);
        Task<Plan?> GetById(int id , CancellationToken ct = default);
        void Add(Plan plan);
        void Update(Plan plan);
        void Delete(int id);
        Task<int> completeAsync();

    }
}
