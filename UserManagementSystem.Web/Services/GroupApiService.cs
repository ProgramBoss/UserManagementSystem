using System.Net.Http.Json;
using UserManagementSystem.Web.Models;

namespace UserManagementSystem.Web.Services;

public interface IGroupApiService
{
    Task<List<GroupDto>> GetAllGroupsAsync();
    Task<GroupDto?> GetGroupByIdAsync(int id);
}

public class GroupApiService : IGroupApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GroupApiService> _logger;

    public GroupApiService(HttpClient httpClient, ILogger<GroupApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<GroupDto>> GetAllGroupsAsync()
    {
        try
        {
            var groups = await _httpClient.GetFromJsonAsync<List<GroupDto>>("api/groups");
            return groups ?? new List<GroupDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching groups");
            return new List<GroupDto>();
        }
    }

    public async Task<GroupDto?> GetGroupByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GroupDto>($"api/groups/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching group {GroupId}", id);
            return null;
        }
    }
}
