using Fast.Core.Dtos.Auth;
using Fast.Core.Dtos.Common;
using Fast.Data.Context;
using Fast.Data.Entities;
using Fast.Business.Interfaces;

namespace Fast.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly FastDbContext _context;

        public AuthService(FastDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if email already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == registerDto.Email);
                if (existingUser != null)
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Email already registered",
                        StatusCode = 409
                    };
                }

                // Create new user
                var user = new User
                {
                    Email = registerDto.Email,
                    Password = HashPassword(registerDto.Password),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Phone = registerDto.Phone,
                    Address = registerDto.Address,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt
                };

                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = response,
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);
                if (user == null || !VerifyPassword(loginDto.Password, user.Password))
                {
                    return new ApiResponseDto<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        StatusCode = 401
                    };
                }

                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt
                };

                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<AuthResponseDto>
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<UserDto>> UpdateProfileAsync(UpdateProfileDto updateProfileDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(updateProfileDto.UserId);
                if (user == null)
                {
                    return new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = "User not found",
                        StatusCode = 404
                    };
                }

                user.FirstName = updateProfileDto.FirstName ?? user.FirstName;
                user.LastName = updateProfileDto.LastName ?? user.LastName;
                user.Phone = updateProfileDto.Phone ?? user.Phone;
                user.Address = updateProfileDto.Address ?? user.Address;
                user.Email = updateProfileDto.Email ?? user.Email;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var response = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = $"Update failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto<UserDto>
                    {
                        Success = false,
                        Message = "User not found",
                        StatusCode = 404
                    };
                }

                var response = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return new ApiResponseDto<UserDto>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = response,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = $"Retrieval failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponseDto> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Message = "User not found",
                        StatusCode = 404
                    };
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new ApiResponseDto
                {
                    Success = true,
                    Message = "User deleted successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = $"Deletion failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Simple password hashing using SHA256 (for demo - use BCrypt.Net in production)
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashOfInput = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                byte[] hashBytes = Convert.FromBase64String(hash);
                return hashOfInput.SequenceEqual(hashBytes);
            }
        }
    }
}
