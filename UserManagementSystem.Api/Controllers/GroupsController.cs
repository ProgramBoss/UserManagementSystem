using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Api.DTOs;
using UserManagementSystem.Api.Repositories;

namespace UserManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(IGroupRepository groupRepository, ILogger<GroupsController> logger)
    {
        _groupRepository = groupRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetAllGroups()
    {
        try
        {
            var groups = await _groupRepository.GetAllGroupsAsync();
            var groupDtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreatedDate = g.CreatedDate,
                Permissions = g.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    CreatedDate = gp.Permission.CreatedDate
                }).ToList()
            });
            return Ok(groupDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all groups");
            return StatusCode(500, "An error occurred while retrieving groups");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> GetGroupById(int id)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound($"Group with ID {id} not found");
            }

            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedDate = group.CreatedDate,
                Permissions = group.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    CreatedDate = gp.Permission.CreatedDate
                }).ToList()
            };

            return Ok(groupDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving group with ID {GroupId}", id);
            return StatusCode(500, "An error occurred while retrieving the group");
        }
    }
}
