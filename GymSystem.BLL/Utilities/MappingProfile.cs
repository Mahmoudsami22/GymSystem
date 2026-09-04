using AutoMapper;
using GymSystem.BLL.ViewModels.MembersViewModels;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapSession();
            MapTrainer();
            MapMember();
            MapPlan();
        }
        private void MapSession()
        {
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore()).ReverseMap();
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }
        private void MapTrainer()
        {
            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => src.Specialty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                src.Address != null
                ? $"{src.Address.BuildingNumber} {src.Address.Street}, {src.Address.City}"
                : string.Empty))


            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src =>
                src.DateOfBirth.ToString("yyyy-MM-dd")))


            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                src.Gender.ToString())).ReverseMap();
            CreateMap<CreateTrainerViewModel, Trainer>();
            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ReverseMap()
                .ForPath(dest => dest.Address.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNumber))
                .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City))
                .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Street));


        }
        private void MapMember()
        {
            CreateMap<Member, MemberViewModel>()
            // 1. تحويل الـ Address المكون من (رقم، شارع، مدينة) إلى String واحد
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                src.Address != null
                ? $"{src.Address.BuildingNumber} {src.Address.Street}, {src.Address.City}"
                : string.Empty))

            // 2. تحويل الـ DateOfBirth من DateOnly إلى String
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src =>
                src.DateOfBirth.ToString("yyyy-MM-dd")))

            // 3. تحويل الـ Enum (Gender) إلى String
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                src.Gender.ToString()))

            // 4. جلب اسم الـ Plan من أحدث اشتراك
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src =>
                src.MemberShips != null && src.MemberShips.Any()
                ? src.MemberShips.OrderByDescending(m => m.EndDate).FirstOrDefault()!.Plan.Name
                : "No Active Plan"))

            // 5. حساب تاريخ البداية (تاريخ النهاية مطروح منه أيام مدة الـ Plan)
            .ForMember(dest => dest.MembershipStartDate, opt => opt.MapFrom(src =>
                src.MemberShips != null && src.MemberShips.Any()
                ? src.MemberShips.OrderByDescending(m => m.EndDate).FirstOrDefault()!.EndDate
                    .AddDays(-src.MemberShips.OrderByDescending(m => m.EndDate).FirstOrDefault()!.Plan.Duration)
                    .ToString("yyyy-MM-dd")
                : null))

            // 6. جلب تاريخ نهاية أحدث اشتراك وتحويله لـ String
            .ForMember(dest => dest.MembershipEndDate, opt => opt.MapFrom(src =>
                src.MemberShips != null && src.MemberShips.Any()
                ? src.MemberShips.OrderByDescending(m => m.EndDate).FirstOrDefault()!.EndDate.ToString("yyyy-MM-dd")
                : null));
            CreateMap<CreateMemberViewModel, Member>();
            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ReverseMap()
                .ForPath(dest => dest.Address.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNumber))
                .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City))
                .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Street));


        }
        private void MapPlan()
        {
            CreateMap<Plan, PlanViewModel>()
       .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.Duration)).ReverseMap();

            CreateMap<Plan, UpdatePlanViewModel>()
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.Duration))
                .ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlanName))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.DurationDays));
        }
    }
}
