using Microsoft.AspNetCore.Mvc;
using Fast.Core.Dtos.Auth;
using Fast.Business.Interfaces;

namespace Fast.API.Controllers
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

        /// <summary>
        /// POST /api/auth/register - Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/auth/login - Login user and get token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// PUT /api/auth/profile - Update user profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var result = await _authService.UpdateProfileAsync(updateProfileDto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// GET /api/auth/user/{userId} - Get user details by ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var result = await _authService.GetUserByIdAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// DELETE /api/auth/user/{userId} - Delete user account
        /// </summary>
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {
            var result = await _authService.DeleteUserAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
