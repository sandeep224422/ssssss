using NYoutubeDL;
using NYoutubeDL.Helpers;
using YouTubeDownloader.Models;
using TagLib;
using System.Text.RegularExpressions;

namespace YouTubeDownloader.Services;

public class YouTubeService : IYouTubeService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<YouTubeService> _logger;
    private readonly string _downloadPath;

    public YouTubeService(IConfiguration configuration, ILogger<YouTubeService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _downloadPath = _configuration["DownloadPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
        
        // Ensure download directory exists
        if (!Directory.Exists(_downloadPath))
        {
            Directory.CreateDirectory(_downloadPath);
        }
    }

    public async Task<DownloadResult> ProcessYouTubeLinkAsync(string youtubeUrl, bool convertToMp3 = true)
    {
        try
        {
            _logger.LogInformation("Processing YouTube URL: {Url}", youtubeUrl);

            // Validate YouTube URL
            if (!IsValidYouTubeUrl(youtubeUrl))
            {
                throw new ArgumentException("Invalid YouTube URL provided");
            }

            // Get video info first
            var videoInfo = await GetVideoInfoAsync(youtubeUrl);
            
            // Create safe filename
            var safeFileName = CreateSafeFileName(videoInfo.Title, videoInfo.Uploader);
            var outputFormat = convertToMp3 ? "mp3" : "m4a";
            var outputFileName = $"{safeFileName}.{outputFormat}";
            var outputPath = Path.Combine(_downloadPath, outputFileName);

            // Check if file already exists
            if (File.Exists(outputPath))
            {
                _logger.LogInformation("File already exists: {FileName}", outputFileName);
                return await CreateDownloadResultAsync(outputPath, videoInfo);
            }

            // Download the audio
            await DownloadAudioAsync(youtubeUrl, outputPath, convertToMp3);

            // Verify file was created
            if (!File.Exists(outputPath))
            {
                throw new Exception("Failed to create output file");
            }

            // Add metadata
            await AddMetadataAsync(outputPath, videoInfo);

            _logger.LogInformation("Successfully processed YouTube URL: {Url} -> {FileName}", youtubeUrl, outputFileName);
            
            return await CreateDownloadResultAsync(outputPath, videoInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing YouTube URL: {Url}", youtubeUrl);
            throw;
        }
    }

    public async Task<DownloadResult> SearchAndDownloadSongAsync(string songName, bool convertToMp3 = true)
    {
        try
        {
            _logger.LogInformation("Searching for song: {SongName}", songName);

            // For now, we'll use a simple search approach
            // In a production environment, you might want to integrate with YouTube Data API
            // or use a more sophisticated search method
            
            // Create a search query
            var searchQuery = $"{songName} music";
            
            // This is a simplified approach - in reality, you'd need to implement
            // actual YouTube search functionality or use the YouTube Data API
            throw new NotImplementedException("YouTube search functionality requires YouTube Data API integration. Please use direct YouTube URLs for now.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for song: {SongName}", songName);
            throw;
        }
    }

    public string GetDownloadPath()
    {
        return _downloadPath;
    }

    private async Task<VideoInfo> GetVideoInfoAsync(string youtubeUrl)
    {
        var youtubeDl = new YoutubeDL();
        youtubeDl.VideoUrl = youtubeUrl;
        
        // Get video info without downloading
        var info = await youtubeDl.GetVideoInfoAsync();
        
        return new VideoInfo
        {
            Title = info.Title ?? "Unknown Title",
            Uploader = info.Uploader ?? "Unknown Artist",
            Duration = info.Duration ?? 0,
            Description = info.Description ?? "",
            UploadDate = info.UploadDate ?? DateTime.Now
        };
    }

    private async Task DownloadAudioAsync(string youtubeUrl, string outputPath, bool convertToMp3)
    {
        var youtubeDl = new YoutubeDL();
        
        // Configure options
        youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
        youtubeDl.Options.PostProcessingOptions.AudioFormat = convertToMp3 ? Enums.AudioFormat.mp3 : Enums.AudioFormat.m4a;
        youtubeDl.Options.PostProcessingOptions.AudioQuality = Enums.AudioQuality.best;
        youtubeDl.Options.PostProcessingOptions.AddMetadata = true;
        youtubeDl.Options.PostProcessingOptions.PreferFfmpeg = true;
        
        // Set output path
        youtubeDl.Options.FilesystemOptions.Output = outputPath;
        
        // Set video URL
        youtubeDl.VideoUrl = youtubeUrl;
        
        // Download
        await youtubeDl.DownloadAsync();
    }

    private async Task AddMetadataAsync(string filePath, VideoInfo videoInfo)
    {
        try
        {
            using var file = TagLib.File.Create(filePath);
            file.Tag.Title = videoInfo.Title;
            file.Tag.Performers = new[] { videoInfo.Uploader };
            file.Tag.AlbumArtists = new[] { videoInfo.Uploader };
            file.Tag.Album = "YouTube Download";
            file.Tag.Year = (uint)videoInfo.UploadDate.Year;
            file.Tag.Comment = "Downloaded via YouTube Music Downloader API";
            file.Save();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add metadata to file: {FilePath}", filePath);
            // Don't throw here as the file is still usable without metadata
        }
    }

    private async Task<DownloadResult> CreateDownloadResultAsync(string filePath, VideoInfo videoInfo)
    {
        var fileInfo = new FileInfo(filePath);
        
        return new DownloadResult
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            Title = videoInfo.Title,
            Artist = videoInfo.Uploader,
            Duration = videoInfo.Duration,
            FileSize = fileInfo.Length,
            DownloadDate = DateTime.Now
        };
    }

    private string CreateSafeFileName(string title, string artist)
    {
        // Remove invalid characters and limit length
        var safeTitle = Regex.Replace(title, @"[<>:""/\\|?*]", "");
        var safeArtist = Regex.Replace(artist, @"[<>:""/\\|?*]", "");
        
        // Limit length to avoid filesystem issues
        safeTitle = safeTitle.Length > 50 ? safeTitle.Substring(0, 50) : safeTitle;
        safeArtist = safeArtist.Length > 30 ? safeArtist.Substring(0, 30) : safeArtist;
        
        return $"{safeArtist} - {safeTitle}".Trim();
    }

    private bool IsValidYouTubeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Basic YouTube URL validation
        return url.Contains("youtube.com") || url.Contains("youtu.be");
    }
}

public class VideoInfo
{
    public string Title { get; set; } = string.Empty;
    public string Uploader { get; set; } = string.Empty;
    public long Duration { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
}
