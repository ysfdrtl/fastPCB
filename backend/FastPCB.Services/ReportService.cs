using FastPCB.Data;
using FastPCB.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.Services
{
    public interface IReportService
    {
        Task<Ticket> CreateReportAsync(int projectId, int userId, string reason, string? details);
        Task<List<Ticket>> GetUserReportsAsync(int userId);
    }

    public class ReportService : IReportService
    {
        private readonly FastPCBContext _context;

        public ReportService(FastPCBContext context)
        {
            _context = context;
        }

        // Kullanici adina proje raporu olusturur ve ayni proje icin tekrar acik rapor acilmasini engeller.
        public async Task<Ticket> CreateReportAsync(int projectId, int userId, string reason, string? details)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
            {
                throw new ReportNotFoundException("Raporlanacak proje bulunamadi.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ReportNotFoundException("Rapor olusturacak kullanici bulunamadi.");
            }

            if (project.UserId == userId)
            {
                throw new ReportValidationException("Kendi projenizi raporlayamazsiniz.");
            }

            var normalizedReason = NormalizeReason(reason);
            var normalizedDetails = NormalizeDetails(details);

            var existingOpenReport = await _context.Reports.AnyAsync(r =>
                r.ProjectId == projectId &&
                r.UserId == userId &&
                (r.Status == TicketStatus.Open || r.Status == TicketStatus.InProgress));

            if (existingOpenReport)
            {
                throw new ReportValidationException("Bu proje icin zaten acik bir raporunuz bulunuyor.");
            }

            var report = new Ticket
            {
                ProjectId = projectId,
                UserId = userId,
                Title = normalizedReason,
                Description = normalizedDetails,
                Status = TicketStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Response = string.Empty
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return await _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User)
                .AsNoTracking()
                .FirstAsync(r => r.Id == report.Id);
        }

        // Giris yapan kullanicinin gonderdigi raporlari proje bilgileriyle birlikte listeler.
        public async Task<List<Ticket>> GetUserReportsAsync(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ReportNotFoundException("Raporlari listelenecek kullanici bulunamadi.");
            }

            return await _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        // Rapor sebebini zorunlu alan ve uzunluk kurallarina gore normalize eder.
        private static string NormalizeReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ReportValidationException("Rapor sebebi bos birakilamaz.");
            }

            var normalizedReason = reason.Trim();
            if (normalizedReason.Length > 255)
            {
                throw new ReportValidationException("Rapor sebebi en fazla 255 karakter olabilir.");
            }

            return normalizedReason;
        }

        // Opsiyonel rapor aciklamasini bosluk ve uzunluk kurallarina gore hazirlar.
        private static string NormalizeDetails(string? details)
        {
            if (string.IsNullOrWhiteSpace(details))
            {
                return string.Empty;
            }

            var normalizedDetails = details.Trim();
            if (normalizedDetails.Length > 4000)
            {
                throw new ReportValidationException("Rapor aciklamasi en fazla 4000 karakter olabilir.");
            }

            return normalizedDetails;
        }
    }

    public sealed class ReportNotFoundException : Exception
    {
        public ReportNotFoundException(string message) : base(message)
        {
        }
    }

    public sealed class ReportValidationException : Exception
    {
        public ReportValidationException(string message) : base(message)
        {
        }
    }
}
