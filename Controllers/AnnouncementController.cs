using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookBagaicha.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAnnouncements()
        {
            var announcements = await _announcementService.GetActiveAnnouncementsAsync();
            return Ok(announcements);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnnouncementById(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null) return NotFound();
            return Ok(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnnouncement([FromBody] AnnouncementDto announcementDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var announcement = new Announcement
            {
                Title = announcementDto.Title,
                Message = announcementDto.Message,
                StartDateTime = announcementDto.StartDateTime,
                EndDateTime = announcementDto.EndDateTime,
                IsActive = announcementDto.IsActive
            };
            await _announcementService.AddAnnouncementAsync(announcement);
            return CreatedAtAction(nameof(GetAnnouncementById), new { id = announcement.Id }, announcement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementDto announcementDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null) return NotFound();

            announcement.Title = announcementDto.Title;
            announcement.Message = announcementDto.Message;
            announcement.StartDateTime = announcementDto.StartDateTime;
            announcement.EndDateTime = announcementDto.EndDateTime;
            announcement.IsActive = announcementDto.IsActive;

            await _announcementService.UpdateAnnouncementAsync(announcement);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null) return NotFound();
            await _announcementService.DeleteAnnouncementAsync(id);
            return NoContent();
        }
    }
}