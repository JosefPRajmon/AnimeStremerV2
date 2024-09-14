using AnimeStreamerV2.Models;

namespace AnimePlayerV2.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<AnimeModel> Animes { get; set; }
    }
}
