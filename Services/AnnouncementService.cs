using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBagaicha.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AppDbContext _context;

        public AnnouncementService(AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<Announcement>> GetActiveAnnouncementsAsync()
        {
            return await _context.Announcements
                .Where(a => a.StartDateTime <= DateTime.UtcNow && a.EndDateTime >= DateTime.UtcNow)
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();
        }

        public virtual async Task<List<Announcement>> GetAllAnnouncementsAsync()
        {
            return await _context.Announcements
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();
        }

        public virtual async Task<Announcement?> GetAnnouncementByIdAsync(int id)
        {
            return await _context.Announcements.FindAsync(id);
        }

        public virtual async Task AddAnnouncementAsync(Announcement announcement)
        {
            announcement.StartDateTime = DateTime.SpecifyKind(announcement.StartDateTime, DateTimeKind.Utc);
            announcement.EndDateTime = DateTime.SpecifyKind(announcement.EndDateTime, DateTimeKind.Utc);
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAnnouncementAsync(Announcement announcement)
        {
            announcement.StartDateTime = DateTime.SpecifyKind(announcement.StartDateTime, DateTimeKind.Utc);
            announcement.EndDateTime = DateTime.SpecifyKind(announcement.EndDateTime, DateTimeKind.Utc);
            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAnnouncementAsync(int id)
        {
            var announcement = await GetAnnouncementByIdAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
            }
        }
    }
}