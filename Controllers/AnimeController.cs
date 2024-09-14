using AnimePlayerV2.Models;
using AnimePlayerV2.Models.AdminSystem;
using AnimeStreamerV2.DbContextFile;
using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test.Models.AdminSystem;

namespace AnimeStreamerV2.Controllers
{
    public class AnimeController : Controller
    {
        private readonly AnimeDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnimeController(AnimeDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment=environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var countries = Enum.GetValues(typeof(CountryEnum))
    .Cast<CountryEnum>()
    .Select(c => new SelectListItem
    {
        Value = c.ToString(),
        Text = c.ToString()
    }).ToList();

            ViewBag.Countries = countries;
            ViewData["baseUrl"]= $"{Request.Scheme}://{Request.Host}/"/*+{ Request.Host.Port ?? 80}*/;
            ViewData["fun"]=$"{Request.Query["fun"]}";


            string id = Request.RouteValues["id"]!=null ? Request.RouteValues["id"].ToString() : null;

            List<AnimeModel> animes;
            if (id!=null)
            {
                animes= await _context.Animes.Where(anime => anime.CreaterId == id).ToListAsync();
            }
            else
            {
                animes=await _context.Animes.Include(a => a.Categories).ToListAsync();
            }
            if (User.Identity.IsAuthenticated)
            {
                string userId = (await _userManager.GetUserAsync(User)).Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    foreach (AnimeModel anime in animes)
                    {
                        anime.Episodes = _context.Episodes.Where(episode => episode.AnimeId == anime.Id).ToList();
                        foreach (var episode in anime.Episodes)
                        {
                            var progress = await _context.WatchProgresses
            .Where(wp => wp.UserId == userId && wp.EpisodeId == episode.Id)
            .Select(wp => wp.Timestamp.TotalSeconds)
            .FirstOrDefaultAsync();
                            episode.WatchProgres = progress;
                        }
                    }


                }
            }

            return View(animes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anime = await _context.Animes
                .Include(a => a.Episodes)
                .FirstOrDefaultAsync(m => m.Id == id);

            anime.Episodes = _context.Episodes.Where(a => a.AnimeId == id).ToList();
            if (User.Identity.IsAuthenticated)
            {
                string userId = (await _userManager.GetUserAsync(User)).Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    foreach (var episode in anime.Episodes)
                    {
                        var progress = await _context.WatchProgresses
        .Where(wp => wp.UserId == userId && wp.EpisodeId == episode.Id)
        .Select(wp => wp.Timestamp.TotalSeconds)
        .FirstOrDefaultAsync();
                        episode.WatchProgres = progress;
                    }

                }
            }
            if (anime == null)
            {
                return NotFound();
            }
            ViewData["baseUrl"]= $"{Request.Scheme}://{Request.Host}/"/*+{ Request.Host.Port ?? 80}*/;
            return View(anime);
        }
        [Authorize(Roles = "Admin,ContentCreator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ContentCreator")]
        public async Task<IActionResult> Create([Bind("Name,Description")] AnimeModel anime, IFormFile AnimeIcon)
        {
            anime.CountryOfOrigin= (await _userManager.GetUserAsync(User)).Country;
            anime.CreaterId = (await _userManager.GetUserAsync(User)).Id;
            if (ModelState.IsValid)
            {

                _context.Add(anime);
                await _context.SaveChangesAsync();

                await AddIcon(anime, AnimeIcon);
                _context.Update(anime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = anime.Id });
            }


            return View(anime);
        }

        private async Task AddIcon(AnimeModel anime, IFormFile AnimeIcon)
        {
            var tempDirectory = Path.Combine(_environment.WebRootPath, "anime", $"{anime.Id}");
            Directory.CreateDirectory(tempDirectory);
            var filePath = Path.Combine(tempDirectory, AnimeIcon.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await AnimeIcon.CopyToAsync(stream);
            }
            anime.IconPath = Path.Combine("anime", $"{anime.Id}", AnimeIcon.FileName);
        }

        [Authorize(Roles = "Admin,ContentCreator")]
        public async Task<IActionResult> Edit(int id)
        {
            var anime = await _context.Animes
                .Include(a => a.Categories)
                .FirstOrDefaultAsync(an => an.Id == id);

            if (anime == null)
            {
                return NotFound();
            }

            var allCategories = await _context.Categories.ToListAsync();
            var viewModel = new AnimeEditViewModel
            {
                Anime = anime,
                AllCategories = allCategories,
                SelectedCategoryIds = anime.Categories.Select(c => c.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ContentCreator")]
        public async Task<IActionResult> Edit(int id, AnimeEditViewModel viewModel, IFormFile? AnimeIcon, List<int> SelectedCategoryIds)
        {
            if (id != viewModel.Anime.Id)
            {
                return NotFound();
            }

            if (AnimeIcon != null)
            {
                await AddIcon(viewModel.Anime, AnimeIcon);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var animeToUpdate = await _context.Animes
                        .Include(a => a.Categories)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (animeToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update basic properties
                    animeToUpdate.Name = viewModel.Anime.Name;
                    animeToUpdate.Description = viewModel.Anime.Description;
                    animeToUpdate.Rating = viewModel.Anime.Rating;

                    // Update categories
                    animeToUpdate.Categories.Clear();
                    if (SelectedCategoryIds != null)
                    {
                        var selectedCategories = await _context.Categories
                            .Where(c => SelectedCategoryIds.Contains(c.Id))
                            .ToListAsync();
                        animeToUpdate.Categories = selectedCategories;
                    }

                    _context.Update(animeToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimeExists(viewModel.Anime.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay form
            viewModel.AllCategories = await _context.Categories.ToListAsync();
            return View(viewModel);
        }
        [Authorize(Roles = "Admin,ContentCreator")]
        public async Task<IActionResult> Delete(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
            {
                return NotFound();
            }

            return View(anime);
        }
        [Authorize]
        [Authorize(Roles = "Admin,ContentCreator")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool remove)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime != null)
            {
                _context.Animes.Remove(anime);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AnimeExists(int id)
        {
            return _context.Animes.Any(e => e.Id == id);
        }
    }
}