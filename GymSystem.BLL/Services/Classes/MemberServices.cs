using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using GymSystemG03.BLL.ViewModels.MembersViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork unitOfWork;

        public MemberServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAll(false, ct);
            if (!members.Any())
            {
                return [];
            }
            var MemberViewModels = members.Select(m => new MemberViewModel()
            {
                Id = m.Id,
                Photo = m.Photo,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender.ToString()

            });
            return MemberViewModels;
        }
        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member == null)
            {
                return null;
            }
            var memberVM = new MemberViewModel()
            {

                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $" {member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City} ",
            };
            var ActiveMemberShip = await unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(mb => mb.MemberId == memberId && mb.EndDate > DateTime.Now, false, ct);
            if(ActiveMemberShip is not null)
            {
                var ActivePlan = await unitOfWork.GetRepository<Plan>().GetById(ActiveMemberShip.PlanId, ct);
                memberVM.PlanName = ActivePlan?.Name;
                memberVM.MembershipStartDate = ActiveMemberShip.CreatedAt.ToShortDateString();
                memberVM.MembershipEndDate = ActiveMemberShip.EndDate.ToShortDateString();
            }
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
            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City
            };
        }
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExsists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var phoneExsists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExsists || phoneExsists)
            {
                return false;
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
            unitOfWork.GetRepository<Member>().Add(member);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(id, ct);
            if (member is null)
            {
                return false;
            }
            if (await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id)) return false;
            if (await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id)) return false;
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;
            member.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Member>().Update(member);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0 ;
        }
        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member =  await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member is null)
            {
                return false;
            }
            var HasfutrueSessions = await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.EndDate >
            DateTime.Now,ct);
            if (HasfutrueSessions)
            {
                return false;
            }
            unitOfWork.GetRepository<Member>().Delete(memberId);
            var result = await unitOfWork.CompeleteAsync();
            return result > 0;
        }
    }
}
