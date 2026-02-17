using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Api.Data;
using UserManagementSystem.Api.Models;
using UserManagementSystem.Api.Repositories;
using Xunit;

namespace UserManagementSystem.Api.Tests.Repositories;

public class GroupRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GroupRepository _repository;

    public GroupRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new GroupRepository(_context);
        SeedDatabase();
    }

    [Fact]
    public async Task GetAllGroupsAsync_ShouldReturnAllGroups()
    {
        // Act
        var result = await _repository.GetAllGroupsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(g => g.Name == "Admin");
        result.Should().Contain(g => g.Name == "Users");
        result.Should().Contain(g => g.Name == "Managers");
    }

    [Fact]
    public async Task GetAllGroupsAsync_ShouldIncludePermissions()
    {
        // Act
        var result = await _repository.GetAllGroupsAsync();

        // Assert
        var adminGroup = result.FirstOrDefault(g => g.Name == "Admin");
        adminGroup.Should().NotBeNull();
        adminGroup!.GroupPermissions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetGroupByIdAsync_WithExistingGroup_ShouldReturnGroup()
    {
        // Arrange
        var groupId = _context.Groups.First().Id;

        // Act
        var result = await _repository.GetGroupByIdAsync(groupId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(groupId);
    }

    [Fact]
    public async Task GetGroupByIdAsync_WithNonExistentGroup_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetGroupByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGroupByIdAsync_ShouldIncludePermissions()
    {
        // Arrange
        var groupId = _context.Groups.First().Id;

        // Act
        var result = await _repository.GetGroupByIdAsync(groupId);

        // Assert
        result.Should().NotBeNull();
        result!.GroupPermissions.Should().NotBeNull();
    }

    private void SeedDatabase()
    {
        // Add permissions
        var permissions = new List<Permission>
        {
            new Permission
            {
                Name = "Read",
                Description = "Read access",
                CreatedDate = DateTime.UtcNow
            },
            new Permission
            {
                Name = "Write",
                Description = "Write access",
                CreatedDate = DateTime.UtcNow
            },
            new Permission
            {
                Name = "Delete",
                Description = "Delete access",
                CreatedDate = DateTime.UtcNow
            }
        };

        _context.Permissions.AddRange(permissions);
        _context.SaveChanges();

        // Add groups
        var adminGroup = new Group
        {
            Name = "Admin",
            Description = "Administrators with full access",
            CreatedDate = DateTime.UtcNow
        };

        var userGroup = new Group
        {
            Name = "Users",
            Description = "Regular users",
            CreatedDate = DateTime.UtcNow
        };

        var managerGroup = new Group
        {
            Name = "Managers",
            Description = "Department managers",
            CreatedDate = DateTime.UtcNow
        };

        _context.Groups.AddRange(adminGroup, userGroup, managerGroup);
        _context.SaveChanges();

        // Add group permissions
        var readPermission = permissions[0];
        var writePermission = permissions[1];
        var deletePermission = permissions[2];

        _context.GroupPermissions.AddRange(
            new GroupPermission { GroupId = adminGroup.Id, PermissionId = readPermission.Id, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = adminGroup.Id, PermissionId = writePermission.Id, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = adminGroup.Id, PermissionId = deletePermission.Id, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = userGroup.Id, PermissionId = readPermission.Id, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = managerGroup.Id, PermissionId = readPermission.Id, GrantedDate = DateTime.UtcNow },
            new GroupPermission { GroupId = managerGroup.Id, PermissionId = writePermission.Id, GrantedDate = DateTime.UtcNow }
        );

        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
