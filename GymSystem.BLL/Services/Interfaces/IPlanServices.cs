using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlanAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsAsync(int planId, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);
        //post
        Task<Result> UpdatePlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
