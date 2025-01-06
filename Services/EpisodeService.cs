using AnimeStreamerV2.Controllers;
using AnimeStreamerV2.DbContextFile;
using AnimeStreamerV2.Models;
using Microsoft.EntityFrameworkCore;


namespace AnimePlayerV2.Services
{
    public class EpisodeService
    {

        private readonly AnimeDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _tempDirectory;
        public AnimeService _animeService { get; set; }

        public EpisodeService( AnimeDbContext context, IWebHostEnvironment environment )
        {

            _context = context;
            _environment = environment;
            _tempDirectory = Path.Combine( _environment.WebRootPath, "temp" );

            _animeService = new AnimeService( context, environment );
        }

        public async Task<List<AnimeEpisodeModel>> GetEpisodesByIDAsync( int id, string includes = "" )
        {
            switch( includes )
            {
                case "subtitles":
                    return _context.Episodes.Where( a => a.Id == id ).Include( epi => epi.Subtitles ).ToList();
                default:
                    return _context.Episodes.Where( a => a.Id == id ).ToList();
            }

        }
        public async Task<AnimeEpisodeModel?> GetEpisodeByIDAsync( int id, string includes = "" )
        {

            return ( await GetEpisodesByIDAsync( id, includes ) ).FirstOrDefault();

        }
        public async Task InsertNewEpisodeAsync( AnimeEpisodeModel episode )
        {
            _context.Episodes.Add( episode );
            await _context.SaveChangesAsync();
        }
        public async Task<Exception> UpdateEpisodeAsync( AnimeEpisodeModel episode )
        {
            try
            {
                _context.Episodes.Add( episode );
                await _context.SaveChangesAsync();
                return null;
            }
            catch( Exception e )
            {
                return e;
            }

        }
        public async Task DeleteEpisodeAsync( AnimeEpisodeModel episode )
        {
            _context.Episodes.Remove( episode );
            await _context.SaveChangesAsync();
        }
        public async Task SaveFileChunkAsync( IFormFile chunk, int chunkIndex, int totalChunks, int id, string fileType, string language = null )
        {
            var tempDirectory = Path.Combine(_environment.WebRootPath, "temp");
            Directory.CreateDirectory( tempDirectory );
            var fileName = $"{id}_{fileType}_{language ?? ""}_{chunk.FileName}_{chunkIndex}";
            var filePath = Path.Combine(tempDirectory, fileName);

            using( var stream = new FileStream( filePath, FileMode.Create ) )
            {
                await chunk.CopyToAsync( stream );
            }
        }
        public async Task<string> SaveFileFromChunkAsync( MergeRequest request, AnimeEpisodeModel episode, string userId )
        {
            string fileType = request.FileType.ToLower();
            string directory = Path.Combine(_environment.WebRootPath, "anime", episode.Id.ToString(), fileType == "video" ? "" : "subtitle");
            Directory.CreateDirectory( directory );
            string outputPath = Path.Combine(directory, request.FileName);

            using( var outputStream = new FileStream( outputPath, FileMode.Create ) )
            {
                for( int i = 0 ; i < request.TotalChunks ; i++ )
                {
                    var chunkPath = Path.Combine(_tempDirectory, $"{request.EpisodeId}_{fileType}_{request.Language ?? ""}_{request.FileName}_{i}");
                    using( var inputStream = new FileStream( chunkPath, FileMode.Open ) )
                    {
                        await inputStream.CopyToAsync( outputStream );
                    }
                    System.IO.File.Delete( chunkPath );
                }
            }

            if( fileType == "video" )
            {
                string fileTypeSave = Path.GetExtension(request.FileName);
                int index = outputPath.IndexOf(fileTypeSave);
                string cleanPath = (index < 0)
                    ? outputPath
                    : outputPath.Remove(index, fileTypeSave.Length);
                episode.VideoType = fileTypeSave;
                episode.VideoPath = cleanPath;

            }
            else if( fileType == "subtitle" )
            {
                string subId = request.SubId;
                SubtitleModel subtitle = new SubtitleModel();

                if( subId != "null" )
                {
                    subtitle = await _context.Subtitles.FirstOrDefaultAsync( s => s.Id == ( subId != null ? int.Parse( subId ) : null ) );
                }
                else
                {
                    request.Version = "1.0";
                }

                subtitle.AnimeEpisodeModelId = episode.Id;
                subtitle.Language = request.Language;
                subtitle.Path = outputPath;
                subtitle.UplouderId = userId;
                subtitle.Version = request.Version;

                episode.Subtitles.Add( subtitle );
            }

            await _context.SaveChangesAsync();
            return fileType;
        }
        public async Task<AnimeEpisodeModel> AutoCreateNameAsync( string nameAutoCreateString, AnimeEpisodeModel episode )
        {
            if( episode.NameAutoCreate = bool.Parse( nameAutoCreateString ) )
            {
                AnimeModel animeEpisode = await _animeService.GetAnimeAsync(episode.AnimeId,"subtitles");//await _context.Animes.Where(a => a.Id==episode.AnimeId).FirstOrDefaultAsync();
                episode.Name = $"{animeEpisode.Name} S:{episode.Season} E:{episode.EpisodeNumber}";
            }
            return episode;
        }
    }
}
