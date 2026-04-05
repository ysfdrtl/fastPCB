using FastPCB.Data;
using FastPCB.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPCB.Services
{
    public interface IProjectLikeService
    {
        Task<ProjectLike> LikeProjectAsync(int projectId, int userId);
        Task UnlikeProjectAsync(int projectId, int userId);
        Task<List<Project>> GetLikedProjectsAsync(int userId);
    }

    public class ProjectLikeService : IProjectLikeService
    {
        private readonly FastPCBContext _context;

        public ProjectLikeService(FastPCBContext context)
        {
            _context = context;
        }

        // Kullanici projeyi begenir; daha once begenmisse mevcut kaydi dondurur.
        public async Task<ProjectLike> LikeProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project is null)
            {
                throw new ProjectLikeNotFoundException("Begenilecek proje bulunamadi.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ProjectLikeNotFoundException("Begeni ekleyecek kullanici bulunamadi.");
            }

            var existingLike = await _context.Set<ProjectLike>()
                .FirstOrDefaultAsync(l => l.ProjectId == projectId && l.UserId == userId);

            if (existingLike is not null)
            {
                return existingLike;
            }

            var like = new ProjectLike
            {
                ProjectId = projectId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<ProjectLike>().Add(like);
            await _context.SaveChangesAsync();

            return await _context.Set<ProjectLike>()
                .Include(l => l.Project)
                .ThenInclude(p => p.User)
                .Include(l => l.User)
                .AsNoTracking()
                .FirstAsync(l => l.Id == like.Id);
        }

        // Kullaniciya ait proje begenisini kaldirir.
        public async Task UnlikeProjectAsync(int projectId, int userId)
        {
            var like = await _context.Set<ProjectLike>()
                .FirstOrDefaultAsync(l => l.ProjectId == projectId && l.UserId == userId);

            if (like is null)
            {
                throw new ProjectLikeNotFoundException("Kaldirilacak begeni bulunamadi.");
            }

            _context.Set<ProjectLike>().Remove(like);
            await _context.SaveChangesAsync();
        }

        // Kullanici tarafindan begenilen projeleri ters kronolojik sirada listeler.
        public async Task<List<Project>> GetLikedProjectsAsync(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ProjectLikeNotFoundException("Begenileri listelenecek kullanici bulunamadi.");
            }

            return await _context.Set<ProjectLike>()
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => l.Project)
                .Include(p => p.User)
                .AsNoTracking()
                .ToListAsync();
        }
    }

    public sealed class ProjectLikeNotFoundException : Exception
    {
        public ProjectLikeNotFoundException(string message) : base(message)
        {
        }
    }
}
