namespace AnimeStreamerV2.Models
{
    public class SubtitleModel
    {
        public int Id { get; set; }
        public string UplouderId { get; set; } = "";
        public int AnimeEpisodeModelId { get; set; }
        public string Version { get; set; } = "1.0";

        public string Language { get; set; }
        public string Path { get; set; }
        

        // Navigační vlastnost zpět na epizodu
        public AnimeEpisodeModel AnimeEpisode { get; set; }
    }
}
