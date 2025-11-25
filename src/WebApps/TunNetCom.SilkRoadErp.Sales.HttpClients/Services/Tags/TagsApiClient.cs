using System.Net;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tags;

public class TagsApiClient : ITagsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TagsApiClient> _logger;

    public TagsApiClient(HttpClient httpClient, ILogger<TagsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<TagResponse>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/tags", cancellationToken: cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TagResponse>>(cancellationToken: cancellationToken) ?? new List<TagResponse>();
            }
            throw new Exception($"GetAllTags: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tags");
            throw;
        }
    }

    public async Task<TagResponse?> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/tags", request, cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return await response.Content.ReadFromJsonAsync<TagResponse>(cancellationToken: cancellationToken);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogWarning("Bad request when creating tag");
                return null;
            }
            throw new Exception($"CreateTag: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag");
            throw;
        }
    }

    public async Task<TagResponse?> UpdateTagAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/tags/{id}", request, cancellationToken: cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TagResponse>(cancellationToken: cancellationToken);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogWarning("Bad request when updating tag {TagId}", id);
                return null;
            }
            throw new Exception($"UpdateTag: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tag {TagId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTagAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/tags/{id}", cancellationToken: cancellationToken);
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag {TagId}", id);
            throw;
        }
    }

    public async Task<DocumentTagResponse> GetDocumentTagsAsync(string documentType, int documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/tags/{documentType}/{documentId}", cancellationToken: cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DocumentTagResponse>(cancellationToken: cancellationToken) ?? new DocumentTagResponse();
            }
            throw new Exception($"GetDocumentTags: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document tags for {DocumentType} {DocumentId}", documentType, documentId);
            throw;
        }
    }

    public async Task<bool> AddTagsToDocumentAsync(string documentType, int documentId, AddTagsToDocumentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/tags/{documentType}/{documentId}", request, cancellationToken: cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tags to document {DocumentType} {DocumentId}", documentType, documentId);
            throw;
        }
    }

    public async Task<bool> RemoveTagsFromDocumentAsync(string documentType, int documentId, RemoveTagsFromDocumentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/tags/{documentType}/{documentId}/remove", request, cancellationToken: cancellationToken);
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tags from document {DocumentType} {DocumentId}", documentType, documentId);
            throw;
        }
    }

    public async Task<bool> AddTagsToDocumentByNameAsync(string documentType, int documentId, AddTagsToDocumentByNameRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/tags/{documentType}/{documentId}/add-by-name", request, cancellationToken: cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tags by name to document {DocumentType} {DocumentId}", documentType, documentId);
            throw;
        }
    }
}

