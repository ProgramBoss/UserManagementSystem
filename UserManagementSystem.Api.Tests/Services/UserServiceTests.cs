using FluentAssertions;
using Moq;
using UserManagementSystem.Api.DTOs;
using UserManagementSystem.Api.Models;
using UserManagementSystem.Api.Repositories;
using UserManagementSystem.Api.Services;
using Xunit;

namespace UserManagementSystem.Api.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _userService = new UserService(_userRepositoryMock.Object, _groupRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            CreateTestUser(1, "John", "Doe", "john@example.com"),
            CreateTestUser(2, "Jane", "Smith", "jane@example.com")
        };
        _userRepositoryMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "john@example.com");
        result.Should().Contain(u => u.Email == "jane@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser(1, "John", "Doe", "john@example.com");
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            GroupIds = new List<int> { 1 }
        };

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(createDto.Email))
            .ReturnsAsync((User?)null);

        var createdUser = CreateTestUser(1, "John", "Doe", "john@example.com");
        _userRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@example.com");
        _userRepositoryMock.Verify(x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        var existingUser = CreateTestUser(1, "Jane", "Smith", "john@example.com");
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(createDto.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.CreateUserAsync(createDto));
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var existingUser = CreateTestUser(1, "John", "Doe", "john@example.com");
        var updateDto = new UpdateUserDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john@example.com",
            PhoneNumber = "9876543210",
            IsActive = true,
            GroupIds = new List<int>()
        };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.UpdateUserAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateUserDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com"
        };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.UpdateUserAsync(999, updateDto));
    }

    [Fact]
    public async Task UpdateUserAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingUser = CreateTestUser(1, "John", "Doe", "john@example.com");
        var otherUser = CreateTestUser(2, "Jane", "Smith", "jane@example.com");
        var updateDto = new UpdateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "jane@example.com", // Trying to use Jane's email
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync("jane@example.com"))
            .ReturnsAsync(otherUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.UpdateUserAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ShouldReturnTrue()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        result.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.DeleteUserAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTotalUserCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetTotalUserCountAsync())
            .ReturnsAsync(10);

        // Act
        var result = await _userService.GetTotalUserCountAsync();

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public async Task GetUserCountByGroupAsync_ShouldReturnCorrectCounts()
    {
        // Arrange
        var groups = new List<Group>
        {
            new Group { Id = 1, Name = "Admin", Description = "Administrators" },
            new Group { Id = 2, Name = "Users", Description = "Regular Users" }
        };

        var userCounts = new Dictionary<int, int>
        {
            { 1, 5 },
            { 2, 15 }
        };

        _groupRepositoryMock.Setup(x => x.GetAllGroupsAsync())
            .ReturnsAsync(groups);
        _userRepositoryMock.Setup(x => x.GetUserCountByGroupAsync())
            .ReturnsAsync(userCounts);

        // Act
        var result = await _userService.GetUserCountByGroupAsync();

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList.Should().Contain(x => x.GroupId == 1 && x.UserCount == 5);
        resultList.Should().Contain(x => x.GroupId == 2 && x.UserCount == 15);
    }

    private User CreateTestUser(int id, string firstName, string lastName, string email)
    {
        return new User
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = "1234567890",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            UserGroups = new List<UserGroup>()
        };
    }
}
