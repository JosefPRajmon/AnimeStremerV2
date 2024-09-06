using AnimeStreamerV2.DbContextFile;
using AnimeStreamerV2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimeStreamerV2.Controllers
{
    public class AnimeController : Controller
    {
        private readonly AnimeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AnimeController(AnimeDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment=environment;
        }

        public async Task<IActionResult> Index()
        {
            var animes = await _context.Animes.ToListAsync();
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

            if (anime == null)
            {
                return NotFound();
            }

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

        [Authorize(Roles = "Admin,SubtitleCreator,ContentCreator")]
        public async Task<IActionResult> Edit(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
            {
                return NotFound();
            }
            return View(anime);
        }
        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,SubtitleCreator,ContentCreator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] AnimeModel anime, IFormFile? AnimeIcon)
        {
            if (id != anime.Id)
            {
                return NotFound();
            }
            if (AnimeIcon!=null)
            {
                await AddIcon(anime, AnimeIcon);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimeExists(anime.Id))
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
            return View(anime);
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