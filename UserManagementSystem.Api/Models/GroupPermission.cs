namespace UserManagementSystem.Api.Models;

// Join table for many-to-many relationship between Groups and Permissions
public class GroupPermission
{
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public DateTime GrantedDate { get; set; } = DateTime.UtcNow;
}
