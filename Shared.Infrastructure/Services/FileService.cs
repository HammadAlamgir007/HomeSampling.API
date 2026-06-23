using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Shared.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _basePath;

    public FileService(IWebHostEnvironment env)
    {
        _basePath = env.ContentRootPath;
    }

    public async Task<string> SaveAsync(IFormFile file, string folder)
    {
        var dir = Path.Combine(_basePath, "uploads", folder);
        Directory.CreateDirectory(dir);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(dir, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"uploads/{folder}/{fileName}";
    }

    public async Task<(Stream stream, string contentType)> GetAsync(string relativePath)
    {
        var fullPath = Path.Combine(_basePath, relativePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found.", relativePath);

        var extension = Path.GetExtension(fullPath).ToLower();
        var contentType = extension switch
        {
            ".pdf"  => "application/pdf",
            ".jpg"  => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png"  => "image/png",
            _       => "application/octet-stream"
        };

        var stream = File.OpenRead(fullPath);
        return await Task.FromResult((stream, contentType));
    }

    public bool Exists(string relativePath)
    {
        var fullPath = Path.Combine(_basePath, relativePath);
        return File.Exists(fullPath);
    }

    public void Delete(string relativePath)
    {
        var fullPath = Path.Combine(_basePath, relativePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
