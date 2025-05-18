using BookBagaicha.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBagaicha.IService
{
    public interface IAnnouncementService
    {
        Task<List<Announcement>> GetActiveAnnouncementsAsync();
        Task<List<Announcement>> GetAllAnnouncementsAsync();
        Task<Announcement?> GetAnnouncementByIdAsync(int id);
        Task AddAnnouncementAsync(Announcement announcement);
        Task UpdateAnnouncementAsync(Announcement announcement);
        Task DeleteAnnouncementAsync(int id);
    }
}