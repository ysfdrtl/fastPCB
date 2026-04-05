using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FastPCB.Data;
using FastPCB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FastPCB.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string email, string password, string firstName, string lastName);
        Task<User> LoginAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(User user);
    }

    public class AuthService : IAuthService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;

        private readonly FastPCBContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(FastPCBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Yeni kullaniciyi olusturur ve sifresini guvenli sekilde hashleyerek veritabanina kaydeder.
        public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (existingUser is not null)
            {
                throw new AuthConflictException("Bu email adresi zaten kayitli.");
            }

            var user = new User
            {
                Email = normalizedEmail,
                PasswordHash = HashPassword(password),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Phone = string.Empty,
                Address = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Kullanici giris denemesini email ve sifreye gore dogrular.
        public async Task<User> LoginAsync(string email, string password)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user is null || !VerifyPassword(password, user.PasswordHash))
            {
                throw new AuthValidationException("Email veya sifre hatali.");
            }

            return user;
        }

        // Kimligi dogrulanmis kullanici icin JWT token uretir.
        public Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var secretKey = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expirationInMinutes = int.TryParse(jwtSection["ExpirationInMinutes"], out var parsedValue)
                ? parsedValue
                : 60;

            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey en az 32 karakter olmalidir.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                signingCredentials: credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        // Sifreyi PBKDF2 algoritmasi ile hash formatina cevirir.
        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        // Girilen sifrenin kayitli hash ile eslesip eslesmedigini kontrol eder.
        private static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split('.', 3);
                if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[1]);
                var expectedHash = Convert.FromBase64String(parts[2]);

                var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    public sealed class AuthConflictException : Exception
    {
        public AuthConflictException(string message) : base(message)
        {
        }
    }

    public sealed class AuthValidationException : Exception
    {
        public AuthValidationException(string message) : base(message)
        {
        }
    }
}
