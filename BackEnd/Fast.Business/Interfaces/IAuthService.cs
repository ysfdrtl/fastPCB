using Fast.Core.Dtos.Auth;
using Fast.Core.Dtos.Common;

namespace Fast.Business.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponseDto<UserDto>> UpdateProfileAsync(UpdateProfileDto updateProfileDto);
        Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId);
        Task<ApiResponseDto> DeleteUserAsync(int userId);
    }
}
