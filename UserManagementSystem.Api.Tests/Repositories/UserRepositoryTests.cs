using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Api.Data;
using UserManagementSystem.Api.Models;
using UserManagementSystem.Api.Repositories;
using Xunit;

namespace UserManagementSystem.Api.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
        SeedDatabase();
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Act
        var result = await _repository.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(u => u.Email == "john@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var userId = _context.Users.First().Id;

        // Act
        var result = await _repository.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithExistingEmail_ShouldReturnUser()
    {
        // Act
        var result = await _repository.GetUserByEmailAsync("john@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetUserByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var newUser = new User
        {
            FirstName = "New",
            LastName = "User",
            Email = "newuser@example.com",
            PhoneNumber = "1234567890",
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateUserAsync(newUser);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        var savedUser = await _context.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldModifyUser()
    {
        // Arrange
        var user = _context.Users.First();
        user.FirstName = "UpdatedName";
        user.Email = "updated@example.com";

        // Act
        var result = await _repository.UpdateUserAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("UpdatedName");
        result.ModifiedDate.Should().NotBeNull();

        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser!.FirstName.Should().Be("UpdatedName");
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ShouldReturnTrue()
    {
        // Arrange
        var userId = _context.Users.First().Id;

        // Act
        var result = await _repository.DeleteUserAsync(userId);

        // Assert
        result.Should().BeTrue();
        var deletedUser = await _context.Users.FindAsync(userId);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteUserAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTotalUserCountAsync_ShouldReturnCorrectCount()
    {
        // Act
        var result = await _repository.GetTotalUserCountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetUserCountByGroupAsync_ShouldReturnCorrectCounts()
    {
        // Arrange
        var group = _context.Groups.First();
        var user = _context.Users.First();
        
        _context.UserGroups.Add(new UserGroup
        {
            UserId = user.Id,
            GroupId = group.Id,
            JoinedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserCountByGroupAsync();

        // Assert
        result.Should().ContainKey(group.Id);
        result[group.Id].Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UserExistsAsync_WithExistingUser_ShouldReturnTrue()
    {
        // Arrange
        var userId = _context.Users.First().Id;

        // Act
        var result = await _repository.UserExistsAsync(userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserExistsAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.UserExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    private void SeedDatabase()
    {
        // Add groups
        var adminGroup = new Group
        {
            Name = "Admin",
            Description = "Administrators",
            CreatedDate = DateTime.UtcNow
        };
        var userGroup = new Group
        {
            Name = "Users",
            Description = "Regular Users",
            CreatedDate = DateTime.UtcNow
        };

        _context.Groups.AddRange(adminGroup, userGroup);

        // Add permissions
        var readPermission = new Permission
        {
            Name = "Read",
            Description = "Read access",
            CreatedDate = DateTime.UtcNow
        };
        var writePermission = new Permission
        {
            Name = "Write",
            Description = "Write access",
            CreatedDate = DateTime.UtcNow
        };

        _context.Permissions.AddRange(readPermission, writePermission);

        // Add users
        var users = new List<User>
        {
            new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "0987654321",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob@example.com",
                PhoneNumber = "5555555555",
                CreatedDate = DateTime.UtcNow,
                IsActive = false
            }
        };

        _context.Users.AddRange(users);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
