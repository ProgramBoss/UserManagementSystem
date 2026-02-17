namespace UserManagementSystem.Api.Models;

// Join table for many-to-many relationship between Users and Groups
public class UserGroup
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
}
