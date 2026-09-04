using AutoMapper;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace GymSystem.BLL.Services.Classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End Date Must Be After Start Date");

            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date Must be in the future"); ;

            var TrainerRepo = unitOfWork.GetRepository<Trainer>();

            var Trainer = await TrainerRepo.GetById(model.TrainerId, ct);

            if (Trainer is null) return Result.NotFound("Trainer Not Found");

            var CategoryRepo = unitOfWork.GetRepository<Category>();
            var Category = await CategoryRepo.GetById(model.CategoryId, ct);
            if (Category is null) return Result.NotFound("Category Not Found");

            var session = mapper.Map<CreateSessionViewModel, Session>(model);

            var SessionRepo = unitOfWork.GetRepository<Session>();

            SessionRepo.Add(session);

            var rowEffected = await unitOfWork.CompeleteAsync();

            return rowEffected > 0 ? Result.Ok() : Result.Fail("Failed to Create Session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct)
        {
            var sessions = await unitOfWork.SessionRepository.GetAllSesionsWithTrinerAndGategoryAsync(ct);
            if (!sessions.Any())
            {
                return [];
            }
            sessions = sessions.OrderByDescending(s => s.StartDate);
            var MappedSessions = mapper.Map<IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in MappedSessions)
            {
                session.AvailableSlots = session.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);
            }
            return MappedSessions;


        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAll(false, ct);

            return mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<SessionViewModel> GetSessionById(int sessionId, CancellationToken ct)
        {
            var session = await unitOfWork.GetRepository<Session>().GetById(sessionId);

            return mapper.Map<Session, SessionViewModel>(session);
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct)
        {
            var Session = await unitOfWork.SessionRepository.GetSessionWithTrinerAndGategoryByIdAsync(sessionId, ct);

            if (Session == null)
                return null;

            var mappedSession = mapper.Map<Session, SessionViewModel>(Session);

            mappedSession.AvailableSlots = mappedSession.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(mappedSession.Id,
            ct);

            return mappedSession;
        }

        public async Task<UpdateSessionViewModel> GetSessionToUpdateAsync(int sessionId, CancellationToken ct)
        {
            var Session = await unitOfWork.GetRepository<Session>().GetById(sessionId, ct);

            if (Session is null) return null;

            if (!await IsSessionValidForUpdateAsync(Session, ct)) return null;

            return mapper.Map<Session, UpdateSessionViewModel>(Session);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var Trainter = await unitOfWork.GetRepository<Trainer>().GetAll(false, ct);

            return mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(Trainter);
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct)
        {
            var repo = unitOfWork.GetRepository<Session>();//Session Repository

            var Session = await repo.GetById(sessionId, ct);

            if (Session is null) return Result.NotFound("Session Not Found");

            if (Session.EndDate >= DateTime.Now)
                return Result.Fail("Can not Delete a session that has not yet ended");

            var bookedCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(sessionId, ct);

            if (bookedCount > 0)
                return Result.Fail("Can not Delete a Session That has Bookings");

            repo.Delete(sessionId);

            var affectedRows = await unitOfWork.CompeleteAsync();

            return affectedRows > 0 ? Result.Ok() : Result.Fail("Failed to Remove Session");
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var SessionRepo = unitOfWork.GetRepository<Session>();

            var session = await SessionRepo.GetById(id, ct);

            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can not Edit a session that has already started");

            var bookedCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);

            if (bookedCount > 0)
                return Result.Fail("Can not Edit a session that has booked slots");

            if (model.EndDate <= model.StartDate) return Result.Validation("End Date Must Be After Start Date");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date Must be in the future");

            var TrainerRepo = unitOfWork.GetRepository<Trainer>();

            var Trainer = await TrainerRepo.GetById(model.TrainerId, ct);

            if (Trainer is null) return Result.NotFound("Trainer Not Found");

            session.UpdatedAt = DateTime.Now;

            mapper.Map(model, session);
            SessionRepo.Update(session);

            var EffecetedRows = await unitOfWork.CompeleteAsync();

            return EffecetedRows > 0 ? Result.Ok() : Result.Fail("Failed to Update Session");
        }

        private async Task<bool> IsSessionValidForUpdateAsync(Session session, CancellationToken ct)
        {

            if (session.StartDate <= DateTime.Now) return false;

            var booked = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);

            return booked == 0;
        }
    }
}
