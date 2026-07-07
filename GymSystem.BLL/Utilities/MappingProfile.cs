using AutoMapper;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Utilities
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            MapSession();
        }
        private void MapSession()
        {
            CreateMap<Session,SessionViewModel>()
                .ForMember(dest => dest.CategoryName,opt => opt.MapFrom(src =>src.Category.CategoryName))
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore()).ReverseMap();
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }
    }
}
