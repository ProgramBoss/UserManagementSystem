using System.Net.Http.Json;
using UserManagementSystem.Web.Models;

namespace UserManagementSystem.Web.Services;

public interface IUserApiService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> CreateUserAsync(CreateUserDto user);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto user);
    Task<bool> DeleteUserAsync(int id);
    Task<int> GetTotalUserCountAsync();
    Task<List<UserCountByGroupDto>> GetUserCountByGroupAsync();
}

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiService> _logger;

    public UserApiService(HttpClient httpClient, ILogger<UserApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all users from {BaseAddress}api/users", _httpClient.BaseAddress);
            var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users");
            _logger.LogInformation("Successfully fetched {Count} users", users?.Count ?? 0);
            return users ?? new List<UserDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching users. Base address: {BaseAddress}. Error: {Message}", 
                _httpClient.BaseAddress, ex.Message);
            throw new Exception($"Cannot connect to API at {_httpClient.BaseAddress}. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users from {BaseAddress}", _httpClient.BaseAddress);
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching user {UserId}", id);
            return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{id}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching user {UserId}. Error: {Message}", id, ex.Message);
            throw new Exception($"Cannot connect to API. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            return null;
        }
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto user)
    {
        try
        {
            _logger.LogInformation("Creating user {Email}", user.Email);
            var response = await _httpClient.PostAsJsonAsync("api/users", user);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            _logger.LogInformation("Successfully created user with ID {UserId}", result?.Id);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error creating user. Error: {Message}", ex.Message);
            throw new Exception($"Cannot connect to API. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return null;
        }
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto user)
    {
        try
        {
            _logger.LogInformation("Updating user {UserId}", id);
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", user);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            _logger.LogInformation("Successfully updated user {UserId}", id);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error updating user {UserId}. Error: {Message}", id, ex.Message);
            throw new Exception($"Cannot connect to API. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting user {UserId}", id);
            var response = await _httpClient.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error deleting user {UserId}. Error: {Message}", id, ex.Message);
            throw new Exception($"Cannot connect to API. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return false;
        }
    }

    public async Task<int> GetTotalUserCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total user count");
            return await _httpClient.GetFromJsonAsync<int>("api/users/count");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching user count. Error: {Message}", ex.Message);
            throw new Exception($"Cannot connect to API. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user count");
            return 0;
        }
    }

    public async Task<List<UserCountByGroupDto>> GetUserCountByGroupAsync()
    {
        try
        {
            _logger.LogInformation("Fetching user count by group");
            var counts = await _httpClient.GetFromJsonAsync<List<UserCountByGroupDto>>("api/users/count-by-group");
            _logger.LogInformation("Successfully fetched user count for {Count} groups", counts?.Count ?? 0);
            return counts ?? new List<UserCountByGroupDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching user count by group. Error: {Message}", ex.Message);
            throw new Exception($"Cannot connect to API. Please ensure the API is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user count by group");
            return new List<UserCountByGroupDto>();
        }
    }
}
