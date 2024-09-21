using AnimeStreamerV2.Models;
using test.Models.AdminSystem;

namespace AnimePlayerV2.Models
{
    /// <summary>
    /// Represents the watching progress of a user for a specific anime episode.
    /// </summary>
    public class WatchProgress
    {
        /// <summary>
        /// Gets or sets the unique identifier for the watch progress entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user associated with this watch progress.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the episode associated with this watch progress.
        /// </summary>
        public int EpisodeId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp representing how far the user has watched the episode.
        /// </summary>
        public TimeSpan Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this watch progress.
        /// This is a navigation property.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Gets or sets the anime episode associated with this watch progress.
        /// This is a navigation property.
        /// </summary>
        public AnimeEpisodeModel? Episode { get; set; }
    }
}