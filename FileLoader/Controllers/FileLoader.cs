using FileLoader.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileLoader.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileLoaderController : ControllerBase
{
    private readonly ILogger<FileLoaderController> _logger;
    private readonly IBlogStorage _storage;

    public FileLoaderController(ILogger<FileLoaderController> logger, IBlogStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }

    [HttpPost]
    public async Task<IActionResult> PostFiles(IFormFile file, [FromForm] string name)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        await _storage.UploadFile(name, ms.ToArray());
        
        await Task.Delay(1000);

        return Ok(new {filename = name});
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var files = await _storage.GetFileList();
        return Ok(files);
    }

    [HttpDelete("{filename}")]
    public async Task<IActionResult> Delete(string filename)
    {
        await _storage.DeleteFile(filename);

        return NoContent();
    }
}