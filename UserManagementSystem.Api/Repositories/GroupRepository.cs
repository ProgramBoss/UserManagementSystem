using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Api.Data;
using UserManagementSystem.Api.Models;

namespace UserManagementSystem.Api.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await _context.Groups
            .Include(g => g.GroupPermissions)
                .ThenInclude(gp => gp.Permission)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Group?> GetGroupByIdAsync(int id)
    {
        return await _context.Groups
            .Include(g => g.GroupPermissions)
                .ThenInclude(gp => gp.Permission)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
