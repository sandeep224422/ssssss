using YouTubeDownloader.Models;

namespace YouTubeDownloader.Services;

public interface IYouTubeService
{
    /// <summary>
    /// Process a YouTube link and download the audio
    /// </summary>
    /// <param name="youtubeUrl">The YouTube URL to process</param>
    /// <param name="convertToMp3">Whether to convert to MP3 format</param>
    /// <returns>Download result with file information</returns>
    Task<DownloadResult> ProcessYouTubeLinkAsync(string youtubeUrl, bool convertToMp3 = true);

    /// <summary>
    /// Search for a song by name and download it
    /// </summary>
    /// <param name="songName">The name of the song to search for</param>
    /// <param name="convertToMp3">Whether to convert to MP3 format</param>
    /// <returns>Download result with file information</returns>
    Task<DownloadResult> SearchAndDownloadSongAsync(string songName, bool convertToMp3 = true);

    /// <summary>
    /// Get the download path for files
    /// </summary>
    /// <returns>The download directory path</returns>
    string GetDownloadPath();
}
