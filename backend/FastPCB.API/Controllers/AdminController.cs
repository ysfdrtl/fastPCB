using FastPCB.Models;
using FastPCB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastPCB.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] AdminUsersQueryRequest request)
    {
        var result = await _adminService.GetUsersAsync(new AdminUserQueryOptions
        {
            Search = request.Search,
            Role = request.Role,
            Page = request.Page,
            PageSize = request.PageSize
        });

        return Ok(ToPagedResponse(result, MapUser));
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUser(int userId)
    {
        try
        {
            var user = await _adminService.GetUserAsync(userId);
            return Ok(MapUser(user));
        }
        catch (AdminNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("users/{userId:int}/role")]
    public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRoleRequest request)
    {
        if (!TryParseEnum<UserRole>(request.Role, out var role))
        {
            return BadRequest(new { message = "Gecerli bir kullanici rolu girin: User veya Admin." });
        }

        if (GetCurrentUserId() == userId && role != UserRole.Admin)
        {
            return BadRequest(new { message = "Kendi admin yetkinizi kaldiramazsiniz." });
        }

        try
        {
            var user = await _adminService.UpdateUserRoleAsync(userId, role);
            return Ok(MapUser(user));
        }
        catch (AdminNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("users/{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        if (GetCurrentUserId() == userId)
        {
            return BadRequest(new { message = "Kendi admin hesabinizi silemezsiniz." });
        }

        try
        {
            await _adminService.DeleteUserAsync(userId);
            return NoContent();
        }
        catch (AdminNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects([FromQuery] AdminProjectsQueryRequest request)
    {
        var result = await _adminService.GetProjectsAsync(new AdminProjectQueryOptions
        {
            UserId = request.UserId,
            Search = request.Search,
            Status = request.Status,
            Page = request.Page,
            PageSize = request.PageSize
        });

        return Ok(ToPagedResponse(result, MapProject));
    }

    [HttpPatch("projects/{projectId:int}/status")]
    public async Task<IActionResult> UpdateProjectStatus(int projectId, [FromBody] UpdateProjectStatusRequest request)
    {
        if (!TryParseEnum<ProjectStatus>(request.Status, out var status))
        {
            return BadRequest(new { message = "Gecerli bir proje durumu girin." });
        }

        try
        {
            var project = await _adminService.UpdateProjectStatusAsync(projectId, status);
            return Ok(MapProject(project));
        }
        catch (AdminNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("projects/{projectId:int}")]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        try
        {
            await _adminService.DeleteProjectAsync(projectId);
            return NoContent();
        }
        catch (AdminNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReports([FromQuery] AdminReportsQueryRequest request)
    {
        var result = await _adminService.GetReportsAsync(new AdminReportQueryOptions
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            Status = request.Status,
            Page = request.Page,
            PageSize = request.PageSize
        });

        return Ok(ToPagedResponse(result, MapReport));
    }

    [HttpPatch("reports/{reportId:int}")]
    public async Task<IActionResult> UpdateReport(int reportId, [FromBody] UpdateReportRequest request)
    {
        if (!TryParseEnum<TicketStatus>(request.Status, out var status))
        {
            return BadRequest(new { message = "Gecerli bir rapor durumu girin." });
        }

        try
        {
            var report = await _adminService.UpdateReportAsync(reportId, status, request.Response);
            return Ok(MapReport(report));
        }
        catch (AdminNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (AdminValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private static bool TryParseEnum<TEnum>(string value, out TEnum result)
        where TEnum : struct, Enum
    {
        result = default;
        return !string.IsNullOrWhiteSpace(value)
            && Enum.TryParse(value.Trim(), ignoreCase: true, out result)
            && Enum.IsDefined(result);
    }

    private static object ToPagedResponse<T>(PagedResult<T> result, Func<T, object> mapper)
    {
        return new
        {
            items = result.Items.Select(mapper),
            pagination = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                totalCount = result.TotalCount,
                totalPages = result.TotalPages
            }
        };
    }

    private static object MapUser(User user)
    {
        return new
        {
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            phone = user.Phone,
            address = user.Address,
            role = user.Role.ToString(),
            createdAt = user.CreatedAt,
            updatedAt = user.UpdatedAt
        };
    }

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
            status = project.Status.ToString(),
            createdAt = project.CreatedAt,
            updatedAt = project.UpdatedAt,
            owner = new
            {
                id = project.User.Id,
                email = project.User.Email,
                firstName = project.User.FirstName,
                lastName = project.User.LastName
            }
        };
    }

    private static object MapReport(Ticket report)
    {
        return new
        {
            id = report.Id,
            projectId = report.ProjectId,
            userId = report.UserId,
            reason = report.Title,
            details = report.Description,
            status = report.Status.ToString(),
            response = report.Response,
            createdAt = report.CreatedAt,
            updatedAt = report.UpdatedAt,
            project = new
            {
                id = report.Project.Id,
                title = report.Project.Title
            },
            reporter = new
            {
                id = report.User.Id,
                email = report.User.Email,
                firstName = report.User.FirstName,
                lastName = report.User.LastName
            }
        };
    }
}

public class AdminUsersQueryRequest
{
    public string? Search { get; set; }
    public UserRole? Role { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AdminProjectsQueryRequest
{
    public int? UserId { get; set; }
    public string? Search { get; set; }
    public ProjectStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AdminReportsQueryRequest
{
    public int? ProjectId { get; set; }
    public int? UserId { get; set; }
    public TicketStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UpdateUserRoleRequest
{
    public string Role { get; set; } = string.Empty;
}

public class UpdateProjectStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class UpdateReportRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Response { get; set; }
}
