using AnimeStreamerV2.Models;

namespace AnimePlayerV2.Models
{
    /// <summary>
    /// Represents a category for classifying anime in the application.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the category.
        /// This property is optional and can be null.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of anime associated with this category.
        /// This is a navigation property for the related AnimeModel entities.
        /// </summary>
        public ICollection<AnimeModel> Animes { get; set; }
    }
}
