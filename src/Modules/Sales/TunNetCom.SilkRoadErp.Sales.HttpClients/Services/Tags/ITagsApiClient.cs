using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tags;

public interface ITagsApiClient
{
    Task<List<TagResponse>> GetAllTagsAsync(CancellationToken cancellationToken = default);

    Task<TagResponse?> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default);

    Task<TagResponse?> UpdateTagAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteTagAsync(int id, CancellationToken cancellationToken = default);

    Task<DocumentTagResponse> GetDocumentTagsAsync(string documentType, int documentId, CancellationToken cancellationToken = default);

    Task<bool> AddTagsToDocumentAsync(string documentType, int documentId, AddTagsToDocumentRequest request, CancellationToken cancellationToken = default);

    Task<bool> RemoveTagsFromDocumentAsync(string documentType, int documentId, RemoveTagsFromDocumentRequest request, CancellationToken cancellationToken = default);

    Task<bool> AddTagsToDocumentByNameAsync(string documentType, int documentId, AddTagsToDocumentByNameRequest request, CancellationToken cancellationToken = default);
}

