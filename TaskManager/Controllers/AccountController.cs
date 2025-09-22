using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTO.DTO.Identity;
using TaskManager.Application.Interface;
using TaskManager.Application.Service;
using TaskManager.Domain.Entities.Models.Identity;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);
            if (response.IsSuccessful)
            {
                // Si hay ReturnUrl, redirigir ahí
                var returnUrl = Request.Query["ReturnUrl"].FirstOrDefault() ?? "/home";

                return Ok(new
                {
                    success = true,
                    message = response.Message,
                    redirectUrl = returnUrl
                });
            }
            else
                return Unauthorized(response.Message);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Logout exitoso" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            try
            {
                var user = await _authService.RegisterAsync(registerRequest);

                return Ok(new
                {
                    message = "Usuario creado correctamente",
                    userId = user.Id,
                    userName = user.UserName
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log aquí si quieres
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        [HttpGet("is-authenticated")]
        public async Task<IActionResult> IsAuthenticated()
        {
            // Pasa el ClaimsPrincipal actual al servicio
            var isAuthenticated = await _authService.IsUserAuthenticatedAsync(User);

            if (isAuthenticated)
                return Ok(new { message = "Usuario autenticado" });

            return Unauthorized(new { message = "Usuario no autenticado" });
        }
    }
}
