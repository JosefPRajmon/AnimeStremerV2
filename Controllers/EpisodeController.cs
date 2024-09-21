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
    [Authorize(Roles = "Admin,ContentCreator,SubtitleCreator")]
    public class EpisodeController : Controller
    {
        private readonly AnimeDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _tempDirectory;
        private readonly ILogger<EpisodeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeController"/> class.
        /// </summary>
        public EpisodeController(AnimeDbContext context, IWebHostEnvironment environment, ILogger<EpisodeController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment=environment;
            _userManager = userManager;
            _tempDirectory = Path.Combine(_environment.WebRootPath, "temp");

            _logger = logger;
        }

        /// <summary>
        /// Displays a list of episodes for a specific anime.
        /// </summary>
        /// <param name="id">The ID of the anime.</param>
        public async Task<IActionResult> Index(int id)
        {

            string userId = (await _userManager.GetUserAsync(User)).Id;
            var episodeModels = _context.Episodes.Where(a => a.AnimeId == id).ToList();
            return View(episodeModels);
        }

        /// <summary>
        /// Displays the form for creating a new episode.
        /// </summary>
        /// <param name="id">The ID of the anime.</param>
        public IActionResult Create(int id)
        {
            List<AnimeEpisodeModel> allepisode = _context.Episodes.Where(a => a.AnimeId == id).ToList();
            int predictEpisode;
            int predictSeason;
            if (allepisode.Count<0)
            {
                predictEpisode = allepisode.MaxBy(a => a.EpisodeNumber).EpisodeNumber;
                predictSeason = allepisode.MaxBy(a => a.Season).Season;
            }
            else
            {
                predictEpisode =0;
                predictSeason = 0;
            }
            ViewData["predictEpisode"]= predictEpisode>1 ? predictEpisode+1 : 1;
            ViewData["predictSeason"] = predictSeason>1 ? predictSeason : 1;
            return View(new AnimeEpisodeModel() { AnimeId = id });
        }

        /// <summary>
        /// Processes the creation of a new episode.
        /// </summary>
        /// <param name="episode">The episode model to create.</param>
        /// <param name="nameAutoCreateString">Indicates whether to auto-create the episode name.</param>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("AnimeId,Name,EpisodeNumber,Season")] AnimeEpisodeModel episode, string nameAutoCreateString)
        {
            if (episode.Name.ToLower().Contains("trayler"))
            {
                episode.Trailer=true;
            }
            if (episode.NameAutoCreate = bool.Parse(nameAutoCreateString))
            {
                AnimeModel animeEpisode = await _context.Animes.Where(a => a.Id==episode.AnimeId).FirstOrDefaultAsync();
                episode.Name = $"{animeEpisode.Name} S:{episode.Season} E:{episode.EpisodeNumber}";
            }
            if (ModelState.IsValid)
            {
                _context.Episodes.Add(episode);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Anime", new { id = episode.AnimeId });
            }
            return RedirectToAction("Details", "Anime", new { id = episode.AnimeId });
        }

        /// <summary>
        /// Displays the form for editing an existing episode.
        /// </summary>
        /// <param name="id">The ID of the episode to edit.</param>
        public async Task<IActionResult> Edit(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }
            string userId = (await _userManager.GetUserAsync(User)).Id;
            episode.Subtitles= await _context.Subtitles
            .Where(s => s.AnimeEpisodeModelId == episode.Id& s.UplouderId ==userId).ToListAsync();

            return View(episode);
        }

        /// <summary>
        /// Processes the editing of an existing episode.
        /// </summary>
        /// <param name="episode">The updated episode model.</param>
        /// <param name="nameAutoCreateString">Indicates whether to auto-create the episode name.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnimeEpisodeModel episode, string nameAutoCreateString)
        {
            AnimeEpisodeModel animeEpisodeModel = await _context.Episodes.FindAsync(episode.Id);
            if (bool.Parse(nameAutoCreateString))
            {
                AnimeModel animeEpisode = await _context.Animes.Where(a => a.Id==animeEpisodeModel.AnimeId).FirstOrDefaultAsync();
                episode.Name = $"{animeEpisode.Name} S:{episode.Season} E:{episode.EpisodeNumber}";
            }
            animeEpisodeModel.Name = episode.Name;
            animeEpisodeModel.EpisodeNumber = episode.EpisodeNumber;
            animeEpisodeModel.Season = episode.Season;
            episode = animeEpisodeModel;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(episode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodeExists(episode.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Anime", new { id = episode.AnimeId });
            }
            return View(episode);
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
        public async Task<IActionResult> AddEditFile(IFormFile chunk, int chunkIndex, int totalChunks, int id, string fileType, string language = null)
        {
            try
            {
                if (chunk == null)
                {
                    return BadRequest("Žádný chunk nebyl přijat.");
                }
                _logger.LogInformation($"Přijat {fileType} chunk: Index={chunkIndex}, TotalChunks={totalChunks}, ID={id}, ChunkSize={chunk.Length}");

                var episode = await _context.Episodes.FindAsync(id);
                if (episode == null)
                {
                    _logger.LogWarning($"Epizoda s ID {id} nebyla nalezena.");
                    return NotFound("Epizoda nebyla nalezena.");
                }

                var tempDirectory = Path.Combine(_environment.WebRootPath, "temp");
                Directory.CreateDirectory(tempDirectory);
                var fileName = $"{id}_{fileType}_{language ?? ""}_{chunk.FileName}_{chunkIndex}";
                var filePath = Path.Combine(tempDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await chunk.CopyToAsync(stream);
                }

                _logger.LogInformation($"{fileType.ToUpperInvariant()} chunk {chunkIndex + 1}/{totalChunks} úspěšně uložen: {filePath}");
                return Json(new { success = true, message = $"{fileType.ToUpperInvariant()} chunk {chunkIndex + 1}/{totalChunks} přijat" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při zpracování {fileType} chunku: {ex.Message}");
                return StatusCode(500, $"Interní chyba serveru: {ex.Message}");
            }
        }


        /// <summary>
        /// Merges uploaded file chunks into a complete file.
        /// </summary>
        /// <param name="request">The merge request containing file details.</param>
        [HttpPost]
        public async Task<IActionResult> MergeFileChunks([FromBody] MergeRequest request)
        {

            var episode = await _context.Episodes.FindAsync(request.EpisodeId);
            if (episode == null)
                return NotFound("Epizoda nebyla nalezena.");

            var fileType = request.FileType.ToLower();
            var directory = Path.Combine(_environment.WebRootPath, "anime", episode.Id.ToString(), fileType == "video" ? "" : "subtitle");
            Directory.CreateDirectory(directory);
            var outputPath = Path.Combine(directory, request.FileName);

            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                for (int i = 0; i < request.TotalChunks; i++)
                {
                    var chunkPath = Path.Combine(_tempDirectory, $"{request.EpisodeId}_{fileType}_{request.Language ?? ""}_{request.FileName}_{i}");
                    using (var inputStream = new FileStream(chunkPath, FileMode.Open))
                    {
                        await inputStream.CopyToAsync(outputStream);
                    }
                    System.IO.File.Delete(chunkPath);
                }
            }

            if (fileType == "video")
            {
                string fileTypeSave = Path.GetExtension(request.FileName);
                int index = outputPath.IndexOf(fileTypeSave);
                string cleanPath = (index < 0)
                    ? outputPath
                    : outputPath.Remove(index, fileTypeSave.Length);
                episode.VideoType =fileTypeSave;
                episode.VideoPath = cleanPath;

            }
            else if (fileType == "subtitle")
            {
                string subId = request.SubId;
                SubtitleModel subtitle = new SubtitleModel();

                if (subId!="null")
                {
                    subtitle =await _context.Subtitles.FirstOrDefaultAsync(s => s.Id == (subId!=null ? int.Parse(subId) : null));
                }
                else
                {
                    request.Version = "1.0";
                }

                subtitle.AnimeEpisodeModelId = episode.Id;
                subtitle.Language = request.Language;
                subtitle.Path = outputPath;
                subtitle.UplouderId = (await _userManager.GetUserAsync(User)).Id;
                subtitle.Version =request.Version;

                episode.Subtitles.Add(subtitle);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"{fileType.ToUpperInvariant()} úspěšně nahráno a spojeno" });
        }

        /// <summary>
        /// Displays the confirmation page for deleting an episode.
        /// </summary>
        /// <param name="id">The ID of the episode to delete.</param>
        public async Task<IActionResult> Delete(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        /// <summary>
        /// Processes the deletion of an episode.
        /// </summary>
        /// <param name="id">The ID of the episode to delete.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                _context.Episodes.Remove(episode);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Anime", new { id = episode.AnimeId });
        }

        /// <summary>
        /// Checks if an episode with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the episode to check.</param>
        /// <returns>True if the episode exists, otherwise false.</returns>
        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.Id == id);
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