using UserManagementSystem.Api.Models;

namespace UserManagementSystem.Api.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<Group?> GetGroupByIdAsync(int id);
}
