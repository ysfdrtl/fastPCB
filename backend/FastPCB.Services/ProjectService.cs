using FastPCB.Data;
using FastPCB.Models;
using FastPCB.Services.Infrastructure.Cache;
using FastPCB.Services.Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FastPCB.Services
{
    public interface IProjectService
    {
        Task<Project> CreateProjectAsync(int userId, string? title = null, string? description = null);
        Task<Project> GetProjectAsync(int projectId);
        Task<PagedResult<Project>> GetProjectsAsync(ProjectQueryOptions? query = null);
        Task<List<Project>> GetUserProjectsAsync(int userId);
        Task<Project> UpdateProjectDetailsAsync(int projectId, ProjectDetails details);
        Task<Project> UploadProjectFileAsync(int projectId, Stream fileStream, string originalFileName, long fileLength, CancellationToken cancellationToken = default);
        Task<bool> DeleteProjectAsync(int projectId);
    }

    public class ProjectService : IProjectService
    {
        private readonly FastPCBContext _context;
        private readonly IProjectFileStorage _projectFileStorage;
        private readonly ICacheService _cacheService;
        private readonly IKafkaProducer _kafkaProducer;
        private readonly KafkaOptions _kafkaOptions;

        public ProjectService(
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

        // Yeni proje kaydini olusturur ve ilk taslak durumunda veritabanina yazar.
        public async Task<Project> CreateProjectAsync(int userId, string? title = null, string? description = null)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ProjectNotFoundException("Proje olusturulacak kullanici bulunamadi.");
            }

            if (!string.IsNullOrWhiteSpace(title) && title.Trim().Length > 50)
            {
                throw new ProjectValidationException("Proje basligi en fazla 50 karakter olabilir.");
            }

            var project = new Project
            {
                UserId = userId,
                Title = string.IsNullOrWhiteSpace(title) ? GenerateProjectTitle() : title.Trim(),
                Description = string.IsNullOrWhiteSpace(description)
                    ? "PCB projesi olusturuldu. Detaylar bekleniyor."
                    : description.Trim(),
                FilePath = string.Empty,
                Status = ProjectStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            await _cacheService.RemoveByPrefixAsync(CacheKeys.ProjectPrefix);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);
            await _kafkaProducer.PublishAsync(
                _kafkaOptions.Topics.Projects,
                project.Id.ToString(),
                new ProjectCreated(project.Id, project.UserId, project.Title, project.CreatedAt));

            return await GetProjectAsync(project.Id);
        }

        // Tek bir projeyi sahibiyle birlikte getirir.
        public async Task<Project> GetProjectAsync(int projectId)
        {
            var project = await _cacheService.GetOrCreateAsync(
                CacheKeys.ProjectDetail(projectId),
                async () => await _context.Projects
                    .Include(p => p.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == projectId));

            if (project is null)
            {
                throw new ProjectNotFoundException("Proje bulunamadi.");
            }

            return project;
        }

        // Filtreleme ve sayfalama kurallarina gore proje listesini dondurur.
        public async Task<PagedResult<Project>> GetProjectsAsync(ProjectQueryOptions? query = null)
        {
            query ??= new ProjectQueryOptions();
            NormalizeQuery(query);

            var cacheKey = CacheKeys.ProjectList(BuildProjectQueryFingerprint(query));
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var projectsQuery = _context.Projects
                    .Include(p => p.User)
                    .AsNoTracking()
                    .AsQueryable();

                if (query.UserId.HasValue)
                {
                    projectsQuery = projectsQuery.Where(p => p.UserId == query.UserId.Value);
                }

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var normalizedSearch = query.Search.Trim();
                    projectsQuery = projectsQuery.Where(p =>
                        EF.Functions.Like(p.Title, $"%{normalizedSearch}%") ||
                        EF.Functions.Like(p.Description, $"%{normalizedSearch}%") ||
                        (p.Material != null && EF.Functions.Like(p.Material, $"%{normalizedSearch}%")));
                }

                if (query.Status.HasValue)
                {
                    projectsQuery = projectsQuery.Where(p => p.Status == query.Status.Value);
                }

                if (query.HasFile.HasValue)
                {
                    projectsQuery = query.HasFile.Value
                        ? projectsQuery.Where(p => !string.IsNullOrWhiteSpace(p.FilePath))
                        : projectsQuery.Where(p => string.IsNullOrWhiteSpace(p.FilePath));
                }

                var totalCount = await projectsQuery.CountAsync();
                var items = await projectsQuery
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                return new PagedResult<Project>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize
                };
            });
        }

        // Belirli bir kullaniciya ait tum projeleri listeler.
        public async Task<List<Project>> GetUserProjectsAsync(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ProjectNotFoundException("Kullanici bulunamadi.");
            }

            return await _context.Projects
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        // Projenin teknik alanlarini gunceller.
        public async Task<Project> UpdateProjectDetailsAsync(int projectId, ProjectDetails details)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
            {
                throw new ProjectNotFoundException("Detaylari guncellenecek proje bulunamadi.");
            }

            ValidateProjectDetails(details);

            project.Layers = details.Layers;
            project.Material = details.Material.Trim();
            project.MinDistance = details.MinDistance;
            project.Quantity = details.Quantity;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _cacheService.RemoveByPrefixAsync(CacheKeys.ProjectPrefix);

            return await GetProjectAsync(project.Id);
        }

        // Yuklenen dosyayi fiziksel depoya kaydeder ve proje kaydindaki yol bilgisini gunceller.
        public async Task<Project> UploadProjectFileAsync(
            int projectId,
            Stream fileStream,
            string originalFileName,
            long fileLength,
            CancellationToken cancellationToken = default)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
            if (project is null)
            {
                throw new ProjectNotFoundException("Dosya yuklenecek proje bulunamadi.");
            }

            var storedFile = await _projectFileStorage.SaveProjectFileAsync(
                projectId,
                fileStream,
                originalFileName,
                fileLength,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(project.FilePath))
            {
                await _projectFileStorage.DeleteProjectFileAsync(project.FilePath, cancellationToken);
            }

            project.FilePath = storedFile.RelativePath;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.ProjectPrefix, cancellationToken);

            return await GetProjectAsync(project.Id);
        }

        // Projeyi ve varsa ona ait yuklu dosya klasorunu siler.
        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
            {
                throw new ProjectNotFoundException("Silinecek proje bulunamadi.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            await _projectFileStorage.DeleteProjectDirectoryAsync(projectId);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.ProjectPrefix);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.AdminDashboardPrefix);
            return true;
        }

        // Baslik girilmediginde benzersiz bir proje basligi uretir.
        private static string GenerateProjectTitle()
        {
            return $"PROJECT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }

        // Teknik detaylarin minimum dogrulamalarini yapar.
        private static void ValidateProjectDetails(ProjectDetails details)
        {
            if (details.Layers <= 0)
            {
                throw new ProjectValidationException("Katman sayisi 0'dan buyuk olmalidir.");
            }

            if (details.Quantity <= 0)
            {
                throw new ProjectValidationException("Adet 0'dan buyuk olmalidir.");
            }

            if (string.IsNullOrWhiteSpace(details.Material))
            {
                throw new ProjectValidationException("Malzeme alani bos birakilamaz.");
            }

            if (details.MinDistance <= 0)
            {
                throw new ProjectValidationException("Minimum mesafe 0'dan buyuk olmalidir.");
            }
        }

        // Listeleme parametrelerini guvenli varsayilanlara ceker.
        private static void NormalizeQuery(ProjectQueryOptions query)
        {
            if (query.Page <= 0)
            {
                query.Page = 1;
            }

            if (query.PageSize <= 0)
            {
                query.PageSize = 10;
            }

            if (query.PageSize > 50)
            {
                query.PageSize = 50;
            }
        }

        private static string BuildProjectQueryFingerprint(ProjectQueryOptions query)
        {
            var search = string.IsNullOrWhiteSpace(query.Search)
                ? "none"
                : Uri.EscapeDataString(query.Search.Trim().ToLowerInvariant());

            return $"user-{query.UserId?.ToString() ?? "all"}:search-{search}:status-{query.Status?.ToString() ?? "all"}:file-{query.HasFile?.ToString() ?? "all"}:page-{query.Page}:size-{query.PageSize}";
        }
    }

    public class ProjectDetails
    {
        public int Layers { get; set; }
        public string Material { get; set; } = string.Empty;
        public double MinDistance { get; set; }
        public int Quantity { get; set; }
    }

    public sealed class ProjectQueryOptions
    {
        public int? UserId { get; set; }
        public string? Search { get; set; }
        public ProjectStatus? Status { get; set; }
        public bool? HasFile { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public interface IProjectFileStorage
    {
        Task<StoredProjectFile> SaveProjectFileAsync(
            int projectId,
            Stream fileStream,
            string originalFileName,
            long fileLength,
            CancellationToken cancellationToken = default);

        Task DeleteProjectFileAsync(string? relativePath, CancellationToken cancellationToken = default);
        Task DeleteProjectDirectoryAsync(int projectId, CancellationToken cancellationToken = default);
    }

    public sealed class StoredProjectFile
    {
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
    }

    public sealed class ProjectNotFoundException : Exception
    {
        public ProjectNotFoundException(string message) : base(message)
        {
        }
    }

    public sealed class ProjectValidationException : Exception
    {
        public ProjectValidationException(string message) : base(message)
        {
        }
    }
}
