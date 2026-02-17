using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Api.Models;

public class User
{
  [Key]
  public int Id { get; set; }

  [Required]
  [StringLength(100)]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  [StringLength(100)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  [StringLength(256)]
  public string Email { get; set; } = string.Empty;

  [StringLength(20)]
  public string? PhoneNumber { get; set; }

  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

  public DateTime? ModifiedDate { get; set; }

  public bool IsActive { get; set; } = true;

  public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
