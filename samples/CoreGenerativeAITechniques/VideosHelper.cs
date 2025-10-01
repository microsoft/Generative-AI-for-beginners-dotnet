/// <summary>
/// Helper class for working with videos.
/// </summary>
public static class VideosHelper
{
    /// <summary>
    /// Gets the file path for the racoon video.
    /// </summary>
    /// <returns>The file path of the racoon video.</returns>
    public static string GetVideoFilePathRacoon()
    {
        var videoFileName = $"racoon.mp4";
        return GetVideoFilePath(videoFileName);
    }

    /// <summary>
    /// Gets the file path for the firetruck video.
    /// </summary>
    /// <returns>The file path of the firetruck video.</returns>
    public static string GetVideoFilePathFireTruck()
    {
        var videoFileName = $"firetruck.mp4";
        return GetVideoFilePath(videoFileName);
    }

    /// <summary>
    /// Gets the file path for the car video.
    /// </summary>
    /// <returns>The file path of the car video.</returns>
    public static string GetVideoFilePathCar()
    {
        var videoFileName = $"insurance_v3.mp4";
        return GetVideoFilePath(videoFileName);
    }

    /// <summary>
    /// Gets the file path for the specified video file.
    /// </summary>
    /// <param name="videoFileName">The name of the video file.</param>
    /// <returns>The file path of the specified video file.</returns>
    public static string GetVideoFilePath(string videoFileName)
    {
        string videosFolder = FindVideosFolder(Directory.GetCurrentDirectory());
        string videoFile = Path.Combine(videosFolder, videoFileName);
        Console.WriteLine($"Video File: {videoFile}");
        return videoFile;
    }

    /// <summary>
    /// Creates the data folder.
    /// </summary>
    /// <returns>The path of the created data folder.</returns>
    public static string CreateDataFolder()
    {
        // Create or clear the "data" folder and the "data/frames" folder
        string dataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
        if (Directory.Exists(dataFolderPath))
        {
            Directory.Delete(dataFolderPath, true);
        }
        Directory.CreateDirectory(dataFolderPath);
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "data/frames"));
        Console.WriteLine($"Data Folder: {dataFolderPath}");
        return dataFolderPath;
    }

    /// <summary>
    /// Finds the videos folder starting from the specified directory.
    /// </summary>
    /// <param name="startDirectory">The starting directory.</param>
    /// <returns>The path of the videos folder.</returns>
    static string FindVideosFolder(string startDirectory)
    {
        var currentDirectory = startDirectory;

        while (true)
        {
            var potentialVideos = Path.Combine(currentDirectory, "videos");
            if (Directory.Exists(potentialVideos))
            {
                return potentialVideos;
            }

            var parentDirectory = Directory.GetParent(currentDirectory);
            if (parentDirectory == null)
            {
                throw new DirectoryNotFoundException("The 'videos' folder was not found in any parent directory.");
            }

            currentDirectory = parentDirectory.FullName;
        }
    }
}
