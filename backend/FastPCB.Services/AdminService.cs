using FastPCB.Data;
using FastPCB.Models;
using FastPCB.Services.Infrastructure.Cache;
using FastPCB.Services.Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FastPCB.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardStats> GetDashboardStatsAsync();
        Task<PagedResult<User>> GetUsersAsync(AdminUserQueryOptions? query = null);
        Task<User> GetUserAsync(int userId);
        Task<User> UpdateUserRoleAsync(int userId, UserRole role);
        Task DeleteUserAsync(int userId);
        Task<PagedResult<Project>> GetProjectsAsync(AdminProjectQueryOptions? query = null);
        Task<Project> UpdateProjectStatusAsync(int projectId, ProjectStatus status);
        Task DeleteProjectAsync(int projectId);
        Task<PagedResult<Ticket>> GetReportsAsync(AdminReportQueryOptions? query = null);
        Task<Ticket> UpdateReportAsync(int reportId, TicketStatus status, string? response);
    }

    public class AdminService : IAdminService
    {
        private readonly FastPCBContext _context;
        private readonly IProjectFileStorage _projectFileStorage;
        private readonly ICacheService _cacheService;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly KafkaOptions _kafkaOptions;

        public AdminService(
            FastPCBContext context,
            IProjectFileStorage projectFileStorage,
            ICacheService cacheService,
            IKafkaProducer kafkaProducer,
            IOptions<KafkaOptions> kafkaOptions)
        {
            _context = context;
            _projectFileStorage = projectFileStorage;
            _cacheService = cacheService;
            _kafkaProducer = kafkaProducer;
            _kafkaOptions = kafkaOptions.Value;
        }

        public async Task<AdminDashboardStats> GetDashboardStatsAsync()
        {
            return await _cacheService.GetOrCreateAsync(CacheKeys.AdminDashboard(), async () => new AdminDashboardStats
            {
                TotalUsers = await _context.Users.CountAsync(),
                AdminUsers = await _context.Users.CountAsync(u => u.Role == UserRole.Admin),
                TotalProjects = await _context.Projects.CountAsync(),
                DraftProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Draft),
                PublishedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Published),
                FeaturedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Featured),
                ArchivedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Archived),
                RemovedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Removed),
                OpenReports = await _context.Reports.CountAsync(r => r.Status == TicketStatus.Open),
                InProgressReports = await _context.Reports.CountAsync(r => r.Status == TicketStatus.InProgress),
                ResolvedReports = await _context.Reports.CountAsync(r => r.Status == TicketStatus.Resolved),
                ClosedReports = await _context.Reports.CountAsync(r => r.Status == TicketStatus.Closed)
            });
        }

        public async Task<PagedResult<User>> GetUsersAsync(AdminUserQueryOptions? query = null)
        {
            query ??= new AdminUserQueryOptions();
            NormalizePaging(query);

            var usersQuery = _context.Users.AsNoTracking().AsQueryable();

            if (query.Role.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.Role == query.Role.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                usersQuery = usersQuery.Where(u =>
                    EF.Functions.Like(u.Email, $"%{search}%") ||
                    EF.Functions.Like(u.FirstName, $"%{search}%") ||
                    EF.Functions.Like(u.LastName, $"%{search}%"));
            }

            var totalCount = await usersQuery.CountAsync();
            var users = await usersQuery
                .OrderByDescending(u => u.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return ToPagedResult(users, totalCount, query);
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                throw new AdminNotFoundException("Kullanici bulunamadi.");
            }

            return user;
        }

        public async Task<User> UpdateUserRoleAsync(int userId, UserRole role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                throw new AdminNotFoundException("Rolu guncellenecek kullanici bulunamadi.");
            }

            var oldRole = user.Role;
            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);

            await _kafkaProducer.PublishAsync(
                _kafkaOptions.Topics.Users,
                user.Id.ToString(),
                new UserRoleChanged(user.Id, oldRole, user.Role, user.UpdatedAt));

            return await GetUserAsync(user.Id);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                throw new AdminNotFoundException("Silinecek kullanici bulunamadi.");
            }

            var projectIds = await _context.Projects
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);

            foreach (var projectId in projectIds)
            {
                await _projectFileStorage.DeleteProjectDirectoryAsync(projectId);
            }
        }

        public async Task<PagedResult<Project>> GetProjectsAsync(AdminProjectQueryOptions? query = null)
        {
            query ??= new AdminProjectQueryOptions();
            NormalizePaging(query);

            var projectsQuery = _context.Projects
                .Include(p => p.User)
                .AsNoTracking()
                .AsQueryable();

            if (query.UserId.HasValue)
            {
                projectsQuery = projectsQuery.Where(p => p.UserId == query.UserId.Value);
            }

            if (query.Status.HasValue)
            {
                projectsQuery = projectsQuery.Where(p => p.Status == query.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                projectsQuery = projectsQuery.Where(p =>
                    EF.Functions.Like(p.Title, $"%{search}%") ||
                    EF.Functions.Like(p.Description, $"%{search}%"));
            }

            var totalCount = await projectsQuery.CountAsync();
            var projects = await projectsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return ToPagedResult(projects, totalCount, query);
        }

        public async Task<Project> UpdateProjectStatusAsync(int projectId, ProjectStatus status)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
            {
                throw new AdminNotFoundException("Durumu guncellenecek proje bulunamadi.");
            }

            project.Status = status;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _cacheService.RemoveByPrefixAsync(CacheKeys.ProjectPrefix);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);

            return await _context.Projects
                .Include(p => p.User)
                .AsNoTracking()
                .FirstAsync(p => p.Id == project.Id);
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
            {
                throw new AdminNotFoundException("Silinecek proje bulunamadi.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            await _projectFileStorage.DeleteProjectDirectoryAsync(projectId);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.ProjectPrefix);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);
        }

        public async Task<PagedResult<Ticket>> GetReportsAsync(AdminReportQueryOptions? query = null)
        {
            query ??= new AdminReportQueryOptions();
            NormalizePaging(query);

            var reportsQuery = _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User)
                .AsNoTracking()
                .AsQueryable();

            if (query.ProjectId.HasValue)
            {
                reportsQuery = reportsQuery.Where(r => r.ProjectId == query.ProjectId.Value);
            }

            if (query.UserId.HasValue)
            {
                reportsQuery = reportsQuery.Where(r => r.UserId == query.UserId.Value);
            }

            if (query.Status.HasValue)
            {
                reportsQuery = reportsQuery.Where(r => r.Status == query.Status.Value);
            }

            var totalCount = await reportsQuery.CountAsync();
            var reports = await reportsQuery
                .OrderByDescending(r => r.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return ToPagedResult(reports, totalCount, query);
        }

        public async Task<Ticket> UpdateReportAsync(int reportId, TicketStatus status, string? response)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
            if (report is null)
            {
                throw new AdminNotFoundException("Guncellenecek rapor bulunamadi.");
            }

            var oldStatus = report.Status;
            report.Status = status;
            report.Response = NormalizeResponse(response);
            report.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);

            var updatedReport = await _context.Reports
                .Include(r => r.Project)
                .Include(r => r.User)
                .AsNoTracking()
                .FirstAsync(r => r.Id == report.Id);

            await _kafkaProducer.PublishAsync(
                _kafkaOptions.Topics.Reports,
                updatedReport.Id.ToString(),
                new ReportStatusChanged(updatedReport.Id, oldStatus, updatedReport.Status, updatedReport.UpdatedAt));

            return updatedReport;
        }

        private static string NormalizeResponse(string? response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return string.Empty;
            }

            var normalizedResponse = response.Trim();
            if (normalizedResponse.Length > 4000)
            {
                throw new AdminValidationException("Rapor cevabi en fazla 4000 karakter olabilir.");
            }

            return normalizedResponse;
        }

        private static void NormalizePaging(AdminPagedQueryOptions query)
        {
            if (query.Page <= 0)
            {
                query.Page = 1;
            }

            if (query.PageSize <= 0)
            {
                query.PageSize = 10;
            }

            if (query.PageSize > 100)
            {
                query.PageSize = 100;
            }
        }

        private static PagedResult<T> ToPagedResult<T>(IReadOnlyList<T> items, int totalCount, AdminPagedQueryOptions query)
        {
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }

    public class AdminPagedQueryOptions
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class AdminUserQueryOptions : AdminPagedQueryOptions
    {
        public string? Search { get; set; }
        public UserRole? Role { get; set; }
    }

    public sealed class AdminProjectQueryOptions : AdminPagedQueryOptions
    {
        public int? UserId { get; set; }
        public string? Search { get; set; }
        public ProjectStatus? Status { get; set; }
    }

    public sealed class AdminReportQueryOptions : AdminPagedQueryOptions
    {
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public TicketStatus? Status { get; set; }
    }

    public sealed class AdminDashboardStats
    {
        public int TotalUsers { get; set; }
        public int AdminUsers { get; set; }
        public int TotalProjects { get; set; }
        public int DraftProjects { get; set; }
        public int PublishedProjects { get; set; }
        public int FeaturedProjects { get; set; }
        public int ArchivedProjects { get; set; }
        public int RemovedProjects { get; set; }
        public int OpenReports { get; set; }
        public int InProgressReports { get; set; }
        public int ResolvedReports { get; set; }
        public int ClosedReports { get; set; }
    }

    public sealed class AdminNotFoundException : Exception
    {
        public AdminNotFoundException(string message) : base(message)
        {
        }
    }

    public sealed class AdminValidationException : Exception
    {
        public AdminValidationException(string message) : base(message)
        {
        }
    }
}
