using FastPCB.Models;
using FastPCB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastPCB.API.Controllers;

[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // Giris yapan kullanicinin bir projeyi moderasyon icin raporlamasini saglar.
    [HttpPost("api/projects/{projectId:int}/report")]
    [Authorize]
    public async Task<IActionResult> CreateReport(int projectId, [FromBody] CreateReportRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var report = await _reportService.CreateReportAsync(projectId, currentUserId.Value, request.Reason, request.Details);
            return Ok(new
            {
                message = "Rapor alindi.",
                report = MapReport(report)
            });
        }
        catch (ReportNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ReportValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Giris yapan kullanicinin gonderdigi raporlari listeler.
    [HttpGet("api/reports/me")]
    [Authorize]
    public async Task<IActionResult> GetMyReports()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var reports = await _reportService.GetUserReportsAsync(currentUserId.Value);
            return Ok(reports.Select(MapReport));
        }
        catch (ReportNotFoundException ex)
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

    // Rapor modelini API cevabinda kullanilacak sade formata mapler.
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
            }
        };
    }
}

public class CreateReportRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
}
