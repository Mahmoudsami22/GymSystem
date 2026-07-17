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
    public class TrainerServices : ITrainerServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TrainerServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainerAsync(CancellationToken ct = default)
        {
            var Trainers = await unitOfWork.GetRepository<Trainer>().GetAll(false, ct);
            if (!Trainers.Any())
            {
                return [];
            }
            var TrainersViewModels = mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerViewModel>>(Trainers);
            return TrainersViewModels;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(trainerId, ct);
            if (trainer == null)
            {
                return null;
            }
            var trainerVM = mapper.Map<Trainer, TrainerViewModel>(trainer);
            return trainerVM;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(trainerId, ct);
            if (trainer is null)
            {
                return null;
            }
            return mapper.Map<Trainer, TrainerToUpdateViewModel>(trainer);
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExsists = await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            var phoneExsists = await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);
            if (emailExsists || phoneExsists)
            {
                return Result.Validation("Error"); 
            }
            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                },
                Specialty = model.Specialties,
            };
            unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Trainer"); ;
        }
        public async Task<Result> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(id, ct);
            if (trainer is null)
            {
                return Result.NotFound("Trainer Not Found"); 
            }
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != id)) return Result.Fail("Error");
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != id)) return Result.Fail("Error");
            mapper.Map(model, trainer);

            unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Trainer"); ;
        }
        public async Task<Result> DeleteTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var HasfutrueSessions = await unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == trainerId && s.EndDate >
            DateTime.Now, ct);
            if (HasfutrueSessions)
            {
                return Result.NotFound("Trainer Not Found"); 
            }

            unitOfWork.GetRepository<Trainer>().Delete(trainerId);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete Trainer"); ;
        }

        public async Task<TrainerViewModel> GetTrainerById(int TrainerId, CancellationToken ct)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(TrainerId);

            return mapper.Map<Trainer, TrainerViewModel>(trainer);
        }
    }
}
