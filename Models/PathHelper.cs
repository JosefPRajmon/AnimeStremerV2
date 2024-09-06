namespace AnimeStreamerV2.Models
{
    public static class PathHelper
    {
        public static string GenerateVideoPath(int animeId, int episodeId, string fileExtension)
        {
            return Path.Combine("AnimeVideos", animeId.ToString(), $"{episodeId}.{fileExtension}");
        }

        public static string GenerateSubtitlePath(int animeId, int episodeId, string language)
        {
            return Path.Combine("AnimeVideos", animeId.ToString(), "subtitles", $"{episodeId}_{language}.srt");
        }
    }
}
