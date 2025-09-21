using AutoMapper;
using TaskManager.Application.DTO;
using TaskManager.Application.DTO.DTO;
using TaskManager.Application.DTO.ViewModel;
using TaskManager.Domain.Entities.Models;

namespace TaskManager.Transversal.Mapper
{
    public class MappingsProfile : Profile
    {

        public MappingsProfile()
        {
            CreateMap<TaskItemDTO, TaskItemME>();
            CreateMap<TaskItemME, TaskItemVM>()
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : string.Empty));

            CreateMap<TaskItemDTO, TaskItemME>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<TaskStatusME, TaskStatusVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<TaskStatusDTO, TaskStatusME>();
            CreateMap<TaskStatusME, TaskStatusDTO>();
        }
    }
}
