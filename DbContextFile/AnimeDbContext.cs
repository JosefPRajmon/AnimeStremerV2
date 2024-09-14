using AnimePlayerV2.Models;
using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test.Models.AdminSystem;

namespace AnimeStreamerV2.DbContextFile
{
    public class AnimeDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AnimeModel> Animes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AnimeEpisodeModel> Episodes { get; set; }
        public DbSet<SubtitleModel> Subtitles { get; set; }
        public DbSet<WatchProgress> WatchProgresses { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public AnimeDbContext(DbContextOptions<AnimeDbContext> options) : base(options)
        {
        }

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
