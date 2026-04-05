using FastPCB.Models;
using FastPCB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastPCB.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // Belirli bir projenin yorumlarini listeler.
    [HttpGet]
    public async Task<IActionResult> GetComments(int projectId)
    {
        try
        {
            var comments = await _commentService.GetProjectCommentsAsync(projectId);
            return Ok(comments.Select(MapComment));
        }
        catch (CommentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Giris yapan kullanici adina projeye yeni yorum ekler.
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment(int projectId, [FromBody] CreateCommentRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            var comment = await _commentService.CreateCommentAsync(projectId, currentUserId.Value, request.Content);
            return Created(string.Empty, MapComment(comment));
        }
        catch (CommentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (CommentValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Yalnizca yorum sahibi tarafindan yorumun silinmesini saglar.
    [HttpDelete("/api/comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { message = "Gecerli kullanici bilgisi bulunamadi." });
        }

        try
        {
            await _commentService.DeleteCommentAsync(commentId, currentUserId.Value);
            return NoContent();
        }
        catch (CommentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (CommentForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    // JWT icindeki kullanici kimligini okuyup sayisal Id'ye cevirir.
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    // Comment modelini API tarafinda daha sade bir cevap nesnesine mapler.
    private static object MapComment(Comment comment)
    {
        return new
        {
            id = comment.Id,
            projectId = comment.ProjectId,
            userId = comment.UserId,
            content = comment.Content,
            createdAt = comment.CreatedAt,
            updatedAt = comment.UpdatedAt,
            author = new
            {
                id = comment.User.Id,
                firstName = comment.User.FirstName,
                lastName = comment.User.LastName
            }
        };
    }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
}
