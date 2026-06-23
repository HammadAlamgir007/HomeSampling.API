using Microsoft.AspNetCore.Http;

namespace Shared.Infrastructure.Services;

public interface IFileService
{
    Task<string> SaveAsync(IFormFile file, string folder);
    Task<(Stream stream, string contentType)> GetAsync(string relativePath);
    bool Exists(string relativePath);
    void Delete(string relativePath);
}
