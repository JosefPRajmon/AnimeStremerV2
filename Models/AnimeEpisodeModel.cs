using System.ComponentModel.DataAnnotations;

namespace AnimeStreamerV2.Models
{
    public class AnimeEpisodeModel
    {

        public int Id { get; set; }
        public int AnimeId { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Season field is required.")]
        public int Season { get; set; }

        [Required(ErrorMessage = "The Episode Number field is required.")]
        public int EpisodeNumber { get; set; }
        public string? VideoPath { get; set; }
        public string? VideoType { get; set; }
        public bool NameAutoCreate { get; set; } = false;
        // Navigační vlastnost pro titulky
        public List<SubtitleModel> Subtitles { get; set; } = new List<SubtitleModel>();
    }
}
