using Microsoft.AspNetCore.Identity;

namespace StudentManagement.Database.Entities
{
    public class User: IdentityUser<long>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; } 
        public string? Address { get; set; }

    }
}
