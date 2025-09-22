using TaskManager.Application.DTO;
using TaskManager.Application.DTO.DTO;
using TaskManager.Application.DTO.ViewModel;
using TaskManager.Application.Interface;
using TaskManager.Application.Service;
using TaskManager.Domain.Entities.Models;

namespace TaskManager.ServiceExtensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServicesLayer(this IServiceCollection services)
        {
            services.AddScoped<IAppService<TaskStatusDTO, TaskStatusVM>, AppService<TaskStatusME, TaskStatusDTO, TaskStatusVM>>();
            services.AddScoped<IAppService<TaskItemDTO, TaskItemVM>, AppService<TaskItemME, TaskItemDTO, TaskItemVM>>();

            return services;
        }
    }
}