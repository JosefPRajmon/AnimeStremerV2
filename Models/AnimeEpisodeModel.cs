using System.ComponentModel.DataAnnotations;

namespace AnimeStreamerV2.Models
{
    /// <summary>
    /// Represents an episode of an anime series.
    /// </summary>
    public class AnimeEpisodeModel
    {
        #region Identification
        /// <summary>
        /// Gets the unique identifier for the episode.
        /// This property is read-only and should be set by the database or ORM.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets or sets the identifier of the anime series this episode belongs to.
        /// </summary>
        public int AnimeId { get; set; }
        #endregion

        #region Basic Information
        /// <summary>
        /// Gets or sets the name of the episode.
        /// </summary>
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the episode.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the release date of the episode.
        /// </summary>
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        #endregion

        #region Position and Metadata
        /// <summary>
        /// Gets or sets a value indicating whether this episode is a trailer.
        /// </summary>
        public bool Trailer { get; set; } = false;

        /// <summary>
        /// Gets or sets the icon for the episode.
        /// </summary>
        public string? EpisodeIcon { get; set; }

        /// <summary>
        /// Gets or sets the season number of the episode.
        /// </summary>
        [Required(ErrorMessage = "The Season field is required.")]
        public int Season { get; set; }

        /// <summary>
        /// Gets or sets the episode number within its season.
        /// </summary>
        [Required(ErrorMessage = "The Episode Number field is required.")]
        public int EpisodeNumber { get; set; }
        #endregion

        #region Video Information
        /// <summary>
        /// Gets or sets the file path of the video for this episode.
        /// </summary>
        public string? VideoPath { get; set; }

        /// <summary>
        /// Gets or sets the video type or format.
        /// </summary>
        public string? VideoType { get; set; }
        #endregion

        #region Creator Settings
        /// <summary>
        /// Gets or sets a value indicating whether the episode name should be automatically created.
        /// </summary>
        public bool NameAutoCreate { get; set; } = false;
        #endregion

        #region Subtitles and Watching Progress
        /// <summary>
        /// Gets or sets the list of subtitles available for this episode.
        /// </summary>
        public List<SubtitleModel> Subtitles { get; set; } = new List<SubtitleModel>();

        /// <summary>
        /// Gets or sets the watching progress of the episode, represented as a percentage.
        /// </summary>
        public double? WatchProgress { get; set; }
        #endregion
    }
}
