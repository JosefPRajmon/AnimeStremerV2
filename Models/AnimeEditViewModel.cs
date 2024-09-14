using AnimeStreamerV2.Models;
using Microsoft.Build.Framework;

namespace AnimePlayerV2.Models
{
    public class AnimeEditViewModel
    {
        [Required]
        public AnimeModel Anime { get; set; }
        public List<Category>? AllCategories { get; set; }
        public List<int>? SelectedCategoryIds { get; set; }
    }
}
