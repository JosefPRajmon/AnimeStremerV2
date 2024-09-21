using AnimePlayerV2.Models;
using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test.Models.AdminSystem;

namespace AnimeStreamerV2.DbContextFile
{
    /// <summary>
    /// Represents the database context for the anime streaming application.
    /// </summary>
    public class AnimeDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Gets or sets the DbSet of anime models.
        /// </summary>
        public DbSet<AnimeModel> Animes { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of categories.
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of anime episode models.
        /// </summary>
        public DbSet<AnimeEpisodeModel> Episodes { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of subtitle models.
        /// </summary>
        public DbSet<SubtitleModel> Subtitles { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of watch progress records.
        /// </summary>
        public DbSet<WatchProgress> WatchProgresses { get; set; }
        /// <summary>
        /// Gets or sets the DbSet of subscriptions.
        /// </summary>
        public DbSet<Subscription> Subscriptions { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AnimeDbContext(DbContextOptions<AnimeDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types exposed in <see cref="DbSet{TEntity}"/> properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnimeEpisodeModel>()
                .HasMany(e => e.Subtitles)
                .WithOne(s => s.AnimeEpisode)
                .HasForeignKey(s => s.AnimeEpisodeModelId);
        }
    }
}
