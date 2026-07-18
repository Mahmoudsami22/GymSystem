using AutoMapper;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAttachementServices attachementServices;

        public MemberServices(IUnitOfWork unitOfWork , IMapper mapper , IAttachementServices attachementServices)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.attachementServices = attachementServices;
        }
        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAll(false, ct);
            if (!members.Any())
            {
                return [];
            }
            var MembersViewModels = mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return MembersViewModels;
        }
        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member == null)
            {
                return null;
            }
            //var memberVM = new MemberViewModel()
            //{

            //    Name = member.Name,
            //    Email = member.Email,
            //    Phone = member.Phone,
            //    Photo = member.Photo,
            //    Gender = member.Gender.ToString(),
            //    DateOfBirth = member.DateOfBirth.ToShortDateString(),
            //    Address = $" {member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City} ",
            //};
            //var ActiveMemberShip = await unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(mb => mb.MemberId == memberId && mb.EndDate > DateTime.Now, false, ct);
            //if(ActiveMemberShip is not null)
            //{
            //    var ActivePlan = await unitOfWork.GetRepository<Plan>().GetById(ActiveMemberShip.PlanId, ct);
            //    memberVM.PlanName = ActivePlan?.Name;
            //    memberVM.MembershipStartDate = ActiveMemberShip.CreatedAt.ToShortDateString();
            //    memberVM.MembershipEndDate = ActiveMemberShip.EndDate.ToShortDateString();
            //}
            var memberVM = mapper.Map<Member, MemberViewModel>(member);
            return memberVM;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(hr => hr.MemberId == memberId, false, ct);
            if (record is null)
                return null;

            return new HealthRecordViewModel()
            {
                Height = record.Height,
                Weight = record.Weight,
                BloodType = record.BloodType,
                Note = record.Note,
            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member is null)
                return null;
            return mapper.Map<Member, MemberToUpdateViewModel>(member);
        }
        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExsists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var phoneExsists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExsists || phoneExsists)
            {
                return Result.Validation("Error");
            }
            var member = new Member()
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
                HealthRecord = new HealthRecord()
                {
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note,
                }

            };
            var NewPhotoName = await attachementServices.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MemberPictures", ct);
            if (string.IsNullOrEmpty(NewPhotoName)) return Result.Fail("NOT Photo");

            member.Photo = NewPhotoName;



            unitOfWork.GetRepository<Member>().Add(member);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Member"); ;
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(id, ct);
            if (member is null)
            {
                return Result.NotFound("Member Not Found");
            }
            if (await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id)) return Result.Validation("Error Valid");
            if (await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id)) return Result.Validation("Error Valid");
            mapper.Map(model, member);

            unitOfWork.GetRepository<Member>().Update(member);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member"); ;
        }
        public async Task<Result> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member =  await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member is null)
            {
                return Result.NotFound("Member Not Found");
            }
            var HasfutrueSessions = await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.EndDate >
            DateTime.Now,ct);
            if (HasfutrueSessions)
            {
                return Result.Fail("Error");
            }
            unitOfWork.GetRepository<Member>().Delete(memberId);
            if (member.Photo is not null)
            {
                attachementServices.Delete(member.Photo, "MemberPictures");
            }
                
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Member"); ;
        }

        public async Task<MemberViewModel> GetMenberById(int memberId, CancellationToken ct)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId);

            return mapper.Map<Member, MemberViewModel>(member);
        }
    }
}
