using AnimePlayerV2.Models;
using AnimeStreamerV2.DbContextFile;
using AnimeStreamerV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimePlayerV2.Services
{
    public class AnimeService
    {
        private readonly AnimeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AnimeService( AnimeDbContext context, IWebHostEnvironment environment )
        {
            _context = context;
            _environment = environment;
        }
        public async Task<List<AnimeModel>> GetAnimesFromUserAsync( string UserId = "null" )
        {
            if( UserId != "null" )
            {
                return await _context.Animes.Where( anime => anime.CreaterId == UserId ).ToListAsync();
            }
            else
            {
                return await _context.Animes.Include( a => a.Categories ).ToListAsync();
            }
        }

        public async Task<List<AnimeModel>> AddTimeSpendToEpisodesAsync( List<AnimeModel> animes, string userId )
        {
            foreach( AnimeModel anime in animes )
            {
                anime.Episodes = _context.Episodes.Where( episode => episode.AnimeId == anime.Id ).ToList();
                foreach( var episode in anime.Episodes )
                {
                    var progress = await _context.WatchProgresses
    .Where(wp => wp.UserId == userId && wp.EpisodeId == episode.Id)
    .Select(wp => wp.Timestamp.TotalSeconds)
    .FirstOrDefaultAsync();
                    episode.WatchProgress = progress;
                }
            }
            return animes;
        }
        public async Task<AnimeModel> AddTimeSpendToEpisodeAsnc( AnimeModel anime, string userId )
        {
            List<AnimeModel> AnimeList = await AddTimeSpendToEpisodesAsync(new List<AnimeModel>() { anime }, userId);
            return AnimeList.First();
        }

        public async Task<AnimeModel?> GetAnimeAsync( int? animeId, string include = "" )
        {
            switch( include )
            {
                case "episodes":
                    return await _context.Animes
                        .Include( a => a.Episodes )
                        .FirstOrDefaultAsync( m => m.Id == animeId );
                case "category":
                    return await _context.Animes
                        .Include( a => a.Categories )
                        .FirstOrDefaultAsync( m => m.Id == animeId );
                case "categoryAndEpisode":
                    return await _context.Animes
                        .Include( a => a.Categories )
                        .Include( a => a.Episodes )
                        .FirstOrDefaultAsync( m => m.Id == animeId );
                default:
                    return await _context.Animes
                        .FirstOrDefaultAsync( m => m.Id == animeId );
            }
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Boolean> CreateNewAnimeAsync( AnimeModel anime, IFormFile AnimeIcon )
        {
            _context.Add( anime );
            await _context.SaveChangesAsync();

            await AddIcon( anime, AnimeIcon );
            _context.Update( anime );
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Adds an icon to the specified anime.
        /// </summary>
        /// <param name="anime">The anime model.</param>
        /// <param name="AnimeIcon">The icon file to add.</param>
        public async Task AddIcon( AnimeModel anime, IFormFile AnimeIcon )
        {
            var tempDirectory = Path.Combine(_environment.WebRootPath, "anime", $"{anime.Id}");
            Directory.CreateDirectory( tempDirectory );
            var filePath = Path.Combine(tempDirectory, AnimeIcon.FileName);

            using( var stream = new FileStream( filePath, FileMode.Create ) )
            {
                await AnimeIcon.CopyToAsync( stream );
            }
            anime.IconPath = Path.Combine( "anime", $"{anime.Id}", AnimeIcon.FileName );
        }
        public async void UpdateAnimeAsync( AnimeEditViewModel viewModel, AnimeModel? animeToUpdate, List<int> SelectedCategoryIds )
        {
            // Update basic properties
            animeToUpdate.Name = viewModel.Anime.Name;
            animeToUpdate.Description = viewModel.Anime.Description;
            animeToUpdate.Rating = viewModel.Anime.Rating;

            // Update categories
            animeToUpdate.Categories.Clear();
            if( SelectedCategoryIds != null )
            {
                var selectedCategories = await _context.Categories
                            .Where(c => SelectedCategoryIds.Contains(c.Id))
                            .ToListAsync();
                animeToUpdate.Categories = selectedCategories;
            }

            _context.Update( animeToUpdate );
            await _context.SaveChangesAsync();
        }
        public async void RemoveAnimeAsync( AnimeModel anime )
        {
            _context.Animes.Remove( anime );
            await _context.SaveChangesAsync();
        }
    }
}
