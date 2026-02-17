using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagementSystem.Api.Controllers;
using UserManagementSystem.Api.DTOs;
using UserManagementSystem.Api.Services;
using Xunit;

namespace UserManagementSystem.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<UsersController>> _loggerMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOkWithUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new UserDto { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        };
        _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedUsers = okResult.Value as IEnumerable<UserDto>;
        returnedUsers.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllUsers_WhenExceptionOccurs_ShouldReturn500()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetAllUsersAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var statusResult = result.Result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetUserById_WithExistingUser_ShouldReturnOk()
    {
        // Arrange
        var user = new UserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        _userServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(1);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedUser = okResult.Value as UserDto;
        returnedUser!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetUserById_WithNonExistentUser_ShouldReturnNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetUserByIdAsync(999)).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetUserById(999);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        var createdUser = new UserDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        _userServiceMock.Setup(x => x.CreateUserAsync(createDto)).ReturnsAsync(createdUser);

        // Act
        var result = await _controller.CreateUser(createDto);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(UsersController.GetUserById));
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        _userServiceMock.Setup(x => x.CreateUserAsync(createDto))
            .ThrowsAsync(new InvalidOperationException("Email already exists"));

        // Act
        var result = await _controller.CreateUser(createDto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var updateDto = new UpdateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };
        var updatedUser = new UserDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        _userServiceMock.Setup(x => x.UpdateUserAsync(1, updateDto)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(1, updateDto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentUser_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };
        _userServiceMock.Setup(x => x.UpdateUserAsync(999, updateDto))
            .ThrowsAsync(new KeyNotFoundException("User not found"));

        // Act
        var result = await _controller.UpdateUser(999, updateDto);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteUser_WithExistingUser_ShouldReturnNoContent()
    {
        // Arrange
        _userServiceMock.Setup(x => x.DeleteUserAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentUser_ShouldReturnNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.DeleteUserAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetTotalUserCount_ShouldReturnOkWithCount()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetTotalUserCountAsync()).ReturnsAsync(10);

        // Act
        var result = await _controller.GetTotalUserCount();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(10);
    }

    [Fact]
    public async Task GetUserCountByGroup_ShouldReturnOkWithCounts()
    {
        // Arrange
        var counts = new List<UserCountByGroupDto>
        {
            new UserCountByGroupDto { GroupId = 1, GroupName = "Admin", UserCount = 5 },
            new UserCountByGroupDto { GroupId = 2, GroupName = "Users", UserCount = 15 }
        };
        _userServiceMock.Setup(x => x.GetUserCountByGroupAsync()).ReturnsAsync(counts);

        // Act
        var result = await _controller.GetUserCountByGroup();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedCounts = okResult.Value as IEnumerable<UserCountByGroupDto>;
        returnedCounts.Should().HaveCount(2);
    }
}
