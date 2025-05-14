using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
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
            try
            {
                var announcements = await _announcementService.GetActiveAnnouncementsAsync();
                return Ok(announcements.Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    StartDateTime = a.StartDateTime,
                    EndDateTime = a.EndDateTime,
                    IsActive = a.IsActive
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching active announcements: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching announcements.");
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAnnouncements()
        {
            try
            {
                var announcements = await _announcementService.GetAllAnnouncementsAsync();
                var announcementDtos = announcements.Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    StartDateTime = a.StartDateTime,
                    EndDateTime = a.EndDateTime,
                    IsActive = a.IsActive
                }).ToList();
                return Ok(announcementDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all announcements: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching announcements.");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnnouncementById(int id)
        {
            try
            {
                var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
                if (announcement == null) return NotFound();
                return Ok(announcement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching announcement by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the announcement.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAnnouncement([FromBody] AnnouncementDto announcementDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var announcement = new Announcement
                {
                    Title = announcementDto.Title,
                    Message = announcementDto.Message,
                    StartDateTime = announcementDto.StartDateTime.Kind == DateTimeKind.Utc
                        ? announcementDto.StartDateTime
                        : announcementDto.StartDateTime.ToUniversalTime(),
                    EndDateTime = announcementDto.EndDateTime.Kind == DateTimeKind.Utc
                        ? announcementDto.EndDateTime
                        : announcementDto.EndDateTime.ToUniversalTime(),
                    IsActive = announcementDto.IsActive
                };
                await _announcementService.AddAnnouncementAsync(announcement);
                return CreatedAtAction(nameof(GetAnnouncementById), new { id = announcement.Id }, announcement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding announcement: {ex.Message}");
                return StatusCode(500, "An error occurred while adding the announcement.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementDto announcementDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
                if (announcement == null) return NotFound();

                announcement.Title = announcementDto.Title;
                announcement.Message = announcementDto.Message;
                announcement.StartDateTime = announcementDto.StartDateTime.Kind == DateTimeKind.Utc
                    ? announcementDto.StartDateTime
                    : announcementDto.StartDateTime.ToUniversalTime();
                announcement.EndDateTime = announcementDto.EndDateTime.Kind == DateTimeKind.Utc
                    ? announcementDto.EndDateTime
                    : announcementDto.EndDateTime.ToUniversalTime();
                announcement.IsActive = announcementDto.IsActive;

                await _announcementService.UpdateAnnouncementAsync(announcement);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating announcement: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the announcement.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            try
            {
                var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
                if (announcement == null) return NotFound();
                await _announcementService.DeleteAnnouncementAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting announcement: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the announcement.");
            }
        }
    }
}