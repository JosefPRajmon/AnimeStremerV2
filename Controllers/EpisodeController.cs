using AnimePlayerV2.Services;
using AnimeStreamerV2.DbContextFile;
using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models.AdminSystem;

namespace AnimeStreamerV2.Controllers
{
    /// <summary>
    /// Controller for managing anime episodes.
    /// </summary>
    [Authorize( Roles = "Admin,ContentCreator,SubtitleCreator" )]
    public class EpisodeController : Controller
    {
        private readonly AnimeDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EpisodeService _episodeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeController"/> class.
        /// </summary>
        public EpisodeController( AnimeDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager )
        {
            _userManager = userManager;
            _episodeService = new EpisodeService( context, environment );
            _context = context;
        }

        /// <summary>
        /// Displays a list of episodes for a specific anime.
        /// </summary>
        /// <param name="id">The ID of the anime.</param>
        public async Task<IActionResult> Index( int id )
        {

            string userId = (await _userManager.GetUserAsync(User)).Id;
            List<AnimeEpisodeModel> episodeModels =await _episodeService.GetEpisodesByIDAsync(id);
            return View( episodeModels );
        }

        /// <summary>
        /// Displays the form for creating a new episode.
        /// </summary>
        /// <param name="id">The ID of the anime.</param>
        public async Task<IActionResult> Create( int id )
        {
            List<AnimeEpisodeModel> allepisode = await _episodeService.GetEpisodesByIDAsync(id);
            int predictEpisode;
            int predictSeason;
            if( allepisode.Count > 0 )
            {
                predictEpisode = allepisode.MaxBy( a => a.EpisodeNumber ).EpisodeNumber;
                predictSeason = allepisode.MaxBy( a => a.Season ).Season;
            }
            else
            {
                predictEpisode = 0;
                predictSeason = 0;
            }
            ViewData["predictEpisode"] = predictEpisode > 0 ? predictEpisode + 1 : 1;
            ViewData["predictSeason"] = predictSeason > 0 ? predictSeason : 1;
            return View( new AnimeEpisodeModel() { AnimeId = id } );
        }

        /// <summary>
        /// Processes the creation of a new episode.
        /// </summary>
        /// <param name="episode">The episode model to create.</param>
        /// <param name="nameAutoCreateString">Indicates whether to auto-create the episode name.</param>
        [HttpPost]
        public async Task<IActionResult> Create( [Bind( "AnimeId,Name,EpisodeNumber,Season" )] AnimeEpisodeModel episode, string nameAutoCreateString )
        {
            if( episode.Name.ToLower().Contains( "trayler" ) )
            {
                episode.Trailer = true;
            }
            episode = await _episodeService.AutoCreateNameAsync( nameAutoCreateString, episode );
            if( ModelState.IsValid )
            {
                await _episodeService.InsertNewEpisodeAsync( episode );
                return RedirectToAction( "Details", "Anime", new { id = episode.AnimeId } );
            }
            return RedirectToAction( "Details", "Anime", new { id = episode.AnimeId } );
        }

        /// <summary>
        /// Displays the form for editing an existing episode.
        /// </summary>
        /// <param name="id">The ID of the episode to edit.</param>
        public async Task<IActionResult> Edit( int id )
        {
            var user = await _userManager.GetUserAsync(User);
            if( user.Country is null )
            {
                return Redirect( "/Identity/Account/Manage/CreatorSettings" );
            }

            AnimeEpisodeModel? episode = await _episodeService.GetEpisodeByIDAsync( id,"subtitles" );
            if( episode == null )
            {
                return NotFound();
            }
            string userId = user.Id;

            return View( episode );
        }

        /// <summary>
        /// Processes the editing of an existing episode.
        /// </summary>
        /// <param name="episode">The updated episode model.</param>
        /// <param name="nameAutoCreateString">Indicates whether to auto-create the episode name.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( AnimeEpisodeModel episode, string nameAutoCreateString )
        {
            AnimeEpisodeModel animeEpisodeModel = await _episodeService.GetEpisodeByIDAsync(episode.Id);
            animeEpisodeModel = await _episodeService.AutoCreateNameAsync( nameAutoCreateString, animeEpisodeModel );
            animeEpisodeModel.Name = episode.Name;
            animeEpisodeModel.EpisodeNumber = episode.EpisodeNumber;
            animeEpisodeModel.Season = episode.Season;
            episode = animeEpisodeModel;
            if( ModelState.IsValid )
            {
                try
                {
                    Exception exeption = await _episodeService.UpdateEpisodeAsync( episode );
                    if( exeption != null )
                    {
                        throw exeption;
                    }
                }
                catch( DbUpdateConcurrencyException )
                {
                    if( !EpisodeExists( episode.Id ) )
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction( "Details", "Anime", new { id = episode.AnimeId } );
            }
            return View( episode );
        }

        /// <summary>
        /// Handles the upload of file chunks for episodes.
        /// </summary>
        /// <param name="chunk">The file chunk being uploaded.</param>
        /// <param name="chunkIndex">The index of the current chunk.</param>
        /// <param name="totalChunks">The total number of chunks for the file.</param>
        /// <param name="id">The ID of the episode.</param>
        /// <param name="fileType">The type of file being uploaded (video or subtitle).</param>
        /// <param name="language">The language of the subtitle (optional).</param>
        [HttpPost]
        public async Task<IActionResult> AddEditFile( IFormFile chunk, int chunkIndex, int totalChunks, int id, string fileType, string language = null )
        {
            try
            {
                if( chunk == null )
                {
                    return BadRequest( "Žádný chunk nebyl přijat." );
                }

                var episode = _episodeService.GetEpisodeByIDAsync(id);
                if( episode == null )
                {
                    return NotFound( "Epizoda nebyla nalezena." );
                }

                await _episodeService.SaveFileChunkAsync( chunk, chunkIndex, totalChunks, id, fileType, language );

                return Json( new { success = true, message = $"{fileType.ToUpperInvariant()} chunk {chunkIndex + 1}/{totalChunks} přijat" } );
            }
            catch( Exception ex )
            {
                return StatusCode( 500, $"Interní chyba serveru: {ex.Message}" );
            }
        }


        /// <summary>
        /// Merges uploaded file chunks into a complete file.
        /// </summary>
        /// <param name="request">The merge request containing file details.</param>
        [HttpPost]
        public async Task<IActionResult> MergeFileChunks( [FromBody] MergeRequest request )
        {

            var episode = await _episodeService.GetEpisodeByIDAsync(request.EpisodeId);
            if( episode == null )
                return NotFound( "Epizoda nebyla nalezena." );

            string fileType= await _episodeService.SaveFileFromChunkAsync(request,episode, ( await _userManager.GetUserAsync( User ) ).Id );
            return Json( new { success = true, message = $"{fileType.ToUpperInvariant()} úspěšně nahráno a spojeno" } );
        }

        /// <summary>
        /// Displays the confirmation page for deleting an episode.
        /// </summary>
        /// <param name="id">The ID of the episode to delete.</param>
        public async Task<IActionResult> Delete( int id )
        {
            AnimeEpisodeModel episode = await _episodeService.GetEpisodeByIDAsync(id);
            if( episode == null )
            {
                return NotFound();
            }

            return View( episode );
        }

        /// <summary>
        /// Processes the deletion of an episode.
        /// </summary>
        /// <param name="id">The ID of the episode to delete.</param>
        [HttpPost, ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( int id )
        {
            AnimeEpisodeModel episode = await _episodeService.GetEpisodeByIDAsync(id);
            if( episode != null )
            {
                await _episodeService.DeleteEpisodeAsync( episode );
            }

            return RedirectToAction( "Details", "Anime", new { id = episode.AnimeId } );
        }

        /// <summary>
        /// Checks if an episode with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the episode to check.</param>
        /// <returns>True if the episode exists, otherwise false.</returns>
        private bool EpisodeExists( int id )
        {
            return _context.Episodes.Any( e => e.Id == id );
        }
    }
    /// <summary>
    /// Represents a request to merge file chunks.
    /// </summary>
    public class MergeRequest
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the ID of the episode.
        /// </summary>
        public int EpisodeId { get; set; }
        /// <summary>
        /// Gets or sets the total number of chunks.
        /// </summary>
        public int TotalChunks { get; set; }
        /// <summary>
        /// Gets or sets the type of file (video or subtitle).
        /// </summary>
        public string FileType { get; set; } // "video" nebo "subtitle"
        /// <summary>
        /// Gets or sets the language of the subtitle.
        /// </summary>
        public string Language { get; set; } // Pro titulky
        /// <summary>
        /// Gets or sets the ID of the subtitle.
        /// </summary>
        public string SubId { get; set; }
        /// <summary>
        /// Gets or sets the version of the subtitle.
        /// </summary>
        public string Version { get; set; }
    }
}