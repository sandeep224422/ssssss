namespace YouTubeDownloader.Models;

public class DownloadResult
{
    /// <summary>
    /// Full file path on the server
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Just the filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Song title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Artist/Uploader name
    /// </summary>
    public string Artist { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration in seconds
    /// </summary>
    public long Duration { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// When the download was completed
    /// </summary>
    public DateTime DownloadDate { get; set; }
    
    /// <summary>
    /// File size formatted for display
    /// </summary>
    public string FormattedFileSize
    {
        get
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = FileSize;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
    
    /// <summary>
    /// Duration formatted for display
    /// </summary>
    public string FormattedDuration
    {
        get
        {
            var timeSpan = TimeSpan.FromSeconds(Duration);
            return timeSpan.TotalHours >= 1 
                ? timeSpan.ToString(@"hh\:mm\:ss") 
                : timeSpan.ToString(@"mm\:ss");
        }
    }
}
