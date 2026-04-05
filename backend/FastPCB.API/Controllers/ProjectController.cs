using FastPCB.Models;
using FastPCB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastPCB.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // JWT ile giris yapan kullanici icin yeni proje olusturur.
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var project = await _projectService.CreateProjectAsync(currentUserId.Value, request.Title, request.Description);
            return CreatedAtAction(nameof(GetProject), new { projectId = project.Id }, MapProject(project));
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProjectValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Projeleri filtreleme ve sayfalama destegiyle listeler.
    [HttpGet]
    public async Task<IActionResult> GetProjects([FromQuery] GetProjectsQueryRequest request)
    {
        try
        {
            var result = await _projectService.GetProjectsAsync(new ProjectQueryOptions
            {
                UserId = request.UserId,
                Search = request.Search,
                Status = request.Status,
                HasFile = request.HasFile,
                Page = request.Page,
                PageSize = request.PageSize
            });
            return Ok(new
            {
                items = result.Items.Select(MapProject),
                pagination = new
                {
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    totalPages = result.TotalPages
                }
            });
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Belirli bir kullaniciya ait projeleri getirir.
    [HttpGet("/api/users/{userId:int}/projects")]
    public async Task<IActionResult> GetUserProjects(int userId)
    {
        try
        {
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Ok(projects.Select(MapProject));
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Tek bir projenin detay bilgisini getirir.
    [HttpGet("{projectId:int}")]
    public async Task<IActionResult> GetProject(int projectId)
    {
        try
        {
            var project = await _projectService.GetProjectAsync(projectId);
            return Ok(MapProject(project));
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Projenin teknik detay alanlarini kaydeder.
    [HttpPost("{projectId:int}/details")]
    [Authorize]
    public async Task<IActionResult> SaveProjectDetails(int projectId, [FromBody] ProjectDetailsRequest request)
    {
        var ownershipCheck = await EnsureProjectOwnershipAsync(projectId);
        if (ownershipCheck is not null)
        {
            return ownershipCheck;
        }

        try
        {
            var project = await _projectService.UpdateProjectDetailsAsync(projectId, new ProjectDetails
            {
                Layers = request.Layers,
                Material = request.Material,
                MinDistance = request.MinDistance,
                Quantity = request.Quantity
            });

            return Ok(new
            {
                message = "Proje detaylari kaydedildi.",
                project = MapProject(project)
            });
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProjectValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Projeye gerber, zip veya gorsel gibi bir dosya yukler.
    [HttpPost("{projectId:int}/files")]
    [Consumes("multipart/form-data")]
    [Authorize]
    public async Task<IActionResult> UploadProjectFile(int projectId, [FromForm] UploadProjectFileRequest request, CancellationToken cancellationToken)
    {
        var ownershipCheck = await EnsureProjectOwnershipAsync(projectId);
        if (ownershipCheck is not null)
        {
            return ownershipCheck;
        }

        if (request.File is null)
        {
            return BadRequest(new { message = "Yuklenecek dosya secilmedi." });
        }

        try
        {
            await using var stream = request.File.OpenReadStream();
            var project = await _projectService.UploadProjectFileAsync(
                projectId,
                stream,
                request.File.FileName,
                request.File.Length,
                cancellationToken);

            return Ok(new
            {
                message = "Proje dosyasi basariyla yuklendi.",
                project = MapProject(project)
            });
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProjectValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Projeyi ve varsa bagli dosya klasorunu siler.
    [HttpDelete("{projectId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        var ownershipCheck = await EnsureProjectOwnershipAsync(projectId);
        if (ownershipCheck is not null)
        {
            return ownershipCheck;
        }

        try
        {
            await _projectService.DeleteProjectAsync(projectId);
            return NoContent();
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // API cevabini frontend tarafinin kullandigi proje formatina mapler.
    private static object MapProject(Project project)
    {
        return new
        {
            id = project.Id,
            userId = project.UserId,
            title = project.Title,
            description = project.Description,
            filePath = project.FilePath,
            technicalDetails = new
            {
                layers = project.Layers,
                material = project.Material,
                minDistance = project.MinDistance,
                quantity = project.Quantity
            },
            createdAt = project.CreatedAt,
            updatedAt = project.UpdatedAt,
            status = project.Status.ToString(),
            owner = new
            {
                id = project.User.Id,
                firstName = project.User.FirstName,
                lastName = project.User.LastName
            }
        };
    }

    // JWT icindeki kullanici kimligini okuyup sayisal kullanici Id'sine cevirir.
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    // Yazan kullanicinin ilgili projenin sahibi olup olmadigini kontrol eder.
    private async Task<IActionResult?> EnsureProjectOwnershipAsync(int projectId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var project = await _projectService.GetProjectAsync(projectId);
            if (project.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            return null;
        }
        catch (ProjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public class ProjectDetailsRequest
{
    public int Layers { get; set; }
    public string Material { get; set; } = string.Empty;
    public double MinDistance { get; set; }
    public int Quantity { get; set; }
}

public class CreateProjectRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GetProjectsQueryRequest
{
    public int? UserId { get; set; }
    public string? Search { get; set; }
    public ProjectStatus? Status { get; set; }
    public bool? HasFile { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UploadProjectFileRequest
{
    public IFormFile? File { get; set; }
}
