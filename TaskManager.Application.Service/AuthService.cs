using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManager.Application.DTO.DTO.Identity;
using TaskManager.Application.Interface;
using TaskManager.Domain.Entities.Models.Identity;

namespace TaskManager.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           CustomAuthenticationStateProvider authStateProvider,
                           RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authStateProvider = authStateProvider;
            _roleManager = roleManager;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                return new LoginResponseDTO { IsSuccessful = false, Message = "Usuario o contraseña incorrectos" };

            await _signInManager.SignOutAsync();

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginRequest.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                _authStateProvider.NotifyUserAuthentication(user.UserName);

                return new LoginResponseDTO
                {
                    IsSuccessful = true,
                    Message = "Login exitoso"
                };
            }

            if (result.IsLockedOut)
                return new LoginResponseDTO { IsSuccessful = false, Message = "Cuenta bloqueada temporalmente" };

            if (result.RequiresTwoFactor)
                return new LoginResponseDTO { IsSuccessful = false, Message = "Se requiere autenticación de dos factores" };

            return new LoginResponseDTO { IsSuccessful = false, Message = "Usuario o contraseña incorrectos" };
        }

        public async Task<ApplicationUser> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("El email ya está registrado");
            }

            var user = new ApplicationUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al crear usuario: {errors}");
            }

            // Rol por defecto
            const string defaultRole = "Asesor";

            // Validar que exista el rol
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                throw new InvalidOperationException($"El rol '{defaultRole}' no existe.");
            }

            // Asignar el rol
            var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al asignar el rol: {errors}");
            }

            return user;
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _authStateProvider.NotifyUserLogout();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUserAuthenticatedAsync(ClaimsPrincipal user)
        {
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return false;

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            var appUser = await _userManager.FindByIdAsync(userId);
            return appUser != null;
        }
    }
}