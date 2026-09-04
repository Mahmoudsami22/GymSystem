using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSesionsWithTrinerAndGategoryAsync(CancellationToken ct );
        Task<Session?> GetSessionWithTrinerAndGategoryByIdAsync(int sessionId, CancellationToken ct);
        Task<int> GetCountOfBookedSlotAsync(int sessionId, CancellationToken ct);
    }
}
