using FastPCB.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Yeni kullanici olusturur ve ilk JWT tokenini doner.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName);
                var token = await _authService.GenerateJwtTokenAsync(user);

                return Created(string.Empty, AuthResponse.FromUser(user, token));
            }
            catch (AuthConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    title: "Kayit islemi sirasinda veritabani hatasi olustu.",
                    detail: ex.InnerException?.Message ?? ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Kayit islemi tamamlanamadi.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Kullanici girisi yapar ve dogrulanmis JWT tokenini doner.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Email, request.Password);
                var token = await _authService.GenerateJwtTokenAsync(user);

                return Ok(AuthResponse.FromUser(user, token));
            }
            catch (AuthValidationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Giris islemi tamamlanamadi.",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public static AuthResponse FromUser(FastPCB.Models.User user, string token)
        {
            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                Token = token
            };
        }
    }
}
