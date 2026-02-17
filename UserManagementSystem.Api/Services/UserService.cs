using UserManagementSystem.Api.DTOs;
using UserManagementSystem.Api.Models;
using UserManagementSystem.Api.Repositories;

namespace UserManagementSystem.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public UserService(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {createUserDto.Email} already exists.");
        }

        var user = new User
        {
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        // Add user groups
        if (createUserDto.GroupIds.Any())
        {
            foreach (var groupId in createUserDto.GroupIds)
            {
                user.UserGroups.Add(new UserGroup
                {
                    UserId = user.Id,
                    GroupId = groupId,
                    JoinedDate = DateTime.UtcNow
                });
            }
        }

        var createdUser = await _userRepository.CreateUserAsync(user);
        
        // Fetch the complete user with groups
        var fullUser = await _userRepository.GetUserByIdAsync(createdUser.Id);
        return MapToUserDto(fullUser!);
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        // Check if email is being changed and if it's already taken
        if (existingUser.Email != updateUserDto.Email)
        {
            var emailExists = await _userRepository.GetUserByEmailAsync(updateUserDto.Email);
            if (emailExists != null)
            {
                throw new InvalidOperationException($"User with email {updateUserDto.Email} already exists.");
            }
        }

        existingUser.FirstName = updateUserDto.FirstName;
        existingUser.LastName = updateUserDto.LastName;
        existingUser.Email = updateUserDto.Email;
        existingUser.PhoneNumber = updateUserDto.PhoneNumber;
        existingUser.IsActive = updateUserDto.IsActive;

        // Update user groups
        existingUser.UserGroups.Clear();
        if (updateUserDto.GroupIds.Any())
        {
            foreach (var groupId in updateUserDto.GroupIds)
            {
                existingUser.UserGroups.Add(new UserGroup
                {
                    UserId = existingUser.Id,
                    GroupId = groupId,
                    JoinedDate = DateTime.UtcNow
                });
            }
        }

        var updatedUser = await _userRepository.UpdateUserAsync(existingUser);
        
        // Fetch the complete user with groups
        var fullUser = await _userRepository.GetUserByIdAsync(updatedUser.Id);
        return MapToUserDto(fullUser!);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteUserAsync(id);
    }

    public async Task<int> GetTotalUserCountAsync()
    {
        return await _userRepository.GetTotalUserCountAsync();
    }

    public async Task<IEnumerable<UserCountByGroupDto>> GetUserCountByGroupAsync()
    {
        var groups = await _groupRepository.GetAllGroupsAsync();
        var userCounts = await _userRepository.GetUserCountByGroupAsync();

        return groups.Select(g => new UserCountByGroupDto
        {
            GroupId = g.Id,
            GroupName = g.Name,
            UserCount = userCounts.ContainsKey(g.Id) ? userCounts[g.Id] : 0
        });
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedDate = user.CreatedDate,
            ModifiedDate = user.ModifiedDate,
            IsActive = user.IsActive,
            Groups = user.UserGroups.Select(ug => new GroupDto
            {
                Id = ug.Group.Id,
                Name = ug.Group.Name,
                Description = ug.Group.Description,
                CreatedDate = ug.Group.CreatedDate,
                Permissions = ug.Group.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    CreatedDate = gp.Permission.CreatedDate
                }).ToList()
            }).ToList()
        };
    }
}
