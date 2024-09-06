using System.ComponentModel.DataAnnotations;

namespace AnimeStreamerV2.Models
{
    public class AnimeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }
        public string? IconPath { get; set; }

        public List<AnimeEpisodeModel> Episodes { get; set; } = new List<AnimeEpisodeModel>();
    }
}