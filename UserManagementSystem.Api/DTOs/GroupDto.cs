namespace UserManagementSystem.Api.DTOs;

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class UserCountByGroupDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int UserCount { get; set; }
}
