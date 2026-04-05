using FastPCB.Data;
using FastPCB.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.Services
{
    public interface ICommentService
    {
        Task<List<Comment>> GetProjectCommentsAsync(int projectId);
        Task<Comment> CreateCommentAsync(int projectId, int userId, string content);
        Task DeleteCommentAsync(int commentId, int userId);
    }

    public class CommentService : ICommentService
    {
        private readonly FastPCBContext _context;

        public CommentService(FastPCBContext context)
        {
            _context = context;
        }

        // Verilen projeye ait yorumlari kullanici bilgileriyle birlikte listeler.
        public async Task<List<Comment>> GetProjectCommentsAsync(int projectId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                throw new CommentNotFoundException("Yorumlari listelenecek proje bulunamadi.");
            }

            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ProjectId == projectId)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        // Yeni yorumu dogrulama kurallarindan gecirip ilgili projeye ekler.
        public async Task<Comment> CreateCommentAsync(int projectId, int userId, string content)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                throw new CommentNotFoundException("Yorum eklenecek proje bulunamadi.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new CommentNotFoundException("Yorum eklenecek kullanici bulunamadi.");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new CommentValidationException("Yorum icerigi bos birakilamaz.");
            }

            var normalizedContent = content.Trim();
            if (normalizedContent.Length > 1000)
            {
                throw new CommentValidationException("Yorum en fazla 1000 karakter olabilir.");
            }

            var comment = new Comment
            {
                ProjectId = projectId,
                UserId = userId,
                Content = normalizedContent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return await _context.Comments
                .Include(c => c.User)
                .AsNoTracking()
                .FirstAsync(c => c.Id == comment.Id);
        }

        // Yorumu sadece sahibi silebilsin diye kullanici kontrolu yaparak kaydi kaldirir.
        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment is null)
            {
                throw new CommentNotFoundException("Silinecek yorum bulunamadi.");
            }

            if (comment.UserId != userId)
            {
                throw new CommentForbiddenException("Bu yorumu silme yetkiniz yok.");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public sealed class CommentNotFoundException : Exception
    {
        public CommentNotFoundException(string message) : base(message)
        {
        }
    }

    public sealed class CommentValidationException : Exception
    {
        public CommentValidationException(string message) : base(message)
        {
        }
    }

    public sealed class CommentForbiddenException : Exception
    {
        public CommentForbiddenException(string message) : base(message)
        {
        }
    }
}
