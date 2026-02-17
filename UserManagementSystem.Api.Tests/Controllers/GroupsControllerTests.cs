using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagementSystem.Api.Controllers;
using UserManagementSystem.Api.DTOs;
using UserManagementSystem.Api.Models;
using UserManagementSystem.Api.Repositories;
using Xunit;

namespace UserManagementSystem.Api.Tests.Controllers;

public class GroupsControllerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<ILogger<GroupsController>> _loggerMock;
    private readonly GroupsController _controller;

    public GroupsControllerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _loggerMock = new Mock<ILogger<GroupsController>>();
        _controller = new GroupsController(_groupRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllGroups_ShouldReturnOkWithGroups()
    {
        // Arrange
        var groups = new List<Group>
        {
            CreateTestGroup(1, "Admin", "Administrators"),
            CreateTestGroup(2, "Users", "Regular Users")
        };
        _groupRepositoryMock.Setup(x => x.GetAllGroupsAsync()).ReturnsAsync(groups);

        // Act
        var result = await _controller.GetAllGroups();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGroups = okResult.Value as IEnumerable<GroupDto>;
        returnedGroups.Should().NotBeNull();
        returnedGroups!.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetAllGroups_WhenExceptionOccurs_ShouldReturn500()
    {
        // Arrange
        _groupRepositoryMock.Setup(x => x.GetAllGroupsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllGroups();

        // Assert
        var statusResult = result.Result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetGroupById_WithExistingGroup_ShouldReturnOk()
    {
        // Arrange
        var group = CreateTestGroup(1, "Admin", "Administrators");
        _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(1)).ReturnsAsync(group);

        // Act
        var result = await _controller.GetGroupById(1);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGroup = okResult.Value as GroupDto;
        returnedGroup.Should().NotBeNull();
        returnedGroup!.Id.Should().Be(1);
        returnedGroup.Name.Should().Be("Admin");
    }

    [Fact]
    public async Task GetGroupById_WithNonExistentGroup_ShouldReturnNotFound()
    {
        // Arrange
        _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(999)).ReturnsAsync((Group?)null);

        // Act
        var result = await _controller.GetGroupById(999);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetGroupById_WhenExceptionOccurs_ShouldReturn500()
    {
        // Arrange
        _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(1))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetGroupById(1);

        // Assert
        var statusResult = result.Result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(500);
    }

    private Group CreateTestGroup(int id, string name, string description)
    {
        return new Group
        {
            Id = id,
            Name = name,
            Description = description,
            CreatedDate = DateTime.UtcNow,
            GroupPermissions = new List<GroupPermission>(),
            UserGroups = new List<UserGroup>()
        };
    }
}
