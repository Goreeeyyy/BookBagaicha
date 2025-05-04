using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models;

public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required  string? Name { get; set; }
    public required string? Email { get; set; }
    public required string? Phone { get; set; }

    [ForeignKey("Address")]
    public Guid?  AddressId { get; set; }

    // Navigation Property
    public Address? Address { get; set; }
}