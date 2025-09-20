using AutoMapper;
using TaskManager.Application.DTO.DTO;
using TaskManager.Application.DTO.ViewModel;
using TaskManager.Domain.Entities.Models;

namespace TaskManager.Transversal.Mapper
{
    public class MappingsProfile : Profile
    {

        public MappingsProfile()
        {
            // Form en vista
            CreateMap<TaskItemDTO, TaskItemME>();
            CreateMap<TaskItemME, TaskItemVM>();

            //DDL en vista
            //CreateMap<TaskStatusME, TaskStatusVM>();

            CreateMap<TaskItemME, TaskItemVM>()
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status.Name));
        }
    }
}
