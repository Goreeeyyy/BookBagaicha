using System;

namespace BookBagaicha.Models.Dto
{
    public class AnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsActive { get; set; }
        public long? CreatedByUserId { get; set; } 
    }
}
