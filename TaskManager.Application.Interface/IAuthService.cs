using System.Security.Claims;
using TaskManager.Application.DTO.DTO.Identity;
using TaskManager.Domain.Entities.Models.Identity;

namespace TaskManager.Application.Interface
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
        Task<ApplicationUser> RegisterAsync(RegisterRequestDTO registerRequest);
        Task<bool> LogoutAsync();
        Task<bool> IsUserAuthenticatedAsync(ClaimsPrincipal user);
    }
}