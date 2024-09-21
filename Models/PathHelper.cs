namespace AnimeStreamerV2.Models
{
    /// <summary>
    /// Provides helper methods for generating file paths for anime videos and subtitles.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Generates the file path for an anime video.
        /// </summary>
        /// <param name="animeId">The ID of the anime.</param>
        /// <param name="episodeId">The ID of the episode.</param>
        /// <param name="fileExtension">The file extension of the video.</param>
        /// <returns>The generated file path for the anime video.</returns>
        public static string GenerateVideoPath(int animeId, int episodeId, string fileExtension)
        {
            return Path.Combine("AnimeVideos", animeId.ToString(), $"{episodeId}.{fileExtension}");
        }

        /// <summary>
        /// Generates the file path for an anime subtitle.
        /// </summary>
        /// <param name="animeId">The ID of the anime.</param>
        /// <param name="episodeId">The ID of the episode.</param>
        /// <param name="language">The language of the subtitle.</param>
        /// <returns>The generated file path for the anime subtitle.</returns>
        public static string GenerateSubtitlePath(int animeId, int episodeId, string language)
        {
            return Path.Combine("AnimeVideos", animeId.ToString(), "subtitles", $"{episodeId}_{language}.srt");
        }
    }
}