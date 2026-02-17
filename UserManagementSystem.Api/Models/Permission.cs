using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Api.Models;

public class Permission
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation property for many-to-many relationship with Groups
    public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}
