using System.ComponentModel.DataAnnotations;

namespace AccountProfileServiceProvider.Entities;

public class UserProfileEntity
{
    [Key]
    public string Id { get; set; } = new Guid().ToString();
    public string AppUserId { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StreetName { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
}
