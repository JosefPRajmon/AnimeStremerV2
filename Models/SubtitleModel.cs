namespace AnimeStreamerV2.Models
{
    /// <summary>
    /// Represents a subtitle model for an anime episode.
    /// </summary>
    public class SubtitleModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the subtitle.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who uploaded the subtitle.
        /// </summary>
        public string UplouderId { get; set; } = "";

        /// <summary>
        /// Gets or sets the identifier of the associated anime episode.
        /// </summary>
        public int AnimeEpisodeModelId { get; set; }

        /// <summary>
        /// Gets or sets the version of the subtitle.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the language of the subtitle.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the file path of the subtitle.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the associated anime episode.
        /// This is a navigation property.
        /// </summary>
        public AnimeEpisodeModel AnimeEpisode { get; set; }
    }
}