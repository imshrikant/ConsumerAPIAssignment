namespace ConsumerAPIAssignment.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsumerAPIAssignment.Models;
using Microsoft.Extensions.Configuration;

public class ExternalUserService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _apiKey;


    public ExternalUserService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _baseUrl = configuration["ApiSettings:BaseUrl"];
        _apiKey = configuration["ApiSettings:ApiKey"];

    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        _httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
        var response = await _httpClient.GetAsync($"{_baseUrl}users/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching user: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        return userResponse?.Data?.FirstOrDefault();
    }

    public async Task<UserResponse> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        _httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
        var response = await _httpClient.GetAsync($"{_baseUrl}users?page={pageNumber}&per_page={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching users: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        if (userResponse == null || userResponse.Data == null)
        {
            throw new Exception("No data found in the response.");
        }

        return userResponse;
    }
}