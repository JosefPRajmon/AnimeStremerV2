using AnimeStreamerV2.Models;
using test.Models.AdminSystem;

namespace AnimePlayerV2.Models
{
    public class WatchProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EpisodeId { get; set; }
        public TimeSpan Timestamp { get; set; }

        public User? User { get; set; }
        public AnimeEpisodeModel? Episode { get; set; }
    }
}
