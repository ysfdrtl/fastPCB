using FastPCB.Models;
using FastPCB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastPCB.API.Controllers;

[ApiController]
public class ProjectLikeController : ControllerBase
{
    private readonly IProjectLikeService _projectLikeService;

    public ProjectLikeController(IProjectLikeService projectLikeService)
    {
        _projectLikeService = projectLikeService;
    }

    // Giris yapan kullanicinin ilgili projeye begeni birakmasini saglar.
    [HttpPost("api/projects/{projectId:int}/like")]
    [Authorize]
    public async Task<IActionResult> LikeProject(int projectId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var like = await _projectLikeService.LikeProjectAsync(projectId, currentUserId.Value);
            return Ok(new
            {
                message = "Proje begenildi.",
                like = MapLike(like)
            });
        }
        catch (ProjectLikeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Giris yapan kullanicinin onceki begenisini kaldirir.
    [HttpDelete("api/projects/{projectId:int}/like")]
    [Authorize]
    public async Task<IActionResult> UnlikeProject(int projectId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            await _projectLikeService.UnlikeProjectAsync(projectId, currentUserId.Value);
            return NoContent();
        }
        catch (ProjectLikeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Giris yapan kullanicinin begendigi projeleri listeler.
    [HttpGet("api/likes/me")]
    [Authorize]
    public async Task<IActionResult> GetMyLikedProjects()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var projects = await _projectLikeService.GetLikedProjectsAsync(currentUserId.Value);
            return Ok(projects.Select(MapProject));
        }
        catch (ProjectLikeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // JWT icindeki kullanici kimligini okuyup sayisal Id'ye cevirir.
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    // Begeni nesnesini kisa bir API cevabina mapler.
    private static object MapLike(ProjectLike like)
    {
        return new
        {
            id = like.Id,
            projectId = like.ProjectId,
            userId = like.UserId,
            createdAt = like.CreatedAt
        };
    }

    // Proje nesnesini begeni listesi icin frontend uyumlu formata mapler.
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
}
