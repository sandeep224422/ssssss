using Microsoft.AspNetCore.Mvc;
using YouTubeDownloader.Models;
using YouTubeDownloader.Services;
using System.ComponentModel.DataAnnotations;

namespace YouTubeDownloader.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelegramBotController : ControllerBase
{
    private readonly IYouTubeService _youTubeService;
    private readonly ILogger<TelegramBotController> _logger;

    public TelegramBotController(IYouTubeService youTubeService, ILogger<TelegramBotController> logger)
    {
        _youTubeService = youTubeService;
        _logger = logger;
    }

    /// <summary>
    /// Process YouTube link and return file information
    /// </summary>
    [HttpPost("process-youtube")]
    public async Task<IActionResult> ProcessYouTubeLink([FromBody] YouTubeLinkRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.YouTubeUrl))
            {
                return BadRequest(new { error = "YouTube URL is required" });
            }

            var result = await _youTubeService.ProcessYouTubeLinkAsync(request.YouTubeUrl, request.ConvertToMp3);
            
            return Ok(new
            {
                success = true,
                filePath = result.FilePath,
                fileName = result.FileName,
                title = result.Title,
                artist = result.Artist,
                duration = result.Duration,
                fileSize = result.FileSize,
                downloadUrl = $"/api/telegrambot/download/{result.FileName}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing YouTube link: {Url}", request.YouTubeUrl);
            return StatusCode(500, new { error = "Failed to process YouTube link", details = ex.Message });
        }
    }

    /// <summary>
    /// Search for a song by name and return download information
    /// </summary>
    [HttpPost("search-song")]
    public async Task<IActionResult> SearchSong([FromBody] SongSearchRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.SongName))
            {
                return BadRequest(new { error = "Song name is required" });
            }

            var result = await _youTubeService.SearchAndDownloadSongAsync(request.SongName, request.ConvertToMp3);
            
            return Ok(new
            {
                success = true,
                filePath = result.FilePath,
                fileName = result.FileName,
                title = result.Title,
                artist = result.Artist,
                duration = result.Duration,
                fileSize = result.FileSize,
                downloadUrl = $"/api/telegrambot/download/{result.FileName}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for song: {SongName}", request.SongName);
            return StatusCode(500, new { error = "Failed to search for song", details = ex.Message });
        }
    }

    /// <summary>
    /// Download file by filename
    /// </summary>
    [HttpGet("download/{fileName}")]
    public IActionResult DownloadFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_youTubeService.GetDownloadPath(), fileName);
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { error = "File not found" });
            }

            var fileInfo = new FileInfo(filePath);
            var contentType = GetContentType(filePath);
            
            return PhysicalFile(filePath, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
            return StatusCode(500, new { error = "Failed to download file" });
        }
    }

    /// <summary>
    /// Get file information without downloading
    /// </summary>
    [HttpGet("info/{fileName}")]
    public IActionResult GetFileInfo(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_youTubeService.GetDownloadPath(), fileName);
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { error = "File not found" });
            }

            var fileInfo = new FileInfo(filePath);
            
            return Ok(new
            {
                fileName = fileName,
                fileSize = fileInfo.Length,
                lastModified = fileInfo.LastWriteTime,
                filePath = filePath
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info: {FileName}", fileName);
            return StatusCode(500, new { error = "Failed to get file info" });
        }
    }

    private string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".mp3" => "audio/mpeg",
            ".m4a" => "audio/mp4",
            ".ogg" => "audio/ogg",
            ".wav" => "audio/wav",
            _ => "application/octet-stream"
        };
    }
}

public class YouTubeLinkRequest
{
    [Required]
    public string YouTubeUrl { get; set; } = string.Empty;
    public bool ConvertToMp3 { get; set; } = true;
}

public class SongSearchRequest
{
    [Required]
    public string SongName { get; set; } = string.Empty;
    public bool ConvertToMp3 { get; set; } = true;
}
