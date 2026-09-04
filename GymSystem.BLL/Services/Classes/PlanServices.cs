using AutoMapper;
using AutoMapper.Execution;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModels;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Enums;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GymSystem.BLL.Services.Classes
{
    public class PlanServices : IPlanServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PlanServices(IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlanAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAll(false, ct);
            if (!plans.Any())
            {
                return [];
            }
            var plansViewModels = mapper.Map<IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);
            return plansViewModels;
        }

        public async Task<PlanViewModel?> GetPlanDetailsAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
            {
                return null;
            }
            var planVM = mapper.Map<Plan, PlanViewModel>(plan);
            return planVM;
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan is null)
            {
                return null;
            }
            return mapper.Map<Plan, UpdatePlanViewModel>(plan);
        }

        public async Task<Result> UpdatePlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(id, ct);
            if (plan is null)
            {
                return Result.NotFound("Plan Not Found");
            }
            mapper.Map(model, plan);

            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Plan"); ;
        }
    }
}
