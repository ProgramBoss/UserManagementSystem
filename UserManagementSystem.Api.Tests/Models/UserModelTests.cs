using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using UserManagementSystem.Api.Models;
using Xunit;

namespace UserManagementSystem.Api.Tests.Models;

public class UserModelTests
{
    [Fact]
    public void User_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void User_WithoutFirstName_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void User_WithoutLastName_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "",
            Email = "john@example.com"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("LastName"));
    }

    [Fact]
    public void User_WithInvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void User_WithoutEmail_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = ""
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void User_WithTooLongFirstName_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = new string('A', 101), // Exceeds 100 character limit
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void User_WithTooLongLastName_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = new string('B', 101), // Exceeds 100 character limit
            Email = "john@example.com"
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("LastName"));
    }

    [Fact]
    public void User_WithTooLongEmail_ShouldFailValidation()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = new string('a', 250) + "@test.com" // Exceeds 256 character limit
        };

        // Act
        var validationResults = ValidateModel(user);

        // Assert
        validationResults.Should().Contain(v => v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void User_DefaultIsActive_ShouldBeTrue()
    {
        // Arrange & Act
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_DefaultCreatedDate_ShouldBeSet()
    {
        // Arrange & Act
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Assert
        user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void User_ModifiedDate_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Assert
        user.ModifiedDate.Should().BeNull();
    }

    [Fact]
    public void User_UserGroups_ShouldBeInitializedAsEmptyList()
    {
        // Arrange & Act
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Assert
        user.UserGroups.Should().NotBeNull();
        user.UserGroups.Should().BeEmpty();
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
