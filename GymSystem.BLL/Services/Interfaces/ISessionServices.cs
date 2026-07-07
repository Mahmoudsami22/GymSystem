using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ISessionServices
    {
        public Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct);
        public Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);
        Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct);
        Task<UpdateSessionViewModel> GetSessionToUpdateAsync(int sessionId, CancellationToken ct);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct);
        public Task<SessionViewModel> GetSessionById(int sessionId, CancellationToken ct);



    }
}
