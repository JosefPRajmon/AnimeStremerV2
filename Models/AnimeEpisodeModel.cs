using System.ComponentModel.DataAnnotations;

namespace AnimeStreamerV2.Models
{
    public class AnimeEpisodeModel
    {

        //iddes
        public int Id { get; set; }
        public int AnimeId { get; set; }


        //data
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        //pozicion
        public bool Trailer { get; set; } = false;
        public string? EpisodaIcon { get; set; }

        [Required(ErrorMessage = "The Season field is required.")]
        public int Season { get; set; }

        [Required(ErrorMessage = "The Episode Number field is required.")]
        public int EpisodeNumber { get; set; }

        //video
        public string? VideoPath { get; set; }
        public string? VideoType { get; set; }

        //setings for creators
        public bool NameAutoCreate { get; set; } = false;


        // subtitles
        public List<SubtitleModel> Subtitles { get; set; } = new List<SubtitleModel>();
        public double? WatchProgres { get; set; }
    }
}
