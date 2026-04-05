using FastPCB.Services;

namespace FastPCB.API.Services;

public sealed class LocalProjectFileStorage : IProjectFileStorage
{
    private static readonly HashSet<string> DefaultAllowedExtensions =
    [
        ".zip",
        ".rar",
        ".7z",
        ".kicad_pcb",
        ".brd",
        ".gbr",
        ".ger",
        ".gerber",
        ".png",
        ".jpg",
        ".jpeg",
        ".pdf"
    ];

    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public LocalProjectFileStorage(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<StoredProjectFile> SaveProjectFileAsync(
        int projectId,
        Stream fileStream,
        string originalFileName,
        long fileLength,
        CancellationToken cancellationToken = default)
    {
        if (fileLength <= 0)
        {
            throw new ProjectValidationException("Yuklenecek dosya bos olamaz.");
        }

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ProjectValidationException("Dosya adi okunamadi.");
        }

        var extension = Path.GetExtension(originalFileName);
        var allowedExtensions = GetAllowedExtensions();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ProjectValidationException("Bu dosya uzantisi desteklenmiyor.");
        }

        var maxFileSizeInMb = _configuration.GetValue<int?>("FileStorage:MaxFileSizeInMb") ?? 25;
        var maxFileSizeInBytes = maxFileSizeInMb * 1024L * 1024L;
        if (fileLength > maxFileSizeInBytes)
        {
            throw new ProjectValidationException($"Dosya boyutu {maxFileSizeInMb} MB limitini asamaz.");
        }

        var uploadsRoot = ResolveUploadsRoot();
        var projectDirectory = Path.Combine(uploadsRoot, projectId.ToString());
        Directory.CreateDirectory(projectDirectory);

        var safeExtension = extension.ToLowerInvariant();
        var storedFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{safeExtension}";
        var absolutePath = Path.Combine(projectDirectory, storedFileName);

        await using var targetStream = File.Create(absolutePath);
        await fileStream.CopyToAsync(targetStream, cancellationToken);

        return new StoredProjectFile
        {
            OriginalFileName = Path.GetFileName(originalFileName),
            StoredFileName = storedFileName,
            RelativePath = $"/uploads/projects/{projectId}/{storedFileName}"
        };
    }

    public Task DeleteProjectFileAsync(string? relativePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        const string uploadsPrefix = "/uploads/projects/";
        if (!relativePath.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        var relativeFilePath = relativePath[uploadsPrefix.Length..]
            .Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.Combine(ResolveUploadsRoot(), relativeFilePath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }

    public Task DeleteProjectDirectoryAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var projectDirectory = Path.Combine(ResolveUploadsRoot(), projectId.ToString());
        if (Directory.Exists(projectDirectory))
        {
            Directory.Delete(projectDirectory, recursive: true);
        }

        return Task.CompletedTask;
    }

    private HashSet<string> GetAllowedExtensions()
    {
        var configuredExtensions = _configuration
            .GetSection("FileStorage:AllowedExtensions")
            .Get<string[]>();

        var source = configuredExtensions is { Length: > 0 }
            ? configuredExtensions
            : [.. DefaultAllowedExtensions];

        return source
            .Where(extension => !string.IsNullOrWhiteSpace(extension))
            .Select(extension => extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private string ResolveUploadsRoot()
    {
        var configuredPath = _configuration["FileStorage:ProjectUploadsPath"];
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.Combine(_environment.ContentRootPath, "uploads", "projects");
        }

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(_environment.ContentRootPath, configuredPath);
    }
}
